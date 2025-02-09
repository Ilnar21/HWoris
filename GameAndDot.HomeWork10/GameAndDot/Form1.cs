using GameAndDot.Packages;
using GameAndDot.Packages.Enums;
using GameAndDot.Packages.Models;
using System.Net.Sockets;
using System.Text.Json;

namespace GameAndDot
{
    public partial class Form1 : Form
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;

        private readonly Graphics _g;
        private readonly TcpClient _client = new TcpClient();
        private readonly StreamReader? _reader = null;
        private readonly StreamWriter? _writer = null;

        public Form1()
        {
            InitializeComponent();

            _g = panel1.CreateGraphics();
            _g.Clear(Color.White);

            _client.Connect(host, port); 
            _reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream());

            Task.Run(() => ReceiveMessageAsync());
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            panel1.Enabled = true;
            button1.Visible = false;
            textBox1.Visible = false;

            MessageObject pkg = new MessageObject
            {
                Type = MessageType.Register,
                Data = textBox1.Text
            };

            string message = JsonSerializer.Serialize(pkg);
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync();
        }

        // получение сообщений
        async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    string? str = await _reader.ReadLineAsync();
                    if (str == null) continue;

                    var message = JsonSerializer.Deserialize<MessageObject>(str);

                    switch (message.Type)
                    {
                        case MessageType.Register:
                            var users = JsonSerializer.Deserialize<string[]>(message.Data);

                            listBox1.Invoke(() =>
                            {
                                listBox1.Items.Clear();
                                foreach (var username in users)
                                {
                                    listBox1.Items.Add(username);
                                }
                            });

                            break;

                        case MessageType.Draw:
                            var points = JsonSerializer.Deserialize<List<PointObject>>(message.Data);

                            panel1.Invoke(() =>
                            {
                                _g.Clear(Color.White);

                                foreach (var point in points)
                                {
                                    _g.DrawEllipse(Pens.Red, point.X, point.Y, 4, 4);
                                }
                            });

                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }
        private async void panel1_MouseClick(object sender, MouseEventArgs e)
        {

            var point = new PointObject() { X = e.X, Y = e.Y };
            string data = JsonSerializer.Serialize(new PointObject() { X = e.X, Y = e.Y });


            MessageObject pkg = new MessageObject
            {
                Type = MessageType.Draw,
                Data = data
            };

            string message = JsonSerializer.Serialize(pkg);
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
