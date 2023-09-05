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

            //Bağlantı Oluşturma
            factory.Uri = new("amqps://dulnzcgr:QsK-Ii77nB-Hfm6n7e_zCH8O2SxGpND0@sparrow.rmq.cloudamqp.com/dulnzcgr");

            //Bağlantı Aktifleştirme ve Kanal Açma

            using IConnection connection = factory.CreateConnection();

            using IModel channel = connection.CreateModel();

            //Queue Oluşturma 
            channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);
            IBasicProperties properties = channel.CreateBasicProperties();

            //Queue Mesaj Gönderme
            //rabbittmq kuyruğa atılan mesajları byte cinsinden kabul edilir.


            for (int i = 0; i < 100; i++)
            {

                byte[] message = Encoding.UTF8.GetBytes("Merhaba C# " + i);
                channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message, basicProperties: properties);
            }







            Console.ReadKey();

        }
    }
}