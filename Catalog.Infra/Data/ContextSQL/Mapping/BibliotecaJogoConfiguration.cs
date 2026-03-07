using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;

namespace Catalog.Infra.Data.Context.Sql.Mapping;

public class BibliotecaJogoConfiguration : IEntityTypeConfiguration<BibliotecaJogo>
{
    public void Configure(EntityTypeBuilder<BibliotecaJogo> builder)
    {
        builder.ToTable("BibliotecaJogos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Property(x => x.PayId);

        builder.Property(x => x.Status)
            .HasDefaultValue(StatusBibliotecaJogo.EmAberto)
            .IsRequired();

        builder.HasOne(x => x.Jogo)
            .WithMany(u => u.BibliotecaJogos)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Jogo)
            .WithMany(j => j.BibliotecaJogos)
            .HasForeignKey(x => x.JogoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => new { x.UserId, x.JogoId })
            .IsUnique();
    }
}
