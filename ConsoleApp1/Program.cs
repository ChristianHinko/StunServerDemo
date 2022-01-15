using STUN;
using System.Net;
using System.Net.Sockets;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


// Our UDP socket
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
socket.Bind(new IPEndPoint(IPAddress.Any, 0));

// Query the STUN server
for (int i = 0; i < 100; ++i)
{
    IPEndPoint endpoint = new IPEndPoint(Dns.GetHostAddresses("stun.l.google.com").First(), 19302);
    STUNQueryResult result = STUNClient.Query(socket, endpoint, STUN.STUNQueryType.PublicIP);

    Console.WriteLine(result.PublicEndPoint.ToString()); 
}

// Let user connect to another player (via that player's external address)
Console.WriteLine("Enter an external address to connect to: ");
IPEndPoint playerEndpoint = IPEndPoint.Parse(Console.ReadLine());

socket.Connect(playerEndpoint);
while (socket.Connected)
{
    socket.Send(new byte[] { 1, 2, 3, 4 });

    byte[] recieved = new byte[1024];
    int length = socket.Receive(recieved);

    byte[] message = new byte[length];
    Array.Copy(recieved, message, length);
    Console.WriteLine("player: " + message.ToString());
}

Console.WriteLine("Disconnected");
