﻿using System.IO;
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
    readonly public string FullName = string.Empty;
    readonly public string Name = string.Empty;
    readonly public string localPath = string.Empty;
    readonly byte[] hash = new byte[16];
    readonly public bool read = true;
    readonly public bool write;



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
    public file(string path,string dir){
        var fi = new FileInfo(path);
        this.FullName = fi.FullName;
        this.hash = this.getHash();
        this.Name = fi.Name;
        this.localPath = FullName.Substring(dir.Length,FullName.Length-dir.Length);
        int attributes = (int)File.GetAttributes(path);
        int test = 4;
        attributes = attributes|test;
        log.l($"Collected info about file : \n "+
        $"Name : {this.Name}\n"+
        $"FullName : {this.FullName}\n"+
        $"localPath : {this.localPath}\n"+
        $"hash : {BitConverter.ToString(this.hash).Replace("-","")}\n"+
        $"Fileattributes : {Convert.ToString(attributes,2)} and it's type is {attributes.GetType().ToString()}");
        
        
    }

    public override string ToString(){
        string result = "";
        result+="<path>"+this.FullName+"</path>";
        
        return "";
    }

    
    /// <summary>
    /// Function convert FileInfo/DirectoryInfo to string !!! to be send
    /// </summary>
    /// <param name="fear">File(FileInfo) or Directory(DirectoryInfo)</param>
    /// <typeparam name="TFileType"></typeparam>
    /// <returns></returns>
    static public string fileOrDirToString(DirectoryInfo fear){
        log.l("Entering dirToString !!!");
    
        //WriteLine($"I am {fear.FullName}");
        return "";
    }
    static public string fileOrDirToString(FileInfo fear){
        log.l("Entering file to string !!!");

        WriteLine($"I am a file {fear.FullName}");
        return "";
    }
}

public interface IFileComparable{


}