IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [FruitConfig] (
    [Id] nvarchar(50) NOT NULL DEFAULT (newid()),
    [FruitId] int NOT NULL,
    [Rate] int NOT NULL,
    [Name] nvarchar(50) NULL,
    [Image] nvarchar(50) NULL,
    [Score] int NOT NULL,
    [CombineFruitId] int NOT NULL,
    CONSTRAINT [PK_FruitConfig] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'合成大西瓜水果配置表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig';
SET @description = N'水果Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'FruitId';
SET @description = N'概率';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'Rate';
SET @description = N'名称';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'Name';
SET @description = N'图片';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'Image';
SET @description = N'合成可得的积分';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'Score';
SET @description = N'合成后的水果Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'FruitConfig', 'COLUMN', N'CombineFruitId';
GO

CREATE TABLE [GameBox] (
    [Id] nvarchar(50) NOT NULL,
    [RoleId] bigint NOT NULL,
    [CouponsId] nvarchar(50) NULL,
    [Amount] bigint NOT NULL,
    [CreateTime] datetime2 NOT NULL,
    [UpateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_GameBox] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'游戏宝箱表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameBox';
SET @description = N'角色Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameBox', 'COLUMN', N'RoleId';
SET @description = N'优惠券Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameBox', 'COLUMN', N'CouponsId';
SET @description = N'优惠金额';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameBox', 'COLUMN', N'Amount';
GO

CREATE TABLE [GameRole] (
    [Id] nvarchar(50) NOT NULL DEFAULT (newid()),
    [RoleId] bigint NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [NickName] nvarchar(50) NOT NULL,
    [CreateTime] datetime2 NOT NULL DEFAULT (getdate()),
    [UpateTime] datetime2 NOT NULL DEFAULT (getdate()),
    CONSTRAINT [PK_GameRole] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'游戏角色表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameRole';
SET @description = N'角色Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameRole', 'COLUMN', N'RoleId';
SET @description = N'用户Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameRole', 'COLUMN', N'UserId';
SET @description = N'昵称';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameRole', 'COLUMN', N'NickName';
GO

CREATE TABLE [GameScore] (
    [Id] nvarchar(50) NOT NULL,
    [RoleId] bigint NOT NULL,
    [Score] bigint NOT NULL,
    [CreateTime] datetime2 NOT NULL,
    [UpateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_GameScore] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'游戏积分表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameScore';
SET @description = N'角色Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameScore', 'COLUMN', N'RoleId';
SET @description = N'积分';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameScore', 'COLUMN', N'Score';
GO

CREATE TABLE [GameTruntable] (
    [Id] nvarchar(50) NOT NULL,
    [RoleId] bigint NOT NULL,
    [AwardId] nvarchar(50) NULL,
    [CreateTime] datetime2 NOT NULL,
    [UpateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_GameTruntable] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'大转盘记录表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameTruntable';
SET @description = N'角色Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameTruntable', 'COLUMN', N'RoleId';
SET @description = N'奖励Id';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'GameTruntable', 'COLUMN', N'AwardId';
GO

CREATE TABLE [TruntableConfig] (
    [Id] nvarchar(50) NOT NULL,
    [AwardDesc] nvarchar(100) NULL,
    [ImagePath] nvarchar(100) NULL,
    [Price] bigint NOT NULL,
    [Weight] int NOT NULL,
    [IsValid] bit NOT NULL,
    [CreateTime] datetime2 NOT NULL,
    [UpateTime] datetime2 NOT NULL,
    CONSTRAINT [PK_TruntableConfig] PRIMARY KEY ([Id])
);
DECLARE @defaultSchema AS sysname;
SET @defaultSchema = SCHEMA_NAME();
DECLARE @description AS sql_variant;
SET @description = N'合成大西瓜大转盘配置表';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig';
SET @description = N'奖励描述';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig', 'COLUMN', N'AwardDesc';
SET @description = N'图片路径';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig', 'COLUMN', N'ImagePath';
SET @description = N'价值（单位：分）';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig', 'COLUMN', N'Price';
SET @description = N'权重';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig', 'COLUMN', N'Weight';
SET @description = N'是否开启';
EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'TruntableConfig', 'COLUMN', N'IsValid';
GO

CREATE UNIQUE INDEX [IX_FruitConfig_FruitId] ON [FruitConfig] ([FruitId]) WHERE [FruitId] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210317064955_InitialCreate', N'5.0.3');
GO

COMMIT;
GO

