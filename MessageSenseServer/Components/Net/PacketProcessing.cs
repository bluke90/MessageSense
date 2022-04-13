using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using Microsoft.EntityFrameworkCore;

namespace MessageSenseServer.Components.Net
{
    public static class PacketProcessing
    {
        public static void PacketException(this Packet packet, string exceptionCode)
        {
            packet.Resposne = $"Exception.{exceptionCode}";
            return;
        }

        public static async void ProcessContactTokenRequest(this Packet packet)
        {
            var user = AppUserExtensions.DeserializeAppUserObj(packet.Data);
            


            // did user provide the following - username, firstname
            if (user == null || string.IsNullOrEmpty(user.ContactToken) || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.FirstName))
            {
                packet.PacketException("0000");
                return;
            }

            ServerContext context = new ServerContext();
            // Check that contact token is unuique
            var _appUser = await context.Users.Where(u => u.ContactToken == user.ContactToken).FirstOrDefaultAsync();
            if (_appUser != null)
            {
                packet.PacketException("0001");
                return;
            }
            // Store AppUser instance
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Set response
            var respString = $"Cmd.0002 | {user.ContactToken},{user.Username}";
            packet.Resposne = respString;
            return;
        }


    }
}
