using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public static class TcpConnectionDictionary
{
    public const string authorizationRequestType = "Authorization";

    public const string status_fail = "failure";
    public const string status_notAccepted = "rejected";
    public const string status_accepted = "accepted";
}
