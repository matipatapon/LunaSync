global using System.Net.Sockets;
global using System.Net;
global using System.Text;
global using System.IO;
global using System.Threading;
global using static System.Console;
global using System.Text.RegularExpressions;
global using files;
global using logger;
global using host.handler;
namespace host;
/// <summary>
/// Shared settings and functions between objects 
/// </summary>
abstract public class hostshared{
    protected filehandler? fhandler;
    protected connectionHandler? chandler;

    protected hostshared(){

    }

    static bool checkResponse(string expected , string gotted){
        if(expected != gotted){
            string message = $"The response received({gotted}) is not equal to the expected response{expected}";
            log.l(message,log.level.error);
            return false;
        }
        return true;
    }

    //Syncing two folder structure 
    /// <summary>
    /// Manage syncing file 
    /// </summary>
    protected void masterFileTransfer(){

        if(chandler is null){
            log.l("chandler is null !!!",log.level.error);
            throw new ArgumentNullException("chandler is null !");
        }
        if(fhandler is null){
            log.l("fhandler is null !!!",log.level.error);
            throw new ArgumentNullException("fhandler is null !");
        }

        var dir = fhandler.getDirectory();

        enumerateThroughSubdirs(dir);

        void enumerateThroughSubdirs(DirectoryInfo dir){
        
            foreach(var d in dir.EnumerateDirectories()){
            
            if(d.LinkTarget is not null){
                log.l($"{d} is link skipping",log.level.info);
                continue;
            }
            
                foreach(var f in d.EnumerateFiles()){
                    int retrylimit = 10;
                    int retry = 1;
                    var finfo = new file(f.FullName,dir.FullName);
                    for(; retry <= retrylimit ; retry++){
                        string rInfo = "";
                        bool ok = getInfo(finfo.localPath,out rInfo);
                        break;
                    }
                    if(retry > retrylimit){
                        log.l($"Failed sync file :{f.FullName}",log.level.error);
                    }
                }
            enumerateThroughSubdirs(d);
        }
        }
        chandler.sendText("<END>");
    }   

    protected bool sendFile(file fi){
        log.l($"Sending file");
        chandler.sendText("RECEIVEFILE");
        //1
        log.l($"Master : waiting for response");
        var rsp1 = chandler.receiveText(); 
        if(rsp1 != "getFile"){
            var msg1 = $"INVALID RESPOND EXPECTED getFile GOT {rsp1}";
            log.l(msg1,log.level.error);
            throw new ArgumentException(msg1);
        }
                        //2
        log.l($"Master : Sending Info about file {fi.ToString()}");
        chandler.sendText(fi.ToString());
        log.l("Master : Waiting for response");
        chandler.receiveText();
        //3
        log.l("Master : Sending file");
        chandler.sendFile(fi);
        log.l("Master : waiting for response");
        string respond = chandler.receiveText();
        switch(respond){
            case "OK":
                return true;
            case "RETRY":
                return false;
            case  "FAIL":
                return false;
            default : 
                throw new ArgumentException($"Wrong respond ! {respond}");
        }
        
    }
    protected bool getFile(string dir , int t = 0){
        chandler.sendText("getFile");

        var inforf = new file(info:chandler.receiveText());
        //if error occured during getting the file
        if(!chandler.receiveFile(inforf.size)){
            chandler.sendText("FAIL");
            return false;
        }
        else{
        chandler.sendText("OK");
        var localPath = dir+inforf.localPath.Substring(1,inforf.localPath.Length-1);
        pathFactor(localPath);
        File.Move("../temp",(localPath));
        log.l($"receiveFile file received info gotted from the server : {inforf.ToString()}");

        
        }
        return true;
    }
    /// <summary>
    /// Get Info about file from sFT 
    /// </summary>
    /// <returns>file.ToString() if file doesn't exist 'nogamenofile'</returns>
    protected bool getInfo(string localPath,out string info){
        string rp1 = "";
        string send1 = "GETINFO";
        if(chandler is null){
            var msg1 = "chandler can't be null";
            log.l(msg1,log.level.error);
            throw new ArgumentNullException(msg1);
        }
        //Asking slave for command
        chandler.sendText($"GETINFO");
        rp1 = chandler.receiveText();
        if(!checkResponse("ACCEPTED",rp1)){
            chandler.sendText("DENY");
            throw new ArgumentException("Wrong response");
        }
        //Send local path to the file
        chandler.sendText(localPath);
        //Get Info about file
        info = chandler.receiveText();
        
        log.l($"getInfo\n->{send1}\n<-{rp1}\n->{localPath}\n<-{info}");

        return true;
    }
    int helpcount = 0;
    protected void sendInfo(string dir){
        
        var localPath = chandler.receiveText();
        var pathToFile = dir.Substring(0,dir.Length-1)+localPath;
        WriteLine($"#{++helpcount} {pathToFile}");
        log.l($"sendInfo crafted path to the file {pathToFile}");
        bool exist = File.Exists(pathToFile);
        string info = "nogamenofile";
        if(exist){
            info = new file(pathToFile,dir).ToString();
        }
        log.l($"sendInfo sending back info {info}");
        chandler.sendText(info);
    }
    /// <summary>
    /// Sync file 
    /// </summary>

    protected void slaveFileTransfer(){
        if(chandler is null){
            log.l("chandler is null !",log.level.error);
            throw new ArgumentNullException("chandler is null !");
        }
        if(fhandler is null){
            log.l("fhandler is null !",log.level.error);
            throw new ArgumentNullException("fhandler is null");
        }
        var dir = fhandler.getDirectory().FullName;
        while(true){
            string command = chandler.receiveText();

            chandler.sendText("ACCEPTED");
            switch(command){
                case "GETINFO":
                    sendInfo(dir);
                break;
                case "RECEIVEFILE":
                    /*if(!getFile(dir)){
                        log.l("Can't send file !");
                    }*/
                break;
            }
        }
    }
    /// <summary>
    /// Create mising folders to file 
    /// </summary>
    /// <returns>True if path was created successful</returns>
    protected bool pathFactor(string path){
        string pattern = @"[^/\\]+/";
        string currentPath = "";
        var matches = Regex.Matches(path,pattern);
        foreach(var ma in matches){
            var m = ma.ToString();
            m = m.Substring(0,m.Length-1);
            currentPath+="/"+m;
            

            //Check if directory doesn't exists and create them if didn't 
            if(!Directory.Exists(currentPath)){
                Directory.CreateDirectory(currentPath);
                log.l($"Created folder {currentPath}");
            }
        }
        return false;
    }
}

