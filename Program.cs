using Botmex;
using Botmex.Strategies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

public enum TendencyMarket
{
    HIGH,
    NORMAL,
    LOW,
    VERY_LOW,
    VERY_HIGH
}

public class MainClass
{
    public static string version = "0.0.4.0";
    public static string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + System.IO.Path.DirectorySeparatorChar;
    public static bool debug = false;
    public static string bitmexKey = "";
    public static string bitmexSecret = "";
    public static string bitmexWebSocketDomain = "";
    public static string bitmexKeyWebSocket = "";
    public static string bitmexSecretWebSocket = "";
    public static string timeGraph = "";
    public static string statusShort = "";
    public static string statusLong = "";
    public static string pair = "";
    public static int qtdyContacts = 0;
    public static int interval = 0;
    public static int intervalOrder = 0;
    public static int intervalCapture = 0;
    public static int intervalCancelOrder = 30;
    public static int positionContracts = 0;
    public static double profit = 0;
    public static int limiteOrder = 0;
    public static double fee = 0;
    public static double stoploss = 10;
    public static string stoplosstype = "orderbook"; // orderbook - bot
    public static double stopgain = 15;
    public static string bitmexDomain = "";
    public static string operation = "normal"; // normal - scalper - surf - bingo - scalperv2
    public static bool roeAutomatic = true;
    public static bool usedb = false;
    public static bool tendencyBook = false;
    public static double traillingProfit = -1;
    public static double traillingStop = -1;
    public static bool carolatr = false;
    public static double atrvalue = 6;
    public static bool marketTaker = false;
    public static double obDiff = 10;
    public static bool apiDebug = false;
    public static double stepValue = 0.5;
    public static TendencyMarket tendencyMarket = TendencyMarket.NORMAL;
    public static BitMEX.BitMEXApi bitMEXApi = null;

    public static List<IIndicator> lstIndicatorsAll = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntry = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntryCross = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsEntryThreshold = new List<IIndicator>();
    public static List<IIndicator> lstIndicatorsInvert = new List<IIndicator>();
    public static Dictionary<string, string> strategyOptions = new Dictionary<string, string>();

    public static double positionPrice = 0;
    public static int sizeArrayCandles = 500;

    public static Dictionary<string, double[]> arrayPriceClose = new Dictionary<string, double[]>();
    public static Dictionary<string, double[]> arrayPriceHigh = new Dictionary<string, double[]>();
    public static Dictionary<string, double[]> arrayPriceLow = new Dictionary<string, double[]>();
    public static Dictionary<string, double[]> arrayPriceVolume = new Dictionary<string, double[]>();
    public static Dictionary<string, double[]> arrayPriceOpen = new Dictionary<string, double[]>();
    /*public static double[] arrayPriceClose = new double[sizeArrayCandles];
    public static double[] arrayPriceHigh = new double[sizeArrayCandles];
    public static double[] arrayPriceLow = new double[sizeArrayCandles];
    public static double[] arrayPriceVolume = new double[sizeArrayCandles];
    public static double[] arrayPriceOpen = new double[sizeArrayCandles];*/
    public static DateTime[] arrayDate = new DateTime[sizeArrayCandles];
    public static bool useWebSockets;
    public static int stoplossInterval = 4;
    public static int maxWebsocketsFail = 3;
    public static Thread wss;
    public static Object data = new Object();
    public static JContainer sharedConfig;
    public static bool SLRunning = false;
    public static bool RunTrigger = false;
    public static void Main(string[] args)
    {
        IStrategies strategy = null;
        try
        {
            //Config
            Console.Title = "Loading...";

            Console.ForegroundColor = ConsoleColor.White;

            log("Krampus - Liquidador do saldo - v" + version);
            log("by Matheus Grijo ", ConsoleColor.Green);
            log(" ======= HALL OF FAME BOTMEX  ======= ");
            log(" - Lucas Sousa", ConsoleColor.Magenta);
            log(" - Carlos Morato", ConsoleColor.Magenta);
            log(" - Luis Felipe Alves", ConsoleColor.Magenta);
            log(" - O Zuca que toda a gente pensa que está Bebendo", ConsoleColor.Green);
            log(" - O Portuga da Maconha", ConsoleColor.Red);
            log(" ======= END HALL OF FAME BOTMEX  ======= ");

            log("http://botmex.ninja/");
            log("GITHUB http://github.com/tperalta82/botmex", ConsoleColor.Blue);
            log(" ******* DONATE ********* ");
            log("BTC: 3NoXn5PHyBAxxQAc2LCruWoEhthWsAdSR8");
            log("LTC: MKPZ2XZep3pTfggjBpiSgjRofDAEbBC8qp ");
            log("ETH: 0x172bdb1ab580128d42993c00a60AF99b726eaF81");
            log("Load config...");
            log("Considere DOAR para o projeto, senão oh, fico sem maconha e as ideias boas vão com o caralho!", ConsoleColor.Green);

            String jsonConfig = System.IO.File.ReadAllText(location + "key.json");
            JContainer config = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));
            sharedConfig = config;
            usedb = config["usedb"].ToString() == "enable";

