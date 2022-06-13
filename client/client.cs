using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using static System.Console;
using System.Threading;
using System.Text.RegularExpressions;
namespace host;
/// <summary>
/// Client side of the FOSSYNC.
/// </summary>
public class client
{
    public void StartClientThread(){
        ThreadStart client_ext = new ThreadStart(StartClient);
        Thread client_thread = new Thread(client_ext);
        client_thread.Start();
    }
    /// <summary>
    /// Starting client side of the FOSSync
    /// </summary>
    void StartClient(){
        WriteLine("Hello there ^^");
        Write("Please enter first dir path : ");
        string path1 = ReadLine();
        get_ip(path1);

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

    /// <summary>
    /// Getting IPv4 address from path 
    /// Return 0.0.0.0 if failed 
    /// </summary>
    /// <param name="path">Path to dir address/dir </param>
    /// <returns></returns>
    public IPAddress get_ip(string path){
        byte[] bytes = new byte[4];

        string isIpFormatVaild = "127";
        string octet = "([0-9]{1,4})";
        bool isMatch = Regex.IsMatch(path,isIpFormatVaild);
        if(isMatch){
            int i = 0;
            foreach(Match match in Regex.Matches(path,octet)){
                bytes[i++] = (byte)int.Parse(match.Value);
                WriteLine($"Octet is {match.Value}");

            }
            
        }
        IPAddress ip4 = new IPAddress(bytes);
        return ip4;
    }
}
