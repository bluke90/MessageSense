using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public static class Authentication
    {

        public static void AuthenticateClientDevice(this AppUser user)
        {
            throw new NotImplementedException();    
        }

        private static string AppUserAuthTokenData(AppUser appUser)
        {
            string authtoken = appUser.CurrentAuthToken;
            string deviceId = "0123abcd";

            var userData = JsonSerializer.Serialize<AppUser>(appUser);

            string authData = $"{authtoken} | {deviceId} | {userData}";
            return authData;
        }

        private static AppUser GenerateAppUser(string username, string firstname)
        {
            AppUser appUser = new AppUser();
            appUser.Username = username;
            appUser.FirstName = firstname;
            // Generate Contact Token
            Random random = new Random();
            var token = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                string num = random.Next(9).ToString();
                token += num;
            }
            appUser.ContactToken = token;
            return appUser;
        }

        public static async Task<AppUser> NewUserNegotiation(string username, string firstname)
        {
            var packet = PacketUtils.GeneratePacket();
            AppUser user; string resp;


            while (true)
            {
                user = GenerateAppUser("bluke", "blake");

                packet.GenerateContactTokenRequest(user);

                resp = await packet.TransmitPacketAsync();

                var resp_split = resp.Split(" | ");
                if (resp_split[0] != "Cmd.0002" || resp_split[1] == user.ContactToken || resp_split[2] == user.Username) break;
            }
            user.CurrentAuthToken = resp.Split(" | ")[3];
            user.Id = int.Parse(resp.Split(" | ")[4]);

            if (user.CurrentAuthToken == null || user.CurrentAuthToken.Length < 1) throw new Exception("Connection Error");

            return user;
        }

    }
}
