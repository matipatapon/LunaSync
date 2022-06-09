// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using static System.Console;
StartServer();
//Server 
void StartServer(){
    Socket lSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    byte[] address = new byte[4];
    IPAddress ip = new IPAddress(address);
    IPEndPoint ep = new IPEndPoint(ip,13000);
    lSocket.Bind(ep);
    WriteLine("Waiting for connection !");
    lSocket.Listen(2);
    Socket con = lSocket.Accept();
    WriteLine("Connected !");
}

