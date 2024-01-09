using ChatAPI;
using ConsoleClient;
using System.Text;

class Programm
{
    static List<String> sessionMessages = new List<String>();

    static void Main(string[] args)
    {
        Task.Run(() => Run()).Wait();
    }

    static async Task Run()
    {
        Startup();
    }

    private static void Startup()
    {
        if (Settings.Default.Username == String.Empty)
        {
            String input = userInput("Didn't found a username. What is your name?");

            Settings.Default.Username = input;
        }
        else
        {
            if (userInputKey("Do you want to change your username? 'Y' (Yes) or 'N' (No):", ConsoleKey.Y, ConsoleKey.N))
            {
                String input = userInput("Please enter you new username:");

                Settings.Default.Username = input;
            }
        }

        if (Settings.Default.IP == String.Empty)
        {
            Console.WriteLine("It seems that you never connected to any server yet.");
            String inputIP = userInput("Please enter the Server IP:");

            Settings.Default.IP = inputIP;

            if (Settings.Default.Port == String.Empty)
            {
                String inputPort = userInput("Please enter the Server Port:");

                Settings.Default.Port = inputPort;
            }
            else
            {
                Console.WriteLine();
                if (!userInputKey("Found a old Port. Do you want to use it? 'Y' (Yes) or 'N' (No):", ConsoleKey.Y, ConsoleKey.N))
                {
                    String inputPort = userInput("Please enter the Server Port:");

                    Settings.Default.Port = inputPort;
                }
            }

            ConnectToServer();
        }
        else
        {
            if (userInputKey("Do you want to reconnect to the old Server? 'Y' (Yes) or 'N' (No):", ConsoleKey.Y, ConsoleKey.N))
            {
                ConnectToServer();
            }
            else
            {
                String inputIP = userInput("Please enter the Server IP:");

                Settings.Default.IP = inputIP;

                String inputPort = userInput("Please enter the Server Port:");

                Settings.Default.Port = inputPort;

                ConnectToServer();
            }
        }
    }

    private static async void ConnectToServer()
    {
        Settings.Default.Save();

        Client client = new Client(Settings.Default.IP, int.Parse(Settings.Default.Port));
        client.MessageReceived += OnMessageReceived;

        Console.Clear();

        Console.WriteLine("Connected to Server!");

        SendMessage(client);
    }

    private static async void SendMessage(Client client)
    {
        StringBuilder userInputBuffer = new StringBuilder();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                string userInput = userInputBuffer.ToString();
                userInputBuffer.Clear();

                Console.Clear();

                foreach (string message in sessionMessages)
                {
                    Console.WriteLine(message);
                }

                await client.SendMessageAsync($"{Settings.Default.Username}: {userInput}");
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (userInputBuffer.Length > 0)
                {
                    userInputBuffer.Length--;
                    Console.Write("\b \b");
                }
            }
            else
            {
                userInputBuffer.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    private static String userInput(string inputMessage)
    {
        Console.WriteLine(inputMessage);
        String input = Console.ReadLine();
        if (input != null || input != String.Empty)
        {
            return input;
        }
        Console.WriteLine("Invalid input...");
        Console.WriteLine();
        return userInput(inputMessage);
    }

    private static bool userInputKey(string inputMessage, ConsoleKey yes, ConsoleKey no)
    {
        Console.WriteLine(inputMessage);
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        if (keyInfo.Key == yes)
        {
            return true;
        }
        else if (keyInfo.Key == no)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Invalid key. Please press either 'Y' (Yes) or 'N' (No).");
            Console.WriteLine();
            return userInputKey(inputMessage, yes, no);
        }
    }

    private static void OnMessageReceived(string message)
    {
        Console.WriteLine(message);
        sessionMessages.Add(message);
    }
}