using Catalog.Application.Events;
using Catalog.Application.Interfaces;
using Catalog.Application.Interfaces.Events;

namespace Catalog.Infra.Messaging;

public class PaymentProcessedEventConsumer : IPaymentProcessedEventConsumer
{
    private readonly IBibliotecaJogoService _bibliotecaJogoService;

    public PaymentProcessedEventConsumer(IBibliotecaJogoService bibliotecaJogoService)
    {
        _bibliotecaJogoService = bibliotecaJogoService;
    }

    public Task ConsumeAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
        => _bibliotecaJogoService.ProcessPaymentProcessedEventAsync(paymentProcessedEvent, ct);
}
