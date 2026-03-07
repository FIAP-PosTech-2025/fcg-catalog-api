using Catalog.Application.DTOs;
using Catalog.Application.Events;

namespace Catalog.Application.Interfaces;

public interface IBibliotecaJogoService
{
    Task AddJogoAoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task<bool> RemoveJogoDoUsuarioAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task<int> RemoveTodosJogosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<BibliotecaJogoDto>> GetJogosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task ProcessPaymentProcessedEventAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct);
}
