using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardReaderLibrary.Serial;
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

    public string[] AvailablePorts()
    {
        return SerialPort.GetPortNames();
    }

    public bool IsOpen()
    {
        return serialPort.IsOpen;
    }

    public void OpenConnection()
    {
        if (!serialPort.IsOpen)
        {
            serialPort.Open();
        }
    }

    public void CloseConnection()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
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

    public static (bool locked, bool open, bool alarm, int breachState, DateTime time) ExtractState(string message)
    {
        const string pattern = @"([A-Z])(\d+)";
        const string dateFormat = "yyyyMMdd";
        const string timeFormat = "HHmmss";
        MatchCollection matches = Regex.Matches(message, pattern);
        var dataBetweenLetters = new Dictionary<char, string>();

        foreach (Match match in matches)
        {
            char letters = match.Groups[1].Value[0];
            string number = match.Groups[2].Value;
            dataBetweenLetters[letters] = number;
        }

        bool isLocked = dataBetweenLetters['D'][5] == '1';
        bool isOpen = dataBetweenLetters['D'][6] == '1';
        bool isAlarm = dataBetweenLetters['D'][7] == '1';
        int isBreachState = int.Parse(dataBetweenLetters['F']);
        string date = dataBetweenLetters['B'];
        string time = dataBetweenLetters['C'];
        DateTime isDateTime = DateTime.ParseExact(date + time, dateFormat + timeFormat, CultureInfo.InvariantCulture);

        return (isLocked, isOpen, isAlarm, isBreachState, isDateTime);
    }

    public async Task SendCommandAsync(string command)
    {
        if (serialPort.IsOpen)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes(command);
            await serialPort.BaseStream.WriteAsync(commandBytes);
        }
    }

}
