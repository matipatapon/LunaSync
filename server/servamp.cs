using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using static System.Console;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace host;
/// <summary>
/// Shared functions between client and server
/// SERVer And clieNT = SERVANT +- SERVAMP
/// </summary>
abstract public class servamp{
    protected TraceSwitch traceSwitch;

    protected servamp(){
        //Setting up Trace level
        traceSwitch = new TraceSwitch("TraceServer","Level of trace messages");
        traceSwitch.Level = TraceLevel.Info;
    }
}