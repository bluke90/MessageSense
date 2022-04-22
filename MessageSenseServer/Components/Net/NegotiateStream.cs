using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.IO;
using System.Threading;
using MessageSenseServer.Data;
using MessageSenseServer.Components.Models;

namespace MessageSenseServer.Components.Net
{
    public class AsynchronousAuthenticatingTcpListener
    {
        public static void Main()
        {
            // Create an IPv4 TCP/IP socket.
            TcpListener listener = new TcpListener(IPAddress.Parse("192.168.1.15"), 11001);
            // Listen for incoming connections.
            listener.Start();
            while (true)
            {
                TcpClient clientRequest;
                // Application blocks while waiting for an incoming connection.
                // Type CNTL-C to terminate the server.
                Console.WriteLine("Listening");
                clientRequest = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                // A client has connected.
                try
                {
                    AuthenticateClient(clientRequest);
                    NetworkStream stream = clientRequest.GetStream();
                    Console.WriteLine("Sending Success");
                    var data = Encoding.UTF8.GetBytes("Success!! <EOF>");
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static void AuthenticateClient(TcpClient clientRequest)
        {
            NetworkStream stream = clientRequest.GetStream();
            // Create the NegotiateStream.
            NegotiateStream authStream = new NegotiateStream(stream, false);
            // Save the current client and NegotiateStream instance
            // in a ClientState object.
            ClientState cState = new ClientState(authStream, clientRequest);
            // Listen for the client authentication request.
            Task authTask = authStream
                .AuthenticateAsServerAsync()
                .ContinueWith(task => { EndAuthenticateCallback(cState); });

            // Any exceptions that occurred during authentication are
            // thrown by the EndAuthenticateAsServer method.
            try
            {
                // This call blocks until the authentication is complete.
                authTask.Wait();
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Authentication failed - closing connection.");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Closing connection.");
                return;
            }

            Task<int> readTask = authStream
                .ReadAsync(cState.Buffer, 0, cState.Buffer.Length);

            readTask
                .ContinueWith((task) => { EndReadCallback(cState, task.Result); })
                .Wait();

            authStream.Close();
            clientRequest.Close();
        }

        private static void EndAuthenticateCallback(ClientState cState)
        {
            // Get the saved data.
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;
            Console.WriteLine("Ending authentication.");

            // Display properties of the authenticated client.
            IIdentity id = authStream.RemoteIdentity;
            Console.WriteLine("{0} was authenticated using {1}.",
                id.Name,
                id.AuthenticationType
            );
        }

        private static void EndReadCallback(ClientState cState, int bytes)
        {
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;
            // Read the client message.
            try
            {

                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));

                if (bytes != 0)
                {
                    Task<int> readTask = authStream.ReadAsync(cState.Buffer, 0, cState.Buffer.Length);
                    readTask
                        .ContinueWith(task => { EndReadCallback(cState, task.Result); })
                        .Wait();

                    return;
                }
            }
            catch (Exception e)
            {
                // A real application should do something
                // useful here, such as logging the failure.
                Console.WriteLine("Client message exception:");
                Console.WriteLine(e);
                return;
            }
            IIdentity id = authStream.RemoteIdentity;
            Console.WriteLine("{0} says {1}", id.Name, cState.Message.ToString());

            // handle authenticated message
            // cState.Message.ToString(); split(" | ") [0] = currentAuthToken , [1] = deviceId
            string data = cState.Message.ToString();
            try {
                Authentication.AuthenticateUser(data);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }

    // ClientState is the AsyncState object.
    internal class ClientState
    {
        private StringBuilder _message = null;

        internal ClientState(AuthenticatedStream a, TcpClient theClient)
        {
            AuthenticatedStream = a;
            Client = theClient;
        }
        internal TcpClient Client { get; }

        internal AuthenticatedStream AuthenticatedStream { get; }

        internal byte[] Buffer { get; } = new byte[2048];

        internal StringBuilder Message
        {
            get { return _message ??= new StringBuilder(); }
        }
    }

}
