using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UserData
{
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
        CardPin = "";
        ValidityPeriod = (DateTime.MinValue, DateTime.MaxValue);
    }

    public string GetFormattedPeriod()
    {
        return $"{ValidityPeriod.start} - {ValidityPeriod.end}";
    }

    public void UpdateCardID(string id)
    {
        //TODO check format
        CardID = id;
    }


    public bool VerifyUser(string cardID, string cardPin)
    {
        //TODO check requirements on this
        return cardID == CardID && cardPin == this.CardPin;
    }
}
