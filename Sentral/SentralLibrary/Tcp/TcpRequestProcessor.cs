using Newtonsoft.Json;
using SentralLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.Tcp;
public class TcpRequestProcessor
{
    public void ProccessClientRequest(TcpClientData clientData, string requestString)
    {
        IRequest request = JsonConvert.DeserializeObject<IRequest>(requestString);

        //TODO delegate to methods for specific request types

    }

}
