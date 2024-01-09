using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatAPI
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;

        /// <summary>
        /// Event for received messages.
        /// </summary>
        public event Action<string> MessageReceived;

        /// <summary>
        /// Client Object.
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public Client(string serverIp, int serverPort)
        {
            try
            {
                tcpClient = new TcpClient(serverIp, serverPort);
                stream = tcpClient.GetStream();

                _ = StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to the Server.");
                Console.WriteLine();
                Console.Error.WriteLine(ex);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Send a message to the Server Asynchronously.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task StartListening()
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(receivedMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listening to the server: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
            }
        }
    }
}