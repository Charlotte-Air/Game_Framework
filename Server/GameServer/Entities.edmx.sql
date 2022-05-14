
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/18/2022 22:43:05
-- Generated from EDMX file: X:\Charlotte-MMO\Src\Server\GameServer\GameServer\Entities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ExtremeWorld];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserPlayer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserPlayer];
GO
IF OBJECT_ID(N'[dbo].[FK_PlayerCharacter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Characters] DROP CONSTRAINT [FK_PlayerCharacter];
GO
IF OBJECT_ID(N'[dbo].[FK_CharacterItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TCharacterItems] DROP CONSTRAINT [FK_CharacterItem];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTCharacterBag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Characters] DROP CONSTRAINT [FK_TCharacterTCharacterBag];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTCharacterQuest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TCharacterQuests] DROP CONSTRAINT [FK_TCharacterTCharacterQuest];
GO
IF OBJECT_ID(N'[dbo].[FK_TCharacterTCharacterFriend]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TCharacterFriends] DROP CONSTRAINT [FK_TCharacterTCharacterFriend];
GO
IF OBJECT_ID(N'[dbo].[FK_TGuildTGuildMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GuildMembers] DROP CONSTRAINT [FK_TGuildTGuildMember];
GO
IF OBJECT_ID(N'[dbo].[FK_TGuildTGuildApply]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GuildApplies] DROP CONSTRAINT [FK_TGuildTGuildApply];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Players]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Players];
GO
IF OBJECT_ID(N'[dbo].[Characters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Characters];
GO
IF OBJECT_ID(N'[dbo].[TCharacterItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TCharacterItems];
GO
IF OBJECT_ID(N'[dbo].[TCharacterBags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TCharacterBags];
GO
IF OBJECT_ID(N'[dbo].[TCharacterQuests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TCharacterQuests];
GO
IF OBJECT_ID(N'[dbo].[TCharacterFriends]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TCharacterFriends];
GO
IF OBJECT_ID(N'[dbo].[Guilds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Guilds];
GO
IF OBJECT_ID(N'[dbo].[GuildMembers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GuildMembers];
GO
IF OBJECT_ID(N'[dbo].[GuildApplies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GuildApplies];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [RegisterDate] datetime  NULL,
    [Player_ID] int  NOT NULL
);
GO

-- Creating table 'Players'
CREATE TABLE [dbo].[Players] (
    [ID] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Characters'
CREATE TABLE [dbo].[Characters] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TID] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Class] int  NOT NULL,
    [MapID] tinyint  NOT NULL,
    [MapPosX] int  NOT NULL,
    [MapPosY] int  NOT NULL,
    [MapPosZ] int  NOT NULL,
    [Gold] bigint  NOT NULL,
    [Equips] binary(28)  NOT NULL,
    [Level] int  NOT NULL,
    [Exp] bigint  NOT NULL,
    [GuildId] int  NOT NULL,
    [Player_ID] int  NOT NULL,
    [Bag_Id] int  NOT NULL
);
GO

-- Creating table 'TCharacterItems'
CREATE TABLE [dbo].[TCharacterItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemID] int  NOT NULL,
    [ItemCount] int  NOT NULL,
    [TCharacterID] int  NOT NULL
);
GO

-- Creating table 'TCharacterBags'
CREATE TABLE [dbo].[TCharacterBags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Items] varbinary(max)  NOT NULL,
    [Unlocked] int  NOT NULL
);
GO

-- Creating table 'TCharacterQuests'
CREATE TABLE [dbo].[TCharacterQuests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TCharacterID] int  NOT NULL,
    [QuestId] int  NOT NULL,
    [Target1] int  NOT NULL,
    [Target2] int  NOT NULL,
    [Target3] int  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'TCharacterFriends'
CREATE TABLE [dbo].[TCharacterFriends] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FriendID] int  NOT NULL,
    [FriendName] nvarchar(max)  NOT NULL,
    [Class] int  NOT NULL,
    [Level] int  NOT NULL,
    [TCharacterID] int  NOT NULL
);
GO

-- Creating table 'Guilds'
CREATE TABLE [dbo].[Guilds] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [LeaderID] int  NOT NULL,
    [LeaderName] nvarchar(20)  NOT NULL,
    [Notice] nvarchar(200)  NOT NULL,
    [CreateTime] datetime  NOT NULL
);
GO

-- Creating table 'GuildMembers'
CREATE TABLE [dbo].[GuildMembers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CharacterId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Class] int  NOT NULL,
    [Level] int  NOT NULL,
    [Title] int  NOT NULL,
    [JoinTime] datetime  NOT NULL,
    [LastTime] datetime  NOT NULL,
    [TGuildId] int  NOT NULL
);
GO

-- Creating table 'GuildApplies'
CREATE TABLE [dbo].[GuildApplies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CharacterId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Class] int  NOT NULL,
    [Level] int  NOT NULL,
    [Result] int  NOT NULL,
    [ApplyTime] datetime  NOT NULL,
    [TGuildId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Players'
ALTER TABLE [dbo].[Players]
ADD CONSTRAINT [PK_Players]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Characters'
ALTER TABLE [dbo].[Characters]
ADD CONSTRAINT [PK_Characters]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'TCharacterItems'
ALTER TABLE [dbo].[TCharacterItems]
ADD CONSTRAINT [PK_TCharacterItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TCharacterBags'
ALTER TABLE [dbo].[TCharacterBags]
ADD CONSTRAINT [PK_TCharacterBags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TCharacterQuests'
ALTER TABLE [dbo].[TCharacterQuests]
ADD CONSTRAINT [PK_TCharacterQuests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TCharacterFriends'
ALTER TABLE [dbo].[TCharacterFriends]
ADD CONSTRAINT [PK_TCharacterFriends]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Guilds'
ALTER TABLE [dbo].[Guilds]
ADD CONSTRAINT [PK_Guilds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GuildMembers'
ALTER TABLE [dbo].[GuildMembers]
ADD CONSTRAINT [PK_GuildMembers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GuildApplies'
ALTER TABLE [dbo].[GuildApplies]
ADD CONSTRAINT [PK_GuildApplies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Player_ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserPlayer]
    FOREIGN KEY ([Player_ID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserPlayer'
CREATE INDEX [IX_FK_UserPlayer]
ON [dbo].[Users]
    ([Player_ID]);
GO

-- Creating foreign key on [Player_ID] in table 'Characters'
ALTER TABLE [dbo].[Characters]
ADD CONSTRAINT [FK_PlayerCharacter]
    FOREIGN KEY ([Player_ID])
    REFERENCES [dbo].[Players]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PlayerCharacter'
CREATE INDEX [IX_FK_PlayerCharacter]
ON [dbo].[Characters]
    ([Player_ID]);
GO

-- Creating foreign key on [TCharacterID] in table 'TCharacterItems'
ALTER TABLE [dbo].[TCharacterItems]
ADD CONSTRAINT [FK_CharacterItem]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CharacterItem'
CREATE INDEX [IX_FK_CharacterItem]
ON [dbo].[TCharacterItems]
    ([TCharacterID]);
GO

-- Creating foreign key on [Bag_Id] in table 'Characters'
ALTER TABLE [dbo].[Characters]
ADD CONSTRAINT [FK_TCharacterTCharacterBag]
    FOREIGN KEY ([Bag_Id])
    REFERENCES [dbo].[TCharacterBags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTCharacterBag'
CREATE INDEX [IX_FK_TCharacterTCharacterBag]
ON [dbo].[Characters]
    ([Bag_Id]);
GO

-- Creating foreign key on [TCharacterID] in table 'TCharacterQuests'
ALTER TABLE [dbo].[TCharacterQuests]
ADD CONSTRAINT [FK_TCharacterTCharacterQuest]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTCharacterQuest'
CREATE INDEX [IX_FK_TCharacterTCharacterQuest]
ON [dbo].[TCharacterQuests]
    ([TCharacterID]);
GO

-- Creating foreign key on [TCharacterID] in table 'TCharacterFriends'
ALTER TABLE [dbo].[TCharacterFriends]
ADD CONSTRAINT [FK_TCharacterTCharacterFriend]
    FOREIGN KEY ([TCharacterID])
    REFERENCES [dbo].[Characters]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TCharacterTCharacterFriend'
CREATE INDEX [IX_FK_TCharacterTCharacterFriend]
ON [dbo].[TCharacterFriends]
    ([TCharacterID]);
GO

-- Creating foreign key on [TGuildId] in table 'GuildMembers'
ALTER TABLE [dbo].[GuildMembers]
ADD CONSTRAINT [FK_TGuildTGuildMember]
    FOREIGN KEY ([TGuildId])
    REFERENCES [dbo].[Guilds]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TGuildTGuildMember'
CREATE INDEX [IX_FK_TGuildTGuildMember]
ON [dbo].[GuildMembers]
    ([TGuildId]);
GO

-- Creating foreign key on [TGuildId] in table 'GuildApplies'
ALTER TABLE [dbo].[GuildApplies]
ADD CONSTRAINT [FK_TGuildTGuildApply]
    FOREIGN KEY ([TGuildId])
    REFERENCES [dbo].[Guilds]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TGuildTGuildApply'
CREATE INDEX [IX_FK_TGuildTGuildApply]
ON [dbo].[GuildApplies]
    ([TGuildId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------