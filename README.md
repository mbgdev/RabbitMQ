# RabbitMQ

# Table of Contents

- [RabbitMQ](#rabbitmq)
- [Smart Queue Architecture](#smart-queue-architecture)
- [autoAck](#autoack)
- [BasicAck](#basicack)
- [BasicNack](#basicnack)
- [BasicCancel](#basiccancel)
- [BasicReject](#basicreject)
- [Message Durability](#message-durability)
- [BasicQos](#basicqos)

## RabbitMQ

**RabbitMQ** is an open-source message queuing software. It is a communication tool that ensures reliable delivery of data and is used in distributed systems.

We should use **RabbitMQ** because:

1. **Data Communication**: RabbitMQ provides reliable and efficient data communication between application components.

2. **Distributed Systems**: It is useful in scenarios where data transfer is required between distributed systems.

3. **Message Queuing**: It prevents message loss, ensures messages are processed in a specific order by queuing and processing them.

4. **Protocol Diversity**: It allows communication between different platforms and languages, supporting various communication protocols.

5. **Flexibility**: You can customize and route complex data flows.

6. **High Availability**: It supports backup and load balancing strategies against failures.

7. **Community Support**: It has a large open-source community and documentation support.

## Smart Queue Architecture

**RabbitMQ's "Smart Queue Architecture"** is an approach used to optimize message queuing operations and manage data. This approach enables secure message delivery, processing according to specific rules or priorities, and provides high availability. RabbitMQ offers a flexible solution for managing data communication and workflows in distributed systems using this "smart queue" logic.

**RabbitMQ Round-Robin Dispatching** is a message distribution method used in RabbitMQ. In this method, incoming messages are evenly and sequentially routed to different consumer processors. Each message is distributed to consumer processors in turn.

**Message Acknowledgment** is used in RabbitMQ to confirm that a consumer processor has received and successfully processed a message. This process is essential to prevent message reprocessing and enhance data security. When a consumer processor receives a message, it sends an acknowledgment (ack) to indicate that the message has been processed successfully. If the processing fails, the message can be marked as unprocessed and requeued. By default, RabbitMQ immediately removes a message from the queue, whether it was successfully processed or not.

## autoAck

`autoAck` (Auto Acknowledgment) is a parameter in RabbitMQ that determines whether a message is automatically acknowledged or confirmed after being received by a consumer processor. When set to `true`, the message is automatically acknowledged and removed from the queue when a consumer receives it. This means that the message is considered successfully processed and will not be requeued. However, this method can lead to data loss due to erroneous processing or unexpected issues during processing.

Here's an example using `autoAck: true` in C#:

```csharp
// C# code with autoAck: true
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
            // Declare a queue
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: true);

            // Define a consumer and start listening to the queue
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Message processing code goes here.

                // When set to autoAck: true, the message is automatically acknowledged and removed from the queue.
                
                Console.WriteLine($"Message received and auto-acknowledged: {message}");
            };

            // Start the consumer (using autoAck: true)
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

## BasicAck
The `channel.BasicAck` method is used to acknowledge that a message has been successfully processed in RabbitMQ. Here are the parameters of this method:

1. `deliveryTag` (ulong): Represents the unique delivery tag of the message, which is automatically assigned by RabbitMQ for each message.

2. `multiple` (bool): This parameter determines whether it covers multiple messages. When set to `false`, it processes only the message with the specified `deliveryTag`. When set to `true`, it also automatically processes all smaller delivery tags that precede the specified `deliveryTag`.

Here is a C# example using the `BasicAck` method:

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
              // Message Acknowledgment setup requires autoAck=false.
             channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Send acknowledgment (BasicAck) after successfully processing the message
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

## BasicCancel

## BasicCancel

The `channel.BasicCancel` method is used in RabbitMQ to remove a consumer from a queue and stop it from receiving messages. Here are some important parameters of this method:

- `consumerTag` (string): Represents a unique tag associated with the consumer listening to the queue. This tag is used to identify the consumer.

- `noWait` (bool): This parameter determines whether the operation should complete immediately or wait. When set to `true`, the operation is executed immediately. When set to `false`, you will receive a response when the operation is completed.

Here is a C# example that demonstrates the usage of the `BasicCancel` method:

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
            // Declare a queue
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            // Define the consumer and start listening to the queue
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            };

            // Start the consumer
            string consumerTag = channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);

            // Cancel the consumer (remove from the queue)
            channel.BasicCancel(consumerTag);

            Console.WriteLine("Consumer canceled and removed from the queue.");
        }
    }
}

```
## BasicReject

The `channel.BasicReject` method is used in RabbitMQ to reject processing a message and requeue it if necessary. Here are some important parameters of this method:

- `deliveryTag` (ulong): This represents the delivery tag of the rejected message. The delivery tag is unique for each message.

- `requeue` (bool): This parameter determines whether the rejected message should be requeued. When set to `true`, the message is requeued. When set to `false`, the message is permanently removed from the queue.

You can use `channel.BasicReject` to reject and optionally requeue a message (requeue: true).

Here is a C# example that demonstrates the usage of the `BasicReject` method:

```csharp
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = a ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Declare a queue
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: false);

            // Define the consumer and start listening to the queue
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Reject the message and requeue it
                channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);

                Console.WriteLine($"Message rejected and requeued: {message}");
            };

            // Start the consumer
            channel.BasicConsume(queue: "message_queue", autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}

```
## Message Durability

**Message Durability** is a feature in RabbitMQ that ensures messages are persistent, preventing data loss. A durable message is made persistent by RabbitMQ, which writes it to a database or storage. This prevents messages from being lost in the event of server or queue failures, ensuring the safe delivery of critical data.

You can make a queue durable with `durable: true` and make a message durable by setting `properties.Persistent = true`.

Here is a C# example demonstrating how to mark a message as durable using message durability:

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
            // Declare a durable queue
            channel.QueueDeclare(queue: "durable_message_queue", durable: true, exclusive: false, autoAck: false);

            string message = "This message is durable.";

            var body = Encoding.UTF8.GetBytes(message);

            // Send the message as durable
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Make the message durable

            channel.BasicPublish(exchange: "", routingKey: "durable_message_queue", basicProperties: properties, body: body);

            Console.WriteLine("Durable message sent.");
        }
    }
}

```
## BasicQos

**`channel.BasicQos`** is a method used in RabbitMQ to determine how many messages a consumer processor will handle at once and process. This method is used to increase efficiency and optimize resource usage.

Here are some important parameters of **`channel.BasicQos`**:

- `prefetchSize` (uint): Typically set to 0 and not used.
- `prefetchCount` (ushort): Specifies how many messages the consumer processor will handle at once.
- `global` (bool): By default, set to `false`. When set to `true`, the specified settings apply to all channel consumer processors.

In this example, `prefetchCount` is set to indicate that the consumer processor will handle only one message at a time.

Here is a C# example demonstrating the use of the `channel.BasicQos` method:

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
            // Declare a queue
            channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoAck: true);

            // Define the consumer and set the BasicQos settings for the channel
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); // Specify that we will handle only one message

            consumer.Received += (model, ea) =>
            {
                // Message processing code goes here.
            };

            // Start the consumer
            channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages. To exit, press CTRL+C");
            Console.ReadLine();
        }
    }
}

```





