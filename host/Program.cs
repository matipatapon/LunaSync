// See https://aka.ms/new-console-template for more information
using host;
using System.Threading;
using System.Diagnostics;
using static System.Console;

//Test zone !!!!
//cli.sandbox();
 //Setting trace level
TraceSwitch tracelevel = new TraceSwitch("TraceServer","Level of trace messages");
tracelevel.Level = TraceLevel.Info;
var myWriter = new TextWriterTraceListener(File.CreateText("log.txt"));
Trace.AutoFlush = true;
Trace.Listeners.Add(myWriter);
Trace.WriteLineIf(tracelevel.TraceInfo,"Error");
//Commenting out normal behavior !

var serv = new server(tracelevel); 
var cli = new client();

cli.StartClientThread();
