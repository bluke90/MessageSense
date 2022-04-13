using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using SocketTesting.ClientNet;


var contactToken = "41882173";

var packet = PacketUtils.GeneratePacket();
packet.GenerateContactTokenRequest(contactToken, "bluke", "blake");

var resp = packet.TransmistPacket();

// Handle Response


Console.ReadLine();






