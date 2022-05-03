using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class SynchronousClient
    {
        static byte[] _buffer = new byte[256];
        public static string SendPacket(string data) {
            var resp = string.Empty;

            try {
                IPAddress ipAddress = IPAddress.Parse("192.168.1.15");
                Console.WriteLine(ipAddress.ToString());
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                try {
                    client.Connect(remoteEP);

                    Console.WriteLine($"Connected to Remote EndPoint => {ipAddress.ToString()}");

                    byte[] msg = Encoding.ASCII.GetBytes(data+ " | <EOF>");

                    int bytesSend = client.Send(msg);

                    while (true) {
                        int bytesRecv = client.Receive(_buffer);

                        var respFrag = Encoding.ASCII.GetString(_buffer, 0, bytesRecv);

                        resp += respFrag;

                        // Check for end of fragment
                        if (resp.Contains("<EOF>")) {
                            break;
                        }
                    }

                    Console.WriteLine($"Received => {resp}");

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();

                    return resp;
                } catch (ArgumentNullException ane) {
                    Console.WriteLine(ane.ToString());
                    Console.WriteLine("Error in synchronous Client Transmission => ClientNet/SynchrounousClient.cs => SendPacket[1]");
                } catch (SocketException se) {
                    Console.WriteLine(se.ToString());
                    Console.WriteLine("Error in synchronous Client Transmission => ClientNet/SynchrounousClient.cs => SendPacket[1]");
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Error in synchronous Client Transmission => ClientNet/SynchrounousClient.cs => SendPacket[1]");
                }

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Error in synchronous Client Transmission => ClientNet/SynchrounousClient.cs => SendPacket[0]");
            }


            return resp;
        }



    }
}
