

using System;
using System.Net;

namespace ServerHelper
{
    internal class AppEndPoint
    {
        private static readonly object HomePage = new
        {
            Title = "Listening to client \'app\'",
            Command_1 = "/ - Ici",
            Command_2 = "/ping - Permet au client de savoir si le serveur est connecté"
        };

        internal static void ProcessRequest(HttpListenerContext context)
        {
            // on récup la requête
            string appRequest = context.Request.Url.LocalPath.Substring("/app/".Length);
            Console.WriteLine($"Receiving a request from the client [{appRequest}]");
            var httpResponse = context.Response;

            try
            {
                // on récup le payload de la requête
                string payload = Helper.ReadInputFromContext(context);

                switch (appRequest)
                {
                    case "":
                        Helper.RespondString(HomePage.ToString(), ref httpResponse);
                        break;

                    case "ping":
                        Helper.RespondString("OK", ref httpResponse);
                        break;

                    default:
                        Helper.RespondString($"Unknwon request [{appRequest}]", ref httpResponse, 400);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex}");
            }
        }


        
    }
}
