namespace PoC.TempTables.EF.Domain;

public abstract class BaseEntity<Tid>
{
    protected BaseEntity(Tid id, DateTime? createdAt, DateTime? updatedAt)
    {
        Id = id;
        CreatedAt = createdAt ?? DateTime.Now.ToUniversalTime();
        UpdatedAt = updatedAt ?? DateTime.Now.ToUniversalTime();
    }

    protected BaseEntity(Tid id)
    {
        Id = id;
    }

    public Tid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}