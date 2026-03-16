using System.Text;
using System.Text.Json;
using Catalog.Application.Events;
using Catalog.Application.Interfaces;
using Catalog.Application.Interfaces.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Catalog.Infra.Messaging;

public class PaymentProcessedEventConsumer : BackgroundService, IPaymentProcessedEventConsumer
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentProcessedEventConsumer> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;

    private IConnection? _connection;
    private IModel? _channel;

    public PaymentProcessedEventConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentProcessedEventConsumer> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    public Task ConsumeAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
        => ProcessMessageAsync(paymentProcessedEvent, ct);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password,
            VirtualHost = _rabbitMqOptions.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _rabbitMqOptions.PaymentProcessedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);

                if (evt is null)
                {
                    _logger.LogWarning("Mensagem de PaymentProcessedEvent inválida: {Message}", message);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                await ProcessMessageAsync(evt, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar PaymentProcessedEvent");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: _rabbitMqOptions.PaymentProcessedQueue,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Consumidor RabbitMQ iniciado para fila {Queue}", _rabbitMqOptions.PaymentProcessedQueue);

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessMessageAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var bibliotecaJogoService = scope.ServiceProvider.GetRequiredService<IBibliotecaJogoService>();

        await bibliotecaJogoService.ProcessPaymentProcessedEventAsync(paymentProcessedEvent, ct);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
