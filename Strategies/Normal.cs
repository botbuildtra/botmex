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
                Operation operation = Operation.buy;
                //VERIFY INDICATORS ENTRY
                foreach (var item in MainClass.lstIndicatorsEntry)
                {
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                    MainClass.log("Indicator: " + item.getName());
                    MainClass.log("Result1: " + item.getResult());
                    MainClass.log("Result2: " + item.getResult2());
                    MainClass.log("Date: " + MainClass.arrayDate[MainClass.arrayPriceOpen.Length - 1]);
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
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
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
                if ( MainClass.lstIndicatorsEntryCross.Count > 0 && operation == Operation.buy)
                {
                    foreach (var item in MainClass.lstIndicatorsEntryCross)
                    {
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
                        MainClass.log("Indicator Cross: " + item.getName());
                        MainClass.log("Result1: " + item.getResult());
                        MainClass.log("Result2: " + item.getResult2());
                        MainClass.log("Operation: " + op.ToString());
                        MainClass.log("");

                        if ( (item.getTypeIndicator() == TypeIndicator.Cross && op != Operation.buy) || op != Operation.buy) 
                        {
                            operation = Operation.nothing;
                            break;
                        }
                    }
                }


                #region indicatorsDecision
                /*
                if (MainClass.lstIndicatorsEntryDecision.Count > 0 && operation == Operation.buy)
                {

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
                                operation = 
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
                }*/
                #endregion


                //EXECUTE OPERATION
                if (operation == Operation.buy )
                    MainClass.makeOrder("Buy",false,"Normal/Surf Buy Order");

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
                    Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
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
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
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
                        Operation op = item.GetOperation(MainClass.arrayPriceOpen, MainClass.arrayPriceClose, MainClass.arrayPriceLow, MainClass.arrayPriceHigh, MainClass.arrayPriceVolume);
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

                #region indicators decision
                //VERIFY INDICATORS DECISION
                /*
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
                }*/
                #endregion

                //EXECUTE OPERATION
                if (operation == Operation.sell)
                    MainClass.makeOrder("Sell",false,"Normal/Surf Buy Order");

                ////////////FINAL VERIFY OPERATION LONG//////////////////
            }
        }
    }
}
