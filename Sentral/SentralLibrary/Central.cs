using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class Central
{
    private List<CardReader> cardReaders;
    private List<UserData> userData;

    public Central()
    {
        cardReaders = new List<CardReader>();
        userData = new List<UserData>();
    }

    public void ValidateCard(int cardID, int cardPin)
    {
        bool result = userData.First(user => user.CardID == cardID).VerifyUser(cardID, cardPin);
    }


    //TODO hook up database

    //TODO hook up card reader access requests

    //TODO report generator

    //TODO make admin methods/fields

    //TODO make alarm triggers (event)
}
