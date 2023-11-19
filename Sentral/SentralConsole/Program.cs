using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SentralLibrary;

namespace SentralConsole;

internal class Program
{
    // INPUT PATTERN MATCHING
    private const string removePattern = @"^remove \d{4}$";
    private const string editPattern = @"^edit \d{4}$";
    private const string showPattern = @"^show \d{4}$";
    private const string addPattern = @"^add \d{4}";

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
    private static readonly TcpServer tcpServer = new(8000);
    private static readonly UIConnection uiConnection = new();
    private static readonly UIDialogs dialogs = new(uiConnection);

    static void Main(string[] args)
    {
        Console.WriteLine("\u001b]0;Sentral\u0007");

        uiConnection.ClassToUI += OnWriteToUI;
        uiConnection.UIStringToClass += OnRecieveFromUI;
        uiConnection.UIKeyToClass += OnGetKeypress;


        tcpServer.Start();
        dialogs.ListCommands();

        while (true)
        {
            Console.Write("> ");
            string? command = Console.ReadLine();


            if (string.IsNullOrEmpty(command))
            {

            }
            else if (command == "clear")
            {
                Console.Clear();
                dialogs.ListCommands();
            }
            else if (command == "exit")
            {
                if (dialogs.ExitProgramConfirmation())
                {
                    Environment.Exit(0);
                }
            }
            else if (command == "show")
            {
                dialogs.ShowUsers(dbConnection.GetUserbase().OrderBy(u => u.id).ToList());
            }
            else if (Regex.IsMatch(command, addPattern))
            {
                string cardID = GetNumbersFromCommand(command);

                if (dbConnection.UserExists(cardID))
                {
                    Console.WriteLine("card id already exists!\n");
                }
                else
                {
                    UserData? newUser = dialogs.MakeUser(cardID);

                    if (newUser != null)
                    {
                        dbConnection.AddUser(newUser);
                    }
                }
            }
            else if (Regex.IsMatch(command, showPattern))
            {
                string cardID = GetNumbersFromCommand(command);

                UserData? selectedUser = dbConnection.GetUser(cardID);

                if (selectedUser != null)
                {
                    dialogs.ShowUser(selectedUser);
                }
                else
                {
                    Console.WriteLine(missingInDbMessage + "\n");
                }
            }
            else if (Regex.IsMatch(command, editPattern))
            {
                string cardID = GetNumbersFromCommand(command);
                UserData? selectedUser = dbConnection.GetUser(cardID);
                if (selectedUser == null)
                {
                    Console.WriteLine(missingInDbMessage);
                }
                else
                {
                    UserData newUser = dialogs.EditUser(selectedUser);
                    if (!dbConnection.UpdateUser(cardID, newUser))
                    {
                        Console.WriteLine(failureInDbMessage);
                    }
                }
            }
            else if (Regex.IsMatch(command, removePattern))
            {
                string cardID = GetNumbersFromCommand(command);
                UserData? userToRemove = dbConnection.GetUser(cardID);

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
            else
            {
                Console.WriteLine("command not recognized\n");
            }
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

    // Helper methods
    private static string GetNumbersFromCommand(string command)
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
            throw new InvalidOperationException();
        }
    }

}
