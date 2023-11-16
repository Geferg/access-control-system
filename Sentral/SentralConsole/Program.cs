using System.Text.RegularExpressions;
using SentralLibrary;

namespace SentralConsole;

internal class Program
{
    private const string removePattern = @"^remove \d{4}$";
    private const string editPattern = @"^edit \d{4}$";
    private const string showPattern = @"^show \d{4}$";
    private static List<UserData> mockDB = new();

    static void Main(string[] args)
    {
        // Adding test users
        UserData kristian = new()
        {
            FirstName = "Kristian",
            LastName = "Klette",
            Email = "geferg.dev@gmail.com",
            CardID = "0000",
            ValidityPeriod = (DateTime.Now, DateTime.Now.AddYears(1)),
            CardPin = "0000"
        };

        UserData ryan = new()
        {
            FirstName = "Ryan",
            LastName = "Le",
            Email = "ryan.le@gmail.com",
            CardID = "1111",
            ValidityPeriod = (DateTime.Now, DateTime.Now.AddYears(1)),
            CardPin = "0001"
        };

        UserData tor = new()
        {
            FirstName = "Tor",
            LastName = "Haldaas",
            Email = "tor.h@gmail.com",
            CardID = "2222",
            ValidityPeriod = (DateTime.Now, DateTime.Now.AddYears(1)),
            CardPin = "0010"
        };

        UserData victor = new()
        {
            FirstName = "Victor",
            LastName = "Newman",
            Email = "victor.new@gmail.com",
            CardID = "3333",
            ValidityPeriod = (DateTime.Now, DateTime.Now.AddYears(1)),
            CardPin = "0011"
        };

        mockDB.Add(kristian);
        mockDB.Add(ryan);
        mockDB.Add(tor);
        mockDB.Add(victor);

        ListCommands();

        while (true)
        {
            Console.Write("> ");
            string? command = Console.ReadLine();


            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("input is empty");
                ListCommands();
            }
            else if (command == "clear")
            {
                Console.Clear();
                ListCommands();
            }
            else if (command == "exit")
            {
                Exit();
            }
            else if (command == "show")
            {
                ShowUsers();
            }
            else if (command == "add")
            {
                AddUser();
            }
            else if (Regex.IsMatch(command, showPattern))
            {
                string? cardID = GetNumbersFromCommand(command);
                if (!string.IsNullOrEmpty(cardID))
                {
                    ShowSpecificUser(cardID);
                }
            }
            else if (Regex.IsMatch(command, editPattern))
            {
                string? cardID = GetNumbersFromCommand(command);
                if (!string.IsNullOrEmpty(cardID))
                {
                    EditSpecificUser(cardID);
                }
            }
            else if (Regex.IsMatch(command, removePattern))
            {
                string? cardID = GetNumbersFromCommand(command);
                if (!string.IsNullOrEmpty(cardID))
                {
                    RemoveSpecificUser(cardID);
                }
            }
            else
            {
                Console.WriteLine("command not recognized");
            }
        }
    }

    private static void Exit()
    {
        Console.WriteLine("are you sure you want to exit? (y/n)\n");

        bool confirm = UserConfirm();

        if (confirm)
        {
            Console.WriteLine("exiting...");
            // Add propper shutdown
            Environment.Exit(0);
        }
    }

    private static void ShowUsers()
    {
        Console.WriteLine("showing users...");

        // Logic

        Console.WriteLine("");
    }

    private static void AddUser()
    {
        Console.WriteLine("adding user, fill in data");
        Console.WriteLine("press esc to go back\n");

        UserData newUser = new();
        // generate card id
        // generate pin?
        // handle user input

        mockDB.Add(newUser);
    }

    private static void ShowSpecificUser(string cardID)
    {
        if (!UserExists(cardID))
        {
            Console.WriteLine($"Card id [{cardID}] does not exist\n");
            return;
        }

        //TODO link with database handler class
        UserData selectedUser = mockDB.First(x => x.CardID == cardID);

        Console.WriteLine($"     first name: {selectedUser.FirstName}");
        Console.WriteLine($"      last name: {selectedUser.LastName}");
        Console.WriteLine($"          email: {selectedUser.Email}");
        Console.WriteLine($"        card id: {selectedUser.CardID}");
        Console.WriteLine($"validity period: {selectedUser.GetFormattedPeriod()}");
        Console.WriteLine($"       card pin: {selectedUser.CardPin}");

        Console.WriteLine("");
    }

    private static void EditSpecificUser(string cardID)
    {
        Console.WriteLine($"editing user {cardID}...");



        Console.WriteLine("");
    }

    private static void RemoveSpecificUser(string cardID)
    {
        Console.WriteLine($"removing user {cardID}...");

        // Logic

        Console.WriteLine("");
    }


    // Helper methods

    public static bool UserExists(string cardID)
    {
        return mockDB.Any(x => x.CardID == cardID);
    }

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

    private static bool UserConfirm(string message = "confirm")
    {
        // add string message to be passed like "are you sure you want to exit?" then add (y/n)
        Console.Write("> ");
        char response = Console.ReadKey(true).KeyChar;

        while (response != 'y' && response != 'n')
        {
            response = Console.ReadKey(true).KeyChar;
        }

        if (response == 'y')
        {
            Console.Write("y");
            return true;
        }
        Console.Write("n");
        Console.WriteLine("\ncanceled");

        Console.WriteLine("");
        return false;
    }

    private static void ListCommands()
    {
        Console.WriteLine("commands:");
        Console.WriteLine("add - add new user");
        Console.WriteLine("remove [card ID] - remove existing user");
        Console.WriteLine("edit [card ID]- change data of existing user");
        Console.WriteLine("show - lists all users");
        Console.WriteLine("show [card ID] - details about a specific user");
        Console.WriteLine("exit - close the program");
        Console.WriteLine("");
    }
}
