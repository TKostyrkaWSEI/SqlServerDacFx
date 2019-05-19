CREATE TABLE [dbo].[DimAccount] (
    [AccountKey]          INT            IDENTITY (1, 1) NOT NULL,
    [ParentAccountKey]    INT            NULL,
    [AccountLabel]        NVARCHAR (100) NULL,
    [AccountName]         NVARCHAR (50)  NULL,
    [AccountDescription]  NVARCHAR (50)  NULL,
    [AccountType]         NVARCHAR (50)  NULL,
    [Operator]            NVARCHAR (50)  NULL,
    [CustomMembers]       NVARCHAR (300) NULL,
    [ValueType]           NVARCHAR (50)  NULL,
    [CustomMemberOptions] NVARCHAR (200) NULL,
    [ETLLoadID]           INT            NULL,
    [LoadDate]            DATETIME       NULL,
    [UpdateDate]          DATETIME       NULL,
    CONSTRAINT [PK_DimAccount_AccountKey] PRIMARY KEY CLUSTERED ([AccountKey] ASC)
);


GO
GRANT DELETE
    ON OBJECT::[dbo].[DimAccount] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT INSERT
    ON OBJECT::[dbo].[DimAccount] TO [SampleRole00_modified]
    AS [dbo];


GO
GRANT SELECT
    ON OBJECT::[dbo].[DimAccount] TO [SampleRole00_modified]
    AS [dbo];

