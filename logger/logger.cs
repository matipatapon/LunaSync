using System.IO;
using System.Text;
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
        fs = File.OpenWrite(path);
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
