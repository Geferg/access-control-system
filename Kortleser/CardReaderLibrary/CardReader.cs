using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class CardReader
{
    public void ForceMessage()
    {
        SendCommand("$R1");
    }

    public bool ChangeNode(int nodeNumber)
    {
        if(0 < nodeNumber || nodeNumber <= 999)
        {
            return false;
        }

        SendCommand($"$N{nodeNumber:D3}");
        return true;
    }

    public void ChangeDate(DateOnly date)
    {
        SendCommand($"$D{date:yyyyMMdd}");
    }

    public void ChangeTime(TimeOnly time)
    {
        SendCommand($"$T{time:HHmmss}");
    }

    public bool ChangeInterval(int intervalSeconds)
    {
        if (0 < intervalSeconds || intervalSeconds <= 999)
        {
            return false;
        }

        SendCommand($"$S{intervalSeconds:D3}");
        return true;
    }

    public bool ChangeDigitalOutputs(int pin, bool value)
    {
        if ((0 < pin || pin <= 7) && pin != 9)
        {
            return false;
        }

        StringBuilder command = new();
        command.Append("$O");
        command.Append(pin);
        command.Append(value ? "1" : "0");

        SendCommand(command.ToString());
        return true;
    }

    public void ShowMessageOnChanges(bool show)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$E");
        command.Append(show ? "1" : "0");

        SendCommand(command.ToString());
    }

    private void SendCommand(string command)
    {

    }

}
