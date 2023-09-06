using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Subcriber
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


            Console.Write("Kuyruk adını giriniz: ");

            string _queueName = Console.ReadLine();

            channel.QueueDeclare(
                queue: _queueName,
                exclusive: false);

            channel.QueueBind(
                queue: _queueName,
                exchange: "fanout-exchange-example",
                routingKey: string.Empty);

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
            };

            Console.ReadKey();
        }


    }
}