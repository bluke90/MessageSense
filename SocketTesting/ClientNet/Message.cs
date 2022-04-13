using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SocketTesting.ClientNet
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
    }
}
