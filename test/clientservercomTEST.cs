using Xunit;
using host.server;
using host.client;
using System.Net;
using System.Net.Sockets;
using System;
namespace clienttest;
public class connectionTEST{
    [Fact]
    public void connect_to_server_SUCCESS()
    {
        // arrange
        var serv = new server();
        var cli = new client();
        byte[] addr = new byte[4];
        addr[0] = 127;
        addr[3] = 1;
        IPAddress ip = new IPAddress(addr);
        int port = serv.getPort();
        // act
        bool connected = cli.connect(ip,port);
    
        // assert
        Assert.Equal(connected,true);
    }
    [Fact]
    public void sendData_to_server_SUCCESS(){
        // arrange 
        var serv = new server();
        var cli = new client();
        // act 
        
    }
}