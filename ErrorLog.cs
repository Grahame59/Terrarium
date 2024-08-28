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
             var client = new TcpClient("localhost", 5000);
    
            // Format the message with line breaks between each part
            var message = $"{Environment.NewLine}Error: {error}{Environment.NewLine}Script: {script}{Environment.NewLine}Context: {networkListener}";
            
            var data = Encoding.UTF8.GetBytes(message);
            
            using (var stream = client.GetStream())
            {
                stream.Write(data, 0, data.Length);
            }
            
            client.Close();
        }

    }
}
