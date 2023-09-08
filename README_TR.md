## Türkçe Dokümantasyon (Turkish Documentation)
[Türkçe README](./README_TR.md)
## English Documentation (İngilizce Dokümantasyon)
[English README](./README.md)

# RabbitMQ

# İçindekiler

- [RabbitMQ ](#rabbitmq )
- [Akilli Kuyruk Mimarisi](#akilli-kuyruk-mimarisi)
- [autoAck](#autoack)
- [BasicAck](#basicack)
- [BasicNack](#basicnack)
- [BasicCancel](#basiccancel)
- [BasicReject](#basicreject)
- [Message Durability](#message-durability)
- [BasicQos](#basicqos)
- [Direct Exchange](#directexchange)




## RabbitMQ 
**RabbitMQ**, açık kaynaklı bir mesaj sıralama yazılımıdır. Verilerin güvenilir bir şekilde iletilmesini sağlayan bir iletişim aracıdır ve dağıtık sistemler arasında kullanılır.

**RabbitMQ**'yi kullanmalıyız çünkü:

1. **Veri İletişimi**: RabbitMQ, uygulama bileşenleri arasında güvenilir ve verimli bir şekilde veri iletişimi sağlar.

2. **Dağıtık Sistemler**: Dağıtık sistemler arasında veri aktarımı gerektiren senaryolarda kullanışlıdır.

3. **Mesaj Sıralama**: Mesajları sıralayarak ve işleyerek kaybolmasını önler, belirli bir sıraya göre işlenmelerini sağlar.

4. **Protokol Çeşitliliği**: Farklı platformlar ve diller arasında iletişim kurmanıza olanak tanır, çeşitli iletişim protokollerini destekler.

5. **Esneklik**: Karmaşık veri akışlarını özelleştirebilir ve yönlendirebilirsiniz.

6. **Yüksek Erişilebilirlik**: Arızalara karşı yedekleme ve yük dengeleme stratejilerini destekler.

7. **Topluluk Desteği**: Geniş bir açık kaynak topluluğu ve dökümantasyon desteği vardır.

## Akilli Kuyruk Mimarisi
 **RabbitMQ'nun "Akıllı Kuyruk Mimarisi,"** mesaj sıralama işlemlerini optimize etmek ve veri yönetimini düzenlemek için kullanılan bir yaklaşımdır. Bu yaklaşım, mesajların güvenli bir şekilde iletilmesini, belirli kurallara veya önceliklere göre işlenmesini ve yüksek erişilebilirlik sağlanmasını mümkün kılar. RabbitMQ, dağıtık sistemlerde veri iletişimini düzenlemek ve iş akışlarını yönetmek için kullanılan bu "akıllı kuyruk" mantığı ile oldukça esnek bir çözüm sunar.


**RabbitMQ Round-Robin Dispatching**, RabbitMQ'da kullanılan bir mesaj dağıtım yöntemidir. Bu yöntemde, gelen mesajlar eşit bir şekilde ve sırayla farklı tüketici işlemcilere yönlendirilir. Yani her mesaj, tüketici işlemcilere sırayla dağıtılır.

**Message Acknowledgment (Mesaj Onaylama)**, RabbitMQ'da bir tüketici işlemcinin bir mesajı aldığını ve başarılı bir şekilde işlediğini bildirmek için kullanılır. Bu işlem, mesajın tekrarlanmasını önlemek ve veri güvenliğini artırmak için önemlidir. Tüketici işlemci mesajı aldığında, onay (acknowledgment) göndererek RabbitMQ'ya mesajın işlendiğini belirtir. Eğer işlem başarısız olursa, mesaj işlenmedi olarak işaretlenebilir ve yeniden işlenebilir. RabbitMQ default olarak tüketiciye gönderdiği mesajı başarılı bir şekilde işlensin veya işlenmesin hemen kuyruktan silinmesi üzere işaretler

## autoAck
`autoAck` (Auto Acknowledgment), RabbitMQ'da bir mesajın tüketici işlemci tarafından alındıktan sonra otomatik olarak onaylandığını veya işlendiğini belirleyen bir parametredir. `autoAck: true` olarak ayarlandığında, tüketici bir mesajı aldığında bu mesaj otomatik olarak onaylanır ve kuyruktan silinir. Bu, mesajın başarılı bir şekilde işlendiği ve tekrarlanmayacağı anlamına gelir. Ancak bu yöntem, hatalı işlemler veya işlem sırasında beklenmeyen sorunlar nedeniyle veri kaybına yol açabilir.

İşte `autoAck: true` parametresinin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk oluşturma
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: true);

            // Tüketiciyi tanımlama ve kuyruğu dinlemeye başlama
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Mesaj işleme kodları burada yer alır.

                // `autoAck: true` olarak ayarlandığında, mesaj otomatik olarak onaylanır ve kuyruktan silinir.

                Console.WriteLine($"Message received and auto-acknowledged: {message}");
            };

            // Tüketiciyi başlatma (`autoAck: true` kullanılıyor)
            channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}
```
## BasicAck
**`channel.BasicAck`** yöntemi, RabbitMQ'da bir mesajın başarıyla işlendiğini onaylamak için kullanılır. İşte bu yöntemin parametreleri:

1. `deliveryTag` (ulong): Mesajın benzersiz teslimat etiketini temsil eder. Bu etiket, her mesaj için RabbitMQ tarafından otomatik olarak atanır.

2. `multiple` (bool): Bu parametre, birden fazla mesajı kapsayıp kapsamayacağını belirler. `false` olarak ayarlandığında, sadece belirtilen `deliveryTag` değerine sahip mesajı işler. `true` olarak ayarlandığında, belirtilen `deliveryTag` değerine sahip mesajı işlerken daha küçük olan tüm önceki mesajları da otomatik olarak işler.

İşte **BasicAck** yönteminin kullanıldığı bir C# örneği:
```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
              //Message Acknowledgment yapılanması için autoAck=false olmalıdır.
             channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Mesajı başarıyla işledikten sonra onaylama (acknowledgment) gönder
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                Console.WriteLine($"Received and acknowledged: {message}");
            };

            channel.BasicConsume(queue: "message_queue", autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}
```

## BasicNack

**`channel.BasicNack`**, RabbitMQ'da bir veya birden fazla mesajın işlenemediğini veya reddedildiğini belirten bir yöntemdir. İşte bu yöntemin parametreleri:

- `deliveryTag` (ulong): Bu, işlenmeyen veya reddedilen mesajın teslimat etiketini temsil eder.

- `multiple` (bool): Bu parametre, birden fazla mesajı kapsayıp kapsamayacağını belirler. `false` olarak ayarlandığında, yalnızca belirtilen `deliveryTag` değerine sahip mesaj reddedilir veya işlenmez. `true` olarak ayarlandığında, belirtilen `deliveryTag` değerine sahip mesajı reddederken daha küçük olan tüm önceki mesajlar da otomatik olarak reddedilir veya işlenmez.
- `channel.BasicNack` ile mesajları reddederiz ve gerektiğinde yeniden sıraya alabiliriz `(requeue: true)`.

İşte `BasicNack` yönteminin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //Message Acknowledgment yapılanması için autoAck=false olmalıdır.
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Mesajı başarıyla işlemediğimizi ve geri almak istediğimizi belirtmek için BasicNack kullanımı
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);

                Console.WriteLine($"Message rejected and requeued: {message}");
            };

            channel.BasicConsume(queue: "message_queue", autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}

```

## BasicCancel

**`channel.BasicCancel`**, RabbitMQ'da bir tüketiciyi kuyruktan kaldırmak ve o tüketiciye ait mesajların alımını durdurmak için kullanılır. İşte bu yöntemin bazı önemli parametreleri:

- `consumerTag` (string): Kuyruğu dinleyen tüketiciye ait benzersiz bir etiketi temsil eder. Bu etiket ile tüketiciyi tanımlarsınız.

- `noWait` (bool): Bu parametre, işlemin hemen tamamlanmasını veya beklemesini belirler. `true` olarak ayarlandığında, işlem hemen gerçekleştirilir. `false` olarak ayarlandığında, işlem tamamlandığında bir yanıt alırsınız.


İşte `BasicCancel` yönteminin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk oluşturma
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            // Tüketiciyi tanımlama ve kuyruğu dinlemeye başlama
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            };

            // Tüketiciyi başlatma
            string consumerTag = channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);

            // Tüketiciyi iptal etme (kuyruktan kaldırma)
            channel.BasicCancel(consumerTag);

            Console.WriteLine("Consumer canceled and removed from the queue.");
        }
    }
}
```
## BasicReject
**`channel.BasicReject`**, RabbitMQ'da bir mesajı işlemeyi reddetmek ve bu mesajı yeniden sıraya almak için kullanılır. İşte bu yöntemin bazı önemli parametreleri:

- `deliveryTag` (ulong): Bu, reddedilen mesajın teslimat etiketini temsil eder. Teslimat etiketi, her mesaja benzersiz bir şekilde atanır.

- `requeue` (bool): Bu parametre, reddedilen mesajın yeniden sıraya alınıp alınmayacağını belirler. `true` olarak ayarlandığında, mesaj yeniden sıraya alınır. `false` olarak ayarlandığında, mesaj kalıcı olarak kuyruktan çıkarılır.
- `channel.BasicReject` ile bir mesajı işlemeyi reddeder ve isteğe bağlı olarak yeniden sıraya alabilirsiniz (requeue: true).

İşte `BasicReject` yönteminin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk oluşturma
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            // Tüketiciyi tanımlama ve kuyruğu dinlemeye başlama
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Mesajı reddetme ve yeniden sıraya alma
                channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);

                Console.WriteLine($"Message rejected and requeued: {message}");
            };

            // Tüketiciyi başlatma
            channel.BasicConsume(queue: "message_queue", autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}
```
## Message Durability
**Message Durability** (Mesaj Dayanıklılığı), RabbitMQ'da mesajların kalıcı olmasını ve veri kaybını önlemeyi sağlayan bir özelliktir. Dayanıklı bir mesaj, RabbitMQ tarafından veritabanına veya depolama alanına yazılarak kalıcı hale getirilir. Bu, sunucu veya kuyruk arızaları durumunda mesajların kaybolmasını engeller ve kritik verilerin güvenli bir şekilde iletilmesini sağlar.

`durable: true` ile kuyruğu ve `properties.Persistent = true` ile mesajı dayanıklı hale getirirsiniz.

İşte mesaj dayanıklılığını kullanarak bir mesajı dayanıklı olarak işaretlemek için kullanılan bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk oluşturma ve dayanıklı olarak işaretleme
            channel.QueueDeclare(queue: "durable_message_queue", durable: true, exclusive: false, autoAck: false);

            string message = "Bu mesaj dayanıklıdır.";

            var body = Encoding.UTF8.GetBytes(message);

            // Mesajı dayanıklı olarak gönderme
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Mesajı dayanıklı hale getirme

            channel.BasicPublish(exchange: "", routingKey: "durable_message_queue", basicProperties: properties, body: body);

            Console.WriteLine("Durable message sent.");
        }
    }
}
```
## BasicQos
**`channel.BasicQos`**, RabbitMQ'da tüketici işlemcinin bir seferde kaç mesajı işleyeceğini ve işleme alacağını belirlemek için kullanılan bir yöntemdir. Bu yöntem, verimliliği artırmak ve kaynak kullanımını optimize etmek amacıyla kullanılır.

İşte **`channel.BasicQos`** yönteminin bazı önemli parametreleri:

- `prefetchSize` (uint): Genellikle 0 olarak ayarlanır ve kullanılmaz.
- `prefetchCount` (ushort): Tüketici işlemcinin bir seferde kaç mesajı işleyeceğini belirler.
- `global` (bool): Varsayılan olarak `false` olarak ayarlanır. `true` olarak ayarlandığında, belirtilen ayarlar tüm kanal tüketici işlemcileri için geçerlidir.
- Bu örnekte `prefetchCount` ile tüketici işlemcinin yalnızca bir mesajı işleyeceği belirtilmiştir.

İşte `channel.BasicQos` yönteminin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk oluşturma
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: true);

            // Tüketiciyi tanımlama ve kanalın BasicQos ayarlarını yapma
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); // Sadece bir mesaj işleyeceğimizi belirtme

            consumer.Received += (model, ea) =>
            {
                // Mesaj işleme kodları burada yer alır.
            };

            // Tüketiciyi başlatma
            channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}
```

