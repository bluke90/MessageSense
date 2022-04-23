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
        public AppUser AppUser { get; set; }
        private Thread RefreshRequestLoop;

        //public ObservableCollection<Packet> PacketQueue { get; set; }

        public bool isSending;
        public bool connectionEstablished;

        public AppManager()
        {
            MessageSenseData = new Data.MessageSenseData();
            isSending = false;
            connectionEstablished = true;
            // PerformHandshake();
            Task.Run(() => PullMessages());
            // RefreshRequestLoop = new Thread(() => PullMessages());
            // RefreshRequestLoop.Start();




            //PacketQueue = new ObservableCollection<Packet>();
            //PacketQueue.CollectionChanged += TransmistQueuedPacket;
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

        private async void PerformHandshake()
        {
            var packet = PacketUtils.GeneratePacket();
            packet.TaskCode = "Req.x";
            packet.Data = " ::: " + // Blank -- authToken -- userId
                " -- " + AppUser.CurrentAuthToken + 
                " -- " + AppUser.Id.ToString();
            var resp = await packet.TransmitPacketAsync();
            if (resp == "OK")
            {
                connectionEstablished = true;
            }
        }

        private async void PullMessages()
        {
            while (true)
            {
                Thread.Sleep(2500);
                if (connectionEstablished && !isSending)
                {
                    foreach (var contact in await MessageSenseData.Contacts.ToListAsync())
                    {
                        
                        if (isSending) break;
                        await contact.SendPullMessageRequest(this);
                        //var count = await contact.SendPullMessageRequest(this);
                    }
                } else {
                    Thread.Sleep(5000);
                }
                
            }
        }

    }
}
