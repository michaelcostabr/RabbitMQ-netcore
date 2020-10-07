using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Producer
{
    public class Topic
    {
        public ConnectionFactory Factory { get; }
        private readonly string exchangeName;
        IConnection conn;
        IModel channel;

        public Topic(ConnectionFactory factory, List<KeyValuePair<string,string>> queuesAndRoutingKeys, string exchangeName)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);


            //foreach(<string,string> item in que)
            queuesAndRoutingKeys.ToList().ForEach(item =>
            {
                channel.QueueDeclare(item.Key, false, false, false, null);
                channel.QueueBind(item.Key, exchangeName, item.Value, null);
            });
            
        }

        public void Send(string msg, string routingKey)
        {
            Console.WriteLine("Enviando mensagem para canal específico (Topic)");
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(this.exchangeName, routingKey, null, messageBodyBytes);
        }

        ~Topic()
        {
            channel.Close();
            conn.Close();
        }
    }
}
