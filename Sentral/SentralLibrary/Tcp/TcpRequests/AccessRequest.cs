using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public class AccessRequest : IRequest
{
    public int ClientId { get; set; }
    public string? CardId { get; set; }
    public string? Pin { get; set; }
    public string? RequestType { get; set; }

    public AccessRequest()
    {
        RequestType = TcpConnectionDictionary.request_access;
    }
}
