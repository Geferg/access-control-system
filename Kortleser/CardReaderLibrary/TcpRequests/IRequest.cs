using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.TcpRequests;
public interface IRequest
{
    string? RequestType { get; }
}