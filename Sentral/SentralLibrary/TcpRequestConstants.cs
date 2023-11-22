﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public static class TcpRequestConstants
{
    // Request Types
    public const string RequestAuthorization = "Authorization";
    public const string RequestAlarmReport = "AlarmReport";
    public const string RequestAccessReport = "AccessReport";
    public const string RequestAccess = "Access";
    public const string RequestInvalid = "None";

    // Statuses
    public const string StatusFail = "failure";
    public const string StatusNotAccepted = "rejected";
    public const string StatusAccepted = "accepted";


    // Alarm Types
    public const string AlarmBreach = "breach";
    public const string AlarmTimeout = "timeout";
}
