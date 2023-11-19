using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UserData
{
    private readonly Random random = new Random();

    private string firstName = string.Empty;
    public string FirstName
    {
        get => firstName;
        set
        {
            if (value.Length > 0)
            {
                firstName = char.ToUpper(value[0]) + value[1..].ToLower();
            }
        }
    }
    private string lastName = string.Empty;
    public string LastName
    {
        get => lastName;
        set
        {
            if (value.Length > 0)
            {
                lastName = char.ToUpper(value[0]) + value[1..].ToLower();
            }
        }
    }
    public string Email { get; set; }
    public string CardID { get; set; }
    public string CardPin { get; set; }
    public (DateTime start, DateTime end) ValidityPeriod { get; set; }

    public UserData(string firstName, string lastName, string email, string cardID, string cardPin, DateTime startTime, DateTime endTime)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CardID = cardID;
        CardPin = cardPin;
        ValidityPeriod = (startTime, endTime);
    }

    public UserData(string firstName, string lastName, string email, string cardID)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CardID = cardID;
        CardPin = GeneratePin();
        ValidityPeriod = (DateTime.MinValue, DateTime.MaxValue);
    }

    public UserData()
    {
        FirstName = "";
        LastName = "";
        Email = "";
        CardID = "";
        CardPin = GeneratePin();
        ValidityPeriod = (DateTime.MinValue, DateTime.MaxValue);
    }

    public string GetFormattedPeriod()
    {
        return $"{ValidityPeriod.start} - {ValidityPeriod.end}";
    }

    private string GeneratePin()
    {
        int number = random.Next(0, 10000);

        return number.ToString("D4");
    }
}
