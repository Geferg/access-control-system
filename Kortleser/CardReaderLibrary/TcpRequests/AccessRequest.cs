﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.TcpRequests;
public class AccessRequest : IRequest
{
    public int ClientId { get; set; }
    public string? CardId { get; set; }
    public string? Pin { get; set; }
    public string? RequestType { get; set; }
    public DateTime Time { get; set; }
    public int DoorNumber { get; set; }

    public AccessRequest()
    {
        RequestType = TcpRequestConstants.RequestAccess;
    }
}