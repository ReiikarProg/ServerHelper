using System;
using System.Net.Http;
using System.Text;
using System.Security.Policy;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Client
{
    class ClientMain
    {
        /// <summary>
        /// Http url
        /// </summary>
        private const string baseUrl = "http://localhost:8666/app";

        /// <summary>
        /// WS url
        /// </summary>
        private const string wsUrl = "ws://localhost:80/";

        /// <summary>
        /// Client object
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine("*** WSClient ***");
            Console.WriteLine("- Liste des commandes -\n");
            Console.WriteLine(" \"ping\" - Ping le serveur");
            Console.WriteLine(" \"websocket\" - Demande la création d'un websocket");
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

                        // les vraies requêtes
                        case "ping":
                        case "websocket":
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
                                Console.WriteLine($"Received: {(int) response.StatusCode} - {responseValue}");

                                ClientMain.ProcessResponse(requestStr, response.StatusCode, responseValue);
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

        /// <param name="initialRequest">la requête dont on traite la réponse</param>
        /// <param name="statusCode">le code retour de la requête</param>
        /// <param name="responseStr"></param>
        static async void ProcessResponse(string initialRequest, HttpStatusCode statusCode, string responseStr)
        {
            Console.WriteLine($"Entering \"ProcessingResponse\" method with parameters: Request -> {initialRequest}, ReturnCode -> {(int) statusCode}");

            if (statusCode == HttpStatusCode.OK && initialRequest.Equals("websocket", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Connecting to the server websocket");
                WSClient wsClient = new WSClient();
                await wsClient.ConnectAndWaitForEvents(ClientMain.wsUrl);
            }
        }
    }
}
