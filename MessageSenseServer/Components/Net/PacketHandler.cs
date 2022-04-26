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

    public class PacketData {
        public int TransmissionId { get; set; }

        // MainPacket => transmissionId | TaskCode | Data | <EOF>
        // MainPacket.Data = Data -- authToken -- userId
        public string? TaskCode { get; set; }
        public string? Data { get; set; }
        public string? AuthToken { get; set; }
        public int AppUserId { get; set; }

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
        public static async Task AnalyzeInboundPacket(this Packet packet)
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
                    await packet.HandleRequestPacket(code);
                    break;
                case "Auth":        //Authentication
                    await HandleAuthenticationPacket(code, packet);
                    break;
                case "Cmd":     // Response/Command
                    await packet.HandleCmdPacket(code);
                    break;
            }
            return;
        }

        private static async Task HandleRequestPacket(this Packet packet, string code)
        {
            switch(code)
            {
                case "0000":        // Contact token request
                    await packet.ProcessContactTokenRequest();
                    break;
                case "0001":        // Messages Request
                    await packet.ProcessMessagePullRequest();
                    break;
                case "0002":        // Store Message Request
                    await packet.ProcessMessageStoreRequest();
                    break;
            }
            return;
        }

        private static async Task HandleAuthenticationPacket(string code, Packet packet)
        {
            await Task.Yield();
            switch (code)
            {
                case "00":        // Request for token authentication
                    break;
                case "01":        // Request for new authentication token
                    break;
            }
        }

        private static async Task HandleCmdPacket(this Packet packet, string code)
        {
            await Task.Yield();
            switch (code)
            {
                case "0000":        // Messages Received
                    await packet.ProcessMessagesReceivedReqeust();
                    break;
                case "0001":        // New Auth Token Received
                    break;
                case "0002":        // Contact Token Received
                    break;
            }
            return;
        }

    }
}
