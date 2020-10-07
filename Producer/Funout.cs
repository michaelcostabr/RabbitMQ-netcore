using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Producer
{
    public class Funout
    {
        public ConnectionFactory Factory { get; }
        private readonly string exchangeName;
        IConnection conn;
        IModel channel;

        public Funout(ConnectionFactory factory, List<string> queues, string exchangeName)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            queues.ForEach(item =>
            {
                channel.QueueDeclare(item, false, false, false, null);
                channel.QueueBind(item, exchangeName, string.Empty, null); //fanout ignora routing key
            });
            
        }

        public void BroadcastSend(string msg)
        {
            Console.WriteLine("Enviando mensagem para todos canais (Fanout)");
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(this.exchangeName, string.Empty, null, messageBodyBytes);
        }

        ~Funout()
        {
            channel.Close();
            conn.Close();
        }
    }
}
