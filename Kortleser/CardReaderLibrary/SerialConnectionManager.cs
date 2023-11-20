using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class SerialConnectionManager
{
    public string Port;

    private const char startChar = '$';
    private const char endChar = '#';
    private const int baudRate = 9600;

    private readonly SerialPort serialPort;
    private readonly StringBuilder buffer;

    public delegate void DataReceivedHandler(string message);
    public event DataReceivedHandler? DataReceived;
    public event EventHandler<string>? LogMessage;

    public SerialConnectionManager(string port)
    {
        Port = port;
        buffer = new StringBuilder();
        serialPort = new()
        {
            BaudRate = baudRate,
            PortName = port
        };
        serialPort.DataReceived += OnDataReceived;
    }

    public void OpenConnection()
    {
        if (!serialPort.IsOpen)
        {
            serialPort.Open();
            OnLogMessage("Serial port opened.");
        }
    }

    public void CloseConnection()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
            OnLogMessage("Serial port closed.");
        }
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string data = serialPort.ReadExisting();
        buffer.Append(data);

        if (buffer.ToString().Contains(startChar) && buffer.ToString().Contains(endChar))
        {
            string message = ExtractMessage(buffer.ToString());
            DataReceived?.Invoke(message);

            buffer.Clear();
        }

    }

    private string ExtractMessage(string data)
    {
        int startIndex = data.IndexOf(startChar);
        int endIndex = data.IndexOf(endChar);

        return data[startIndex..endIndex];
    }

    public async Task SendCommandAsync(string command)
    {
        if (serialPort.IsOpen)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes(command);
            await serialPort.BaseStream.WriteAsync(commandBytes);
        }
    }

    protected virtual void OnLogMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }

}
