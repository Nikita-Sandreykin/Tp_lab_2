using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
namespace Tp_client2
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        static bool active = false;
        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("Добро пожаловать, {0}", userName);
                SendMessage();
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
        // отправка сообщений
        static void SendMessage()
        {
            //Console.WriteLine("Введите сообщение: ");
            while (true)
            {
                if (active)
                {
                    string message = Console.ReadLine();
                    IncomingMessage Out = new IncomingMessage();
                    Out.PoemString = message;
                    Out.active = true;
                    message = JsonConvert.SerializeObject(Out);
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Ожидайте очереди для ввода...");
                    active = false;
                }
            }
        }
        // получение сообщений
        static void ReceiveMessage()
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
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    //Console.WriteLine(message);
                    IncomingMessage get = JsonConvert.DeserializeObject<IncomingMessage>(message);
                    active = get.active;
                    //Console.WriteLine(get.PoemString);//вывод сообщения
                    Console.WriteLine(get.PoemString);//вывод сообщения
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

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}
