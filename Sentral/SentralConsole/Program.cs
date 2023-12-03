using System.Data;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using SentralLibrary.Console;
using SentralLibrary.Database;
using SentralLibrary.Database.Config;
using SentralLibrary.Database.DataClasses;
using SentralLibrary.Database.Processing;
using SentralLibrary.Database.Services;
using SentralLibrary.Tcp;

namespace SentralConsole;

internal class Program
{
    // INPUT PATTERN MATCHING
    private static readonly Regex removeRegex = new(@"^remove \d{4}");
    private static readonly Regex editRegex = new(@"^edit \d{4}$");
    private static readonly Regex showRegex = new(@"^show \d{4}$");
    private static readonly Regex addRegex = new(@"^add \d{4}");
    private static readonly Regex alarmRegex = new(@"^alarm \d{1,3}");


    private static readonly ConsoleConnectionManager uiConnection = new();
    private static readonly ConsoleDialogs dialogs = new(uiConnection);

    static void Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Sentral\u0007");
        Console.Clear();

        // Database dependency injections
        DatabaseConnectionManager sharedDatabaseConnection = new(DatabaseCredentials.dbIP,
            DatabaseCredentials.dbDatabase, DatabaseCredentials.dbPort, DatabaseCredentials.dbUsername,
            DatabaseCredentials.dbPassword);

        DatabaseAccess sharedDatabaseAccess = new(sharedDatabaseConnection);

        DatabaseUserProcessing userProcessing = new(sharedDatabaseAccess);
        DatabaseAccessLogProcessing accessProcessing = new(sharedDatabaseAccess);
        DatabaseAlarmLogProcessing alarmLogProcessing = new(sharedDatabaseAccess);

        DatabaseService databaseService = new(userProcessing, accessProcessing, alarmLogProcessing);

        TcpConnectionManager tcpConnection = new(8000, databaseService);
        tcpConnection.Start();

        // Database connection
        while (!sharedDatabaseConnection.TestConnection())
        {
            Console.WriteLine("failed to connect to database, retrying...");
            Thread.Sleep(1000);
        }

        // Attach events for ui communication
        uiConnection.ClassToUI += (message) => Console.Write(message);
        uiConnection.UIStringToClass += () => Console.ReadLine();
        uiConnection.UIKeyToClass += () => Console.ReadKey(true);

        // Main user loop
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
                case "exit":
                    Environment.Exit(0);
                    break;

                case "clear":
                    Console.Clear();
                    dialogs.ListCommands();
                    break;

                case "show":
                    HandleShowCommand(databaseService);
                    break;

                case "database":
                    HandleDatabaseDetailsCommand(sharedDatabaseConnection);
                    break;

                case "system":
                    HandleSystemDetailsCommand(tcpConnection);
                    break;

                case "access":
                    HandleAccessLogDetailsCommand(databaseService);
                    break;

                case "alarm":
                    HandleAlarmLogDetailsCommand(databaseService);
                    break;

                case "suspicious":
                    HandleSuspiciousCommand(databaseService);
                    break;

