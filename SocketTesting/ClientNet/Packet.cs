﻿using System;
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

        public static string TransmitPacket(this Packet packet)
        {
            var data = $"{packet.TaskCode} | {packet.Data}";

            var resp_data = AsynchronousClient.StartClient(data);
            
            return resp_data;
        }

        public static async Task<string> TransmitPacketAsync(this Packet packet)
        {
            await Task.Yield();
            var resp = TransmitPacket(packet);
            return resp;
        }

        // TaskObjects
        public static void GenerateMessageStoreRequest(this Packet packet, Message msg)
        {
            packet.Data = JsonSerializer.Serialize(msg);
            packet.TaskCode = "Req.0002";
            return;
        }

        public static void GenerateContactTokenRequest(this Packet packet , AppUser user)
        {
            var deviceId = "ABC123";
            var userData = user.SerializeAppUserObj();
            packet.Data = userData + " -- " + deviceId;
            packet.TaskCode = "Req.0000";
            return;
        }

    }
}
