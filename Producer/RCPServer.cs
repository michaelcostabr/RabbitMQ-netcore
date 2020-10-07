using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Producer
{
    public class RCPServer
    {
        private readonly ConnectionFactory factory;
        private readonly string queueName;

        public RCPServer(ConnectionFactory factory, string queueName)
        {
            this.factory = factory;
            this.queueName = queueName;

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                Console.WriteLine(" Aguardando chamadas RPC...");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body.ToArray();
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var nome = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" Recebido msg de ({0})", nome);
                        response = $"Olá {nome}!";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" erro: " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                Console.WriteLine(" Pressione [enter] para sair.");
                Console.ReadLine();
            }
        }
    }
}
