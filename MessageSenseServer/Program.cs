// See https://aka.ms/new-console-template for more information
using MessageSenseServer.Components.Models;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Data;
using System.Net.Security;


// Task.Run(() => AsynchronousAuthenticatingTcpListener.Main());
AsyncConnector.StartListening();



Console.ReadLine();

