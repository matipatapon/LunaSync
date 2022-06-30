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
        //restart logging file
        while(true){
            try{
                fs = File.OpenWrite(path);
            break;
            }
            catch(System.IO.IOException e){
                WriteLine($"Error occured during accessing {path} {e.Message}!!! retry in one second !");
                Thread.Sleep(1000);
            }
        }
        reset();
    }

    private static void reset(){
        fs.SetLength(0);
        fs.Flush();
    }
    private static FileStream fs;

    /// <summary>
    /// Log string to the text 
    /// </summary>
    /// <param name="what">Message to log </param>
    public static void l(string what,level lvl = level.verbose){
        if(lvl > messagelevel){
            return;
        }
        try{
        var bytes = new UTF8Encoding(true).GetBytes(what+"\n");
        fs.Write(bytes,0,bytes.Count());
        fs.Flush();
        current+=1;
        if(max != 0 && current > max){
            current = 0;
            reset();
        }
        }
        catch(IOException){
            Console.WriteLine($"Can't log message to file {what}");
        }
        

    }

    

}
