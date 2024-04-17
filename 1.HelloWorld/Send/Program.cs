using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory {HostName = "localhost"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
    durable:false,//RabbitMQ kapandığında hayatta kal
    exclusive:false,//Özel kuyruk oluştur
    autoDelete: false,//Bağlı consume yoksa kuyruğu sil
    arguments:null//Özel Argümanlar
    );

const string message = "Hello world!";
var body = Encoding.UTF8.GetBytes( message );

channel.BasicPublish(
    exchange: string.Empty,//direct topic fonut ve headers
    routingKey:"hello",//Bağlanacağımız kuyruğun adı
    basicProperties: null,//temel özellikler
    body: body);//göndereceğimiz mesaj

Console.WriteLine($"[x] send {message}");

Console.WriteLine($"Press [enter] to exit");

Console.ReadLine();