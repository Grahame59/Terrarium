using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Error 
{
    public class ErrorLogger
    {

        public static void SendTestError(string message)
        {
            var client = new TcpClient("localhost", 5000);
            var data = Encoding.UTF8.GetBytes(message);
            using (var stream = client.GetStream())
            {
                stream.Write(data, 0, data.Length);
            }
            client.Close();
        }
    }
}
