namespace ServerHelper
{
    using System;

    class ServerMain
    {
        public static void Main()
        {
            Server server = new Server();
            server.Start();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
