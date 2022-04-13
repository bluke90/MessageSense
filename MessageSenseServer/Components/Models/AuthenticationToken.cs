using MessageSenseServer.Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenseServer.Components.Models
{
    public class AuthenticationToken
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
    }

    public static class Authentication
    {
        public static AuthenticationToken GenerateAuthToken(this AppUser user)
        {
            AuthenticationToken token = new AuthenticationToken()
            {
                AppUserId = user.Id,
                Token = GetNewToken(),
                Created = DateTime.Now
            };
            return token;
        }

        private static string GetNewToken()
        {
            throw new NotImplementedException();
        }
    }


}
