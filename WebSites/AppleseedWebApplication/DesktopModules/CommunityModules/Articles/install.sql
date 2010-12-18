IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Articles] 
(
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100)  NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (100)  NULL ,
	[Subtitle] [nvarchar] (200)  NULL ,
	[Abstract] [ntext] NULL ,
	[StartDate] [datetime] NULL ,
	[ExpireDate] [datetime] NULL ,
	[IsInNewsletter] [bit] NULL ,
	[MoreLink] [nvarchar] (150)  NULL ,
	[Description] [ntext]  NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [rb_Articles] WITH NOCHECK ADD 
	CONSTRAINT [PK_rb_Articles] PRIMARY KEY  CLUSTERED 
	(
		[ItemID]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
ALTER TABLE [rb_Articles] ADD 
	CONSTRAINT [FK_rb_Articles_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE 
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  IF NOT EXISTS (select b.length From sysobjects a inner join  syscolumns b ON a.id = b.id where a.name = 'rb_Articles' and b.name = 'Abstract' AND b.length = 16) 
	BEGIN 
	   ALTER TABLE [rb_Articles] ALTER COLUMN [Abstract][ntext] NULL
	END 
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[PK_rb_Articles]') AND OBJECTPROPERTY(id, N'IsPrimaryKey') = 1)
		IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
			ALTER TABLE [rb_Articles] DROP CONSTRAINT [PK_rb_Articles]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Articles_rb_Modules]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
		IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
			ALTER TABLE [rb_Articles] DROP CONSTRAINT [FK_rb_Articles_rb_Modules]
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
		/*Upgrade only... we preserve existing articles*/
		EXECUTE sp_rename N'rb_Articles', N'rb_Articles_st', 'OBJECT'
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[rb_Articles_st]'))
	IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[PK_rb_Articles_st]'))
		ALTER TABLE [rb_Articles_st] WITH NOCHECK ADD 
			CONSTRAINT [PK_rb_Articles_st] PRIMARY KEY  CLUSTERED 
			(
				[ItemID]
			) WITH  FILLFACTOR = 90  ON [PRIMARY] 
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[rb_Articles_st]'))
	IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Articles_st_rb_Modules]'))
		ALTER TABLE [rb_Articles_st] WITH NOCHECK ADD CONSTRAINT
			FK_rb_Articles_st_rb_Modules FOREIGN KEY
			(
			ModuleID
			) REFERENCES rb_Modules
			(
			ModuleID
			) ON DELETE CASCADE 	
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Articles_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100)  NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (100)  NULL ,
	[Subtitle] [nvarchar] (200)  NULL ,
	[Abstract] [ntext] NULL ,
	[StartDate] [datetime] NULL ,
	[ExpireDate] [datetime] NULL ,
	[IsInNewsletter] [bit] NULL ,
	[MoreLink] [nvarchar] (150)  NULL ,
	[Description] [ntext]  NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [rb_Articles_st] WITH NOCHECK ADD 
	CONSTRAINT [PK_rb_Articles_st] PRIMARY KEY  CLUSTERED 
	(
		[ItemID]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
ALTER TABLE [rb_Articles] ADD 
	CONSTRAINT [FK_rb_Articles_st_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE 
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
  IF NOT EXISTS (select b.length From sysobjects a inner join  syscolumns b ON a.id = b.id where a.name = 'rb_Articles_st' and b.name = 'Abstract' AND b.length = 16) 
	BEGIN 
	   ALTER TABLE [rb_Articles_st] ALTER COLUMN [Abstract][ntext] NULL
	END 
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles_stModified]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Articles_stModified]
GO

CREATE TRIGGER [rb_Articles_stModified]
ON [rb_Articles_st]
FOR DELETE, INSERT, UPDATE 
AS 
BEGIN
	DECLARE ChangedModules CURSOR FOR
		SELECT ModuleID
		FROM inserted
		UNION
		SELECT ModuleID
		FROM deleted
	DECLARE @ModID	int
	OPEN ChangedModules	
	FETCH NEXT FROM ChangedModules
	INTO @ModID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC rb_ModuleEdited @ModID
		FETCH NEXT FROM ChangedModules
		INTO @ModID
	END
	CLOSE ChangedModules
	DEALLOCATE ChangedModules
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetArticles]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetArticles]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetArticlesAll]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetArticlesAll]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleArticleWithImages]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleArticleWithImages]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateArticle]
GO

