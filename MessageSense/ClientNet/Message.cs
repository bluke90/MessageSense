using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MessageSense.Data;

namespace MessageSense.ClientNet
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

        public static async Task SendStoreMessageRequest(this Message msg, AppUser user) {
            var packet = PacketUtils.GeneratePacket();
            await packet.GenerateMessageStoreRequest(msg, user);
            var resp = await packet.TransmitPacketAsync();
            if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] == "Cmd.0000" && resp.Split(NetControlChars.PrimarySeperator.Value)[1] == msg.Id.ToString()) {
                return;
            }
        }
        public static async Task<int> SendPullMessageRequest(this Models.Contact contact, AppManager appManager)
        {
            var packet = PacketUtils.GeneratePacket();
            await packet.GenerateMessagePullRequest(contact, appManager.AppUser);

            try
            {
                var resp = await packet.TransmitPacketAsync();
                if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] == "Cmd.0004") return 0;
                if (resp.Split(NetControlChars.PrimarySeperator.Value)[0] != "Cmd.0003") throw new Exception($"Unkown TaskCode => {resp.Split(NetControlChars.PrimarySeperator.Value)[0]}");

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
                await SendMessageReceivedConfirmation(msgIdList, appManager.AppUser);

                var data = new Data.MessageSenseData();
                await data.Messages.AddRangeAsync(msgList);
                await data.SaveChangesAsync();

                return msgList.Count;
            } catch (Exception ex) {
                Console.WriteLine("Unknown Exception in Method => SendPullMessageRequest | File => Message.cs | Line => 37");
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        private static async Task SendMessageReceivedConfirmation(List<int> msg_ids, AppUser user)
        {
            try
            {
                var packet = PacketUtils.GeneratePacket();
                await packet.GenerateMessageReceivedConfirmation(msg_ids, user);
                var resp = await packet.TransmitPacketAsync();
                if (resp == "OK")
                {
                    return;
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            // Confirmation not received
            
        }
    }
}
