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
    private const int lockPin = 5;
    private const int openPin = 6;
    private const int alarmPin = 7;

    private readonly SerialProcessing serialProcessing;
    private readonly TcpProcessing tcpProcessing;

    public bool IsOpen { get; private set; }
    public bool IsLocked { get; private set; }
    public int BreachLevel { get; private set; }

    public bool IsBreached { get; private set; }
    public bool IsTimeout { get; private set; }

    private DateTime LastTimeClosed { get; set; }

    public DoorStateManager(SerialProcessing serialProcessing, TcpProcessing tcpProcessing)
    {
        this.serialProcessing = serialProcessing;
        this.tcpProcessing = tcpProcessing;

        serialProcessing.DataReceived += OnHardwareMessageReceived;

        IsOpen = false;
        IsLocked = false;
        BreachLevel = 0;
        LastTimeClosed = DateTime.Now;
    }

    public async Task UnlockDoor()
    {
        await serialProcessing.ChangeDigitalOutputs(lockPin, false);
    }

    private async Task DoorOpened(DateTime time)
    {
        IsOpen = true;

        if (LastTimeClosed.AddSeconds(doorOpenTimeoutThreshold) < time)
        {
            await TimeoutAlarm(time);
        }
    }

    private async Task DoorClosed(DateTime time)
    {
        IsOpen = false;
        LastTimeClosed = time;

        await serialProcessing.ChangeDigitalOutputs(lockPin, true);
    }

    private void DoorLocked()
    {
        IsLocked = true;
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
            await DoorClosed(time);
        }

        if (locked)
        {
            DoorLocked();
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
