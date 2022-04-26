using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using SocketTesting.ClientNet;
using SocketTesting;

//AppManager app = new AppManager();

// Thread.Sleep(2);

// var user = await Authentication.NewUserNegotiation("bluke", "blake");

// Send Message

ManualResetEvent reset1 = new ManualResetEvent(true);

dosomething2();
dosomething1();

Console.ReadLine();


void dosomething1() {
    reset1.WaitOne();
    Console.WriteLine("task1");
}

void dosomething2() {
    reset1.Reset();
    Console.WriteLine("task2");
    
}

void dosomething3() {

    Console.WriteLine("task1");
}


