using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class PacketHandler
    {
        private Data.MessageSenseData MessageSenseData { get; set; }
        private int _currentTransmissionId { get; set; }
        private Thread ClientServerThread { get; set; }

        private ManualResetEvent NonReccuringTransmissionActive = new ManualResetEvent(true);
        private ManualResetEvent UpdateInProgress = new ManualResetEvent(true);

        private readonly AppUser _appUser;
        private AppManager _appManager;

        public PacketHandler(AppManager manager) {

            _appManager = manager;
            _appUser = manager.AppUser;
            
            // ClientServerThread = new Thread(() => ClientServerLoop());
            // ClientServerThread.Start();
        }

        public async Task<Packet> SendAsync(Packet packet) {
            NonReccuringTransmissionActive.Reset();
            UpdateInProgress.WaitOne();
            var resp = await packet.TransmitPacketAsync(_appUser);
            
            NonReccuringTransmissionActive.Set();
            return resp;

        }


        // (UpdateThread)thread => Handle Recurring transmissions (example: request for new messages)
        private async void UpdateThread() {
            while (true) {
                Thread.Sleep(4000);
                // Stop if non reuccuring transmission active
                NonReccuringTransmissionActive.WaitOne();
                // Block until finished
                UpdateInProgress.Set();
                // What we need to update
                // New messages - If non, move on
                Console.WriteLine("Checking for new messages....");
                var count = await _appManager.SendPullMessageRequest();
                if (count < 1) Console.WriteLine("No new Messages");
                
                UpdateInProgress.Reset();
            }
        }



    }
}