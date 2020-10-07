using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Producer
{
    public class DirectWithConfirmCallback
    {
        public ConnectionFactory Factory { get; }
        private readonly string exchangeName;
        IConnection conn;
        IModel channel;

        public DirectWithConfirmCallback(ConnectionFactory factory, Dictionary<string, string> queuesAndRoutingKeys, string exchangeName)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.ConfirmSelect();
            channel.BasicAcks += Channel_BasicAcks;
            channel.BasicNacks += Channel_BasicNacks;

            //foreach(<string,string> item in que)
            queuesAndRoutingKeys.ToList().ForEach(item =>
            {
                channel.QueueDeclare(item.Key, false, false, false, null);
                channel.QueueBind(item.Key, exchangeName, item.Value, null);
            });

        }

        private void Channel_BasicNacks(object sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            Console.WriteLine($"NACK recebido. DeliveryTag: {e.DeliveryTag}");
        }

        private void Channel_BasicAcks(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        {
            Console.WriteLine($"ACK recebido. DeliveryTag: {e.DeliveryTag}");
        }

        public void Send(string msg, string routingKey)
        {
            Console.WriteLine("Enviando mensagem para canal específico (Direct)");
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(this.exchangeName, routingKey, null, messageBodyBytes);
        }

        ~DirectWithConfirmCallback()
        {
            channel.Close();
            conn.Close();
        }
    }
}
