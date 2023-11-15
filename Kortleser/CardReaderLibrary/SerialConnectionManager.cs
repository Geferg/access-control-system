using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class SerialConnectionManager
{
    private SerialPort serialPort;
    private StringBuilder buffer;

    public delegate void DataReceivedHandler(string message);
    public event DataReceivedHandler? DataReceived;
    public event EventHandler<string>? LogMessage;

    public SerialConnectionManager(string port)
    {
        buffer = new StringBuilder();
        serialPort = new()
        {
            BaudRate = 9600,
            PortName = port
        };
        serialPort.DataReceived += OnDataReceived;
    }

    public void OpenConnection()
    {

    }

    public void CloseConnection()
    {

    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {

    }

    private string ExtractMessage()
    {
        //? move to separate class

        return "";
    }

    public async Task SendCommandAsync(string command)
    {

    }

    protected virtual void OnLogMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }

}
