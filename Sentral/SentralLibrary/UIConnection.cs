using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UIConnection
{
    public event Action<string>? ClassToUI;
    public event Func<string?>? UIStringToClass;
    public event Func<ConsoleKeyInfo?>? UIKeyToClass;

    public void PutOnUI(string message)
    {
        ClassToUI?.Invoke(message);
    }

    public string? GetStringFromUI()
    {
        return UIStringToClass?.Invoke();
    }

    public ConsoleKeyInfo? GetKeyFromUI()
    {
        return UIKeyToClass?.Invoke();
    }
}
