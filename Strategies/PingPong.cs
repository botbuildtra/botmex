using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Botmex.Strategies
{
    public static class PingPong
    {
        public static void run()
        {
            MainClass.Indicatorless = true;
            if (MainClass.strategyOptions.Count < 2 
                || !MainClass.strategyOptions.ContainsKey("buyat") 
                || !MainClass.strategyOptions.ContainsKey("sellat") )
            {
                throw new Exception("Please check if buyat and sellat are set");
            }


            int maxContracts = MainClass.qtdyContacts;
            int positions = MainClass.getPosition();
            int oOrders = MainClass.getOpenOrderQty();
            double buyAt = double.Parse(MainClass.strategyOptions["buyat"]);
            double sellAt = double.Parse(MainClass.strategyOptions["sellat"]);

            if( Math.Abs(oOrders) == 0 && Math.Abs(positions) == 0)
            {
                String json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, "Buy", buyAt, Math.Abs(MainClass.qtdyContacts), false, false, "PingPong Buy Order");
                if (!PostResult(json))
                {
                    return;
                }
                JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                BitMEX.Order bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(6000);
                    bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                    if (Math.Abs(MainClass.getPosition()) > 0 && bmOrder.OrdStatus == "Filled")
                    {
                        MainClass.fixOrdersPosition(true, sellAt, "Ping Pong Sell Exit Order");
                        MainClass.runSL(null);
                        break;
                    }
                }
                if (Math.Abs(MainClass.getPosition()) == 0 || bmOrder.OrdStatus != "Filled")
                    MainClass.bitMEXApi.CancelOrderById(MainClass.pair, order["orderID"].ToString());
            }

            MainClass.fixOrdersPosition(true, sellAt, "Ping Pong Sell Exit Order");

        }

        private static bool PostResult(string json)
        {
            MainClass.log(json);
            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
            {
                if (json.ToLower().IndexOf("overload") >= 0)
                {/* In case of overload, close the position to prevent losses*/
                    MainClass.log("System is on overload, Not posting position");
                    //log(bitMEXApi.MarketClose(pair, side));
                }
                return false;
            }
            return true;
        }
    }
}
