using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClientSocket.Sockets
{
    public class Listener : IDisposable
    {
        public event EventHandler<RecieveEventArgs> Recieved;

        private Socket Ear { get; set; }
        private Socket Connection { get; set; }

        public Task Listening { get; private set; }
        public int Port => (Ear.LocalEndPoint as IPEndPoint).Port;

        public Listener(string endpoint, int port = 0)
        {
            IPAddress address = IPAddress.Parse(endpoint);
            EndPoint epoint = new IPEndPoint(address, port);

            Ear = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Ear.Bind(epoint);
            Ear.Listen(16);
        }

        public void Init()
        {
            Listening = Task.Run(ListenLoop);
        }

        private void ListenLoop()
        {
            Connection = Ear.Accept();

            byte[] messageSizeBytes = new byte[sizeof(int)];
            Connection.Receive(messageSizeBytes);
            int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);

            byte[] messageBytes = new byte[messageSize];
            Connection.Receive(messageBytes);

            RecieveEventArgs args = new RecieveEventArgs(Connection.RemoteEndPoint, messageBytes);
            Recieved?.Invoke(Connection, args);

            Connection.Shutdown(SocketShutdown.Both);
            Connection.Close();

            ListenLoop();
        }

        public void Dispose()
        {
            Ear.Shutdown(SocketShutdown.Both);
            Ear.Close();
            Ear.Dispose();
        }

        public class ConnectEventArgs : EventArgs
        {
            public string From { get; private set; }
            public ConnectEventArgs(EndPoint endpoint) =>
                From = (endpoint as IPEndPoint)?.Address.ToString();
        }

        public class RecieveEventArgs : EventArgs
        {
            public string From { get; private set; }
            public byte[] Data { get; private set; }

            public RecieveEventArgs(EndPoint endpoint, byte[] data)
            {
                From = (endpoint as IPEndPoint)?.Address.ToString();
                Data = data;
            }
        }
    }
}
