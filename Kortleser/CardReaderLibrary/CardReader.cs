using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
internal class CardReader
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

        SendCommand($"$N{nodeNumber}");
        return true;
    }

    public void ChangeDate(DateOnly date)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$D");

        SendCommand(command.ToString());
    }

    public void ChangeTime(TimeOnly time)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$");

        SendCommand(command.ToString());
    }

    public bool ChangeInterval(int intervalSeconds)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$");

        SendCommand(command.ToString());
        return true;
    }

    public bool ChangeDigitalValues(int pin, int value)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$");

        SendCommand(command.ToString());
        return true;
    }

    public void ShowMessageOnChanges(bool show)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$");

        SendCommand(command.ToString());
    }

    private void SendCommand(string command)
    {

    }

}
