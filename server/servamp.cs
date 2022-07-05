using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Text.RegularExpressions;
using files;
using logger;
using host.handler;
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

                    var fi = new file(f.FullName,dir.FullName);
                    //#1 Get info about file
                    log.l($"Master : sending GETINFO");
                    chandler.sendText("GETINFO");
                    log.l($"Master : waiting for response");
                    var response = chandler.receiveText();
                    log.l($"Master : got response {response}");
                    if(response != "OK"){
                        log.l($"Master : Wrong response {response}",log.level.error);
                        throw new ArgumentException($"Wrong response {response}");
                    }
                    log.l($"Master : sending localPath{fi.localPath}");
                    chandler.sendText(fi.localPath);

                    var rInfo = chandler.receiveText();
                    log.l($"Master : received info{rInfo}");
                    WriteLine($"Got {rInfo} ^^");

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
        while(true){
            string command = chandler.receiveText();
            switch(command){
                case "GETINFO":
                    log.l("Slave : GETINFO sending OK");
                    chandler.sendText("OK");
                    //#1 get local path to the file 
                    var localPath = chandler.receiveText();
                    var dir = fhandler.getDirectory().FullName;
                    var pathToFile = dir.Substring(0,dir.Length-1)+localPath;
                    WriteLine($"{pathToFile}");
                    log.l($"Slave crafted path to the file {pathToFile}");
                    //#2 Check if this file exist 
                    bool exist = File.Exists(pathToFile);
                    //#3 if exist get info about file and send it if not send "nogamenofile"
                    string info = "nogamenofile";
                    if(exist){
                        info = new file(pathToFile,dir).ToString();
                    }
                    log.l($"Slave sending back info {info}");
                    chandler.sendText(info);
                    

                break;
            }
        }
    }
}

