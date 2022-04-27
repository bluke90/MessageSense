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

        public static async Task SendStoreMessageRequest(this Message msg, AppManager app) {
            Console.WriteLine("Attempting SendStoreMessageRequest");
            try {
                var packet = PacketUtils.GeneratePacket();
                await packet.GenerateMessageStoreRequest(msg);
                var respPacket = await app.PacketHandler.SendAsync(packet);
                if (respPacket.Data.Data != "OK") throw new Exception("Unkown resposne for StoreMessageRequest");
            } catch (Exception ex) {
                Console.WriteLine("Exception Location => Message.cs => MessgaeUtils.SendStoreMessageRequest");
                Console.WriteLine(ex.ToString());
            }

        }
        public static async Task<int> SendPullMessageRequest(this AppManager appManager)
        {
            Console.WriteLine("Attempted SendPullMessageRequest");
            var packet = PacketUtils.GeneratePacket();

            await packet.GenerateMessagePullRequest();

            try
            {
                var respPacket = await appManager.PacketHandler.SendAsync(packet);
                var respData = respPacket.Data;

                if (respData.TaskCode == "Cmd.0004") { return 0; }
                if (respData.TaskCode != "Cmd.0003") throw new Exception("Error pulling Packet Response in SendPullMessageRequest => Message.cs => 40-50");

                var msgList = new List<Message>();
                var msgIdList = new List<int>();

                var msgs = respData.Data.Split(NetControlChars.PrimarySeperator.Value)[1];

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
                await SendMessageReceivedConfirmation(msgIdList, appManager);

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

        private static async Task SendMessageReceivedConfirmation(List<int> msg_ids, AppManager appManager)
        {
            Console.WriteLine("Attempting SendMessageReceivedConfirmation");
            try
            {
                var packet = PacketUtils.GeneratePacket();
                await packet.GenerateMessageReceivedConfirmation(msg_ids);

                var respPacket = await appManager.PacketHandler.SendAsync(packet);
                var respData = respPacket.Data;
                

                if (respData.Data.Contains("OK"))
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
