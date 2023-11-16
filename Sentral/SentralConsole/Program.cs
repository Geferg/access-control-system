using System.Text.RegularExpressions;

namespace SentralConsole;

internal class Program
{
    private const string removePattern = @"^remove \d{4}$";
    private const string editPattern = @"^edit \d{4}$";
    private const string showPattern = @"^show \d{4}$";


    static void Main(string[] args)
    {
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
                ShowSpecificUser();
            }
            else if (Regex.IsMatch(command, editPattern))
            {
                EditSpecificUser();
            }
            else if (Regex.IsMatch(command, removePattern))
            {
                RemoveSpecificUser();
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

        Console.Write("> ");
        char response = Console.ReadKey(true).KeyChar;

        while (response != 'y' && response != 'n')
        {
            response = Console.ReadKey(true).KeyChar;
        }

        if (response == 'y')
        {
            Console.Write("y");
            Console.WriteLine("exiting...");
            // Add propper shutdown
            Environment.Exit(0);
        }
        Console.Write("n");
        Console.WriteLine("\ncanceled");

        Console.WriteLine("");
    }

    private static void ShowUsers()
    {
        Console.WriteLine("showing users...");

        // Logic

        Console.WriteLine("");
    }

    private static void AddUser()
    {
        Console.WriteLine("adding user...");

        // Logic

        Console.WriteLine("");
    }




    private static void ShowSpecificUser()
    {
        Console.WriteLine("showing user...");

        // Logic

        Console.WriteLine("");
    }

    private static void EditSpecificUser()
    {
        Console.WriteLine("editing user...");

        // Logic

        Console.WriteLine("");
    }

    private static void RemoveSpecificUser()
    {
        Console.WriteLine("removing user...");

        // Logic

        Console.WriteLine("");
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
