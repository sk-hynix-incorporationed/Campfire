using System;

namespace Campfire
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  campfire server <port>");
                Console.WriteLine("  campfire client");
                return;
            }
            if (args[0] == "server")
            {
                if (args.Length < 2 || !int.TryParse(args[1], out int port))
                {
                    Console.WriteLine("You must provide a valid port for the server.");
                    return;
                }
                Server.Start(port);
            }
            else if (args[0] == "client")
            {
                Client.Start();
            }
            else
            {
                Console.WriteLine("Unknown mode. Use 'server' or 'client'.");
            }
        }
    }
}
