namespace ServerHelper
{
    using System.Net;
    using System;
    using System.Net.WebSockets;
    using System.Threading;

    class Server
    {
        private const string url = "https://localhost:8666/";

        public async void Start()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(Server.url);
            httpListener.Start();

            Console.WriteLine($"Server started, listening on {Server.url}");

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    Server.ProcessRequest(context);
                }
                else
                {
                    // Return 400, then close
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private static async void ProcessRequest (HttpListenerContext context)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                string ipAdress = context.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine($"Connected: IpAdress {ipAdress}");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
                Console.WriteLine($"Exception caught: {e}");
            }

            WebSocket socket = webSocketContext.WebSocket;
            try
            {
                byte[] receivedBuffer = new byte[1024];
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(receivedBuffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else
                    {
                        await socket.SendAsync(new ArraySegment<byte>(receivedBuffer, 0, result.Count), WebSocketMessageType.Binary, result.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                if (socket != null)
                    socket.Dispose();
            }
        }
    }
}

