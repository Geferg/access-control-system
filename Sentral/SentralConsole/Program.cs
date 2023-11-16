using System.Net.Mail;
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
                Console.WriteLine("command not recognized\n");
            }
        }
    }

    private static void Exit()
    {
        if (!UserConfirm("are you sure you want to exit?"))
        {
            Console.WriteLine("cancled\n");
            return;
        }
        Console.WriteLine("exiting...");
        // Add propper shutdown
        Environment.Exit(0);
    }

    private static void ShowUsers()
    {
        foreach(UserData user in mockDB)
        {
            Console.WriteLine($"[{user.CardID}] {user.FirstName} {user.LastName}");
        }

        Console.WriteLine("");
    }

    private static void AddUser()
    {
        Console.WriteLine("adding user, fill in data below\n");

        string? cardIdInput = "";
        string? firstNameInput = "";
        string? lastNameInput = "";
        string? emailInput = "";

        InputValidation idValidation = InputValidation.NotValidated;
        InputValidation firstNameValidation = InputValidation.NotValidated;
        InputValidation lastNameValidation = InputValidation.NotValidated;
        InputValidation emailValidation = InputValidation.NotValidated;

        // Card id
        while (idValidation != InputValidation.Valid)
        {
            Console.Write("> card id: ");
            cardIdInput = Console.ReadLine();

            idValidation = ValidateFourDigits(cardIdInput);

            switch (idValidation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("card id cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("card id must be a number\n");
                    break;

                case InputValidation.OutsideRange:
                    Console.WriteLine("card id must be between 0000 and 9999\n");
                    break;

                case InputValidation.IncorrectLength:
                    Console.WriteLine("card id must be of length 4\n");
                    break;
            }
        }

        // Name
        while (firstNameValidation != InputValidation.Valid)
        {
            Console.Write("> first name: ");
            firstNameInput = Console.ReadLine();

            firstNameValidation = ValidateName(firstNameInput);

            switch (firstNameValidation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("name cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("name can only contain letters\n");
                    break;
            }
        }

        while (lastNameValidation != InputValidation.Valid)
        {
            Console.Write("> last name: ");
            lastNameInput = Console.ReadLine();

            lastNameValidation = ValidateName(lastNameInput);

            switch (lastNameValidation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("name cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("name can only contain letters\n");
                    break;
            }
        }

        // Email
        while (emailValidation != InputValidation.Valid)
        {
            Console.Write("> email: ");
            emailInput = Console.ReadLine();

            emailValidation = ValidateEmail(emailInput);

            switch (emailValidation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("email cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("invalid email\n");
                    break;
            }
        }

        // Possible unsafe non-null assertion, but should be fine
        UserData newUser = new()
        {
            FirstName = firstNameInput!,
            LastName = lastNameInput!,
            Email = emailInput!,
            CardID = cardIdInput!
        };

        if(!UserConfirm("confirm addition of user?"))
        {
            Console.WriteLine("cancled\n");
            return;
        }

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

        if (!UserExists(cardID))
        {
            Console.WriteLine($"card id [{cardID}] does not exist\n");
            return;
        }

        UserData userToRemove = mockDB.First(x => x.CardID == cardID);

        if (!UserConfirm($"are you sure you want to delete [{cardID}] {userToRemove.FirstName} {userToRemove.LastName}"))
        {
            return;
        }

        //TODO change to DB connection class
        mockDB.Remove(userToRemove);
        Console.WriteLine($"removed user [{cardID}] {userToRemove.FirstName} {userToRemove.LastName}\n");
    }


    // Helper methods
    private static InputValidation ValidateName(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return InputValidation.IsEmpty;
        }
        else if (!input.All(char.IsLetter))
        {
            return InputValidation.IncorrectFormat;
        }

        return InputValidation.Valid;
    }

    private static InputValidation ValidateEmail(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return InputValidation.IsEmpty;
        }
        else if (!MailAddress.TryCreate(input, out _))
        {
            return InputValidation.IncorrectFormat;
        }

        return InputValidation.Valid;
    }

    private static InputValidation ValidateFourDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return InputValidation.IsEmpty;
        }
        else if (!int.TryParse(input, out int number))
        {
            return InputValidation.IncorrectFormat;
        }
        else if (number < 0 || 9999 < number)
        {
            return InputValidation.OutsideRange;
        }
        else if (input.Length != 4)
        {
            return InputValidation.IncorrectLength;
        }

        return InputValidation.Valid;
    }

    private static bool UserExists(string cardID)
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
        Console.WriteLine($"{message} (y/n)\n");

        Console.Write("> ");
        char response = Console.ReadKey(true).KeyChar;

        while (response != 'y' && response != 'n')
        {
            response = Console.ReadKey(true).KeyChar;
        }

        if (response == 'y')
        {
            Console.Write("y");
            Console.WriteLine("");
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
        Console.WriteLine("clear - clear the console");
        Console.WriteLine("exit - close the program");
        Console.WriteLine("add - add new user");
        Console.WriteLine("show - lists all users");
        Console.WriteLine("show [card ID] - details about a specific user");
        Console.WriteLine("edit [card ID]- change data of existing user");
        Console.WriteLine("remove [card ID] - remove existing user");
        Console.WriteLine("");
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
