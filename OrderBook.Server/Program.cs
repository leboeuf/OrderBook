using OrderBook.Core.Model;
using OrderBook.Core.Model.Enums;
using System.Messaging;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OrderBook.Server
{
    class Program
    {
        private const string MESSAGE_QUEUE_RX_NAME = @".\Private$\OrderBookServer_OrdersQueue";
        private const string MESSAGE_QUEUE_TX_NAME = @".\Private$\OrderBookServer_ProcessedOrdersQueue";
        private static MessageQueue _rxMessageQueue;
        private static MessageQueue _txMessageQueue;

        private static readonly IReadOnlyList<string> AvailableSymbols = new List<string> { "TEST", "TEST1", "TEST2" };
        private static Dictionary<string, Core.Model.OrderBook> _orderBooks;

        static void Main(string[] args)
        {
            InitializeOrderBooks();
            InitializeMessageQueues();
            
            while (true)
            {
                // TODO:
                // if (current time is in market closed time)
                //   Cleanup "good til x" orders: refer to the Duration section in http://apps.tmx.com/en/trading/order_types/
                //   block on WaitForMarketOpen()

                var message = _rxMessageQueue.Receive();
                ProcessOrder((Order)message.Body);
            }
        }

        private static void InitializeOrderBooks()
        {
            _orderBooks = new Dictionary<string, Core.Model.OrderBook>();

            // TODO: load orderbooks from disk

            foreach (var symbol in AvailableSymbols)
            {
                _orderBooks.Add(symbol, new Core.Model.OrderBook());
            }
        }

        private static void InitializeMessageQueues()
        {
            CreateOrOpenQueue(ref _rxMessageQueue, MESSAGE_QUEUE_RX_NAME);
            CreateOrOpenQueue(ref _txMessageQueue, MESSAGE_QUEUE_TX_NAME);
        }

        private static void CreateOrOpenQueue(ref MessageQueue messageQueue, string queueName)
        {
            if (MessageQueue.Exists(queueName))
            {
                messageQueue = new MessageQueue(queueName);
            }
            else
            {
                messageQueue = MessageQueue.Create(queueName);
            }

            messageQueue.Formatter = new BinaryMessageFormatter();
        }

        private static void ProcessOrder(Order order)
        {
            // Update timestamp to server time
            order.Timestamp = DateTime.UtcNow;

            // Try to match the order
            var orderBook = _orderBooks[order.Symbol];
            var matches = orderBook.Orders
                .Where(bookOrder => bookOrder.OrderSide != order.OrderSide
                    && (order.OrderSide == OrderSide.Buy ? bookOrder.Price <= order.Price : bookOrder.Price >= order.Price)
                    && bookOrder.Quantity == order.Quantity) // TODO: temporary (remove when handle partial fills)
                .OrderBy(bookOrder => bookOrder.Price)
                .ThenBy(bookOrder => bookOrder.Timestamp)
                .ToList();

            // TODO: handle partial fills (execute some trades and put the rest of the order in the queue)

            if (matches.Any())
            {
                // Delete from orders if matched and broadcast the trade to everyone
                var match = matches.First();
                orderBook.Orders.Remove(match);
                BroadcastProcessedOrder(ProcessedOrder.CreateFromOrder(match));
                BroadcastProcessedOrder(ProcessedOrder.CreateFromOrder(order));
            }
            else
            {
                // Put in order book if not matched
                orderBook.Orders.Add(order);
            }
        }

        private static void BroadcastProcessedOrder(ProcessedOrder order)
        {
            _txMessageQueue.Send(order);
        }
    }
}
