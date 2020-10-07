using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Producer
{
    public class DirectMultiple
    {
        private readonly string exchangeName;
        IConnection conn;
        IModel channel;
        public ConnectionFactory Factory { get; }

        public DirectMultiple(ConnectionFactory factory, List<string> queues, string routingKey, string exchangeName)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            queues.ForEach(queue =>
            {
                channel.QueueDeclare(queue, false, false, false, null);
                channel.QueueBind(queue, exchangeName, routingKey, null);
            });
            
        }

        public void Send(string msg, string routingKey)
        {
            Console.WriteLine("Enviando mensagem para multiplos canais específicos (Direct)");
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(this.exchangeName, routingKey, null, messageBodyBytes);
        }

        ~DirectMultiple()
        {
            channel.Close();
            conn.Close();
        }
    }
}
