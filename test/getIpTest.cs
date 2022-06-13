using Xunit;
using host;
using System.Net;
namespace clienttest;

public class UnitTest1
{
    [Theory]
    [InlineData("127.0.0.1/home/itam/whatever","127.0.0.1")]
    [InlineData("/home/itam/whatever","127.0.0.1")]
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