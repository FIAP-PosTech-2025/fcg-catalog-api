using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Application.Interfaces;

public interface IBibliotecaJogoRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task AddAsync(BibliotecaJogo item, CancellationToken ct);
    Task<bool> RemoveAsync(Guid userId, Guid jogoId, CancellationToken ct);
    Task<int> RemoveAllAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<BibliotecaJogo>> GetJogosDoUsuarioAsync(Guid userId, CancellationToken ct);
    Task<bool> UpdateStatusAsync(Guid userId, Guid jogoId, Guid payId, StatusBibliotecaJogo status, CancellationToken ct);
}
