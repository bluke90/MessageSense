using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using SocketTesting.ClientNet;

namespace SocketTexting.ClientNet
{
    public class Message
    {
        public int Id { get; set; }
        public string RecipientToken { get; set; }
        public string SenderToken { get; set; }
        public DateTime DateTime { get; set; }
        public string Data { get; set; }
        public bool Read { get; set; }
    }

    public static class MessageUtils
    {
        public static string SearializeMessage(this Message msg)
        {
            var data = JsonSerializer.Serialize(msg);
            return data;
        }

        public static async Task SendStoreMessageRequest(this Message msg, AppUser user)
        {
            var packet = PacketUtils.GeneratePacket();
            await packet.GenerateMessageStoreRequest(msg, user);
            var resp = await packet.TransmitPacketAsync();
            if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] == "Cmd.0000" && resp.Split(NetControlChars.PrimarySeperator.Value)[1] == msg.Id.ToString())
            {
                return;
            }
        }
        public static async Task<int> SendPullMessageRequest(AppUser user)
        {
            var packet = PacketUtils.GeneratePacket();
            packet.GenerateMessagePullRequest(user);
            var resp = await packet.TransmitPacketAsync();

            if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] == "Cmd.0004") return 0;
            if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] != "Cmd.0003") throw new Exception();

            var msgList = new List<Message>();
            var msgIdList = new List<int>();

            var msgs = resp.Split(NetControlChars.PrimarySeperator.Value)[1];

            if (msgs.Contains(NetControlChars.DataObjSeperator.Value))
            {
                var msgArray = msgs.Split(NetControlChars.DataObjSeperator.Value);
                foreach (var msg in msgArray)
                {
                    var msgObj = JsonSerializer.Deserialize<Message>(msg);
                    msgIdList.Add(msgObj.Id);
                    msgList.Add(msgObj);
                }
            }
            else
            {
                var msgObj = JsonSerializer.Deserialize<Message>(msgs);
                msgIdList.Add(msgObj.Id);
            }


            // Send Messages Received Confirmation
            await SendMessageReceivedConfirmation(msgIdList, user);

            // MessageSenseData data = new MessageSenseData();
            // var addTask = data.Messages.AddRangeAsync(msgList);
            // await data.SaveChangesAsync();
            // addTask.Wait();

            return msgList.Count;
        }

        private static async Task SendMessageReceivedConfirmation(List<int> msg_ids, AppUser user)
        {
            var packet = PacketUtils.GeneratePacket();
            packet.GenerateMessageReceivedConfirmation(msg_ids, user);
            var resp = await packet.TransmitPacketAsync();
            if (resp == "OK")
            {
                return;
            }
            // Confirmation not received

        }
    }
}
