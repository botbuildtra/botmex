using System;
using System.IO;
using System.Threading;
using Botmex.Strategies;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using static Google.Apis.Gmail.v1.GmailService;

namespace Botmex.Strategies
{
    public class Tradingview: IStrategies
    {
        static string[] Scopes = { GmailService.Scope.GmailModify };
        public static string ApplicationName = "BotMex Trader";
        public static string LabelId, UserMail;
        public static string lastAlert = "";
        public static string buyTrigger = "Buy!";
        public static string sellTrigger = "Sell!";

        public void run()
        {
            MainClass.log("Running TradingView Strategy");
            StaticSetup();
            StaticRun();
        }

        public static void StaticSetup()
        {
            if (MainClass.strategyOptions.Count > 0)
            {
                if (MainClass.strategyOptions.ContainsKey("labelid".ToLower()))
                    LabelId = MainClass.strategyOptions["labelid"];

                if (MainClass.strategyOptions.ContainsKey("usermail".ToLower()))
                    UserMail = MainClass.strategyOptions["usermail"];

                if (MainClass.strategyOptions.ContainsKey("lastalert".ToLower()) && string.IsNullOrEmpty(lastAlert))
                {
                    lastAlert = MainClass.strategyOptions["lastalert"];
                    MainClass.log("Last Alert set to: " + lastAlert + " as per config");   
                }

                if (MainClass.strategyOptions.ContainsKey("buytrigger"))
                    buyTrigger = MainClass.strategyOptions["buytrigger"];

                if (MainClass.strategyOptions.ContainsKey("selltrigger"))
                    sellTrigger = MainClass.strategyOptions["selltrigger"];


            }

            if( string.IsNullOrEmpty(LabelId) || string.IsNullOrEmpty(UserMail))
            {
                MainClass.log("Please check strategy settings");
                System.Environment.Exit(1);
            }
        }


        public static void StaticRun()
        {
            UserCredential credential;

            using (var stream =
              new FileStream("creds.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = MainClass.location + "gcreds";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  GoogleClientSecrets.Load(stream).Secrets,
                  Scopes,
                  "user",
                  CancellationToken.None,
                  new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service. 
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            var inboxlistRequest = service.Users.Messages.List(UserMail);
            inboxlistRequest.LabelIds = LabelId;
            inboxlistRequest.Q = "is:unread";
            inboxlistRequest.IncludeSpamTrash = false;
            //get our emails 
            var emailListResponse = inboxlistRequest.Execute();

            if (emailListResponse != null && emailListResponse.Messages != null)
            {
                //loop through each email and get what fields you want... 
                foreach (var email in emailListResponse.Messages)
                {

                    var emailInfoRequest = service.Users.Messages.Get(UserMail, email.Id);
                    var emailInfoResponse = emailInfoRequest.Execute();

                    MainClass.positionContracts = MainClass.getPosition();
                    if (emailInfoResponse != null)
                    {
                        string mData = emailInfoResponse.Snippet;
                        if(mData.Contains(MainClass.pair) && mData.Contains(sellTrigger) && (MainClass.positionContracts > 0 || MainClass.positionContracts == 0) && lastAlert != "Sell")
                        {
                            MainClass.log(emailInfoResponse.Snippet);
                            MainClass.log("Short: " + MainClass.pair);
                            string json = MainClass.bitMEXApi.MarketOrder(MainClass.pair, "Sell", Math.Abs(MainClass.positionContracts) + MainClass.qtdyContacts, "Tradingview Short");
                            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                            {
                                if (json.ToLower().IndexOf("overload") >= 0 && Math.Abs(MainClass.positionContracts) > 0 )
                                {/* In case of overload, close the position to prevent losses*/
                                    MainClass.log("System is on overload, trying to close position to prevent losses");
                                    MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, "Sell"));
                                }
                                return;
                            }

                            lastAlert = "Sell";

                        }
                        else if (mData.Contains(MainClass.pair) && mData.Contains(buyTrigger) && (MainClass.positionContracts < 0 || MainClass.positionContracts == 0) && lastAlert != "Buy") 
                        {
                            MainClass.log(emailInfoResponse.Snippet);
                            MainClass.log("Long: " + MainClass.pair);
                            string json = MainClass.bitMEXApi.MarketOrder(MainClass.pair, "Buy", Math.Abs(MainClass.positionContracts) + MainClass.qtdyContacts, "Tradingview Long");
                            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                            {
                                if (json.ToLower().IndexOf("overload") >= 0 && Math.Abs(MainClass.positionContracts) > 0)
                                {/* In case of overload, close the position to prevent losses*/
                                    MainClass.log("System is on overload, trying to close position to prevent losses");
                                    MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, "Buy"));
                                }
                                return;
                            }

                            lastAlert = "Buy";
                        }
                        /* we'll do some crap here*/
                        var markAsReadRequest = new ModifyThreadRequest { RemoveLabelIds = new[] { "UNREAD" } };
                        var markRead = service.Users.Threads.Modify(markAsReadRequest, UserMail, emailInfoResponse.ThreadId);
                        markRead.Execute();
                    }
                }
            }
        }
    }
}