            bitmexKey = config["key"].ToString();
            bitmexSecret = config["secret"].ToString();
            bitmexWebSocketDomain = config["websocketDomain"].ToString();
            bitmexKeyWebSocket = config["websocketKey"].ToString();
            bitmexSecretWebSocket = config["websocketSecret"].ToString();
            bitmexDomain = config["domain"].ToString();
            statusShort = config["short"].ToString();
            statusLong = config["long"].ToString();
            pair = config["pair"].ToString();
            ClassDB.strConn = config["dbcon"].ToString();
            ClassDB.dbquery = config["dbquery"].ToString();
            timeGraph = config["timeGraph"].ToString();
            qtdyContacts = int.Parse(config["contract"].ToString());
            interval = int.Parse(config["interval"].ToString());
            intervalCancelOrder = int.Parse(config["intervalCancelOrder"].ToString());
            intervalOrder = int.Parse(config["intervalOrder"].ToString());
            intervalCapture = int.Parse(config["webserverIntervalCapture"].ToString());
            profit = double.Parse(config["profit"].ToString());
            fee = double.Parse(config["fee"].ToString());
            stoploss = double.Parse(config["stoploss"].ToString());
            stepValue = double.Parse(config["stepvalue"].ToString());
            stopgain = double.Parse(config["stopgain"].ToString());
            roeAutomatic = config["roe"].ToString() == "automatic";
            tendencyBook = config["tendencyBook"].ToString() == "enable";
            operation = config["operation"].ToString();
            limiteOrder = int.Parse(config["limiteOrder"].ToString());
            carolatr = config["carolatr"].ToString() == "enable";
            atrvalue = double.Parse(config["atrvalue"].ToString());
            apiDebug = bool.Parse(config["apidebug"].ToString());
            bitMEXApi = new BitMEX.BitMEXApi(bitmexKey, bitmexSecret, bitmexDomain,5000,apiDebug);
            marketTaker = config["marketTaker"].ToString() == "enable";
            obDiff = double.Parse(config["obDiff"].ToString());
            stoplosstype = config["stoplosstype"].ToString();
            stoplossInterval = int.Parse(config["stoplossInterval"].ToString());

            if (config["webserver"].ToString() == "enable")
            {
                WebServer ws = new WebServer(WebServer.SendResponse, config["webserverConfig"].ToString());
                ws.Run();
                System.Threading.Thread tCapture = new Thread(Database.captureDataJob);
                tCapture.Start();
                System.Threading.Thread.Sleep(1000);
                OperatingSystem os = Environment.OSVersion;
                PlatformID pid = os.Platform;
                if (pid != PlatformID.Unix)
                {
                    System.Diagnostics.Process.Start(config["webserverConfig"].ToString());
                }
            }



            log("Total open orders: " + bitMEXApi.GetOpenOrders(pair).Count);

            log("");
            log("Wallet: " + bitMEXApi.GetWallet());

            foreach (var item in config["indicatorsEntry"])
            {
                IIndicator ind = LoadIndicator(item["name"].ToString().Trim().ToUpper());
                Dictionary<string, string> cfg = new Dictionary<string, string>();
                foreach (JProperty cfgitem in item)
                {
                    cfg.Add(cfgitem.Name.ToString(), cfgitem.Value.ToString());

                }
                ind.Setup(cfg);
                lstIndicatorsEntry.Add(ind);
            }

