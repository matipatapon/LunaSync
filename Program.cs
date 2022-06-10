// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
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
    lSocket.Listen(2);
    //Listen For incoming data !
    do{
        WriteLine("Waiting for connection !");
        Socket handler = lSocket.Accept();
        WriteLine("Connected !");
        int count = 0;
        //Reviving data !!!
        string data = string.Empty;
        var fs = new FileStream("file2.txt",FileMode.Append,FileAccess.Write);
        do{
            byte[] bytes = new byte[1024];
            count = handler.Receive(bytes);
            fs.Write(bytes,0,bytes.Length);
            
            data += Encoding.ASCII.GetString(bytes,0,count);
        }while(count!=0);
        fs.Flush();
    }while(true);
}
//Client 
void StartClient(){
    Socket sSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
    try{
        sSocket.Connect("127.0.0.1",13000);
        string message ="Witaj ziemniaku !";
        byte[] bytek = Encoding.ASCII.GetBytes(message);
        sSocket.SendFile("file.txt");
    }
    catch(SocketException e){
        WriteLine($"Failed to connect error {e.ErrorCode}");
    }
}

