using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHelper
{
    internal static class WSEndPoint
    {
        public static byte[] ProcessRequest(byte[] receivedRequest)
        {
            byte[] output = null;

            // quelle requête avons-nous reçue ?
            string request = Encoding.UTF8.GetString(receivedRequest);
            Console.WriteLine($"Traitement de la requête {request}");
            switch (request)
            {
                case "ping":
                    output = Encoding.UTF8.GetBytes("Je suis bien en vie !");
                    break;

                default:
                    Console.WriteLine("Requête inconnue");
                    output = Encoding.UTF8.GetBytes($"La requête [{request}] est inconnue");
                    break;
            }

            return output;
        }
    }
}
