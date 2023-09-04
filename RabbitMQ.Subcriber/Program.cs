using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Subcriber
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Bağlantı Oluşturma

            ConnectionFactory factory = new();

            factory.Uri = new("amqps://dulnzcgr:QsK-Ii77nB-Hfm6n7e_zCH8O2SxGpND0@sparrow.rmq.cloudamqp.com/dulnzcgr");


            //Bağlantı Aktifleştirme ve Kanal Açma

            using IConnection connection = factory.CreateConnection();

            using IModel channel = connection.CreateModel();


            //Queue Oluşturma 

            channel.QueueDeclare(queue: "example-queue", exclusive: false);//consumer ve publisher aynı yapılandırma olmalıdır

            //Queue Mesaj Okuma
            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(queue: "example-queue", autoAck: false, consumer);
            consumer.Received += (sender, e) =>
            {
                
                Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
            };

        





            Console.ReadKey();
        }

       
    }
}