using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    const int BROADCAST_PORT = 5000;
    const int MULTICAST_PORT = 5001;
    const int TCP_PORT = 5002;
    const string MULTICAST_GROUP = "224.1.1.1";

    static void BroadcastMessage(string msg)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        byte[] data = Encoding.UTF8.GetBytes(msg);
        udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT));
        udpClient.Close();
    }

    static void MulticastMessage(string msg)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_GROUP));
        byte[] data = Encoding.UTF8.GetBytes(msg);
        udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(MULTICAST_GROUP), MULTICAST_PORT));
        udpClient.Close();
    }

    static void HandleTCPClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int size = stream.Read(buffer, 0, buffer.Length);
        string name = Encoding.UTF8.GetString(buffer, 0, size);
        string response = $"[USER:{name}] Привіт, {name}, не забудь забрати документи.";

        byte[] responseData = Encoding.UTF8.GetBytes(response);
        stream.Write(responseData, 0, responseData.Length);

        client.Close();
    }

    static void StartTCPServer()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, TCP_PORT);
        listener.Start();
        Console.WriteLine("[TCP] Сервер запущено...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(HandleTCPClient, client);
        }
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Thread tcpThread = new Thread(StartTCPServer);
        tcpThread.IsBackground = true;
        tcpThread.Start();

        while (true)
        {
            Console.WriteLine("\n1 - Надіслати [ALL]");
            Console.WriteLine("2 - Надіслати [FLOOR:n]");
            Console.Write(">> ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                Console.Write("Оголошення: ");
                string msg = Console.ReadLine();
                BroadcastMessage($"[ALL] {msg}");
            }
            else if (input == "2")
            {
                Console.Write("Номер поверху: ");
                string floor = Console.ReadLine();
                Console.Write("Повідомлення: ");
                string msg = Console.ReadLine();
                MulticastMessage($"[FLOOR:{floor}] {msg}");
            }
        }
    }
}