CREATE PROCEDURE rb_AddArticle
(
	@ModuleID	   int,
	@UserName	   nvarchar(100),
	@Title		  nvarchar(100),
	@Subtitle	   nvarchar(200),
	@Abstract	   ntext,
	@Description	ntext,
	@StartDate	  datetime,
	@ExpireDate	 datetime,
	@IsInNewsletter bit,
	@MoreLink	   nvarchar(150),
	@ItemID		 int OUTPUT
)
AS

INSERT INTO rb_Articles_st
(
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	Subtitle,
	Abstract,
	Description,
	StartDate,
	ExpireDate,
	IsInNewsletter,
	MoreLink
)
VALUES
(
	@ModuleID,
	@UserName,
	GETDATE(),
	@Title,
	@Subtitle,
	@Abstract,
	@Description,
	@StartDate,
	@ExpireDate,
	@IsInNewsletter,
	@MoreLink
)

SELECT
	@ItemID = @@IDENTITY
GO

CREATE PROCEDURE rb_DeleteArticle
(
	@ItemID int
)
AS
DELETE FROM
	rb_Articles_st
WHERE
	ItemID = @ItemID
GO
CREATE PROCEDURE rb_GetArticles
(
	@ModuleID int,
	@WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
SELECT		ItemID, 
			ModuleID, 
			CreatedByUser, 
			CreatedDate, 
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink,
			0 as Expired
FROM		rb_Articles
WHERE (ModuleID = @ModuleID) AND (GETDATE() <= ExpireDate) AND (GETDATE() >= StartDate)
ORDER BY StartDate DESC
ELSE
SELECT		ItemID, 
			ModuleID, 
			CreatedByUser, 
			CreatedDate, 
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink,
			0 as Expired
FROM		rb_Articles_st
WHERE (ModuleID = @ModuleID) AND (GETDATE() <= ExpireDate) AND (GETDATE() >= StartDate)
ORDER BY StartDate DESC
GO

CREATE PROCEDURE rb_GetSingleArticle
(
	@ItemID int,
	@WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
SELECT		ItemID,
			ModuleID,
			CreatedByUser,
			CreatedDate,
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink
FROM	rb_Articles
WHERE   (ItemID = @ItemID)
ELSE
SELECT		ItemID,
			ModuleID,
			CreatedByUser,
			CreatedDate,
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink
FROM	rb_Articles_st
WHERE   (ItemID = @ItemID)
GO

CREATE PROCEDURE rb_UpdateArticle
(
	@ItemID		 int,
	@ModuleID	   int,
	@UserName	   nvarchar(100),
	@Title		  nvarchar(100),
	@Subtitle	   nvarchar(200),
	@Abstract	   ntext,
	@Description	ntext,
	@StartDate	  datetime,
	@ExpireDate	 datetime,
	@IsInNewsletter bit,
	@MoreLink	   nvarchar(150)
)
AS
UPDATE rb_Articles_st
SET 
ModuleID = @ModuleID,
CreatedByUser = @UserName,
CreatedDate = GETDATE(),
Title =@Title ,
Subtitle =  @Subtitle,
Abstract =@Abstract,
Description =@Description,
StartDate = @StartDate,
ExpireDate =@ExpireDate,
IsInNewsletter = @IsInNewsletter,
MoreLink =@MoreLink
WHERE 
ItemID = @ItemID
GO

CREATE PROCEDURE rb_GetArticlesAll
(
	@ModuleID int,
	@WorkflowVersion int
)
AS
IF ( @WorkflowVersion = 1 )
SELECT		ItemID, 
			ModuleID, 
			CreatedByUser, 
			CreatedDate, 
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink,
			CASE
			WHEN ((GETDATE() <= ExpireDate) AND (GETDATE() >= StartDate)) THEN 0 
				ELSE 1 
			END 
			AS Expired
FROM		rb_Articles
WHERE		(ModuleID = @ModuleID)
ORDER BY	StartDate DESC
ELSE
SELECT		ItemID, 
			ModuleID, 
			CreatedByUser, 
			CreatedDate, 
			Title, 
			Subtitle, 
			Abstract, 
			Description, 
			StartDate, 
			ExpireDate, 
			IsInNewsletter, 
			MoreLink,
			CASE
			WHEN ((GETDATE() <= ExpireDate) AND (GETDATE() >= StartDate)) THEN 0 
				ELSE 1 
			END 
			AS Expired
FROM		rb_Articles_st
WHERE		(ModuleID = @ModuleID)
ORDER BY	StartDate DESC
GO

