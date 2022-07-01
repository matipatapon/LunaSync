using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
using logger;
namespace host.handler;

/// <summary>
/// Abstract class for handling connection
/// </summary>
abstract public class handler{
    protected Socket sSocket;

    public handler(){
        sSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
    }
    /// <summary>
    /// Receiving Text from the connection !
    /// </summary>
    /// <returns>Return (string)Text from connection</returns>
    public string receiveText()
    {
        if(sSocket is null){
            throw new ArgumentNullException("Socket is null !");
        }
        string data = "";
        string end = "";
        int count = 0;
        do{
            try{
            byte[] bytes = new byte[1024];
            count = sSocket.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes,0,count);
            end = data.Substring(data.Length-5,5);
            }
            catch(SocketException e){
                Trace.WriteLineIf(true,$"Receive error : {e}");
                break;
            }
        }while(count!=0 && end != "<EOF>");
        return data;
    }    

   
    /// <summary>
    /// Send data to the conection as Text
    /// </summary>
    /// <param name="data">(string) data to send</param>
    public void sendText(string data){
        if(sSocket is null){
            throw new ArgumentNullException("Socket is null !");
        }
        var bytes = Encoding.ASCII.GetBytes(data);
        sSocket.Send(bytes);

    }

}
/// <summary>
/// Handler for server connection
/// </summary>
public class serverhandler : handler{
    
    public serverhandler(IPAddress ipv4,int port,string dir){
        
        IPEndPoint ep = new IPEndPoint(ipv4,port); 
        sSocket.Connect(ep);
        sendText($"<COMMAND>SETDIR</COMMAND><DATA>{dir}</DATA><EOF>");
        string response = receiveText();
        response = response.Substring(0,response.Length-5);
        switch(response){
            case "OK":
                log.l($"Server successfully set dir to {dir}",log.level.info);
            break;
            case "DENY":
                log.l($"Server couldn't set dir to {dir}",log.level.error);
            throw new ArgumentException($"Server couldn't set dir to {dir}");
            
        }
    }


    
}

public class clienthandler : handler{
    public clienthandler(){
        IPEndPoint ep = new IPEndPoint(IPAddress.Any,0);
        sSocket.Bind(ep);
        sSocket.Listen(2);
        sSocket.ReceiveTimeout = 5000;
         WriteLine($"Server started listening on port {getPort().ToString()}");
        sSocket = sSocket.Accept();
    }
    public int getPort(){
        if(sSocket is not null && sSocket.LocalEndPoint is not null){
        int port =  ((IPEndPoint)sSocket.LocalEndPoint).Port;
            return port;
        }
        return 0;
    }
}