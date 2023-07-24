using RabbitMQ.Client;
using System.Reflection;
using System.Text;

namespace RabbitMQ.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory= new ConnectionFactory();
            factory.Uri = new Uri("amqps://yphmwybv:ai2I1SKf8nZWx2iKpr64yQISh2sEBK_r@goose.rmq2.cloudamqp.com/yphmwybv");


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue",true,false,false);

            Enumerable.Range(1, 50).ToList().ForEach(Range =>
            {
                string message = $"Selamun Aleykum{Range} " + DateTime.Now.ToLongTimeString();

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                Console.WriteLine($"Mesaj Gönderilmiştir. {message} " );
            });
         

            Console.ReadKey();





        }
    }
}