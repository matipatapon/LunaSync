using System.IO;
using System.Collections;
using static System.Console;
namespace files;
public class filehandler
{
    string dir = "";
    
    List<file> Files;

    public filehandler(string dir){
        this.dir = dir;

    }
    public string LIST()
    {   
        var directory = new DirectoryInfo(dir);

        bool exist = directory.Exists;

        WriteLine("Directory exists");

        FileAttributes directoryattributes = directory.Attributes;
        
        var subdir = directory.GetDirectories();

        foreach(var dir in subdir){
            var files = dir.EnumerateFiles();
            
            foreach(var file in files){
                WriteLine($"{file.FullName}");
            }
        }
        return "";
    }
    
}

internal class file{
    string dir;
}