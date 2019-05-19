CREATE ROLE [SampleRole00_modified]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [SampleRole00_modified] ADD MEMBER [sys\apt];

