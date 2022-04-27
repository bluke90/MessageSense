using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MessageSenseServer.Components.Net
{
    public static class PacketProcessing
    {
        public static void PacketException(this Packet packet, PacketExceptionCode exception)
        {
            packet.Data.TaskCode = "Exception";
            packet.Data.Data = $"{exception}";
            return;
        }

        public enum PacketExceptionCode
        {
            ProcessingError = 0,
            AuthenticationError = 1000
        }
        
        // use Authentication.AuthenticatePacket();
        public static async Task ProcessMessageStoreRequest(this Packet packet)
        {
            var authenticated = await packet.SimpleAuthenticatePacket();
            if (!authenticated) packet.PacketException(PacketExceptionCode.AuthenticationError);

            var msg = JsonSerializer.Deserialize<Message>(packet.Data.Data);
            var ClientMsgId = msg.Id;
            ServerContext context = new ServerContext();
            msg.Id = 0;
            context.Messages.Add(msg);
            await context.SaveChangesAsync();

            packet.Data.TaskCode = "Cmd.0000";
            packet.Data.Data = ClientMsgId.ToString();
            return;
        }

        public static async Task ProcessMessagePullRequest(this Packet packet)
        {
            var appUser = await packet.GetAppUser();
            var authenticated = await packet.SimpleAuthenticatePacket();
            if (!authenticated) packet.PacketException(PacketExceptionCode.AuthenticationError);

            var token = packet.Data.Data;

            ServerContext context = new ServerContext();
            var msgs = await context.Messages.Where(m => m.RecipientToken == appUser.ContactToken).ToListAsync();

            if (msgs.Count() < 1) {
                packet.Data.TaskCode = "Cmd.0004";
                return;
            }

            packet.Data.TaskCode = "Cmd.0003";
            var respMsgs = string.Empty;

            for (int i = 0; i < msgs.Count; i++) {
                respMsgs += JsonSerializer.Serialize(msgs[i]);
                if (i != msgs.Count - 1) respMsgs += " <|> ";
            }
            packet.Data.Data = respMsgs;
            return;
        }

        public static async Task ProcessMessagesReceivedReqeust(this Packet packet)
        {
            var authenticated = await packet.SimpleAuthenticatePacket();
            if (!authenticated) packet.PacketException(PacketExceptionCode.AuthenticationError);

            var msg_ids = packet.Data.Data;
            var idArray = msg_ids.Split(" <|> ");

            ServerContext context = new ServerContext();

            foreach (var id in idArray)
            {
                var int_id = Convert.ToInt32(id);
                var msg = await context.Messages.FirstOrDefaultAsync(m => m.Id == int_id);
                if (msg != null)
                {
                    context.Messages.Remove(msg);
                }
            }
            await context.SaveChangesAsync();
            packet.Data.Data = "OK";
            return;
        }


        public static async Task ProcessContactTokenRequest(this Packet packet)
        {
            var userData = packet.Data.Data.Split(" -- ")[0];
            var deviceId = packet.Data.Data.Split(" -- ")[1];

            var user = AppUserExtensions.DeserializeAppUserObj(userData);
            
            // did user provide the following - username, firstname
            if (user == null || string.IsNullOrEmpty(user.ContactToken) || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.FirstName))
            {
                packet.PacketException(PacketExceptionCode.ProcessingError);
                return;
            }

            await user.GenerateAuthToken(deviceId);

            ServerContext context = new ServerContext();
            // Check that contact token is unuique
            var _appUser = await context.Users.Where(u => u.ContactToken == user.ContactToken).FirstOrDefaultAsync();
            if (_appUser != null)
            {
                packet.PacketException(PacketExceptionCode.ProcessingError);
                return;
            }

            user.Created = DateTime.Now;
            // Store AppUser instance
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Set response
            var respString = $"{user.ContactToken} -- {user.Username} -- {user.CurrentAuthToken} -- {user.Id}";
            packet.Data.TaskCode = "Cmd.0002";
            packet.Data.Data = respString;
            return;
        }


    }
}
