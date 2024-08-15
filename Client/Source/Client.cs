using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Client
{
    public class Client
    {
        /// <summary>
        /// Encodage par défaut pour envoi et reception de données.
        /// </summary>
        public static UTF8Encoding uft8Encoding = new UTF8Encoding();

        /// <summary>
        /// L'url de connexion Websocket, qui ne commence pas par 'http(s)'.
        /// </summary>
        internal const string wsUrl = "ws://localhost:8666/";

        /// <summary>
        /// Objet websocket
        /// </summary>
        internal ClientWebSocket WebSocket = null;

        /// <summary>
        /// Connect le client en Websocket au serveur d'écoute à l'URL fournie,
        /// puis envoie/reçoie des données en asynchrone.
        /// </summary>
        /// 
        /// <param name="url">URL.</param>
        public static async Task Connect(string url)
        {
            ClientWebSocket webSocket = null;
            try
            {
                webSocket = new ClientWebSocket();
                // connection au serveur
                await webSocket.ConnectAsync(new Uri(wsUrl), CancellationToken.None);

                // une fois connecté, envoi et reçoit de manière asynchrone
                await Task.WhenAll(Client.Receive(webSocket), Client.Send(webSocket));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e}");
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
               
            }
        }

        /// <summary>
        /// Envoie asynchrone
        /// </summary>
        ///
        /// <param name="webSocket">le client</param>
        private static async Task Send(ClientWebSocket webSocket)
        {
            // Permet l'envoi de donnée tant que le WS est ouvert
            while (webSocket.State == WebSocketState.Open)
            {
                Console.Write("Données à envoyer au serveur : ");
                string data = Console.ReadLine();
                byte[] buffer = uft8Encoding.GetBytes(data);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                Console.WriteLine($"Sent: {data}");

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Reception asynchrone
        /// </summary>
        ///
        /// <param name="webSocket">le client</param>
        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            // Permet la reception de données tant que le ws est ouvert
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // ferme le ws si l'on reçoit un "Close"
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", CancellationToken.None);
                }
                // sinon, traite la donnée reçue.
                else
                {
                    Console.WriteLine($"Received: {Encoding.UTF8.GetString(buffer).TrimEnd('\0')}");
                }
            }
        }
    }
}
