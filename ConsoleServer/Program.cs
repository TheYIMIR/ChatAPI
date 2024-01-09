using ChatAPI;

public class Programm
{
    static void Main(string[] args)
    {
        Task.Run(() => Run()).Wait();
    }

    static async Task Run()
    {
        try
        {
            Server server = new Server();

            server.MessageReceived += OnMessageReceived;

            await server.Start();

            Console.WriteLine("Press Enter to stop the server.");
            Console.ReadLine();

            server.Stop();

        }
        catch (Exception ex)
        {
            Console.WriteLine("");
            Console.WriteLine("Error while starting the Server.");
            Console.Error.WriteLine(ex);
            Console.ReadKey();
        }
    }

    private static void OnMessageReceived(string message)
    {
        Console.WriteLine($"Log >> \"{message}\"");
    }
}