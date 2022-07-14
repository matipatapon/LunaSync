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
            try{
            if(d.LinkTarget is not null){
                log.l($"{d} is link skipping",log.level.info);
                break;
            }
                foreach(var f in d.EnumerateFiles()){

                    var fi = new file(f.FullName,fhandler.getDirectory().FullName);
                    var localPath = fi.localPath;

                    var rInfo = getInfo(localPath);
                    //#2 Compare this file to remote file then perform action 

                    //Send file to server and replace with it 
                    if(rInfo == "nogamenofile"){

                        log.l($"Master : Sending RECEIVEFILE");
                        chandler.sendText("RECEIVEFILE");
                        //1
                        log.l($"Master : waiting for response");
                        chandler.receiveText();
                        //2
                        log.l($"Master : Sending Info about file {fi.ToString()}");
                        chandler.sendText(fi.ToString());
                        log.l("Master : Waiting for response");
                        chandler.receiveText();
                        //3
                        log.l("Master : Sending file");
                        chandler.sendFile(fi);
                        log.l("Master : waiting for response");
                        chandler.receiveText();
                    }

                }
            enumerateThroughSubdirs(d);
            }
            catch(Exception e){
            log.l($"{e.Message} Exception",log.level.error);
            }
        }
        }
        chandler.sendText("<END>");
    }   

    /// <summary>
    /// Get Info about file from sFT 
    /// </summary>
    /// <returns>file.ToString() if file doesn't exist 'nogamenofile'</returns>
    protected string getInfo(string localPath){
        log.l("Getting info about file from sFT");
        chandler.sendText("GETINFO");
        var response = chandler.receiveText();
        if(response != "OK"){
            log.l($"Master : Wrong response {response}",log.level.error);
            throw new ArgumentException($"Wrong response {response}");
        }
        chandler.sendText(localPath);
        var rInfo = chandler.receiveText();
        log.l($"Got info about file {rInfo}");
        return rInfo;
    }
    protected void sendInfo(string dir){
        log.l("Sending info...");
        chandler.sendText("OK");
        //#1 get local path to the file 
        var localPath = chandler.receiveText();
        var pathToFile = dir.Substring(0,dir.Length-1)+localPath;
        WriteLine($"{pathToFile}");
        log.l($"sendInfo crafted path to the file {pathToFile}");
        //#2 Check if this file exist 
        bool exist = File.Exists(pathToFile);
        //#3 if exist get info about file and send it if not send "nogamenofile"
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
            

            switch(command){
                case "GETINFO":
                    sendInfo(dir);
                break;
                case "RECEIVEFILE":
                    //1
                    log.l("Slave : sending OK");
                    chandler.sendText("OK");
                    //2
                    log.l("Slave : receiving info about file");
                    var inforf = new file(info:chandler.receiveText());
                    log.l($"Slave : got information about file : {inforf.ToString()}");
                    log.l("Slave : sending response");
                    chandler.sendText("OK");
                    //3
                    log.l("Slave : receiving file");
                    chandler.receiveFile(inforf.size);
                    log.l("Slave : sending OK");
                    chandler.sendText("OK");

                    var localPath = dir+inforf.localPath.Substring(1,inforf.localPath.Length-1);

                    pathFactor(localPath);
                    
                    File.Move("../temp",(localPath));
                    
                    

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
            WriteLine($"Gotcha {currentPath}");

            //Check if directory doesn't exists and create them if didn't 
            if(!Directory.Exists(currentPath)){
                Directory.CreateDirectory(currentPath);
            }
        }
        return false;
    }
}

