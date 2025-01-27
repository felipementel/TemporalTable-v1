using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoC.TempTables.EF.Domain;

namespace PoC.TempTables.EF.Infra.Database;

public class ContratoEntityConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder
            .ToTable("Contratos", t => t.IsTemporal(true));

        builder
            .HasKey(x => x.Id);
            
        builder
           .Property(x => x.Id)
           .HasColumnName("ContratoId")
           .ValueGeneratedNever();

        builder
          .Property(x => x.Numero)
          .HasColumnName("Numero")
          .HasColumnType("bigint")
          .IsRequired();

        builder
           .Property(x => x.DataInicio)
          .HasColumnName("DataInicio")
          .HasColumnType("date")
          .IsRequired();

        builder
          .Property(x => x.DataFim)
          .HasColumnName("DataFim")
          .HasColumnType("date")
          .IsRequired();

        builder
          .Property(x => x.Ativo)
          .HasColumnName("Ativo")
          .HasColumnType("bit")
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