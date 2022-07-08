﻿using System.IO;
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
    readonly byte[] hash = new byte[16];
    long size = 0;
    long wTimeTicks = 0;
    FileAttributes attributes = 0;


    /// <summary>
    /// Get hash of the file
    /// </summary>
    /// <returns>Return hash (byte[16]) md5</returns>
    byte[] getHash(){
        var md5 = MD5.Create();
        try{
            var stream = new FileInfo(this.fullName).OpenRead();
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
    public file(string? path = null,string? dir = null , string? info = null){
        //file object can be created for two metohds 
        //1) get path and dir 
        //2) get file object as string then convert it do file object
        if(path is not null && dir is not null){
            var fi = new FileInfo(path);
            this.fullName = fi.FullName;
            this.hash = this.getHash();
            this.name = fi.Name;
            this.localPath = fullName.Substring(dir.Length,fullName.Length-dir.Length);
            this.attributes = File.GetAttributes(path);
            this.size = fi.Length;
            this.wTimeTicks = fi.LastWriteTime.Ticks;
            log.l($"Collected info about file : {this.ToString()}");
        }
        else if (info is not null){
            log.l($"file constructor got info : {info}");
         
            
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
        attrpattern,
        valuePattern
    }
    public static Match RegexIHateU(string info , Pattern pattern){
        IDictionary<string,string> valuePatterns = new Dictionary<string,string>();
        
        valuePatterns["infoSegment"] = @"<\w+>[^<>/]+</\w+>";
        valuePatterns["fullName"] = @"(/\w+)*/\w+";
        valuePatterns["localPath"] = @"(\w+/)*";
        valuePatterns["hash"] = @".{16}";

        return Regex.Match(info,valuePatterns[pattern.ToString()]);
 
    }

    public override string ToString(){
        string result = "";
        
        result+=$"<fullName>{this.fullName}</fullName>";
        result+=$"<name>{this.name}</name>";
        result+=$"<localPath>{this.localPath}</localPath>";
        result+=$"<hash>{BitConverter.ToString(this.hash).Replace("-","")}</hash>";
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