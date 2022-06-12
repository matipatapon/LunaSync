// See https://aka.ms/new-console-template for more information
using host;
using System.Threading;
using static System.Console;
var serv = new server(); 
var cli = new client();
serv.StartServerThread();
cli.StartClient();
ReadLine();