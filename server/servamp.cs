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
    protected void masterFileTransfer(bool switchAfterFinish = true){

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
                    file? rInfo = null;
                    string action = "OK";
                    for(; retry <= retrylimit ; retry++){
                        string rsp2 = "";
                        bool ok = getInfo(finfo.localPath,out rsp2);
                        if(rsp2 != "nogamenofile"){
                            rInfo = new file(info:rsp2);
                        }
                        if(!ok){
                            WriteLine($"Failed getting info about {f.FullName} from server retry");
                            action = "ERROR";
                            continue;
                        }
                        if(rInfo is null || (rInfo.hash != finfo.hash)){
                            try{
                            chandler.sendText("GETFILE");
                            var rsp1 = chandler.receiveText();
                            if(rsp1 != "ACCEPTED"){
                                log.l($"Wrong respond from the slave : {rsp1} retry ...");
                                action = "ERROR";
                                continue;
                            }
                            if( rInfo is null || rInfo.wTimeTicks < finfo.wTimeTicks ){
                                ok = sendFile(finfo);
                                action = "UPLOAD";
                            }
                            else if( rInfo.wTimeTicks > finfo.wTimeTicks){
                                ok = getFile(dir);
                                action = "DOWNLOAD";
                            }
                            }
                            catch(Exception e){
                                string msg1 = $"Failed send file due to {e}";
                                log.l(msg1,log.level.error);
                                ok = false;
                            }
                            if(!ok){
                                WriteLine("Error sending file retry ...");
                                action = "ERROR";
                                continue;
                            }
                            
                        }
             

                        break;
                    }
                    if(retry > retrylimit){
                        log.l($"Failed sync file :{f.FullName}",log.level.error);
                    }
                    writeRecord(finfo,rInfo,action);
                }
        }
        
        WriteLine("Switching roles !");
        if(switchAfterFinish){
            chandler.sendText("REZERO");
            chandler.receiveText();
            slaveFileTransfer();
        }
        
    }   

    protected void writeRecord(file lInfo , file? rInfo = null, string action = "ERROR"){
        string rHash = "NOTFOUND";
        string rTime = "NOTFOUND";
        if(rInfo is not null){
            rHash = rInfo.hash;
            rTime = rInfo.wTimeTicks.ToString();
        }
        WriteLine($"|{lInfo.name}|{lInfo.hash}|{rHash}|{lInfo.wTimeTicks}|{rTime}|{action}|");
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
        
        //Check if file is correct
        var tInfo = new file("../temp","/");
        if(tInfo.hash != inforf.hash){
            log.l("File hash isn't correct !");
            return false;
        }
        if(File.Exists(localPath)){
            File.Delete(localPath);
        }
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
    protected void sendInfo(string dir,out file? fInfo){
        var localPath = chandler.receiveText();
        var pathToFile = dir.Substring(0,dir.Length-1)+localPath;
        
        log.l($"sendInfo crafted path to the file {pathToFile}");
        bool exist = File.Exists(pathToFile);
        string info = "nogamenofile";
        fInfo = null;
        if(exist){
            fInfo = new file(pathToFile,dir);
            info = fInfo.ToString();
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
        file? lastFile = null;
        while(true){
            string command = chandler.receiveText();

            chandler.sendText("ACCEPTED");
            switch(command){
                case "GETINFO":
                    sendInfo(dir,out lastFile);
                break;
                case "GETFILE":
                    getFile(dir);
                break;
                case "SENDLASTFILE":
                    if(lastFile is null){
                        log.l("There is no last file !!!",log.level.error);
                        break;
                    }
                    sendFile(lastFile);
                break;
                case "Eternal Natsu Dragneel":
                    WriteLine("Sync ended !");
                return;
                case "REZERO":
                    log.l("Slave switching roles");
                    masterFileTransfer(false);
                return;
                
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

