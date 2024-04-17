using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "task_queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);//Eğer worker newTask'tan önce çalışırsa kuyruk bulamadığı için hata verir. QueueDeclare eğer kuyruk bulunamazsa kendi oluşturuyor.Kuyruk varsa bir şey yapmaz

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global:false);//prefetchSize => Şu boyuttaki mesajları işle. 0 değeri verirsen boyut sınırı kalkar
                                                           //prefetchCount => Aynı anda kaç tane kuyruktan mesaj çekeceğini söyler.
                                                           //true => tüm kanallarda bu ayarı kullan false => sadece bu kanalda bu ayarı kullan

Console.WriteLine("[*] waiting for message.");

var consumer  = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split(".").Length - 1;
    Thread.Sleep(dots*5000+5000);

    Console.WriteLine(" [x] Done");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);//Kuyruktan gelen mesajların deliveryTag'ı olur. Burada kuyruğa DeliveryTag bilgisini vererek silmesini sağlıyoruz.
                                                                   //Multiple true yaparsak önceden gelenleri de OK işaretler.
};


channel.BasicConsume(
    queue: "task_queue",
    autoAck:false,
    consumer:consumer);

Console.WriteLine("Press [enter] to exit");
Console.ReadLine();