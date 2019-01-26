using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitBotBackToTheFuture.Strategies
{
    public static class ScalperV2
    {
        public static void run()
        {
            int maxContracts = MainClass.limiteOrder * MainClass.qtdyContacts;
            bool operation = false;
            bool invertop = false;
            bool invertmarket = false;
            Operation _operation = Operation.nothing;
            int positions = MainClass.getPosition();
            bool invert = false;
            int invertMultiplier = 2;
            bool averagedown = false;

            if(MainClass.strategyOptions.Count > 0)
            {
                if (MainClass.strategyOptions.ContainsKey("invert".ToLower()))
                    invert = bool.Parse(MainClass.strategyOptions["invert"]);

                if (MainClass.strategyOptions.ContainsKey("invertmarket".ToLower()))
                    invertmarket = bool.Parse(MainClass.strategyOptions["invertmarket"]);

                if (MainClass.strategyOptions.ContainsKey("invertmultiplier".ToLower()))
                    invertMultiplier = int.Parse(MainClass.strategyOptions["invertmultiplier"]);

                if (MainClass.strategyOptions.ContainsKey("averagedown".ToLower()))
                    averagedown = bool.Parse(MainClass.strategyOptions["averagedown"]);
            }

            if (MainClass.lstIndicatorsEntry.Count == 0)
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

            if(invert && MainClass.lstIndicatorsInvert.Count > 0 )
            {
                Operation _invertop = Operation.nothing;
                MainClass.log("Checking Invert Indicators", ConsoleColor.Green);
                foreach (var item in MainClass.lstIndicatorsInvert)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                    MainClass.log("Invert Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + op.ToString());
                    if (!invertop)
                    {
                        _invertop = op;

                        invertop = true;
                    }
                    else if (_invertop != op)
                    {
                        _invertop = Operation.nothing;
                        break;
                    }
                }

                if( ( positions > 0 && _invertop != Operation.sell) || positions == 0 )
                {
                    invert = false;
                }

                if( (positions < 0 && _invertop != Operation.buy) || positions == 0)
                {
                    invert = false;
                }
            }

            if (_operation != Operation.nothing || invert )
            {
                int oOrders = MainClass.getOpenOrderQty();
                if (Math.Abs(positions) < maxContracts || invert)
                {
                    if (invert)
                    {
                        _operation = Operation.nothing;
                    }

                    if (_operation == Operation.buy && positions >= 0 )
                    {
                        if(averagedown && Math.Abs(positions) > 0 && MainClass.getPriceActual("Buy") > MainClass.positionPrice)
                        {// We are averaging down, so... nope
                            return;
                        }

                        String json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, "Buy", MainClass.getPriceActual("Buy"), Math.Abs(MainClass.qtdyContacts), true, MainClass.marketTaker, "Scalp Buy Order");
                        if(!PostResult(json))
                        {
                            return;
                        }
                        JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                        BitMEX.Order bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                        //MainClass.makeOrder("Buy", true, "Scalp Buy Order", true);
                        for (int i = 0; i < 20; i++)
                        {
                            Thread.Sleep(6000);
                            bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                            if (Math.Abs(MainClass.getPosition()) > 0 && bmOrder.OrdStatus == "Filled")
                            {
                                MainClass.fixOrdersPosition();
                                MainClass.runSL(null);
                                break;
                            }
                        }
                        if (Math.Abs(MainClass.getPosition()) == 0 || bmOrder.OrdStatus != "Filled")
                            MainClass.bitMEXApi.CancelOrderById(MainClass.pair,order["orderID"].ToString());
                    }
                    else if (_operation == Operation.sell && positions <= 0 )
                    {

                        if (averagedown && Math.Abs(positions) > 0 && MainClass.getPriceActual("Sell") < MainClass.positionPrice)
                        {// We are averaging down, so... nope
                            return;
                        }

                        String json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, "Sell", MainClass.getPriceActual("Sell"), Math.Abs(MainClass.qtdyContacts), true, MainClass.marketTaker, "Scalp Sell Order");
                        if (!PostResult(json))
                        {
                            return;
                        }
                        JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                        BitMEX.Order bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                        //MainClass.makeOrder("Sell", true, "Scalp Sell Order", true);
                        for (int i = 0; i < 20; i++)
                        {
                            Thread.Sleep(6000);
                            bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                            if (Math.Abs(MainClass.getPosition()) > 0 && bmOrder.OrdStatus == "Filled")
                            {
                                MainClass.fixOrdersPosition();
                                MainClass.runSL(null);
                                break;
                            }
                        }
                        if (Math.Abs(MainClass.getPosition()) == 0 || bmOrder.OrdStatus != "Filled")
                            MainClass.bitMEXApi.CancelOrderById(MainClass.pair, order["orderID"].ToString());
                    }
                    else if(invert)
                    {
                        if( positions > 0)
                        {
                            MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair, "Inverting Positions");
                            String json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, "Sell", MainClass.getPriceActual("Sell"), Math.Abs(positions) * invertMultiplier, true, invertmarket, "Inverting to Short");
                            JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            BitMEX.Order bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                            for(int i = 0; i < 20; i++)
                            {
                                Thread.Sleep(6000);
                                bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                                if (Math.Abs(MainClass.getPosition()) > 0 && bmOrder.OrdStatus == "Filled")
                                {
                                    MainClass.fixOrdersPosition();
                                    MainClass.runSL(null);
                                    break;
                                }
                            }
                            if (Math.Abs(MainClass.getPosition()) == 0 || bmOrder.OrdStatus != "Filled")
                                MainClass.bitMEXApi.CancelOrderById(MainClass.pair, order["orderID"].ToString());

                        }
                        else if( positions < 0 )
                        {
                            MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair, "Inverting Positions");
                            String json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, "Buy", MainClass.getPriceActual("Buy"), Math.Abs(positions) * invertMultiplier, true, invertmarket, "Inverting to Long");
                            JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            BitMEX.Order bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                            for (int i = 0; i < 20; i++)
                            {
                                Thread.Sleep(6000);
                                bmOrder = MainClass.bitMEXApi.GetOrderById(order["orderID"].ToString());
                                if (Math.Abs(MainClass.getPosition()) > 0 && bmOrder.OrdStatus == "Filled")
                                {
                                    MainClass.fixOrdersPosition();
                                    MainClass.runSL(null);
                                    break;
                                }
                            }
                            if (Math.Abs(MainClass.getPosition()) == 0 || bmOrder.OrdStatus != "Filled")
                                MainClass.bitMEXApi.CancelOrderById(MainClass.pair, order["orderID"].ToString());
                        }
                    }
                }

            }
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
