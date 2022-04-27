using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class PacketHandler
    {
        private List<Packet> _packetRespList { get; set; }
        private Data.MessageSenseData MessageSenseData { get; set; }
        private int _currentTransmissionId { get; set; }
        private Thread _updateThread { get; set; }

        private ManualResetEvent isSending = new ManualResetEvent(true);

        private readonly AppUser _appUser;
        private AppManager _appManager;

        public PacketHandler(AppManager manager) {

            _appManager = manager;
            _appUser = manager.AppUser;
            _packetRespList = new List<Packet>();
            _updateThread = new Thread(UpdateThread);
            _updateThread.Start();
        }

        public async Task<Packet> SendAsync(Packet packet) {
            isSending.Reset();
            Console.WriteLine($"Sending => T_ID: {packet.Data.TransmissionId}");
            var tId = packet.Data.TransmissionId;

            var resp = await packet.TransmitPacket(_appUser);
            _packetRespList.Add(resp);
            Console.WriteLine($"Searching for response => T_ID: {packet.Data.TransmissionId}");
            resp = await GetOrWaitResponse(tId);
            Console.WriteLine($"Retreived response => T_ID: {packet.Data.TransmissionId}");
            isSending.Set();
            return resp;
        }

        public async Task<Packet> SendUnauthenticatedAsync(Packet packet, AppUser appUser)
        {
            isSending.Reset();
            var tId = packet.Data.TransmissionId;

            var resp = await packet.TransmitPacket(appUser, authenticate: false);

            isSending.Set();
            return resp;
        }

        private Task<Packet> GetOrWaitResponse(int t_id)
        {
            int attempts = 0;
            while (true)
            {
                attempts += 1;
                foreach (var response in _packetRespList)
                {
                    if (response.Data.TransmissionId == t_id)
                    {
                        _packetRespList.Remove(response);
                        return Task.FromResult(response);
                    }
                    if (attempts > 50) return Task.FromResult(response);
                }
                Thread.Sleep(250);
            }
        }

        // (UpdateThread)thread => Handle Recurring transmissions (example: request for new messages)
        private void UpdateThread() {
            while (true) {
                Thread.Sleep(10000);
                // What we need to update
                // New messages - If non, move on
                Console.WriteLine("Checking for new messages....");
                var task = _appManager.SendPullMessageRequest();
                task.Wait();
                isSending.WaitOne();
            }
        }



    }
}