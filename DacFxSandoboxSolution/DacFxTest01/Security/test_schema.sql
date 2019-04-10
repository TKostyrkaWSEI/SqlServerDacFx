CREATE SCHEMA [test_schema]
    AUTHORIZATION [dbo];




GO
GRANT SELECT
    ON SCHEMA::[test_schema] TO [test_role];


GO
GRANT INSERT
    ON SCHEMA::[test_schema] TO [test_role];


GO
GRANT EXECUTE
    ON SCHEMA::[test_schema] TO [test_role];


GO
GRANT DELETE
    ON SCHEMA::[test_schema] TO [test_role];

