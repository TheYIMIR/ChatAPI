using ChatAPI.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatAPI
{
    public class Server
    {
        private TcpListener tcpListener;
        private bool isRunning;
        private List<NetworkStream> clientStreams;

        /// <summary>
        /// Event for received messages.
        /// </summary>
        public event Action<string> MessageReceived;

        /// <summary>
        /// Server Object.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public Server(string ipAddress = "127.0.0.1", int port = 8888)
        {
            IniFile iniFile = new IniFile("server_config.ini");

            Dictionary<string, string> serverSection = iniFile.GetSection("Server");

            string ip = serverSection.TryGetValue("IP", out string ipValue) ? ipValue : ipAddress;
            string portStr = serverSection.TryGetValue("Port", out string portValue) ? portValue : port.ToString();

            tcpListener = new TcpListener(IPAddress.Parse(ip), int.Parse(portStr));
            isRunning = false;
            clientStreams = new List<NetworkStream>();
        }

        /// <summary>
        /// Start Server Asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            if (isRunning)
                return;

            isRunning = true;
            tcpListener.Start();

            Console.WriteLine($"Server started on {tcpListener.LocalEndpoint}");

            try
            {
                while (isRunning)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    NetworkStream stream = tcpClient.GetStream();
                    clientStreams.Add(stream);

                    _ = HandleClientAsync(tcpClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Stop Server.
        /// </summary>
        public void Stop()
        {
            if (!isRunning)
                return;

            isRunning = false;
            tcpListener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = new byte[1024];

                while (isRunning)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);

                    await BroadcastMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                clientStreams.Remove(tcpClient.GetStream());
                tcpClient.Close();
            }
        }

        private async Task BroadcastMessageAsync(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var stream in clientStreams)
            {
                try
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
                catch
                {

                }
            }
        }
    }
}