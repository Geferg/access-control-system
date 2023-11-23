using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public class AlarmReportRequest : IRequest
{
    public DateTime Time { get; set; }
    public string? AlarmType { get; set; }
    public string? RequestType { get; set; }

    public AlarmReportRequest()
    {
        RequestType = TcpRequestConstants.RequestAlarmReport;
    }
}
