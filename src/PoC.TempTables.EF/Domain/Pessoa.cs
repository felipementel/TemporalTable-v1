namespace PoC.TempTables.EF.Domain;

public class Pessoa : BaseEntity<Guid>
{
    public Pessoa(Guid id,
        string nome,
        string email,
        string telefone,
        string documento,
        string endereco,
        DateOnly dataNascimento)
        : base(id)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        Documento = documento;
        Endereco = endereco;
        DataNascimento = dataNascimento;
    }

    public string Nome { get; private set; }

    public string Email { get; private set; }

    public string Telefone { get; init; }

    public string Documento { get; init; }

    public string Endereco { get; init; }

    public DateOnly DataNascimento { get; init; }

    public List<Contrato> Contratos { get; set; }

    public void AddContrato(Contrato contrato)
    {
        Contratos ??= new List<Contrato>();
        Contratos.Add(contrato);
    }

    public void RemoveContrato(Contrato contrato)
    {
        Contratos?.Remove(contrato);
    }

    public void SetNewName(string newName)
    {
        Nome = newName;
    }

    public void SetNewEmail(string newEmail)
    {
        Email = newEmail;
    }
}
