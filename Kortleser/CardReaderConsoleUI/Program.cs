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
                break;
            }

            userInput.Split(' ');
        }
    }
}
