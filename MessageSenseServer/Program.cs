// See https://aka.ms/new-console-template for more information
using MessageSenseServer;
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net.Security;



// Console Appearance Settings
Console.ForegroundColor = ConsoleColor.Green;


// -----------------------------------
// Component Manager
Print("Initializing Component Manager...");
ComponentManager ComponentManager = new ComponentManager();
Print("Success");


// Network Services (AsyncConnector)
Print("Initializing Network Services....");
var hostSettings = ComponentManager.Configuration.GetSection("HostSettings");
var ip = hostSettings.GetValue<string>("IpAddress");
var port = hostSettings.GetValue<int>("Port");
AsyncConnector.StartListening(ip, port);
Print("Success");


Print("MessageSense Server Successfully Initialized and ready for connection!");
Console.ReadLine();






static void Print(string msg) {
    Console.ForegroundColor = ConsoleColor.Yellow;
    var str = $"[MainThread] {msg}";
    Console.WriteLine(str);
    Console.ForegroundColor= ConsoleColor.Green;
}

