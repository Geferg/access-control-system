using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
internal class AuthorizationRequest : BaseRequest
{
    public int ClientId { get; set; }
    public string AdditionalInfo { get; set; }

    public AuthorizationRequest(int id)
    {
        ClientId = id;
        AdditionalInfo = string.Empty;
        RequestType = TcpConnectionDictionary.authorizationRequestType;
    }
}
