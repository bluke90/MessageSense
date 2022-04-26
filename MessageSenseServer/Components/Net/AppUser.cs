using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MessageSenseServer.Data;
using Microsoft.EntityFrameworkCore;

namespace MessageSenseServer.Components.Net
{
    public class AppUser
    {
        public int Id { get; set; }
#nullable disable
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string ContactToken { get; set; }
#nullable enable
        public string? CurrentAuthToken { get; set; }
        public string? NextAuthToken { get; set; }

        public DateTime? Created { get; set; }
    }

    public static class AppUserExtensions
    {
        public static AppUser DeserializeAppUserObj(string appUserData)
        {
            var obj = JsonSerializer.Deserialize<AppUser>(appUserData);
            return obj;

        }

        public static async Task<AppUser> GetAppUser(this Packet packet)
        {
            var userId = packet.Data.AppUserId;

            ServerContext context = new ServerContext();
            var user = await context.Users.FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(userId));
            if (user == null) throw new NullReferenceException();
            return user;
        }

    }
}
