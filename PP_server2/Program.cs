using System;
using System.Collections.Generic;
using System.IO;
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

        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания
        public static string shema;
        public static List<string> poem = new List<string>();
        public static List<string> usernames = new List<string>();
        public static int ClientsCount = 0;
        public static int MessageCount = 0;
        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader(@"Shemajson.json", System.Text.Encoding.Default))
            {
                string str;
                while ((str= sr.ReadLine()) != null)
                {
                    shema += str + "\n";
                }
            }
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }

        }
    }
}