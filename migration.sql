﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    IF SCHEMA_ID(N'db_stock') IS NULL EXEC(N'CREATE SCHEMA [db_stock];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[Stocks] (
        [Id] uniqueidentifier NOT NULL,
        [Symbol] nvarchar(100) NOT NULL,
        [Name] nvarchar(300) NOT NULL,
        [Price_Value] money NOT NULL,
        [Currency] nvarchar(100) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [Change_Value] money NOT NULL,
        [TrendingScore] float NULL,
        CONSTRAINT [PK_Stocks] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [db_stock].[AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [db_stock].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[Portfolios] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(300) NOT NULL,
        [TotalValue] money NOT NULL,
        CONSTRAINT [PK_Portfolios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Portfolios_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[TimeSeries] (
        [Id] int NOT NULL IDENTITY,
        [StockId] uniqueidentifier NOT NULL,
        [Inteval] nvarchar(100) NOT NULL,
        [TimeZone] nvarchar(max) NULL,
        CONSTRAINT [PK_TimeSeries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TimeSeries_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[Trades] (
        [Id] uniqueidentifier NOT NULL,
        [StockId] uniqueidentifier NOT NULL,
        [Quantity] int NOT NULL,
        [Price_Value] money NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Type] int NOT NULL,
        CONSTRAINT [PK_Trades] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Trades_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_Trades_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[StockPosition] (
        [PortfolioId] uniqueidentifier NOT NULL,
        [Id] int NOT NULL IDENTITY,
        [StockId] uniqueidentifier NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_StockPosition] PRIMARY KEY ([PortfolioId], [Id]),
        CONSTRAINT [FK_StockPosition_Portfolios_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [db_stock].[Portfolios] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StockPosition_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE TABLE [db_stock].[StockSnapshots] (
        [Id] int NOT NULL IDENTITY,
        [Close] money NOT NULL,
        [Low] money NOT NULL,
        [High] money NOT NULL,
        [Open] money NOT NULL,
        [Datetime] datetime2 NOT NULL,
        [TimeSeriesId] int NOT NULL,
        CONSTRAINT [PK_StockSnapshots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StockSnapshots_TimeSeries_TimeSeriesId] FOREIGN KEY ([TimeSeriesId]) REFERENCES [db_stock].[TimeSeries] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [db_stock].[AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [db_stock].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [db_stock].[AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [db_stock].[AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [db_stock].[AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [EmailIndex] ON [db_stock].[AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [db_stock].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_Portfolios_UserId] ON [db_stock].[Portfolios] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_StockPosition_StockId] ON [db_stock].[StockPosition] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_StockSnapshots_TimeSeriesId] ON [db_stock].[StockSnapshots] ([TimeSeriesId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_TimeSeries_StockId] ON [db_stock].[TimeSeries] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_Trades_StockId] ON [db_stock].[Trades] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    CREATE INDEX [IX_Trades_UserId] ON [db_stock].[Trades] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230306094157_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230306094157_Initial', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230307130105_change_field_nullable')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Stocks]') AND [c].[name] = N'Change_Value');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Stocks] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [db_stock].[Stocks] ALTER COLUMN [Change_Value] money NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230307130105_change_field_nullable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230307130105_change_field_nullable', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    ALTER TABLE [db_stock].[TimeSeries] DROP CONSTRAINT [FK_TimeSeries_Stocks_StockId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    DROP INDEX [IX_TimeSeries_StockId] ON [db_stock].[TimeSeries];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[TimeSeries]') AND [c].[name] = N'StockId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[TimeSeries] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [db_stock].[TimeSeries] DROP COLUMN [StockId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    EXEC sp_rename N'[db_stock].[TimeSeries].[Inteval]', N'Interval', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    ALTER TABLE [db_stock].[TimeSeries] ADD [StockSymbol] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD CONSTRAINT [AK_Stocks_Symbol] UNIQUE ([Symbol]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    CREATE INDEX [IX_TimeSeries_StockSymbol] ON [db_stock].[TimeSeries] ([StockSymbol]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    ALTER TABLE [db_stock].[TimeSeries] ADD CONSTRAINT [FK_TimeSeries_Stocks_StockSymbol] FOREIGN KEY ([StockSymbol]) REFERENCES [db_stock].[Stocks] ([Symbol]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308104904_Updated Stock Table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230308104904_Updated Stock Table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    ALTER TABLE [db_stock].[StockSnapshots] DROP CONSTRAINT [FK_StockSnapshots_TimeSeries_TimeSeriesId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    DROP INDEX [IX_TimeSeries_StockSymbol] ON [db_stock].[TimeSeries];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[StockSnapshots]') AND [c].[name] = N'TimeSeriesId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[StockSnapshots] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [db_stock].[StockSnapshots] ALTER COLUMN [TimeSeriesId] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_TimeSeries_StockSymbol] ON [db_stock].[TimeSeries] ([StockSymbol]) WHERE [StockSymbol] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    ALTER TABLE [db_stock].[StockSnapshots] ADD CONSTRAINT [FK_StockSnapshots_TimeSeries_TimeSeriesId] FOREIGN KEY ([TimeSeriesId]) REFERENCES [db_stock].[TimeSeries] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308114129_Changed Stock TimeSeries Relationship')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230308114129_Changed Stock TimeSeries Relationship', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    ALTER TABLE [db_stock].[TimeSeries] DROP CONSTRAINT [FK_TimeSeries_Stocks_StockSymbol];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    DROP INDEX [IX_TimeSeries_StockSymbol] ON [db_stock].[TimeSeries];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    ALTER TABLE [db_stock].[Stocks] DROP CONSTRAINT [AK_Stocks_Symbol];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[TimeSeries]') AND [c].[name] = N'StockSymbol');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[TimeSeries] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [db_stock].[TimeSeries] DROP COLUMN [StockSymbol];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD [TimeSeriesId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    CREATE UNIQUE INDEX [IX_Stocks_TimeSeriesId] ON [db_stock].[Stocks] ([TimeSeriesId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD CONSTRAINT [FK_Stocks_TimeSeries_TimeSeriesId] FOREIGN KEY ([TimeSeriesId]) REFERENCES [db_stock].[TimeSeries] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230308115427_Changed Stock TimeSeries Key')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230308115427_Changed Stock TimeSeries Key', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    ALTER TABLE [db_stock].[Stocks] DROP CONSTRAINT [FK_Stocks_TimeSeries_TimeSeriesId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    DROP INDEX [IX_Stocks_TimeSeriesId] ON [db_stock].[Stocks];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Stocks]') AND [c].[name] = N'TimeSeriesId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Stocks] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [db_stock].[Stocks] ALTER COLUMN [TimeSeriesId] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    CREATE TABLE [db_stock].[RefreshTokens] (
        [Token] nvarchar(450) NOT NULL,
        [JwtId] nvarchar(max) NOT NULL,
        [CreationDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NOT NULL,
        [Used] bit NOT NULL,
        [Invalidated] bit NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Token]),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Stocks_TimeSeriesId] ON [db_stock].[Stocks] ([TimeSeriesId]) WHERE [TimeSeriesId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [db_stock].[RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD CONSTRAINT [FK_Stocks_TimeSeries_TimeSeriesId] FOREIGN KEY ([TimeSeriesId]) REFERENCES [db_stock].[TimeSeries] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230313152439_Added refreshtoken table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230313152439_Added refreshtoken table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230315090114_Added volume field')
BEGIN
    ALTER TABLE [db_stock].[StockSnapshots] ADD [Volume] float NOT NULL DEFAULT 0.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230315090114_Added volume field')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230315090114_Added volume field', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316095522_Changed_Trade_Schema')
BEGIN
    ALTER TABLE [db_stock].[Trades] DROP CONSTRAINT [FK_Trades_AspNetUsers_UserId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316095522_Changed_Trade_Schema')
BEGIN
    DROP INDEX [IX_Trades_UserId] ON [db_stock].[Trades];
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Trades]') AND [c].[name] = N'UserId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Trades] DROP CONSTRAINT [' + @var5 + '];');
    EXEC(N'UPDATE [db_stock].[Trades] SET [UserId] = N'''' WHERE [UserId] IS NULL');
    ALTER TABLE [db_stock].[Trades] ALTER COLUMN [UserId] nvarchar(450) NOT NULL;
    ALTER TABLE [db_stock].[Trades] ADD DEFAULT N'' FOR [UserId];
    CREATE INDEX [IX_Trades_UserId] ON [db_stock].[Trades] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316095522_Changed_Trade_Schema')
BEGIN
    ALTER TABLE [db_stock].[Trades] ADD CONSTRAINT [FK_Trades_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316095522_Changed_Trade_Schema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230316095522_Changed_Trade_Schema', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316161037_Change_Trade_Table')
BEGIN
    ALTER TABLE [db_stock].[Trades] ADD [Status] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230316161037_Change_Trade_Table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230316161037_Change_Trade_Table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428090018_refactored trade into marketorder')
BEGIN
    DROP TABLE [db_stock].[Trades];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428090018_refactored trade into marketorder')
BEGIN
    CREATE TABLE [db_stock].[MarketTrades] (
        [Id] uniqueidentifier NOT NULL,
        [IsBuy] bit NOT NULL,
        [StockId] uniqueidentifier NOT NULL,
        [OpenQuantity_Value] decimal(18,0) NULL,
        [OrderAmount_Value] money NULL,
        [Price_Value] money NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Status] int NOT NULL,
        [Type] int NOT NULL,
        [TradeCondition] int NOT NULL,
        CONSTRAINT [PK_MarketTrades] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketTrades_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MarketTrades_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428090018_refactored trade into marketorder')
BEGIN
    CREATE INDEX [IX_MarketTrades_StockId] ON [db_stock].[MarketTrades] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428090018_refactored trade into marketorder')
BEGIN
    CREATE INDEX [IX_MarketTrades_UserId] ON [db_stock].[MarketTrades] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428090018_refactored trade into marketorder')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230428090018_refactored trade into marketorder', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230428121300_Quick lil refactor on valueobjects')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230428121300_Quick lil refactor on valueobjects', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230501102720_changed price column')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230501102720_changed price column', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] ADD [Cost_Value] decimal(18,0) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] ADD [FeeAmount_Value] decimal(18,0) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] ADD [FeeId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] ADD [Symbol] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    CREATE TABLE [db_stock].[Fees] (
        [Id] int NOT NULL IDENTITY,
        [MakerFee] decimal(18,2) NOT NULL,
        [TakerFee] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Fees] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    CREATE INDEX [IX_MarketTrades_FeeId] ON [db_stock].[MarketTrades] ([FeeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] ADD CONSTRAINT [FK_MarketTrades_Fees_FeeId] FOREIGN KEY ([FeeId]) REFERENCES [db_stock].[Fees] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508202855_Added Fee table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230508202855_Added Fee table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508203205_Fixed symbol field in MarketOrder table')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[MarketTrades]') AND [c].[name] = N'Symbol');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [db_stock].[MarketTrades] ALTER COLUMN [Symbol] nvarchar(24) NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508203205_Fixed symbol field in MarketOrder table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230508203205_Fixed symbol field in MarketOrder table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230514134627_Added Tradefootprint table + some minor fixes')
BEGIN
    CREATE TABLE [db_stock].[TradeDetails] (
        [Id] int NOT NULL IDENTITY,
        [BidFee_Value] money NULL,
        [AskFee_Value] money NULL,
        [BidCost_Value] money NULL,
        [RemainingQuantity_Value] decimal(18,0) NULL,
        CONSTRAINT [PK_TradeDetails] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230514134627_Added Tradefootprint table + some minor fixes')
BEGIN
    CREATE TABLE [db_stock].[TradeFootprints] (
        [Id] int NOT NULL IDENTITY,
        [ProcessedOrderIsBuy] bit NOT NULL,
        [ProcessedOrderId] uniqueidentifier NOT NULL,
        [RestingOrderId] uniqueidentifier NOT NULL,
        [ProcessedOrderUserId] nvarchar(100) NOT NULL,
        [RestingOrderUserId] nvarchar(100) NOT NULL,
        [MatchPrice_Value] money NOT NULL,
        [Quantity_Value] decimal(18,0) NOT NULL,
        [TradeDetailsId] int NOT NULL,
        CONSTRAINT [PK_TradeFootprints] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TradeFootprints_TradeDetails_TradeDetailsId] FOREIGN KEY ([TradeDetailsId]) REFERENCES [db_stock].[TradeDetails] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230514134627_Added Tradefootprint table + some minor fixes')
BEGIN
    CREATE INDEX [IX_TradeFootprints_TradeDetailsId] ON [db_stock].[TradeFootprints] ([TradeDetailsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230514134627_Added Tradefootprint table + some minor fixes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230514134627_Added Tradefootprint table + some minor fixes', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230515130446_changed cost and fee field types')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[MarketTrades]') AND [c].[name] = N'OpenQuantity_Value');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [db_stock].[MarketTrades] ALTER COLUMN [OpenQuantity_Value] money NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230515130446_changed cost and fee field types')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[MarketTrades]') AND [c].[name] = N'FeeAmount_Value');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [db_stock].[MarketTrades] ALTER COLUMN [FeeAmount_Value] money NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230515130446_changed cost and fee field types')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[MarketTrades]') AND [c].[name] = N'Cost_Value');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [db_stock].[MarketTrades] ALTER COLUMN [Cost_Value] money NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230515130446_changed cost and fee field types')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230515130446_changed cost and fee field types', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [FK_MarketTrades_AspNetUsers_UserId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [FK_MarketTrades_Fees_FeeId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [FK_MarketTrades_Stocks_StockId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketTrades] DROP CONSTRAINT [PK_MarketTrades];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    EXEC sp_rename N'[db_stock].[MarketTrades]', N'MarketOrders';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    EXEC sp_rename N'[db_stock].[MarketOrders].[IX_MarketTrades_UserId]', N'IX_MarketOrders_UserId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    EXEC sp_rename N'[db_stock].[MarketOrders].[IX_MarketTrades_StockId]', N'IX_MarketOrders_StockId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    EXEC sp_rename N'[db_stock].[MarketOrders].[IX_MarketTrades_FeeId]', N'IX_MarketOrders_FeeId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[MarketOrders]') AND [c].[name] = N'Timestamp');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[MarketOrders] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [db_stock].[MarketOrders] ALTER COLUMN [Timestamp] datetimeoffset NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketOrders] ADD CONSTRAINT [PK_MarketOrders] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    CREATE TABLE [db_stock].[StopOrders] (
        [Id] uniqueidentifier NOT NULL,
        [IsBuy] bit NOT NULL,
        [IsTriggered] bit NOT NULL,
        [StockId] uniqueidentifier NOT NULL,
        [StopPrice_Value] money NOT NULL,
        [OpenQuantity_Value] money NULL,
        [FeeAmount_Value] money NOT NULL,
        [Cost_Value] money NOT NULL,
        [FeeId] int NOT NULL,
        [Timestamp] datetimeoffset NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Symbol] nvarchar(24) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_StopOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StopOrders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StopOrders_Fees_FeeId] FOREIGN KEY ([FeeId]) REFERENCES [db_stock].[Fees] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StopOrders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    CREATE INDEX [IX_StopOrders_FeeId] ON [db_stock].[StopOrders] ([FeeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    CREATE INDEX [IX_StopOrders_StockId] ON [db_stock].[StopOrders] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    CREATE INDEX [IX_StopOrders_UserId] ON [db_stock].[StopOrders] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketOrders] ADD CONSTRAINT [FK_MarketOrders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketOrders] ADD CONSTRAINT [FK_MarketOrders_Fees_FeeId] FOREIGN KEY ([FeeId]) REFERENCES [db_stock].[Fees] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    ALTER TABLE [db_stock].[MarketOrders] ADD CONSTRAINT [FK_MarketOrders_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230517162413_Added StopOrders table and fixed marketorders')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230517162413_Added StopOrders table and fixed marketorders', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230530150646_AddedVolumeField')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD [HighMonth] decimal(18,0) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230530150646_AddedVolumeField')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD [LowMonth] decimal(18,0) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230530150646_AddedVolumeField')
BEGIN
    ALTER TABLE [db_stock].[Stocks] ADD [Volume] decimal(18,0) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230530150646_AddedVolumeField')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230530150646_AddedVolumeField', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    DROP TABLE [db_stock].[StockPosition];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    DROP INDEX [IX_Portfolios_UserId] ON [db_stock].[Portfolios];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Portfolios]') AND [c].[name] = N'Name');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Portfolios] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [db_stock].[Portfolios] DROP COLUMN [Name];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    CREATE TABLE [db_stock].[ValueSnapshots] (
        [Id] int NOT NULL IDENTITY,
        [PortfolioId] uniqueidentifier NULL,
        [Timestamp] datetimeoffset NOT NULL,
        [Value] money NOT NULL,
        CONSTRAINT [PK_ValueSnapshots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ValueSnapshots_Portfolios_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [db_stock].[Portfolios] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    CREATE UNIQUE INDEX [IX_Portfolios_UserId] ON [db_stock].[Portfolios] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    CREATE INDEX [IX_ValueSnapshots_PortfolioId] ON [db_stock].[ValueSnapshots] ([PortfolioId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230605194932_added value snapshot table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230605194932_added value snapshot table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230606075029_added security table')
BEGIN
    CREATE TABLE [db_stock].[Securities] (
        [Id] int NOT NULL IDENTITY,
        [StockId] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Quantity_Value] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Securities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Securities_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [db_stock].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Securities_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230606075029_added security table')
BEGIN
    CREATE UNIQUE INDEX [IX_Securities_StockId] ON [db_stock].[Securities] ([StockId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230606075029_added security table')
BEGIN
    CREATE INDEX [IX_Securities_UserId] ON [db_stock].[Securities] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230606075029_added security table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230606075029_added security table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230609062930_added purchased price to security table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD [PurchasedPrice] money NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230609062930_added purchased price to security table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230609062930_added purchased price to security table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] DROP CONSTRAINT [FK_Securities_AspNetUsers_UserId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] DROP CONSTRAINT [FK_Securities_Stocks_StockId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    DROP INDEX [IX_Securities_StockId] ON [db_stock].[Securities];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    DROP INDEX [IX_Securities_UserId] ON [db_stock].[Securities];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Securities]') AND [c].[name] = N'UserId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Securities] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [db_stock].[Securities] DROP COLUMN [UserId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[db_stock].[Securities]') AND [c].[name] = N'StockId');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [db_stock].[Securities] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [db_stock].[Securities] ALTER COLUMN [StockId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD [OrderId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD [PortfolioId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD [Timestamp] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[AspNetUsers] ADD [PortfolioId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    CREATE INDEX [IX_Securities_OrderId] ON [db_stock].[Securities] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD CONSTRAINT [FK_Securities_MarketOrders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [db_stock].[MarketOrders] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD CONSTRAINT [FK_Securities_Portfolios_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [db_stock].[Portfolios] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    ALTER TABLE [db_stock].[Securities] ADD CONSTRAINT [FK_Securities_Stocks_StockId] FOREIGN KEY ([StockId]) REFERENCES [db_stock].[Stocks] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230610145708_refactor portfolio table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230610145708_refactor portfolio table', N'7.0.3');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612085532_added stockid column to tradedetails table')
BEGIN
    ALTER TABLE [db_stock].[TradeFootprints] ADD [StockId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230612085532_added stockid column to tradedetails table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230612085532_added stockid column to tradedetails table', N'7.0.3');
END;
GO

COMMIT;
GO

