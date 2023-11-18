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

    private static readonly DatabaseConnection dbConnection = new(dbIP, dbPort, dbUsername, dbPassword, dbDatabase);
    private static readonly TcpServer tcpServer = new(8000);
    private static UILogger uiLogger = new UILogger();

    private static List<UserData> mockDB = new();

    static void Main(string[] args)
    {
        dbConnection.AttachLogger(uiLogger);
        tcpServer.AttachLogger(uiLogger);

        uiLogger.LogMessageEvent += OnLogMessageReceived;

        // Adding test users
        UserData kristian = new()
        {
            FirstName = "Kristian",
            LastName = "Klette",
            Email = "geferg.dev@gmail.com",
            CardID = "1234",
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

        UserData testUser = new("test", "user", "test@user.com", "1234", "5050");

        //TODO replace with real db
        dbConnection.AddUser(testUser);

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
            else if (Regex.IsMatch(command, addPattern))
            {
                string? cardID = GetNumbersFromCommand(command);
                if(!string.IsNullOrEmpty(cardID))
                {
                    AddSpecificUser(cardID);
                }
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
        foreach (var user in dbConnection.GetUserbase().OrderBy(u => u.id))
        {
            Console.WriteLine($"[{user.id}] {user.firstName} {user.lastName}");
        }
        /*
        foreach(UserData user in mockDB.OrderBy(u => u.CardID))
        {
            Console.WriteLine($"[{user.CardID}] {user.FirstName} {user.LastName}");
        }
        */
        Console.WriteLine("");
    }

    private static void AddSpecificUser(string cardID)
    {
        /*
        if (mockDB.Any(u => u.CardID == cardID))
        {
            Console.WriteLine("card id already exists!\n");
            return;
        }
        */
        if (dbConnection.UserExists(cardID))
        {
            Console.WriteLine("card id already exists!\n");
            return;
        }

        Console.WriteLine("adding user, fill in data below\n");

        string firstNameInput = GetNameInput("first name");
        string lastNameInput = GetNameInput("last name");
        string emailInput = GetEmailInput("email");

        //TODO verify card id does not exist in db

        UserData newUser = new()
        {
            FirstName = firstNameInput,
            LastName = lastNameInput,
            Email = emailInput,
            CardID = cardID
        };

        Console.WriteLine("");
        Console.WriteLine($"name: {newUser.FirstName} {newUser.LastName}");
        Console.WriteLine($"email: {newUser.Email}");
        Console.WriteLine($"card id: {newUser.CardID}");
        Console.WriteLine($"card pin: {newUser.CardPin}");
        Console.WriteLine($"validity period: {newUser.GetFormattedPeriod()}\n");

        if (!UserConfirm("confirm addition of user?"))
        {
            return;
        }

        dbConnection.AddUser(newUser);
        //mockDB.Add(newUser);
    }

    private static void ShowSpecificUser(string cardID)
    {
        if (!dbConnection.UserExists(cardID))
        {
            Console.WriteLine($"Card id [{cardID}] does not exist\n");
            //return;
        }
        else
        {
            Console.WriteLine($"Card id [{cardID}] does exist\n");
        }

        //TODO link with database handler class
        //UserData selectedUser = mockDB.First(x => x.CardID == cardID);
        UserData? selectedUser = dbConnection.GetUser(cardID);

        if (selectedUser == null)
        {
            Console.WriteLine("user does not exist");
            return;
        }

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
        //UserData selectedUser = mockDB.First(x => x.CardID == cardID);
        UserData? selectedUser = dbConnection.GetUser(cardID);

        if (selectedUser == null)
        {
            Console.WriteLine(missingInDbMessage);
            return;
        }

        Console.WriteLine($"editing user {cardID}...");

        Console.WriteLine($"1. first name: {selectedUser.FirstName}");
        Console.WriteLine($"2. last name: {selectedUser.LastName}");
        Console.WriteLine($"3. email: {selectedUser.Email}");
        Console.WriteLine($"4. card id: {selectedUser.CardID}");
        Console.WriteLine($"5. validity period: {selectedUser.GetFormattedPeriod()}");
        Console.WriteLine($"6. card pin: {selectedUser.CardPin}");
        Console.WriteLine($"7. cancel");

        Console.WriteLine("press a number 1-7\n");
        Console.Write("> ");

        bool dialog = true;

        while (dialog)
        {
            char response = Console.ReadKey(true).KeyChar;

            switch (response)
            {
                case '1':
                    Console.WriteLine("1");
                    string newFirstName = GetNameInput("new first name");
                    selectedUser.FirstName = newFirstName;
                    dialog = false;
                    break;

                case '2':
                    Console.WriteLine("2");
                    string newLastName = GetNameInput("new last name");
                    selectedUser.LastName = newLastName;
                    dialog = false;
                    break;

                case '3':
                    Console.WriteLine("3");
                    string newEmail = GetEmailInput("new email");
                    selectedUser.Email = newEmail;
                    dialog = false;
                    break;

                case '4':
                    Console.WriteLine("4");
                    string newCardId = GetFourDigitInput("new card id");
                    //TODO user is stuck if they dont want to change, can ignore but trash
                    if(mockDB.Any(u => u.CardID == newCardId && selectedUser.CardID != newCardId))
                    {
                        Console.WriteLine("card id already exists!\n");
                    }
                    else
                    {
                        selectedUser.CardID = newCardId;
                    }
                    dialog = false;
                    break;

                case '5':
                    Console.WriteLine("5");
                    //TODO add this
                    Console.WriteLine("NOT IMPLEMENTED\n");
                    dialog = false;
                    break;

                case '6':
                    Console.WriteLine("6");
                    string newCardPin = GetFourDigitInput("new pin");
                    selectedUser.CardPin = newCardPin;
                    dialog = false;
                    break;

                case '7':
                    Console.WriteLine("7");
                    dialog = false;
                    break;
            }
        }
    }

    private static void RemoveSpecificUser(string cardID)
    {
        Console.WriteLine($"removing user {cardID}...");

        if (!UserExists(cardID))
        {
            Console.WriteLine($"card id [{cardID}] does not exist\n");
            return;
        }

        //UserData userToRemove = mockDB.First(x => x.CardID == cardID);
        UserData? userToRemove = dbConnection.GetUser(cardID);

        if (userToRemove == null)
        {
            Console.WriteLine(missingInDbMessage);
            return;
        }

        if (!UserConfirm($"are you sure you want to delete [{cardID}] {userToRemove.FirstName} {userToRemove.LastName}"))
        {
            return;
        }

        //TODO change to DB connection class
        dbConnection.RemoveUser(userToRemove.CardID);
        //mockDB.Remove(userToRemove);
        Console.WriteLine($"removed user [{cardID}] {userToRemove.FirstName} {userToRemove.LastName}\n");
    }

    private static string GetEmailInput(string prompt)
    {
        string? emailInput = "";
        InputValidation emailValidation = InputValidation.NotValidated;

        while (emailValidation != InputValidation.Valid)
        {
            Console.Write($"> {prompt}: ");
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

        //TODO trim
        return emailInput!;
    }

    private static string GetFourDigitInput(string prompt)
    {
        string? cardIdInput = "";
        InputValidation idValidation = InputValidation.NotValidated;

        while (idValidation != InputValidation.Valid)
        {
            Console.Write($"> {prompt}: ");
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

        return cardIdInput!;
    }

    private static string GetNameInput(string prompt)
    {
        string? nameInput = "";
        InputValidation nameValidation = InputValidation.NotValidated;

        while (nameValidation != InputValidation.Valid)
        {
            Console.Write($"> {prompt}: ");
            nameInput = Console.ReadLine();

            nameValidation = ValidateName(nameInput);

            switch (nameValidation)
            {
                case InputValidation.IsEmpty:
                    Console.WriteLine("name cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    Console.WriteLine("name can only contain letters\n");
                    break;
            }
        }

        return nameInput!;
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
        Console.WriteLine("show - lists all users");
        Console.WriteLine("add [card id] - add new user with a card id");
        Console.WriteLine("show [card id] - details about a specific user");
        Console.WriteLine("edit [card id]- change data of existing user");
        Console.WriteLine("remove [card id] - remove existing user");
        Console.WriteLine("");
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
