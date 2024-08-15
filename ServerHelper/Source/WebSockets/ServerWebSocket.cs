namespace ServerHelper
{
    using System.Net;
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    class ServerWebSocket
    {
        internal const string wsUrl = "http://localhost:80/";

        internal static bool isConnected = false;

        public event EventHandler SampleEventHandler;

        public async void Start()
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(ServerWebSocket.wsUrl);
            httpListener.Start();

            Logger.Log($"ServerWebSocket started, listening on {ServerWebSocket.wsUrl}");

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    // connexion
                    WebSocketContext webSocketContext = null;
                    try
                    {
                        webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        string ipAdress = context.Request.RemoteEndPoint.Address.ToString();
                        ServerWebSocket.isConnected = true;
                        Logger.Log($"[WS] - Client connected:(IpAdress: {ipAdress})");

                        // register the event
                        this.SampleEventHandler += OnSampleEventFired;
                        Logger.Log($"[WS] - Event successfully registered");

                        var t = Task.Run(async delegate
                        {
                            await Task.Delay(5000);
                            if (this.SampleEventHandler != null)
                            {
                                this.SampleEventHandler.Invoke(webSocketContext, null);
                            }
                        });
                        t.Wait();
                    }
                    catch (Exception e)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.Close();
                        Logger.Log($"[WS] - Exception caught: {e}");
                    }
                }
                else
                {
                    // Return 400, then close
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        public static async void OnSampleEventFired(object sender, EventArgs e)
        {
            // Casting
            WebSocketContext webSocketContext = (WebSocketContext)sender;
            Logger.Log("[WS] - Event fired, sending...");

            WebSocket socket = webSocketContext.WebSocket;
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes("Event fired");
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Binary, false, CancellationToken.None);
                }
            }
            catch (Exception eex)
            {
                Logger.Log(eex.Message);
            }
        }
     }
}

