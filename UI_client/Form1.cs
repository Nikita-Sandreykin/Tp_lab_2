using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI_client
{
    delegate void ButtonAction();
    public partial class Form1 : Form
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        ButtonAction ButtonAction;
        public Form1()
        {
            InitializeComponent();
        }
        public void SendMessage()
        {
            //Console.WriteLine("Введите сообщение: ");
            while (true)
            {
                if (Program.active)
                {
                    string message = textBox1.Text;
                    IncomingMessage Out = new IncomingMessage();
                    Out.PoemString = message;
                    Out.active = true;
                    message = JsonConvert.SerializeObject(Out);
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    Program.stream.Write(data, 0, data.Length);
                    Program.ButtonText = "Ожидайте очереди";//button1.Text = "Ожидайте очереди";
                    Program.IsButtonVisible = false;
                    Program.active = false;
                }
            }
        }
        private void UpdateForm()
        {
            while (true)
            {
                Action action = () =>
                {
                    button1.Visible = Program.IsButtonVisible;
                    button1.Text = Program.ButtonText;
                    richTextBox1.Text = Program.TextBoxString;
                    label1.Text = Program.LabelText;
                };
                Invoke(action);
                Thread.Sleep(60);
            }
        }
        private void enterName()
        {
            Program.userName = textBox1.Text;
            Program.client = new TcpClient();
            try
            {
                Program.client.Connect(host, port); //подключение клиента
                Program.stream = Program.client.GetStream(); // получаем поток
                Thread receiveThread = new Thread(new ThreadStart(Program.ReceiveMessage));
                receiveThread.Start(); //старт потока
                string message = Program.userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
                Program.TextBoxString += $"Добро пожаловать, {Program.userName}\n";
                ButtonAction = SendMessage;
                Program.ButtonText = "Дождитесь очереди...";
                Program.LabelText = "Ваше сообщение:";

                //SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Program.Disconnect();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ButtonAction = enterName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonAction();
        }

       
        private void Button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
