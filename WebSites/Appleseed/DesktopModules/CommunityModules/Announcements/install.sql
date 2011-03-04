/* Install script, Announcements module, mario@hartmann.net, 09/11/03 */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Announcements]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Announcements] (
	[ItemID] [int] IDENTITY (1,1)NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (150) NULL ,
	[MoreLink] [nvarchar] (150) NULL ,
	[MobileMoreLink] [nvarchar] (150) NULL ,
	[ExpireDate] [datetime] NULL ,
	[Description] [nvarchar] (2000) NULL ,
	CONSTRAINT [PK_rb_Announcements] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Announcements_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Announcements_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Announcements_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Title] [nvarchar] (150) NULL ,
	[MoreLink] [nvarchar] (150) NULL ,
	[MobileMoreLink] [nvarchar] (150) NULL ,
	[ExpireDate] [datetime] NULL ,
	[Description] [nvarchar] (2000) NULL ,
	CONSTRAINT [PK_rb_Announcements_st] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Announcements_st_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Announcements_stModified]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Announcements_stModified]
GO

CREATE TRIGGER [rb_Announcements_stModified]
ON [rb_Announcements_st]
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

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'.[rb_AddAnnouncement]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddAnnouncement]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteAnnouncement]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteAnnouncement]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetAnnouncements]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetAnnouncements]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleAnnouncement]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleAnnouncement]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateAnnouncement]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateAnnouncement]
GO

CREATE   PROCEDURE rb_AddAnnouncement
(
    @ModuleID       int,
    @UserName       nvarchar(100),
    @Title          nvarchar(150),
    @MoreLink       nvarchar(150),
    @MobileMoreLink nvarchar(150),
    @ExpireDate     DateTime,
    @Description    nvarchar(2000),
    @ItemID         int OUTPUT
)
AS

INSERT INTO rb_Announcements_st
(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    MoreLink,
    MobileMoreLink,
    ExpireDate,
    Description
)

VALUES
(
    @ModuleID,
    @UserName,
    GETDATE(),
    @Title,
    @MoreLink,
    @MobileMoreLink,
    @ExpireDate,
    @Description
)

SELECT
    @ItemID = @@IDENTITY
GO


CREATE   PROCEDURE rb_DeleteAnnouncement
(
    @ItemID int
)
AS

DELETE FROM
    rb_Announcements_st

WHERE
    ItemID = @ItemID
GO



CREATE   PROCEDURE rb_GetAnnouncements
(
    @ModuleID int,
    @WorkflowVersion int,
    @Page int,
    @RecordsPerPage int
)
AS
-- Find out the first AND last record we want
DECLARE @FirstRec int, @LastRec int

--Create a temporary table
CREATE TABLE #TempItems
(
	ID				int IDENTITY,
 	ItemID				int,
	CreatedByUser			nvarchar(100),
	CreatedDate			datetime,
	Title				nvarchar(150),
	MoreLink			nvarchar(150),
	MobileMoreLink			nvarchar(150),
	ExpireDate			datetime,
	Description			nvarchar(2000)
)


IF ( @WorkflowVersion = 1 )
	INSERT INTO
	#TempItems
	(
		ItemID, CreatedByUser, CreatedDate, Title, MoreLink, MobileMoreLink, ExpireDate, Description
	)
	SELECT
	    ItemID,
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
	FROM 
	    rb_Announcements
	WHERE
	    ModuleID = @ModuleID
	  AND
	    ExpireDate > GETDATE()
	ORDER BY CreatedDate DESC
ELSE
	INSERT INTO
	#TempItems
	(
		ItemID, CreatedByUser, CreatedDate, Title, MoreLink, MobileMoreLink, ExpireDate, Description
	)
	SELECT
	    ItemID,
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
	FROM 
	    rb_Announcements_st
	WHERE
	    ModuleID = @ModuleID
	  AND
	    ExpireDate > GETDATE()
	ORDER BY CreatedDate DESC


SELECT @FirstRec = (@Page - 1) * @RecordsPerPage
SELECT @LastRec = (@Page * @RecordsPerPage + 1)

-- Now, RETURN the SET of paged records, plus, an indiciation of we
-- have more records or not!
SELECT *, (SELECT COUNT(*) FROM #TempItems) RecordCount
FROM #TempItems
WHERE ID > @FirstRec AND ID < @LastRec
ORDER BY ID
GO

CREATE   PROCEDURE rb_GetSingleAnnouncement
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
	FROM
	    rb_Announcements
	WHERE
	    ItemID = @ItemID
ELSE
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
	FROM
	    rb_Announcements_st
	WHERE
	    ItemID = @ItemID
GO

CREATE   PROCEDURE rb_UpdateAnnouncement
(
    @ItemID         int,
    @UserName       nvarchar(100),
    @Title          nvarchar(150),
    @MoreLink       nvarchar(150),
    @MobileMoreLink nvarchar(150),
    @ExpireDate     datetime,
    @Description    nvarchar(2000)
)
AS

UPDATE
    rb_Announcements_st

SET
    CreatedByUser   = @UserName,
    CreatedDate     = GETDATE(),
    Title           = @Title,
    MoreLink        = @MoreLink,
    MobileMoreLink  = @MobileMoreLink,
    ExpireDate      = @ExpireDate,
    Description     = @Description

WHERE
    ItemID = @ItemID
GO

