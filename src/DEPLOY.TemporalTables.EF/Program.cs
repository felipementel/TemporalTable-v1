using Bogus;
using Bogus.Extensions.Brazil;
using DEPLOY.TemporalTables.EF.Domain;
using DEPLOY.TemporalTables.EF.Infra.Database;
using DEPLOY.TemporalTables.EF.Infra.Database.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DEPLOY.TemporalTables.EF
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var contratoFaker = new Faker<Contrato>(locale: "pt_BR")
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
                .Generate(3);

            var PessoaFaker =
                new Faker<Pessoa>(locale: "pt_BR").CustomInstantiator(p =>
                {
                    return new Pessoa(
                        p.Random.Guid(),
                        createdAt: DateTime.Now.ToUniversalTime(),
                        updatedAt: DateTime.Now.ToUniversalTime(),
                        nome: p.Person.FullName,
                        email: p.Person.Email,
                        telefone: p.Person.Phone,
                        documento: p.Person.Cpf(),
                        endereco: p.Person.Address.Street,
                        dataNascimento: p.Date.PastDateOnly()
                    );
                }).FinishWith((f, u) =>
                {
                    Console.WriteLine("Pessoa criado com id {0}", u.Id);
                })
                .Generate(2);

            using DeployDbContext dbContext = new();

            dbContext.Pessoas.AddRange(PessoaFaker);
            await dbContext.SaveChangesAsync();

            dbContext.Contratos.AddRange(contratoFaker);
            await dbContext.SaveChangesAsync();

            // Live ate esse ponto

            var position = 0;

            var nextPage = dbContext.Pessoas
                .OrderBy(b => b.Nome)
                .Skip(position * 10)
                .Take(10)
                .ToListAsync();

            //Data Definition Language (DDL) - Manipula a estrutura do banco de dados
            //CREATE, ALTER, DROP, TRUNCATE

            // Data Manipulation Language (DML) - Manipula os dados
            //SELECT, INSERT, UPDATE, DELETE

            try
            {
                var PessoaCanalDEPLOY = await dbContext.Pessoas
                .SingleAsync(product => product.Nome.Contains(PessoaFaker[1].Nome));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //O EF Core suporta vários operadores de consulta de tabela temporal:

            //TemporalAsOf:   Retorna linhas que estavam ativas (atuais) no horário UTC fornecido. 
            //                Esta é uma única linha da tabela de histórico para uma determinada chave primária.

            var PessoaTemporalAsOf = dbContext.Pessoas
            .TemporalAsOf(DateTime.UtcNow)
            .SingleOrDefaultAsync(p => p.Nome.Contains(PessoaFaker[0].Nome));


            /*TemporalAll:    Retorna todas as linhas nos dados históricos. 
                            Normalmente, são muitas linhas da tabela de histórico para uma
                            determinada chave primária.*/

            var PessoaTemporalAll = dbContext.Pessoas
            .TemporalAll()
            .Where(p => p.Nome.Contains(PessoaFaker[0].Nome));

            /*TemporalFromTo: Retorna todas as linhas que estavam ativas entre dois horários UTC fornecidos.
                            Podem ser muitas linhas da tabela de histórico para uma determinada chave primária.*/

            var PessoaTemporalFromTo = await dbContext.Pessoas
            .TemporalFromTo(DateTime.UtcNow, DateTime.UtcNow)
            .SingleAsync(product => product.Nome.Contains(PessoaFaker[0].Nome));


            /*TemporalBetween: O mesmo que TemporalFromTo, exceto que as linhas
                            incluídas se tornaram ativas no limite superior.*/

            var PessoaTemporalBetween = await dbContext.Pessoas
            .TemporalBetween(DateTime.UtcNow, DateTime.UtcNow)
            .SingleAsync(product => product.Nome.Contains(PessoaFaker[0].Nome));


            /*TemporalContainedIn: : Retorna todas as linhas que começaram a ser ativas e terminaram a ser ativas 
                            entre dois horários UTC fornecidos. Podem ser muitas linhas da tabela de histórico 
                            para uma determinada chave primária.*/

            var PessoaTemporalContainedIn = await dbContext.Pessoas
            .TemporalContainedIn(DateTime.UtcNow, DateTime.UtcNow)
            .SingleAsync(product => product.Nome.Contains("CANAL"));

            // Pessoas
            //var user = new Microsoft.Data.SqlClient.SqlParameter("Nome", "CANAL DEPLOY");
            //var pessoa1 = dbContext.Pessoas
            //    .FromSql($"EXECUTE dbo.GetMostPopularBlogsByName @filterByUser={user}")
            //    .ToList();
        }
    }
}
