using Client;
using System.Net;
using System.Net.Sockets;

ServerSession server = new();

try
{
    //server.Connect(IPAddress.Any);
    server.Connect(IPAddress.Parse("127.0.0.1"), 13000);
    ReceiveLoopAsync(server);

    while (true)
    {
        string? input = Console.ReadLine();
        if (input == null)
            break;
        server.Send(input);
    }
}
catch(SocketException)
{
    Console.WriteLine("서버에 연결할 수 없습니다.");
}
catch(IOException)
{
    Console.WriteLine("서버와의 연결이 끊어졌습니다.");
}
finally
{
    server.Close();
}

async Task ReceiveLoopAsync(ServerSession server)
{
    while (true)
    {
        string? msg = await server.Receive();
        if (msg == null)
            break;
        Console.WriteLine(msg);
    }
}


#region window 참고 코드
//Connect("127.0.0.1", "한글도되나 바꿔보자");
//
//static void Connect(String server, String message)
//{
//    try
//    {
//        Int32 port = 13000;
//        using TcpClient client = new TcpClient(server, port);
//        /*
//            생성자는 객체를 만들면서 바로 server:port로 접속을 시도함.
//                -> client.Connect(server, port);가 생성자에서 진행
//            접속이 완료되거나 실패할 때까지 기다린다.
//		  		  -> 서버가 꺼져 있으면 여기서 SocketException이이 난다
//                -> 아래가 비동기 방식
//
//            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
//            var ipEndPoint = new IPEndPoint(localAddr, port);
//            using TcpClient client2 = new();    //new(ipEndPoint);
//            await client.ConnectAsync(ipEndPoint);
//         */
//
//        Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
//
//        NetworkStream stream = client.GetStream();  //await 필요?
//        stream.Write(data, 0, data.Length);
//        Console.WriteLine("Sent: {0}", message);
//
//        data = new Byte[256];
//        String responseData = String.Empty;
//
//        Int32 bytes = stream.Read(data, 0, data.Length);
//        responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
//        Console.WriteLine("Received: {0}", responseData);
//    }
//    catch (ArgumentNullException e)
//    {
//        Console.WriteLine("ArgumentNullException: {0}", e);
//    }
//    catch (SocketException e)
//    {
//        Console.WriteLine("SocketException: {0}", e);
//    }
//
//    Console.WriteLine("\n Press Enter to continue...");
//    Console.Read();
//}
#endregion