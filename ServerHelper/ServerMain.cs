namespace ServerHelper
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Reflection;
    using System.Threading.Tasks;

    class ServerMain
    {
        /// <summary>
        /// Url used for the http request/httpResponse
        /// </summary>
        private const string httpUrl = "http://localhost:8666/";

        /// <summary>
        /// Home page
        /// </summary>
        private static readonly object HomePage = new
        {
            Title = "Serveur de test",
            Command_1 = "/ - Ici",
            Command_2 = "/test - Affiche le nom du poste",
            Command_3 = "/app - Tratement des requête du client "
        };

        /// <summary>
        /// Chemin du fichier de logs
        /// </summary>
        public static string LogsDir = string.Empty;

        public static void Main()
        {
            // place le chemin des logs
            ServerMain.LogsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + "logs.txt";

            Logger.ClearLogFile();
            Logger.Log("Starting...");

            // Initialise le menu et l'icone
            Task task = Task.Factory.StartNew(Interface.InitIconMenu);

            // Create a listener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(httpUrl);
            listener.IgnoreWriteExceptions = true;

            Logger.Log("Successfully started");

            bool running = true;
            try
            {
                listener.Start();
                Logger.Log($"Start listening on {httpUrl}");

                while (running)
                {
                    // bloque jusqu'à reception d'une requête
                    HttpListenerContext context = listener.GetContext();

                    // process
                    ServerMain.ProcessRequest(context);
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"Exception caught in \"ServerMain\": {ex}");
                running = false;
            }

            /* Websocket managment */
            // ServerWebSocket serverWS = new ServerWebSocket();
            // serverWS.Start();
        }

        internal static void ProcessRequest(HttpListenerContext context)
        {
            string type = context.Request.HttpMethod;
            string request = context.Request.Url.AbsolutePath;

            Console.WriteLine($"Receiving {type} request from {request}");

            HttpListenerResponse httpResponse = context.Response;
            httpResponse.StatusCode = 200;

            try
            {
                switch (type)
                {
                    case "GET":
                        // Affiche la page par défaut
                        if (request == "/")
                        {
                            Helper.RespondString(HomePage.ToString(), ref httpResponse);
                        }
                        // Renvoie le persoId de l'utilisateur
                        else if (request == "/test")
                        {
                            Helper.RespondString(Environment.MachineName, ref httpResponse);
                        }
                        else if (request.StartsWith("/app/", StringComparison.OrdinalIgnoreCase))
                        {
                            AppEndPoint.ProcessRequest(context);
                        }
                        else
                        {
                            Helper.RespondString("Error: unknown GET request", ref httpResponse, code: 400);
                        }
                        break;

                    case "POST":
                        // Affiche la page par défaut
                        if (request == "/")
                        {
                            Helper.RespondString(HomePage.ToString(), ref httpResponse);
                        }
                        // Renvoie le persoId de l'utilisateur
                        else if (request == "/test")
                        {
                            Helper.RespondString(Environment.MachineName, ref httpResponse);
                        }
                        else if (request.StartsWith("/app/", StringComparison.OrdinalIgnoreCase))
                        {
                            AppEndPoint.ProcessRequest(context);
                        }
                        else
                        {
                            Helper.RespondString("Error: unknown GET request", ref httpResponse, code: 400);
                        }
                        break;

                    default:

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }
    }
}
