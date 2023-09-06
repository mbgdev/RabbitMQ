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
                exchange: "headers-exchange-example", 
                type: ExchangeType.Headers);

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(2000);
                byte[] message =Encoding.UTF8.GetBytes($"Merhaba {i}");

                Console.Write("Headers value Giriniz: ");
                string headersValue=Console.ReadLine();
              
                IBasicProperties properties=  channel.CreateBasicProperties();

                properties.Headers = new Dictionary<string,object>
                {
                    ["Lenovo"] = headersValue
                };

                channel.BasicPublish(
                    exchange: "headers-exchange-example",
                    routingKey: string.Empty,
                    body: message,
                    basicProperties:properties);
            }


            Console.ReadKey();

        }
    }
}