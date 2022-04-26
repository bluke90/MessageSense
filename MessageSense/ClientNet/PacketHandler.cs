using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSense.ClientNet
{
    public class PacketHandler
    {
        public Queue<Packet> PacketQueue { get; set; }
        public List<Packet> PacketRespList { get; set; }
        private Data.MessageSenseData MessageSenseData { get; set; }

        private Thread ClientServerThread { get; set; }

        private ManualResetEvent waitingForResponse = new ManualResetEvent(false);

        public PacketHandler(AppUser user) {
            PacketQueue = new Queue<Packet>();
            PacketRespList = new List<Packet>(); 

            ClientServerThread = new Thread(() => ClientServerLoop(user));
            ClientServerThread.Start();
        }

        public async Task<Packet> WaitForResponse(int transmissionId) {
            Console.WriteLine($"Waiting for respone to transmissionId => {transmissionId}");
            await Task.Yield();
            waitingForResponse.Reset();
            while (true) {
                foreach (Packet packet in PacketRespList) {
                    if (packet.Data.TransmissionId == transmissionId) {
                        waitingForResponse.Set();
                        PacketRespList.Remove(packet);
                        return packet;
                    }
                }
                await Task.Delay(50);
            }
        }

        public async Task<int> QueuePacketForTransmission(Packet packet) {
            await Task.Yield();
            var id = packet.Data.TransmissionId;
            PacketQueue.Enqueue(packet);
            return id;
        }

        private async void ClientServerLoop(AppUser user) {
            while (true) {
                if (PacketQueue.Count > 0) {
                    var packet = PacketQueue.Dequeue();

                    var resp = await packet.TransmitPacketAsync(user);

                    PacketRespList.Add(resp);
                }
                Thread.Sleep(1000);
            }


        }
    }
}