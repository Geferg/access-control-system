namespace CardReaderConsoleUI;

internal class Program
{
    static void Main(string[] args)
    {
        CardReaderLibrary.CardReader reader = new();

        Console.WriteLine("Hello, World!");

        while (true)
        {
            string? userInput = Console.ReadLine();
            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Empty input");
                break;
            }

            string[] inputFragments = userInput.Split(' ');
            string command = inputFragments[0];

            switch (command)
            {
                case "snode":
                    if (inputFragments.Length != 2)
                    {
                        Console.WriteLine("Wrong amount of arguments");
                        break;
                    }

                    if (int.TryParse(inputFragments[1], out int nodeNumber))
                    {
                        Console.WriteLine("");
                    }

                    break;

                case "sdate":
                    break;

                case "stime":
                    break;

                case "sout":
                    break;

                case "on":
                    break;

                case "off":
                    break;

            }
        }
    }
}
