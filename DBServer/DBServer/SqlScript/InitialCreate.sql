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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210220092652_InitialCreate')
BEGIN
    CREATE TABLE [GameRole] (
        [Id] uniqueidentifier NOT NULL DEFAULT (newid()),
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
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210220092652_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210220092652_InitialCreate', N'5.0.2');
END;
GO

COMMIT;
GO

