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
    private level messagelevel = level.verbose;
    
    static log(){
        //restart logging file
         fs = File.OpenWrite(path);
    }

    private static FileStream fs;

    /// <summary>
    /// Log string to the text 
    /// </summary>
    /// <param name="what">Message to log </param>
    public static void l(string what,level lvl = level.verbose){
        try{
        var bytes = new UTF8Encoding(true).GetBytes(what+"\n");
        fs.Write(bytes,0,bytes.Count());
        fs.Flush();
        }
        catch(IOException){
            Console.WriteLine($"Can't log message to file {what}");
        }
        

    }

    

}
