using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicHTTPServer;

class Program
{
    static void Main(string[] args)
    {
        var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080 );
        Console.WriteLine("Server started");
        listener.Start();
        Console.WriteLine("Listening on port 8080");
        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");
            var steam = client.GetStream();
            var buffer = new byte[1024];
            steam.Read(buffer, 0, buffer.Length);
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
            var content = "<html><body><h1>Hello World!</h1></body></html>";
            var respose = "HTTP/1.1 200 OK" + Environment.NewLine +
                          $"Content-Length: {content.Length}" + Environment.NewLine + Environment.NewLine + content;
            steam.Write(Encoding.UTF8.GetBytes(respose));
        }
    }
}