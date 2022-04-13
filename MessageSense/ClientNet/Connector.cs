using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class Connector
    {
        public static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint ep = new IPEndPoint(ipAddress, 11000);

                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Connect to end point
                    sender.Connect(ep);

                    // Encode string to bytes
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test <EOF>");

                    // Send data
                    int bytesSent = sender.Send(msg);

                    // Receive Response
                    int bytesReceived = sender.Receive(msg);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentException");
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Socket Exception");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
