using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class RequestReceivedEventArgs
{
    public string Message { get; }

    public RequestReceivedEventArgs(string message)
    {
        Message = message;
    }
}
