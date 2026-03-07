using Catalog.Application.Events;

namespace Catalog.Application.Interfaces.Events;

public interface IOrderPlacedEventPublisher
{
    Task PublishAsync(OrderPlacedEvent orderPlacedEvent, CancellationToken ct);
}
