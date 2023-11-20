using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public static class TcpConnectionDictionary
{
    public const string request_authorization = "Authorization";
    public const string request_alarmReport = "AlarmReport";
    public const string request_accessReport = "AccessReport";
    public const string request_access = "Access";

    public const string alarm_breach = "breach";
    public const string alarm_timeout = "timeout";

    public const string status_fail = "failure";
    public const string status_notAccepted = "rejected";
    public const string status_accepted = "accepted";
}
