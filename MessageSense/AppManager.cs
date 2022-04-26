using MessageSense.ClientNet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSense
{
    public class AppManager
    {
        public Data.MessageSenseData MessageSenseData { get; set; }
        public PacketHandler PacketHandler { get; set; }
        public AppUser AppUser { get; set; }


        public bool connectionEstablished;

        public AppManager()
        {
            MessageSenseData = new Data.MessageSenseData();
            connectionEstablished = true;
            Console.WriteLine("Starting Pull Message Refreash Thread");
            SetAppUser();
            while (AppUser == null) { continue; }
            PacketHandler = new PacketHandler(this);   
        }

        public void SetAppUser()
        {
            var appUser = Preferences.Get("appUser", "");

            if (!String.IsNullOrEmpty(appUser))
            {
                AppUser = JsonSerializer.Deserialize<AppUser>(appUser);
                Console.WriteLine("AppUser Instance Initiated");
            }
            return;
        }

    }
}
