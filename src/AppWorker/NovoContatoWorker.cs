using AppWorker.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AppWorker;

public class NovoContatoWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_HOST")!,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER")!,
            Password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS")!,
        };

        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        string queueName = "novo_contato";

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = System.Text.Encoding.UTF8.GetString(body);

            try
            {
                var novoContato = Newtonsoft.Json.JsonConvert.DeserializeObject<Contato>(message);

                await ImportarNovoContatoMongoAsync(novoContato);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Error processing message: {ex.Message}");

                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        // Mantém o worker vivo até o cancelamento, sem fechar connection/channel
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }

    private async Task ImportarNovoContatoMongoAsync(Contato? novoContato)
    {
        if (novoContato != null)
        {
            using var mongoCliente = new MongoClient(Environment.GetEnvironmentVariable("MongoConnectionString"));

            var database = mongoCliente.GetDatabase("app");

            database.CreateCollection("novo_contato");

            var collection = database.GetCollection<Contato>("novo_contato");

            await collection.InsertOneAsync(novoContato);
        }
    }

}