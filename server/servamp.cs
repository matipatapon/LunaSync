using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace host;
/// <summary>
/// Shared functions between client and server
/// serv + ent 
/// SERVer + cliENT
/// </summary>
abstract public class servamp{
    protected Socket sSocket;
    protected TraceSwitch traceSwitch;

    protected servamp(){
        //Setting up Trace level
        traceSwitch = new TraceSwitch("TraceServer","Level of trace messages");
        traceSwitch.Level = TraceLevel.Info;

        sSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
    }
    
    public string receiveText(ref Socket handler)
    {
        if(handler is null){
            throw new ArgumentNullException("Socket is null !");
        }
        string data = "";
        string end = "";
        int count = 0;
        do{
            try{
            byte[] bytes = new byte[1024];
            count = handler.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes,0,count);
            end = data.Substring(data.Length-5,5);
            }
            catch(SocketException e){
                Trace.WriteLineIf(traceSwitch!.TraceError,$"Receive error : {e}");
                break;
            }
        }while(count!=0 && end != "<EOF>");
        return data;
    }    
    public void sendText(ref Socket handler,string data){
        if(handler is null){
            throw new ArgumentNullException("Socket is null !");
        }
        var bytes = Encoding.ASCII.GetBytes(data);
        handler.Send(bytes);

    }

}