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


            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

            while (true)
            {
                Console.Write("Mesaj: ");
                string message = Console.ReadLine();
                byte[] byteMessage = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                  exchange: "direct-exchange-example",
                  routingKey: "direct-queue-example",
                  body: byteMessage);

            }




            Console.ReadKey();

        }
    }
}