using System;
using RabbitMQ.Client;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Producer
{
    public class DirectWithConfirm
    {
        public ConnectionFactory Factory { get; }
        private readonly string exchangeName;
        IConnection conn;
        IModel channel;

        public DirectWithConfirm(ConnectionFactory factory, Dictionary<string, string> queuesAndRoutingKeys, string exchangeName)
        {
            this.Factory = factory;
            this.exchangeName = exchangeName;
            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.ConfirmSelect();

            //foreach(<string,string> item in que)
            queuesAndRoutingKeys.ToList().ForEach(item =>
            {
                channel.QueueDeclare(item.Key, false, false, false, null);
                channel.QueueBind(item.Key, exchangeName, item.Value, null);
            });

        }

        public void Send(string msg, string routingKey)
        {
            Console.WriteLine("Enviando mensagem para canal específico (Direct)");
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            try
            {
                channel.BasicPublish(this.exchangeName, routingKey, null, messageBodyBytes);
                //se nao obter confirmação em 5 segundos, dá exceção
                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~DirectWithConfirm()
        {
            channel.Close();
            conn.Close();
        }
    }
}
