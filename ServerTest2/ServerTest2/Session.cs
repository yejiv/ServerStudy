using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    internal class Session
    {
        public Session(int _id, TcpClient _client) 
        {
            id = _id;
            client = _client;
            stream = client.GetStream();
        }

        public int id{ get; init; }
        TcpClient client;
        NetworkStream stream;

        Byte[] bytes = new Byte[256];   //받을 데이터
        IPEndPoint endpoint;

        public async Task RunAsync()
        {
            endpoint = (IPEndPoint)client.Client.RemoteEndPoint!;

            try
            {
                string data;
                int i;

                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)  //0이 아닐 때까지 읽는다.
                {
                    // [ UTF8 string으로 변환해서 출력 ]
                    data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                    Console.WriteLine($"[{endpoint}]: {data}");

                    // [처리 예시]
                    data = data.ToUpper();

                    // Send back a response.
                    await SessionManager.BroadcastAsync($"[{id}]: {data}");
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"[{id}] 연결이 강제로 끊김");
            }
            finally
            {
                Close();
            }
        }
        public async Task SendAsync(string message)
        {
            byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);

            try { await stream.WriteAsync(msg, 0, msg.Length); }
            catch (Exception)
            {
                client.Close();
                Console.WriteLine($"[{id}] 전송 실패 → 연결 닫음");
            }
            //Console.WriteLine(message);
        }
        private void Close()
        {
            SessionManager.RemoveSession(id);
            client.Close();
            Console.WriteLine($"[{id}] 연결 종료");
        }
    }
}
