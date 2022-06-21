using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace host.server;

public class server:servamp
{
    
    
    public server(int port = 0){
        
        
        Trace.WriteLineIf(traceSwitch.TraceVerbose,"Entering server constructor !");
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Setting traceSwitch of the server to {traceSwitch.Level.ToString()}");
        
        IPEndPoint ep = new IPEndPoint(IPAddress.Any,port);
        sSocket.Bind(ep);
        sSocket.Listen(2);
        WriteLine($"Server started listening on port {getPort().ToString()}");
        
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Starting server thread !");
        StartServerThread();
        
    }
    /// <summary>
    /// Return port number of the sSocket 
    /// </summary>
    /// <returns>Return port number of the sSocket. If there is no port return 0</returns>
    public int getPort(){
        if(sSocket is not null && sSocket.LocalEndPoint is not null){
        int port =  ((IPEndPoint)sSocket.LocalEndPoint).Port;
            return port;
        }
        return 0;
        
    }


    enum commands{
        TEST
    }
    public void StartServerThread(){
        string x = $"{commands.TEST.ToString()}";
        var bytes = Encoding.ASCII.GetBytes(x);
        WriteLine($"char have {bytes.Count()} bytes");
        ThreadStart server_ext= new ThreadStart(StartServer);
        Thread server_thread = new Thread(server_ext);
        server_thread.Start();
    }

    


    int requestHandler(ref Socket handler){
        // Get command from client 
        string data = receiveText(ref handler);
        // validating command syntax 
        string commandPattern = @"<COMMAND>\w*</COMMAND>";
        string command = Regex.Match(data,commandPattern).Value;
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Got command {data}");
        // Respond OK | DENY 
        switch(command){
            case "<COMMAND>TEST</COMMAND>":
            WriteLine("Its test command !\n Hello world !");
            break;
        }
        sendText(ref handler,"OK");
        // 
        return 0;
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
    public void StartServer(){
    sSocket.ReceiveTimeout = 5000;
    //Listen For incoming data !
    do{
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for connection !");
        Socket handler = sSocket.Accept();
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Connected!");
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for command !");
        requestHandler(ref handler);
 
    }while(true);
    }

}
