using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SentralLibrary.DataClasses;

namespace SentralLibrary;
public class UIDialogs
{
    //TODO use data classes
    private UIConnection connection;

    public UIDialogs(UIConnection connection)
    {
        this.connection = connection;
    }

    // SERVE UI
    public void ShowUsers(List<(string id, string firstName, string lastName)> users)
    {
        foreach (var (id, firstName, lastName) in users)
        {
            WriteLine($"[{id}] {firstName} {lastName}");
        }
        WriteLine("");
    }
    public UserDetailedData? MakeUser(string cardId)
    {
        WriteLine("adding user, fill in data below\n");

        string firstName = GetNameInput("first name");
        string lastName = GetNameInput("last name");
        string email = GetEmailInput("email");

        UserDetailedData newUser = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };

        WriteLine("");
        WriteLine($"name: {newUser.FirstName} {newUser.LastName}");
        WriteLine($"email: {newUser.Email}");
        WriteLine($"card id: {newUser.CardID}");
        WriteLine($"card pin: {newUser.CardPin}");
        WriteLine($"validity period: {newUser.StartValidityTime} - {newUser.EndValidityTime}\n");

        if (!UserConfirm("confirm addition of user?"))
        {
            return null;
        }

        return newUser;
    }
    public void ShowUser(UserDetailedData user)
    {
        WriteLine($"     first name: {user.FirstName}");
        WriteLine($"      last name: {user.LastName}");
        WriteLine($"          email: {user.Email}");
        WriteLine($"        card id: {user.CardID}");
        WriteLine($"validity period: {user.StartValidityTime} - {user.EndValidityTime}");
        WriteLine($"       card pin: {user.CardPin}");

        WriteLine("");
    }
    public bool DeleteUserConfirmation(UserDetailedData user)
    {
        return UserConfirm($"are you sure you want to delete [{user.CardID}] {user.FirstName} {user.LastName}");
    }
    public bool ExitProgramConfirmation()
    {
        if (!UserConfirm("are you sure you want to exit?"))
        {
            return false;
        }
        WriteLine("exiting...");
        return true;
    }
    public void ListCommands()
    {
        WriteLine("commands:");
        WriteLine("clear - clear the console");
        WriteLine("exit - close the program");
        WriteLine("database - shows status of database");
        WriteLine("system - shows status of card readers");
        WriteLine("show - lists all users");
        WriteLine("add [card id] - add new user with a card id");
        WriteLine("show [card id] - details about a specific user");
        WriteLine("edit [card id]- change data of existing user");
        WriteLine("remove [card id] - remove existing user");
        WriteLine("accesslog - lists all access attempts between two dates");
        WriteLine("suspicious - lists all suspicious users...");
        WriteLine("door log...");
        WriteLine("alarm log...");
        WriteLine("");
    }
    public UserDetailedData EditUser(UserDetailedData user)
    {
        WriteLine($"editing user {user.CardID}...");

        WriteLine($"1. first name: {user.FirstName}");
        WriteLine($"2. last name: {user.LastName}");
        WriteLine($"3. email: {user.Email}");
        WriteLine($"4. validity start: {user.StartValidityTime}");
        WriteLine($"5. validity end: {user.EndValidityTime}");
        WriteLine($"6. card pin: {user.CardPin}");
        WriteLine($"0. cancel");

        WriteLine("press a number key\n");
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

                    user.EndValidityTime =  newEndTime;
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
    public void ShowAccessLogs(List<(string id, bool approved, DateTime time, int doorNumber)> logs)
    {
        if(logs.Count == 0)
        {
            WriteLine("no logs found");
            return;
        }
        WriteLine("access logs:");
        foreach (var (id, approved, time, doorNumber) in logs)
        {
            string approvedMessage = "not approved";
            if (approved)
            {
                approvedMessage = "approved";
            }
            WriteLine($"[{id}] was {approvedMessage} at door {doorNumber} {time}");
        }
        WriteLine("");
    }

    public void ShowAlarmLogs(List<(DateTime time, int doorNumber, string alarmType)> logs)
    {
        if (logs.Count == 0)
        {
            WriteLine("no logs found");
            return;
        }
        WriteLine("alarm logs:");
        foreach (var (time, doorNumber, alarmType) in logs)
        {
            WriteLine($"{alarmType} at door {doorNumber} {time}");
        }
        WriteLine("");
    }

    // SPECIAL
    private bool UserConfirm(string message = "confirm")
    {
        WriteLine($"{message} (y/n)");

        Write("> ");
        char response = ReadKey().KeyChar;

        while (response != 'y' && response != 'n')
        {
            response = ReadKey().KeyChar;
        }

        if (response == 'y')
        {
            Write("y");
            WriteLine("");
            return true;
        }
        Write("n");
        WriteLine("\ncanceled");

        WriteLine("");
        return false;
    }

    // BOTHER UI
    private string ReadLine()
    {
        string? line = connection.GetStringFromUI();
        if (line == null)
        {
            line = "";
        }
        return line;
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
        int year = GetValidatedNumberInput("year", minYear, maxYear);
        int month = GetValidatedNumberInput("month", 1, 12);
        int day = GetValidatedNumberInput("day", 1, DateTime.DaysInMonth(year, month));
        int hour = GetValidatedNumberInput("time (hour)", 0, 23);
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
                    WriteLine("Input cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("Input must be a number\n");
                    break;

                case InputValidation.OutsideRange:
                    WriteLine($"Input must be between {minValue} and {maxValue}\n");
                    break;

                case InputValidation.IncorrectLength:
                    WriteLine("Input has incorrect length\n");
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
                    WriteLine("name cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("name can only contain letters\n");
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
                    WriteLine("card id cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("card id must be a number\n");
                    break;

                case InputValidation.OutsideRange:
                    WriteLine("card id must be between 0000 and 9999\n");
                    break;

                case InputValidation.IncorrectLength:
                    WriteLine("card id must be of length 4\n");
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
                    WriteLine("email cannot be empty\n");
                    break;

                case InputValidation.IncorrectFormat:
                    WriteLine("invalid email\n");
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
