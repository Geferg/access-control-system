using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class AlarmReportRequest : BaseRequest
{
    public DateTime Time {  get; set; }
    public string AlarmType { get; set; }

    public AlarmReportRequest(DateTime time, string alarmType)
    {
        RequestType = TcpConnectionDictionary.request_alarmReport;
        Time = time;
        AlarmType = alarmType;
    }

    public AlarmReportRequest()
    {
        AlarmType = "";
        RequestType = TcpConnectionDictionary.request_alarmReport;
    }

}