            foreach (var item in config["indicatorsEntryCross"])
            {
                IIndicator ind = LoadIndicator(item["name"].ToString().Trim().ToUpper());
                Dictionary<string, string> cfg = new Dictionary<string, string>();
                foreach (JProperty cfgitem in item)
                {
                    cfg.Add(cfgitem.Name.ToString(), cfgitem.Value.ToString());

                }
                ind.Setup(cfg);
                lstIndicatorsEntryCross.Add(ind);
            }

            foreach (var item in config["indicatorsEntryThreshold"])
            {
                IIndicator ind = LoadIndicator(item["name"].ToString().Trim().ToUpper());
                Dictionary<string, string> cfg = new Dictionary<string, string>();
                foreach (JProperty cfgitem in item)
                {
                    cfg.Add(cfgitem.Name.ToString(), cfgitem.Value.ToString());

                }
                ind.Setup(cfg);
                lstIndicatorsEntryThreshold.Add(ind);
            }

            foreach (var item in config["indicatorsInvert"])
            {
                IIndicator ind = LoadIndicator(item["name"].ToString().Trim().ToUpper());
                Dictionary<string, string> cfg = new Dictionary<string, string>();
                foreach (JProperty cfgitem in item)
                {
                    cfg.Add(cfgitem.Name.ToString(), cfgitem.Value.ToString());

                }
                ind.Setup(cfg);
                lstIndicatorsInvert.Add(ind);
            }

            foreach (var item in config["strategyOptions"])
            {
                strategyOptions[item["name"].ToString().Trim().ToLower()] = item["value"].ToString().Trim();
            }

            bool automaticTendency = statusLong == "automatic";

            /* Get initial candles */
            getCandles("1m", false);
            getCandles("5m", false);
            getCandles("1h", false);
            Thread.Sleep(2000);

