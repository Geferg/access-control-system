using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class Central
{
    public TcpClient Client { get; set; }
    private List<CardReader> cardReaders;
    private List<UserData> userData;

    public Central()
    {
        Client = new TcpClient();
        cardReaders = new List<CardReader>();
        userData = new List<UserData>();
    }

    //TODO hook up card reader access requests








    //TODO hook up database

    //TODO report generator

    //TODO make admin methods/fields

    //TODO make alarm triggers (event)
}
