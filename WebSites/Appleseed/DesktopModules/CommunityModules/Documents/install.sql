/* Install script, Documents module, manudea 27/10/2003  */
/* Changes for correct save documents in datatable. jviladiu@portalServices.net 02/07/2204 */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Documents]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [rb_Documents] (
	[ItemID] [int] IDENTITY (1,1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[FileNameUrl] [nvarchar] (250) NULL ,
	[FileFriendlyName] [nvarchar] (150) NULL ,
	[Category] [nvarchar] (50) NULL ,
	[Content] [image] NULL ,
	[ContentType] [nvarchar] (50) NULL ,
	[ContentSize] [int] NULL ,
	CONSTRAINT [PK_rb_Documents] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Documents_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Documents_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Documents_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[FileNameUrl] [nvarchar] (250) NULL ,
	[FileFriendlyName] [nvarchar] (150) NULL ,
	[Category] [nvarchar] (50) NULL ,
	[Content] [image] NULL ,
	[ContentType] [nvarchar] (50) NULL ,
	[ContentSize] [int] NULL ,
	CONSTRAINT [PK_rb_Documents_st] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Documents_st_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Documents_stModified]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Documents_stModified]
GO

CREATE  TRIGGER [rb_Documents_stModified]
ON [rb_Documents_st]
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

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteDocument]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteDocument]
GO

CREATE   PROCEDURE rb_DeleteDocument
(
    @ItemID int
)
AS

DELETE FROM
    rb_Documents_st
WHERE
    ItemID = @ItemID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetDocumentContent]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetDocumentContent]
GO

CREATE   PROCEDURE rb_GetDocumentContent
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT
	    Content,
	    ContentType,
	    ContentSize,
	    FileFriendlyName,
	    FileNameUrl
	FROM
	    rb_Documents
	WHERE
	    ItemID = @ItemID
ELSE
	SELECT
	    Content,
	    ContentType,
	    ContentSize,
	    FileFriendlyName,
	    FileNameUrl
	FROM
	    rb_Documents_st
	WHERE
	    ItemID = @ItemID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetDocuments]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetDocuments]
GO

CREATE   PROCEDURE rb_GetDocuments
(
    @ModuleID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT
	    ItemID,
	    FileFriendlyName,
	    FileNameUrl,
	    CreatedByUser,
	    CreatedDate,
	    Category,
	    ContentSize,
	    ContentType
	FROM
	    rb_Documents
	WHERE
	    ModuleID = @ModuleID
ELSE
	SELECT
	    ItemID,
	    FileFriendlyName,
	    FileNameUrl,
	    CreatedByUser,
	    CreatedDate,
	    Category,
	    ContentSize,
	    ContentType
	FROM
	    rb_Documents_st
	WHERE
	    ModuleID = @ModuleID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleDocument]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleDocument]
GO

CREATE   PROCEDURE rb_GetSingleDocument
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT
	    FileFriendlyName,
	    FileNameUrl,
	    CreatedByUser,
	    CreatedDate,
	    Category,
	    ContentSize,
	    ContentType
	FROM
	    rb_Documents
	WHERE
	    ItemID = @ItemID
ELSE
	SELECT
	    FileFriendlyName,
	    FileNameUrl,
	    CreatedByUser,
	    CreatedDate,
	    Category,
	    ContentSize,
	    ContentType
	FROM
	    rb_Documents_st
	WHERE
	    ItemID = @ItemID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateDocument]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateDocument]
GO

CREATE PROCEDURE rb_UpdateDocument
(
    @ItemID           int,
    @ModuleID         int,
    @FileFriendlyName nvarchar(150),
    @FileNameUrl      nvarchar(250),
    @UserName         nvarchar(100),
    @Category         nvarchar(50),
    @Content          image,
    @ContentType      nvarchar(50),
    @ContentSize      int
)
AS
IF (@ItemID=0) OR NOT EXISTS (
    SELECT 
        * 
    FROM 
        rb_Documents_st 
    WHERE 
        ItemID = @ItemID
)
INSERT INTO rb_Documents_st
(
    ModuleID,
    FileFriendlyName,
    FileNameUrl,
    CreatedByUser,
    CreatedDate,
    Category,
    Content,
    ContentType,
    ContentSize
)

VALUES
(
    @ModuleID,
    @FileFriendlyName,
    @FileNameUrl,
    @UserName,
    GETDATE(),
    @Category,
    @Content,
    @ContentType,
    @ContentSize
)
ELSE

BEGIN

IF (@ContentSize=0)

UPDATE 
    rb_Documents_st

SET 
    CreatedByUser    = @UserName,
    CreatedDate      = GETDATE(),
    Category         = @Category,
    FileFriendlyName = @FileFriendlyName,
    FileNameUrl      = @FileNameUrl,
    ContentType	     = @ContentType

WHERE
    ItemID = @ItemID
ELSE

UPDATE
    rb_Documents_st

SET
    CreatedByUser     = @UserName,
    CreatedDate       = GETDATE(),
    Category          = @Category,
    FileFriendlyName  = @FileFriendlyName,
    FileNameUrl       = @FileNameUrl,
    Content           = @Content,
    ContentType       = @ContentType,
    ContentSize       = @ContentSize
WHERE
    ItemID = @ItemID
END
GO