            Timer wsHandle = new Timer(handleWebsockets, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));


            if (operation == "manual")
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }

            //Threaded SL
            if(stoplosstype != "strategy")
            {
                Timer slt = new Timer(runSL, null, TimeSpan.Zero, TimeSpan.FromSeconds(stoplossInterval));
            }

            //TESTS HERE
            if(operation == "debug")
            {
                tests();
                System.Environment.Exit(1);
            }

            //FINAL
            //LOOP 
            while (true)
            {
                try
                {
                    if ( ( useWebSockets && RunTrigger ) || !useWebSockets )
                    {
                        positionContracts = getPosition(); // FIX CARLOS MORATO                                            
                        positionPrice = 0;

                        if (positionContracts != 0)
                            positionPrice = getPositionPrice();

                        log("positionContracts " + positionContracts);
                        log("positionPrice " + positionPrice);

                        #region "Fix position not orders
                        if (operation == "normal" || operation == "scalper" || operation == "scalperv2" || operation == "bingo")
                            fixOrdersPosition();

                        #endregion

                        if (automaticTendency)
                            verifyTendency();

                        //GET CANDLES
                        if (useWebSockets || getCandles(timeGraph, false, true))
                        {
                            if (operation == "normal" || operation == "surf")
                            {
                                Normal.run();
                            }
                            else if (operation == "scalper")
                            {
                                Scalper.run();
                            }
                            else if (operation == "scalperv2")
                            {
                                ScalperV2.run();
                            }
                            else if (operation == "bingo")
                            {
                                Bingo.run();
                            }
                            else
                            {
                                if (strategy == null)
                                {
                                    MainClass.log("Loading Strategies");
                                    string[] strategies = Directory.GetFiles(location, "Strategy*.dll");
                                    Type strat = Type.GetType("Botmex.Strategies." + operation.First().ToString().ToUpper() + operation.Substring(1));
                                    log(operation.First().ToString().ToUpper() + operation.Substring(1));
                                    foreach (string dllStrategy in strategies)
                                    {
                                        log(dllStrategy);
                                        try
                                        {
                                            log("Trying to load Strategy: " + dllStrategy);
                                            var assembly = Assembly.LoadFile(@dllStrategy);
                                            Type[] types = assembly.GetTypes();
                                            foreach (Type type in types)
                                            {
                                                if (type.ToString().Equals("Botmex.Strategies." + operation.First().ToString().ToUpper() + operation.Substring(1)))
                                                {
                                                    strat = assembly.GetType("Botmex.Strategies." + operation.First().ToString().ToUpper() + operation.Substring(1));
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            log("Cold not load strategy: " + dllStrategy + ex.ToString());
                                        }
                                    }


                                    if (strat == null)
                                    {
                                        log("Estrategia não encontrada", ConsoleColor.Red);
                                        System.Environment.Exit(1);
                                    }
                                    strategy = (IStrategies)Activator.CreateInstance(strat);
                                }
                                strategy.run();
                            }
                        }
                        RunTrigger = false;
                    }

                    if(useWebSockets)
                    {
                        Thread.Sleep(20);
                    }
                    else
                    {
                        log("wait " + interval + "ms", ConsoleColor.Blue);
                        Thread.Sleep(interval);
                    }

                }
                catch (Exception ex)
                {
                    RunTrigger = false;
                    log("while true::" + ex.Message + ex.StackTrace);
                }
            }

        }
        catch (Exception ex)
        {
            log("ERROR FATAL::" + ex.Message + ex.StackTrace);
            Console.ReadLine();
        }
    }

    public static bool existsOrderOpenById(string id)
    {
        List<BitMEX.Order> lst = bitMEXApi.GetOpenOrders(pair);
        foreach (var item in lst)
        {
            if (item.OrderId.ToUpper().Trim() == id.ToUpper().Trim())
                return true;
        }
        return false;
    }


    public static void makeOrder(string side, bool nothing = false, string text = "botmex", bool force = false)
    {
        bool execute = false;
        try
        {
            log(" wait 5s Make order " + side);
            double price = 0;
            String json = "";

            if (nothing)
            {
                price = Math.Abs(getPriceActual(side));
                json = bitMEXApi.PostOrderPostOnly(pair, side, price, Math.Abs(qtdyContacts), force, marketTaker, text);
                return;
            }

            if (side == "Sell" && statusShort == "enable" && Math.Abs(limiteOrder) > Math.Abs(bitMEXApi.GetOpenOrders(pair).Count) && getPosition() >= 0)
            {


                if (tendencyBook)
                    if (getTendencyOrderBook() != Tendency.low)
                        return;


                if (operation == "surf" || operation == "normal")
                {
                    price = Math.Abs(getPriceActual(side));
                    json = bitMEXApi.PostOrderPostOnly(pair, side, price + obDiff, Math.Abs(getPosition()) + Math.Abs(qtdyContacts), true, marketTaker, text);
                }

                log(json);

                if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                {
                    if (json.ToLower().IndexOf("overload") >= 0)
                    {/* In case of overload, close the position to prevent losses*/
                        log("System is on overload, trying to close position to prevent losses");
                        log(bitMEXApi.MarketClose(pair, side));
                    }
                    return;
                }


                log("Short Order at: " + price);

                JContainer config = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));


                for (int i = 0; i < intervalCancelOrder; i++)
                {
                    if (!existsOrderOpenById(config["orderID"].ToString()) && !existsOrderOpenById(config["orderID"].ToString()))
                    {
                        if (operation == "scalper")
                        {
                            price = price - stepValue;
                        }
                        else
                        {
                            price -= (price * profit) / 100;
                            price = Math.Abs(Math.Floor(price));
                        }

                        traillingProfit = -1;
                        traillingStop = -1;

                        log(json);
                        execute = true;
                        break;
                    }
                    log("wait order limit " + i + " of " + intervalCancelOrder + "...");
                    if (operation != "scalper")
                        Thread.Sleep(800);
                }

                if (!execute)
                {
                    bitMEXApi.DeleteOrders(config["orderID"].ToString());
                    while (existsOrderOpenById(config["orderID"].ToString()))
                        bitMEXApi.DeleteOrders(config["orderID"].ToString());
                    log("Cancel order ID " + config["orderID"].ToString());
                }

            }

            if (side == "Buy" && statusLong == "enable" && Math.Abs(limiteOrder) > Math.Abs(bitMEXApi.GetOpenOrders(pair).Count) && getPosition() <= 0)
            {

                if (tendencyBook)
                    if (getTendencyOrderBook() != Tendency.high)
                        return;


                price = 0;
                json = "";
                if (operation == "surf" || operation == "normal")
                {
                    price = Math.Abs(getPriceActual(side));
                    json = bitMEXApi.PostOrderPostOnly(pair, side, price - obDiff, Math.Abs(getPosition()) + Math.Abs(qtdyContacts), true, marketTaker, text);
                }


                log(json);
                if (json.ToLower().IndexOf("error") >= 0 || json.ToLower().IndexOf("canceled") >= 0)
                {
                    if(json.ToLower().IndexOf("overload") >= 0)
                    {/* In case of overload, close the position to prevent losses*/
                        log("System is on overload, trying to close position to prevent losses");
                        log(bitMEXApi.MarketClose(pair, side));
                    }
                    return;
                }


                log("Long Order at: " + price);
                JContainer config = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));

                log("wait total...");
                for (int i = 0; i < intervalCancelOrder; i++)
                {
                    if (!existsOrderOpenById(config["orderID"].ToString()) && !existsOrderOpenById(config["orderID"].ToString()))
                    {

                        if (operation == "scalper")
                        {
                            price = price + stepValue;
                        }
                        else
                        {
                            price += (price * profit) / 100;
                            price = Math.Abs(Math.Floor(price));
                        }

                        traillingProfit = -1;
                        traillingStop = -1;
                        log(json);
                        execute = true;
                        break;
                    }
                    log("wait order limit " + i + " of " + intervalCancelOrder + "...");
                    if (operation != "scalper")
                        Thread.Sleep(1000);
                }


                if (!execute)
                {
                    bitMEXApi.DeleteOrders(config["orderID"].ToString());
                    while (existsOrderOpenById(config["orderID"].ToString()))
                        bitMEXApi.DeleteOrders(config["orderID"].ToString());
                    log("Cancel order ID " + config["orderID"].ToString());
                }


            }
        }
        catch (Exception ex)
        {
            log("makeOrder()::" + ex.Message + ex.StackTrace);
        }

        if (execute)
        {

            log("wait " + intervalOrder + "ms", ConsoleColor.Blue);
            Thread.Sleep(intervalOrder);


        }
    }

    public static void verifyTendency()
    {
        try
        {
            String json = Http.get("https://api.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT");
            JContainer j = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);

            tendencyMarket = TendencyMarket.NORMAL;
            decimal priceChangePercent = decimal.Parse(j["priceChangePercent"].ToString().Replace(".", ","));
            if (priceChangePercent < -1.0m)
                tendencyMarket = TendencyMarket.LOW;
            if (priceChangePercent > -1.0m && priceChangePercent < 1.5m)
                tendencyMarket = TendencyMarket.NORMAL;
            if (priceChangePercent > 1.5m)
                tendencyMarket = TendencyMarket.HIGH;
            if (priceChangePercent < -2)
                tendencyMarket = TendencyMarket.VERY_LOW;
            if (priceChangePercent > 3.5m)
                tendencyMarket = TendencyMarket.VERY_HIGH;


            if (tendencyMarket == TendencyMarket.VERY_HIGH || tendencyMarket == TendencyMarket.HIGH)
            {
                statusShort = "disable";
                statusLong = "enable";
            }
            else if (tendencyMarket == TendencyMarket.NORMAL)
            {
                statusShort = "enable";
                statusLong = "enable";
            }
            else if (tendencyMarket == TendencyMarket.LOW || tendencyMarket == TendencyMarket.VERY_LOW)
            {
                statusShort = "enable";
                statusLong = "disable";
            }

            //if (tendencyMarket == TendencyMarket.VERY_HIGH || tendencyMarket == TendencyMarket.VERY_LOW)
            //  timeGraph = "1m";
            //else
            //  timeGraph = "5m";
        }
        catch (Exception ex)
        {
            throw new Exception("verifyTendency::" + ex.Message + ex.StackTrace);
        }
    }


    public static double getPriceActual(string type)
    {
        if( MainClass.useWebSockets)
        {
            if (type == "Buy")
                return WebSocket.BitmexWS.priceSideBuy;
            if (type == "Sell")
                return WebSocket.BitmexWS.priceSideSell;
        }
        try
        {
            List<BitMEX.OrderBook> listBook = bitMEXApi.GetOrderBook(pair, 1);
            foreach (var item in listBook)
            {
                if (item.Side.ToUpper() == type.ToUpper())
                    return item.Price;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("getPriceActual::" + ex.Message + ex.StackTrace);
        }
        /* this will be fixed on the refactored version but should be ok now*/
        return 0;
    }

    public static bool getCandles(string timeGraph = "5m", bool partial = false, bool initial = true)
    {
        try
        {
            if (!initial && useWebSockets)
            {
                Console.Title = DateTime.Now.ToString() + " - " + pair + " - $ " + arrayPriceClose[timeGraph][499].ToString() + " v" + version + " - " + bitmexDomain + " | " + tendencyMarket + "| operation " + operation;
                return true;
            }

            arrayPriceClose[timeGraph] = new double[sizeArrayCandles];
            //arrayPriceClose = new double[sizeArrayCandles];
            arrayPriceHigh[timeGraph] = new double[sizeArrayCandles];
            arrayPriceLow[timeGraph] = new double[sizeArrayCandles];
            arrayPriceVolume[timeGraph] = new double[sizeArrayCandles];
            arrayPriceOpen[timeGraph] = new double[sizeArrayCandles];
            arrayDate = new DateTime[sizeArrayCandles];
            List<BitMEX.Candle> lstCandle = bitMEXApi.GetCandleHistory(pair, sizeArrayCandles, timeGraph, partial);
            int i = 0;
            foreach (var candle in lstCandle)
            {
                arrayPriceClose[timeGraph][i] = (double)candle.close;
                arrayPriceHigh[timeGraph][i] = (double)candle.high;
                arrayPriceLow[timeGraph][i] = (double)candle.low;
                arrayPriceVolume[timeGraph][i] = (double)candle.volume;
                arrayPriceOpen[timeGraph][i] = (double)candle.open;
                arrayDate[i] = (DateTime)candle.TimeStamp;
                i++;
            }

            Array.Reverse(arrayPriceClose[timeGraph]);
            Array.Reverse(arrayPriceHigh[timeGraph]);
            Array.Reverse(arrayPriceLow[timeGraph]);
            Array.Reverse(arrayPriceVolume[timeGraph]);
            Array.Reverse(arrayPriceOpen[timeGraph]);
            Array.Reverse(arrayDate);


            Console.Title = DateTime.Now.ToString() + " - " + pair + " - $ " + arrayPriceClose[timeGraph][499].ToString() + " v" + version + " - " + bitmexDomain + " | " + tendencyMarket + "| operation " + operation;
            return true;
        }
        catch (Exception ex)
        {
            log("GETCANDLES::" + ex.Message + ex.StackTrace, ConsoleColor.White,"error");
            //log("wait " + intervalOrder + "ms");
            //Thread.Sleep(intervalOrder);
            return false;
        }

    }

    //By Lucas Sousa modify MatheusGrijo
    public static int getPosition()
    {
        try
        {
            log("getPosition...");
            List<BitMEX.Position> OpenPositions = bitMEXApi.GetOpenPositions(pair);
            int _qtdContacts = 0;
            foreach (var Position in OpenPositions)
                _qtdContacts += (int)Position.CurrentQty;
            log("getPosition: " + _qtdContacts);
            return _qtdContacts;
        }
        catch (Exception ex)
        {
            log("getPosition::" + ex.Message + ex.StackTrace);
            throw new Exception("Error getPosition");
        }
    }

    public static double getRoe()
    {
        try
        {
            log("getRoe...");
            List<BitMEX.Position> OpenPositions = bitMEXApi.GetOpenPositions(pair);
            double _roe = 0;
            foreach (var Position in OpenPositions)
                _roe += Position.percentual();
            log("getRoe: " + _roe);
            return _roe;
        }
        catch (Exception ex)
        {
            log("getRoe::" + ex.Message + ex.StackTrace);
            throw new Exception("Error getRoe");
        }
    }

    //GetOpenOrderQty
    //by Carlos Morato
    public static int getOpenOrderQty()
    {
        try
        {
            string orderid;
            List<BitMEX.Order> OpenOrderQty = bitMEXApi.GetOpenOrders(pair);
            int _contactsQty = 0;
            foreach (var Order in OpenOrderQty)
            if( Order.OrdType != "Stop" && Order.OrdType != "StopLimit" )
            {
                orderid = Order.OrderId;
                if (Order.Side == "Sell")
                    _contactsQty += (int)Order.OrderQty * (-1);
                else
                    _contactsQty += (int)Order.OrderQty;
            }
            return _contactsQty;
        }
        catch (Exception ex)
        {
            throw new Exception("getOpenOrderQty:: " + ex.Message + ex.StackTrace);
        }
    }

    public static string getOpenOrderId()
    {
        try
        {
            string orderid = "";
            List<BitMEX.Order> OpenOrderQty = bitMEXApi.GetOpenOrders(pair);
            foreach (var Order in OpenOrderQty)
                if (Order.OrdType != "Stop" && Order.OrdType != "StopLimit")
                {
                    return Order.OrderId;
                    /*if (Order.Side == "Sell")
                        _contactsQty += (int)Order.OrderQty * (-1);
                    else
                        _contactsQty += (int)Order.OrderQty;*/
                }
            return orderid;
        }
        catch (Exception ex)
        {
            throw new Exception("getOpenOrderQty:: " + ex.Message + ex.StackTrace);
        }
    }

    public static int getOpenSLOrderQty()
    {
        try
        {
            List<BitMEX.Order> OpenOrderQty = bitMEXApi.GetOpenOrders(pair);
            int _contactsQty = 0;
            foreach (var Order in OpenOrderQty)
                if( Order.OrdType == "Stop" || Order.OrdType == "StopLimit" )
                {
                    if (Order.Side == "Sell")
                        _contactsQty += (int)Order.OrderQty * (-1);
                    else
                        _contactsQty += (int)Order.OrderQty;
                }
                return _contactsQty;
        }
        catch (Exception ex)
        {
            throw new Exception("getOpenSLOrderQty:: " + ex.Message + ex.StackTrace);
        }
    }


    //GetPositionPrice
    //by Carlos Morato
    public static double getPositionPrice()
    {
        try
        {
            List<BitMEX.Position> OpenPositionsPrice = bitMEXApi.GetOpenPositions(pair);
            double _priceContacts = 0;
            foreach (var Position in OpenPositionsPrice)
                _priceContacts = (double)Position.AvgEntryPrice;
            return _priceContacts;
        }
        catch (Exception ex)
        {
            throw new Exception("getPositionPrice::" + ex.Message + ex.StackTrace);
        }
    }

    public static string getValue(String nameList, String nameIndicator, String nameParameter)
    {
        String jsonConfig = System.IO.File.ReadAllText(location + "key.json");
        JContainer config = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));
        foreach (var item in config[nameList])
            if (item["name"].ToString().Trim().ToUpper() == nameIndicator.ToUpper().Trim())
                return item[nameParameter].ToString().Trim();
        return null;
    }

    public static void log(string value, ConsoleColor color = ConsoleColor.White, string type = "main")
    {
        try
        {

            value = "[" + DateTime.Now.ToString() + "] - " + value;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;
            System.IO.StreamWriter w;
            if (type == "error")
            {
                w = new StreamWriter(location + DateTime.Now.ToString("yyyyMMdd") + "error_log.txt", true);
            }
            else if (type == "api")
            {
                w = new StreamWriter(location + DateTime.Now.ToString("yyyyMMdd") + "_api_log.txt", true);
            }
            else
            {
                w = new StreamWriter(location + DateTime.Now.ToString("yyyyMMdd") + "_log.txt", true);
            }
            w.WriteLine(value);
            w.Close();
            w.Dispose();

        }
        catch { }
    }



    public static Tendency getTendencyOrderBook()
    {
        try
        {
            List<BitMEX.OrderBook> lstOrderBook = bitMEXApi.GetOrderBook(pair, 100);
            int totalBuy = 0;
            int totalSell = 0;
            foreach (var item in lstOrderBook)
            {
                if (item.Side == "Buy")
                    totalBuy += Math.Abs(item.Size);
                if (item.Side == "Sell")
                    totalSell += Math.Abs(item.Size);
            }


            log("totalBuy " + totalBuy);
            log("totalSell " + totalSell);

            if (totalBuy > totalSell)
                return Tendency.high;
            else if (totalBuy < totalSell)
                return Tendency.low;
            else
                return Tendency.nothing;
        }
        catch
        {
            return Tendency.nothing;
        }
    }


    public static void handleWebsockets(object state)
    {
        //useWebSockets = false;
        if (bitmexWebSocketDomain != "")
        {
            if(wss == null)
            {
                wss = new Thread(WebSocket.BitmexWS.run);
                wss.Start();
                useWebSockets = true;
                log("Sleeping 4s to get initial data", ConsoleColor.Cyan);
                Thread.Sleep(4000);
            }
            else if (!wss.IsAlive)
            {
                wss = new Thread(WebSocket.BitmexWS.run);
                wss.Start();
                useWebSockets = true;
                log("Retrying connection to websockets...", ConsoleColor.Red);
                Thread.Sleep(4000);
            }

        }
    }

    public static void runSL(object state)
    {
        try
        {
            SLRunning = true;
            log("Checking for possible Stop Losses", ConsoleColor.Red);
            if (stoplosstype == "orderbook")
            {
                StopLoss.OrderBook.run();
            }
            else if (stoplosstype == "bot")
            {
                StopLoss.MarketClose.run();
            }
            else
            {
                log("Invalid Stop Loss Setting, EXITING", ConsoleColor.Red);
                System.Environment.Exit(1);
            }
            SLRunning = false;
        }
        catch(Exception e)
        {
            SLRunning = false;
            log("Error in the STOP Loss Engine");
            log(e.Message + " " + e.StackTrace + " " + e.ToString(), ConsoleColor.White, "error");
        }

    }

    public static void fixOrdersPosition(bool force = true)
    {
        int oOrders = getOpenOrderQty();
        if (operation != "surf" && roeAutomatic && (Math.Abs(oOrders) < Math.Abs(positionContracts)))
        {
            positionContracts = getPosition(); // FIX CARLOS MORATO                                            
            log("Get Position " + positionContracts);


            int qntContacts = (Math.Abs(positionContracts) - Math.Abs(getOpenOrderQty()));


            if (positionContracts > 0)
            {

                string side = "Sell";
                double priceContacts = Math.Abs(getPositionPrice());
                double actualPrice = Math.Abs(getPriceActual(side));
                double priceContactsProfit = Math.Abs(Math.Floor(priceContacts + (priceContacts * (profit + fee) / 100)));



                double price = priceContacts + stepValue;
                if(Math.Abs(oOrders) > 0 && Math.Abs(oOrders) < Math.Abs(positionContracts))
                {
                    string orderid = getOpenOrderId();
                    if(orderid == "" )
                    {
                        return;
                    }

                    String json = bitMEXApi.EditOrder(orderid, price, Math.Abs(positionContracts));
                    log(json);
                }
                else
                {
                    String json = bitMEXApi.PostOrderPostOnly(pair, side, price, Math.Abs(qntContacts), force, marketTaker, "Normal/Scalp Sell Exit Order", true);
                    JContainer config2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                    log(json);
                }
            }

            if (positionContracts < 0)
            {

                string side = "Buy";
                double priceContacts = Math.Abs(getPositionPrice());
                double actualPrice = Math.Abs(getPriceActual(side));
                double priceContactsProfit = Math.Abs(Math.Floor(priceContacts - (priceContacts * (profit + fee) / 100)));

                double price = priceContacts - stepValue;
                if (Math.Abs(oOrders) > 0 && Math.Abs(oOrders) < Math.Abs(positionContracts))
                {
                    string orderid = getOpenOrderId();
                    if (orderid == "")
                    {
                        return;
                    }

                    String json = bitMEXApi.EditOrder(orderid, price, Math.Abs(positionContracts));
                    log(json);
                }
                else
                {
                    String json = bitMEXApi.PostOrderPostOnly(pair, side, price, Math.Abs(qntContacts), force, marketTaker, "Normal/Scalp Buy Exit Order", true);
                    JContainer config2 = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
                    log(json);
                }

            }

        }
    }

    public static IIndicator LoadIndicator(string name = "")
    {
        if (name == "")
            throw new Exception("Invalid Indicator");

        string IndClass = "Indicator" + name;

        Type ind = Type.GetType(IndClass);
        if (ind == null)
            throw new Exception("Indicator: " + name + " Does not exist");

        IIndicator loaded = (IIndicator)Activator.CreateInstance(ind);
        return loaded;
    }

    public static void tests()
    {

        BackTest.run();



        return;
    }


}





