using STUN;
using System.Net;
using System.Net.Sockets;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
socket.Bind(new IPEndPoint(IPAddress.Any, 0));

for (int i = 0; i < 100; ++i)
{
    IPEndPoint endpoint = new IPEndPoint(Dns.GetHostAddresses("stun.l.google.com").First(), 19302);
    STUNQueryResult result = STUNClient.Query(socket, endpoint, STUN.STUNQueryType.PublicIP);

    Console.WriteLine(result.PublicEndPoint.ToString()); 
}
