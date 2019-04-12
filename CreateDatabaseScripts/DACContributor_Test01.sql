USE [master]
GO
DROP DATABASE IF EXISTS [DACContributor_Test01]
GO
CREATE DATABASE [DACContributor_Test01]
GO

------------------------------------------------------------------

USE [DACContributor_Test01]
GO

DROP TABLE IF EXISTS [dbo].[CommonTable]

CREATE TABLE [dbo].[CommonTable]
(
	[column_01]	INT
,	[column_02]	INT
)
GO

------------------------------------------------------------------

DROP SCHEMA IF EXISTS [dev]	
DROP SCHEMA IF EXISTS [dev01]
GO

CREATE SCHEMA [dev]	
GO
CREATE SCHEMA [dev01]
GO

------------------------------------------------------------------

DROP TABLE IF EXISTS [dev].[Table01]
DROP TABLE IF EXISTS [dev01].[Table01]

CREATE TABLE [dev].[Table01]	(test_column01	INT)
CREATE TABLE [dev01].[Table01]	(test_column01	INT)
GO