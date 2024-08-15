

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
            Command_2 = "/ping - Permet au client de savoir si le serveur est connecté",
            Command_3 = "/websocket - Créer un websocket pour envoyer des events au client"
        };

        internal static void ProcessRequest(HttpListenerContext context)
        {
            // on récup la requête
            string appRequest = context.Request.Url.LocalPath.Substring("/app/".Length);
            Logger.Log($"Receiving a request from the client [{appRequest}]");
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

                    case "websocket":
                        // Event registry


                        // Prepare WS
                        ServerWebSocket serverWS = new ServerWebSocket();
                        serverWS.Start();

                        Helper.RespondString($"Websocket created on url: {ServerWebSocket.wsUrl}", ref httpResponse);
                        break;

                    default:
                        Helper.RespondString($"Unknwon request [{appRequest}]", ref httpResponse, 400);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception caught: {ex}");
            }
        }


        
    }
}
