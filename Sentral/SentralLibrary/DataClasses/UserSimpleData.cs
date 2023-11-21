using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.DataClasses;
public class UserSimpleData
{
    public string? CardID { get; set; }
    public string? FirstName
    {
        get => firstName;
        set
        {
            if (value?.Length > 0)
            {
                firstName = char.ToUpper(value[0]) + value[1..].ToLower();
            }
        }
    }
    public string? LastName
    {
        get => lastName;
        set
        {
            if (value?.Length > 0)
            {
                lastName = char.ToUpper(value[0]) + value[1..].ToLower();
            }
        }
    }

    private string? firstName;
    private string? lastName;
}
