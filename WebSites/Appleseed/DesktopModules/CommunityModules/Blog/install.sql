/*
Created by Joe Audette joe@joeaudette.com

*/

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogAdd]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogAdd]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogUpdate]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogCommentAdd]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogCommentAdd]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogCommentDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogCommentDelete]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogCommentsGet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogCommentsGet]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogDelete]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogGetSingle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogGetSingle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogGetSingleWithImages]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogGetSingleWithImages]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogStatsGet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogStatsGet]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogsByMonthArchiveGet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogsByMonthArchiveGet]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogsByMonthGet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogsByMonthGet]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogsGet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_BlogsGet]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[FK_rb_BlogComments_rb_Modules]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [rb_BlogComments] DROP CONSTRAINT FK_rb_BlogComments_rb_Modules
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[FK_rb_Blogs_rb_Modules]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [rb_Blogs] DROP CONSTRAINT FK_rb_Blogs_rb_Modules
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[FK_rb_BlogStats_rb_Modules]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [rb_BlogStats] DROP CONSTRAINT FK_rb_BlogStats_rb_Modules
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogComments]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_BlogComments] (
	[BlogCommentID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[ItemID] [int] NOT NULL ,
	[Comment] [ntext] NOT NULL ,
	[Title] [nvarchar] (100) NULL ,
	[Name] [nvarchar] (100) NULL ,
	[URL] [nvarchar] (200) NULL ,
	[DateCreated] [datetime] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_BlogStats]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_BlogStats] (
	[ModuleID] [int] NOT NULL ,
	[EntryCount] [int] NOT NULL ,
	[CommentCount] [int] NOT NULL ,
	[TrackBackCount] [int] NOT NULL 
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Blogs]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Blogs] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (100) NULL ,
	[Excerpt] [nvarchar] (512) NULL ,
	[StartDate] [datetime] NULL ,
	[IsInNewsletter] [bit] NULL ,
	[Description] [ntext] NULL ,
	[CommentCount] [int] NOT NULL ,
	[TrackBackCount] [int] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[PK_rb_BlogComments]') and OBJECTPROPERTY(id, N'IsPrimaryKey') = 1)
BEGIN
ALTER TABLE [rb_BlogComments] WITH NOCHECK ADD 
	CONSTRAINT [PK_rb_BlogComments] PRIMARY KEY  CLUSTERED 
	(
		[BlogCommentID]
	)  ON [PRIMARY] 
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[PK_rb_Blogs]') and OBJECTPROPERTY(id, N'IsPrimaryKey') = 1)
BEGIN
ALTER TABLE [rb_Blogs] WITH NOCHECK ADD 
	CONSTRAINT [PK_rb_Blogs] PRIMARY KEY  CLUSTERED 
	(
		[ItemID]
	) WITH  FILLFACTOR = 90  ON [PRIMARY] 
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_BlogComments_DateCreated]'))
BEGIN
ALTER TABLE [rb_BlogComments] ADD 
	CONSTRAINT [DF_rb_BlogComments_DateCreated] DEFAULT (GETDATE()) FOR [DateCreated]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_BlogStats_EntryCount]'))
BEGIN
ALTER TABLE [rb_BlogStats] ADD 
	CONSTRAINT [DF_rb_BlogStats_EntryCount] DEFAULT (0) FOR [EntryCount]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_BlogStats_CommentCount]'))
BEGIN
ALTER TABLE [rb_BlogStats] ADD 
	CONSTRAINT [DF_rb_BlogStats_CommentCount] DEFAULT (0) FOR [CommentCount]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_BlogStats_TrackBackCount]'))
BEGIN
ALTER TABLE [rb_BlogStats] ADD 
	CONSTRAINT [DF_rb_BlogStats_TrackBackCount] DEFAULT (0) FOR [TrackBackCount]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_Blogs_Comments]'))
BEGIN
ALTER TABLE [rb_Blogs] ADD 
	CONSTRAINT [DF_rb_Blogs_Comments] DEFAULT (0) FOR [CommentCount]
