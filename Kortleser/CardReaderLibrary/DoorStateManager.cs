using CardReaderLibrary.Serial;
using CardReaderLibrary.Tcp;
using CardReaderLibrary.Tcp.TcpRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderLibrary;
public class DoorStateManager
{
    private const int doorOpenTimeoutThreshold = 15;
    private const int doorBreachLevelThreshold = 500;

    private readonly SerialProcessing serialProcessing;
    private readonly TcpProcessing tcpProcessing;

    public bool IsOpen { get; private set; }
    public bool IsLocked { get; private set; }
    public int BreachLevel { get; private set; }

    private DateTime LastTimeClosed { get; set; }
    private DateTime LastTimeLocked { get; set; }


    public DoorStateManager(SerialProcessing serialProcessing, TcpProcessing tcpProcessing)
    {
        this.serialProcessing = serialProcessing;
        this.tcpProcessing = tcpProcessing;

        serialConnection.DataReceived += OnHardwareMessageReceived;

        IsOpen = false;
        IsLocked = false;
        BreachLevel = 0;
        LastTimeClosed = DateTime.Now;
        LastTimeLocked = DateTime.Now;
    }

    public void UnlockDoor()
    {

        //TODO send command to unlock door via serial (DI for serial connection probably)
    }

    private async Task DoorOpened(DateTime time)
    {
        IsOpen = true;

        if (LastTimeClosed.AddSeconds(doorOpenTimeoutThreshold) < time)
        {
            await TimeoutAlarm(time);
        }
    }

    private void DoorClosed(DateTime time)
    {
        IsOpen = false;
        LastTimeClosed = time;

        //TODO send command to lock door via serial (DI for serial connection probably)
    }

    private void DoorLocked(DateTime time)
    {
        IsLocked = true;
        LastTimeLocked = time;
    }

    private void DoorUnlocked()
    {
        IsLocked = false;
    }

    private async Task SetBreachLevel(int breachLevel, DateTime time)
    {
        BreachLevel = breachLevel;

        if (BreachLevel > doorBreachLevelThreshold)
        {
            await BreachAlarm(time);
        }
    }

    private async Task BreachAlarm(DateTime time)
    {
        Response? breachResponse = await tcpProcessing.SendAlarmReportRequestAsync(time, TcpRequestConstants.AlarmBreach);

        if (breachResponse == null)
        {
            Console.WriteLine("failed to get a response from the server.");
        }
        else
        {
            Console.WriteLine(breachResponse.Message);
        }
    }

    private async Task TimeoutAlarm(DateTime time)
    {
        Response? timeoutResponse = await tcpProcessing.SendAlarmReportRequestAsync(time, TcpRequestConstants.AlarmTimeout);

        if (timeoutResponse == null)
        {
            Console.WriteLine("failed to get a response from the server.");
        }
        else
        {
            Console.WriteLine(timeoutResponse.Message);
        }
    }

    private async void OnHardwareMessageReceived(string message)
    {
        var (locked, open, alarm, breachLevel, time) = SerialConnectionManager.ExtractState(message);

        if (open)
        {
            await DoorOpened(time);
        }
        else
        {
            DoorClosed(time);
        }

        if (locked)
        {
            DoorLocked(time);
        }
        else
        {
            DoorUnlocked();
        }

        await SetBreachLevel(breachLevel, time);

        Console.WriteLine($"Received (hardware): {message}");
        Console.WriteLine($"Locked: {locked}");
        Console.WriteLine($"Open: {open}");
        Console.WriteLine($"Alarm: {alarm}");
        Console.WriteLine($"Breach State: {breachLevel}");
        Console.WriteLine($"Time: {time}");

    }

}
