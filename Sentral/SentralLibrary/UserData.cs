using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UserData
{
    private readonly Random random = new Random();

    public string FirstName { get; set; }
    public string LastName { get; set; }
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
