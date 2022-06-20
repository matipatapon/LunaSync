// See https://aka.ms/new-console-template for more information
using host.client;
using host.server;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Console;

//Test zone !!!!
//cli.sandbox();
 //Setting trace level
var myWriter = new TextWriterTraceListener(File.CreateText("log.txt"));
Trace.AutoFlush = true;
Trace.Listeners.Add(myWriter);
//Commenting out normal behavior !

var cli = new client();
var server = new server();

