using System.IO;
using System.Collections;
using static System.Console;
using System.Security.Cryptography;
using System.Text;
using logger;
namespace files;
public class filehandler
{
    string dir = "";
    
    public filehandler(string dir){
        this.dir = dir;
        
    }

    /// <summary>
    /// Return list of the files in the current folder
    /// </summary>
    /// <returns>List<FileInfo> of files in directory</returns>
    public List<file> LIST()
    {   
        
        var directory = new DirectoryInfo(dir);
        if(!directory.Exists){
            throw new DirectoryNotFoundException("Directory doesn't exist");
        }
        
        var subdir = directory.GetDirectories();

        List<file> result = new List<file>();

        foreach(var dir in subdir){
            var files = dir.EnumerateFiles();
            
            foreach(var f in files){    
                result.Add(new file(f));
            }
        }
        WriteLine();
        return result;
    }
    
}

/// <summary>
/// File 
/// </summary>
public class file{
    public string FullName = string.Empty;
    bool read = true;
    bool write;

    static int counterxd = 0;

    /// <summary>
    /// Get hash of the file
    /// </summary>
    /// <returns>Return hash (byte[16]) md5</returns>
    byte[] getHash(){
        var md5 = MD5.Create();
        try{
            var stream = new FileInfo(this.FullName).OpenRead();
            return (md5.ComputeHash(stream));
        }
        catch(UnauthorizedAccessException){
            
            return new byte[16];
        }

        
    }
    /// <summary>
    /// Create instance from FileInfo object
    /// </summary>
    /// <param name="x">FileInfo instance</param>
    public file(FileInfo fi){
        this.FullName = fi.FullName;
        this.getHash();
        WriteLine($"This is {++counterxd} file !");
    }

    /// <summary>
    /// Create object from string pattern 
    /// path\t...to be continue
    /// </summary>
    /// <param name="x">(string)info about file</param>
    public file(string x){
        
    }
}

public interface IFileComparable{


}