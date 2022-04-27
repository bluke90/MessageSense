using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Maui;
using System.Security.Cryptography;

namespace MessageSense.ClientNet
{
    public class Packet
    {
        public ManualResetEvent isTransmiting= new ManualResetEvent(true);

        public TaskCodes TaskCode { get; set; }
        public PacketData Data { get; set; }

    }

    public class PacketData
    {
        public int TransmissionId { get; set; }

        // MainPacket => transmissionId | TaskCode | Data | <EOF>
        // MainPacket.Data = Data -- authToken -- userId
        public string TaskCode { get; set; }
        public string Data { get; set; }
        public string AuthToken { get; set; }
        public int AppUserId { get; set; }

    }

    public class TaskCodes
    {
        private TaskCodes(string value) { Value = value; }

        public string Value { get; private set; }

        public static TaskCodes ContactTokenRequest { get { return new TaskCodes("Req.0000"); } }
        public static TaskCodes MessagePullRequest { get { return new TaskCodes("Req.0001"); } }
        public static TaskCodes StoreMessageRequest { get { return new TaskCodes("Req.0002"); } }
        public static TaskCodes MessagesReceived { get { return new TaskCodes("Cmd.0000"); } }
        public static TaskCodes NewAuthTokenReceived { get { return new TaskCodes("Cmd.0001"); } }
        public static TaskCodes ContactTokenReceived { get { return new TaskCodes("Cmd.0002"); } }
        public static TaskCodes MessagesPullReceived { get { return new TaskCodes("Cmd.0003"); } }

        public static TaskCodes Parse(string taskCode) {
            switch (taskCode) {
                case "Req.0001":
                    return TaskCodes.ContactTokenRequest;
            }
            return null;
        }
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
            packet.Data = new PacketData()
            {
                TransmissionId = RandomNumberGenerator.GetInt32(100, 500),

            };
            return packet;
        }

        public static Task<Packet> TransmitPacket(this Packet packet, AppUser user)
        {
            Console.WriteLine($"Transmiting Packet Data with TransmissionId => {packet.Data.TransmissionId}");
            Packet respPacket = null;
            packet.Data.TaskCode = packet.TaskCode.Value;
            try {
                packet.Data.AuthToken = user.CurrentAuthToken;
                packet.Data.AppUserId = user.Id;

                var packetData = JsonSerializer.Serialize(packet.Data);

                var resp_data = AsynchronousClient.StartClient(packetData);

                var array = resp_data.Split(NetControlChars.PrimarySeperator.Value);
                var respPacketData = JsonSerializer.Deserialize<PacketData>(array[0]);
                respPacket = new Packet() { Data = respPacketData };
                Console.WriteLine("Received: " + array[0]);

                return Task.FromResult(respPacket);
            } catch (Exception ex) {
                Console.WriteLine("Exception Location => Packet.cs => PacketUtils.TransmitPacket");
                Console.WriteLine(ex.ToString());
            }
            return Task.FromResult(respPacket);
        }

        public static Task<Packet> Transmit

        // TaskObjects
        public static async Task GenerateMessageStoreRequest(this Packet packet, Message msg)
        {
            await Task.Yield();
            packet.Data.Data = $"{JsonSerializer.Serialize(msg)}";
            packet.TaskCode = TaskCodes.StoreMessageRequest;
            return;
        }
        public static async Task GenerateMessageReceivedConfirmation(this Packet packet, List<int> msg_ids)
        {
            await Task.Yield();
            if (msg_ids.Count > 1) {
                packet.Data.Data = $"{string.Join(NetControlChars.DataObjSeperator.Value, msg_ids)}";
            } else {
                packet.Data.Data = $"{msg_ids[0].ToString()}";
            }
            packet.TaskCode = TaskCodes.MessagesPullReceived;
            return;
        }
        public static async Task GenerateMessagePullRequest(this Packet packet)
        {
            await Task.Yield();
            packet.TaskCode = TaskCodes.MessagePullRequest;
            return;
        }

        public static void GenerateContactTokenRequest(this Packet packet , AppUser user)
        {
            var deviceId = "ABC123";
            var userData = user.SerializeAppUserObj();
            packet.Data.Data = userData + NetControlChars.SecondarySeperator.Value + deviceId;
            packet.TaskCode = TaskCodes.ContactTokenRequest;
            return;
        }

    }
}
