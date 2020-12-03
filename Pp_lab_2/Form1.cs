using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
namespace Pp_lab_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        const int port = 8888;
        const string address = "127.0.0.1";
        string lastMessage = "";
        private string hello = "hello world";
        BackgroundWorker textRefresh = new BackgroundWorker();
        TcpClient client = null;
        NetworkStream stream = null;
        public string Hello { get => hello; set => hello = value; }
        private async void resetText()
        {
            while (true)
            {
                /*
                byte[] data;
                data = new byte[64]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                string message = builder.ToString();
                */
                Action action = () =>
                {
                    richTextBox1.Text += "Ceрвер: " + lastMessage;
                };
                Invoke(action);
                Thread.Sleep(50);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Console.Write("Введите свое имя:");
            string userName = textBox1.Text;//Console.ReadLine();
            try
            {
                Console.Write("Введите свое имя:");
                TcpClient client = null;
                try
                {
                    client = new TcpClient(address, port);
                    NetworkStream stream = client.GetStream();

                    while (true)
                    {
                        Console.Write(userName + ": ");
                        // ввод сообщения
                        
                        string message = textBox2.Text;// Console.ReadLine();
                        lastMessage = message;
                        message = String.Format("{0}: {1}", userName, message);
                            // преобразуем сообщение в массив байтов
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        // отправка сообщения
                        if(lastMessage != textBox2.Text)
                        { 
                            stream.Write(data, 0, data.Length);
                        }

                        // получаем ответ
                        data = new byte[64]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        message = builder.ToString();
                        Console.WriteLine("Сервер: {0}", message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;//Console.ReadLine();
            try
            {
                client = new TcpClient(address, port);
                stream = client.GetStream();
                //Console.Write(userName + ": ");
                // ввод сообщения
                string message = textBox2.Text;// Console.ReadLine();
                message = String.Format("{0}: {1}", userName, message);
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(message);
                // отправка сообщения
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
