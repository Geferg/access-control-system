using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary.DataClasses;
public class UserDetailedData
{
    private readonly Random random = new();

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
    public string? Email { get; set; }
    public string? CardID { get; set; }
    public string? CardPin { get; set; }
    public DateTime StartValidityTime { get; set; }
    public DateTime EndValidityTime { get; set; }

    private string? firstName;
    private string? lastName;

    public UserDetailedData()
    {
        CardPin = GeneratePin();
        StartValidityTime = DateTime.MinValue;
        EndValidityTime = DateTime.MinValue;
    }

    private string GeneratePin()
    {
        int number = random.Next(0, 10000);
        return number.ToString("D4");
    }
}
