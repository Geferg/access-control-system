﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp.TcpRequests;
public interface IRequest
{
    string? RequestType { get; set; }
}
