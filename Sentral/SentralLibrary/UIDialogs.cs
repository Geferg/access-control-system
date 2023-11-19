using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class UIDialogs
{
    private UIConnection connection;

    public UIDialogs(UIConnection connection)
    {
        this.connection = connection;
    }






















    private string ReadLine()
    {
        string? line = connection.GetFromUI();
        if (line == null)
        {
            line = "";
        }
        return line;
    }

    private void WriteLine(string message)
    {
        connection.PutOnUI(message + "\n");
    }

    private void Write(string message)
    {
        connection.PutOnUI(message);
    }
}
