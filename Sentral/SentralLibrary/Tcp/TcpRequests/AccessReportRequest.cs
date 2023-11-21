using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public class AccessReportRequest : IRequest
{
    public DateTime Time { get; set; }
    public bool Approved { get; set; }
    public int DoorNumber { get; set; }
    public string? CardId { get; set; }
    public string? RequestType { get; set; }

    public AccessReportRequest()
    {
        RequestType = TcpConnectionDictionary.request_accessReport;
    }
}
