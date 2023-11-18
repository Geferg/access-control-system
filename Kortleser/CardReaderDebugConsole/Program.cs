using System.Net.Sockets;
using System.Text;
using CardReaderLibrary;

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
    private static SerialConnectionManager serialConnection = new("COM6");
    private static TcpConnectionManager tcpConnection = new("127.0.0.1", 8000);
    int accessPointNumber;

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Kortleser\u0007");

        GetValidatedNumberInput("card reader number", 1, 99);

        tcpConnection.LogMessage += OnLogMessageReceived;
        serialConnection.LogMessage += OnLogMessageReceived;
        serialConnection.DataReceived += OnHardwareMessageReceived;

        try
        {
            serialConnection.OpenConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
            serialConnection.CloseConnection();
        }

        /* TCP COMM. TESTING
        try
        {
            await tcpConnection.SendRequestAsync("Hello, server");

            string response = await tcpConnection.ReceiveResponseAsync();
            Console.WriteLine($"Response: {response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
            tcpConnection.CloseConnection();
        }*/


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
