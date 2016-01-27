
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2015 12:29:49
-- Generated from EDMX file: C:\Users\rgonzalez\Documents\Proyectos\Reaccion Digital\rdlms\backend-net\RD.Entities\RDModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [dbLMSRD];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserUserCourse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserCourses] DROP CONSTRAINT [FK_UserUserCourse];
GO
IF OBJECT_ID(N'[dbo].[FK_CourseUserCourse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserCourses] DROP CONSTRAINT [FK_CourseUserCourse];
GO
IF OBJECT_ID(N'[dbo].[FK_ScormUserCourse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserCourses] DROP CONSTRAINT [FK_ScormUserCourse];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Courses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Courses];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UserCourses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCourses];
GO
IF OBJECT_ID(N'[dbo].[Scorms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Scorms];
GO
IF OBJECT_ID(N'[dbo].[BnnAppUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BnnAppUsers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Courses'
CREATE TABLE [dbo].[Courses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Thumbnail] nvarchar(max)  NOT NULL,
    [ScormPackage] nvarchar(max)  NOT NULL,
    [ParentCourses] nvarchar(max)  NOT NULL,
    [ScoIndex] nvarchar(max)  NULL,
    [IsEnabled] bit  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(max)  NULL,
    [Password] nvarchar(max)  NULL,
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NULL,
    [SecondLastName] nvarchar(max)  NOT NULL,
    [IsAdmin] bit  NOT NULL,
    [IsLogged] bit  NOT NULL,
    [LastLogged] datetime  NULL,
    [Email] nvarchar(max)  NOT NULL,
    [BirthDay] datetime  NULL,
    [Gender] nvarchar(1)  NOT NULL,
    [Ocupation] smallint  NOT NULL,
    [Organization] smallint  NOT NULL,
    [IsActive] bit  NOT NULL,
    [SerialSession] nvarchar(max)  NULL
);
GO

-- Creating table 'UserCourses'
CREATE TABLE [dbo].[UserCourses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [CourseId] int  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [ScormId] int  NOT NULL
);
GO

-- Creating table 'Scorms'
CREATE TABLE [dbo].[Scorms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LessonLocation] nvarchar(max)  NULL,
    [Credit] nvarchar(max)  NULL,
    [ScoreRaw] nvarchar(max)  NOT NULL,
    [ScoreMin] nvarchar(max)  NULL,
    [ScoreMax] nvarchar(max)  NULL,
    [TotalTime] nvarchar(max)  NULL,
    [SessionTime] nvarchar(max)  NULL,
    [SuspendData] nvarchar(max)  NULL,
    [LaunchData] nvarchar(max)  NULL,
    [DataMasteryScore] nvarchar(max)  NULL,
    [Entry] nvarchar(max)  NULL,
    [Exit] nvarchar(max)  NULL,
    [Version] nvarchar(max)  NULL
);
GO

-- Creating table 'BnnAppUsers'
CREATE TABLE [dbo].[BnnAppUsers] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StateId] bigint  NOT NULL,
    [FbId] varchar(500)  NULL,
    [Audience] int  NOT NULL,
    [IsInterested] int  NOT NULL,
    [UserName] varchar(200)  NOT NULL,
    [Name] varchar(100)  NOT NULL,
    [LastNames] varchar(100)  NOT NULL,
    [Mail] varchar(50)  NOT NULL,
    [Password] varchar(64)  NULL,
    [Extension] varchar(10)  NULL,
    [ChildrenCount] int  NOT NULL,
    [LocationCount] int  NOT NULL,
    [LockedAt] datetime  NULL,
    [FechaRegistro] datetime  NULL,
    [FechaAviso] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [PK_Courses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserCourses'
ALTER TABLE [dbo].[UserCourses]
ADD CONSTRAINT [PK_UserCourses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Scorms'
ALTER TABLE [dbo].[Scorms]
ADD CONSTRAINT [PK_Scorms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BnnAppUsers'
ALTER TABLE [dbo].[BnnAppUsers]
ADD CONSTRAINT [PK_BnnAppUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'UserCourses'
ALTER TABLE [dbo].[UserCourses]
ADD CONSTRAINT [FK_UserUserCourse]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserCourse'
CREATE INDEX [IX_FK_UserUserCourse]
ON [dbo].[UserCourses]
    ([UserId]);
GO

-- Creating foreign key on [CourseId] in table 'UserCourses'
ALTER TABLE [dbo].[UserCourses]
ADD CONSTRAINT [FK_CourseUserCourse]
    FOREIGN KEY ([CourseId])
    REFERENCES [dbo].[Courses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CourseUserCourse'
CREATE INDEX [IX_FK_CourseUserCourse]
ON [dbo].[UserCourses]
    ([CourseId]);
GO

-- Creating foreign key on [ScormId] in table 'UserCourses'
ALTER TABLE [dbo].[UserCourses]
ADD CONSTRAINT [FK_ScormUserCourse]
    FOREIGN KEY ([ScormId])
    REFERENCES [dbo].[Scorms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ScormUserCourse'
CREATE INDEX [IX_FK_ScormUserCourse]
ON [dbo].[UserCourses]
    ([ScormId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------