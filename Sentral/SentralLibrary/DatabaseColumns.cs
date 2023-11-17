using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
internal static class DatabaseColumns
{
    public const string TABLE_NAME = "userdata";
    public const string FirstNameCol = "fornavn";
    public const string LastNameCol = "etternavn";
    public const string EmailCol = "email";
    public const string IdCol = "kortid";
    public const string PinCol = "pinkode";
    public const string ValidStartCol = "?";
    public const string ValidEndCol = "?";
}