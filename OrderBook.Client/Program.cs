using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using OrderBook.Client.ConsoleUtils;

namespace OrderBook.Client
{
    class Program
    {
        private const int ORDERBOOK_ENDPOINTSERVER_PORT = 32000;

        private static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");
            
            var iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = iphostInfo.AddressList[0];
            var ipEndpoint = new IPEndPoint(ipAddress, ORDERBOOK_ENDPOINTSERVER_PORT);
            var connection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                connection.Connect(ipEndpoint);

                Console.WriteLine("Socket created to {0}", connection.RemoteEndPoint.ToString());
                StartListeningToServer(connection);

                // Listen to console input
                ConsoleWindowManager.Initialize();
                ConsoleWindowManager.Draw();
                while (true)
                {
                    byte[] messageToSend = null;
                    var input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            messageToSend = Encoding.ASCII.GetBytes("{\"o\": \"SELL\", \"q\": 100, \"s\": \"TEST\", \"p\": 10}\n");
                            break;
                        case "2":
                            messageToSend = Encoding.ASCII.GetBytes("{\"o\": \"BUY\", \"q\": 100, \"s\": \"TEST\", \"p\": 10}\n");
                            break;
                        case "3":
                            messageToSend = Encoding.ASCII.GetBytes("{\"o\": \"SELL\", \"q\": 250, \"s\": \"TEST\", \"p\": 10}\n");
                            break;
                        case "4":
                            messageToSend = Encoding.ASCII.GetBytes("{\"o\": \"BUY\", \"q\": 250, \"s\": \"TEST\", \"p\": 10}\n");
                            break;
                        default:
                            break;
                    }

                    if (messageToSend != null)
                    {
                        var bytesSent = connection.Send(messageToSend);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadKey();

            //client.Shutdown(SocketShutdown.Both);
            //client.Close();

        }

        private static void StartListeningToServer(Socket connection)
        {
            var thread = new Thread(() => ListenToServerLoop(connection));
            thread.Start();
        }

        private static void ListenToServerLoop(Socket connection)
        {
            var data = new byte[512];

            while (true)
            {
                var bytesRead = connection.Receive(data);
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, bytesRead));
            }
        }
    }
}
