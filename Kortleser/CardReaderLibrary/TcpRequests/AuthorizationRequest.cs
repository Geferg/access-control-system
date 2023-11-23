using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.TcpRequests;
public class AuthorizationRequest : IRequest
{
    public int ClientId { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? RequestType { get; }

    public AuthorizationRequest()
    {
        RequestType = TcpRequestConstants.RequestAuthorization;
    }
}