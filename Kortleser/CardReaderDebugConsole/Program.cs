using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;
using Newtonsoft.Json;
using System.IO.Ports;
using CardReaderLibrary.Tcp.TcpRequests;
using CardReaderLibrary.Tcp;
using CardReaderLibrary.Serial;
using System.Net;

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
    // LOOPBACK IP!
    private const string tcpIP = "127.0.0.1";

    private static readonly TcpConnectionManager tcpConnection = new(tcpIP, 8000);
    private static int accessPointNumber;
    private static DoorStateManager? doorStateManager;

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Kortleser\u0007");
        Console.Clear();

        SerialConnectionManager serialConnection;

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
                Console.WriteLine($"Connected to serial port {serialConnection.Port}");
                break;
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not connect to serial port {serialConnection!.Port}, retrying");
                continue;
            }
        }


        while (!tcpConnection.Connected)
        {
            try
            {
                tcpConnection.OpenConnection();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to central, retrying");
                Thread.Sleep(2000);
            }
        }

        tcpConnection.OpenConnection();

        Console.WriteLine($"connected to server {tcpConnection!.ServerAddress}");

        SerialProcessing serialProcessing = new(serialConnection);
        TcpProcessing tcpProcessing = new(tcpConnection);

        await AuthorizeTcpConnection(tcpProcessing);

        doorStateManager = new(serialProcessing, tcpProcessing, accessPointNumber);

        while (true)
        {
            string cardId = GetFourDigitInput("enter id");
            string cardPin = GetFourDigitInput("enter pin");

            bool accessGranted = await CheckUserAccess(cardId, cardPin, tcpProcessing);

            if (accessGranted)
            {
                await doorStateManager.UnlockDoor();
                Console.WriteLine("door unlocked");
            }
        }
    }

    // NEW METHODS

    private static async Task<bool> CheckUserAccess(string cardId, string cardPin, TcpProcessing tcpProcessing)
    {
        Response? response = await tcpProcessing.SendAccessRequestAsync(accessPointNumber, cardId, cardPin, DateTime.Now);

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

    private static async Task AuthorizeTcpConnection(TcpProcessing tcpProcessing)
    {
        bool isAuthorized = false;
        while (!isAuthorized)
        {
            accessPointNumber = GetValidatedNumberInput("access point number", 1, 99);
            Response? authorizationResponse = await tcpProcessing.SendAuthorizationRequestAsync(accessPointNumber);

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

    // ========================= INPUT HANDLERS =========================

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
