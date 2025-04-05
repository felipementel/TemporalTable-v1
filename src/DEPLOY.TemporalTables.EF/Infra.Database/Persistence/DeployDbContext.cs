using DEPLOY.TemporalTables.EF.Domain;
using DEPLOY.TemporalTables.EF.Infra.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DEPLOY.TemporalTables.EF.Infra.Database.Persistence;

public class DeployDbContext : DbContext
{
    public DbSet<Pessoa> Pessoas { get; set; }

    public DbSet<Contrato> Contratos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>();

        IConfigurationRoot configuration = builder.Build();

        optionsBuilder
            .UseSqlServer(configuration.GetConnectionString("DEPLOYConnection"),
                p => p.EnableRetryOnFailure(
                        maxRetryCount: 20,
                        maxRetryDelay: TimeSpan.FromSeconds(3),
                        errorNumbersToAdd: null)
            .MigrationsHistoryTable("_ControleMigracoes", "dbo"))
            .EnableSensitiveDataLogging() // habilita os parametros das instrucoes sql
            .LogTo(System.Console.WriteLine, LogLevel.Debug);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PessoaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContratoEntityConfiguration());

        MapearPropriedadesEsquecidas(modelBuilder);

        modelBuilder.HasServiceTier("Basic");
        modelBuilder.HasPerformanceLevel("Basic");
    }

    private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
    {
        foreach (var item in modelBuilder.Model.GetEntityTypes())
        {
            var prop = item.GetProperties().Where(c => c.ClrType == typeof(string));

            foreach (var itemProp in prop)
            {
                if (string.IsNullOrEmpty(itemProp.GetColumnType())
                    && !itemProp.GetMaxLength().HasValue)
                {
                    //itemProp.SetMaxLength(100);
                    itemProp.SetColumnType("VARCHAR(100)");
                }
            }
        }
    }
}

public class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity<T>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        //Table-per-hierarchy Tph
        //Table-per-type Tpt
        //Table-per-concrete-type Tpc
        //https://learn.microsoft.com/en-us/ef/core/modeling/inheritance

        builder.UseTpcMappingStrategy();
    }
}