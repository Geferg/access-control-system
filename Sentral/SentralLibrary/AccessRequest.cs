using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class AccessRequest : BaseRequest
{
    public int ClientId { get; set; }
    public string CardId { get; set; }
    public string Pin {  get; set; }

    public AccessRequest(int clientId, string cardId, string pin)
    {
        ClientId = clientId;
        CardId = cardId;
        Pin = pin;
        RequestType = TcpConnectionDictionary.request_access;
    }

    public AccessRequest()
    {
        ClientId = -1;
        CardId = "";
        Pin = "";
        RequestType = TcpConnectionDictionary.request_access;
    }
}
