using STUN;
using System.Net;
using System.Net.Sockets;
using System.Text;



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


// Our UDP socket
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//socket.Bind(new IPEndPoint(IPAddress.Any, 0)); // TODO: idk what this does

// Query the STUN server
for (int i = 0; i < 100; ++i)
{
    IPEndPoint endpoint = new IPEndPoint(Dns.GetHostAddresses("stun.l.google.com").First(), 19302);
    STUNQueryResult result = STUNClient.Query(socket, endpoint, STUN.STUNQueryType.PublicIP);

    Console.WriteLine("External address: " + (result.PublicEndPoint is not null ? result.PublicEndPoint.ToString() : string.Empty));
}
Console.WriteLine();

// Let user connect to another player (via that player's external address)
Console.Write("Enter an external address to connect to: ");
IPEndPoint playerEndpoint = IPEndPoint.Parse(Console.ReadLine());

//socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true); // TODO: idk what this does
if (TestConnection(socket) == false)
{
    return;
}


// Player chat room loop
Console.WriteLine("Connected!");
Console.WriteLine();
while (TestConnection(socket))
{
    // Send message
    Console.Write("Chat message: ");
    string message = Console.ReadLine();

    socket.SendTo(Encoding.ASCII.GetBytes(message is not null ? message : string.Empty), playerEndpoint);


    // Recieve message
    if (socket.Poll(2000 * 1000, SelectMode.SelectRead) == false)
    {
        // Nothing recieved
        continue;
    }
    byte[] recieved = new byte[2048];
    int length = socket.Receive(recieved);

    byte[] bytesRecieved = new byte[length];
    Array.Copy(recieved, bytesRecieved, length);
    string messageRecieved = Encoding.ASCII.GetString(bytesRecieved);
    Console.WriteLine("player: " + messageRecieved);
}

Console.WriteLine("Disconnected");









bool TestConnection(Socket socket, int maxTries = 5)
{
    for (int i = 0; i < maxTries; ++i)
    {
        if (socket.Poll(1000 * 1000, SelectMode.SelectWrite))
        {
            //if (socket.Connected)
            {
                // Successful!
                break;
            }

        }

        Console.Write(". ");

        if (i >= maxTries - 1)
        {
            Console.WriteLine();
            Console.WriteLine("Failed to connect");
            return false;
        }
    }

    return true;
}
