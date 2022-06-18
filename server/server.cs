using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;

namespace host;
public class server
{
    
    public void StartServerThread(){
        ThreadStart server_ext= new ThreadStart(StartServer);
        Thread server_thread = new Thread(server_ext);
        server_thread.Start();
    }

    Socket lSocket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    TraceSwitch traceSwitch;
    public server(TraceSwitch ts){
    traceSwitch = ts;
    Trace.WriteLineIf(traceSwitch.TraceVerbose,"Entering server constructor !");
    Trace.WriteLineIf(traceSwitch.TraceInfo,$"Setting traceSwitch of the server to {traceSwitch.Level.ToString()}");
    IPEndPoint ep = new IPEndPoint(IPAddress.Any,0);
    lSocket.Bind(ep);
    lSocket.Listen(2);
    WriteLine($"Server started listening on port {((IPEndPoint)lSocket.LocalEndPoint).Port.ToString()}");
    Trace.WriteLineIf(traceSwitch.TraceInfo,$"Starting server thread !");
    StartServerThread();
    }

     string receiveData(Socket handler)
    {
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
                Trace.WriteLineIf(traceSwitch.TraceError,$"Receive error : {e}");
                break;
            }
        }while(count!=0 && end != "<EOF>");
        return data;
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
    void StartServer(){
    lSocket.ReceiveTimeout = 5000;
    //Listen For incoming data !
    do{
        WriteLine("Waiting for connection !");
        Socket handler = lSocket.Accept();
        WriteLine("Connected !");
        string data = receiveData(handler);
        WriteLine($"Get {data}");
 
    }while(true);
    }

    byte[] ReceiveData(){

        return new byte[1_048_578];
    }

}

