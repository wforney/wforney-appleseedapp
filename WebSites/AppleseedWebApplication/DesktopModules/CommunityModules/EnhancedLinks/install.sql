/* Install script */

if not exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_EnhancedLinks] (


	[ItemID] [int] IDENTITY (0, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (100) NULL ,
	[Url] [nvarchar] (800) NULL ,
	[MobileUrl] [nvarchar] (250) NULL ,
	[ViewOrder] [int] NULL ,
	[Description] [nvarchar] (2000) NULL ,
    [ImageUrl] [nvarchar] (250) NULL,
    [Clicks] [int] NULL,
	[Target] [nvarchar] (10) NOT NULL DEFAULT ('_new'),
	CONSTRAINT [PK_rb_EnhancedLinks] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_EnhancedLinks_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

if not exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_EnhancedLinks_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (100) NULL ,
	[Url] [nvarchar] (800) NULL ,
	[MobileUrl] [nvarchar] (250) NULL ,
	[ViewOrder] [int] NULL ,
	[Description] [nvarchar] (2000) NULL ,
    [ImageUrl] [nvarchar] (250) NULL,
    [Clicks] [int] NULL,
	[Target] [nvarchar] (10) NOT NULL ,
	CONSTRAINT [PK_rb_EnhancedLinks_st] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_EnhancedLinks_st_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

if exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_EnhancedLinks_stModified]
GO

CREATE  TRIGGER [rb_EnhancedLinks_stModified]
ON rb_EnhancedLinks_st
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

if exists (select * from sysobjects where id = object_id(N'[rb_AddEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_AddEnhancedLink]
GO

CREATE   PROCEDURE rb_AddEnhancedLink
(
    @ModuleID    int,
    @UserName    nvarchar(100),
    @Title       nvarchar(100),
    @Url         nvarchar(800),
    @MobileUrl   nvarchar(250),
    @ViewOrder   int,
    @Description nvarchar(2000),
    @ImageUrl    nvarchar(250),
    @Clicks      int,
    @Target	     nvarchar(10),
    @ItemID      int OUTPUT
)
AS

INSERT INTO rb_EnhancedLinks_st
(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Url,
    MobileUrl,
    ViewOrder,
    Description,
    ImageUrl,
    Clicks,
    Target
)
VALUES
(
    @ModuleID,
    @UserName,
    GetDate(),
    @Title,
    @Url,
    @MobileUrl,
    @ViewOrder,
    @Description,
    @ImageUrl,
    @Clicks,
    @Target
)

SELECT
    @ItemID = @@Identity
GO

if exists (select * from sysobjects where id = object_id(N'[rb_DeleteEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_DeleteEnhancedLink]
GO

CREATE   PROCEDURE rb_DeleteEnhancedLink
(
    @ItemID int
)
AS

DELETE FROM
    rb_EnhancedLinks_st

WHERE
    ItemID = @ItemID
GO

if exists (select * from sysobjects where id = object_id(N'[rb_GetEnhancedLinks]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetEnhancedLinks]
GO

CREATE   PROCEDURE rb_GetEnhancedLinks
(
    @ModuleID int,
    @WorkflowVersion int
)
AS

IF (@WorkflowVersion = 1)
	SELECT
	    ItemID,
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    Url,
	    ViewOrder,
	    Description,
	    ImageUrl,
	    Clicks,
	    Target
	FROM
	    rb_EnhancedLinks
	WHERE
	    ModuleID = @ModuleID
	ORDER BY
	    ViewOrder
ELSE
	SELECT
	    ItemID,
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    Url,
	    ViewOrder,
	    Description,
	    ImageUrl,
	    Clicks,
	    Target
	FROM
	    rb_EnhancedLinks_st
	WHERE
	    ModuleID = @ModuleID
	ORDER BY
	    ViewOrder
GO

if exists (select * from sysobjects where id = object_id(N'[rb_GetSingleEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetSingleEnhancedLink]
GO

CREATE   PROCEDURE rb_GetSingleEnhancedLink
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF (@WorkflowVersion = 1)
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    Url,
	    MobileUrl,
	    ViewOrder,
	    Description,
	    ImageUrl,
	    Clicks,
	    Target
	FROM
	    rb_EnhancedLinks
	WHERE
	    ItemID = @ItemID
ELSE
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    Url,
	    MobileUrl,
	    ViewOrder,
	    Description,
	    ImageUrl,
	    Clicks,
	    Target
	FROM
	    rb_EnhancedLinks_st
	WHERE
	    ItemID = @ItemID
GO

if exists (select * from sysobjects where id = object_id(N'[rb_UpdateEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_UpdateEnhancedLink]
GO

CREATE   PROCEDURE rb_UpdateEnhancedLink
(
    @ItemID      int,
    @UserName    nvarchar(100),
    @Title       nvarchar(100),
    @Url         nvarchar(800),
    @MobileUrl   nvarchar(250),
    @ViewOrder   int,
    @Description nvarchar(2000),
    @ImageUrl	 nvarchar(250),
    @Clicks      int,
    @Target		 nvarchar(10)
)
AS

UPDATE
    rb_EnhancedLinks_st

SET
    CreatedByUser = @UserName,
    CreatedDate   = GetDate(),
    Title         = @Title,
    Url           = @Url,
    MobileUrl     = @MobileUrl,
    ViewOrder     = @ViewOrder,
    Description   = @Description,
    ImageUrl      = @ImageUrl,
    Clicks        = @Clicks,
    Target		  = @Target

WHERE
    ItemID = @ItemID
GO
