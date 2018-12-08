using System;
using System.Threading;
namespace BitBotBackToTheFuture.Strategies
{
    public static class Normal
    {
        public static void run()
        {
            if (MainClass.statusLong == "enable")
            {
                MainClass.log("");
                MainClass.log("==========================================================");
                MainClass.log(" ==================== Verify LONG OPERATION =============", ConsoleColor.Green);
                MainClass.log("==========================================================");
                /////VERIFY OPERATION LONG
                string operation = "buy";
                //VERIFY INDICATORS ENTRY
                foreach (var item in MainClass.lstIndicatorsEntry)
                {
                    Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                    MainClass.log("Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Date: " + MainClass.arrayDate[MainClass.arrayPriceOpen.Length - 1]);
                    MainClass.log("Operation: " + operationBuy.ToString());
                    MainClass.log("");
                    if (operationBuy != Operation.buy)
                    {
                        operation = "nothing";
                        MainClass.nothing = true;
                        break;
                    }
                }

                if (MainClass.lstIndicatorsEntryThreshold.Count > 0)
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
                            operation = "nothing";
                            MainClass.nothing = true;
                            break;
                        }
                    }
                }

                //VERIFY INDICATORS CROSS
                if (operation == "buy")
                {
                    //Prepare to long                        
                    while (true)
                    {
                        MainClass.log("wait operation long...");
                        MainClass.getCandles();
                        foreach (var item in MainClass.lstIndicatorsEntryCross)
                        {
                            Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                            MainClass.log("Indicator Cross: " + item.getName());
                            MainClass.log("Result1: " + item.getResult());
                            MainClass.log("Result2: " + item.getResult2());
                            MainClass.log("Operation: " + operationBuy.ToString());
                            MainClass.log("");

                            if (item.getTypeIndicator() == TypeIndicator.Cross)
                            {
                                if (operationBuy == Operation.buy)
                                {
                                    operation = "long";
                                    break;
                                }
                            }
                            else if (operationBuy != Operation.buy)
                            {
                                operation = "long";
                                break;
                            }
                        }
                        if (MainClass.lstIndicatorsEntryCross.Count == 0)
                            operation = "long";
                        if (operation != "buy")
                            break;

                        MainClass.log("wait " + MainClass.interval + "ms");
                        Thread.Sleep(MainClass.interval);


                    }
                }

                //VERIFY INDICATORS DECISION
                if (operation == "long" && MainClass.lstIndicatorsEntryDecision.Count > 0)
                {
                    operation = "decision";
                    foreach (var item in MainClass.lstIndicatorsEntryDecision)
                    {
                        Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                        MainClass.log("Indicator Decision: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + operationBuy.ToString());
                        MainClass.log("");



                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" && MainClass.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")

                        {
                            int decisionPoint = int.Parse(MainClass.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                            if (item.getResult() >= decisionPoint && item.getTendency() == Tendency.high)
                            {
                                operation = "long";
                                break;
                            }
                        }


                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")

                        {
                            int decisionPoint = int.Parse(MainClass.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                            if (item.getResult() >= decisionPoint)
                            {
                                operation = "long";
                                break;
                            }
                        }

                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")

                        {
                            if (item.getTendency() == Tendency.high)
                            {
                                operation = "long";
                                break;
                            }
                        }

                    }
                }


                //EXECUTE OPERATION
                if (operation == "long" )
                    MainClass.makeOrder("Buy");

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
                string operation = "sell";
                //VERIFY INDICATORS ENTRY
                foreach (var item in MainClass.lstIndicatorsEntry)
                {
                    Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                    MainClass.log("Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Operation: " + operationBuy.ToString());
                    MainClass.log("");
                    if (operationBuy != Operation.sell)
                    {
                        operation = "nothing";
                        MainClass.nothing = true;
                        break;
                    }
                }

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
                            operation = "nothing";
                            MainClass.nothing = true;
                            break;
                        }
                    }
                }

                //VERIFY INDICATORS CROSS
                if (operation == "sell")
                {
                    //Prepare to long                        
                    while (true)
                    {
                        MainClass.log("wait operation short...");
                        MainClass.getCandles();
                        foreach (var item in MainClass.lstIndicatorsEntryCross)
                        {
                            Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                            MainClass.log("Indicator Cross: " + item.getName());
                            MainClass.log("Result1: " + item.getResult());
                            MainClass.log("Result2: " + item.getResult2());
                            MainClass.log("Operation: " + operationBuy.ToString());
                            MainClass.log("");

                            if (item.getTypeIndicator() == TypeIndicator.Cross)
                            {
                                if (operationBuy == Operation.sell)
                                {
                                    operation = "short";
                                    break;
                                }
                            }
                            else if (operationBuy != Operation.sell)
                            {
                                operation = "short";
                                break;
                            }
                        }
                        if (MainClass.lstIndicatorsEntryCross.Count == 0)
                            operation = "short";
                        if (operation != "sell")
                            break;

                        MainClass.log("wait " + MainClass.interval + "ms");
                        Thread.Sleep(MainClass.interval);


                    }
                }

                #region indicators decision
                //VERIFY INDICATORS DECISION
                if (operation == "short" && MainClass.lstIndicatorsEntryDecision.Count > 0)
                {
                    operation = "decision";
                    foreach (var item in MainClass.lstIndicatorsEntryDecision)
                    {
                        Operation operationBuy = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                        MainClass.log("Indicator Decision: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + operationBuy.ToString());
                        MainClass.log("");



                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable" && MainClass.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")

                        {
                            int decisionPoint = int.Parse(MainClass.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                            if (item.getResult() <= decisionPoint && item.getTendency() == Tendency.low)
                            {
                                operation = "short";
                                break;
                            }
                        }


                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "decision") == "enable")

                        {
                            int decisionPoint = int.Parse(MainClass.getValue("indicatorsEntryDecision", item.getName(), "decisionPoint"));
                            if (item.getResult() <= decisionPoint)
                            {
                                operation = "short";
                                break;
                            }
                        }

                        if (MainClass.getValue("indicatorsEntryDecision", item.getName(), "tendency") == "enable")

                        {
                            if (item.getTendency() == Tendency.low)
                            {
                                operation = "short";
                                break;
                            }
                        }

                    }
                }
                #endregion

                //EXECUTE OPERATION
                if (operation == "short")
                    MainClass.makeOrder("Sell");

                ////////////FINAL VERIFY OPERATION LONG//////////////////
            }
        }
    }
}
