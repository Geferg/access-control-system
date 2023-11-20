﻿using System.Data;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using SentralLibrary;

namespace SentralConsole;

internal class Program
{
    // INPUT PATTERN MATCHING
    private static readonly Regex removeRegex = new(@"^add \d{4}");
    private static readonly Regex editRegex = new(@"^edit \d{4}$");
    private static readonly Regex showRegex = new(@"^show \d{4}$");
    private static readonly Regex addRegex = new(@"^add \d{4}");

    // DATABASE CONNECTION STRINGS
    //? move to static class
    private const string dbIP = "129.151.221.119";
    private const string dbPort = "5432";
    private const string dbUsername = "599146";
    private const string dbPassword = "Ha1FinDagIDag!";
    private const string dbDatabase = "599146";

    // UI DIALOG MESSAGES
    private const string missingInDbMessage = "user not found in database";
    private const string failureInDbMessage = "could not perform operation in database";

    private static readonly DatabaseConnection dbConnection = new(dbIP, dbPort, dbUsername, dbPassword, dbDatabase);
    private static readonly TcpConnection tcpServer = new(8000);
    private static readonly UIConnection uiConnection = new();
    private static readonly UIDialogs dialogs = new(uiConnection);

    static async Task Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Sentral\u0007");
        Console.Clear();

        uiConnection.ClassToUI += OnWriteToUI;
        uiConnection.UIStringToClass += OnRecieveFromUI;
        uiConnection.UIKeyToClass += OnGetKeypress;
        tcpServer.RequestReceived += OnTcpRequest;

        // Database connection
        while (!dbConnection.TestConnection())
        {
            Console.WriteLine("failed to connect to database, retrying...");
            Thread.Sleep(1000);
        }

        // TCP connection
        tcpServer.Start();

        dialogs.ListCommands();

        while (true)
        {
            Console.Write("> ");
            string? command = Console.ReadLine();

            if (string.IsNullOrEmpty(command))
            {
                continue;
            }

            switch (command)
            {
                case "clear":
                    HandleClearCommand();
                    break;

                case "exit":
                    HandleExitCommand();
                    break;

                case "show":
                    HandleShowCommand();
                    break;

                case "database":
                    HandleDatabaseDetailsCommand(dbConnection);
                    break;

                case "system":
                    HandleSystemDetailsCommand(tcpServer);
                    break;

                default:
                    FindPatternOnCommandAndHandle(command);
                    break;
            }
        }
    }

    // CONNECTION COMMANDS
    private static void HandleDatabaseDetailsCommand(DatabaseConnection connection)
    {
        if(connection.TestConnection())
        {
            Console.WriteLine("connected to database");
            Console.WriteLine($"  Database: {connection.DatabaseName}");
            Console.WriteLine($"IP address: {connection.HostIp}");
            Console.WriteLine($"      Port: {connection.HostPort}");
        }
        else
        {
            Console.WriteLine("failed to connect to database");
            Console.WriteLine($"  Database: {connection.DatabaseName}");
            Console.WriteLine($"IP address: {connection.HostIp}");
            Console.WriteLine($"      Port: {connection.HostPort}");
        }
        Console.WriteLine("");
    }

    private static void HandleSystemDetailsCommand(TcpConnection connection)
    {
        Console.WriteLine("tcp system details");
        Console.WriteLine($"  autorized connections: {connection.GetAutorizedClientCount()}");
        Console.WriteLine($"unauthorized connetions: {connection.GetUnauthorizedClientCount()}");

        Console.WriteLine("");
    }

    // DATABASE INTERRACTION COMMANDS
    private static void FindPatternOnCommandAndHandle(string command)
    {
        string? cardId = GetNumbersFromCommand(command);
        if (string.IsNullOrEmpty(cardId))
        {
            Console.WriteLine("parameter format not accepted\n");
            return;
        }

        if (addRegex.IsMatch(command))
        {
            HandleAddSpecificCommand(cardId);
        }
        else if (showRegex.IsMatch(command))
        {
            HandleShowSpecificCommand(cardId);
        }
        else if (editRegex.IsMatch(command))
        {
            HandleEditSpecificCommand(cardId);
        }
        else if (removeRegex.IsMatch(command))
        {
            HandleRemoveSpecificCommand(cardId);
        }
        else
        {
            Console.WriteLine("command not recognized\n");
        }
    }
    private static void HandleClearCommand()
    {
        Console.Clear();
        dialogs.ListCommands();
    }
    private static void HandleExitCommand()
    {
        if (dialogs.ExitProgramConfirmation())
        {
            tcpServer.Stop();
            Environment.Exit(0);
        }
    }
    private static void HandleShowCommand()
    {
        dialogs.ShowUsers(dbConnection.GetUserbase().OrderBy(u => u.id).ToList());
    }
    private static void HandleShowSpecificCommand(string cardId)
    {
        UserData? selectedUser = dbConnection.GetUser(cardId);

        if (selectedUser != null)
        {
            dialogs.ShowUser(selectedUser);
        }
        else
        {
            Console.WriteLine(missingInDbMessage + "\n");
        }
    }
    private static void HandleAddSpecificCommand(string cardId)
    {
        if (dbConnection.UserExists(cardId))
        {
            Console.WriteLine("card id already exists!\n");
        }
        else
        {
            UserData? newUser = dialogs.MakeUser(cardId);

            if (newUser != null)
            {
                dbConnection.AddUser(newUser);
            }
        }
    }
    private static void HandleEditSpecificCommand(string cardId)
    {
        UserData? selectedUser = dbConnection.GetUser(cardId);
        if (selectedUser == null)
        {
            Console.WriteLine(missingInDbMessage);
        }
        else
        {
            UserData newUser = dialogs.EditUser(selectedUser);
            if (!dbConnection.UpdateUser(cardId, newUser))
            {
                Console.WriteLine(failureInDbMessage);
            }
        }
    }
    private static void HandleRemoveSpecificCommand(string cardId)
    {
        UserData? userToRemove = dbConnection.GetUser(cardId);

        if (userToRemove == null)
        {
            Console.WriteLine(missingInDbMessage + "\n");
        }
        else if (dialogs.DeleteUserConfirmation(userToRemove))
        {
            dbConnection.RemoveUser(userToRemove.CardID);
            Console.WriteLine($"removed user\n");
        }
    }

    // EVENT HANDLERS
    private static void OnWriteToUI(string message)
    {
        Console.Write(message);
    }
    private static string? OnRecieveFromUI()
    {
        return Console.ReadLine();
    }
    private static ConsoleKeyInfo? OnGetKeypress()
    {
        return Console.ReadKey(true);
    }

    // Deprecated? handle all on tcp class unless need for user interaction
    private static void OnTcpRequest(TcpClient client, string request, Action<TcpClient, string> respondCallback)
    {
        var requestObject = JsonSerializer.Deserialize<TcpClient>(request);
        respondCallback(client, "my response");
    }

    // Helper methods
    private static string? GetNumbersFromCommand(string command)
    {
        string pattern = @"\b\w+ (\d{4})\b";

        Match match = Regex.Match(command, pattern);

        if (match.Success)
        {
            string digits = match.Groups[1].Value;
            return digits;
        }
        else
        {
            return null;
        }
    }

}