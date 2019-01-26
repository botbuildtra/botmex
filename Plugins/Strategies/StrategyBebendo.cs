using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BitMEX;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Botmex.Strategies
{
    public class Bebendo : IStrategies
    {
        public static int scalpPercent = 10;
        public static string timeGraphEMA = "5m";
        public static int scalpQty, surfQty = 0;
        public static int positions = 0;
        public static string lastScalpOrder = "";
        public static string lastFixPosOrder = "";
        public static Timer slt = null;


        public void run()
        {
            MainClass.log("Running Strategy Bebendo");
            StaticSetup();
            StaticRun();
        }


        public static void StaticSetup()
        {
            if (MainClass.strategyOptions.Count > 0)
            {
                if (MainClass.strategyOptions.ContainsKey("scalpPercent".ToLower()))
                    scalpPercent = int.Parse(MainClass.strategyOptions["scalpPercent"]);

                if (MainClass.strategyOptions.ContainsKey("timeGraphEMA".ToLower()))
                    timeGraphEMA = MainClass.strategyOptions["timeGraphEMA"].ToLower();
            }
        }

        public static void StaticRun()
        {
            try
            {
                positions = MainClass.getPosition();
                scalpQty = (MainClass.qtdyContacts * scalpPercent) / 100;
                surfQty = MainClass.qtdyContacts - scalpQty;

                MainClass.log(scalpQty.ToString());
                MainClass.log(surfQty.ToString());

                if (MainClass.stoplosstype == "strategy")
                {
                    if (slt == null)
                    {
                        slt = new Timer(runSL, null, TimeSpan.Zero, TimeSpan.FromSeconds(MainClass.stoplossInterval));
                    }
                }

                #region scalper code
                bool operation = false;
                Operation _operation = Operation.nothing;

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

                if (_operation != Operation.nothing && allowPosition("scalp", _operation))
                {
                    //if ( ( Math.Abs(MainClass.getPosition()) <  && Math.Abs(MainClass.getOpenOrderQty()) == 0 )  )
                    if ((MainClass.qtdyContacts - Math.Abs(positions) == scalpQty || Math.Abs(positions) < scalpQty) && MainClass.getOpenOrderQty() == 0)
                    {


                        List<double> lst = MainClass.arrayPriceClose.OfType<double>().ToList();

                        MainClass.log("Min: " + lst.Min());
                        MainClass.log("Avg: " + lst.Average());
                        MainClass.log("Max: " + lst.Max());

                        double percaux = 0.3;
                        double min5 = ((lst.Min() * percaux) / 100) + (lst.Min());
                        double max5 = (lst.Max()) - ((lst.Max() * percaux) / 100);

                        MainClass.log("Min5: " + min5);
                        MainClass.log("Max5: " + max5);
                        if (MainClass.arrayPriceClose[MainClass.timeGraph][499] > min5 && MainClass.arrayPriceClose[MainClass.timeGraph][499] < max5)
                        {
                            int pos = 0;
                            if (_operation == Operation.buy && allowPosition("scalp", _operation))
                            {

                                makeOrder("Buy", true, "Scalp Buy Order");
                                for (int i = 0; i < 20; i++)
                                {
                                    Thread.Sleep(6000);
                                    pos = MainClass.getPosition();
                                    if (Math.Abs(pos) == scalpQty || Math.Abs(pos) == MainClass.qtdyContacts)
                                    {
                                        fixOrdersPosition();
                                        break;
                                    }
                                }
                                pos = MainClass.getPosition();
                                if (Math.Abs(pos) == 0 || Math.Abs(pos) == surfQty)
                                    MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);
                            }
                            else if (_operation == Operation.sell && allowPosition("scalp", _operation))
                            {
                                makeOrder("Sell", true, "Scalp Sell Order");
                                for (int i = 0; i < 20; i++)
                                {
                                    Thread.Sleep(6000);
                                    pos = MainClass.getPosition();
                                    if (Math.Abs(pos) == scalpQty || Math.Abs(pos) == MainClass.qtdyContacts)
                                    {
                                        fixOrdersPosition();
                                        break;
                                    }
                                }
                                pos = MainClass.getPosition();
                                if (Math.Abs(pos) == 0 || Math.Abs(pos) == surfQty)
                                    MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);

                            }

                        }
                    }

                }

                #endregion
                fixOrdersPosition();
                #region surf code
                if (MainClass.timeGraph != "5m")
                    MainClass.getCandles("5m");

                if (MainClass.statusLong == "enable")
                {
                    MainClass.log("");
                    MainClass.log("==========================================================");
                    MainClass.log(" ==================== Verify LONG OPERATION =============", ConsoleColor.Green);
                    MainClass.log("==========================================================");
                    /////VERIFY OPERATION LONG
                    string _surfoperation = "long";
                    //VERIFY INDICATORS ENTRY


                    foreach (var item in MainClass.lstIndicatorsEntryCross)
                    {
                        Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                        MainClass.log("Indicator: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Date: " + MainClass.arrayDate[MainClass.arrayPriceOpen[item.getTimegraph()].Length - 1]);
                        MainClass.log("Operation: " + operationBuy.ToString());
                        MainClass.log("");
                        if (operationBuy != Operation.buy)
                        {
                            _surfoperation = "nothing";
                            break;
                        }
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
                            if (op != Operation.allow)
                            {
                                _surfoperation = "nothing";
                                break;
                            }
                        }
                    }

                    //EXECUTE OPERATION
                    if (_surfoperation == "long" && allowPosition("surf", Operation.buy))
                    {
                        positions = MainClass.getPosition();
                        MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);
                        if (positions < 0)
                        {
                            MainClass.bitMEXApi.MarketClose(MainClass.pair, "Buy", "Market close for enter long");
                            lastScalpOrder = "";
                            lastFixPosOrder = "";

                        }
                        makeOrder("Buy", false, "Surf Long Position", true, "surf", true);
                    }


                    ////////////FINAL VERIFY OPERATION LONG//////////////////
                }



                if (MainClass.statusShort == "enable")
                {

                    //////////////////////////////////////////////////////////////
                    MainClass.log("");
                    MainClass.log("==========================================================");
                    MainClass.log(" ==================== Verify SHORT OPERATION =============", ConsoleColor.Red);
                    MainClass.log("==========================================================");
                    /////VERIFY OPERATION LONG
                    string _surfoperation = "short";
                    //VERIFY INDICATORS ENTRY
                    foreach (var item in MainClass.lstIndicatorsEntryCross)
                    {
                        Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                        MainClass.log("Indicator: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + operationBuy.ToString());
                        MainClass.log("");
                        if (operationBuy != Operation.sell)
                        {
                            _surfoperation = "nothing";
                            break;
                        }
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
                            if (op != Operation.allow)
                            {
                                _surfoperation = "nothing";
                                break;
                            }
                        }
                    }

                    //EXECUTE OPERATION
                    if (_surfoperation == "short" && allowPosition("surf", Operation.sell))
                    {
                        MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);
                        positions = MainClass.getPosition();
                        if (positions > 0)
                        {
                            MainClass.bitMEXApi.MarketClose(MainClass.pair, "Sell", "Market Close for enter short");
                            lastScalpOrder = "";
                            lastFixPosOrder = "";
                        }
                        makeOrder("Sell", false, "Surf Short Position", true, "surf", true);
                    }


                    ////////////FINAL VERIFY OPERATION LONG//////////////////
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.log("MainLoop Bebendo Error:: " + e.Message + e.StackTrace);
            }

        }

        public static bool allowPosition(string type, Operation op)
        {
            int pos = MainClass.getPosition();
            if (type == "scalp")
            {
                if ((Math.Abs(pos) == surfQty && pos > 0 && op == Operation.buy) || (pos > 0 && Math.Abs(pos) < MainClass.qtdyContacts) || Math.Abs(pos) == 0)
                {/* Allow only to scalp when on a surf position*/
                    return true;
                }
                if ((Math.Abs(pos) == surfQty && pos < 0 && op == Operation.sell) || (pos < 0 && Math.Abs(pos) < MainClass.qtdyContacts) || Math.Abs(pos) == 0)
                {
                    return true;
                }
            }
            else if (type == "surf")
            {
                return true;
            }
            return false;
        }

        public static void fixOrdersPosition(bool force = true)
        {
            try
            {
                positions = MainClass.getPosition();
                int OpenOrders = MainClass.getOpenOrderQty();
                MainClass.log("Open Orders: " + OpenOrders);
                if (MainClass.roeAutomatic && (Math.Abs(OpenOrders) < Math.Abs(scalpQty)))
                {
                    MainClass.log("Get Position " + positions);


                    int qntContacts = (Math.Abs(positions) - Math.Abs(OpenOrders));


                    if (positions > 0 && (Math.Abs(positions) == scalpQty || Math.Abs(positions) == MainClass.qtdyContacts))
                    {

                        string side = "Sell";
                        double priceContacts = Math.Abs(MainClass.getPositionPrice());
                        double actualPrice = Math.Abs(MainClass.getPriceActual(side));
                        //double priceContactsProfit = Math.Abs(Math.Floor(priceContacts + (priceContacts * (profit + fee) / 100)));


                        Order lastOrder = MainClass.bitMEXApi.GetOrderById(lastScalpOrder);
                        if (lastOrder.OrdStatus == "Filled" && lastOrder.OrderQty == scalpQty && lastOrder.AvgPx != null && lastOrder.Side == "Buy")
                        {
                            double price = (double)lastOrder.AvgPx + MainClass.stepValue;
                            MainClass.log("Creating Scalp Sell Order to Exit Buy", ConsoleColor.Red);
                            string json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(scalpQty), force, false, "Scalp Sell Order, EXIT BUY");
                            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                            {
                                MainClass.log("[ERROR FIX POSITION] :" + json);
                                return;
                            }
                            JContainer config2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            lastFixPosOrder = config2["orderID"].ToString();
                            MainClass.log(json);
                        }

                    }

                    if (positions < 0 && (Math.Abs(positions) == scalpQty || Math.Abs(positions) == MainClass.qtdyContacts))
                    {

                        string side = "Buy";
                        double priceContacts = Math.Abs(MainClass.getPositionPrice());
                        double actualPrice = Math.Abs(MainClass.getPriceActual(side));
                        //double priceContactsProfit = Math.Abs(Math.Floor(priceContacts - (priceContacts * (Mainprofit + fee) / 100)));


                        Order lastOrder = MainClass.bitMEXApi.GetOrderById(lastScalpOrder);
                        if (lastOrder.OrdStatus == "Filled" && lastOrder.OrderQty == scalpQty && lastOrder.AvgPx != null && lastOrder.Side == "Sell")
                        {
                            double price = (double)lastOrder.AvgPx - MainClass.stepValue;
                            MainClass.log("Creating Scalp Buy Order to Exit Sell", ConsoleColor.Red);
                            string json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(scalpQty), force, false, "Scalp Buy Order, EXIT SELL");
                            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                            {
                                MainClass.log("[ERROR FIX POSITION] :" + json);
                                return;
                            }
                            JContainer config2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                            lastFixPosOrder = config2["orderID"].ToString();
                            MainClass.log(json);
                        }

                    }
                    return;
                }


            }
            catch (Exception e)
            {
                MainClass.log("FixPositions Bebendo Error:: " + e.Message + e.StackTrace);
            }

        }

        public static void makeOrder(string side, bool nothing = false, string text = "botmex", bool force = false, string operation = "surf", bool MarketClose = false)
        {
            bool execute = false;
            try
            {
                MainClass.log(" Make order " + side);
                double price = 0;
                String json = "";

                if (nothing)
                {
                    price = Math.Abs(MainClass.getPriceActual(side));
                    json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(scalpQty), false, MainClass.marketTaker, text);
                    JContainer order = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                    lastScalpOrder = order["orderID"].ToString();
                    return;
                }

                if (side == "Sell" && MainClass.statusShort == "enable" && Math.Abs(MainClass.limiteOrder) > Math.Abs(MainClass.bitMEXApi.GetOpenOrders(MainClass.pair).Count) && MainClass.getPosition() >= 0)
                {


                    if (operation == "surf")
                    {
                        price = Math.Abs(MainClass.getPriceActual(side));
                        if (MarketClose)
                        {
                            MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);
                            MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "Sell Market Close for Surf");
                        }
                        json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price + MainClass.obDiff, Math.Abs(MainClass.getPosition()) + Math.Abs(surfQty), true, true, text);
                    }

                    MainClass.log(json);

                    if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                    {
                        if (json.ToLower().IndexOf("overload") >= 0)
                        {/* In case of overload, close the position to prevent losses*/
                            MainClass.log("System is on overload, trying to close position to prevent losses");
                            MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, side));
                        }
                        return;
                    }


                    MainClass.log("Short Order at: " + price);

                    JContainer config = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));


                    for (int i = 0; i < MainClass.intervalCancelOrder; i++)
                    {
                        if (!MainClass.existsOrderOpenById(config["orderID"].ToString()) && !MainClass.existsOrderOpenById(config["orderID"].ToString()))
                        {
                            if (operation == "scalper")
                            {
                                price = price - MainClass.stepValue;
                            }
                            else
                            {
                                price -= (price * MainClass.profit) / 100;
                                price = Math.Abs(Math.Floor(price));
                            }


                            MainClass.traillingProfit = -1;
                            MainClass.traillingStop = -1;

                            MainClass.log(json);
                            execute = true;
                            break;
                        }
                        MainClass.log("wait order limit " + i + " of " + MainClass.intervalCancelOrder + "...");
                        if (operation != "scalper")
                            Thread.Sleep(800);
                    }



                    if (!execute)
                    {
                        MainClass.bitMEXApi.DeleteOrders(config["orderID"].ToString());
                        while (MainClass.existsOrderOpenById(config["orderID"].ToString()))
                            MainClass.bitMEXApi.DeleteOrders(config["orderID"].ToString());
                        MainClass.log("Cancel order ID " + config["orderID"].ToString());
                    }

                }

                if (side == "Buy" && MainClass.statusLong == "enable" && Math.Abs(MainClass.limiteOrder) > Math.Abs(MainClass.bitMEXApi.GetOpenOrders(MainClass.pair).Count) && MainClass.getPosition() <= 0)
                {
                    price = 0;
                    json = "";
                    if (operation == "surf" || operation == "normal")
                    {
                        price = Math.Abs(MainClass.getPriceActual(side));
                        if (MarketClose)
                        {
                            MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);
                            MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "Buy Market Close for Surf");
                        }
                        json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price - MainClass.obDiff, Math.Abs(MainClass.getPosition()) + Math.Abs(surfQty), true, true, text);
                    }


                    MainClass.log(json);
                    if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                    {
                        if (json.ToLower().IndexOf("overload") >= 0)
                        {/* In case of overload, close the position to prevent losses*/
                            MainClass.log("System is on overload, trying to close position to prevent losses");
                            MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "Market Close Overload"));
                        }
                        return;
                    }


                    MainClass.log("Long Order at: " + price);
                    JContainer config = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));

                    MainClass.log("wait total...");
                    for (int i = 0; i < MainClass.intervalCancelOrder; i++)
                    {
                        if (!MainClass.existsOrderOpenById(config["orderID"].ToString()) && !MainClass.existsOrderOpenById(config["orderID"].ToString()))
                        {

                            if (operation == "scalper")
                            {
                                price = price + MainClass.stepValue;
                            }
                            else
                            {
                                price += (price * MainClass.profit) / 100;
                                price = Math.Abs(Math.Floor(price));
                            }


                            MainClass.traillingProfit = -1;
                            MainClass.traillingStop = -1;
                            MainClass.log(json);
                            execute = true;
                            break;
                        }
                        MainClass.log("wait order limit " + i + " of " + MainClass.intervalCancelOrder + "...");
                        if (operation != "scalper")
                            Thread.Sleep(1000);
                    }


                    if (!execute)
                    {
                        MainClass.bitMEXApi.DeleteOrders(config["orderID"].ToString());
                        while (MainClass.existsOrderOpenById(config["orderID"].ToString()))
                            MainClass.bitMEXApi.DeleteOrders(config["orderID"].ToString());
                        MainClass.log("Cancel order ID " + config["orderID"].ToString());
                    }


                }
            }
            catch (Exception ex)
            {
                MainClass.log("makeOrder()::" + ex.Message + ex.StackTrace);
            }

            if (execute)
            {

                MainClass.log("wait " + MainClass.intervalOrder + "ms", ConsoleColor.Blue);
                Thread.Sleep(MainClass.intervalOrder);
            }
        }

        public static void runSL(object state)
        {
            try
            {
                MainClass.log("Checking for possible stop losses", ConsoleColor.Red);
                bool _stop = false;

                if ((Math.Abs(MainClass.positionContracts) == Math.Abs(scalpQty) || Math.Abs(MainClass.positionContracts) == Math.Abs(MainClass.qtdyContacts)) && lastScalpOrder != "")
                {
                    Order lastOrder = MainClass.bitMEXApi.GetOrderById(lastScalpOrder);
                    if (lastOrder.AvgPx != null)
                    {
                        double scalpPrice = (double)lastOrder.AvgPx;
                        if (MainClass.positionContracts < 0)
                        {
                            double priceActual = MainClass.getPriceActual("Buy");
                            double perc = ((priceActual * 100) / scalpPrice) - 100;
                            MainClass.log("perc" + perc);
                            if (perc > 0 && !double.IsInfinity(perc))
                                if (perc > MainClass.stoploss)
                                    _stop = true;
                        }

                        if (MainClass.positionContracts > 0)
                        {
                            double priceActual = MainClass.getPriceActual("Sell");
                            double perc = ((priceActual * 100) / scalpPrice) - 100;
                            MainClass.log("perc" + perc);
                            if (perc < 0 && !double.IsInfinity(perc))
                                if (Math.Abs(perc) > MainClass.stoploss)
                                    _stop = true;
                        }


                        if (_stop)
                        {
                            string json = "";
                            //Stop loss
                            MainClass.log(MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair));
                            string side = "Buy";
                            if (MainClass.positionContracts > 0)
                                side = "Sell";


                            //MainClass.log(MainClass.bitMEXApi.MarketClose(MainClass.pair, side, "STOP LOSS Market Close"));
                            if (side == "Sell")
                            {
                                json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, MainClass.getPriceActual("Sell") - MainClass.obDiff, Math.Abs(scalpQty), true, true, "STOP LOSS");
                            }


                            if (side == "Buy")
                            {
                                json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, MainClass.getPriceActual("Sell") + MainClass.obDiff, Math.Abs(scalpQty), true, true, "STOP LOSS");
                            }

                            if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                            {
                                MainClass.log("[ERROR STOP LOSS] :" + json);
                            }
                            else
                            {
                                lastScalpOrder = "";
                                MainClass.log("[STOP LOSS] " + MainClass.pair + " " + side + " " + Math.Abs(scalpQty));

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.log("Error RunSL Bebendo:: " + e.Message + e.StackTrace);
            }

        }

        public static void RunSLOB(object state)
        {
            try
            {
                MainClass.log("Checking and Adjusting/Canceling orderbook SL's if needed", ConsoleColor.Green);
                List<Order> SLOrders = MainClass.bitMEXApi.GetOpenSLOrders(MainClass.pair);
                int OpenOrders = MainClass.getOpenOrderQty();


                if ((Math.Abs(MainClass.positionContracts) == Math.Abs(scalpQty) || Math.Abs(MainClass.positionContracts) == Math.Abs(MainClass.qtdyContacts)) && lastScalpOrder != "")
                {
                    if (SLOrders.Count == 0)
                    {
                        Order lastOrder = MainClass.bitMEXApi.GetOrderById(lastScalpOrder);
                        if (lastOrder.AvgPx != null)
                        {
                            double scalpPrice = (double)lastOrder.AvgPx;

                            string json = "";
                            MainClass.log("Creating Stop Loss Order", ConsoleColor.Red);
                            if (MainClass.positionContracts < 0)
                            {
                                double SLValue = scalpPrice + (scalpPrice / 100) * MainClass.stoploss;
                                json = MainClass.bitMEXApi.CreateStopOrder(MainClass.pair, "Buy", Math.Abs(MainClass.positionContracts), SLValue, true, 0, "STOP LOSS MARKET ORDER");
                                MainClass.log(json);
                            }
                            else if (MainClass.positionContracts > 0)
                            {
                                double SLValue = scalpPrice - (scalpPrice / 100) * MainClass.stoploss;
                                json = MainClass.bitMEXApi.CreateStopOrder(MainClass.pair, "Sell", Math.Abs(MainClass.positionContracts), SLValue, true, 0, "STOP LOSS MARKET ORDER");
                                MainClass.log(json);
                            }
                        }
                    }
                    return;
                }

                if (SLOrders.Count > 0 && MainClass.positionContracts == 0 || Math.Abs(MainClass.positionContracts) == surfQty && Math.Abs(MainClass.bitMEXApi.GetOpenSLOrders(MainClass.pair).Count) > 0)
                {
                    MainClass.log("Canceling SL Orders as they are invalid ATM", ConsoleColor.Red);
                    MainClass.bitMEXApi.CancelAllSLOrders(MainClass.pair);
                }

                if ((MainClass.positionContracts == 0 || Math.Abs(MainClass.positionContracts) == surfQty) && lastFixPosOrder != "")
                {
                    Order fixpos = MainClass.bitMEXApi.GetOrderById(lastFixPosOrder);
                    if (fixpos.OrdStatus == "New" || fixpos.OrdStatus == "Open")
                    {
                        MainClass.log("Closing all orders 1");
                        MainClass.log(MainClass.positionContracts.ToString());
                        MainClass.bitMEXApi.CancelAllOpenOrders(MainClass.pair);
                        lastFixPosOrder = "";
                    }
                    return;
                }

                if (((MainClass.positionContracts == scalpQty || MainClass.positionContracts == MainClass.qtdyContacts) && OpenOrders == scalpQty)
                 || ((MainClass.positionContracts == (scalpQty * (-1)) || MainClass.positionContracts == (MainClass.qtdyContacts * (-1)) && OpenOrders < (scalpQty * (-1)))))
                {
                    MainClass.log("Closing all orders 2");
                    MainClass.log(MainClass.positionContracts.ToString());
                    MainClass.bitMEXApi.CancelAllLimitOrders(MainClass.pair);
                    lastScalpOrder = "";
                    return;
                }

            }
            catch (Exception e)
            {
                MainClass.log("Error Run SL OB Strategy: " + e.Message + e.StackTrace + e.ToString());
            }
        }
    }
}
