
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/11/2017 21:24:44
-- Generated from EDMX file: C:\Users\FLM\Source\Repos\integrador_sharp\DDS\EDM.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DDS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CuentaEmpresa]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cuentas] DROP CONSTRAINT [FK_CuentaEmpresa];
GO
IF OBJECT_ID(N'[dbo].[FK_CondicionMetodologia]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Condiciones] DROP CONSTRAINT [FK_CondicionMetodologia];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Empresas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Empresas];
GO
IF OBJECT_ID(N'[dbo].[Cuentas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cuentas];
GO
IF OBJECT_ID(N'[dbo].[Indicadores]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Indicadores];
GO
IF OBJECT_ID(N'[dbo].[Metodologias]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Metodologias];
GO
IF OBJECT_ID(N'[dbo].[Condiciones]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Condiciones];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Empresas'
CREATE TABLE [dbo].[Empresas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [nombre] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'Cuentas'
CREATE TABLE [dbo].[Cuentas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmpresaId] int  NOT NULL,
    [nombre] nvarchar(255)  NOT NULL,
    [valor] float  NOT NULL,
    [periodo] int  NOT NULL
);
GO

-- Creating table 'Indicadores'
CREATE TABLE [dbo].[Indicadores] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [nombre] nvarchar(255)  NOT NULL,
    [formula] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'Metodologias'
CREATE TABLE [dbo].[Metodologias] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [nombre] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'Condiciones'
CREATE TABLE [dbo].[Condiciones] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MetodologiaId] int  NOT NULL,
    [formula] nvarchar(255)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Empresas'
ALTER TABLE [dbo].[Empresas]
ADD CONSTRAINT [PK_Empresas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cuentas'
ALTER TABLE [dbo].[Cuentas]
ADD CONSTRAINT [PK_Cuentas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Indicadores'
ALTER TABLE [dbo].[Indicadores]
ADD CONSTRAINT [PK_Indicadores]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Metodologias'
ALTER TABLE [dbo].[Metodologias]
ADD CONSTRAINT [PK_Metodologias]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Condiciones'
ALTER TABLE [dbo].[Condiciones]
ADD CONSTRAINT [PK_Condiciones]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [EmpresaId] in table 'Cuentas'
ALTER TABLE [dbo].[Cuentas]
ADD CONSTRAINT [FK_CuentaEmpresa]
    FOREIGN KEY ([EmpresaId])
    REFERENCES [dbo].[Empresas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CuentaEmpresa'
CREATE INDEX [IX_FK_CuentaEmpresa]
ON [dbo].[Cuentas]
    ([EmpresaId]);
GO

-- Creating foreign key on [MetodologiaId] in table 'Condiciones'
ALTER TABLE [dbo].[Condiciones]
ADD CONSTRAINT [FK_CondicionMetodologia]
    FOREIGN KEY ([MetodologiaId])
    REFERENCES [dbo].[Metodologias]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CondicionMetodologia'
CREATE INDEX [IX_FK_CondicionMetodologia]
ON [dbo].[Condiciones]
    ([MetodologiaId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------