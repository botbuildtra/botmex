//using ServiceStack.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace BitMEX
{
    public class OrderBookItem
    {
        public string Symbol { get; set; }
        public int Level { get; set; }
        public int BidSize { get; set; }
        public decimal BidPrice { get; set; }
        public int AskSize { get; set; }
        public decimal AskPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class BitMEXApi
    {
        private string domain = "https://testnet.bitmex.com";
        private string apiKey;
        private string apiSecret;
        private int rateLimit;
        private bool apiLog;

        public BitMEXApi(string bitmexKey = "", string bitmexSecret = "", string bitmexDomain = "", int rateLimit = 5000, bool apiLog = false)
        {
            this.apiKey = bitmexKey;
            this.apiSecret = bitmexSecret;
            this.rateLimit = rateLimit;
            this.domain = bitmexDomain;
            this.apiLog = apiLog;
        }

        #region API Connector - Don't touch
        private string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        private string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));

            return "{" + string.Join(",", entries) + "}";
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        static object objLock = new object();
        private long GetExpires()
        {
            //lock (objLock)
            {
                System.Threading.Thread.Sleep(800);
                //DateTime yearBegin = new DateTime(2018, 1, 1);
                //return DateTime.UtcNow.Ticks - yearBegin.Ticks;
                return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600;
                //long ret = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                //return ret;
            }

        }

        private byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        static object objLockQuery = new object();
        private string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
        {
            //   lock (objLockQuery)
            {
                string paramData = json ? BuildJSON(param) : BuildQueryData(param);
                string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
                string postData = (method != "GET") ? paramData : "";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
                webRequest.Method = method;

                if (auth)
                {
                    string expires = GetExpires().ToString();
                    string message = method + url + expires + postData;
                    byte[] signatureBytes = hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                    string signatureString = ByteArrayToString(signatureBytes);

                    webRequest.Headers.Add("api-expires", expires);
                    webRequest.Headers.Add("api-key", apiKey);
                    webRequest.Headers.Add("api-signature", signatureString);
                }

                try
                {
                    if (postData != "")
                    {
                        webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                        var data = Encoding.UTF8.GetBytes(postData);
                        using (var stream = webRequest.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }

                    using (WebResponse webResponse = webRequest.GetResponse())
                    using (Stream str = webResponse.GetResponseStream())
                    using (StreamReader sr = new StreamReader(str))
                    {
                        string reply = sr.ReadToEnd();
                        if(this.apiLog)
                        {
                            MainClass.log("URL: " + url, ConsoleColor.White, "api");
                            MainClass.log("POSTDATA :" +postData, ConsoleColor.White, "api");
                            MainClass.log("RESPONSE: " + reply, ConsoleColor.White, "api");
                        }
                        return reply;
                    }
                }
                catch (WebException wex)
                {
                    using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                    {
                        if (response == null)
                            throw;

                        using (Stream str = response.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(str))
                            {
                                string reply = sr.ReadToEnd();
                                if (this.apiLog)
                                {
                                    MainClass.log("URL: " + url, ConsoleColor.White, "api");
                                    MainClass.log("POSTDATA :" + postData, ConsoleColor.White, "api");
                                    MainClass.log("RESPONSE: " + reply, ConsoleColor.White, "api");
                                }
                                return reply;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Examples from BitMex
        //public List<OrderBookItem> GetOrderBook(string symbol, int depth)
        //{
        //    var param = new Dictionary<string, string>();
        //    param["symbol"] = symbol;
        //    param["depth"] = depth.ToString();
        //    string res = Query("GET", "/orderBook", param);
        //    return JsonSerializer.DeserializeFromString<List<OrderBookItem>>(res);
        //}

        public string GetOrders(string Symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["filter"] = "{\"open\":true}";
            //param["columns"] = "";
            //param["count"] = 100.ToString();
            //param["start"] = 0.ToString();
            //param["reverse"] = false.ToString();
            //param["startTime"] = "";
            //param["endTime"] = "";
            return Query("GET", "/order", param, true);
        }

        public string PostOrders()
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = "XBTUSD";
            param["side"] = "Buy";
            param["orderQty"] = "1";
            param["ordType"] = "Market";
            return Query("POST", "/order", param, true);
        }

        public string DeleteOrders(String id)
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = id;
            param["text"] = "cancel order by ID";
            return Query("DELETE", "/order", param, true, true);
        }
        #endregion

        #region Our Calls
        public List<OrderBook> GetOrderBook(string symbol, int depth)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["depth"] = depth.ToString();
            string res = Query("GET", "/orderBook/L2", param);
            return JsonConvert.DeserializeObject<List<OrderBook>>(res);
        }

        public string PostOrderPostOnly(string Symbol, string Side, double Price, int Quantity, bool force = false, bool marketTaker = false, string text = "BotMex", bool close = false)
        {
            if(marketTaker)
            {
                return MarketOrder(Symbol, Side, Quantity, text);
            }

            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["side"] = Side;
            if (Symbol.Contains("XBT"))
            {
                Price = this.RoundToNearest(Price, 2);
            }
            else if (Symbol.Equals("ETHUSD"))
            {
                Price = this.RoundToNearest(Price, 20);
            }
            param["orderQty"] = Quantity.ToString();
            param["text"] = text;
            param["ordType"] = "Limit";
            if(!force)
                param["execInst"] = "ParticipateDoNotInitiate";
            if (close)
                param["execInst"] = "Close";
            //param["displayQty"] = 1.ToString(); // Shows the order as hidden, keeps us from moving price away from our own orders
            param["price"] = Price.ToString().Replace(",", ".");
            string ret = Query("POST", "/order", param, true);

            return ret;
        }

        public string MarketOrder(string Symbol, string Side, int Quantity, string text = "BotMex")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["side"] = Side;
            param["orderQty"] = Quantity.ToString();
            param["ordType"] = "Market";
            param["text"] = text;
            String ret = Query("POST", "/order", param, true);

            return ret;
        }

        public string MarketClose(string Symbol, string Side, string text = "BotMex")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["side"] = Side;
            param["ordType"] = "Market";
            param["execInst"] = "Close";
            param["text"] = text;
            String ret = Query("POST", "/order", param, true);
            return ret;
        }

        public string CreateStopOrder( string Symbol, string Side, double Quantity, double stopPx, bool Market = false, double Price = 0, string text = "BotMex")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["side"] = Side;
            param["orderQty"] = Quantity.ToString();
            param["ordType"] = Market ? "Stop" : "StopLimit";
            if(!Market)
            {
                if (Symbol.Contains("XBT"))
                {
                    Price = this.RoundToNearest(Price, 2);
                }
                else if (Symbol.Equals("ETHUSD"))
                {
                    Price = this.RoundToNearest(Price, 20);
                }
                param["price"] = Price.ToString().Replace(",", ".");
            }
            if (Symbol.Contains("XBT"))
            {
                stopPx = this.RoundToNearest(stopPx, 2);
            }
            else if (Symbol.Equals("ETHUSD"))
            {
                stopPx = this.RoundToNearest(stopPx, 20);
            }
            param["stopPx"] = stopPx.ToString().Replace(",", ".");
            param["execInst"] = "Close,LastPrice";
            param["text"] = text;
            String ret = Query("POST", "/order", param, true);

            return ret;
        }

        public string CancelAllLimitOrders(string symbol, string Note = "" )
        {
            List<Order> ords = GetOpenOrders(symbol);
            if (ords.Count == 0)
                return "";
            string orderIds = string.Join(",", from Order ord in ords select ord.OrderId);
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["orderID"] = orderIds;
            param["text"] = Note;
            String ret = Query("DELETE", "/order",param,true,true);

            return ret;
        }

        public string CancelAllSLOrders(string symbol, string Note = "")
        {
            List<Order> ords = GetOpenSLOrders(symbol);
            if (ords.Count == 0)
                return "";
            string orderIds = string.Join(",", from Order ord in ords select ord.OrderId);
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["orderID"] = orderIds;
            param["text"] = Note;
            String ret = Query("DELETE", "/order", param, true, true);

            return ret;
        }

        public string CancelAllOpenOrders(string symbol, string Note = "")
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["text"] = Note;
            return Query("DELETE", "/order/all", param, true, true);
        }

        public string CancelOrderById(string symbol, string orderID)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["orderID"] = orderID;
            return Query("DELETE", "/order", param, true, true);
        }

        public List<Instrument> GetActiveInstruments()
        {
            string res = Query("GET", "/instrument/active");
            return JsonConvert.DeserializeObject<List<Instrument>>(res);
        }

        public List<Instrument> GetInstrument(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            string res = Query("GET", "/instrument", param);
            return JsonConvert.DeserializeObject<List<Instrument>>(res);
        }

        public List<Candle> GetCandleHistory(string symbol, int count, string size, bool partial = false)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["count"] = count.ToString();
            param["reverse"] = true.ToString();
            param["partial"] = partial ? "true" : "false";
            param["binSize"] = size;
            string res = Query("GET", "/trade/bucketed", param);
            return JsonConvert.DeserializeObject<List<Candle>>(res).OrderByDescending(a => a.TimeStamp).ToList();
        }

        public List<Position> GetOpenPositions(string symbol)
        {
            var param = new Dictionary<string, string>();
            string res = Query("GET", "/position", param, true);
            return JsonConvert.DeserializeObject<List<Position>>(res).Where(a => a.Symbol == symbol && a.IsOpen == true).OrderByDescending(a => a.TimeStamp).ToList();
        }

        public List<Order> GetOpenOrders(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["reverse"] = true.ToString();
            string res = Query("GET", "/order", param, true);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(res).Where(a => a.OrdStatus == "New" || a.OrdStatus == "PartiallyFilled" && (a.OrdType != "Stop" && a.OrdType != "StopLimit")).OrderByDescending(a => a.TimeStamp).ToList();
            List<Order> returnables = new List<Order>();
            foreach(Order ord in orders )
            {
                if(ord.OrdType != "Stop" && ord.OrdType != "StopLimit" )
                returnables.Add(ord);
            }
            return returnables;
            //return JsonConvert.DeserializeObject<List<Order>>(res).Where(a => a.OrdStatus == "New" || a.OrdStatus == "PartiallyFilled" && (a.OrdType != "Stop" && a.OrdType != "StopLimit")).OrderByDescending(a => a.TimeStamp).ToList();
        }

        public List<Order> GetOpenSLOrders(string symbol)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["reverse"] = true.ToString();
            string res = Query("GET", "/order", param, true);
            return JsonConvert.DeserializeObject<List<Order>>(res).Where(a => a.OrdStatus == "New" && (a.OrdType == "Stop" || a.OrdType == "StopLimit")).OrderByDescending(a => a.TimeStamp).ToList();
        }

        public Order GetOrderById( string orderId)
        {
            var param = new Dictionary<string, string>();
            param["filter"] = "{\"orderID\":\""+orderId+"\"}";
            string res = Query("GET", "/order", param, true);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(res).ToList();
            if(orders.Count > 0)
                return orders[0];

            return new Order();
        }

        public string EditSLOrderPx(string OrderId, double stopPx, int Quantity = 0)
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = OrderId;
            param["stopPx"] = stopPx.ToString();
            if(Quantity != 0)
            {
                param["orderQty"] = Quantity.ToString();
            }
            return Query("PUT", "/order", param, true, true);
        }

        public string EditOrder(string OrderId, double Price, int Qty = 0)
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = OrderId;
            if(Qty != 0)
            {
                param["orderQty"] = Qty.ToString();
            }

            if (MainClass.pair.Contains("XBT"))
            {
                Price = this.RoundToNearest(Price, 2);
            }
            else if (MainClass.pair.Contains("ETHUSD"))
            {
                Price = this.RoundToNearest(Price, 20);
            }
            param["price"] = Price.ToString().Replace(",", ".");
            param["price"] = Price.ToString();
            return Query("PUT", "/order", param, true, true);
        }

        public string GetWallet()
        {
            var param = new Dictionary<string, string>();
            param["currency"] = "XBt";
            return Query("GET", "/user/walletHistory", param, true);
        }



        #endregion



        #region RateLimiter

        private long lastTicks = 0;
        private object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        #endregion RateLimiter

        public Double RoundToNearest(Double val, int multiple)
        {
            return Math.Round(val * multiple, MidpointRounding.AwayFromZero) / multiple;
        }
    }


    // Working Classes
    public class OrderBook
    {
        public string Side { get; set; }
        public double Price { get; set; }
        public int Size { get; set; }
    }

    public class Instrument
    {
        public string Symbol { get; set; }
        public double TickSize { get; set; }
        public double Volume24H { get; set; }
    }

    public class Candle
    {
        public DateTime TimeStamp { get; set; }
        public double? open { get; set; }
        public double? close { get; set; }
        public double? high { get; set; }
        public double? low { get; set; }
        public double? volume { get; set; }
    }

    public class Position
    {
        public DateTime TimeStamp { get; set; }
        public double? Leverage { get; set; }
        public int? CurrentQty { get; set; }
        public double? CurrentCost { get; set; }
        public bool IsOpen { get; set; }
        public double? MarkPrice { get; set; }
        public double? MarkValue { get; set; }
        public double? UnrealisedPnl { get; set; }
        public double? UnrealisedPnlPcnt { get; set; }
        public double? AvgEntryPrice { get; set; }
        public double? BreakEvenPrice { get; set; }
        public double? LiquidationPrice { get; set; }
        public double? LastValue { get; set; }

        public string Symbol { get; set; }

        public double percentual()
        {
            if (UnrealisedPnl < 0)
                return (((((double)UnrealisedPnl * (-1)) * 100) / (double)LastValue) * (double)Leverage) * (-1);
            else
                return ((((double)UnrealisedPnl) * 100) / (double)LastValue) * (double)Leverage;
        }

    }

    public class Order
    {
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("ordStatus")]
        public string OrdStatus { get; set; }
        [JsonProperty("ordType")]
        public string OrdType { get; set; }
        [JsonProperty("orderID")]
        public string OrderId { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("price")]
        public double? Price { get; set; }
        [JsonProperty("orderQty")]
        public int? OrderQty { get; set; }
        [JsonProperty("displayQty")]
        public int? DisplayQty { get; set; }
        [JsonProperty("stopPx")]
        public double? StopPx { get; set; }
        [JsonProperty("avgPx")]
        public double? AvgPx { get; set; }
    }
}