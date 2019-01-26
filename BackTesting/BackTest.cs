using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Botmex
{
    public class BackTest
    {

        /*private static string symbol = "XBTUSD";
        private static int sizeArrayCandles = 500;
        private static  string size = "5m";*/


        public static double[] SubArray(double[] data, int index, int length)
        {
            double[] result = new double[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void run()
        {
            while(true)
            {
                try
                {
                    MainClass.log("BACKTESTING....");
                    MainClass.getCandles(MainClass.timeGraph, false);
                    int maxContracts = MainClass.limiteOrder * MainClass.qtdyContacts;
                    bool operation = false;
                    Operation _operation = Operation.nothing;
                    int positions = MainClass.getPosition();

                    if (MainClass.lstIndicatorsEntry.Count == 0)
                    {
                        throw new Exception("Scalper sem indicadores nao funciona ... foda-se!");
                    }


                    if (MainClass.lstIndicatorsEntryThreshold.Count > 0)
                    {
                        foreach (var item in MainClass.lstIndicatorsEntryThreshold)
                        {
                            Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                            MainClass.log("Threshold Indicator: " + item.getName());
                            MainClass.log("Result1: " + item.getResult());
                            MainClass.log("Result2: " + item.getResult2());
                            MainClass.log("Operation: " + op.ToString());
                            /*if (op != Operation.allow)
                            {
                                _operation = Operation.nothing;
                                break;
                            }*/
                        }
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
                        else if (_operation != op)
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
                            if (op != Operation.allow)
                            {
                                _operation = Operation.nothing;
                                break;
                            }
                        }
                    }


                    if (MainClass.lstIndicatorsEntryCross.Count > 0 && _operation != Operation.nothing)
                    {
                        foreach (var item in MainClass.lstIndicatorsEntryCross)
                        {
                            Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                            MainClass.log("Cross Indicator: " + item.getName());
                            MainClass.log("Result1: " + item.getResult());
                            MainClass.log("Result2: " + item.getResult2());
                            MainClass.log("Operation: " + op.ToString());
                            if (_operation != op)
                            {
                                _operation = Operation.nothing;
                                break;
                            }
                        }
                    }

                    if (_operation != Operation.nothing)
                    {

                        if (Math.Abs(positions) < maxContracts && Math.Abs(MainClass.getOpenOrderQty()) == 0)
                        {
                            if (_operation == Operation.buy)
                            {
                                MainClass.log("Would have tried to buy at: " + MainClass.getPriceActual("Buy"));
                                /*return;
                                MainClass.makeOrder("Buy", true, "Scalp Buy Order", true);
                                for (int i = 0; i < 20; i++)
                                {
                                    Thread.Sleep(6000);
                                    if (Math.Abs(MainClass.getPosition()) > 0)
                                    {
                                        MainClass.fixOrdersPosition();
                                        break;
                                    }
                                }
                                if (Math.Abs(MainClass.getPosition()) == 0)
                                    MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);*/
                            }
                            else if (_operation == Operation.sell)
                            {
                                MainClass.log("Would have tried to sell at: " + MainClass.getPriceActual("Sell"));
                                /*return;
                                MainClass.makeOrder("Sell", true, "Scalp Sell Order", true);
                                for (int i = 0; i < 20; i++)
                                {
                                    Thread.Sleep(6000);
                                    if (Math.Abs(MainClass.getPosition()) > 0)
                                    {
                                        MainClass.fixOrdersPosition();
                                        break;
                                    }
                                }
                                if (Math.Abs(MainClass.getPosition()) == 0)
                                    MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);*/
                            }
                        }

                    }

                    MainClass.log("Sleeping");
                    Thread.Sleep(MainClass.interval);
                }
                catch( Exception e)
                {
                    MainClass.log(e.StackTrace.ToString());
                    MainClass.log(e.Message);
                    MainClass.log(e.ToString());
                }

            }




        }

    }
}
