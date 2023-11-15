using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
internal class UserData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int CardID { get; private set; }
    private int cardPin;
    public (DateTime start, DateTime end) ValidityPeriod { get; private set; }

    public UserData(string firstName, string lastName, string email, int cardID, int cardPin, DateTime startTime, DateTime endTime)
    {

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CardID = cardID;
        this.cardPin = cardPin;
        ValidityPeriod = (startTime, endTime);
    }

    public bool VerifyUser(int cardID, int cardPin)
    {
        //TODO check requirements on this
        return cardID == CardID && cardPin == this.cardPin;
    }
}
