using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public class Request : IRequest
{
    public string? RequestType { get; set; }
}
