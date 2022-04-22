using MessageSense.ClientNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSense
{
    public class AppManager
    {
        public Data.MessageSenseData MessageSenseData { get; set; }
        public AppUser AppUser { get; set; }
        public AppManager()
        {
            MessageSenseData = new Data.MessageSenseData();
        }

        public void SetAppUser()
        {
            var appUser = Preferences.Get("appUser", "");

            if (!String.IsNullOrEmpty(appUser))
            {
                AppUser = JsonSerializer.Deserialize<AppUser>(appUser);
            }
            return;
        }

    }
}
