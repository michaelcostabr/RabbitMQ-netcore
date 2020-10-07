using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Rabbit!");

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = 5672
            };

            //outra forma
            //ConnectionFactory factory = new ConnectionFactory();
            //factory.Uri = "amqp://user:pass@hostName:port/vhost";

            #region topic com filtro por routing key
            //*(star)can substitute for exactly one word.
            //# (hash) can substitute for zero or more words.
            //se nao usar # ou *, o topico funcionará exatamente como im Direct
            //factory.ClientProvidedName = "app:demo component:event-consumer";
            //var topicConsumer = new Consumer.Topic(factory, "exchange-topic-wildcard", "cotacao.*");
            #endregion topic com filtro por routing key


            #region direct com filtro por routing key
            //factory.ClientProvidedName = "app:audit component:event-consumer";
            //var topicConsumer = new Consumer.Direct(factory, "exchange-direct-routing-key", "RoutingKey2");
            #endregion direct com filtro por routing key


            #region RPC
            factory.ClientProvidedName = "app:audit component:event-consumer";
            var rpcClient = new Consumer.RPCClient(factory, "RPC_func_1");

            Console.WriteLine("RPC Client");
            Task t = RPCClient.InvokeAsync(rpcClient, "Michael");
            t.Wait();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
            #endregion RPC

        }
    }
}
