using System;
using System.Threading;

namespace BitBotBackToTheFuture.Strategies
{
    public static class Bingo
    {
        public static void run()
        {

            if (MainClass.lstIndicatorsEntryThreshold.Count > 0 )
            {
                foreach (var item in MainClass.lstIndicatorsEntryThreshold)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                    MainClass.log("Threshold Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + op.ToString());
                    if (op != Operation.allow)
                    {
                        return;
                    }
                }
            }

            if (Math.Abs(MainClass.getPosition()) == 0 && Math.Abs(MainClass.getOpenOrderQty()) == 0)
            {
                MainClass.makeOrder("Buy", true,"Bingo Buy Order", true);
                MainClass.makeOrder("Sell", true,"Bingo Sell Order",true);
                MainClass.log("wait " + 60000 * 5 + "ms", ConsoleColor.Blue);
                Thread.Sleep(60000 * 5);
                MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);
            }

        }
    }
}
