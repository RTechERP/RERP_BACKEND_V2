using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RERPAPI.Services;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly RabbitMqConnection _connection;
    public RabbitMqPublisher(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(
     
        T message,
           string queue = "send-email",
        CancellationToken cancellationToken = default)
    {
        await using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message)
        );

        var props = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}

