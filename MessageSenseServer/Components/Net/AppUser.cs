using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenseServer.Components.Net
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string ContactToken { get; set; }
        public string CurrentAuthToken { get; set; }
        public string NextAuthToken { get; set; }

        public DateTime Created { get; set; }



    }
}
