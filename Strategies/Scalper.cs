using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Botmex.Strategies
{
    public static class Scalper
    {
        public static void run()
        {
            bool operation = false;
            Operation _operation = Operation.nothing;

            if( MainClass.lstIndicatorsEntry.Count == 0 )
            {
                throw new Exception("Scalper sem indicadores nao funciona ... foda-se!");
            }

            foreach (var item in MainClass.lstIndicatorsEntry)
            {
                Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);

                MainClass.log("Indicator: " + item.getName());
                MainClass.log("Result1: " + item.getResult());
                MainClass.log("Result2: " + item.getResult2());
                MainClass.log("Operation: " + op.ToString());
                MainClass.log("");

                if (!operation)
                {
                    _operation = op;

                    operation = true;
                }
                else if( _operation != op )
                {
                    _operation = Operation.nothing;
                    break;
                }
            }

            if (MainClass.lstIndicatorsEntryThreshold.Count > 0 && _operation != Operation.nothing)
            {
                foreach (var item in MainClass.lstIndicatorsEntryThreshold)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                    MainClass.log("Threshold Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + op.ToString());
                    if ( op != Operation.allow )
                    {
                        _operation = Operation.nothing;
                        break;
                    }
                }
            }


            if ( MainClass.lstIndicatorsEntryCross.Count > 0 && _operation != Operation.nothing )
            {
                foreach( var item in MainClass.lstIndicatorsEntryCross)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                    MainClass.log("Cross Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + op.ToString());
                    if ( _operation != op )
                    {
                        _operation = Operation.nothing;
                        break;
                    }
                }
            }

            if (_operation != Operation.nothing)
            {

                if (Math.Abs(MainClass.getPosition()) == 0 && Math.Abs(MainClass.getOpenOrderQty()) == 0)
                {


                    List<double> lst = MainClass.arrayPriceClose[MainClass.timeGraph].OfType<double>().ToList();

                    MainClass.log("Min: " + lst.Min());
                    MainClass.log("Avg: " + lst.Average());
                    MainClass.log("Max: " + lst.Max());

                    double percaux  = 0.3;
                    double min5 = ((lst.Min() * percaux) / 100) + (lst.Min());
                    double max5 = (lst.Max()) - ((lst.Max() * percaux) / 100);

                    MainClass.log("Min5: " + min5);
                    MainClass.log("Max5: " + max5);
                    if (MainClass.arrayPriceClose[MainClass.timeGraph][499] > min5 && MainClass.arrayPriceClose[MainClass.timeGraph][499] < max5)
                    {

                        if (_operation == Operation.buy)
                        {
                            MainClass.makeOrder("Buy", true,"Scalp Buy Order",true);
                            for (int i = 0; i < 20; i++)
                            {
                                Thread.Sleep(6000);
                                if (Math.Abs(MainClass.getPosition()) > 0)
                                {
                                    MainClass.fixOrdersPosition();
                                    MainClass.runSL(null);
                                    break;
                                }
                            }
                            if (Math.Abs(MainClass.getPosition()) == 0)
                                MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);
                        }
                        else if (_operation == Operation.sell)
                        {
                            MainClass.makeOrder("Sell", true,"Scalp Sell Order",true);
                            for (int i = 0; i < 20; i++)
                            {
                                Thread.Sleep(6000);
                                if (Math.Abs(MainClass.getPosition()) > 0)
                                {
                                    MainClass.fixOrdersPosition();
                                    MainClass.runSL(null);
                                    break;
                                }
                            }
                            if (Math.Abs(MainClass.getPosition()) == 0)
                                MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);

                        }

                    }
                }

            }
        
        }
    }
}
