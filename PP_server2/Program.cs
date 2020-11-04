using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PP_server2
{
    delegate void doGet();
    class Program
    {
        internal static string remoteAddress = "127.0.0.1"; // хост для отправки данных
        internal static int remotePort = 8001; // порт для отправки данных
        static List<int> ports = new List<int>();
        static List<string> poem = new List<string>();
        internal static int localPort = 8002; // локальный порт для прослушивания входящих подключений
        static doGet message = connect;
        static UdpClient receiver; // UdpClient для получения данных
        static IPEndPoint remoteIp; // адрес входящего подключения
        static int iter = 0;
        static void Main(string[] args)
        {
            try
            {
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void send()
        {
            UdpClient sender = new UdpClient();
            IPEndPoint temp = null;
            byte[] dataGet = receiver.Receive(ref temp); // получаем данные
            if (temp != null)
            {
                string JsonGet = Encoding.Unicode.GetString(dataGet);
                string poemGet = JsonConvert.DeserializeObject<string>(JsonGet);
                string JsonPost = JsonConvert.SerializeObject(poem[poem.Count - 1]);
                byte[] dataPost = Encoding.Unicode.GetBytes(JsonPost);
                sender.Send(dataPost, dataPost.Length, remoteAddress, ports[(ports.IndexOf(temp.Port)+1)%4]);
                poem.Add(poemGet);
                iter++;
            }
        }
        public static void connect()
        {

            byte[] data = receiver.Receive(ref remoteIp); // получаем данные
            
            if (ports.Contains(remoteIp.Port))
            {
                ports.Add(remoteIp.Port);
            }
            if (ports.Count == 4) message = send;
        }
        private static void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    message();
                }
            }
           
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
    }
}
