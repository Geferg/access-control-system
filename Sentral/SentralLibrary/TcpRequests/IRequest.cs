using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.TcpRequests;
internal interface IRequest
{
    string? RequestType { get; set; }
}
