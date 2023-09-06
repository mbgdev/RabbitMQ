using RabbitMQ.Client;
using System.Reflection;
using System.Text;

namespace RabbitMQ.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {

            ConnectionFactory factory = new();

            factory.Uri = new("amqps://dulnzcgr:QsK-Ii77nB-Hfm6n7e_zCH8O2SxGpND0@sparrow.rmq.cloudamqp.com/dulnzcgr");

            using IConnection connection = factory.CreateConnection();

            using IModel channel = connection.CreateModel();


            channel.ExchangeDeclare(
                exchange: "fanout-exchange-example", 
                type: ExchangeType.Fanout);

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1000);
                byte[] message =Encoding.UTF8.GetBytes($"Merhaba {i}");

                channel.BasicPublish(
                    exchange: "fanout-exchange-example",
                    routingKey: string.Empty,
                    body: message);
            }


            Console.ReadKey();

        }
    }
}