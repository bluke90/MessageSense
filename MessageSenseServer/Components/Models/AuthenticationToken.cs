using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSenseServer.Components.Models
{
    /*
     * Thoughts:
             
            - always remove last character before comparing to database token
            - Use DeviceId For authentication
     */
    public class AuthenticationToken {
        public int Id { get; set; }
        public int? AppUserId { get; set; }
        public string Token { get; set; }
        public string? DeviceId { get; set; }
        public DateTime Created { get; set; }
    }

    public static class Authentication
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static void AuthenticatePacket(string authToken, int appUserId, string? contactToken = null)
        {
            ServerContext context = new ServerContext();
            var token = context.AuthenticationTokens.FirstOrDefault(m => m.AppUserId == appUserId);
            if (token.Token != authToken) throw new Exception("Unable to Authenticate Packet");
            if (contactToken != null) {
                var user = context.Users.FirstOrDefault(m => m.ContactToken == contactToken);
                if (contactToken != user.ContactToken) throw new Exception("Unable to Authenticate Packet");
            }

        }

        public static async Task<bool> SimpleAuthenticatePacket(this Packet packet)
        {
            var authToken = packet.Data.AuthToken;
            var userId = packet.Data.AppUserId;

            ServerContext context = new ServerContext();
            var appUserObj = await context.Users.FirstOrDefaultAsync(m => m.Id == userId);
            var authTokenObj = await context.AuthenticationTokens.FirstOrDefaultAsync(m => m.AppUserId == userId);

            if (appUserObj != null)
            {
                if (authTokenObj != null && authTokenObj.Token == authToken) {
                    return true;
                }
            }

            return false;
        }

        
        public static void AuthenticateUser(string data)
        {
            var authToken = data.Split(" | ")[0];
            var deviceId = data.Split(" | ")[1];
            var userData = data.Split(" | ")[2];
            var user = JsonSerializer.Deserialize<AppUser>(userData);


            ServerContext context = new ServerContext();
            var token = context.AuthenticationTokens.FirstOrDefault(m => m.AppUserId == user.Id);
            if (token.Token != authToken || token.DeviceId != deviceId)
            {
                throw new Exception("Unable to Authenicate Client");
            }
            return;
        }

        public static async Task<AuthenticationToken> GenerateAuthToken(this AppUser user, string deviceId) {
            MessageSenseServer.Data.ServerContext context = new MessageSenseServer.Data.ServerContext();

            AuthenticationToken token = new AuthenticationToken()
            {
                AppUserId = user.Id,
                Token = GetNewToken(),
                Created = DateTime.Now
            };
            if (deviceId != null) {
                token.DeviceId = deviceId;
            }

            user.CurrentAuthToken = token.Token;

            context.AuthenticationTokens.Add(token);
            await context.SaveChangesAsync();
            return token;
        }

        private static string GetNewToken() {
            var random = new Random();
            var token = new string(Enumerable.Repeat(Chars, 8).Select(token => token[random.Next(token.Length)]).ToArray());

            string authToken = string.Empty;

            foreach (char c in token)
            {
                authToken += c.ToString();
                authToken += random.Next(99);
            }
            return authToken;
        }
    }


}
