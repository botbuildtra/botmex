using BitMEX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StopLoss
{
    public static class OrderBook
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


            if(_stop)
            {
                MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);
            }

            List<BitMEX.Order> lst = MainClass.bitMEXApi.GetOpenSLOrders(MainClass.pair);
            if(lst.Count() == 0 )
            {
                
                if( MainClass.positionContracts < 0 )
                {
                    double pactual = Math.Abs(MainClass.getPositionPrice());
                    double SLValue = pactual + (pactual / 100 ) * MainClass.stoploss;
                    string json = MainClass.bitMEXApi.CreateStopOrder(MainClass.pair,"Buy",Math.Abs(MainClass.positionContracts),SLValue, true,0,"STOP LOSS MARKET ORDER");

                    MainClass.log(json);
                }
                else if(MainClass.positionContracts > 0 )
                {
                    double pactual = Math.Abs(MainClass.getPositionPrice());
                    double SLValue = pactual - (pactual / 100 ) * MainClass.stoploss;
                    string json = MainClass.bitMEXApi.CreateStopOrder(MainClass.pair,"Sell",Math.Abs(MainClass.positionContracts),SLValue, true,0,"STOP LOSS MARKET ORDER");
                    MainClass.log(json);
                }
            }
            else
            { /* Update OB SL */
                if(MainClass.positionContracts != 0 )
                {
                    double pactual = Math.Abs(MainClass.getPositionPrice());
                    double SLValue = 0;
                    if(MainClass.positionContracts < 0 )
                        SLValue = pactual + (pactual / 100 ) * MainClass.stoploss;
                    else
                        SLValue = pactual - (pactual / 100 ) * MainClass.stoploss;
                        
                    if (MainClass.pair.Contains("XBT"))
                    {
                        SLValue = MainClass.bitMEXApi.RoundToNearest(SLValue, 2);
                    }
                    else if (MainClass.pair.Equals("ETHUSD"))
                    {
                        SLValue = MainClass.bitMEXApi.RoundToNearest(SLValue, 20);
                    }

                    if( lst[0].OrderQty != Math.Abs(MainClass.positionContracts) || lst[0].StopPx != SLValue )
                    {
                        string json = MainClass.bitMEXApi.EditSLOrderPx(lst[0].OrderId,SLValue, Math.Abs(MainClass.positionContracts));
                        MainClass.log(json);
                    }
                }
                else 
                {
                    string json = MainClass.bitMEXApi.DeleteOrders(lst[0].OrderId);
                    MainClass.log(json);
                }
            }
        }
    }
}