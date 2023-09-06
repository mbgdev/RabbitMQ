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
                exchange: "topic-exchange-example",
                type: ExchangeType.Topic);


            Console.Write("Topic formatını Giriniz: ");
           
            string topic = Console.ReadLine();

            string queueName=channel.QueueDeclare().QueueName;

            channel.QueueBind(
                queue: queueName,
                exchange: "topic-exchange-example",
                routingKey: topic);

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(
                queue: queueName,
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