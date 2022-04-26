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
        private Thread RefreshRequestLoop;

        //public ObservableCollection<Packet> PacketQueue { get; set; }

        public bool connectionEstablished;

        public AppManager()
        {
            MessageSenseData = new Data.MessageSenseData();
            connectionEstablished = true;
            // PerformHandshake();
            Console.WriteLine("Starting Pull Message Refreash Thread");
            RefreshRequestLoop = new Thread(() => PullMessages());

            //PacketQueue = new ObservableCollection<Packet>();
            //PacketQueue.CollectionChanged += TransmistQueuedPacket;
        }

        public async Task InitPacketHandler() {
            await SetAppUser();

            PacketHandler = new PacketHandler(AppUser);
            RefreshRequestLoop.Start();

        }

        public async Task SetAppUser()
        {
            await Task.Yield();
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
            //packet.TaskCode = "Req.x";
            packet.Data.Data = " ::: ";
            //var resp = await packet.TransmitPacketAsync();
            //if (resp == "OK")
            //{
            //    connectionEstablished = true;
            //}
        }

        private async void PullMessages()
        {

            Console.WriteLine("Started Pull Messages Refreash loop");
            while (true) {
                Thread.Sleep(5000);
                
                if (connectionEstablished) {

                    Console.WriteLine("Pulling new messages...");
                    foreach (var contact in await MessageSenseData.Contacts.ToListAsync()) {

                        var count = await contact.SendPullMessageRequest(this);
                    }

                } else {
                    Thread.Sleep(5000);
                }
                
            }
        }

    }
}
