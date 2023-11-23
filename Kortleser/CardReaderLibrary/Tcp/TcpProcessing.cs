using CardReaderLibrary.Tcp.TcpRequests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.Tcp;
public class TcpProcessing
{
    private readonly TcpConnectionManager tcpConnection;

    public TcpProcessing(TcpConnectionManager tcpConnection)
    {
        this.tcpConnection = tcpConnection;
    }

    public async Task<Response?> SendAuthorizationRequestAsync(int id)
    {
        try
        {
            AuthorizationRequest requestObject = new();
            requestObject.ClientId = id;
            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection!.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Response?> SendAlarmReportRequestAsync(DateTime time, string alarmType)
    {
        try
        {
            AlarmReportRequest requestObject = new()
            {
                AlarmType = alarmType,
                Time = time
            };
            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection!.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response?>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Response?> SendAccessRequestAsync(int accessPoint, string cardId, string pin, DateTime time)
    {
        try
        {
            AccessRequest requestObject = new()
            {
                DoorNumber = accessPoint,
                CardId = cardId,
                Pin = pin,
                Time = time
            };

            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection!.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response?>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

}
