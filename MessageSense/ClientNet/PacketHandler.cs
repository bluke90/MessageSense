using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class PacketHandler
    {
        public bool Connected = false;

        private Queue<Packet> packetQueue = new Queue<Packet>();
        private List<Packet> _packetRespList { get; set; }
        private Data.MessageSenseData MessageSenseData { get; set; }
        private int _currentTransmissionId { get; set; }
        private Thread _updateThread { get; set; }
        private Thread _sendThread { get; set; }

        // private ManualResetEvent isSending = new ManualResetEvent(true);

        private readonly AppUser _appUser;
        private AppManager _appManager;

        public PacketHandler(AppManager manager) {

            _appManager = manager;
            _appUser = manager.AppUser;
            packetQueue = new Queue<Packet>();
            _packetRespList = new List<Packet>();
            Connected = SendConnectionTest();
            _sendThread = new Thread(SendThread);
            _sendThread.Start();
            _updateThread = new Thread(UpdateThread);
            _updateThread.Start();
        }

        public void QueuePacket(Packet packet) {
            // isSending.Reset();
            Console.WriteLine($"Queuing => T_ID: {packet.Data.TransmissionId}");
            var tId = packet.Data.TransmissionId;
            packetQueue.Enqueue(packet);
            // Console.WriteLine($"Searching for response => T_ID: {packet.Data.TransmissionId}");
            // var resp = await GetOrWaitResponse(tId);
            
            // isSending.Set();
            
        }

        public void Send() {
            var packet = packetQueue.Dequeue();
            var t_id = packet.Data.TransmissionId;
            var packetData = packet.PreparePacketForTransmission(_appUser);
            var resp = SynchronousClient.SendPacket(packetData);
            var respPacket = PacketUtils.AnalyseResposnePacket(resp);
            _packetRespList.Add(respPacket);
        }

        public bool SendConnectionTest() {
            var packet = PacketUtils.GeneratePacket();
            packet.TaskCode = TaskCodes.ConnectionTest;
            packet.Data.TransmissionId = 0;
            var packetData = packet.PreparePacketForTransmission(_appUser, auth: false);
            try {
                var resp = SynchronousClient.SendPacket(packetData);
                var respPacket = PacketUtils.AnalyseResposnePacket(resp);
                if (respPacket.Data.TransmissionId != 0) throw new Exception("Unkown Error when processing connection test response | PacketHandler.cs => PacketHandler.SendConnectionTest()");
                var result = bool.Parse(respPacket.Data.Data);
                return result;
            } catch (Exception ex) {
                Console.WriteLine("Unable to verify connection integrity...");
                return false;
            }
        }

        public Packet SendUnauthenticatedAsync(Packet packet, AppUser appUser)
        {
            // isSending.Reset();
            var tId = packet.Data.TransmissionId;

            packet.PreparePacketForTransmission(appUser);
            var packetData = packet.PreparePacketForTransmission(appUser, auth: false);
            var resp = SynchronousClient.SendPacket(packetData);
            var respPacket = PacketUtils.AnalyseResposnePacket(resp);
            if (respPacket.Data.TransmissionId != tId) throw new Exception("Unkown Error when processing connection test response | PacketHandler.cs => PacketHandler.SendConnectionTest()");

            return respPacket;
        }

        public Packet GetOrWaitResponse(Packet packet)
        {
            var t_id = packet.Data.TransmissionId;
            int attempts = 0;
            Console.WriteLine($"Searching for response => T_ID: {t_id}");
            while (true)
            {
                attempts += 1;
                foreach (var response in _packetRespList)
                {
                    if (response.Data.TransmissionId == t_id)
                    {
                        Console.WriteLine($"Retreived response => T_ID: {response.Data.TransmissionId}");
                        _packetRespList.Remove(response);
                        return response;
                    }
                }
                Thread.Sleep(100);
            }
        }

        // (UpdateThread)thread => Handle Recurring transmissions (example: request for new messages)
        private void SendThread() {
            while (true) {
                Thread.Sleep(50);
                while (packetQueue.Count > 0) { // Send queued packets
                    var packet = packetQueue.Dequeue();
                    var t_id = packet.Data.TransmissionId;
                    var packetData = packet.PreparePacketForTransmission(_appUser);
                    var resp = SynchronousClient.SendPacket(packetData);
                    var respPacket = PacketUtils.AnalyseResposnePacket(resp);
                    _packetRespList.Add(respPacket);
                }
                // What we need to update
                // New messages - If non, move on
            }
        }
        private async void UpdateThread() {
            while (true) {
                Thread.Sleep(5000);
                Console.WriteLine("Checking for new messages....");
                var msgCount = await _appManager.SendPullMessageRequest();
                // isSending.WaitOne();
            }
        }


    }
}