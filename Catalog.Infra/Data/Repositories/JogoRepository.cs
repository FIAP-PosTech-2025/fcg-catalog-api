using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infra.Data.Context.Sql.Context;

namespace Catalog.Infra.Data.Repositories;

public class JogoRepository : IJogoRepository
{
    private readonly CatalogDbContext _db;

    public JogoRepository(CatalogDbContext db) => _db = db;

    public async Task AddAsync(Jogo jogo, CancellationToken ct)
    {
        _db.Jogos.Add(jogo);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Jogo?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Jogos.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Jogo>> GetAllAsync(CancellationToken ct)
        => await _db.Jogos.AsNoTracking()
            .OrderBy(x => x.Descricao)
            .ToListAsync(ct);

    public async Task UpdateAsync(Jogo jogo, CancellationToken ct)
    {
        _db.Jogos.Update(jogo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var jogo = await _db.Jogos.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (jogo is null) return false;

        _db.Jogos.Remove(jogo);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> ExistsByDescricaoGeneroAsync(string descricao, string genero, Guid? ignoreId, CancellationToken ct)
        => _db.Jogos.AnyAsync(x =>
            x.Descricao == descricao
            && x.Genero == genero
            && (!ignoreId.HasValue || x.Id != ignoreId.Value), ct);
}