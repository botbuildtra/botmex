using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bitmex.Client.Websocket;
using Bitmex.Client.Websocket.Client;
using Bitmex.Client.Websocket.Requests;
using Bitmex.Client.Websocket.Websockets;

namespace WebSocket
{
    public static class BitmexWS
    {
        public static double priceSideBuy = 0;
        public static double priceSideSell = 0;
        public static double position = 0;
        public static string wsURl = MainClass.bitmexWebSocketDomain;
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        public static void run()
        {
            var url = new Uri(wsURl);

            using (var connector = new BitmexWebsocketCommunicator(url))
            {
                using (var client = new BitmexWebsocketClient(connector))
                {
                    client.Streams.InfoStream.Subscribe(info =>
                    {
                        Console.WriteLine($"Info received, reconnection happened.");
                        client.Send(new PingRequest()).Wait();
                        SendSubscriptionRequests(client).Wait();
                    });

                    SubscribeToStreams(client);
                    connector.Start();

                    ExitEvent.WaitOne();
                }
            }
        }

        private static async Task SendSubscriptionRequests(BitmexWebsocketClient client)
        {
            await client.Send(new PingRequest());
            //await client.Send(new BookSubscribeRequest(MainClass.pair));
            //await client.Send(new TradesSubscribeRequest(MainClass.pair));
            //await client.Send(new TradeBinSubscribeRequest("1m", MainClass.pair));
            //await client.Send(new TradeBinSubscribeRequest("5m", MainClass.pair));
            //await client.Send(new TradeBinSubscribeRequest("1h", MainClass.pair));
            await client.Send(new QuoteSubscribeRequest(MainClass.pair));
            //await client.Send(new LiquidationSubscribeRequest());

            //if (!string.IsNullOrWhiteSpace(MainClass.bitmexKeyWebSocket))
                //await client.Send(new AuthenticationRequest(MainClass.bitmexKeyWebSocket, MainClass.bitmexSecretWebSocket));
        }

        private static void SubscribeToStreams(BitmexWebsocketClient client)
        {
            client.Streams.ErrorStream.Subscribe(x =>
                Console.WriteLine($"Error received, message: {x.Error}, status: {x.Status}"));

            client.Streams.AuthenticationStream.Subscribe(x =>
            {
                Console.WriteLine($"Authentication happened, success: {x.Success}");
                //client.Send(new WalletSubscribeRequest()).Wait();
                //client.Send(new OrderSubscribeRequest()).Wait();
                //client.Send(new PositionSubscribeRequest()).Wait();
            });


            client.Streams.SubscribeStream.Subscribe(x =>
                Console.WriteLine($"Subscribed ({x.Success}) to {x.Subscribe}"));

            client.Streams.PongStream.Subscribe(x =>
                Console.WriteLine($"Pong received ({x.Message})"));


            client.Streams.WalletStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Console.WriteLine($"Wallet {x.Account}, {x.Currency} amount: {x.BalanceBtc}"))
            );

            client.Streams.OrderStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Console.WriteLine(
                        $"Order {x.Symbol} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.OrderQty}, " +
                        $"Price: {x.Price}, Direction: {x.Side}, Working: {x.WorkingIndicator}, Status: {x.OrdStatus}"))
            );

            client.Streams.PositionStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Console.WriteLine(
                        $"Position {x.Symbol}, {x.Currency} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.CurrentQty}, " +
                        $"Price: {x.LastPrice}, PNL: {x.SimplePnlPcnt} EntryPrice: {x.ExecSellCost}"))
            );

            client.Streams.TradesStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Console.WriteLine($"Trade {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, [{x.Side}] Amount: {x.Size}, " +
                                    $"Price: {x.Price}"))
            );

            client.Streams.BookStream.Subscribe(book =>
                book.Data.Take(100).ToList().ForEach(x => Console.WriteLine(
                    $"Book | {book.Action} pair: {x.Symbol}, price: {x.Price}, amount {x.Size}, side: {x.Side}"))
            );

            client.Streams.QuoteStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    //Console.WriteLine($"Quote {x.Symbol}. Bid: {x.BidPrice} - {x.BidSize} Ask: {x.AskPrice} - {x.AskSize}"))
                    setPrices(x.BidPrice, x.AskPrice))
            );

            client.Streams.LiquidationStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Console.WriteLine(
                        $"Liquadation Action: {y.Action}, OrderID: {x.OrderID}, Symbol: {x.Symbol}, Side: {x.Side}, Price: {x.Price}, LeavesQty: {x.leavesQty}"))
            );

            client.Streams.TradeBinStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                Console.WriteLine($"TradeBin table:{y.Table} {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, Open: {x.Open}, " +
                        $"Close: {x.Close}, Volume: {x.Volume}, Trades: {x.Trades}"))
            );

        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Exiting process");
            ExitEvent.Set();
        }

        private static void setPrices( double? buy, double? sell)
        {
            priceSideBuy = (double)buy;
            priceSideSell = (double)sell;

            //MainClass.log("Price on Buy Side: " + priceSideBuy, ConsoleColor.Green);
            //MainClass.log("Price on Sell Side: " + priceSideSell, ConsoleColor.Green);
        }

        private static void setPosition( double? wPosition, double? wPrice)
        {

        }
    }
}
