using Xunit;
using host.client;
using System.Net;
using System;
namespace clienttest;

public class pathtest
{
    [Theory]
    //Correct 
    [InlineData("127.0.0.1","127.0.0.1")]
    [InlineData("192.168.0.20","192.168.0.20")]
    [InlineData("1.1.1.1...","1.1.1.1")]

    public void get_ip_CorrectResult(string path,string expected)
    {
        // arrange


        // act 
        var result = client.get_ip(path);
    
        // assert 
        Assert.Equal(expected,result.ToString());
    }
    [Theory]
    [InlineData("127.0.0.1:50/home","/home/")]
    [InlineData("127.0.0.1:50/home/","/home/")]
    [InlineData("127.0.0.1:50/home/","/home/")]
    [InlineData("127.0.0.1:50/home/home","/home/home/")]
    //[InlineData(".../home/.../home","")]
    public void get_path_TEST(string path,string expected){

        // arrange & act
        var result = client.get_path(path);

        // assert 
        Assert.Equal(expected,result);
    }

    [Theory]
    //Testing addresses
    [InlineData("127432w13123123qweqeqw")]
    [InlineData("127.127.127.127:50:50/home")]
    [InlineData("126.126.126:50/home")]
    [InlineData("127..127.127:50/home")]
    [InlineData("127.127.127.127:50/home//")]
    [InlineData("127.127.127.1272:50/home")]
    [InlineData("265.1.1.1:50/home")]
    [InlineData("-1.1.1.1:50/home")]
    [InlineData("265.1.1.1.50/home")]

    //Testing port 
    [InlineData("1.1.1.1:123456/home")]
    [InlineData("1.1.1.1:73456/home")]
    [InlineData("1.1.1.1:0/home")]
    public void getInfoFromPath_ArgumentException(string path){
        // arrange
  
        // act & arragne
        Assert.Throws<ArgumentException>(() => client.getInfoFromPath(path));
        
    }
    [Theory]
    [InlineData("127.0.0.1:51/whatever/","127.0.0.1,51,/whatever/")]
    [InlineData("185.56.98.69:6996/whatever9/rikodontleaveme69/xd/","185.56.98.69,6996,/whatever9/rikodontleaveme69/xd/")]
    [InlineData("192.168.0.20:8310/whateverserver/jakistojestserwer/jakitoniewiem","192.168.0.20,8310,/whateverserver/jakistojestserwer/jakitoniewiem/")]
    public void getIpFromPatch_ValueTest(string path, string expected){
        // arrange
        string result = "";
        // act
        (int port, IPAddress ipv4, string dir) = client.getInfoFromPath(path);
        result = $"{ipv4.ToString()},{port.ToString()},{dir.ToString()}";

        // arrange 
        Assert.Equal(expected,result);
    }

    
}