CREATE TABLE [test_schema].[TestTable01] (
    [CustomerLabel] NVARCHAR (100) NOT NULL,
    [Title]         NVARCHAR (8)   NULL,
    [FirstName]     NVARCHAR (50)  NULL,
    [MiddleName]    NVARCHAR (50)  NULL,
    [LastName]      NVARCHAR (50)  NULL,
    [NameStyle]     BIT            NULL,
    [BirthDate]     DATE           NULL
);




GO
CREATE STATISTICS [Stats02]
    ON [test_schema].[TestTable01]([FirstName], [MiddleName], [LastName]);


GO
CREATE STATISTICS [Stats01]
    ON [test_schema].[TestTable01]([Title]);

