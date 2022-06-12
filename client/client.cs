using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using static System.Console;

namespace host;
public class client
{
    public void StartClient(){
        Socket sSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        try{
            sSocket.Connect("127.0.0.1",13000);
            string message ="Witaj ziemniaku !";
            byte[] bytek = Encoding.ASCII.GetBytes(message);
            sSocket.SendFile("file.mp4");
        }
        catch(SocketException e){
            WriteLine($"Failed to connect error {e.ErrorCode}");
        }

    }
}
