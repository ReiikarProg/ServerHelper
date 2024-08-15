using System.IO;
using System;

namespace ServerHelper
{
    internal static class Logger
    {
        public static void Log(string logMessage) 
        {
            try
            {
                using (StreamWriter w = File.AppendText(ServerMain.LogsDir))
                {
                    w.WriteLine($"{DateTime.Now.ToLocalTime()}: {logMessage}");
                    w.WriteLine("------------------------------------------------------------------------------------");
                }
            }
            catch
            {
                // ignore
            }
        }    

        public static void ClearLogFile()
        {
            if (!File.Exists(ServerMain.LogsDir))
                File.Create(ServerMain.LogsDir);

            TextWriter tw = new StreamWriter(ServerMain.LogsDir, false);
            tw.Write(string.Empty);
            tw.Close();
        }
    }
}
