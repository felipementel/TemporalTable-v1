using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoC.TempTables.EF.Domain;

namespace PoC.TempTables.EF.Infra.Database;

public class PessoaEntityConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder
            .ToTable("Pessoas", t => t
                .IsTemporal(p => p
                    .UseHistoryTable("HistoricoTabelaPessoa")
                    .HasPeriodEnd("fim")
                    .HasColumnName("column-fim")));

        builder
            .HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasColumnName("PessoaId")
            .ValueGeneratedNever();

        builder.Property(p => p.Nome)
            .IsRequired();

        builder.Property(p => p.Email)
            .IsRequired();

        builder.Property(p => p.Telefone);

        builder.Property(p => p.Documento);

        builder.Property(p => p.Endereco);

        builder.Property(p => p.DataNascimento)
            .HasColumnType("date")
            .IsRequired();

        builder
            .Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime")
            .IsRequired();

        builder
            .Property(x => x.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime")
            .IsRequired();
    }
}