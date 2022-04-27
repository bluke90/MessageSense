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
        private Thread _updateThread { get; set; }

        private ManualResetEvent NonReccuringTransmissionActive = new ManualResetEvent(true);
        private ManualResetEvent UpdateInProgress = new ManualResetEvent(false);

        private readonly AppUser _appUser;
        private AppManager _appManager;

        public PacketHandler(AppManager manager) {

            _appManager = manager;
            _appUser = manager.AppUser;
            _updateThread = new Thread(UpdateThread);
            _updateThread.Start();
        }

        public async Task<Packet> SendAsync(Packet packet) {
            NonReccuringTransmissionActive.Reset();
            UpdateInProgress.WaitOne();

            var resp = await packet.TransmitPacketAsync(_appUser);
            
            NonReccuringTransmissionActive.Set();
            return resp;

        }

        public async Task<Packet> SendUpdatePacketAsync(Packet packet) {
            UpdateInProgress.Reset();
            NonReccuringTransmissionActive.WaitOne();

            var resp = await packet.TransmitPacketAsync(_appUser);

            UpdateInProgress.Set();
            return resp;

        }

        // (UpdateThread)thread => Handle Recurring transmissions (example: request for new messages)
        private async void UpdateThread() {
            while (true) {
                Thread.Sleep(4000);
                // What we need to update
                // New messages - If non, move on
                Console.WriteLine("Checking for new messages....");
                var count = await _appManager.SendPullMessageRequest();
                if (count < 1) Console.WriteLine("No new Messages");
            }
        }



    }
}