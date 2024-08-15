using System;

namespace Client
{
    class ClientMain
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Websocket client side");
            Client.Connect(Client.wsUrl).Wait();

            Console.ReadKey();
        }
    }
}
