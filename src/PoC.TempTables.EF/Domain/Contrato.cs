namespace PoC.TempTables.EF.Domain;

public class Contrato : BaseEntity<Guid>
{
    public Contrato(Guid id,
        long numero,
        DateOnly dataInicio,
        DateOnly dataFim,
        bool ativo)
        : base(id)
    {
        Numero = numero;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Ativo = ativo;
    }

    public Contrato(Guid id)
        : base(id)
    {

    }

    public long Numero { get; init; }

    public DateOnly DataInicio { get; init; }

    public DateOnly DataFim { get; init; }

    public bool Ativo { get; init; }
}