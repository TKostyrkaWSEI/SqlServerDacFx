CREATE TABLE [dbo].[DimEmployee] (
    [EmployeeKey]           INT           IDENTITY (1, 1) NOT NULL,
    [ParentEmployeeKey]     INT           NULL,
    [FirstName]             NVARCHAR (50) NOT NULL,
    [LastName]              NVARCHAR (50) NOT NULL,
    [MiddleName]            NVARCHAR (50) NULL,
    [Title]                 NVARCHAR (50) NULL,
    [HireDate]              DATE          NULL,
    [BirthDate]             DATE          NULL,
    [EmailAddress]          NVARCHAR (50) NULL,
    [Phone]                 NVARCHAR (25) NULL,
    [MaritalStatus]         NCHAR (1)     NULL,
    [EmergencyContactName]  NVARCHAR (50) NULL,
    [EmergencyContactPhone] NVARCHAR (25) NULL,
    [SalariedFlag]          BIT           NULL,
    [Gender]                NCHAR (1)     NULL,
    [PayFrequency]          TINYINT       NULL,
    [BaseRate]              MONEY         NULL,
    [VacationHours]         SMALLINT      NULL,
    [CurrentFlag]           BIT           NOT NULL,
    [SalesPersonFlag]       BIT           NOT NULL,
    [DepartmentName]        NVARCHAR (50) NULL,
    [StartDate]             DATE          NULL,
    [EndDate]               DATE          NULL,
    [Status]                NVARCHAR (50) NULL,
    [ETLLoadID]             INT           NULL,
    [LoadDate]              DATETIME      NULL,
    [UpdateDate]            DATETIME      NULL
);








GO
GRANT SELECT
    ON OBJECT::[dbo].[DimEmployee] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT INSERT
    ON OBJECT::[dbo].[DimEmployee] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT DELETE
    ON OBJECT::[dbo].[DimEmployee] TO [SampleRole00_modified]
    AS [dbo];

