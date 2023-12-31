﻿using Newtonsoft.Json;
using SentralLibrary.Tcp.DataClasses;
using SentralLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.Processing;
public class TcpResponseProcessing
{
    public void SendResponse(TcpClientData clientData, Response response)
    {
        string jsonResponse = JsonConvert.SerializeObject(response);
        var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
        clientData.TcpClient.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
