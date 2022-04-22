using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
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

    public static class AppUserExtensions
    {
        public static AppUser DeserializeAppUserObj(string appUserData)
        {
            var obj = JsonSerializer.Deserialize<AppUser>(appUserData);
            return obj;

        }
        public static string SerializeAppUserObj(this AppUser user)
        {
            var data = JsonSerializer.Serialize<AppUser>(user);
            return data;
        }
    }
}
