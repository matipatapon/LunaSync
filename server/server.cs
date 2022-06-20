using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace host.server;

public class server:servent
{
    //Socket to get connection from the client 
    private Socket lSocket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    
    TraceSwitch traceSwitch;


    public server(int port = 0){
        
        //Setting up Trace level
        traceSwitch = new TraceSwitch("TraceServer","Level of trace messages");
        traceSwitch.Level = TraceLevel.Info;
        Trace.WriteLineIf(traceSwitch.TraceVerbose,"Entering server constructor !");
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Setting traceSwitch of the server to {traceSwitch.Level.ToString()}");
        
        IPEndPoint ep = new IPEndPoint(IPAddress.Any,port);
        lSocket.Bind(ep);
        lSocket.Listen(2);
        WriteLine($"Server started listening on port {getPort().ToString()}");
        
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Starting server thread !");
        StartServerThread();
        
    }
    /// <summary>
    /// Return port number of the lSocket 
    /// </summary>
    /// <returns>Return port number of the lSocket. If there is no port return 0</returns>
    public int getPort(){
        if(lSocket is not null && lSocket.LocalEndPoint is not null){
        int port =  ((IPEndPoint)lSocket.LocalEndPoint).Port;
            return port;
        }
        return 0;
        
    }

    public void StartServerThread(){
        ThreadStart server_ext= new ThreadStart(StartServer);
        Thread server_thread = new Thread(server_ext);
        server_thread.Start();
    }

    


    int requestHandler(Socket handler){
        // Get command from client 
        string command = receiveData(handler);
        // validating command syntax 
        string pattern = @"<\w*>";
        bool isOk = Regex.IsMatch(command,pattern);
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Got command {command}");
        // Respond OK | DENY 
        switch(command){

        }
        sendData(handler,"OK");
        // 
        return 0;
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
    void StartServer(){
    lSocket.ReceiveTimeout = 5000;
    //Listen For incoming data !
    do{
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for connection !");
        Socket handler = lSocket.Accept();
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Connected!");
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for command !");
        requestHandler(handler);
 
    }while(true);
    }

    byte[] ReceiveData(){

        return new byte[1_048_578];
    }

}
