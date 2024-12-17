IF OBJECT_ID(N'[dbo].[_ControleMigracoes]') IS NULL
BEGIN
    CREATE TABLE [dbo].[_ControleMigracoes] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK__ControleMigracoes] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN
DECLARE @db_name nvarchar(max) = DB_NAME();
EXEC(N'ALTER DATABASE [' + @db_name + '] MODIFY ( 
EDITION = ''Basic'', SERVICE_OBJECTIVE = ''Basic'' );');
END

GO

BEGIN TRANSACTION;
DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [Pessoas] (
    [PessoaId] uniqueidentifier NOT NULL,
    [Nome] VARCHAR(100) NOT NULL,
    [Email] VARCHAR(100) NOT NULL,
    [Telefone] VARCHAR(100) NOT NULL,
    [Documento] VARCHAR(100) NOT NULL,
    [Endereco] VARCHAR(100) NOT NULL,
    [DataNascimento] date NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [column-fim] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [CreatedAt] datetime NOT NULL,
    [UpdatedAt] datetime NOT NULL,
    CONSTRAINT [PK_Pessoas] PRIMARY KEY ([PessoaId]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [column-fim])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[HistoricoTabelaPessoa]))');

DECLARE @historyTableSchema sysname = SCHEMA_NAME()
EXEC(N'CREATE TABLE [Contratos] (
    [ContratoId] uniqueidentifier NOT NULL,
    [Numero] bigint NOT NULL,
    [DataInicio] date NOT NULL,
    [DataFim] date NOT NULL,
    [Ativo] bit NOT NULL,
    [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [PessoaId] uniqueidentifier NULL,
    [CreatedAt] datetime NOT NULL,
    [UpdatedAt] datetime NOT NULL,
    CONSTRAINT [PK_Contratos] PRIMARY KEY ([ContratoId]),
    CONSTRAINT [FK_Contratos_Pessoas_PessoaId] FOREIGN KEY ([PessoaId]) REFERENCES [Pessoas] ([PessoaId]),
    PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ContratosHistory]))');

CREATE INDEX [IX_Contratos_PessoaId] ON [Contratos] ([PessoaId]);

COMMIT;
GO

INSERT INTO [dbo].[_ControleMigracoes] ([MigrationId], [ProductVersion])
VALUES (N'20241216230724_version-temporal-tables', N'9.0.0');
GO

