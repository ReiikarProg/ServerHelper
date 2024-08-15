using System;
using System.Net.Http;
using System.Text;
using System.Security.Policy;
using System.Threading.Tasks;
using System.IO;

namespace Client
{
    class ClientMain
    {
        private const string baseUrl = "http://localhost:8666/app";
        
        private static readonly HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine("*** WSClient ***");
            Console.WriteLine("- Liste des commandes -\n");
            Console.WriteLine(" \"ping\" - Ping le serveur");
            Console.WriteLine(" \"q/Q\" - Exit");

            while (true)
            {
                Console.Write($"\nEnter you request: ");
                var requestStr = Console.ReadLine();

                if (!string.IsNullOrEmpty(requestStr))
                {
                    string uri = baseUrl + "//" + requestStr;
                    string contentStr = "";
                    bool shouldSend = true;

                    switch (requestStr)
                    {
                        case "Q":
                        case "q":
                            Environment.Exit(0);
                            break;

                        case "ping":
                            break;

                        default:
                            shouldSend = false;
                            Console.WriteLine($"Unknown request [{requestStr}], ignoring");
                            break;
                    }

                    if (shouldSend)
                    {
                        StringContent httpContents = new StringContent(contentStr, Encoding.UTF8, "application/json");

                        try
                        {
                            // On récupère la réponse du serveur
                            HttpResponseMessage response = _httpClient.PostAsync(uri, httpContents).Result;

                            if (response != null)
                            {
                                var responseValue = string.Empty;

                                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                                {
                                    var stream = t.Result;
                                    using (var reader = new StreamReader(stream))
                                    {
                                        responseValue = reader.ReadToEnd();
                                    }
                                });

                                task.Wait(2000); // 2 sec timeout
                                Console.WriteLine($"Received: {responseValue}");
                                // Do some thing depending on the response !
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"Exception lors de l'envoie de la requête: {e}");
                        }
                    }
                }
            }
        }
    }
}
