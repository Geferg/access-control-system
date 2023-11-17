using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UILogger
{
    public event EventHandler<string>? LogMessageEvent;

    public void LogMessage(string message)
    {
        LogMessageEvent?.Invoke(this, message);
    }
}
