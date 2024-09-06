using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Error
{
    public class ErrorLogger
    {
        private static readonly BlockingCollection<string> _logQueue = new BlockingCollection<string>();
        private static readonly Task _loggingTask;
        private static readonly string _serverAddress = "localhost";
        private static readonly int _port = 5000;

        static ErrorLogger()
        {
            // Start a background task to process the log queue
            _loggingTask = Task.Run(ProcessLogQueue);
        }

        private static async Task ProcessLogQueue()
        {
            foreach (var logMessage in _logQueue.GetConsumingEnumerable())
            {
                try
                {
                    using (var client = new TcpClient(_serverAddress, _port))
                    using (var stream = client.GetStream())
                    {
                        var data = Encoding.UTF8.GetBytes(logMessage);
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging to a file)
                    Console.WriteLine($"Logging failed: {ex.Message}");
                }
            }
        }

        public static void SendError(string error, string script, string context)
        {
            var message = $"{Environment.NewLine}Error: {error}{Environment.NewLine}Script: {script}{Environment.NewLine}Context: {context}";
            _logQueue.Add(message);
        }

        public static void SendDebug(string error, string script, string context)
        {
            var message = $"{Environment.NewLine}Debug: {error}{Environment.NewLine}Script: {script}{Environment.NewLine}Context: {context}";
            _logQueue.Add(message);
        }
    }
}
