using OrderBook.Core.Model;
using System;
using System.Messaging;
using System.Net.Sockets;

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
        private static AsynchronousSocketListener _clientListener;

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
            _clientListener = new AsynchronousSocketListener(LISTEN_PORT);

            // Bind events
            _clientListener.OnReceiveCommandFromClient += OnReceiveCommandFromClient;

            // Start listening to client commands
            _clientListener.StartListening();
        }

        private static void OnReceiveCommandFromClient(string message, Socket connection)
        {
            // message.length = number of bytes received

            // TODO: this assumes the message is always well formed. Need to put some checks in place.
            var order = JsonToOrderDeserializer.GetOrder(message);

            // Pass connection around to be able to send reponse to client

            Console.WriteLine($"{order.Timestamp}: Got the following order: {order.OrderSide} {order.Quantity} {order.Symbol} @ {order.Price} (Ref #: {order.Reference})");
            _clientListener.Send(connection, order.Reference.ToString());
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