## Direct Exchange

RabbitMQ, mesaj iletişimi için kullanılan bir mesaj aracıdır ve farklı mesaj türlerini işlemek için farklı türdeki değişimler (exchange) sunar. Bunlardan biri de "direct exchange"dir.

Direct exchange, gönderilen mesajların belirli bir "routing key" (yol yönlendirme anahtarı) ile eşleştirildiği ve yalnızca bu eşleşen kuyruklara iletilmesini sağlayan bir türdür. Yani, mesajlarınızın sadece belirli alıcılarına gitmesini istediğinizde direct exchange kullanılır.

Direct exchange kullanırken şunları yapabilirsiniz:
- Bir veya daha fazla kuyruğu exchange ile bağlayabilirsiniz.
- Kuyruklar, belirtilen routing key ile eşleşen mesajları alır.
- Birden fazla routing key ile birden fazla kuyruğa mesaj gönderebilirsiniz.

Özetle, direct exchange, mesajları doğrudan belirli alıcılarına yönlendirmek için kullanılır ve belirli bir routing key ile eşleşen kuyruklara iletilir.

İşte `ExchangeType.Direct` kullanıldığı bir C# örneği:
```csharp
using System;
using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Direct Exchange adı
            string exchangeName = "direct_exchange";

            // Exchange oluştur
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            // Kuyruk adı
            string queueName = "direct_queue";

            // Kuyruk oluştur
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false,
                                );

            // Routing anahtarları (binding key'leri)
            string[] routingKeys = { "error", "warning", "info" };

            // Routing anahtarlarıyla Exchange'i kuyruğa bağla
            foreach (var routingKey in routingKeys)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: routingKey);
            }

            Console.WriteLine("Mesaj göndermek için bir tuşa basın.");
            Console.ReadKey();

            // Mesaj gönder
            string message = "Bu bir test mesajıdır.";
            var body = Encoding.UTF8.GetBytes(message);

            // Örneğin, "error" routing key'i ile mesaj gönder
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "error",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine(" [x] Gönderilen '{0}':'{1}'", "error", message);
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}

```









