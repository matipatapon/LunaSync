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
        WriteLine($"False is {false.ToString()} True is {true.ToString()}");
        string? path1 = ReadLine();
        
        while(path1 == null){
            WriteLine("You must insert correct path !");
            path1 = ReadLine();
        }
        
        
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

    public void sandbox(){
        object[] info = new object[3];
        info = getInfoFromPath("127.0.0.1:50/home/itam");
        WriteLine(info.ToString());
    }

    /// <summary>
    /// Getting IPv4 address from path 
    /// Returns 0.0.0.0 if failed 
    /// </summary>
    /// <param name="path">selected dir address:port/dir </param>
    /// <returns>Return address IPv4</returns>
    public IPAddress get_ip(string path){
        byte[] bytes = new byte[4];

        string octet = @"[1-2]?[0-9]{1,3}";
        int i = 0;
        foreach(Match match in Regex.Matches(path,octet)){
            int value = int.Parse(match.Value);
            //Don't go for 5th octet in ip address !
            if(i > 3){
                break;
            }

            //Value is too large !!! (to not overcomplicate regex) 
            if (value > 255){
                bytes = new byte[4];
                break;
            }

            bytes[i++] = (byte)value;
            WriteLine($"Octet is {match.Value}");

        }
    
        
        IPAddress ip4 = new IPAddress(bytes);
        return ip4;
    }

    /// <summary>
    /// Separate path to directory from path (serveraddress:port/directory)
    /// </summary>
    /// <param name="path">selected dir address:port/dir </param>
    /// <returns>return path ex /home/itam/whatever/ </returns>
    public string get_path(string path){
        string pattern = @"/(\w+/)*(\w+)/?$";
        string result = "";
        bool isMatch = Regex.IsMatch(path,pattern);
        if(isMatch){
            result = Regex.Match(path,pattern).ToString();
            if(result[result.Length-1] != '/'){
                result += "/";
            }
        }
        return result;
    }
    /// <summary>
    /// Separete port from path
    /// </summary>
    /// <param name="path">selected dir</param>
    /// <returns>port int</returns>
    public uint get_port(string path){
        string pattern = @":\d*";
        uint result = 0;
        if(Regex.IsMatch(path,pattern)){
            string resultTemp = "";
            resultTemp = Regex.Match(path,pattern).Value;
            resultTemp = resultTemp.Substring(1,resultTemp.Length-1);
            result = uint.Parse(resultTemp);
        }
        return result;
    }
    /// <summary>
    /// Get data from (string) path with syntax : addressIPv4:port/dir1/dir2/
    /// </summary>
    /// <param name="path">selected dir address:port/dir </param>
    /// <returns>
    /// Returns data retrived from path
    /// [0] IPAddress ipaddress 
    /// [1] int port
    /// [2] string path
    /// </returns>

    public Object[] getInfoFromPath(string path){
        var result = new object[3];

        //Testing roughly is syntax valid ? 
        string octet = @"((2[0-4][0-9])|(25[0-5])|(1[0-9]{2})|([1-9][0-9])|([0-9]))";
        string pattern = @"^("+octet+@"\.){3}("+octet+@"):(\d*){1,5}/((\w+/)*(\w+)/?)?$";
        bool isMatch = Regex.IsMatch(path,pattern);
        if(!isMatch){
            throw new ArgumentException("Path syntax is invalid");
        }
        //Getting values ! 
        uint port = get_port(path);
        IPAddress ipv4 = get_ip(path);
        string dir = get_path(path); 

        //Port value testing 
        if(port == 0 || port > 65535 ){
            throw new ArgumentException("Port value is incorrect !");
        }
        result[0] = ipv4;
        result[1] = port;
        result[2] = dir;

        return result;
    }

}
