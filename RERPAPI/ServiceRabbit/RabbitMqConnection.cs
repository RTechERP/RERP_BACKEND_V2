using RabbitMQ.Client;
namespace RERPAPI.Services;



public sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly IConnection _connection;

    public RabbitMqConnection(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"],
            UserName = config["RabbitMQ:User"],
            Password = config["RabbitMQ:Pass"],
        };

        _connection = factory
            .CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();
    }

    public async Task<IChannel> CreateChannelAsync()
        => await _connection.CreateChannelAsync();

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}
