using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
namespace PP_server2
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера
        bool active = false;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                userName = message;

                message = userName + " вошел в игру";
                IncomingMessage Out = new IncomingMessage();
                Out.active = false;
                Out.PoemString = message;
                message = JsonConvert.SerializeObject(Out);
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message);
                if (Program.ClientsCount == 4)
                {
                    IncomingMessage Out1 = new IncomingMessage();
                    Out.PoemString = "Игра началась, введите первое сообщение";
                    Out.active = true;
                    Thread.Sleep(1000);
                    server.FirstMessage(JsonConvert.SerializeObject(Out));
                }
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        //message = String.Format("{0}: {1}", userName, message);
                        IncomingMessage get = JsonConvert.DeserializeObject<IncomingMessage>(message);
                        Program.poem.Add(get.PoemString);
                        Console.WriteLine(message);
                        if (Program.MessageCount < 3)
                        {
                            server.NextMessage(message, this.Id);
                        }
                        else
                        {
                            server.FinalMessage();
                        }
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.NextMessage(message, this.Id);
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
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
