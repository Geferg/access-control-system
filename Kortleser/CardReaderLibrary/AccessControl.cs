using CardReaderLibrary.Serial;
using CardReaderLibrary.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CardReaderLibrary;
public class AccessControl
{
    private const int doorOpenTimeoutThreshold = 15;
    private const int doorOpenOnUnlockThreshold = 10;
    private const int doorBreachLevelThreshold = 500;
    private const int lockPin = 5;
    private const int openPin = 6;
    private const int alarmPin = 7;

    private readonly int accessPointNumber;
    private readonly SerialProcessing serialProcessing;
    private readonly TcpProcessing tcpProcessing;

    private bool doorLocked;
    private bool doorOpen;
    private DateTime lastOpened;
    private System.Timers.Timer unlockTimer;
    private System.Timers.Timer openDoorTimer;

    public AccessControl(SerialProcessing serialProcessing, TcpProcessing tcpProcessing, int accessPointNumber)
    {
        this.tcpProcessing = tcpProcessing;
        this.serialProcessing = serialProcessing;
        this.accessPointNumber = accessPointNumber;

        unlockTimer = new System.Timers.Timer(doorOpenOnUnlockThreshold * 1000);
        openDoorTimer = new System.Timers.Timer(doorOpenTimeoutThreshold * 1000);

    }

    public async Task UnlockDoor()
    {
        await serialProcessing.ChangeDigitalOutputs(lockPin, false);
        doorLocked = false;
        unlockTimer.Start();
    }

    private async Task LockDoor()
    {
        await serialProcessing.ChangeDigitalOutputs(lockPin, false);
        doorLocked = true;
        unlockTimer.Stop();
    }

    private void DoorOpened(DateTime time)
    {
        doorOpen = true;
        lastOpened = time;
        openDoorTimer.Start();
    }

    private async Task DoorClosed()
    {
        doorOpen = false;
        await LockDoor();
    }

    private async void OnUnlockTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (!doorOpen)
        {
            await LockDoor();
        }
    }

    private async void OnOpenDoorTimerElapsed(object sender, ElapsedEventArgs e)
    {

    }
}
