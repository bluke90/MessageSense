// See https://aka.ms/new-console-template for more information
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net.Security;






var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);


// Task.Run(() => AsynchronousAuthenticatingTcpListener.Main());
AsyncConnector.StartListening();



Console.ReadLine();





