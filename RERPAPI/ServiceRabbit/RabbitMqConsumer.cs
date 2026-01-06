using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RERPAPI.Services;

public abstract class RabbitMqConsumer<T> : BackgroundService
{
    protected abstract string QueueName { get; }

    private readonly RabbitMqConnection _connection;

    protected RabbitMqConsumer(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            QueueName,
            true,
            false,
            false,
            cancellationToken: stoppingToken
        );

        await channel.BasicQosAsync(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var data = JsonSerializer.Deserialize<T>(json);

                await HandleAsync(data!, stoppingToken);

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch
            {
                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    false,
                    requeue: true
                );
            }
        };

        await channel.BasicConsumeAsync(
              queue: QueueName,
              autoAck: false,
              consumerTag: "",
              noLocal: false,
              exclusive: false,
              arguments: null,
              consumer: consumer,
              cancellationToken: stoppingToken
          );
    }

    protected abstract Task HandleAsync(T message, CancellationToken token);
}

