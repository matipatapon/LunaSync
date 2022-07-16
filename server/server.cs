namespace host.server;

public class server: hostshared
{
    
    public server(int port = 0){
        log.l("Starting server thread",log.level.verbose);
        fhandler = new filehandler("/home/itam/Desktop/TestZone/SERVER");
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
        ThreadStart server_ext= new ThreadStart(StartServer);
        Thread server_thread = new Thread(server_ext);
        server_thread.Start();
    }

    /// <summary>
    /// Setting up and starting the server 
    /// </summary>
    public void StartServer(){
    
    //Listen For incoming data !
        log.l("Server is waiting for connection...",log.level.info);
        chandler = new connectionHandler(connectionHandler.handlertype.client);
        log.l("Server is connected !");
        slaveFileTransfer();
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
            log.l($"Server got command {command}",log.level.verbose);
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
}
