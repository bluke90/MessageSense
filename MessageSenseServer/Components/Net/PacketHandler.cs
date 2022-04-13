using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;

namespace MessageSenseServer.Components.Net
{
    public class Packet
    {
        public AppUser User { get; set; }
        public Socket ClientSocket { get; set; } 
        public string Data { get; set; } // any searialized object - Ex. Message, Authentication, Request
        public string Resposne { get; set; }
        public string TaskCode { get; set; }
    }


    public static class PacketHandler
    {
        public static Packet GeneratePacketFromStateObj(this StateObject state)
        {
            var data = state.sb.ToString();
            var splitData = data.Split(" | ");

            var parsedData = new List<string>();
            foreach (var group in splitData) {
                if (group != "<EOF>") parsedData.Add(group);
            }

            Packet packet = new Packet() {
                ClientSocket = state.workSocket,
                TaskCode = parsedData[0],
                Data = parsedData[1]
            };
            return packet;

        }
        public static void AnalyzeInboundPacket(this Packet packet)
        {
            var data = packet.Data;

            //var msg = MessageExtensions.DeserializeMessage(data);

            var taskCode = packet.TaskCode;
            var task = taskCode.Split('.')[0];
            var code = taskCode.Split('.')[1];

            // Handle Desearialization of packet -> Request, Response, Message
            switch (task)
            {
                case "Req":     // Request
                    packet.HandleRequestPacket(code);
                    break;
                case "Auth":        //Authentication
                    HandleAuthenticationPacket(code, packet);
                    break;
                case "Cmd":     // Response/Command
                    HandleCmdPacket(code);
                    break;
            }
            return;
        }

        private static void HandleRequestPacket(this Packet packet, string code)
        {
            switch(code)
            {
                case "0000":        // Contact token request
                    packet.ProcessContactTokenRequest();
                    break;
                case "0001":        // Messages Request
                    break;
                case "0002":        // Store Message Request
                    break;
            }
            return;
        }

        private static void HandleAuthenticationPacket(string code, Packet packet)
        {
            switch (code)
            {
                case "00":        // Request for token authentication
                    break;
                case "01":        // Request for new authentication token
                    break;
            }
        }

        private static void HandleCmdPacket(string code)
        {
            switch (code)
            {
                case "0000":        // Messages Received
                    break;
                case "0001":        // New Auth Token Received
                    break;
                case "0002":        // Contact Token Received
                    break;
            }
        }

    }
}
