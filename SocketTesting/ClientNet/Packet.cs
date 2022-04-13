using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;



namespace SocketTesting.ClientNet
{
    public class Packet
    {
        public string TaskCode { get; set; }
        public string Data { get; set; }
        public string Response { get; set; }
    }

    public static class PacketUtils
    {
        public static string SearializePacket(this Packet packet)
        {
            var data = JsonSerializer.Serialize(packet);
            return data;
        }

        public static Packet GeneratePacket()
        {
            var packet = new Packet();
            return packet;
        }

        public static string TransmistPacket(this Packet packet)
        {
            var data = $"{packet.TaskCode} | {packet.Data}";

            var resp_data = AsynchronousClient.StartClient(data);
            
            return resp_data;
        }

        // TaskObjects
        public static void GenerateContactTokenRequest(this Packet packet ,string token, string username, string firstname)
        {
            AppUser user = new AppUser()
            {
                ContactToken = token,
                Username = username,
                FirstName = firstname
            };
            var data = user.SerializeAppUserObj();
            packet.Data = data;
            packet.TaskCode = "Req.0000";
            return;
        }

    }
}
