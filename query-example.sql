SELECT TOP 1 ContratoId FROM Contratos ORDER BY CreatedAt DESC



DECLARE @ContratoIdLocal UNIQUEIDENTIFIER;

SELECT TOP 1 @ContratoIdLocal = ContratoId FROM [dbo].[Contratos]

PRINT @ContratoIdLocal;

UPDATE
	[dbo].[Contratos]
SET
	Numero = 123
WHERE
	ContratoId = @ContratoIdLocal

-- #########

SELECT * FROM ContratosHistory


-- $$$$$$$$$$$$$$$$$$$

DECLARE @PessoaIdLocal UNIQUEIDENTIFIER;

SELECT TOP 1 @PessoaIdLocal = PessoaId FROM [dbo].[Pessoas]

PRINT @PessoaIdLocal;

UPDATE
	[dbo].[Pessoas]
SET
	Nome = 'Felipe Augusto'
WHERE
	PessoaId = @PessoaIdLocal

SELECT
	*
FROM
	[dbo].[Pessoas]
WHERE
	PessoaId = 'F561A4D9-CACC-7459-5321-2B926BEB9B4E' --@PessoaIdLocal
-- #########

SELECT * FROM PessoasHistory


--- $$$$$$$$$$$$$$$$$$
ALTER TABLE [dbo].[Contratos] SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE [dbo].[Pessoas] SET (SYSTEM_VERSIONING = OFF);

DROP TABLE [dbo].[ContratosHistory]
DROP TABLE [dbo].[HistoricoTabelaPessoa]

DROP TABLE [dbo].[Contratos]
DROP TABLE [dbo].[Pessoas]

DROP TABLE [dbo].[_ControleMigracoes]



SELECT
	*
FROM
	[dbo].[Pessoas]
FOR SYSTEM_TIME
	BETWEEN '2024-12-11 00:00:00.0000000' AND '2024-12-13 00:00:00.0000000'
 ORDER BY
	ValidFrom;