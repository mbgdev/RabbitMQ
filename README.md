# RabbitMQ

# İçindekiler

- [RabbitMQ ](#rabbitmq )
- [Publisher ve Consumer](#publisher-ve-consumer)
- [Akilli Kuyruk Mimarisi](#akilli-kuyruk-mimarisi)
- [autoDelete](#autodelete)
- [exclusive](#exclusive)
- [autoAck](#autoack)
- [BasicAck](#basicack)
- [BasicNack](#basicnack)
- [BasicCancel](#basiccancel)
- [BasicReject](#basicreject)
- [Message Durability](#message-durability)
- [BasicQos](#basicqos)
- [Direct Exchange](#direct-exchange)
- [Fanout Exchange](#fanout-exchange)
- [Topic Exchange](#topic-exchange)
- [Headers Exchange](#headers-exchange)
- [Request Response Tasarimi](#Request-Response-tasarimi)




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


## Publisher ve Consumer 

RabbitMQ, bir mesaj sırası işleme sistemi olarak çalışır ve mesajların iletimi ve işlenmesi için iki temel rol vardır: Publisher ve Consumer.

### Publisher (Yayıncı):

- Publisher, verileri mesajlar halinde RabbitMQ sunucusuna gönderen bir bileşeni temsil eder.
- Bu bileşen, RabbitMQ sunucusuna mesajları üretir ve iletimi gerçekleştirir.
- Yayıncılar, mesajları bir takas (exchange) noktasına gönderirler.
- Örneğin, bir uygulama tarafından oluşturulan verileri bir RabbitMQ takas noktasına gönderen bir sistemi düşünebiliriz.

### Consumer (Tüketici):

- Consumer, RabbitMQ sunucusundan mesajları alan ve bu mesajları işleyen bir bileşeni temsil eder.
- Bu bileşen, RabbitMQ kuyruklarından mesajları alır ve bu mesajları belirli bir işlemi gerçekleştirmek için kullanabilir.
- Tüketiciler, genellikle belirli bir mesaj türünü veya konusunu işlemek üzere tasarlanır.
- Örneğin, bir e-posta gönderme hizmeti, e-posta mesajlarını almak ve göndermek için bir RabbitMQ tüketici kullanabilir.

### İşbirliği:

- Publisher ve Consumer, RabbitMQ'nun güçlü mesaj iletişim sistemini kullanarak birbiriyle işbirliği yaparlar.
- Publisher, mesajları bir takas noktasına gönderir ve bu mesajlar belirli kuyruklara yönlendirilir.
- Consumer, bu kuyruklardan mesajları alır ve işler.
- Bu işbirliği, dağıtık sistemlerde ve mikroservis mimarilerinde veri iletimi ve işlenmesi için kullanılır.

RabbitMQ'da Publisher ve Consumer, mesajların güvenli ve etkili bir şekilde iletilmesini ve işlenmesini sağlayan temel bileşenlerdir. Bu iki rol, karmaşık uygulamaların ve sistemlerin veri iletimi ve işlemesi için önemlidir.



## Basic Code

### Publisher (Yayıncı)

```csharp
using System;
using RabbitMQ.Client;
using System.Text;

class Publisher
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // İletilecek mesaj
            string message = "Merhaba, RabbitMQ!";
            var body = Encoding.UTF8.GetBytes(message);

            // İletilecek kuyruk (queue) adı
            string queueName = "hello_queue";

            // Mesajı belirtilen kuyruğa gönder
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoAck: true);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

            Console.WriteLine($"Mesaj gönderildi: {message}");
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}

```


### Consumer (Tüketici)

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Consumer
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // İzlenecek kuyruk (queue) adı
            string queueName = "hello_queue";

             channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoAck: true);

            // Kuyruğu dinlemek için bir consumer oluştur
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Tüketici bekliyor...");

            // Mesajları dinle ve işle
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Mesaj alındı: {message}");
            };

            Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
            Console.ReadKey();
        }
    }
}
```

## Akilli Kuyruk Mimarisi
 **RabbitMQ'nun "Akıllı Kuyruk Mimarisi,"** mesaj sıralama işlemlerini optimize etmek ve veri yönetimini düzenlemek için kullanılan bir yaklaşımdır. Bu yaklaşım, mesajların güvenli bir şekilde iletilmesini, belirli kurallara veya önceliklere göre işlenmesini ve yüksek erişilebilirlik sağlanmasını mümkün kılar. RabbitMQ, dağıtık sistemlerde veri iletişimini düzenlemek ve iş akışlarını yönetmek için kullanılan bu "akıllı kuyruk" mantığı ile oldukça esnek bir çözüm sunar.


**RabbitMQ Round-Robin Dispatching**, RabbitMQ'da kullanılan bir mesaj dağıtım yöntemidir. Bu yöntemde, gelen mesajlar eşit bir şekilde ve sırayla farklı tüketici işlemcilere yönlendirilir. Yani her mesaj, tüketici işlemcilere sırayla dağıtılır.

**Message Acknowledgment (Mesaj Onaylama)**, RabbitMQ'da bir tüketici işlemcinin bir mesajı aldığını ve başarılı bir şekilde işlediğini bildirmek için kullanılır. Bu işlem, mesajın tekrarlanmasını önlemek ve veri güvenliğini artırmak için önemlidir. Tüketici işlemci mesajı aldığında, onay (acknowledgment) göndererek RabbitMQ'ya mesajın işlendiğini belirtir. Eğer işlem başarısız olursa, mesaj işlenmedi olarak işaretlenebilir ve yeniden işlenebilir. RabbitMQ default olarak tüketiciye gönderdiği mesajı başarılı bir şekilde işlensin veya işlenmesin hemen kuyruktan silinmesi üzere işaretler

## autoDelete

RabbitMQ'da kullanılan `autoDelete` parametresi, bir kuyruğun veya bir değişimin ne zaman otomatik olarak silineceğini belirleyen bir özelliktir. Bu parametre, kuyruğun veya değişimin ne kadar süre boyunca kullanılmadığını izler ve kullanılmadığı süre boyunca otomatik olarak silinmesine olanak tanır.

### Kuyruk (Queue) için autoDelete:

- Bir kuyruk `autoDelete` parametresi `true` olarak ayarlandığında, bu kuyruk bağlantılar tarafından kullanılmadığı süre boyunca otomatik olarak silinir.
- Kuyruk, son tüketici bağlantısı kapatıldıktan sonra kullanılmaz hale geldiğinde veya bağlantı kaynaklı bir hata nedeniyle kullanılamaz hale geldiğinde otomatik olarak silinebilir.
- Bu özellik, geçici kuyrukların veya geçici işlemlerin oluşturulması ve kullanılması için yaygın olarak kullanılır.

### Değişim (Exchange) için autoDelete:

- Bir değişim `autoDelete` parametresi `true` olarak ayarlandığında, bu değişim bağlı kuyruklar olmadığı süre boyunca otomatik olarak silinir.
- Değişim, bağlı kuyrukların sayısı sıfıra düştüğünde otomatik olarak silinebilir.
- Bu özellik, geçici değişimlerin veya dinamik değişimlerin oluşturulması ve kullanılması için yaygın olarak kullanılır.

`autoDelete` parametresi, kaynakların otomatik olarak temizlenmesi ve kullanılmayan kaynakların sistemi gereksiz yere işgal etmesini önleme amacıyla oldukça faydalıdır. Ancak, bu parametre yanlış yapılandırıldığında istenmeyen sonuçlara yol açabileceğinden dikkatle kullanılmalıdır.

Kuyruk (Queue) için  `autoDelete: true` parametresinin kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;

class QueueAutoDeleteExample
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk adı
            string queueName = "my_queue_with_auto_delete";

            // Kuyruğu otomatik olarak silmek için `autoDelete` parametresini `true` olarak ayarla
            bool autoDelete = true;

            // Kuyruğu oluştur ve `autoDelete` parametresini kullan
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: autoDelete,
                                 arguments: null);

            Console.WriteLine($"Kuyruk oluşturuldu ve `autoDelete` parametresi ayarlandı: {autoDelete}");

            // Kuyruğu kullanma veya işleme devam etme...

            // Belirli bir süre sonra, kuyruk otomatik olarak silinir (eğer `autoDelete` `true` olarak ayarlandıysa).
            Console.WriteLine("Kuyruk otomatik olarak silinmesini bekliyor...");
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}
```
 Değişim (Exchange) için  `autoDelete: true` parametresinin kullanıldığı bir C# örneği:
 
```csharp
using System;
using RabbitMQ.Client;

class ExchangeAutoDeleteExample
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Değişim adı
            string exchangeName = "my_exchange_with_auto_delete";

            // Değişimi otomatik olarak silmek için `autoDelete` parametresini `true` olarak ayarla
            bool autoDelete = true;

            // Değişimi oluştur ve `autoDelete` parametresini kullan
            channel.ExchangeDeclare(exchange: exchangeName,
                                    type: ExchangeType.Direct,
                                    durable: false,
                                    autoDelete: autoDelete,
                                    arguments: null);

            Console.WriteLine($"Değişim oluşturuldu ve `autoDelete` parametresi ayarlandı: {autoDelete}");

            // Değişimi kullanma veya işleme devam etme...

            // Belirli bir süre sonra, değişim otomatik olarak silinir (eğer `autoDelete` `true` olarak ayarlandıysa).
            Console.WriteLine("Değişim otomatik olarak silinmesini bekliyor...");
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}
```

## exclusive

RabbitMQ'da kullanılan "exclusive" parametresi, bir kuyruğun sadece bir bağlantı (connection) tarafından kullanılabilir olup olmayacağını belirler. Bu parametre, kuyruğun kapsamını ve erişimini kontrol etmek için kullanılır.

### Exclusive Kuyruk (Exclusive Queue):

- Bir kuyruk "exclusive" olarak işaretlendiğinde, yalnızca kuyruğu oluşturan bağlantı bu kuyruğa erişebilir.
- Başka herhangi bir bağlantı, aynı kuyruğa erişemez veya bu kuyruk üzerinden mesaj iletemez.
- Bu özellik, özellikle geçici ve özel kullanım kuyrukları için faydalıdır. Örneğin, bir geçici işlem kuyruğu sadece bir bağlantı tarafından kullanılmalıdır.

### Non-Exclusive Kuyruk (Non-Exclusive Queue):

- Kuyruğun "exclusive" olarak işaretlenmediği durumda, birden fazla bağlantı aynı kuyruğa erişebilir ve bu kuyruk üzerinden mesaj gönderebilir.
- Bu tür kuyruklar, birden çok tüketici veya uygulama arasında veri paylaşımına izin vermek için kullanılır.

### Kullanım Alanları:

- Exclusive kuyruklar, belirli bir bağlantıya özel geçici işlem kuyrukları oluşturmak için yaygın olarak kullanılır.
- Örneğin, bir istemci uygulama, bağlantısının özel bir geçici kuyruk üzerinden mesaj göndermesini ve bu kuyruğu sadece kendisinin kullanmasını sağlayabilir.

Exclusive parametresi, kuyrukların erişimini ve kullanımını kontrol etmek için güçlü bir araç sağlar ve özellikle çoklu bağlantılar ve uygulamalar arasında veri izolasyonu gerektiren senaryolarda kullanışlıdır.


İşte `exclusive: true` parametresinin kullanıldığı bir C# örneği:
```csharp
using System;
using RabbitMQ.Client;
using System.Text;

class ExclusiveQueueExample
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Kuyruk adı
            string queueName = "my_exclusive_queue";

            // Kuyruğu `exclusive` olarak tanımla (yalnızca bu bağlantı tarafından erişilebilir)
            bool isExclusive = true;

            // Kuyruğu oluştur
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: isExclusive,
                                 autoAck: false,
                                 arguments: null);

            // Gönderilecek mesaj
            string message = "Bu bir exclusive kuyruk örneğidir.";
            var body = Encoding.UTF8.GetBytes(message);

            // Mesajı kuyruğa gönder
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"Mesaj gönderildi: {message}");
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}
```

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

## Fanout Exchange

RabbitMQ, mesaj iletişimi için kullanılan bir mesaj aracıdır ve farklı mesaj türlerini işlemek için farklı türdeki değişimler (exchange) sunar. Bu türlerden biri "fanout exchange" olarak adlandırılır.

Fanout exchange, gönderilen bir mesajın tüm bağlı kuyruklara iletildiği bir türdür. Yani, bu exchange türü, bir mesajı alan tüm kuyruklara kopyalar.

Fanout exchange kullanırken şunları yapabilirsiniz:
- Birden fazla kuyruğu exchange ile bağlayabilirsiniz.
- Bir mesajı gönderdiğinizde, o mesajı exchange'e bağlı tüm kuyruklara iletilir.
- Fanout exchange, mesajın içeriğine veya özelliklerine bakmaksızın mesajı alan tüm kuyruklara yönlendirir.

Özetle, fanout exchange, bir mesajı birden fazla alıcıya iletmenin gerektiği senaryolarda kullanılır ve bu alıcılar arasında mesaj içeriği veya türüne bakılmaksızın eşit şekilde dağıtılır.

İşte `ExchangeType.Fanout` kullanıldığı bir C# örneği:

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
            // Fanout Exchange adı
            string exchangeName = "fanout_exchange";

            // Exchange oluştur
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

            // Kuyruk adları
            string queue1Name = "queue1";
            string queue2Name = "queue2";
            string queue3Name = "queue3";

            // Kuyrukları oluştur
            channel.QueueDeclare(queue: queue1Name,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false);
                              

            channel.QueueDeclare(queue: queue2Name,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false);
                               

            channel.QueueDeclare(queue: queue3Name,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false);

            // Exchange ile kuyrukları bağla (Fanout exchange olduğu için routing key gerekli değil)
            channel.QueueBind(queue: queue1Name, exchange: exchangeName, routingKey: "");
            channel.QueueBind(queue: queue2Name, exchange: exchangeName, routingKey: "");
            channel.QueueBind(queue: queue3Name, exchange: exchangeName, routingKey: "");

            Console.WriteLine("Mesaj göndermek için bir tuşa basın.");
            Console.ReadKey();

            // Mesaj gönder
            string message = "Bu bir test mesajıdır.";
            var body = Encoding.UTF8.GetBytes(message);

            // Fanout exchange'e mesaj gönder (Tüm kuyruklara iletilecek)
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine(" [x] Gönderilen: '{0}'", message);
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}

```
## Topic Exchange

RabbitMQ, mesaj iletişimi için kullanılan bir mesaj aracıdır ve farklı mesaj türlerini işlemek için farklı türdeki değişimler (exchange) sunar. Bu türlerden biri "topic exchange" olarak adlandırılır.

Topic exchange, gönderilen bir mesajı alıcılarına yönlendirmek için özel bir "routing key" (yol yönlendirme anahtarı) yapılandırmasına dayanır. Yani, mesajın nasıl yönlendirileceğini esnek bir şekilde tanımlamanızı sağlar.

Topic exchange kullanırken şunları yapabilirsiniz:
- Kuyrukları exchange ile bağlarken, kuyrukları belirli "routing pattern" (yol yönlendirme şablonu) veya "binding key" (bağlama anahtarı) ile eşleştirebilirsiniz.
- Routing key'ler ve binding key'ler, "." ile ayrılmış birden fazla kelime içerebilir ve "*" (tek karakteri temsil eder) veya "#" (birden fazla karakteri temsil eder) özel karakterlerini kullanarak esnek eşleştirmeler yapabilirsiniz.
- Örneğin, "logs.*" eşleştirme şablonu, "logs.error" ve "logs.info" gibi tüm routing key'lerle eşleşirken, "logs.#" eşleştirme şablonu, "logs.error", "logs.info", ve "logs.debug" gibi tüm routing key'lerle eşleşir.

Özetle, topic exchange, mesajların dinamik bir şekilde yönlendirilmesini sağlar ve belirli eşleştirme kurallarına dayanarak mesajları alıcılarına iletmek için kullanılır.

## Routing Pattern Nedir?

RabbitMQ'da topic exchange kullanılırken, mesajların kuyruklara nasıl yönlendirileceğini belirlemek için routing pattern (yol yönlendirme deseni) kullanılır. Routing pattern, mesajın routing key'i (yol yönlendirme anahtarı) ile karşılaştırılarak belirli bir kuyruğa yönlendirilip yönlendirilmeyeceğini belirler.

Routing pattern, bir veya daha fazla kelimenin "." ile ayrıldığı bir dizedir ve belirli kurallara göre tanımlanır. Aşağıda en yaygın kullanılan iki özel karakter ve örnek routing pattern'ler bulunmaktadır:

- `*` (yıldız): Tek bir kelimeyi temsil eder. Örneğin, "logs.*" deseni "logs.error" ve "logs.info" gibi tüm routing key'lerle eşleşir.
- `#` (hashtag): Birden fazla kelimeyi temsil eder. Örneğin, "logs.#" deseni "logs.error", "logs.info" ve "logs.debug" gibi tüm routing key'lerle eşleşir.

Örnek bir routing pattern: "weather.us.*" deseni, "weather.us.ny" ve "weather.us.ca" gibi routing key'lerle eşleşirken, "weather.eu.fr" gibi başka bir routing key ile eşleşmez.

Routing pattern'lar, mesajların belirli konulara (topic) veya kategorilere göre yönlendirilmesine izin verir ve esnek bir yönlendirme mekanizması sunar. Bu, farklı alıcıların belirli mesajları dinlemesini ve ilgilendikleri kategorilere göre filtrelemesini sağlar.

Özetle, RabbitMQ'da topic exchange kullanırken routing pattern'lar, mesajların belirli alıcılarına yönlendirilmesini kontrol etmek için kullanılan özel desenlerdir.


İşte `ExchangeType.Topic` kullanıldığı bir C# örneği:

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
            // Topic Exchange adı
            string exchangeName = "topic_exchange";
          
            // Exchange oluştur
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            // Kuyruk adı
            string queueName = "topic_queue";

            // Kuyruk oluştur
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false);
                                 

            // Routing pattern (binding key)
            string routingPattern = "logs.*"; // Örnek bir routing pattern

            // Kuyruğu Exchange'e belirli bir routing pattern ile bağla
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: routingPattern);

            Console.WriteLine("Mesaj göndermek için bir tuşa basın.");
            Console.ReadKey();

            // Mesaj gönder
            string message = "Bu bir test mesajıdır.";
            var body = Encoding.UTF8.GetBytes(message);

            // Belirli bir routing key ile mesaj gönder
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "logs.error", // Örnek bir routing key
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine(" [x] Gönderilen '{0}':'{1}'", "logs.error", message);
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}
```


## Headers Exchange

RabbitMQ, mesaj iletişimi için kullanılan bir mesaj aracıdır ve farklı mesaj türlerini işlemek için farklı türdeki değişimler (exchange) sunar. Bu türlerden biri "headers exchange" olarak adlandırılır.

Headers exchange, mesajların belirli özelliklere sahip olup olmadığını değerlendirerek mesajları alıcılara yönlendiren bir türdür. Yani, mesajın header (başlık) bölümündeki belirli özelliklere sahip olması gerekmektedir.

Headers exchange kullanırken şunları yapabilirsiniz:
- Bir veya daha fazla kuyruğu exchange ile bağlayabilirsiniz.
- Mesajları gönderirken, mesaj başlığındaki (header) belirli özelliklere ve değerlere dayalı olarak mesajı belirli kuyruklara iletebilirsiniz.
- Alıcılar, mesajı almadan önce mesaj başlığındaki özellikleri değerlendirir ve sadece belirli koşulları karşılayan mesajları alır.

Örnek olarak, bir mesajın "priority" (öncelik) veya "content-type" (içerik türü) gibi özelliklerini kullanarak mesajları yönlendirebilirsiniz. Ancak, headers exchange, routing key (yol yönlendirme anahtarı) yerine mesaj başlığındaki özelliklere odaklanır.

Özetle, headers exchange, mesajları belirli özelliklere dayalı olarak alıcılara yönlendirmek için kullanılır ve mesaj başlığındaki özelliklere dayalı olarak esnek bir yönlendirme sağlar.


İşte `ExchangeType.Headers` kullanıldığı bir C# örneği:

```csharp
using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Headers Exchange adı
            string exchangeName = "headers_exchange";

            // Exchange oluştur
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            // Kuyruk adı
            string queueName = "headers_queue";

            // Kuyruk oluştur
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoAck: false);
                                 

            // Headers (başlık) özellikleri ve değerleri belirle
            var headers = new Dictionary<string, object>();
            headers.Add("priority", 1); // Örnek bir özellik

            // Kuyruğu Exchange'e belirli başlık özellikleri ile bağla
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: string.Empty,
                              arguments: headers);

            Console.WriteLine("Mesaj göndermek için bir tuşa basın.");
            Console.ReadKey();

            // Mesaj gönder
            string message = "Bu bir test mesajıdır.";
            var body = Encoding.UTF8.GetBytes(message);

            // Mesaj başlığına belirli özellikleri ekle
            var messageProperties = channel.CreateBasicProperties();
            messageProperties.Headers = headers;

            // Mesajı gönder
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: string.Empty,
                                 basicProperties: messageProperties,
                                 body: body);

            Console.WriteLine(" [x] Gönderilen '{0}'", message);
        }

        Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
        Console.ReadKey();
    }
}
```



## Request/Response Tasarimi 

Request/Response tasarımı, iki taraflı bir iletişim modelini tanımlar: bir istemci (client) ve bir sunucu (server). Bu tasarım, genellikle RPC (Remote Procedure Call - Uzak Prosedür Çağrısı) olarak da adlandırılır.

1. **İstemci (Client)**:
   - İstemci, sunucuya bir istekte bulunur.
   - Bu istek bir mesaj (request) olarak sunucuya gönderilir.
   - İstemci, bu isteğin yanıtını bekler.

2. **Sunucu (Server)**:
   - Sunucu, istemcinin gönderdiği isteği alır.
   - İstekte belirtilen işlemi gerçekleştirir.
   - Yanıtı bir mesaj olarak istemciye gönderir.

3. **RabbitMQ ve Mesaj Kuyrukları**:
   - RabbitMQ, bu iletişimi sağlamak için kullanılır.
   - İstemci, bir mesajı bir kuyruğa (queue) gönderir.
   - Sunucu, bu kuyruğu dinler ve gelen istekleri işler.
   - Sunucu, işlemi tamamladığında yanıtı bir kuyruğa gönderir.
   - İstemci, bu yanıt kuyruğunu dinler ve yanıtı alır.

4. **Message Correlation (Mesaj İlişkilendirme)**:
   - İstemci ve sunucu arasındaki en önemli konseptlerden biri, isteğin ve yanıtın ilişkilendirilmesidir.
   - Her istek mesajına benzersiz bir kimlik (correlation ID) eklenir.
   - İstemci, bu kimliği kullanarak doğru yanıtı bulur.

5. **Zaman Aşımı (Timeout)**:
   - İstemci, bir isteğin yanıtını beklerken bir zaman aşımı belirler.
   - Eğer yanıt belirtilen süre içinde gelmezse, istemci işlemi sonlandırır veya tekrar deneme yapar.

Request/Response tasarımı, dağıtık sistemlerde ve mikroservis mimarilerinde sıkça kullanılır. İstemci ve sunucu arasındaki asenkron iletişim sayesinde sistemler arası bağımlılıkları azaltır ve verimli bir şekilde çalışmalarını sağlar. RabbitMQ gibi mesaj araçları, bu tür iletişim senaryolarını kolayca uygulamak için kullanılır.


## Request Response Tasarimi 

Bu örnek, RabbitMQ kullanarak C# ile Request/Response tasarımını uygular. İstemci, sunucuya bir istek gönderir ve sunucu bu isteği yanıtlar. İstemci ve sunucu arasındaki iletişim RabbitMQ kuyrukları (queues) üzerinden gerçekleşir. Bu tasarım, uzak prosedür çağrıları (RPC) ve benzeri senaryolarda yaygın olarak kullanılır.

### İstemci (Client)

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Client
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Sunucuya gönderilecek isteğin queue adı
            string requestQueueName = "rpc_queue";

            // Yanıt almak için rastgele bir correlation ID oluştur
            string correlationId = Guid.NewGuid().ToString();

            // Yanıt almak için geçici bir queue oluştur
            var replyQueueName = channel.QueueDeclare().QueueName;

            // Yanıtı almak için callback queue oluştur
            var callbackQueue = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: callbackQueue);

            // İstek mesajı
            string message = "Hello, RabbitMQ!";
            var body = Encoding.UTF8.GetBytes(message);

            // İstek mesajına correlation ID ve callback queue eklenir
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            // İstek mesajını gönder
            channel.BasicPublish(exchange: "", routingKey: requestQueueName, basicProperties: props, body: body);

            Console.WriteLine($"İstek gönderildi: {message}");

            // Yanıt beklenir
            while (true)
            {
                var ea = (BasicDeliverEventArgs)callbackQueue.Queue.Dequeue();

                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body);
                    Console.WriteLine($"Yanıt alındı: {response}");
                    break;
                }
            }
        }
    }
}
```

### Sunucu(Server)

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Server
{
    static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Sunucu için request queue oluştur
            string requestQueueName = "rpc_queue";
            channel.QueueDeclare(queue: requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // İstekleri dinlemek için bir consumer oluştur
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: requestQueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Sunucu bekliyor...");

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"İstek alındı: {message}");

                // İstekle ilgili işlemi gerçekleştir
                string response = $"Merhaba, {message}!";
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                // Yanıtı gönder
                channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: Encoding.UTF8.GetBytes(response));
            };

            Console.WriteLine("Çıkış yapmak için bir tuşa basın.");
            Console.ReadLine();
        }
    }
}
```
