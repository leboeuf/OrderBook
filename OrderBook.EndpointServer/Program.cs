using OrderBook.Core.Model;
using System;
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
        private const int LISTEN_PORT = 32000;

        static void Main(string[] args)
        {
            InitializeMessageQueues();
            //StartListeningToProcessedOrders();
            StartListeningToClients();
        }

        private static void StartListeningToProcessedOrders()
        {
            while (true)
            {
                var message = _rxMessageQueue.Receive();
                var processedOrder = (ProcessedOrder)message.Body;
                Console.WriteLine("Trade {0} executed at {1}.", processedOrder.Reference, processedOrder.TimestampExecuted);
            }
        }

        private static void StartListeningToClients()
        {
            var listener = new AsynchronousSocketListener(LISTEN_PORT);
            // TODO: bind events
            listener.StartListening();
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
