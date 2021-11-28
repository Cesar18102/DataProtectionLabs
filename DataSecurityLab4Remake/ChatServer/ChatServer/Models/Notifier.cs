using System;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

using Newtonsoft.Json;

using ChatServer.Models.SocketActions;

namespace ChatServer.Models
{
    public class Notifier : IDisposable
    {
        private EndPoint Endpoint { get; set; }
        private Socket Sender { get; set; }

        private Notifier() { }
        public Notifier(string endpoint, int port)
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(endpoint), port);
        }

        public void Send(SocketActionBase action)
        {
            //if (Sender == null || !Sender.Connected)
            {
                Sender = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Sender.Connect(Endpoint);
            }

            string json = JsonConvert.SerializeObject(action);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] message = BitConverter.GetBytes(data.Length).Concat(data).ToArray();
            Sender.Send(message);

            Sender.Shutdown(SocketShutdown.Both);
            Sender.Close();
        }

        public void Dispose()
        {
            Sender.Shutdown(SocketShutdown.Both);
            Sender.Close();
            Sender.Dispose();
        }
    }
}