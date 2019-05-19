CREATE ROLE [SampleRole01_deleted]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [SampleRole01_deleted] ADD MEMBER [sys\xtomp];

