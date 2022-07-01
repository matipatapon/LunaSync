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
    public enum level {
        error,
        info,
        verbose
    }
    private static level messagelevel = level.error;
    
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

    /// <summary>
    /// Log string to the text 
    /// </summary>
    /// <param name="what">Message to log </param>
    public static void l(string what,level lvl = level.verbose){
        if(lvl > messagelevel){
            return;
        }
        //Try write log to the log file if not retry up to five times !
        int trycount = 1;
        while(trycount <= 5){
            try{
                using(var fs = File.Open(path,FileMode.Append)){
                    var bytes = new UTF8Encoding(true).GetBytes(what+"\n");
                    fs.Write(bytes,0,bytes.Count());
                    current+=1;
                }
                if(max != 0 && current > max){
                    current = 0;
                    reset();
                }
                break;
            }
            catch(IOException e){
                trycount++;
                WriteLine($"Error during logging message to the file due to {e.Message} Retry in one second !");
                Thread.Sleep(1000);
                continue;
            }
        }

        if(trycount > 5){
            WriteLine($"Can't write '{what}' to log.txt ");
        }
        

    }

    

}