                default:
                    FindPatternOnCommandAndHandle(databaseService, command);
                    break;
            }
        }

    }

    // DECODES COMMANDS
    private static void FindPatternOnCommandAndHandle(DatabaseService databaseService, string command)
    {
        string? cardId = GetNumbersFromCommand(command);
        if (string.IsNullOrEmpty(cardId))
        {
            Console.WriteLine("parameter format not accepted\n");
            return;
        }

        if (addRegex.IsMatch(command))
        {
            HandleAddSpecificCommand(databaseService, cardId);
        }
        else if (showRegex.IsMatch(command))
        {
            HandleShowSpecificCommand(databaseService, cardId);
        }
        else if (editRegex.IsMatch(command))
        {
            HandleEditSpecificCommand(databaseService, cardId);
        }
        else if (removeRegex.IsMatch(command))
        {
            HandleRemoveSpecificCommand(databaseService, cardId);
        }
        else if (alarmRegex.IsMatch(command))
        {
            HandleDoorLogDetailsCommand(databaseService, command);
        }
        else
        {
            Console.WriteLine("command not recognized\n");
        }
    }

    // COMMAND HANDLERS
    private static void HandleDatabaseDetailsCommand(DatabaseConnectionManager connection)
    {
        dialogs.ShowDatabaseStatus(connection);
    }

    private static void HandleSystemDetailsCommand(TcpConnectionManager connection)
    {
        dialogs.ShowTcpStatus(connection);
    }

    private static void HandleShowCommand(DatabaseService databaseService)
    {
        dialogs.ShowUsers(databaseService.GetAllUsers());
    }

    private static void HandleShowSpecificCommand(DatabaseService databaseService, string cardId)
    {
        UserDetailedData? selectedUser = databaseService.GetUserById(cardId);

        if (selectedUser != null)
        {
            dialogs.ShowUser(selectedUser);
        }
        else
        {
            Console.WriteLine("user not found in database\n");
        }
    }

    private static void HandleAddSpecificCommand(DatabaseService databaseService, string cardId)
    {
        if (databaseService.UserExists(cardId))
        {
            Console.WriteLine("card id already exists!\n");
        }
        else
        {
            UserDetailedData? newUser = dialogs.MakeUser(cardId);

            if (newUser != null)
            {
                databaseService.AddUser(newUser);
            }
        }
    }

    private static void HandleEditSpecificCommand(DatabaseService databaseService, string cardId)
    {
        UserDetailedData? selectedUser = databaseService.GetUserById(cardId);
        if (selectedUser == null)
        {
            Console.WriteLine("user not found in database\n");
        }
        else
        {
            UserDetailedData newUser = dialogs.EditUser(selectedUser);
            if (!databaseService.EditUser(cardId, newUser))
            {
                Console.WriteLine("could not perform operation in database");
            }
        }
    }

    private static void HandleRemoveSpecificCommand(DatabaseService databaseService, string cardId)
    {
        UserDetailedData? userToRemove = databaseService.GetUserById(cardId);

        if (userToRemove == null || userToRemove.CardID == null)
        {
            Console.WriteLine("user not found in database\n");
        }
        else if (dialogs.DeleteUserConfirmation(userToRemove))
        {
            databaseService.RemoveUser(userToRemove.CardID);
            Console.WriteLine($"removed user\n");
        }
    }

    private static void HandleAccessLogDetailsCommand(DatabaseService databaseService)
    {
        Console.WriteLine("start date");
        DateTime start = dialogs.GetDateTime(0, 9999);
        Console.WriteLine("\nend date");
        DateTime end = dialogs.GetDateTime(0, 9999);

        if (start > end)
        {
            Console.WriteLine("start date cannot be after end date\n");
            return;
        }

        List<AccessLogData> logs = databaseService.GetAccessLogs(start, end);

        dialogs.ShowAccessLogs(logs);
    }

    private static void HandleAlarmLogDetailsCommand(DatabaseService databaseService)
    {
        Console.WriteLine("start date");
        DateTime start = dialogs.GetDateTime(0, 9999);
        Console.WriteLine("\nend date");
        DateTime end = dialogs.GetDateTime(0, 9999);

        if (start > end)
        {
            Console.WriteLine("start date cannot be after end date\n");
            return;
        }

        List<AlarmLogData> logs = databaseService.GetAlarmLogs(start, end);

        dialogs.ShowAlarmLogs(logs);
    }

    private static void HandleDoorLogDetailsCommand(DatabaseService databaseService, string command)
    {
        string? number = GetNumbersFromCommand(command);

        if (number == null)
        {
            Console.WriteLine("user not found in database\n");
            return;
        }
        int doorNumber = Convert.ToInt32(number);

        Console.WriteLine("start date");
        DateTime start = dialogs.GetDateTime(0, 9999);
        Console.WriteLine("\nend date");
        DateTime end = dialogs.GetDateTime(0, 9999);

        if (start > end)
        {
            Console.WriteLine("start date cannot be after end date\n");
            return;
        }

        List<AccessLogData> logs = databaseService.GetDoorLogs(start, end, doorNumber);

        dialogs.ShowAccessLogs(logs);
    }

    private static void HandleSuspiciousCommand(DatabaseService databaseService)
    {
        List<string> suspiciousUserIds = databaseService.GetSuspiciousUserIds();

        dialogs.ShowSuspiciousUserIds(suspiciousUserIds);
    }

    // Helper methods
    private static string? GetNumbersFromCommand(string command)
    {
        string pattern = @"\b\w+ (\d{1,4})\b";

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