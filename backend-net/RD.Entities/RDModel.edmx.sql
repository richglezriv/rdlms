
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/18/2015 18:12:25
-- Generated from EDMX file: C:\Users\rgonzalez\Documents\Visual Studio 2013\Projects\RD.LMS\backend-net\RD.Entities\RDModel.edmx
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
    [Login] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NULL,
    [IsAdmin] bit  NOT NULL,
    [IsLogged] bit  NOT NULL
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