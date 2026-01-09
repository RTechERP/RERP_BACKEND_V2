namespace RERPAPI.Services;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(
        T message,
         string queue = "send-email",
        CancellationToken cancellationToken = default
    );
}
