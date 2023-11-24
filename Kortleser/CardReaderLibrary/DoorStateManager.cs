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
    private DateTime LastTimeUnlocked { get; set; }
    private bool HasBeenOpenedAfterUnlock { get; set; }

    public DoorStateManager(SerialProcessing serialProcessing, TcpProcessing tcpProcessing, int nodeNumber)
    {
        this.serialProcessing = serialProcessing;
        this.tcpProcessing = tcpProcessing;
        _ = serialProcessing.ChangeNode(nodeNumber);

        serialProcessing.DataReceived += OnHardwareMessageReceived;

        IsOpen = false;
        IsLocked = false;
        HasBeenOpenedAfterUnlock = false;
        BreachLevel = 0;
        LastTimeClosed = DateTime.Now;
        LastTimeUnlocked = DateTime.Now;
    }

    public async Task UnlockDoor()
    {
        await serialProcessing.ChangeDigitalOutputs(lockPin, false);
        IsLocked = false;
        HasBeenOpenedAfterUnlock = false;
        //LastTimeUnlocked = time;
        //TODO update unlock time
    }

    public async Task LockDoor()
    {
        await serialProcessing.ChangeDigitalOutputs(lockPin, true);
        IsLocked = true;
    }

    private async Task DoorOpened(DateTime time)
    {
        await serialProcessing.ChangeDigitalOutputs(openPin, true);
        IsOpen = true;
        HasBeenOpenedAfterUnlock = true;

        if (LastTimeClosed.AddSeconds(doorOpenTimeoutThreshold) < time && !IsTimeout)
        {
            await TimeoutAlarm(time);
            IsTimeout = true;
        }
    }

    private async Task DoorClosed(DateTime time)
    {
        await serialProcessing.ChangeDigitalOutputs(openPin, false);
        if (HasBeenOpenedAfterUnlock || LastTimeUnlocked.AddSeconds(doorOpenTimeoutThreshold) < time)
        {
            await LockDoor();
        }
        IsOpen = false;
        LastTimeClosed = time;
        IsTimeout = false;
    }

    private async Task SetBreachLevel(int breachLevel, DateTime time)
    {
        BreachLevel = breachLevel;

        if (BreachLevel > doorBreachLevelThreshold && !IsBreached)
        {
            IsBreached = true;
            await BreachAlarm(time);
        }
        else if (BreachLevel <= doorBreachLevelThreshold)
        {
            IsBreached = false;
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

    private async Task CheckAlarms()
    {
        if (IsBreached || IsTimeout)
        {
            await serialProcessing.ChangeDigitalOutputs(alarmPin, true);
        }
        else
        {
            await serialProcessing.ChangeDigitalOutputs(alarmPin, false);
        }
    }

    private async Task GetTimeNow()
    {
        await serialProcessing.ForceMessage();
    }

    private async void OnHardwareMessageReceived(string message)
    {
        var (locked, open, alarm, breachLevel, time) = SerialConnectionManager.ExtractState(message);

        if (open)
        {
            if (!IsLocked)
            {
                await DoorOpened(time);
            }
        }
        else
        {
            await DoorClosed(time);
        }

        await SetBreachLevel(breachLevel, time);
        await CheckAlarms();

        //Console.WriteLine($"Received (hardware): {message}");
        //Console.WriteLine($"\nLocked: {locked}");
        //Console.WriteLine($"Open: {open}");
        //Console.WriteLine($"Alarm: {alarm}");
        //Console.WriteLine($"Breach State: {breachLevel}");
        //Console.WriteLine($"Time: {time}");

    }

}
