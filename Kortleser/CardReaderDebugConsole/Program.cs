using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;
using Newtonsoft.Json;
using CardReaderLibrary.TcpRequests;

/*
 * Concerns:
 * - Handling partial messages
 * - Resource management for tcpclient and networkstream (using or dispose)
 * - Fixed buffer size
 * - Testing different network scenarios
 */

namespace CardReaderDebugConsole;
internal class Program
{
    private static SerialConnectionManager? serialConnection;
    private static TcpConnectionManager? tcpConnection;
    private static int accessPointNumber;

    private static DateTime lastTimeClosed = DateTime.Now;
    private static DateTime lastTimeLocked = DateTime.Now;

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Kortleser\u0007");
        Console.Clear();

        //TODO take com port input (list available?)

        InitializeSerialConnection("COM7");
        Console.WriteLine($"initialized serial port {serialConnection!.Port}");

        OpenSerialConnection(serialConnection);
        Console.WriteLine($"Connected to serial port {serialConnection.Port}");

        serialConnection.DataReceived += OnHardwareMessageReceived;

        InitializeTcpConnection("127.0.0.1", 8000);
        Console.WriteLine($"connected to server {tcpConnection!.ServerAddress}");

        await AuthorizeTcpConnection();

        //TODO take id and pin and send request to central


        Console.WriteLine("Press escape to close.");
        while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        serialConnection.CloseConnection();
    }

    // NEW METHODS

    private static async Task AuthorizeTcpConnection()
    {
        bool isAuthorized = false;
        while (!isAuthorized)
        {
            int id = GetValidatedNumberInput("access point number", 1, 99);
            Response? authorizationResponse = await SendAuthorizationRequestAsync(id);

            if (authorizationResponse == null)
            {
                Console.WriteLine("failed to get a response from the server.");
            }
            else
            {
                Console.WriteLine(authorizationResponse.Message);
                isAuthorized = authorizationResponse.Status == TcpConnectionDictionary.status_accepted;
            }
        }
    }

    // OLD METHODS

    private static async void OnHardwareMessageReceived(string message)
    {
        //TODO link access attempts to lock state
        //TODO lock door when closed

        bool debug = true;

        var (locked, open, alarm, breachState, time) = SerialConnectionManager.ExtractState(message);

        if (debug)
        {
            Console.WriteLine($"Received (hardware): {message}");
            Console.WriteLine($"Locked: {locked}");
            Console.WriteLine($"Open: {open}");
            Console.WriteLine($"Alarm: {alarm}");
            Console.WriteLine($"Breach State: {breachState}");
            Console.WriteLine($"Time: {time}");
        }

        if (!open)
        {
            lastTimeClosed = time;
        }

        if (!locked)
        {
            lastTimeLocked = time;
        }

        if (breachState > 500)
        {
            Response? breachResponse = await SendAlarmReportRequestAsync(time, TcpConnectionDictionary.alarm_breach);

            if (breachResponse == null)
            {
                Console.WriteLine("failed to get a response from the server.");
            }
            else
            {
                Console.WriteLine(breachResponse.Message);
            }
        }

        if (open && lastTimeClosed.AddSeconds(30) < time)
        {
            Response? timeoutResponse = await SendAlarmReportRequestAsync(time, TcpConnectionDictionary.alarm_timeout);

            if (timeoutResponse == null)
            {
                Console.WriteLine("failed to get a response from the server.");
            }
            else
            {
                Console.WriteLine(timeoutResponse.Message);
            }
        }

        //await tcpConnection!.SendRequestAsync(message);
        //Console.WriteLine($"Sent (central): {message}");
        //string response = await tcpConnection.ReceiveResponseAsync();
        //Console.WriteLine($"Recieved (central): {response}");
    }

    private static void InitializeSerialConnection(string portName)
    {
        while (serialConnection == null)
        {
            try
            {
                serialConnection = new SerialConnectionManager(portName);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not initialize serial port");
                Thread.Sleep(2000);
            }
        }
    }

    private static void OpenSerialConnection(SerialConnectionManager connection)
    {
        while (!connection.IsOpen())
        {
            try
            {
                serialConnection!.OpenConnection();
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not connect to serial port {serialConnection!.Port}");
                Thread.Sleep(2000);
            }
        }
    }

    private static void InitializeTcpConnection(string ipAddress, int port)
    {
        while (tcpConnection == null)
        {
            try
            {
                tcpConnection = new TcpConnectionManager(ipAddress, port);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to central");
                Thread.Sleep(2000);
            }
        }
    }

    private static async Task<Response?> SendAuthorizationRequestAsync(int id)
    {
        try
        {
            AuthorizationRequest requestObject = new();
            requestObject.ClientId = id;
            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection!.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private static async Task<Response?> SendAlarmReportRequestAsync(DateTime time, string alarmType)
    {
        try
        {
            AlarmReportRequest requestObject = new()
            {
                AlarmType = alarmType,
                Time = time
            };
            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection!.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response?>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private static int GetValidatedNumberInput(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write($"> {prompt}: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty\n");
                continue;
            }

            if (!int.TryParse(input, out int number))
            {
                Console.WriteLine("Input must be a number\n");
                continue;
            }

            if (number < minValue || number > maxValue)
            {
                Console.WriteLine($"Input must be between {minValue} and {maxValue}\n");
                continue;
            }

            return number;
        }
    }
}
