using System.IO;
using System.Collections;
using static System.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using logger;
namespace files;
public class filehandler
{
    string dir = "";
    
    public filehandler(string dir){
        this.dir = dir;
        //Ensure / is on the end of the path
        if(this.dir[this.dir.Length-1] != '/'){
            this.dir+="/";
        }
        
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
                result.Add(new file(f.FullName,this.dir));
            }
        }
        WriteLine();
        return result;
    }

    /// <summary>
    /// Get string of the relative names of all directories !
    /// </summary>
    /// <param name="path">Path to dictionary</param>
    /// <returns></returns>
    public List<string> RelativeListDir(string? path = null){
        var result = new List<string>();

        if(path is null){
            path = this.dir;
        }
        try{
        var directory = new DirectoryInfo(path);

        foreach(var d in directory.EnumerateDirectories()){
            var name = d.FullName.Substring(path.Length-1);
            log.l($"Found subdirectory {d.FullName}");
            result.Add(name);
            result.AddRange(RelativeListDir($"{path}{name}/"));
        }
        }
        catch(Exception e){
            log.l($"Exception in RelativeListDir {e.Message}");
        }

        return result;
    }

    public DirectoryInfo getDirectory(){
        return new DirectoryInfo(this.dir);
    }
    
}

/// <summary>
/// file object get info about file !!! 
/// </summary>
public class file{
    readonly public string fullName = string.Empty;
    readonly public string name = string.Empty;
    readonly public string localPath = string.Empty;
    readonly string hash = "";
    public long size = 0;
    long wTimeTicks = 0;
    int attributes = 0;


    /// <summary>
    /// Get hash of the file
    /// </summary>
    /// <returns>Return hash (byte[16]) md5</returns>
    string getHash(){
        var md5 = MD5.Create();
        try{
            var stream = new FileInfo(this.fullName).OpenRead();
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","");
        }
        catch(UnauthorizedAccessException){
            
            return "";
        }

        
    }
    /// <summary>
    /// Create instance from FileInfo object
    /// </summary>
    /// <param name="x">FileInfo instance</param>
    public file(string? path = null,string? dir = null , string? info = null){
        //file object can be created for two metohds 
        //1) get path and dir 
        //2) get file object as string then convert it do file object
        if(path is not null && dir is not null){
            var fi = new FileInfo(path);
            this.fullName = fi.FullName;
            this.hash = "AAAABBBBEEEERRRRQQQQWWWWEEEERRRR";//this.getHash();
            this.name = fi.Name;
            if(dir[dir.Length-1] == '/'){
                this.localPath = fullName.Substring(dir.Length-1,fullName.Length-dir.Length+1);
            }
            else{
                this.localPath = fullName.Substring(dir.Length,fullName.Length-dir.Length);
            }       
            this.attributes = (int)File.GetAttributes(path);
            this.size = File.ReadAllBytes(path).LongLength;
            this.wTimeTicks = fi.LastWriteTime.Ticks;
            log.l($"Collected info about file : {this.ToString()}");
        }
        else if (info is not null){
            log.l($"file constructor got info : {info}");
            Match match = RegexIHateU(info,Pattern.info);
            
            if(match.Success == true){
                Match segment = RegexIHateU(info,Pattern.infoSegment);
                while(segment.Success){

                    var attrMatch = RegexIHateU(segment.Value,Pattern.attrname);
                    var valueIDKMatch = RegexIHateU(segment.Value,Pattern.valueIDK);

                    if(attrMatch.Success && valueIDKMatch.Success ){
                                            
                        var valueMatch = RegexIHateU<string>(segment.Value,attrMatch.Value);
                        if(valueMatch.Success){
                            string valueString = valueMatch.Value;
                            bool ok = true;
                            switch(attrMatch.Value){
                                
                                case "fullName":
                                    this.fullName = valueString;  
                                break;
                                case "localPath":
                                    this.localPath = valueString;
                                break;
                                case "hash":
                                    this.hash = valueString;
                                break;
                                case "size":
                                    ok = long.TryParse(valueString,out this.size);
                                break;
                                case "attributes":
                                    ok = int.TryParse(valueString,out this.attributes);
                                break;
                                case "wTimeTicks":
                                    ok = long.TryParse(valueString,out this.wTimeTicks);
                                break;
                            }
                            if(!ok){
                                log.l($"Can't convert {valueString} to {attrMatch.Value}",log.level.error);
                            }
                        }
                    else{
                        log.l($"Can't get attrname and value from segmen {segment.Value}");
                    }
                    //validate value format
                    
                    
                    }
                    segment = segment.NextMatch();
                
                }
            }
            else{
                var message = $"info is not valid {info}";
                log.l(message,log.level.error);
                throw new ArgumentException(message);           
            }
            
        }
        else{
            throw new ArgumentException("Wrong arguments were given to file constructor");
        }
    }

