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
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения

        protected internal void AddConnection(ClientObject clientObject)
        {
            if (Program.ClientsCount < 4)
            {
                clients.Add(clientObject);
                Program.ClientsCount++;
            }
            
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                 clients[i].Stream.Write(data, 0, data.Length); //передача данных
            }
        }
        protected internal void NextMessage(string message, string id)
        {

            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id) // если id клиента не равно id отправляющего
                {
                    clients[(i+1)%4].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
            Program.MessageCount++;
        }
        protected internal void FirstMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            clients[0].Stream.Write(data, 0, data.Length); //передача данных
        }
        protected internal void FinalMessage()
        {
            string message = "Весь стих целиком: \n";
            foreach(string a in Program.poem)
            {
                message += a + "\n";
            }
            IncomingMessage Out = new IncomingMessage();
            Out.final = true;
            Out.PoemString = message;
            Out.active = false;
            message = JsonConvert.SerializeObject(Out);
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Stream.Write(data, 0, data.Length); //передача данных
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
