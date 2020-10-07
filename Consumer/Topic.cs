using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer
{
    public class Topic
    {
        public ConnectionFactory Factory { get; }
        private readonly string exchangeName;
        private readonly string routingKey;
        IConnection conn;
        IModel channel;

        public Topic(ConnectionFactory factory, string exchangeName, string routingKey)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            this.routingKey = routingKey;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
            var queueName = channel.QueueDeclare().QueueName;


            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

            Console.WriteLine("Será criada (caso não exista) um novo binding para pegar apenas as mensagens da routing key especificada. Vale lembrar que pegará apenas mensagens produzidas após esse bind ser criado. Aguardando mensagens...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" recebeu routing key:mensagem -> '{0}':'{1}'",
                                  routingKey,
                                  message);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Aperte [enter] para sair.");
            Console.ReadLine();
        }

        ~Topic()
        {
            channel.Close();
            conn.Close();
        }
    }
}
