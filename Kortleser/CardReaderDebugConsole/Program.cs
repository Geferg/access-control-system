using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;
using Newtonsoft.Json;
using System.IO.Ports;
using CardReaderLibrary.Tcp.TcpRequests;
using CardReaderLibrary.Tcp;
using CardReaderLibrary.Serial;

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
    private static TcpConnectionManager tcpConnection = new("127.0.0.1", 8000);
    private static int accessPointNumber;
    private static DoorStateManager? doorStateManager;

    private static DateTime lastTimeClosed = DateTime.Now;
    private static DateTime lastTimeLocked = DateTime.Now;

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Kortleser\u0007");
        Console.Clear();

        //TODO take com port input (list available?)

        InitializeSerialConnection();

        Console.WriteLine($"Connected to serial port {serialConnection!.Port}");

        serialConnection.DataReceived += OnHardwareMessageReceived;

        InitializeTcpConnection("127.0.0.1", 8000);
        Console.WriteLine($"connected to server {tcpConnection!.ServerAddress}");

        await AuthorizeTcpConnection();

        doorStateManager = new(serialConnection, tcpConnection);

        string cardId = GetFourDigitInput("enter id");
        string cardPin = GetFourDigitInput("enter pin");

        bool accessGranted = await CheckUserAccess(cardId, cardPin);

        //TODO unlock door

        //TODO on door close, lock door

        Console.WriteLine("Press escape to close.");
        while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        serialConnection.CloseConnection();
    }

    // NEW METHODS

    private static async Task<bool> CheckUserAccess(string cardId, string cardPin)
    {
        Response? response = await SendAccessRequestAsync(accessPointNumber, cardId, cardPin, DateTime.Now);

        if (response == null)
        {
            Console.WriteLine("error in response");
            return false;
        }

        Console.WriteLine(response.Message);

        if (response.Status != TcpRequestConstants.StatusAccepted)
        {
            return false;
        }

        return true;
    }

    private static async Task AuthorizeTcpConnection()
    {
        bool isAuthorized = false;
        while (!isAuthorized)
        {
            accessPointNumber = GetValidatedNumberInput("access point number", 1, 99);
            Response? authorizationResponse = await SendAuthorizationRequestAsync(accessPointNumber);

            if (authorizationResponse == null)
            {
                Console.WriteLine("failed to get a response from the server.");
            }
            else
            {
                Console.WriteLine(authorizationResponse.Message);
                isAuthorized = authorizationResponse.Status == TcpRequestConstants.StatusAccepted;
            }
        }
    }

    private static void InitializeSerialConnection()
    {
        while (true)
        {
            while (SerialPort.GetPortNames().Length == 0)
            {
                Console.WriteLine("no com ports available, retrying");
                Thread.Sleep(2000);
            }

            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("select a port:");
            foreach (var port in ports)
            {
                Console.WriteLine(port);
            }

            Console.Write("> ");
            string? selectedPort = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrEmpty(selectedPort) || Array.IndexOf(ports, selectedPort) == -1)
            {
                Console.WriteLine("invalid com port, retrying");
                continue;
            }

            try
            {
                serialConnection = new SerialConnectionManager(selectedPort);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not initialize serial port, retrying");
                continue;
            }

            try
            {
                serialConnection.OpenConnection();
                return;
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not connect to serial port {serialConnection!.Port}, retrying");
                continue;
            }
        }
    }


    // ===================== CENTRAL CONNECTION =====================

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
                Console.WriteLine("Could not connect to central, retrying");
                Thread.Sleep(2000);
            }
        }
    }

    // OLD METHODS

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

    private static string GetFourDigitInput(string prompt)
    {
        string input;

        while (true)
        {
            Console.Write($"> {prompt}: ");
            input = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("card id cannot be empty\n");
                continue;
            }

            if (!int.TryParse(input, out int number))
            {
                Console.WriteLine("card id must be a number\n");
                continue;
            }

            if (input.Length != 4 || number < 0 || number > 9999)
            {
                Console.WriteLine("Card ID must be a 4-digit number between 0000 and 9999.\n");
                continue;
            }

            break;
        }

        return input;
    }
}
