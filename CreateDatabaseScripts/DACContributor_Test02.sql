USE [master]
GO
DROP DATABASE IF EXISTS [DACContributor_Test02]
GO
CREATE DATABASE [DACContributor_Test02]
GO

------------------------------------------------------------------

USE [DACContributor_Test02]
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
DROP SCHEMA IF EXISTS [dev02]
GO

CREATE SCHEMA [dev]	
GO
CREATE SCHEMA [dev02]
GO

------------------------------------------------------------------

DROP TABLE IF EXISTS [dev].[Table02]
DROP TABLE IF EXISTS [dev02].[Table02]

CREATE TABLE [dev].[Table02]	(test_column01	INT)
CREATE TABLE [dev02].[Table02]	(test_column01	INT)
GO