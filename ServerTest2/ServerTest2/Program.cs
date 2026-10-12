// See https://aka.ms/new-console-template for more information

using Server;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

TcpListener server = null;
try
{
    // [어디서 기다릴지 정함]
    Int32 port = 13000;
    IPAddress localAddr = IPAddress.Parse("127.0.0.1"); //IPAddress.Any로 하면  모든 네트워크에서 받기

    // [ 리스너 객체 (서버)생성 + 포트 열고 대기 ]
    server = new TcpListener(localAddr, port);
    server.Start();

    // [ 세션마다 붙일 번호 ]
    int nextId = 1;

    while (true)
    {
        // [ 접속 요청이 올 때까지 대기 (await: 기다리는 동안 스레드를 붙잡지 않음) ]
        Console.WriteLine("Waiting for a connection... ");
        TcpClient client = await server.AcceptTcpClientAsync(); //대신 AcceptSocketAsync()를 써도 된다
        Console.WriteLine("Connected!");
            /*
                [Socket 방식]
                using Socket socket = server.AcceptSocket();
                int i = socket.Receive(bytes);
                socket.Send(msg);
             */

        Session session = new Session(nextId++, client);
        SessionManager.AddSession(session.id, session);
        _ = session.RunAsync();
    }
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    server.Stop();
}

Console.WriteLine("\nHit enter to continue...");
Console.Read();

#region window 참고 코드

////[ 어디서 기다릴지 정함 ]
//
//// Set the TcpListener on port 13000.
//Int32 port = 13000; //서버가 사용할 포트 번호
//
////문자열 "127.0.0.1"을 IPAddress 객체로 변환(네트워크 API에 전달하기 위함)
//IPAddress localAddr = IPAddress.Parse("127.0.0.1");
//
//// 리스너 객체를 만듦 (== 서버를 만듦)
//// TcpListener server = new TcpListener(port);
//server = new TcpListener(localAddr, port);
//
//// Start listening for client requests.
//server.Start();
///*
//    실제로 포트를 열고 접속 대기를 시작! (== 서버를 엶)
//    이 시점부터 클라이언트가 접속할 수 있다.
//     13000번 포트를 다른 프로그램(예: 이미 실행 중인 서버)이 쓰고 있으면 여기서 SocketException이남
//*/
//
//// Buffer for reading data
//Byte[] bytes = new Byte[256];
///*
//    받은 데이터를 담을 256바이트짜리 버퍼
//    네트워크로는 바이트만 오간다.
//     stream.Read(bytes, ...)가 받은 데이터를 이 배열에 채워넣음
//*/
//String data = null;
//while (true)
//{
//    Console.Write("Waiting for a connection... ");
//
//    // Perform a blocking call to accept requests.
//    // You could also use server.AcceptSocket() here.
//    using TcpClient client = server.AcceptTcpClient();
//    /*
//        클라이언트의 접속 요청을 받아들이는 함수.
//        접속 요청이 올 때까지 이 줄에서 프로그램이 멈춰서 기다림.
//        접속이 오면 그 클라이언트와 연결된 TcpClient 객체를 하나 새로 만들어서 돌려줌.
//        클라이언트 프로그램이라는 뜻이 아니라 "상대방과의 TCP 연결"이라는 뜻
//
//      이 변수가 속한 블록(여기서는 while 루프 한 바퀴)이 끝나면 client.Dispose()를 자동으로 호출
//      Dispose()는 연결을 닫습니다. 그러면 상대 클라이언트 쪽에서 Read()가 0을 반환
//
//        이 함수 대신 AcceptSocket()를 써도 된다.
//        사실 TcpClient는 내부에 Socket을 하나 가지고 있는 포장 클래스
//        client.Client 속성으로 안에 든 Socket을 꺼낼 수도 있다.
//    */
//    Console.WriteLine("Connected!");
//
//    data = null;
//
//    // Get a stream object for reading and writing
//    NetworkStream stream = client.GetStream();
//    /*
//        연결을 통해 실제로 데이터를 주고받을 통로(NetworkStream)를 꺼냄
//        client에 속해 있어서, using 안써도 클라에서 처리해줌
//    */
//
//    int i;
//
//    // Loop to receive all the data sent by the client.
//    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
//    //읽은 바이트 수를 i에 저장
//    //멈춤(블로킹): 클라이언트가 데이터를 보낼 때까지 이 줄에서 기다립니다.???
//    {
//        // Translate data bytes to a ASCII string.
//        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
//        //받은 바이트를 문자열로 변환 (바이트를 ASCII 규칙으로 해석)
//        //Encoding.UTF8로 바꿔야함. -> 인코딩은 양쪽이 같아야 한다.  클라이언트가 UTF8로 보내고 서버가 ASCII로 읽으면 깨짐
//        Console.WriteLine("Received: {0}", data); //받은 내용을 서버 콘솔에 출력합니다. 서버가 무엇을 받았는지 확인하는 로그
//        /*
//            버퍼는 재사용되므로, 이전에 받았던 데이터가 뒤쪽에 남아 있을 수 있음
//            1번째 Read: "hello" 받음 → bytes = [h e l l o 0 0 0 ...]   i = 5
//            2번째 Read: "hi"    받음 → bytes = [h i l l o 0 0 0 ...]   i = 2
//            2번째에서 배열 전체를 문자열로 바꾸면 "hillo"가 됨. 그래서 항상 i만큼만 사용해야한다. 
//            이러한 이유로 다음 줄에서 i를 넘김.
//        */
//        // Process the data sent by the client.
//        data = data.ToUpper(); //문자열을 대문자로
//        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
//        //보낼 문자열을 바이트 배열로 변환 -> stream.Write는 바이트만 보낼 수 있기 때문
//
//        // Send back a response.
//        stream.Write(msg, 0, msg.Length); // 바이트 배열을 클라이언트에게 보냄
//        Console.WriteLine("Sent: {0}", data);
//    }
//}
#endregion