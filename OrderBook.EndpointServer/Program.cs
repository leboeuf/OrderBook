using OrderBook.Core.Model;
using System;
using System.Collections.Generic;
using System.Messaging;

namespace OrderBook.EndpointServer
{
    class Program
    {
        // The RX and TX queues are the inverse of those in OrderBook.Server (because one's TX is the other's RX)
        private const string MESSAGE_QUEUE_RX_NAME = @".\Private$\OrderBookServer_ProcessedOrdersQueue";
        private const string MESSAGE_QUEUE_TX_NAME = @".\Private$\OrderBookServer_OrdersQueue";
        private static MessageQueue _rxMessageQueue;
        private static MessageQueue _txMessageQueue;

        static void Main(string[] args)
        {
            InitializeMessageQueues();

            var order1 = new Core.Model.Order("TEST", Core.Model.Enums.OrderSide.Sell, 100, 10);
            var order2 = new Core.Model.Order("TEST", Core.Model.Enums.OrderSide.Buy, 100, 10);

            _txMessageQueue.Send(order1);
            _txMessageQueue.Send(order2);

            while (true)
            {
                var message = _rxMessageQueue.Receive();
                var processedOrder = (ProcessedOrder)message.Body;
                Console.WriteLine("Trade {0} executed at {1}.", processedOrder.Reference, processedOrder.TimestampExecuted);
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
    }
}
