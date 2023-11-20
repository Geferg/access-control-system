using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class AccessReportRequest : BaseRequest
{
    public DateTime Time { get; set; }
    public bool Approved { get; set; }
    public int DoorNumber { get; set; }
    public string CardId { get; set; }

    public AccessReportRequest(DateTime time, string cardId, int doorNumber, bool approved)
    {
        CardId = cardId;
        DoorNumber = doorNumber;
        Approved = approved;
        Time = time;
        RequestType = TcpConnectionDictionary.request_accessReport;
    }

    public AccessReportRequest()
    {
        CardId = "";
        Approved = false;
        RequestType = TcpConnectionDictionary.request_accessReport;
    }
}
