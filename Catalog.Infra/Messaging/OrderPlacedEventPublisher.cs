using System.Text.Json;
using Catalog.Application.Events;
using Catalog.Application.Interfaces.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Infra.Messaging;

public class OrderPlacedEventPublisher : IOrderPlacedEventPublisher
{
    private readonly ILogger<OrderPlacedEventPublisher> _logger;

    public OrderPlacedEventPublisher(ILogger<OrderPlacedEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(OrderPlacedEvent orderPlacedEvent, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(orderPlacedEvent);

        _logger.LogInformation(
            "OrderPlacedEvent publicado: {Payload}",
            payload);

        return Task.CompletedTask;
    }
}
