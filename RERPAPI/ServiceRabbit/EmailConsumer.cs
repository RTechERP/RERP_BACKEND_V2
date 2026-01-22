using RERPAPI.Model.Entities;

namespace RERPAPI.Services;
public class EmailConsumer
    : RabbitMqConsumer<EmployeeSendEmail>
{
    protected override string QueueName => "send-email";

    public EmailConsumer(RabbitMqConnection connection)
        : base(connection)
    {
    }

    protected override Task HandleAsync(
        EmployeeSendEmail message,
        CancellationToken token)
    {
        // xử lý gửi mail
        Console.WriteLine($"Gửi email đến: {message.EmailTo}, Chủ đề: {message.Subject}");
        return Task.CompletedTask;
    }
}

