using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OrderBook.Client
{
    class Program
    {
        private const int ORDERBOOK_ENDPOINTSERVER_PORT = 32000;

        private static void Main(String[] args)
        {
            System.Threading.Thread.Sleep(1000); // DEBUG: wait for server to be ready because server and client start at the same time when debugging

            byte[] data = new byte[512];

            var iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = iphostInfo.AddressList[0];
            var ipEndpoint = new IPEndPoint(ipAddress, ORDERBOOK_ENDPOINTSERVER_PORT);
            var client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(ipEndpoint);

                Console.WriteLine("Socket created to {0}", client.RemoteEndPoint.ToString());
                Console.WriteLine("Press any key to send an order to the server.");

                byte[] sendmsg = Encoding.ASCII.GetBytes("{\"o\": \"BUY\", \"q\": 100, \"s\": \"TEST\", \"p\": 10}\n");

                int n = client.Send(sendmsg);

                int bytesRead = client.Receive(data);

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, bytesRead));
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Transmission end.");
            Console.ReadKey();

        }
    }
}
