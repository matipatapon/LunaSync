using System.IO;
using static System.Console;
using System.Text;
using System.Threading;
namespace logger;
/// <summary>
/// My replacment for Trace 
/// Simpler , maybe even faster :D
/// </summary>
public class log{ 
    private static string path = "../log.txt";
    private static bool showTime = true;
    private static bool showLevel = true;
    public enum level {
        error,
        info,
        verbose
    }
    private static level messagelevel = level.verbose;
    
    //how much lines can be in log.txt 0 - disable
    private static int max = int.MaxValue-1;
    private static int current = 0;
    static log(){

        reset();
    }

    private static void reset(){
        using(var fs = File.Open(path,FileMode.Create)){

        }
    }
    //If file is busy then buffer is used to store message for later
    static string buffer = "";
    /// <summary>
    /// Log string to the text 
    /// </summary>
    /// <param name="what">Message to log </param>
    
    public static void l(string what,level lvl = level.verbose){
        if(lvl > messagelevel){
            return;
        }
        //Try write log to the log file if not retry up to five times !
        try{
            using(var fs = File.Open(path,FileMode.Append)){
                
                if(showTime){
                    var now = DateTime.Now;
                    what = $"<{now:hh:mm:ss}> {what}";
                }
                if(showLevel){
                    var lev = lvl.ToString();
                    what = $"<{lev}> {what}";
                }
                var bytes = new UTF8Encoding(true).GetBytes(what+"\n"+buffer);
                fs.Write(bytes,0,bytes.Count());
                current+=1;
                buffer = "";
            }
            if(max != 0 && current > max){
                current = 0;
                reset();
            }
          
        }
        catch(IOException){
            buffer+=what+"\n";
        }        

    }

    

}
