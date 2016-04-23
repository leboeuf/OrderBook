using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OrderBook.EndpointServer
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 512;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    /// <remarks>
    /// Adapted from https://msdn.microsoft.com/en-us/library/fx6588te(v=vs.110).aspx
    /// </remarks>
    public class AsynchronousSocketListener
    {
        private readonly int _listenPort;
        public ManualResetEvent _allDone = new ManualResetEvent(false);

        public delegate void OnClientConnectedEventHandler();
        public event OnClientConnectedEventHandler OnClientConnected;

        public delegate void OnReceiveCommandFromClientEventHandler(string message, Socket connection);
        public event OnReceiveCommandFromClientEventHandler OnReceiveCommandFromClient;

        public AsynchronousSocketListener(int listenPort)
        {
            _listenPort = listenPort;
        }

        public void StartListening()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, _listenPort);
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100); // backlog size may be improved with load testing metrics http://tangentsoft.net/wskfaq/advanced.html#backlog

                while (true)
                {
                    // Set the event to nonsignaled state.
                    _allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    _allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.Read();

        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            // Signal the main thread to continue.
            _allDone.Set();

            // Get the socket that handles the client request.
            var listener = (Socket)asyncResult.AsyncState;
            var connection = listener.EndAccept(asyncResult);

            // Create the state object.
            var state = new StateObject();
            state.workSocket = connection;

            if (OnClientConnected != null)
                OnClientConnected();

            connection.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult asyncResult)
        {
            var content = string.Empty;

            // Retrieve the state object and the connection socket from the asynchronous state object.
            var state = (StateObject)asyncResult.AsyncState;
            var connection = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = connection.EndReceive(asyncResult);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-line tag. If it is not there, read more data.
                content = state.sb.ToString();
                if (content.IndexOf("\n") > -1)
                {
                    // All the data has been read from the client. 
                    if (OnReceiveCommandFromClient != null)
                        OnReceiveCommandFromClient(content, connection);

                    // Echo the data back to the client.
                    Send(connection, content);
                }

                // Either not all data received (no \n found) or we finished processing a command. Get more.
                connection.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        public void Send(Socket connection, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            connection.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), connection);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.
                var connection = (Socket)asyncResult.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = connection.EndSend(asyncResult);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //connection.Shutdown(SocketShutdown.Both);
                //connection.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
