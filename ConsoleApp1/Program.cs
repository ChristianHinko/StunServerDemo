using STUN;
using System.Net;
using System.Net.Sockets;
using System.Text;



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
Console.Write("Enter an external address to connect to: ");
IPEndPoint playerEndpoint = IPEndPoint.Parse(Console.ReadLine());

socket.Connect(playerEndpoint);
if (socket.Connected == false)
{
    Console.WriteLine("Failed to connect");
    return;
}


// Player chat room loop
Console.WriteLine("Connected!");
while (socket.Connected)
{
    // Send message
    Console.Write("Chat message: ");
    string message = Console.ReadLine();

    socket.Send(Encoding.ASCII.GetBytes(message is not null ? message : string.Empty));

    // Recieve message
    byte[] recieved = new byte[2048];
    int length = socket.Receive(recieved);

    byte[] bytesRecieved = new byte[length];
    Array.Copy(recieved, bytesRecieved, length);
    string messageRecieved = Encoding.ASCII.GetString(bytesRecieved);
    Console.WriteLine("player: " + messageRecieved);
}

Console.WriteLine("Disconnected");
