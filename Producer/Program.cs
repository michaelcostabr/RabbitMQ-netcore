using System;
using System.Threading;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
                Port = 5672
            };

            factory.ClientProvidedName = "app:audit component:event-consumer";

            #region funout
            //var f = new Producer.Funout(factory, new System.Collections.Generic.List<string>() { "queue1", "queue2" }, "minha-exchange");
            //f.BroadcastSend("Oi para todos");
            #endregion funout

            #region direct
            //var queuesAndRoutingKeys = new Dictionary<string, string>();
            //queuesAndRoutingKeys.Add("queue1", "RoutingKey1");
            //queuesAndRoutingKeys.Add("queue2", "RoutingKey2");

            //var d = new Producer.Direct(factory, queuesAndRoutingKeys, "minha-exchange-direct");
            //d.Send("Mensagem para RoutingKey1", "RoutingKey1");
            //d.Send("Mensagem para RoutingKey2", "RoutingKey2");
            #endregion direct

            #region direct com confirmacao
            //var queuesAndRoutingKeys = new Dictionary<string, string>();
            //queuesAndRoutingKeys.Add("queue1", "RoutingKey1");
            //queuesAndRoutingKeys.Add("queue2", "RoutingKey2");

            //var d = new Producer.DirectWithConfirm(factory, queuesAndRoutingKeys, "minha-exchange-direct");
            //d.Send("Mensagem para RoutingKey1", "RoutingKey1");
            #endregion direct com confirmacao


            #region direct com confirmacao e callback
            //var queuesAndRoutingKeys = new Dictionary<string, string>();
            //queuesAndRoutingKeys.Add("queue1", "RoutingKey1");
            //queuesAndRoutingKeys.Add("queue2", "RoutingKey2");

            //var d = new Producer.DirectWithConfirmCallback(factory, queuesAndRoutingKeys, "minha-exchange-direct");
            //d.Send("Mensagem para RoutingKey1", "RoutingKey1");
            #endregion direct com confirmacao e callback

            #region direct multiplo
            //var d = new Producer.DirectMultiple(factory, new List<string>() { "queue1", "queue2" } , "RoutingKey2", "minha-exchange-direct-multiple");
            //d.Send("Mensagem para RoutingKey2 via Direct Multiple", "RoutingKey2");
            #endregion direct multiplo

            #region topic
            //*(star)can substitute for exactly one word.
            //# (hash) can substitute for zero or more words.
            //se nao usar # ou *, o topico funcionará exatamente como im Direct

            //var queuesAndRoutingKeys = new List<KeyValuePair<string, string>>();
            //queuesAndRoutingKeys.Add(new KeyValuePair<string, string>("cotacoes", "cotacao.*"));
            //var d = new Producer.Topic(factory, queuesAndRoutingKeys, "exchange-topic-wildcard");
            //d.Send("Mensagem A", "cotacao.usd");
            //d.Send("Mensagem B", "cotacao.brl");
            //d.Send("Mensagem C", "brl"); //esta será ignorada
            #endregion topic

            #region direct routing key
            //var queuesAndRoutingKeys = new Dictionary<string, string>();
            //queuesAndRoutingKeys.Add("queue1", "RoutingKey1");
            //queuesAndRoutingKeys.Add("queue2", "RoutingKey2");

            //var d = new Producer.Direct(factory, queuesAndRoutingKeys, "exchange-direct-routing-key");
            //d.Send("Mensagem para RoutingKey1", "RoutingKey1");
            //d.Send("Mensagem para RoutingKey2", "RoutingKey2");
            #endregion direct

            #region RPC
            var rpc = new Producer.RCPServer(factory, "RPC_func_1");
            #endregion RPC

            Console.ReadLine();
        }
    }
}
