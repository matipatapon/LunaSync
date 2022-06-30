using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
using files;
using logger;
namespace host;
/// <summary>
/// Shared settings between objects 
/// </summary>
abstract public class settings{
    protected TraceSwitch traceSwitch;
    protected filehandler? fhandler;

    protected settings(){
        //Setting up Trace level
        traceSwitch = new TraceSwitch("TraceServer","Level of trace messages");
        traceSwitch.Level = TraceLevel.Info;
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

            foreach(var d in dir.EnumerateDirectories()){
            try{
            log.l($"{d.FullName} found link target is {d.LinkTarget}");
            if(d.LinkTarget is not null){
                break;
            }
                foreach(var f in d.EnumerateFiles()){
                    log.l($"{f.Name} found");
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
    protected void DownDirStruct(){

    }
}
