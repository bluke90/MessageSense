﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using SocketTesting.ClientNet;

namespace SocketTexting.ClientNet
{
    public class Packet
    {
        public string TaskCode { get; set; }
        public string Data { get; set; }
        public string Response { get; set; }
    }

    public class NetControlChars
    {
        private NetControlChars(string value) { Value = value; }

        public string Value { get; private set; }

        public static NetControlChars EndOfTransmission { get { return new NetControlChars(" <EOF>"); } }
        public static NetControlChars PrimarySeperator { get { return new NetControlChars(" | "); } }
        public static NetControlChars SecondarySeperator { get { return new NetControlChars(" -- "); } }
        public static NetControlChars DataObjSeperator { get { return new NetControlChars(" <|> "); } }
        public static NetControlChars Blank { get { return new NetControlChars(" ::: "); } }

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
            var data = packet.TaskCode + NetControlChars.PrimarySeperator.Value + packet.Data;

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
        public static async Task GenerateMessageStoreRequest(this Packet packet, Message msg, AppUser user)
        {
            await Task.Yield();
            packet.Data = $"{JsonSerializer.Serialize(msg)} -- {user.CurrentAuthToken} -- {user.Id}";
            packet.TaskCode = "Req.0002";
            return;
        }
        public static void GenerateMessageReceivedConfirmation(this Packet packet, List<int> msg_ids, AppUser user)
        {
            if (msg_ids.Count > 1)
            {
                packet.Data = $"{string.Join(NetControlChars.DataObjSeperator.Value, msg_ids)} -- {user.CurrentAuthToken} -- {user.Id}";
            }
            else
            {
                packet.Data = $"{msg_ids[0].ToString()} -- {user.CurrentAuthToken} -- {user.Id}";
            }
            packet.TaskCode = "Cmd.0000";
            return;
        }
        public static void GenerateMessagePullRequest(this Packet packet, AppUser user)
        {
            packet.Data = "token" + NetControlChars.SecondarySeperator.Value + user.CurrentAuthToken + NetControlChars.SecondarySeperator.Value + user.Id;
            packet.TaskCode = "Req.0001";
            return;
        }

        public static void GenerateContactTokenRequest(this Packet packet, AppUser user)
        {
            var deviceId = "ABC123";
            var userData = user.SerializeAppUserObj();
            packet.Data = userData + NetControlChars.SecondarySeperator.Value + deviceId;
            packet.TaskCode = "Req.0000";
            return;
        }

    }
}
