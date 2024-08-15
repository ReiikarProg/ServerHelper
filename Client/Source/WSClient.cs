using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Windows.Forms;

namespace Client
{
    public class WSClient
    {
        /// <summary>
        /// Encodage par défaut pour envoi et reception de données.
        /// </summary>
        public static UTF8Encoding uft8Encoding = new UTF8Encoding();

        /// <summary>
        /// Objet websocket
        /// </summary>
        internal ClientWebSocket WebSocket = null;

        /// <summary>
        /// Connecte le client en Websocket au serveur d'écoute à l'URL fournie,
        /// puis envoie/reçoie des données en asynchrone.
        /// </summary>
        /// 
        /// <param name="url">URL.</param>
        public async Task ConnectAndWaitForEvents(string url)
        {
            ClientWebSocket webSocket = null;
            try
            {
                webSocket = new ClientWebSocket();
                // connection au serveur
                await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            
                // une fois connecté, attend en asynchrone des info du serveur
                await WSClient.Receive(webSocket);
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
                    MessageBox.Show("Reception d'un évènement WebSocket !");
                }
            }
        }
    }
}
