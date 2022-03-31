using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenseServer.Components.Net
{
    public class Packet
    {
        public int Id { get; set; }
        public AppUser User { get; set; }
        public Socket ClientSocket { get; set; } 
        public string Data { get; set; }
        public string Resposne { get; set; }
    }

    public static class PacketHandler
    {
        public static void GeneratePacketFromStateObj(this StateObject state)
        {
            Packet packet = new Packet()
            {
                ClientSocket = state.workSocket,
                Data = state.sb.ToString(),
            };

        }
        public static void AnalyzeInboundPacket(this Packet packet)
        {
            var data = packet.Data;





        }
    }
}
