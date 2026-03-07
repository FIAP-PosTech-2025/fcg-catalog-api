using Catalog.Application.Events;

namespace Catalog.Application.Interfaces.Events;

public interface IPaymentProcessedEventConsumer
{
    Task ConsumeAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct);
}
