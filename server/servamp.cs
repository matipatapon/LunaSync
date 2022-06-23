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
/// Shared settings between objects 
/// </summary>
abstract public class settings{
    protected TraceSwitch traceSwitch;

    protected settings(){
        //Setting up Trace level
        traceSwitch = new TraceSwitch("TraceServer","Level of trace messages");
        traceSwitch.Level = TraceLevel.Info;
    }
}