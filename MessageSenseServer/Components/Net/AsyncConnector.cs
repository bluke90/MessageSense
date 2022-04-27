using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace MessageSenseServer.Components.Net
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
        // Client socket.
        public Socket workSocket = null;
    }
    public class AsyncConnector
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsyncConnector(){}

        public static void StartListening(string host_ip = "127.0.0.1", int port = 11000)
        { 
            IPAddress ipAddress = IPAddress.Parse(host_ip);
            Console.WriteLine($"Using {ipAddress.ToString()}");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
                        
            try {       // Bind the socket to the local endpoint and listen for incoming connections.  
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true) {
                    allDone.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
                                       
                    allDone.WaitOne();      // Wait until a connection is made before continuing.  
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static async void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    // GeneratePacket
                    var packet = state.GeneratePacketFromStateObj();
                    // Handle Packet | Req, Resp, Msg
                    var resp = await packet.AnalyzeInboundPacket();

                    Send(handler, resp + " | <EOF>");
                    Console.WriteLine("Sent: " + resp + " | <EOF>");
                    Console.WriteLine("\n");
                } else {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }



    }
}
