using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Botmex.Strategies
{
    public class Crosstrail : IStrategies
    {
        public static double pctdiff = 0.25;
        public static double sellTrail = 3;
        public static double leavecross = 20;
        public static int limitnow = 0;
        public static int limitnowretries = 4;
        public static int limitnowtimeout = 2;
        public static int closes = 0;
        public static int possibleCloses = 0;

        /* if macd */
        public static double macdInverse = 4;

        public void run()
        {
            MainClass.log("Running Strategy CrossTrail");
            StaticSetup();
            StaticRun();
        }


        public static void StaticSetup()
        {
            if (MainClass.strategyOptions.Count > 0)
            {
                if (MainClass.strategyOptions.ContainsKey("pctdiff".ToLower()))
                    pctdiff = double.Parse(MainClass.strategyOptions["pctdiff"]);

                if (MainClass.strategyOptions.ContainsKey("selltrail".ToLower()))
                    sellTrail = double.Parse(MainClass.strategyOptions["selltrail"]);

                if (MainClass.strategyOptions.ContainsKey("leavecross".ToLower()))
                    leavecross = double.Parse(MainClass.strategyOptions["leavecross"]);

                if (MainClass.strategyOptions.ContainsKey("limitretry".ToLower()))
                    limitnowretries = int.Parse(MainClass.strategyOptions["limitretry"]);
                
                if (MainClass.strategyOptions.ContainsKey("limittimeout".ToLower()))
                    limitnowtimeout = int.Parse(MainClass.strategyOptions["limittimeout"]);
        
                if (MainClass.strategyOptions.ContainsKey("limitnow".ToLower()))
                    limitnow = int.Parse(MainClass.strategyOptions["limitnow"]);

            }
        }

        public static void StaticRun()
        {

            PercentageTrailer();
            if (MainClass.statusLong == "enable")
            {
                MainClass.log("");
                MainClass.log("==========================================================");
                MainClass.log(" ==================== Verify LONG OPERATION =============", ConsoleColor.Green);
                MainClass.log("==========================================================");
                /////VERIFY OPERATION LONG
                Operation operation = Operation.buy;
                //VERIFY INDICATORS ENTRY
                foreach (var item in MainClass.lstIndicatorsEntry)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                    MainClass.log("Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Date: " + MainClass.arrayDate[MainClass.arrayPriceOpen[item.getTimegraph()].Length - 1]);
                    MainClass.log("Operation: " + op.ToString());
                    MainClass.log("");
                    if (op != Operation.buy)
                    {
                        operation = Operation.nothing;

                        break;
                    }
                }

                /* Verify Threshold Indicators*/
                if (MainClass.lstIndicatorsEntryThreshold.Count > 0 && operation == Operation.buy)
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
                            operation = Operation.nothing;
                            break;
                        }
                    }
                }

                //VERIFY INDICATORS CROSS
                if (MainClass.lstIndicatorsEntryCross.Count > 0 && operation == Operation.buy)
                {
                    foreach (var item in MainClass.lstIndicatorsEntryCross)
                    {
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                        MainClass.log("Indicator Cross: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + op.ToString());
                        MainClass.log("");

                        if ((item.getTypeIndicator() == TypeIndicator.Cross && op != Operation.buy) || op != Operation.buy)
                        {
                            operation = Operation.nothing;
                            break;
                        }
                    }
                }

                //EXECUTE OPERATION
                if (operation == Operation.buy)
                    makeOrder("Buy", false, "Normal/Surf Buy Order");

                ////////////FINAL VERIFY OPERATION LONG//////////////////
            }



            if (MainClass.statusShort == "enable")
            {

                //////////////////////////////////////////////////////////////
                MainClass.log("");
                MainClass.log("==========================================================");
                MainClass.log(" ==================== Verify SHORT OPERATION =============", ConsoleColor.Red);
                MainClass.log("==========================================================");
                /////VERIFY OPERATION SHORT
                Operation operation = Operation.sell;
                //VERIFY INDICATORS ENTRY
                foreach (var item in MainClass.lstIndicatorsEntry)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                    MainClass.log("Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + op.ToString());
                    MainClass.log("");
                    if (op != Operation.sell)
                    {
                        operation = Operation.nothing;
                        break;
                    }
                }

                if (MainClass.lstIndicatorsEntryThreshold.Count > 0 && operation == Operation.sell)
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
                            operation = Operation.nothing;
                            break;
                        }
                    }
                }

                if (MainClass.lstIndicatorsEntryCross.Count > 0 && operation == Operation.sell)
                {
                    foreach (var item in MainClass.lstIndicatorsEntryCross)
                    {
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen[item.getTimegraph()], MainClass.arrayPriceClose[item.getTimegraph()], MainClass.arrayPriceLow[item.getTimegraph()], MainClass.arrayPriceHigh[item.getTimegraph()], MainClass.arrayPriceVolume[item.getTimegraph()]);
                        MainClass.log("Indicator Cross: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + op.ToString());
                        MainClass.log("");

                        if ((item.getTypeIndicator() == TypeIndicator.Cross && op != Operation.sell) || op != Operation.sell)
                        {
                            operation = Operation.nothing;
                            break;
                        }
                    }
                }

                //EXECUTE OPERATION
                if (operation == Operation.sell)
                    makeOrder("Sell", false, "Normal/Surf Buy Order");

                ////////////FINAL VERIFY OPERATION LONG//////////////////
            }
        }

        public static void PercentageTrailer()
        {
            MainClass.log("Running Percentage Trailer");
            string json = "";
            int curContracts = MainClass.positionContracts;
            if (Math.Abs(curContracts) == 0)
            {
                return;
            }

            if (curContracts < 0)
            {
                int trailAmount = canTrail("Buy");
                if (trailAmount > 0)
                {
                    json = MainClass.bitMEXApi.MarketOrder(MainClass.pair, "Buy", trailAmount, "Percentage Trailer Buy");
                    MainClass.log(json);
                    if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                    {
                        return;
                    }
                    closes += possibleCloses;
                }

            }

            if (curContracts > 0)
            {
                int trailAmount = canTrail("Sell");
                if (trailAmount > 0)
                {
                    json = MainClass.bitMEXApi.MarketOrder(MainClass.pair, "Sell", trailAmount, "Percentage Trailer Sell");
                    MainClass.log(json);
                    if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                    {
                        return;
                    }
                    closes += possibleCloses;
                }
            }
        }

        public static int canTrail(string Side)
        {
            possibleCloses = 0;
            int desiredClose = 0;
            double priceActual = MainClass.getPriceActual(Side);
            MainClass.log("priceActual " + Side + " " + priceActual);
            double percPrice = ((priceActual * 100) / MainClass.positionPrice) - 100;
            MainClass.log("perc" + percPrice);

            if ((Side == "Buy" && percPrice > 0) || (Side == "Sell" && percPrice < 0) || double.IsInfinity(percPrice) || Math.Abs(percPrice) < pctdiff)
            {
                return desiredClose;
            }


            percPrice = Math.Abs(percPrice);
            string percString = percPrice.ToString();
            int percPriceDecimals = percString.Substring(percString.IndexOf(".") + 1).Length;
            percPrice = RoundDown(percPrice, percPriceDecimals);


            double perc = ((Math.Abs(MainClass.positionContracts) * 100) / MainClass.qtdyContacts);
            MainClass.log("perc " + perc);
            int maxTrail = (int)Math.Round(MainClass.qtdyContacts * (sellTrail / 100));
            MainClass.log("maxTrail " + maxTrail);
            int leaveQtdy = (int)Math.Round(MainClass.qtdyContacts * (leavecross / 100));
            MainClass.log("leaveqtdy " + leaveQtdy);
            int maxCloses = MainClass.qtdyContacts / maxTrail;

            MainClass.log("maxCloses " + maxCloses);
            if ((perc - leavecross) >= leavecross && (Math.Abs(MainClass.positionContracts) - maxTrail) >= leaveQtdy)
            {   /* We need to calculate how much we would close here */

                for (int i = 1; i <= maxCloses; i++)
                {
                    if ((percPrice * 100) >= ((pctdiff * 100) * i))
                    {
                        desiredClose += maxTrail;
                        possibleCloses++;
                    }
                    else
                        break;
                }

                if (closes > 0)
                {
                    for (int i = 1; i <= closes; i++)
                    {
                        desiredClose -= maxTrail;
                        possibleCloses--;
                    }
                }

                if (desiredClose > (Math.Abs(MainClass.positionContracts) - leaveQtdy))
                {
                    return (Math.Abs(MainClass.positionContracts) - leaveQtdy);
                }

                return desiredClose;

            }
            return desiredClose;
        }


        public static void makeOrder(string side, bool nothing = false, string text = "botmex", bool force = false, int qty = 0)
        {
            bool execute = false;
            try
            {
                MainClass.log(" wait 5s Make order " + side);
                double price = 0;
                String json = "";

                if (nothing)
                {
                    price = Math.Abs(MainClass.getPriceActual(side));
                    json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(qty), force, true, text);
                    return;
                }

                if (side == "Sell" && MainClass.statusShort == "enable" && Math.Abs(MainClass.limiteOrder) > Math.Abs(MainClass.bitMEXApi.GetOpenOrders(MainClass.pair).Count) && MainClass.getPosition() >= 0)
                {

                    price = Math.Abs(MainClass.getPriceActual(side));
                    json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(MainClass.getPosition()) + Math.Abs(MainClass.qtdyContacts), true, true, text);

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
                            if (MainClass.operation == "scalper")
                            {
                                price = price - MainClass.stepValue;
                            }
                            else
                            {
                                price -= (price * MainClass.profit) / 100;
                                price = Math.Abs(Math.Floor(price));
                            }

                            MainClass.log(json);
                            execute = true;
                            break;
                        }

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

                    price = Math.Abs(MainClass.getPriceActual(side));
                    json = MainClass.bitMEXApi.PostOrderPostOnly(MainClass.pair, side, price, Math.Abs(MainClass.getPosition()) + Math.Abs(MainClass.qtdyContacts), true, true, text);

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


                    MainClass.log("Long Order at: " + price);
                    JContainer config = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));

                    MainClass.log("wait total...");
                    for (int i = 0; i < MainClass.intervalCancelOrder; i++)
                    {
                        if (!MainClass.existsOrderOpenById(config["orderID"].ToString()) && !MainClass.existsOrderOpenById(config["orderID"].ToString()))
                        {

                            if (MainClass.operation == "scalper")
                            {
                                price = price + MainClass.stepValue;
                            }
                            else
                            {
                                price += (price * MainClass.profit) / 100;
                                price = Math.Abs(Math.Floor(price));
                            }

                            MainClass.log(json);
                            execute = true;
                            break;
                        }
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
                closes = 0;
                MainClass.log("wait " + MainClass.intervalOrder + "ms", ConsoleColor.Blue);
                Thread.Sleep(MainClass.intervalOrder);
            }
        }

        public static double RoundDown(double number, int decimalPlaces)
        {
            return Math.Floor(number * Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces);
        }
    }

}
