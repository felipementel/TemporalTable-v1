using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.EntityFrameworkCore;
using PoC.TempTables.EF.Domain;
using PoC.TempTables.EF.Infra.Database;

namespace PoC.TempTables.EF
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int qtdContratos = 3;
            int qtdPessoas = 2;

            var contratoFaker = new Bogus.Faker<Contrato>(locale: "pt_BR")
                           .CustomInstantiator(f =>
                           {
                               return new Contrato(
                                   id: f.Random.Guid(),
                                   createdAt: DateTime.Now.ToUniversalTime(),
                                   updatedAt: DateTime.Now.ToUniversalTime(),
                                   numero: f.Random.Long(int.MinValue, int.MaxValue),
                                   dataInicio: f.Date.RecentDateOnly(),
                                   dataFim: f.Date.FutureDateOnly(),
                                   ativo: f.Random.Bool()
                               );
                           }).FinishWith((f, u) =>
                           {
                               Console.WriteLine("Contrato criado com id {0}", u.Id);
                           })
                           .Generate(qtdContratos);

            var PessoaFaker =
                new Faker<Pessoa>(locale: "pt_BR").CustomInstantiator(p =>
                {
                    return new Pessoa(
                        p.Random.Guid(),
                        createdAt: DateTime.Now.ToUniversalTime(),
                        DateTime.Now.ToUniversalTime(),
                        p.Person.FullName,
                        p.Person.Email,
                        p.Person.Phone,
                        p.Person.Cpf(),
                        p.Person.Address.Street,
                        p.Date.PastDateOnly()
                    );
                }).FinishWith((f, u) =>
                {
                    Console.WriteLine("Pessoa criado com id {0}", u.Id);
                })
                .Generate(qtdPessoas);

            using DeployDbContext dbContext = new();

            dbContext.Pessoas.AddRange(PessoaFaker);
            await dbContext.SaveChangesAsync();

            dbContext.Contratos.AddRange(contratoFaker);
            await dbContext.SaveChangesAsync();

            // Live ate esse ponto

            var position = 0;

            var nextPage = dbContext.Pessoas
                .OrderBy(b => b.CreatedAt)
                .Skip(position * 10)
                .Take(10)
                .ToList();

            //Data Definition Language (DDL) - Manipula a estrutura do banco de dados
            //CREATE, ALTER, DROP, TRUNCATE

            // Data Manipulation Language (DML) - Manipula os dados
            //SELECT, INSERT, UPDATE, DELETE

              // Pessoas
            var user = new Microsoft.Data.SqlClient.SqlParameter("Nome", "CANAL DEPLOY");
            var pessoa1 = dbContext.Pessoas
                .FromSql($"EXECUTE dbo.GetMostPopularBlogsByName @filterByUser={user}")
                .ToList();


            var pessoa2 = dbContext.Pessoas
                .FromSql<Pessoa>($"SELECT TOP 1 * FROM Pessoas ORDER BY CreatedAt DESC")
                .ToList();

            var pessoa3 = await dbContext.Pessoas.FindAsync(pessoa2[0].Id);

            // update pessoa nome
            var item5 = dbContext.Pessoas.Where(p => p.Nome.Any()).ExecuteUpdate(s => s.SetProperty(p => p.Nome, "NOVO NOME"));

            var item6 = dbContext.Pessoas.Where(p => p.Nome.StartsWith("NO")).ExecuteUpdate(s => s.SetProperty(p => p.Nome, "NOVO NOME 2"));
        }
    }
}
