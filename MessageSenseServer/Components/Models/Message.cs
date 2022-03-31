using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSenseServer.Components.Models
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

    public static class MessageExtensions
    {
        public static string SearlizeMessage(this Message msg)
        {
            var data = JsonSerializer.Serialize<Message>(msg);
            return data;
        }
        
        public static Message DeserializeMessage(string jsonStr)
        {
            Message msgObj = JsonSerializer.Deserialize<Message>(jsonStr);
            return msgObj;
        }
    }

}