    public enum Pattern{
        attrname,
        valueIDK,
        fullName,
        localPath,
        attributes,
        size,
        wTimeTicks,
        infoSegment,
        info
    }

    /// <summary>
    /// Get match from given pattern 
    /// </summary>
    /// <param name="info">info (file.ToString)</param>
    /// <param name="pattern">Regex pattern from Patterns</param>
    /// <returns>Returns Match , Match.Empty if failed</returns>
    public static Match RegexIHateU<T>(string info , T pattern){
        if(pattern is null){
            string message = "pattern can't be null !";
            log.l(message,log.level.error);
            throw new ArgumentNullException(message);
        }

        IDictionary<string,string> valuePatterns = new Dictionary<string,string>();
        valuePatterns["info"] = @"^(<\w+>[^<>]+</\w+>)+$";
        valuePatterns["infoSegment"] = @"<\w+>[^<>]+</\w+>";
        valuePatterns["attrname"] = @"<\w+>";
        valuePatterns["valueIDK"] = @">[^<>]+<";
        valuePatterns["fullName"] = @"^(/[^<>\\/]+)*/[^<>\\/]+$";
        valuePatterns["localPath"] = valuePatterns["fullName"];
        valuePatterns["hash"] = @"^[A-Z0-9]{32}$";
        valuePatterns["wTimeTicks"] = @"^\d+$";
        valuePatterns["size"] = valuePatterns["wTimeTicks"];
        valuePatterns["attributes"] = valuePatterns["wTimeTicks"];
        valuePatterns["name"] = @"^[^<>\\/]+$";
        //Treat this keys as value !!
        string[] values = new string[7];
        values[0] = "localPath";
        values[1] = "hash";
        values[2] = "fullName";
        values[3] = "wTimeTicks";
        values[4] = "size";
        values[5] = "name";
        values[6] = "attributes";
        //trim <> characters !!!
        string [] trimBy2 = new string[2];
        trimBy2[0] = "valueIDK";
        trimBy2[1] = "attrname";

        string patternString = pattern.ToString()!;
        if(!valuePatterns.ContainsKey(patternString))
        {
            string message = $"valuePattrns dictionary doesn't contain {pattern.ToString()!} key"; 
            log.l(message,log.level.error);
            throw new ArgumentException(message);
        }
        try{
         
            if(values.Contains(patternString)){
                var matchValue = Regex.Match(info,valuePatterns["valueIDK"]);
                
                if(!matchValue.Success){
                    string message = $"Can't find {valuePatterns["valueIDK"]} pattern in info : {info}";
                    log.l(message,log.level.error);
                    throw new ArgumentException(message);
                }

                info = matchValue.Value;
                //Remove ><
                info = info.Substring(1,info.Length-2);
            }
            
            
            var match = Regex.Match(info,valuePatterns[patternString]);
            
            if(!match.Success){
                string message = $"Can't find pattern : '{valuePatterns[patternString]}' in info : '{info}'";
                log.l(message,log.level.error);
                throw new ArgumentException(message);
            }

            if(trimBy2.Contains(patternString)){

                return Regex.Match(match.Value,"[^<>]+");
            }

            return match;
        }
        catch(KeyNotFoundException e){
            log.l($"Key not found Regex I hate you {e.Message}");
            return Match.Empty;
        }
    }

    public override string ToString(){
        string result = "";
        result+=$"<fullName>{this.fullName}</fullName>";
        result+=$"<name>{this.name}</name>";
        result+=$"<localPath>{this.localPath}</localPath>";
        result+=$"<hash>{this.hash}</hash>";
        result+=$"<attributes>{this.attributes}</attributes>";
        result+=$"<size>{this.size}</size>";
        result+=$"<wTimeTicks>{this.wTimeTicks}</wTimeTicks>";
        
        return result;
    }
    /// <summary>
    /// Convert info(string) to file object
    /// </summary>
    /// <param name="info">file.ToString()</param>
   
        

}

public interface IFileComparable{


}