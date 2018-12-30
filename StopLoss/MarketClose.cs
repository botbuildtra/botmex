using System;

namespace StopLoss
{
    public static class MarketClose
    {
        public static void run()
        {
            bool _stop = false;

            if (MainClass.positionContracts < 0)
            {
                double priceActual = MainClass.getPriceActual("Buy");
                double perc = ((priceActual * 100) / MainClass.positionPrice) - 100;
                MainClass.log("perc" + perc);
                if (perc > 0 && !double.IsInfinity(perc))
                    if (perc > MainClass.stoploss)
                        _stop = true;
            }

            if (MainClass.positionContracts > 0)
            {
                double priceActual = MainClass.getPriceActual("Sell");
                double perc = ((priceActual * 100) / MainClass.positionPrice) - 100;
                MainClass.log("perc" + perc);
                if (perc < 0 && !double.IsInfinity(perc))
                    if (Math.Abs(perc) > MainClass.stoploss)
                        _stop = true;
            }


            if (_stop)
            {
                //Stop loss
                MainClass.log(MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair));
                string side = "Buy";
                if (MainClass.positionContracts > 0)
                    side = "Sell";

                MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "STOP LOSS Market Close"));
                /*if (side == "Sell")
                    log(bitMEXApi.PostOrderPostOnly(pair, side, getPriceActual("Sell") - 10, Math.Abs(positionContracts), true));

                if (side == "Buy")
                    log(bitMEXApi.PostOrderPostOnly(pair, side, getPriceActual("Sell") + 10, Math.Abs(positionContracts), true));*/
                MainClass.log("[STOP LOSS] " + MainClass.pair + " " + side + " " + MainClass.positionContracts);
            }




            bool _stopgain = false;
            if (MainClass.positionContracts < 0)
            {
                double priceActual = MainClass.getPriceActual("Buy");
                double perc = ((priceActual * 100) / MainClass.positionPrice) - 100;
                MainClass.log("perc" + perc);
                if (perc < 0 && !double.IsInfinity(perc))
                {

                    if (MainClass.traillingProfit > Math.Abs(perc))
                        if (Math.Abs(perc) > MainClass.stopgain)
                            _stopgain = true;

                    if (Math.Abs(perc) > MainClass.traillingProfit)
                        MainClass.traillingProfit = Math.Abs(perc);

                    if (Math.Abs(perc) > MainClass.stopgain)
                        _stopgain = true;
                }
            }

            if (MainClass.positionContracts > 0)
            {
                double priceActual = MainClass.getPriceActual("Sell");
                double perc = ((priceActual * 100) / MainClass.positionPrice) - 100;
                MainClass.log("perc" + perc);
                if (perc > 0 && !double.IsInfinity(perc))
                {
                    if (MainClass.traillingProfit > Math.Abs(perc))
                        if (Math.Abs(perc) > MainClass.stopgain)
                            _stopgain = true;

                    if (Math.Abs(perc) > MainClass.traillingProfit)
                        MainClass.traillingProfit = Math.Abs(perc);

                    if (Math.Abs(perc) > MainClass.stopgain)
                        _stopgain = true;
                }
            }


            if (_stopgain)
            {
                //Stop loss
                MainClass.log(MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair));
                String side = "Buy";
                if (MainClass.positionContracts > 0)
                    side = "Sell";

                MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "STOP GAIN MarketClose"));
                /*if (side == "Sell")
                    log(bitMEXApi.PostOrderPostOnly(pair, side, getPriceActual("Sell") - 10, Math.Abs(positionContracts), true));
                if (side == "Buy")
                    log(bitMEXApi.PostOrderPostOnly(pair, side, getPriceActual("Sell") + 10, Math.Abs(positionContracts), true));*/
                MainClass.log("[STOP GAIN] " + MainClass.pair + " " + side + " " + MainClass.positionContracts);
            }
        }
    }

    
}