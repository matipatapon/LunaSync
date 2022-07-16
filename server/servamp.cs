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
        string dir = fhandler.getDirectory().FullName;

        syncDir(fhandler.getDirectory());
        enumerateThroughSubdirs(fhandler.getDirectory());
        /// <summary>
        /// Enumerate through subdirectories of the currentDir,
        /// and execute syncDir on them
        /// </summary>
        /// <param name="currentDir">DirectoryInfo</param>
        void enumerateThroughSubdirs(DirectoryInfo currentDir){
        
            foreach(var d in currentDir.EnumerateDirectories()){
            syncDir(d);
            enumerateThroughSubdirs(d);
        }
        }
        /// <summary>
        /// Go through files of the directory and sync them 
        /// </summary>
        /// <param name="d">DirectoryInfo</param>
        void syncDir(DirectoryInfo d){
                if(d.LinkTarget is not null){
                log.l($"{d} is link skipping",log.level.info);
                return;
            }
            
                foreach(var f in d.EnumerateFiles()){
                    int retrylimit = 10;
                    int retry = 1;
                    var finfo = new file(f.FullName,dir);
                    for(; retry <= retrylimit ; retry++){
                        string rInfo = "";
                        bool ok = getInfo(finfo.localPath,out rInfo);
                        if(!ok){
                            WriteLine("Failed getting info about {f.FullName} from server");
                        }
                        if(rInfo == "nogamenofile"){
                            try{
                            chandler.sendText("GETFILE");
                            var rsp1 = chandler.receiveText();
                            if(rsp1 != "ACCEPTED"){
                                log.l($"Wrong respond from the slave : {rsp1} retry ...");
                                continue;
                            }
                            ok = sendFile(finfo);
                            }
                            catch(Exception e){
                                string msg1 = $"Failed send file due to {e}";
                                log.l(msg1,log.level.error);
                                WriteLine(msg1); 
                            }
                            if(!ok){
                                WriteLine("Error sending file retry ...");
                                continue;
                            }
                        }
                        
                        break;
                    }
                    if(retry > retrylimit){
                        log.l($"Failed sync file :{f.FullName}",log.level.error);
                    }
                }
        }
        chandler.sendText("Eternal Natsu Dragneel");
    }   

    protected bool sendFile(file fi){

        chandler.sendText(fi.ToString());
        chandler.receiveText();

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
        

        file inforf;
        inforf = new file(info:chandler.receiveText());
        chandler.sendText("OK");

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
            info = "ERROR";
            return false;
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
                case "GETFILE":
                    getFile(dir);
                break;
                case "Eternal Natsu Dragneel":
                    WriteLine("Sync ended");
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

