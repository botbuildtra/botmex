using System;
using System.IO;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace BitBotBackToTheFuture.TradingView
{

    public class GmailAlerts
    {
        static string[] Scopes = { GmailService.Scope.GmailModify };
        static string ApplicationName = "BotMex Trader";
        public GmailAlerts()
        {
        }

        public void run()
        {
            UserCredential credential;

            using (var stream =
              new FileStream("creds.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = MainClass.location + "creds2.json";

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


            var inboxlistRequest = service.Users.Messages.List("tperalta82@gmail.com");
            inboxlistRequest.LabelIds = "Label_4430037365957264008";
            inboxlistRequest.Q = "is:unread";
            inboxlistRequest.IncludeSpamTrash = false;
            //get our emails 
            var emailListResponse = inboxlistRequest.Execute();

            if (emailListResponse != null && emailListResponse.Messages != null)
            {
                //loop through each email and get what fields you want... 
                foreach (var email in emailListResponse.Messages)
                {

                    var emailInfoRequest = service.Users.Messages.Get("tperalta82@gmail.com", email.Id);
                    var emailInfoResponse = emailInfoRequest.Execute();

                    if (emailInfoResponse != null)
                    {
                        Console.WriteLine(emailInfoResponse.Snippet);
                        Console.WriteLine(emailInfoResponse.InternalDate);
                        /* we'll do some crap here*/
                        var markAsReadRequest = new ModifyThreadRequest { RemoveLabelIds = new[] { "UNREAD" } };
                        var pex = service.Users.Threads.Modify(markAsReadRequest, "tperalta82@gmail.com", emailInfoResponse.ThreadId);
                        pex.Execute();
                        //Console.ReadLine();

                        //loop through the headers to get from,date,subject, body  
                        /*foreach (var mParts in emailInfoResponse.Payload.Headers)
                        {
                            if (mParts.Name == "Date")
                            {
                                date = mParts.Value;
                            }
                            else if (mParts.Name == "From")
                            {
                                from = mParts.Value;
                            }
                            else if (mParts.Name == "Subject")
                            {
                                subject = mParts.Value;
                            }

                            if (date != "" && from != "")
                            {


                                foreach (MessagePart p in emailInfoResponse.Payload.Parts)
                                {
                                    if (p.MimeType == "text/html")
                                    {
                                        byte[] data = FromBase64ForUrlString(p.Body.Data);
                                        string decodedString = Encoding.UTF8.GetString(data);
                                        Console.WriteLine(decodedString);

                                    }
                                }



                            }

                        }*/
                    }
                }

            }
        }

        public static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }
    }
}
