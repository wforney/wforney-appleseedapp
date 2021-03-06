USE [master]
GO
/****** Object:  Database [ContentDB]    Script Date: 04/22/2006 22:11:29 ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ContentDB')
BEGIN
CREATE DATABASE [ContentDB] ON  PRIMARY 
( NAME = N'ContentDB', FILENAME = N'C:\ContentDB\ContentDB.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ContentDB_log', FILENAME = N'C:\ContentDB\ContentDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

GO
EXEC dbo.sp_dbcmptlevel @dbname=N'ContentDB', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ContentDB].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [ContentDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ContentDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ContentDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ContentDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ContentDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ContentDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ContentDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [ContentDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ContentDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ContentDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ContentDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ContentDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ContentDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ContentDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ContentDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ContentDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ContentDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ContentDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ContentDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ContentDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ContentDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ContentDB] SET  READ_WRITE 
GO
ALTER DATABASE [ContentDB] SET RECOVERY FULL 
GO
ALTER DATABASE [ContentDB] SET  MULTI_USER 
GO
ALTER DATABASE [ContentDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ContentDB] SET DB_CHAINING OFF 
USE [ContentDB]
GO
/****** Object:  Table [dbo].[ItemTypes]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemTypes](
	[ItemTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[BaseType] [int] NULL,
	[typeSettings] [xml] NULL,
	[parentType] [int] NULL,
	[ItemGUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED 
(
	[ItemTypeId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemTypes', @level2type=N'COLUMN', @level2name=N'ItemTypeId'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Format', @value=NULL ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemTypes', @level2type=N'COLUMN', @level2name=N'ItemTypeId'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemTypes', @level2type=N'COLUMN', @level2name=N'Name'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Format', @value=NULL ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemTypes', @level2type=N'COLUMN', @level2name=N'Name'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemTypes', @level2type=N'COLUMN', @level2name=N'Name'

GO
/****** Object:  Table [dbo].[ItemContent]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemContent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemContent](
	[ItemId] [bigint] NOT NULL,
	[Abstract] [nvarchar](max) NOT NULL,
	[FullText] [ntext] NOT NULL,
	[KeyWords] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ItemContent] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ItemStandardData]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemStandardData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemStandardData](
	[ParentItemId] [bigint] NULL,
	[CreationUserId] [bigint] NOT NULL,
	[CreationDate] [datetime] NOT NULL CONSTRAINT [DF_ItemStandardData_CreationDate]  DEFAULT (getdate()),
	[GroupItemId] [bigint] NULL,
	[ItemID] [bigint] NOT NULL,
 CONSTRAINT [PK_ItemStandardData] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[SelectItems]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItems]
AS
	SET NOCOUNT ON;
SELECT ItemId, ItemTypeId, CultureCode, Title, Description, IsStoredInDb, Url, MimeType, Path, ParentItemId, StatusId, Version, CreationUserId, CreationDate, IsEncrypted FROM dbo.Items' 
END
GO
/****** Object:  StoredProcedure [dbo].[SelectItemByID]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemByID]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemByID]
(
	@ItemID bigint
)
AS
	SET NOCOUNT ON;
SELECT ItemId, ItemTypeId, CultureCode, Title, Description, IsStoredInDb, Url, MimeType, ParentItemId, Path, StatusId, Version, CreationUserId, 
               CreationDate, IsEncrypted
FROM  Items
WHERE (ItemId = @ItemID)' 
END
GO
/****** Object:  StoredProcedure [dbo].[InsertItem]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertItem]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[InsertItem]
(
	@ItemTypeId int,
	@CultureCode varchar(10),
	@Title nvarchar(50),
	@Description nvarchar(255),
	@IsStoredInDb bit,
	@Url nvarchar(255),
	@MimeType nvarchar(50),
	@Path nvarchar(255),
	@ParentItemId bigint,
	@StatusId int,
	@Version bigint,
	@CreationUserId bigint,
	@CreationDate datetime,
	@IsEncrypted bit
)
AS
	SET NOCOUNT OFF;
INSERT INTO [dbo].[Items] ([ItemTypeId], [CultureCode], [Title], [Description], [IsStoredInDb], [Url], [MimeType], [Path], [ParentItemId], [StatusId], [Version], [CreationUserId], [CreationDate], [IsEncrypted]) VALUES (@ItemTypeId, @CultureCode, @Title, @Description, @IsStoredInDb, @Url, @MimeType, @Path, @ParentItemId, @StatusId, @Version, @CreationUserId, @CreationDate, @IsEncrypted);
	
SELECT ItemId, ItemTypeId, CultureCode, Title, Description, IsStoredInDb, Url, MimeType, Path, ParentItemId, StatusId, Version, CreationUserId, CreationDate, IsEncrypted FROM Items WHERE (ItemId = SCOPE_IDENTITY())' 
END
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Categories](
	[CategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[resourceKey] [varchar](max) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Cultures]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cultures]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Cultures](
	[culturename] [varchar](10) NOT NULL,
	[NeutralCode] [char](2) NULL,
	[CountryID] [nchar](2) NULL,
	[Description] [nvarchar](max) NULL,
	[Identifier] [int] NOT NULL,
	[Direction] [varchar](5) NOT NULL CONSTRAINT [DF_Cultures_Direction]  DEFAULT ('ltr'),
	[resourceKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_Cultures] PRIMARY KEY CLUSTERED 
(
	[culturename] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Index [IX_Cultures]    Script Date: 04/22/2006 22:11:34 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Cultures]') AND name = N'IX_Cultures')
CREATE NONCLUSTERED INDEX [IX_Cultures] ON [dbo].[Cultures] 
(
	[Direction] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ltr or rtl' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Cultures', @level2type=N'COLUMN', @level2name=N'Direction'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Cultures', @level2type=N'COLUMN', @level2name=N'Direction'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Format', @value=NULL ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Cultures', @level2type=N'COLUMN', @level2name=N'Direction'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Cultures', @level2type=N'COLUMN', @level2name=N'Direction'

GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Permissions](
	[PermissionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Status]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Status]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Status](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NOT NULL,
	[resourceKey] [varchar](50) NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Items]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Items](
	[ItemId] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemMasterID] [bigint] NULL,
	[ItemTypeId] [int] NOT NULL,
	[culturename] [varchar](10) NOT NULL CONSTRAINT [DF_rb_Items_Language]  DEFAULT (N'Invariant'),
	[Title] [nvarchar](max) NULL,
	[IsStoredInDb] [bit] NOT NULL CONSTRAINT [DF_Items_StoredInDb]  DEFAULT ((0)),
	[Version] [bigint] NULL,
	[IsEncrypted] [bit] NOT NULL CONSTRAINT [DF_Items_IsEncrypted]  DEFAULT ((0)),
 CONSTRAINT [PK_rb_Items] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'this field connects items, when an item is the same item but different version, or culture, then this field groups the items.' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Items', @level2type=N'COLUMN', @level2name=N'ItemMasterID'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Is Content encrypted?' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Items', @level2type=N'COLUMN', @level2name=N'IsEncrypted'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DisplayControl', @value=N'109' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Items', @level2type=N'COLUMN', @level2name=N'IsEncrypted'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Format', @value=NULL ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Items', @level2type=N'COLUMN', @level2name=N'IsEncrypted'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_IMEMode', @value=N'0' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Items', @level2type=N'COLUMN', @level2name=N'IsEncrypted'

GO
/****** Object:  Table [dbo].[CategoryRelations]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CategoryRelations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CategoryRelations](
	[CategoryID] [bigint] NOT NULL,
	[ParentCategoryID] [bigint] NOT NULL,
	[dateCreated] [datetime] NULL CONSTRAINT [DF_CategoryRelations_dateCreated]  DEFAULT (getdate()),
 CONSTRAINT [PK_CategoryRelations] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC,
	[ParentCategoryID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[StatusCategories]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatusCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StatusCategories](
	[StatusId] [int] NOT NULL,
	[CategoryId] [bigint] NOT NULL,
 CONSTRAINT [PK_StatusCategories] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC,
	[CategoryId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ItemCategories]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemCategories](
	[ItemId] [bigint] NOT NULL,
	[CategoryId] [bigint] NOT NULL,
 CONSTRAINT [PK_ItemCategories] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[CategoryId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ItemPermissions]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemPermissions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemPermissions](
	[ItemId] [bigint] NOT NULL,
	[PermissionId] [bigint] NOT NULL,
	[UserGroupId] [nvarchar](max) NOT NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ItemStatuses]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemStatuses]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemStatuses](
	[StatusId] [int] NOT NULL,
	[itemid] [bigint] NOT NULL,
 CONSTRAINT [PK_ItemStatuses] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC,
	[itemid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ItemSettings]    Script Date: 04/22/2006 22:11:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemSettings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemSettings](
	[ItemId] [bigint] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[XmlSettings] [xml] NULL,
	[DataType] [nvarchar](max) NULL
) ON [PRIMARY]
END
GO

/****** Object:  Index [IX_ItemSettings]    Script Date: 04/22/2006 22:11:34 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ItemSettings]') AND name = N'IX_ItemSettings')
CREATE NONCLUSTERED INDEX [IX_ItemSettings] ON [dbo].[ItemSettings] 
(
	[ItemId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'We can implement a one row per item in settings if xml field proves to perform well with large content. we may want to have a group of small types, and other rows for larger texts' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'ItemSettings', @level2type=N'COLUMN', @level2name=N'XmlSettings'

GO
USE [ContentDB]
GO
USE [ContentDB]
GO
USE [ContentDB]
GO
USE [ContentDB]
GO
USE [ContentDB]
GO
USE [ContentDB]
GO
USE [ContentDB]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Items_Cultures]') AND parent_object_id = OBJECT_ID(N'[dbo].[Items]'))
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_Cultures] FOREIGN KEY([culturename])
REFERENCES [dbo].[Cultures] ([culturename])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Items_ItemContent]') AND parent_object_id = OBJECT_ID(N'[dbo].[Items]'))
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemContent] FOREIGN KEY([ItemId])
REFERENCES [dbo].[ItemContent] ([ItemId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Items_ItemStandardData]') AND parent_object_id = OBJECT_ID(N'[dbo].[Items]'))
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemStandardData] FOREIGN KEY([ItemId])
REFERENCES [dbo].[ItemStandardData] ([ItemID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Items_ItemTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Items]'))
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CategoryRelations_Categories]') AND parent_object_id = OBJECT_ID(N'[dbo].[CategoryRelations]'))
ALTER TABLE [dbo].[CategoryRelations]  WITH CHECK ADD  CONSTRAINT [FK_CategoryRelations_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CategoryRelations_Categories1]') AND parent_object_id = OBJECT_ID(N'[dbo].[CategoryRelations]'))
ALTER TABLE [dbo].[CategoryRelations]  WITH CHECK ADD  CONSTRAINT [FK_CategoryRelations_Categories1] FOREIGN KEY([ParentCategoryID])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StatusCategories_Categories]') AND parent_object_id = OBJECT_ID(N'[dbo].[StatusCategories]'))
ALTER TABLE [dbo].[StatusCategories]  WITH CHECK ADD  CONSTRAINT [FK_StatusCategories_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StatusCategories_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[StatusCategories]'))
ALTER TABLE [dbo].[StatusCategories]  WITH CHECK ADD  CONSTRAINT [FK_StatusCategories_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StatusCategories_Status1]') AND parent_object_id = OBJECT_ID(N'[dbo].[StatusCategories]'))
ALTER TABLE [dbo].[StatusCategories]  WITH CHECK ADD  CONSTRAINT [FK_StatusCategories_Status1] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemCategories_Categories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemCategories]'))
ALTER TABLE [dbo].[ItemCategories]  WITH CHECK ADD  CONSTRAINT [FK_ItemCategories_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemCategories_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemCategories]'))
ALTER TABLE [dbo].[ItemCategories]  WITH CHECK ADD  CONSTRAINT [FK_ItemCategories_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemPermissions_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemPermissions]'))
ALTER TABLE [dbo].[ItemPermissions]  WITH CHECK ADD  CONSTRAINT [FK_ItemPermissions_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemPermissions_Permissions]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemPermissions]'))
ALTER TABLE [dbo].[ItemPermissions]  WITH CHECK ADD  CONSTRAINT [FK_ItemPermissions_Permissions] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permissions] ([PermissionId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemStatuses_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemStatuses]'))
ALTER TABLE [dbo].[ItemStatuses]  WITH CHECK ADD  CONSTRAINT [FK_ItemStatuses_Items] FOREIGN KEY([itemid])
REFERENCES [dbo].[Items] ([ItemId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemStatuses_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemStatuses]'))
ALTER TABLE [dbo].[ItemStatuses]  WITH CHECK ADD  CONSTRAINT [FK_ItemStatuses_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ItemSettings_Items]') AND parent_object_id = OBJECT_ID(N'[dbo].[ItemSettings]'))
ALTER TABLE [dbo].[ItemSettings]  WITH CHECK ADD  CONSTRAINT [FK_ItemSettings_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId])
ON UPDATE CASCADE
ON DELETE CASCADE
