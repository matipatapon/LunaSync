// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using static System.Console;

WriteLine("Choose mode ! server/client");
string? mode = ReadLine();
switch (mode){
    case "server":
        StartServer();
    break;
    case "client":
        StartClient();
    break;
}
//Server 
void StartServer(){
    Socket lSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    byte[] address = new byte[4];
    IPAddress ip = new IPAddress(address);
    IPEndPoint ep = new IPEndPoint(ip,13000);
    lSocket.Bind(ep);
    WriteLine("Waiting for connection !");
    lSocket.Listen(2);
    Socket handler = lSocket.Accept();
    WriteLine("Connected !");
    do{
        byte[] bytes = new byte[1024];
        int count = handler.Receive(bytes);
        string data = Encoding.ASCII.GetString(bytes,0,count);
        WriteLine($"Recived {data} !");
    }while(1==2);
}
//Client 
void StartClient(){
    Socket sSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
    try{
        sSocket.Connect("127.0.0.1",13000);
        string message ="Witaj ziemniaku !";
        byte[] bytek = Encoding.ASCII.GetBytes(message);
        sSocket.Send(bytek,0);
    }
    catch(SocketException e){
        WriteLine($"Failed to connect error {e.ErrorCode}");
    }
}

