﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.TcpRequests;
public class AlarmReportRequest : IRequest
{
    public DateTime Time { get; set; }
    public string? AlarmType { get; set; }
    public string? RequestType { get; set; }

    public AlarmReportRequest()
    {
        RequestType = TcpConnectionDictionary.request_alarmReport;
    }
}