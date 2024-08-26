using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Error 
{
    public class ErrorLogger
    {

        public static void SendError(string error, string script, string networkListener)
        {
            // Concatenate the strings with a delimiter (e.g., '|')
            string message = $"{error}|{script}|{networkListener}";
            
            // Convert the concatenated message to a byte array
            var client = new TcpClient("localhost", 5000);
            var data = Encoding.UTF8.GetBytes(message);
            
            using (var stream = client.GetStream())
            {
                // Send the data over the network
                stream.Write(data, 0, data.Length);
            }
            
            // Close the client connection
            client.Close();
        }

    }
}
