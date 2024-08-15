using System.IO;
using System.Net;
using System.Text;

namespace ServerHelper
{
    internal static class Helper
    {
        /// <summary>
        /// Read the payload from a request as a string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static string ReadInputFromContext (HttpListenerContext context)
        {
            string payload = "";
            var request = context.Request;

            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                payload = reader.ReadToEnd();
            }
            return payload;
        }

        /// <summary>
        /// Prepare a response
        /// </summary>
        /// <param name="data"></param>
        /// <param name="response"></param>
        /// <param name="code"></param>
        internal static void RespondString(string data, ref HttpListenerResponse response, int code = 200)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(data);
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = dataArray.LongLength;
            if (code != 200) 
                response.StatusCode = code;

            Logger.Log($"Replied: {code} - {data}");

            response.OutputStream.Write(dataArray, 0, data.Length);
        }
    }
}
