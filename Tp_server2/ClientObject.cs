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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

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
        public void SaveData()
        {
            server.data.Poem = Program.poem;
            server.data.Iterations++;
            string output = JsonConvert.SerializeObject(server.data);
            JSchema valid = JSchema.Parse(Program.shema);
            //output =@"{ 'name': 'Arnie Admin',  'roles': ['Developer', 'Administrator']}";
            JObject jdata = JObject.Parse(output);
            if (jdata.IsValid(valid))
            {
                using (StreamWriter sw = new StreamWriter("Last_data.json", false, System.Text.Encoding.Default))
                {
                    sw.Write(output);
                }
            }
            else
            {
                Console.WriteLine("Cannot save data");
            }    
        }
        public async void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                Program.usernames.Add(message);
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
                    server.data.Clients = Program.usernames;
                    foreach(string temp in server.data.clients)
                    {
                        Console.WriteLine(temp);
                    }
                    Out.PoemString = "Игра началась, введите первое сообщение";
                    Out.active = true;
                    Thread.Sleep(1000);
                    server.FirstMessage(JsonConvert.SerializeObject(Out));
                }
                //server.data.clients.Add(userName);
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
                        //SaveData();
                        await Task.Run(() => SaveData());
                        Console.WriteLine(message);
                        if (Program.MessageCount < 10)
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
