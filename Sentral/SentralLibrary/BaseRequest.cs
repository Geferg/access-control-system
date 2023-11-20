using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public abstract class BaseRequest
{
    public string RequestType { get; set; } = string.Empty;
}
