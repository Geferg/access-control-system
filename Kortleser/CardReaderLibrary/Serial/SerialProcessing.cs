using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary.Serial;
public class SerialProcessing
{
    private readonly SerialConnectionManager serialConnection;

    public SerialProcessing(SerialConnectionManager serialConnection)
    {
        this.serialConnection = serialConnection;
    }

    public async Task ForceMessage()
    {
        await serialConnection.SendCommandAsync("$R1");
    }

    public async Task<bool> ChangeNode(int nodeNumber)
    {
        if (0 < nodeNumber || nodeNumber <= 999)
        {
            return false;
        }

        await serialConnection.SendCommandAsync($"$N{nodeNumber:D3}");
        return true;
    }

    public async Task ChangeDate(DateOnly date)
    {
        await serialConnection.SendCommandAsync($"$D{date:yyyyMMdd}");
    }

    public async Task ChangeTime(TimeOnly time)
    {
        await serialConnection.SendCommandAsync($"$T{time:HHmmss}");
    }

    public async Task<bool> ChangeInterval(int intervalSeconds)
    {
        if (0 < intervalSeconds || intervalSeconds <= 999)
        {
            return false;
        }

        await serialConnection.SendCommandAsync($"$S{intervalSeconds:D3}");
        return true;
    }

    public async Task<bool> ChangeDigitalOutputs(int pin, bool value)
    {
        if ((0 < pin || pin <= 7) && pin != 9)
        {
            return false;
        }

        StringBuilder command = new();
        command.Append("$O");
        command.Append(pin);
        command.Append(value ? "1" : "0");

        await serialConnection.SendCommandAsync(command.ToString());
        return true;
    }

    public async Task ShowMessageOnChanges(bool show)
    {
        StringBuilder command = new StringBuilder();
        command.Append("$E");
        command.Append(show ? "1" : "0");

        await serialConnection.SendCommandAsync(command.ToString());
    }
}
