using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Drawing;
using GameAndDot.Packages;
using GameAndDot.Packages.Enums;
using GameAndDot.Packages.Models;

ServerObject server = new ServerObject();
await server.ListenAsync();

class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
    List<ClientObject> clients = new List<ClientObject>();
    List<PointObject> points = new List<PointObject>();

    // Словарь для хранения цвета каждого клиента
    Dictionary<string, string> clientColors = new Dictionary<string, string>();

    protected internal void RemoveConnection(string id)
    {
        ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
        if (client != null) clients.Remove(client);
        client?.Close();
    }

    protected internal async Task ListenAsync()
    {
        try
        {
            tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                ClientObject clientObject = new ClientObject(tcpClient, this);
                clients.Add(clientObject);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    protected internal async Task BroadcastMessageAsync(MessageObject message, string id)
    {
        string package = JsonSerializer.Serialize(message);

        foreach (var client in clients)
        {
            await client.Writer.WriteLineAsync(package);
            await client.Writer.FlushAsync();
        }
    }

    protected internal void Disconnect()
    {
        foreach (var client in clients)
        {
            client.Close();
        }
        tcpListener.Stop();
    }

    protected internal string GeUsersMessage()
    {
        var usernames = clients.Select(p => p.Username);
        string result = JsonSerializer.Serialize(usernames);
        return result;
    }

    protected internal void AddPoint(PointObject point)
    {
        points.Add(point);
    }

    protected internal string GetPoints()
    {
        string result = JsonSerializer.Serialize(points);
        return result;
    }

    // Метод для сохранения цвета клиента
    protected internal void SetClientColor(string clientId, string color)
    {
        if (!clientColors.ContainsKey(clientId))
        {
            clientColors.Add(clientId, color);
        }
        else
        {
            clientColors[clientId] = color;
        }
    }

    // Метод для получения цвета клиента
    protected internal string GetClientColor(string clientId)
    {
        return clientColors.ContainsKey(clientId) ? clientColors[clientId] : "Black"; 
    }
}

class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    public string Username { get; set; }
    public string ClientColor { get; set; }  

    TcpClient client;
    ServerObject server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        var stream = client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        try
        {
            while (true)
            {
                string? str = await Reader.ReadLineAsync();
                if (str == null) continue;

                var message = JsonSerializer.Deserialize<MessageObject>(str);
                MessageObject pkg = new MessageObject
                {
                    Type = message.Type
                };

                switch (message.Type)
                {
                    case MessageType.Register:
                        Username = message.Data;
                        // Сохраняем цвет клиента на сервере
                        var color = GenerateRandomColor(); 
                        ClientColor = color;
                        server.SetClientColor(Id, color);

                        pkg.Data = server.GeUsersMessage();
                        await server.BroadcastMessageAsync(pkg, Id);
                        break;

                    case MessageType.Draw:
                        var newPoint = JsonSerializer.Deserialize<PointObject>(message.Data);
                        newPoint.Color = ClientColor; // Устанавливаем цвет точки
                        server.AddPoint(newPoint);
                        pkg.Data = server.GetPoints();

                        await server.BroadcastMessageAsync(pkg, Id);
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            server.RemoveConnection(Id);
        }
    }

    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }

    private string GenerateRandomColor()
    {
        Random rand = new Random();
        return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)).Name;
    }
}
