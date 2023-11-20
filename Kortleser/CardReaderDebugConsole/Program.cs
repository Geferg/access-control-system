using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;
using Newtonsoft.Json;

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

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Kortleser\u0007");
        Console.Clear();

        while (serialConnection == null)
        {
            try
            {
                serialConnection = new("COM6");
            }
            catch (Exception)
            {
                Console.WriteLine("could not connect to serial port");
                Thread.Sleep(2000);
            }
        }
        Console.WriteLine($"connected to serial port {serialConnection.Port}");

        while (tcpConnection == null)
        {
            try
            {
                tcpConnection = new("127.0.0.1", 8000);
            }
            catch (Exception)
            {
                Console.WriteLine("could not connect to central");
                Thread.Sleep(2000);
            }
        }
        Console.WriteLine($"connected to server {tcpConnection.ServerAddress}");

        serialConnection.DataReceived += OnHardwareMessageReceived;

        // Add to central
        Response? authorizationResponse = null;
        while (authorizationResponse == null || authorizationResponse.Status != TcpConnectionDictionary.status_accepted)
        {
            int id = GetValidatedNumberInput("access point number", 1, 99);
            authorizationResponse = await SendAuthorizationRequestAsync(id);

            Console.WriteLine(authorizationResponse?.Message ?? "failed to get response");
        }

        // Communicate with hardware
        //serialConnection.OpenConnection();


        Console.WriteLine("Press any key to close.");
        Console.ReadKey(true);
        serialConnection.CloseConnection();
    }

    private static async Task<Response?> SendAuthorizationRequestAsync(int id)
    {
        try
        {
            AuthorizationRequest requestObject = new(id);
            string requestJson = JsonConvert.SerializeObject(requestObject);
            string responseJson = await tcpConnection.SendRequestAsync(requestJson);
            return JsonConvert.DeserializeObject<Response>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private static int GetValidatedNumberInput(string prompt, int minValue, int maxValue)
    {
        string? input = null;
        InputValidation validation = InputValidation.NotValidated;

        while (validation != InputValidation.Valid)
        {
            Console.Write($"> {prompt}: ");
            input = Console.ReadLine();

            validation = ValidateNumberInput(input, minValue, maxValue);

            switch (validation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("Input cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("Input must be a number\n");
                    break;

                case InputValidation.OutsideRange:
                    Console.WriteLine($"Input must be between {minValue} and {maxValue}\n");
                    break;

                case InputValidation.IncorrectLength:
                    Console.WriteLine("Input has incorrect length\n");
                    break;
            }
        }

        return int.Parse(input!);
    }

    private static InputValidation ValidateNumberInput(string? input, int minValue, int maxValue)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return InputValidation.IsEmpty;
        }
        else if (!int.TryParse(input, out int number))
        {
            return InputValidation.IncorrectFormat;
        }
        else if (number < minValue || number > maxValue)
        {
            return InputValidation.OutsideRange;
        }

        return InputValidation.Valid;
    }

    private static async void OnHardwareMessageReceived(string message)
    {
        Console.WriteLine($"Received (hardware): {message}");
        await tcpConnection.SendRequestAsync(message);
        Console.WriteLine($"Sent (central): {message}");
        string response = await tcpConnection.ReceiveResponseAsync();
        Console.WriteLine($"Recieved (central): {response}");
    }

    private static void OnLogMessageReceived(object? sender, string message)
    {
        Console.WriteLine($"Log: {message}");
    }


    private enum InputValidation
    {
        NotValidated,
        Valid,
        IsEmpty,
        OutsideRange,
        IncorrectLength,
        IncorrectFormat,
    }
}
