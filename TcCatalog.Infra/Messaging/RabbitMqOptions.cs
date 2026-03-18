namespace TcCatalog.Infra.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";

    public string OrderPlacedQueue { get; init; } = "tccatalog.order.placed";
    public string PaymentProcessedQueue { get; init; } = "tccatalog.payment.processed";
}
