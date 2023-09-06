using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Subcriber
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            ConnectionFactory factory = new();

            factory.Uri = new("amqps://dulnzcgr:QsK-Ii77nB-Hfm6n7e_zCH8O2SxGpND0@sparrow.rmq.cloudamqp.com/dulnzcgr");

            using IConnection connection = factory.CreateConnection();

            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                exchange: "direct-exchange-example",
                routingKey: "direct-queue-example");

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(queueName,true, consumer);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.Span);
                Console.WriteLine(message);
            };

            Console.ReadKey();
        }


    }
}