CREATE TABLE [dbo].[DimCustomer] (
    [CustomerKey]          INT            IDENTITY (1, 1) NOT NULL,
    [GeographyKey]         INT            NOT NULL,
    [CustomerLabel]        NVARCHAR (100) NOT NULL,
    [Title]                NVARCHAR (8)   NULL,
    [FirstName]            NVARCHAR (50)  NULL,
    [MiddleName]           NVARCHAR (50)  NULL,
    [LastName]             NVARCHAR (50)  NULL,
    [NameStyle]            BIT            NULL,
    [BirthDate]            DATE           NULL,
    [MaritalStatus]        NCHAR (1)      NULL,
    [Suffix]               NVARCHAR (10)  NULL,
    [Gender]               NVARCHAR (1)   NULL,
    [EmailAddress]         NVARCHAR (50)  NULL,
    [YearlyIncome]         MONEY          NULL,
    [TotalChildren]        TINYINT        NULL,
    [NumberChildrenAtHome] TINYINT        NULL,
    [Education]            NVARCHAR (40)  NULL,
    [Occupation]           NVARCHAR (100) NULL,
    [HouseOwnerFlag]       NCHAR (1)      NULL,
    [NumberCarsOwned]      TINYINT        NULL,
    [AddressLine1]         NVARCHAR (120) NULL,
    [AddressLine2]         NVARCHAR (120) NULL,
    [Phone]                NVARCHAR (20)  NULL,
    [DateFirstPurchase]    DATE           NULL,
    [CustomerType]         NVARCHAR (15)  NULL,
    [CompanyName]          NVARCHAR (100) NULL,
    [ETLLoadID]            INT            NULL,
    [LoadDate]             DATETIME       NULL,
    [UpdateDate]           DATETIME       NULL
);








GO
GRANT SELECT
    ON OBJECT::[dbo].[DimCustomer] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT INSERT
    ON OBJECT::[dbo].[DimCustomer] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT DELETE
    ON OBJECT::[dbo].[DimCustomer] TO [SampleRole00_modified]
    AS [dbo];

