using Xunit;
using host;
using System.Net;
namespace clienttest;

public class UnitTest1
{
    [Theory]
    //Correct 
    [InlineData("127.0.0.1/home/itam/whatever","127.0.0.1")]
    [InlineData("192.168.0.20/whatever","192.168.0.20")]
    //Error
    [InlineData("028.028.44.44/whatever","0.0.0.0")]
    [InlineData("GutsBlackSwordsman127.0.0.1/whatever","0.0.0.0")]
    [InlineData("1233.123.123.123/whatever","0.0.0.0")]
    [InlineData("000.000.000.255","0.0.0.0")]
    [InlineData("127+0+0+1/whatever","0.0.0.0")]
    [InlineData("256.255.255.254/whatever","0.0.0.0")]
    //[InlineData("/home/itam/whatever","127.0.0.1")]
    public void get_ip_CorrectResult(string path,string expected)
    {
        // arrange
        var cli = new client();

        // act 
        var result = cli.get_ip(path);
    
        // assert 
        Assert.Equal(expected,result.ToString());
    }

    
}