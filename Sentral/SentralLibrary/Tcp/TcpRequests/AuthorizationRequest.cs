using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
internal class AuthorizationRequest : IRequest
{
    public int ClientId { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? RequestType { get; set; }

    public AuthorizationRequest()
    {
        RequestType = SentralLibrary.TcpRequestConstants.request_authorization;
    }
}
