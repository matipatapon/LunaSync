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
    /// Upload directory structure to server/client 
    /// In shortcut this function will ask server if he have every
    /// subdirectory
    /// </summary>
    protected void upDirStruct(){

        var dir = fhandler.getDirectory();

        listdir(dir);

        void listdir(DirectoryInfo dir){

            if(chandler is null){
                log.l("chandler is null !!!",log.level.error);
                throw new ArgumentNullException("chandler is null !");
            }

            foreach(var d in dir.EnumerateDirectories()){
            try{
            if(d.LinkTarget is not null){
                log.l($"{d} is link skipping",log.level.info);
                break;
            }
                foreach(var f in d.EnumerateFiles()){
                    log.l($"{f.Name} found");
                    chandler.sendText(f.FullName+"<EOF>");
                    
                }
            file.fileOrDirToString(d);
            listdir(d);
            }
            catch(Exception e){
            log.l($"{e.Message} Exception",log.level.error);
            }
        }
        }
    }    
    /// <summary>
    /// Download Directory Structure from server/client 
    /// This function will ensure that host have the same structure as
    /// server/client 
    /// </summary>
    //TODO
    //It takes all messages of files name as one ... ...
    protected void DownDirStruct(){
        if(chandler is null){
            log.l("chandler is null !",log.level.error);
            throw new ArgumentNullException("chandler is null !");
        }
        while(true){
            var x = chandler.receiveText();
            WriteLine($"Received {x}");
        }
    }
}
