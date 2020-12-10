using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI_client
{
    static class Program
    {
        public static string userName;
        public static TcpClient client;
        public static NetworkStream stream;
        public static bool active = false;
        public static string ButtonText = "";
        public static string TextBoxString = "";
        public static string LabelText = "";
        public static bool IsButtonVisible = true;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static void Disconnect()
        {
            if (Program.stream != null)
                Program.stream.Close();//отключение потока
            if (Program.client != null)
                Program.client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
        public static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = Program.stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Program.stream.DataAvailable);
                    string message = builder.ToString();
                    IncomingMessage get = JsonConvert.DeserializeObject<IncomingMessage>(message);
                    Program.active = get.active;
                    if (Program.active)
                    {
                        IsButtonVisible = true;
                        ButtonText = "Введите строку: ";
                    }
                    TextBoxString += $"{get.PoemString}\n";
                    if (get.final)
                    {
                        Disconnect();
                        Console.WriteLine("Игра завершена"); //соединение было прервано
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
    }
}
