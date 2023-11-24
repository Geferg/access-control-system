using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SentralLibrary.Database;
using SentralLibrary.Database.DataClasses;
using SentralLibrary.Tcp;
using SentralLibrary.Tcp.TcpRequests;

namespace SentralLibrary.Console;
public class ConsoleDialogs
{
    private readonly ConsoleConnectionManager connection;

    public ConsoleDialogs(ConsoleConnectionManager connection)
    {
        this.connection = connection;
    }

    // SERVE UI
    public void ListCommands()
    {
        WriteLine("Available Commands");
        WriteLine("  clear        - Clears the console screen.");
        WriteLine("  database     - Displays the current status of the database.");
        WriteLine("  system       - Displays the status of card readers in the system.");
        WriteLine("  show         - Lists all registered users.");
        WriteLine("  add [ID]     - Adds a new user with the specified card ID.");
        WriteLine("  show [ID]    - Displays details about a specific user by card ID.");
        WriteLine("  edit [ID]    - Modifies data for an existing user by card ID.");
        WriteLine("  remove [ID]  - Removes an existing user by card ID.");
        WriteLine("  access       - Lists all access attempts within a specified date range.");
        WriteLine("  suspicious   - Lists all users with 10 or more unsuccessful access attempts.");
        WriteLine("  alarm        - Displays all alarms within a specified time range.");
        WriteLine("  alarm [Door] - Shows alarms for a specific door within a specified time range.");
        WriteLine("");
    }

    public void ShowTcpStatus(TcpConnectionManager tcpConnection)
    {
        WriteLine("TCP System Details");
        WriteLine(new string('-', 50));
        WriteLine($"  Authorized Connections   : {tcpConnection.GetAuthorizedClientCount()}");
        WriteLine($"  Unauthorized Connections : {tcpConnection.GetUnauthorizedClientCount()}");

        foreach (var id in tcpConnection.GetClientIds())
        {
            WriteLine($"  Connected Client ID      : {id}");
        }

        WriteLine(new string('-', 50));
        WriteLine("");
    }

    public void ShowDatabaseStatus(DatabaseConnectionManager databaseConnection)
    {
        if (databaseConnection.TestConnection())
        {
            WriteLine("Connection to Database SUCCESSFUL");
        }
        else
        {
            WriteLine("Connection to Database FAILED");
        }

        WriteLine(new string('-', 50));
        WriteLine($"  Database Name : {databaseConnection.DatabaseName}");
        WriteLine($"  IP Address    : {databaseConnection.HostIp}");
        WriteLine($"  Port          : {databaseConnection.HostPort}");
        WriteLine(new string('-', 50));
        WriteLine("");
    }

    public void ShowUsers(List<UserSimpleData> users)
    {
        WriteLine("User List:");
        WriteLine(new string('-', 50));
        foreach (var user in users.OrderBy(u => u.CardID).ToList())
        {
            WriteLine($"  ID: {user.CardID} - {user.FirstName} {user.LastName}");
        }
        WriteLine(new string('-', 50));
        WriteLine("");

    }

    public UserDetailedData? MakeUser(string cardId)
    {
        WriteLine("Adding New User - Please Fill in the Data Below");
        WriteLine(new string('-', 50));

        string firstName = GetNameInput("First Name");
        string lastName = GetNameInput("Last Name");
        string email = GetEmailInput("Email");

        UserDetailedData newUser = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            CardID = cardId
        };

        WriteLine($"New User Creation");
        WriteLine(new string('-', 50));
        WriteLine($"  First Name      : {newUser.FirstName}");
        WriteLine($"  Last Name       : {newUser.LastName}");
        WriteLine($"  Email           : {newUser.Email}");
        WriteLine($"  Card ID         : {newUser.CardID}");
        WriteLine($"  Validity Period : {newUser.StartValidityTime} - {newUser.EndValidityTime}");
        WriteLine($"  Card PIN        : {newUser.CardPin}");
        WriteLine(new string('-', 50));
        WriteLine("");

        if (!UserConfirm("confirm addition of user?"))
        {
            return null;
        }

