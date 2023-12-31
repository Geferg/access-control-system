﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public class Response
{
    public string Action { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }

    public Response(string action, string status, string message)
    {
        Action = action;
        Status = status;
        Message = message;
    }

    public Response()
    {
        Action = "";
        Status = "";
        Message = "";
    }
}