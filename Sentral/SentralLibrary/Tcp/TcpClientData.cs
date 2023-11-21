using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpClientData
{
    public TcpClient TcpClient { get; set; }
    public int ClientId { get; set; }
    public bool IsAuthenticated { get; set; }

    public TcpClientData(TcpClient client)
    {
        TcpClient = client;
        IsAuthenticated = false;
    }
}