        return newUser;
    }

    public void ShowUser(UserDetailedData user)
    {
        WriteLine($"User Details for {user.FirstName} {user.LastName}");
        WriteLine(new string('-', 50));
        WriteLine($"  First Name      : {user.FirstName}");
        WriteLine($"  Last Name       : {user.LastName}");
        WriteLine($"  Email           : {user.Email}");
        WriteLine($"  Card ID         : {user.CardID}");
        WriteLine($"  Validity Period : {user.StartValidityTime} - {user.EndValidityTime}");
        WriteLine($"  Card PIN        : {user.CardPin}");
        WriteLine(new string('-', 50));
        WriteLine("");
    }

    public bool DeleteUserConfirmation(UserDetailedData user)
    {
        return UserConfirm($"are you sure you want to delete [{user.CardID}] {user.FirstName} {user.LastName}");
    }

    public UserDetailedData EditUser(UserDetailedData user)
    {
        WriteLine($"Editing User - Card ID: {user.CardID}");
        WriteLine(new string('-', 50));

        WriteLine($"1. First Name       : {user.FirstName}");
        WriteLine($"2. Last Name        : {user.LastName}");
        WriteLine($"3. Email            : {user.Email}");
        WriteLine($"4. Validity Start   : {user.StartValidityTime}");
        WriteLine($"5. Validity End     : {user.EndValidityTime}");
        WriteLine($"6. Card PIN         : {user.CardPin}");
        WriteLine("0. Cancel");

        WriteLine(new string('-', 50));
        WriteLine("Please press a number key to select an option:");
        Write("> ");

        bool ongoingDialog = true;

        while (ongoingDialog)
        {
            char responseKeyChar = ReadKey().KeyChar;

            switch (responseKeyChar)
            {
                case '1':
                    WriteLine("1");
                    string newFirstName = GetNameInput("new first name");
                    user.FirstName = newFirstName;
                    ongoingDialog = false;
                    break;

                case '2':
                    WriteLine("2");
                    string newLastName = GetNameInput("new last name");
                    user.LastName = newLastName;
                    ongoingDialog = false;
                    break;

                case '3':
                    WriteLine("3");
                    string newEmail = GetEmailInput("new email");
                    user.Email = newEmail;
                    ongoingDialog = false;
                    break;

                case '4':
                    WriteLine("4");
                    DateTime newStartTime = GetDateTime(1900, 2100);

                    user.StartValidityTime = newStartTime;
                    ongoingDialog = false;
                    break;

                case '5':
                    WriteLine("5");
                    DateTime newEndTime = GetDateTime(1900, 2100);

                    user.EndValidityTime = newEndTime;
                    ongoingDialog = false;
                    break;

                case '6':
                    WriteLine("6");
                    string newCardPin = GetFourDigitInput("new pin");
                    user.CardPin = newCardPin;
                    ongoingDialog = false;
                    break;

                case '0':
                    WriteLine("0");
                    ongoingDialog = false;
                    break;
            }
        }

        return user;
    }

    public void ShowSuspiciousUserIds(List<string> ids)
    {
        ids.Sort();

        if (ids.Count == 0)
        {
            WriteLine("No suspicious users found.");
            return;
        }

        WriteLine("Suspicious Users:");
        WriteLine(new string('-', 50)); // Separator for clarity

        foreach (var id in ids)
        {
            WriteLine($"  User ID: [{id}]");
        }

        WriteLine(new string('-', 50));
        WriteLine("");
    }

    public void ShowAccessLogs(List<AccessLogData> logs)
    {
        logs = logs.OrderBy(l => l.Time).ToList();

        if (logs.Count == 0)
        {
            WriteLine("No access logs found.");
            return;
        }

        WriteLine("Access Logs:");
        WriteLine(new string('-', 50));

        foreach (var log in logs)
        {
            string accessStatus = log.AccessGranted ? "Approved" : "Not Approved";
            string dateTimeFormatted = log.Time.ToString("yyyy-MM-dd HH:mm:ss");

            WriteLine($"[{dateTimeFormatted}] Card ID {log.CardId} attempted access at Door {log.DoorNumber}: {accessStatus}");
        }

        WriteLine(new string('-', 50));
        WriteLine("");
    }

    public void ShowAlarmLogs(List<AlarmLogData> logs)
    {
        logs = logs.OrderBy(l => l.Time).ToList();

        if (logs.Count == 0)
        {
            WriteLine("No alarm logs found.");
            return;
        }

        WriteLine("Alarm Logs:");
        WriteLine(new string('-', 50));

        foreach (var log in logs)
        {
            string dateTimeFormatted = log.Time.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLine($"[{dateTimeFormatted}] Alarm: {log.AlarmType} at Door {log.DoorNumber}");
        }

        WriteLine(new string('-', 50));
        WriteLine("");
    }

    // SPECIAL
    private bool UserConfirm(string message = "confirm")
    {
        WriteLine($"{message} (Y/N)");

        Write("> ");
        char response = char.ToLowerInvariant(ReadKey().KeyChar);
        WriteLine("");

        while (response != 'y' && response != 'n')
        {
            Write("Please enter 'Y' for Yes or 'N' for No: ");
            response = char.ToLowerInvariant(ReadKey().KeyChar);
            WriteLine("");
        }

        if (response == 'y')
        {
            WriteLine("Confirmation received.");
            return true;
        }

        WriteLine("Operation canceled.");

        WriteLine("");
        return false;
    }

    // BOTHER UI
    private string ReadLine()
    {
        string? line = connection.GetStringFromUI();
        return line ?? "";
    }

    private ConsoleKeyInfo ReadKey()
    {
        ConsoleKeyInfo? keyInfo = connection.GetKeyFromUI();
        if (keyInfo == null)
        {
            return new ConsoleKeyInfo();
        }
        return keyInfo.Value;
    }

    private void WriteLine(string message)
    {
        connection.PutOnUI(message + "\n");
    }

    private void Write(string message)
    {
        connection.PutOnUI(message);
    }

    // INPUT JANITORS
    public DateTime GetDateTime(int minYear, int maxYear)
    {
        int year = GetValidatedNumberInput("Year (YYYY)", minYear, maxYear);
        int month = GetValidatedNumberInput("Month (MM)", 1, 12);
        int day = GetValidatedNumberInput("Day (DD)", 1, DateTime.DaysInMonth(year, month));
        int hour = GetValidatedNumberInput("Time (Hour, 0-23)", 0, 23);
        DateTime newStartTime = new(year, month, day, hour, 0, 0);

        return newStartTime;
    }

    private int GetValidatedNumberInput(string prompt, int minValue, int maxValue)
    {
        string input = "";
        InputValidation validation = InputValidation.NotValidated;

        while (validation != InputValidation.Valid)
        {
            Write($"> {prompt}: ");
            input = ReadLine();
            validation = ValidateNumberInput(input, minValue, maxValue);

            switch (validation)
            {
                case InputValidation.IsEmpty:
                    WriteLine($"Error: Input cannot be empty. Please try again.\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine($"Error: Input must be a number. Please try again.\n");
                    break;

                case InputValidation.OutsideRange:
                    WriteLine($"Error: Input must be between {minValue} and {maxValue}. Please try again.\n");
                    break;

                case InputValidation.IncorrectLength:
                    WriteLine($"Error: Input has incorrect length. Please try again.\n");
                    break;
            }
        }

        return int.Parse(input);
    }

    private string GetNameInput(string prompt)
    {
        string input = "";
        InputValidation validation = InputValidation.NotValidated;

        while (validation != InputValidation.Valid)
        {
            Write($"> {prompt}: ");
            input = ReadLine();

            validation = ValidateName(input);

            switch (validation)
            {
                case InputValidation.IsEmpty:
                    WriteLine("Error: Name cannot be empty. Please try again.\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("Error: Name can only contain letters. Please try again.\n");
                    break;
            }
        }

        return input;
    }

    private string GetFourDigitInput(string prompt)
    {
        string input = "";
        InputValidation validation = InputValidation.NotValidated;

        while (validation != InputValidation.Valid)
        {
            Write($"> {prompt}: ");
            input = ReadLine();

            validation = ValidateFourDigits(input);

            switch (validation)
            {
                case InputValidation.IsEmpty:
                    WriteLine("Error: Card ID cannot be empty. Please try again.\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("Error: Card ID must be a number. Please try again.\n");
                    break;

                case InputValidation.OutsideRange:
                    WriteLine("Error: Card ID must be between 0000 and 9999. Please try again.\n");
                    break;

                case InputValidation.IncorrectLength:
                    WriteLine("Error: Card ID must have a length of 4 digits. Please try again.\n");
                    break;
            }
        }

        return input;
    }

    private string GetEmailInput(string prompt)
    {
        string input = "";
        InputValidation validation = InputValidation.NotValidated;

        while (validation != InputValidation.Valid)
        {
            Write($"> {prompt}: ");
            input = ReadLine();

            validation = ValidateEmail(input);

            switch (validation)
            {
                case InputValidation.IsEmpty:
                    WriteLine("Error: Email cannot be empty. Please try again.\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("Error: Invalid email format. Please try again.\n");
                    break;
            }
        }

        return input;
    }

    // VALIDATORS
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
