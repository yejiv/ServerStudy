using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ServerSession
    {
        public ServerSession()
        {
            client = new();
            buffer = new Byte[256];
        }

        TcpClient? client;

        NetworkStream? stream;

        Byte[] buffer;

        public void Connect(IPAddress iPAddress, Int32 port)
        {
            if (client == null)
                throw new InvalidOperationException("이미 닫힌 세션입니다.");

            try
            {
                client.Connect(iPAddress, port);
                stream = client.GetStream();
            }
            catch (SocketException) 
            {
                Close();
                throw;
            }

            //IPEndPoint ipEndPoint = new IPEndPoint(iPAddress, port);
            //client.Connect(ipEndPoint);

            //client = new TcpClient(server, port);
            
        }
        public void Send(string message)
        {
            if (stream == null)
                throw new InvalidOperationException("서버에 연결되지 않았습니다.");

            if (client == null)
                throw new InvalidOperationException("이미 닫힌 세션입니다.");

            Byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);
            stream.Write(msg, 0, msg.Length);
            //Console.WriteLine("Sent: {0}", message);
        }
        async public Task<string?> Receive()
        {
            if (stream == null)
                throw new InvalidOperationException("서버에 연결되지 않았습니다.");

            if (client == null)
                throw new InvalidOperationException("이미 닫힌 세션입니다.");

            Int32 bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            return (bytes == 0) ? null : System.Text.Encoding.UTF8.GetString(buffer, 0, bytes);
        }
        public void Close()
        {
            if (client != null)
            {
                client.Close();
                stream = null; 
                client = null;
            }
        }

        public async Task ReceiveLoopAsync()
        {
            while (true)
            {
                string? msg = await Receive();
                if (msg == null)
                    break;
                Console.WriteLine(msg);
            }
        }
    }
}
