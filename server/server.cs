using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
using host.handler;
namespace host.server;

public class server: hostshared
{
    
    public server(int port = 0){
        
        
        Trace.WriteLineIf(traceSwitch.TraceVerbose,"Entering server constructor !");
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Setting traceSwitch of the server to {traceSwitch.Level.ToString()}");
       
        Trace.WriteLineIf(traceSwitch.TraceInfo,$"Starting server thread !");
        StartServerThread();
        
    }
    /// <summary>
    /// Return port number of the sSocket 
    /// </summary>
    /// <returns>Return port number of the sSocket. If there is no port return 0</returns>
    public int getPort(){

        if (chandler is null){
            throw new ArgumentNullException("handler is null");
        }

        return chandler.getPort();
        
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

    

    
    int requestHandler(){

        if (chandler is null){
            throw new ArgumentNullException("handler is null");
        }
        // Get command from client 
        string data = chandler.receiveText();
        // validating command syntax 
        string commandPattern = @"<COMMAND>\w*</COMMAND>";
        bool isCommand = Regex.IsMatch(data,commandPattern);
        if(isCommand){
            
            string command = Regex.Match(data,commandPattern).Value;
            command = command.Substring(9,command.Length-19);
            Trace.WriteLineIf(traceSwitch.TraceInfo,$"Server got command {command}");
            // Respond OK | DENY 
            switch(command){
            case "TEST":
            WriteLine("Its test command !\n Hello world !");
            break;
            case "SETDIR":
                WriteLine($"GOTTED {data}");
            break;
            
            }

            chandler.sendText("OK<EOF>");
            return 0;
        }
        chandler.sendText("DENY<EOF>");
        
        
        // 
        return -1;
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
    public void StartServer(){
    
    //Listen For incoming data !
    do{
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for connection !");
        chandler = new connectionHandler(connectionHandler.handlertype.client);
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Connected!");
        Trace.WriteLineIf(traceSwitch.TraceInfo,"Waiting for command !");
        requestHandler();
 
    }while(true);
    }

}