END
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[DF_rb_Blogs_TrackBackCount]'))
BEGIN
ALTER TABLE [rb_Blogs] ADD 
	CONSTRAINT [DF_rb_Blogs_TrackBackCount] DEFAULT (0) FOR [TrackBackCount]
END
GO

ALTER TABLE [rb_BlogComments] ADD 
	CONSTRAINT [FK_rb_BlogComments_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE 
GO

ALTER TABLE [rb_BlogStats] ADD 
	CONSTRAINT [FK_rb_BlogStats_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE 
GO

ALTER TABLE [rb_Blogs] ADD 
	CONSTRAINT [FK_rb_Blogs_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO




CREATE PROCEDURE rb_BlogAdd
(

    @ModuleID       int,

    @UserName       nvarchar(100),

    @Title          nvarchar(100),

    @Excerpt	    nvarchar(512),

    @Description    ntext,

    @StartDate      datetime,

    @IsInNewsletter bit,

    @ItemID         int OUTPUT

)

AS



INSERT INTO rb_Blogs

(

    ModuleID,

    CreatedByUser,

    CreatedDate,

    Title,


    Excerpt,

	Description,

	StartDate,

	IsInNewsletter

	

)

VALUES

(

    @ModuleID,

    @UserName,

    GETDATE(),

    @Title,

    @Excerpt,

    @Description,

    @StartDate,

    @IsInNewsletter

 

)



SELECT

    @ItemID = @@IDENTITY


IF EXISTS(SELECT ModuleID FROM rb_BlogStats WHERE ModuleID = @ModuleID)
	BEGIN
		UPDATE rb_BlogStats
		SET 	EntryCount = EntryCount + 1
		WHERE ModuleID = @ModuleID

	END
ELSE
	BEGIN
		INSERT INTO rb_BlogStats(ModuleID, EntryCount)
		VALUES (@ModuleID, 1)


	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE rb_BlogUpdate

(

    @ItemID         int,

    @ModuleID       int,

    @UserName       nvarchar(100),

    @Title          nvarchar(100),

    @Excerpt       nvarchar(512),

    @Description    ntext,

    @StartDate      datetime,

    @IsInNewsletter bit

   

)

AS



UPDATE rb_Blogs



SET 

ModuleID = @ModuleID,

CreatedByUser = @UserName,

CreatedDate = GETDATE(),

Title =@Title ,

Excerpt =@Excerpt,

Description =@Description,

StartDate = @StartDate,

IsInNewsletter = @IsInNewsletter



WHERE 

ItemID = @ItemID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogCommentAdd
(

    	@ModuleID       int,
	@ItemID	int,

    	@Name       nvarchar(100),

    	@Title          nvarchar(100),

   	@URL       nvarchar(200),

    	@Comment    ntext

    	

    	

)

AS





INSERT INTO rb_BlogComments

(

    	ModuleID,
	ItemID,

    	Name,

    	Title,

	URL,

    	Comment,

	DateCreated

)

VALUES

(

    @ModuleID,

    @ItemID,

   @Name,

    @Title,

    @URL,

    @Comment,

    GETDATE()

)



UPDATE rb_Blogs
SET CommentCount = CommentCount + 1
WHERE ModuleID = @ModuleID AND ItemID = @ItemID


UPDATE rb_BlogStats
SET 	CommentCount = CommentCount + 1
WHERE ModuleID = @ModuleID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogCommentDelete
(
    @BlogCommentID int
)
AS

DECLARE @ModuleID int
DECLARE @ItemID int

SELECT @ModuleID = ModuleID, @ItemID = ItemID
FROM	rb_BlogComments
WHERE BlogCommentID = @BlogCommentID

DELETE FROM
    rb_BlogComments

WHERE
    BlogCommentID = @BlogCommentID



UPDATE rb_Blogs
SET CommentCount = CommentCount - 1
WHERE ModuleID = @ModuleID AND ItemID = @ItemID

UPDATE rb_BlogStats
SET 	CommentCount = CommentCount - 1
WHERE ModuleID = @ModuleID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogCommentsGet
(
    @ModuleID int,
    @ItemID int
)
AS

SELECT		BlogCommentID,
			ItemID, 
			ModuleID, 
			Name, 
			Title, 
			URL, 
			Comment, 
			DateCreated

FROM        rb_BlogComments

WHERE
    		ModuleID = @ModuleID
		AND ItemID = @ItemID

   ORDER BY
   	BlogCommentID DESC,  DateCreated DESC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE rb_BlogDelete
(
    @ItemID int
)
AS

DECLARE @ModuleID int
SET @ModuleID = (SELECT TOP 1 ModuleID FROM rb_Blogs WHERE ItemID = @ItemID)

DELETE FROM
    rb_Blogs

WHERE
    ItemID = @ItemID

UPDATE rb_BlogStats
SET 	EntryCount = EntryCount - 1
WHERE ModuleID = @ModuleID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO




CREATE PROCEDURE rb_BlogGetSingle
(
    @ItemID int
)
AS

SELECT		ItemID,
			ModuleID,
			CreatedByUser,
			CreatedDate,
			Title, 
			Excerpt, 
			Description, 
			StartDate, 
			IsInNewsletter
			
FROM	rb_Blogs
WHERE   (ItemID = @ItemID)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE rb_BlogGetSingleWithImages
(
    @ItemID int,
    @Variation varchar(50)
)
AS

SELECT		rb_Blogs.ItemID, 
			rb_Blogs.ModuleID, 
			rb_Blogs.CreatedByUser, 
			rb_Blogs.CreatedDate, 
			rb_Blogs.Title, 
			rb_Blogs.Excerpt, 
			rb_Blogs.Description, 
            rb_Blogs.StartDate, 
            rb_Blogs.IsInNewsletter
         
            
FROM        rb_Blogs
WHERE     (ItemID = @ItemID)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogStatsGet
(
    @ModuleID int
)
AS

SELECT		
			ModuleID, 
			EntryCount,
			CommentCount

FROM       		 rb_BlogStats

WHERE
    			ModuleID = @ModuleID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogsByMonthArchiveGet


(
	@ModuleID int
)

AS

SELECT 	MONTH(StartDate) AS [MONTH], 
		DATENAME(MONTH,StartDate) AS [MonthName],
		YEAR(StartDate) AS [YEAR], 
		1 AS Day, 
		COUNT(*) AS [COUNT] 

FROM 		rb_Blogs
 
WHERE 	ModuleID = @ModuleID 

GROUP BY 	YEAR(StartDate), 
		MONTH(StartDate) ,
		DATENAME(MONTH,StartDate)

ORDER BY 	[YEAR] desc, [MONTH] desc
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE rb_BlogsByMonthGet

(
	@MONTH int,
	@YEAR int,
	@ModuleID int
)

AS

SELECT	*

FROM 		rb_Blogs

WHERE 	ModuleID = @ModuleID
		AND MONTH(StartDate)  = @MONTH 
		AND YEAR(StartDate)  = @YEAR


ORDER BY	 StartDate DESC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE rb_BlogsGet
(
    @ModuleID int
)
AS

DECLARE @RowsToGet int

SET @RowsToGet = COALESCE((SELECT TOP 1 SettingValue FROM rb_ModuleSettings WHERE SettingName = 'Entries To Show' AND ModuleID = @ModuleID),1)

SET rowcount @RowsToGet

SELECT		ItemID, 
			ModuleID, 
			CreatedByUser, 
			CreatedDate, 
			Title, 
			Excerpt, 
			Description, 
			StartDate,
			IsInNewsletter, 
			'CommentCount' = CONVERT(nvarchar(20), CommentCount)

FROM        rb_Blogs

WHERE
    (ModuleID = @ModuleID)  AND (GETDATE() >= StartDate)

   ORDER BY
   	ItemID DESC,  StartDate DESC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

