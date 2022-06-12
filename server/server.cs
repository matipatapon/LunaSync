using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;

namespace host;
public class server
{
    public void StartServerThread(){
        ThreadStart server_ext= new ThreadStart(StartServer);
        Thread server_thread = new Thread(server_ext);
        server_thread.Start();
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
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
        var fs = new FileStream("file2.mp4",FileMode.Create,FileAccess.Write);
        do{
            byte[] bytes = new byte[1_048_578];
            count = handler.Receive(bytes);
            fs.Write(bytes,0,count);
        }while(count!=0);
        fs.Flush();
    }while(true);
}
}
