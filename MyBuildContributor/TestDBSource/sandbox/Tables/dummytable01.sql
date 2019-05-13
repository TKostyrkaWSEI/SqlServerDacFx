CREATE TABLE [sandbox].[dummytable01] (
    [dummycolumn1] INT          NULL,
    [dummycolumn2] VARCHAR (30) NULL
);
GO

GRANT SELECT
    ON OBJECT::[sandbox].[dummytable01] TO [SampleRole00_modified]
    AS [dbo];
GO

GRANT INSERT
    ON OBJECT::[sandbox].[dummytable01] TO [SampleRole00_modified]
    AS [dbo];
GO

GRANT DELETE
    ON OBJECT::[sandbox].[dummytable01] TO [SampleRole00_modified]
    AS [dbo];

