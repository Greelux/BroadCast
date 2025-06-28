using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    const int BROADCAST_PORT = 5000;
    const int MULTICAST_PORT = 5001;
    const int TCP_PORT = 5002;
    const string MULTICAST_GROUP = "224.1.1.1";

    static void ListenBroadcast()
    {
        UdpClient udpClient = new UdpClient(BROADCAST_PORT);
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, BROADCAST_PORT);
        Console.WriteLine("[Broadcast] Слухаю...");

        while (true)
        {
            byte[] data = udpClient.Receive(ref ep);
            Console.WriteLine(Encoding.UTF8.GetString(data));
        }
    }

    static void ListenMulticast()
    {
        UdpClient udpClient = new UdpClient(MULTICAST_PORT);
        udpClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_GROUP));
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, MULTICAST_PORT);
        Console.WriteLine("[Multicast] Слухаю групу...");

        while (true)
        {
            byte[] data = udpClient.Receive(ref ep);
            Console.WriteLine(Encoding.UTF8.GetString(data));
        }
    }

    static void GetPersonalMessage(string name)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", TCP_PORT);
        NetworkStream stream = tcpClient.GetStream();

        byte[] data = Encoding.UTF8.GetBytes(name);
        stream.Write(data, 0, data.Length);

        byte[] buffer = new byte[1024];
        int size = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, size));

        tcpClient.Close();
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write("Ваше ім'я: ");
        string name = Console.ReadLine();
        Console.Write("Ваш поверх: ");
        string floor = Console.ReadLine();

        new Thread(ListenBroadcast).Start();
        new Thread(ListenMulticast).Start();

        while (true)
        {
            Console.WriteLine("\n1 - Отримати особисте повідомлення");
            Console.Write(">> ");
            string input = Console.ReadLine();

            if (input == "1")
                GetPersonalMessage(name);
        }
    }
}
