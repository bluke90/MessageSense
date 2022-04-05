using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;

namespace MessageSenseServer.Components.Net
{
    public static class PacketProcessing
    {
        public static void ProcessContactTokenRequest(this Packet packet)
        {
            var user = AppUserExtensions.DeserializeAppUserObj(packet.Data);
            
            // did user provide the following - username, firstname
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Username))
                {
                    
                }
            }

            ServerContext context = new ServerContext();
            context.Users.Add(user);



        }


    }
}
