using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.Models
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
}
