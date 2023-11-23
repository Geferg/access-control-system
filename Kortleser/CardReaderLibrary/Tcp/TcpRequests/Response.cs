using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CardReaderLibrary.Tcp.TcpRequests;
public class Response
{
    public string Action { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }

    public Response()
    {
        Action = "";
        Status = "";
        Message = "";
    }
}
