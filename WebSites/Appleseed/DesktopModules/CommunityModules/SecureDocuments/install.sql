
/** insert the module into Appleseed's list ***************************************/

-- INSERT the record

IF(NOT EXISTS(SELECT GeneralModDefID FROM [rb_GeneralModuleDefinitions] WHERE GeneralModDefID = '{08E7FDAD-3033-49b8-9B10-BA7EC7AF5415}'
BEGIN
 INSERT INTO [dbo].[rb_GeneralModuleDefinitions]
 (
	[GeneralModDefID], 
	[FriendlyName], 
	[DesktopSrc], 
	[MobileSrc], 
	[AssemblyName], 
	[ClassName], 
	[Admin], 
	[Searchable]
 )

 VALUES
 (
	'{08E7FDAD-3033-49b8-9B10-BA7EC7AF5415}',					-- GUID
	'Secure Documents',								-- FriendlyName
	'DesktopModules/SecureDocuments/SecureDocuments.ascx',	-- DesktopSrc
	'',															-- MobileSrc
	'Appleseed.Modules.SecureDocuments.DLL',					-- AssemblyName
	'Appleseed.Content.Web.ModulesSecureDocuments',				-- ClassName
	0, 															-- Admin
	0															-- Searchable
 ) 
END
/*******************************************************************************/

/** create tables used by this module ****************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[rb_SecureDocumentFiles_stModified]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[rb_SecureDocuments_stModified]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentFiles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentFiles_st]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocuments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentsAccess]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentsAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocuments_st]
GO

CREATE TABLE [dbo].[rb_SecureDocumentFiles] (
	[ItemId] [int] NOT NULL ,
	[ModuleId] [int] NOT NULL ,
	[DocumentId] [int] NOT NULL ,
	[FileName] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[FileNameUrl] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Content] [image] NULL ,
	[ContentType] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ContentSize] [int] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[rb_SecureDocumentFiles_st] (
	[ItemId] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleId] [int] NOT NULL ,
	[DocumentId] [int] NOT NULL ,
	[FileName] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[FileNameUrl] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Content] [image] NULL ,
	[ContentType] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ContentSize] [int] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[rb_SecureDocuments] (
	[ItemID] [int] NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[CreatedDate] [datetime] NULL ,
	[FileFriendlyName] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Category] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[rb_SecureDocumentsAccess] (
	[itemId] [int] NOT NULL ,
	[groupId] [int] NOT NULL ,
	[granted] [bit] NOT NULL DEFAULT 1,
	[denied] [bit] NOT NULL DEFAULT 0
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[rb_SecureDocuments_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[CreatedDate] [datetime] NULL ,
	[FileFriendlyName] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Category] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY] 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Trigger dbo.rb_SecureDocuments_stModified    Script Date: 11/17/2004 1:20:01 PM ******/
/****** Object:  Trigger dbo.rb_SecureDocuments_stModified    Script Date: 11/10/2004 3:41:13 PM ******/
CREATE  TRIGGER [rb_SecureDocumentFiles_stModified]
ON dbo.rb_SecureDocumentFiles_st
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
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



/****** Object:  Trigger dbo.rb_SecureDocuments_stModified    Script Date: 11/17/2004 1:20:01 PM ******/
/****** Object:  Trigger dbo.rb_SecureDocuments_stModified    Script Date: 11/10/2004 3:41:13 PM ******/
CREATE  TRIGGER [rb_SecureDocuments_stModified]
ON [rb_SecureDocuments_st]
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
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/*******************************************************************************/

/** create tables used by this module ****************************************/

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_AddSecureDocumentAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_AddSecureDocumentAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocumentAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocumentAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocumentFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocumentFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocumentContent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocumentContent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocumentFiles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocumentFiles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocuments]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocuments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSingleSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSingleSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSingleSecureDocumentRoles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSingleSecureDocumentRoles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_UpdateSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_UpdateSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_UpdateSecureDocumentFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_UpdateSecureDocumentFile]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Add access for a role to a secure document
 *
 * In:		@ItemId	- Id of the secure document
 *		@GroupdID  - Id of the role
 * Out:		none
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE PROCEDURE rb_AddSecureDocumentAccess
(
	@ItemID		int, 
	@GroupID	int
) AS

INSERT INTO
	rb_SecureDocumentsAccess 

	(itemid, groupid)
VALUES
	(@ItemID,@groupid)
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Delete a secure document, its files and permissions
 *
 * In:		@ItemId	- Id of the item to delete
 * Out:		none
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_DeleteSecureDocument
(
    @ItemID int
)
AS

SET NOCOUNT ON

DECLARE @Error	int

-- do a tx since we are delete from 3 tables
BEGIN TRAN tranDelete

-- delete the secure doc record
DELETE FROM
    rb_SecureDocuments_st
WHERE
    ItemID = @ItemID

SELECT @Error = @@ERROR

-- delete access records
IF @Error = 0
 BEGIN
	DELETE FROM
		rb_SecureDocumentsAccess
	WHERE
		ItemId = @ItemId

	SELECT @Error = @@ERROR
 END

-- delete files
IF @Error = 0
 BEGIN
	DELETE FROM
		rb_SecureDocumentFiles_st
	WHERE
		DocumentId = @ItemId

	SELECT @Error = @@ERROR
END

IF @Error = 0
	COMMIT TRAN tranDelete
ELSE
	ROLLBACK TRAN tranDelete
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Delete all access records for a secure document
 *
 * In:		@ItemId	- Id of the item 
 * Out:		none
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE PROCEDURE rb_DeleteSecureDocumentAccess
(
	@itemID	int
) AS

DELETE FROM 
	rb_SecureDocumentsAccess 
WHERE
	itemid = @itemid
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Delete a secure document file record
 *
 * In:		@ItemID	- Id of the record to be deleted
 * Out:		@FilenameUrl - Url of the file that was deleted
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_DeleteSecureDocumentFile
(
	@ItemID		int,
	@FileNameUrl	nvarchar(250)	output
	
)
AS

SET NOCOUNT ON

-- get the url of the file to pass back
SELECT	@FileNameUrl = FileNameUrl
FROM	rb_SecureDocumentFiles_st
WHERE	ItemId = @ItemId

-- delete the record
DELETE FROM
	rb_SecureDocumentFiles_st
WHERE
	ItemId = @ItemId
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Get the specified file, with permissions
 *
 * In:		@ItemId	- Id of the item 
 *		@WorkflowVersion - toggles stage or production versions
 *		@UserId - UserId used to test permissions
 * Out:		SecureDocumentFiles record

 * Out:		none
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_GetSecureDocumentContent
(
	@ItemID			int,
	@WorkflowVersion	int
)
AS

SET NOCOUNT ON


IF ( @WorkflowVersion = 1 )
	SELECT
	   	f.Content, f.ContentType, f.ContentSize, f.FileNameUrl, f.FileName
	FROM
	   	rb_SecureDocumentFiles f
	WHERE
		f.ItemID = @ItemID

ELSE
	SELECT
	   	f.Content, f.ContentType, f.ContentSize, f.FileNameUrl, f.FileName
	FROM
	   	rb_SecureDocumentFiles_st f
	WHERE
		f.ItemID = @ItemID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Get a list of files associated with a secure document,
 *		taking permissions into account. If the user is a
 *		member of 'Admin' return all records.
 *
 * In:		@DocumentId - Id of the secure document
 *		@WorkflowVersion - toggles stage or production versions
 *		@UserId - UserId used to test permissions
 * Out:		SecureDocumentFiles record(s)
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_GetSecureDocumentFiles
(
	@DocumentID		int,
	@WorkflowVersion	int
)
AS

SET NOCOUNT ON

IF ( @WorkflowVersion = 1 )
	SELECT
		sd.ItemID, sd.FileName, sd.FileNameUrl, Content, ContentType, ContentSize
	FROM
		rb_SecureDocumentFiles sd
	WHERE
		sd.DocumentId = @DocumentID

ELSE
	SELECT
		sd.ItemID, sd.FileName, sd.FileNameUrl, Content, ContentType, ContentSize
	FROM
		rb_SecureDocumentFiles_st sd
	WHERE
		sd.DocumentId = @DocumentID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Get a list of secure document,
 *		taking permissions into account. If the user is a
 *		member of 'Admin' return all records.
 *
 * In:		@ModuleID - Id of the secure document module 
 *		@WorkflowVersion - toggles stage or production versions
 *		@UserId - UserId used to test permissions
 * Out:		SecureDocumentFiles record(s)
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_GetSecureDocuments
(
	@ModuleID 		int,
	@WorkflowVersion	int,
	@UserID 		int
)
AS

SET NOCOUNT ON

/* 
	Deal with special cases

	AllUsers = -1,			- show all records, regardless of userid
	AuthenticatedUsers = -2,	- show all records where userid != 0
	UnauthenticatedUsers = -3	- show all records if userid = 0
*/


-- Test the UserId to see if its an admin user, if yes, show all records
IF EXISTS( 
	SELECT	r.RoleId 
	FROM	rb_Roles r 
	INNER JOIN 
		rb_UserRoles ur 
	ON	r.RoleID = ur.RoleID 
	AND	ur.UserID = @UserID 
	AND	r.RoleName='Admins'
)
 BEGIN
	IF ( @WorkflowVersion = 1 )
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments sd
		WHERE
			sd.ModuleID = @ModuleID

	ELSE
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments_st sd
		WHERE
			sd.ModuleID = @ModuleID

	RETURN
END		




-- otherwise, get records visible to this user
IF ( @WorkflowVersion = 1 )
 BEGIN
	
		-- get all docs in table with a role matching a role 
		-- that userid is assigned too
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments sd
		
		INNER JOIN
			rb_SecureDocumentsAccess sda
		ON
			sda.ItemId = sd.ItemId
		AND	sd.ModuleID = @ModuleID
		
		INNER JOIN
			rb_UserRoles ur
		ON
			ur.UserID = @UserID
		AND	ur.RoleID = sda.GroupId

	UNION
		-- get special cases
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments_st sd
		
		INNER JOIN
			rb_SecureDocumentsAccess sda
		ON
			sda.ItemId = sd.ItemId
		AND	sd.ModuleID = @ModuleID
		AND(	sda.GroupId = -1	-- All Users
		OR	(@UserID != 0		
		AND	sda.GroupId = -2)	-- AuthenticatedUsers
		OR	(@UserID = 0
		AND	sda.GroupId = -3))	-- UnauthenticatedUsers

		
 END
ELSE
 BEGIN
		-- get all docs in table with a role matching a role 
		-- that userid is assigne too
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments_st sd
		
		INNER JOIN
			rb_SecureDocumentsAccess sda
		ON
			sda.ItemId = sd.ItemId
		AND	sd.ModuleID = @ModuleID
		
		INNER JOIN
			rb_UserRoles ur
		ON	ur.UserID = @UserID
		AND	ur.RoleID = sda.GroupId

	UNION
		-- get special cases
		SELECT
			sd.ItemID, sd.FileFriendlyName, sd.CreatedByUser,
			sd.CreatedDate, sd.Category
		FROM
			rb_SecureDocuments_st sd
		
		INNER JOIN
			rb_SecureDocumentsAccess sda
		ON
			sda.ItemId = sd.ItemId
		AND	sd.ModuleID = @ModuleID
		AND(	sda.GroupId = -1	-- All Users
		OR	(@UserID != 0		
		AND	sda.GroupId = -2)	-- AuthenticatedUsers
		OR	(@UserID = 0
		AND	sda.GroupId = -3))	-- UnauthenticatedUsers
		
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Get a secure document,
 *		taking permissions into account. 
 *
 * In:		@ItemId - Id of the secure document 
 *		@WorkflowVersion - toggles stage or production versions
 *		@UserId - UserId used to test permissions
 * Out:		SecureDocumentFiles record(s)
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_GetSingleSecureDocument
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT	sd.ItemID, sd.ModuleID,
		sd.FileFriendlyName, sd.CreatedByUser, sd.CreatedDate, sd.Category
	FROM	rb_SecureDocuments sd
	WHERE	sd.ItemID = @ItemID
ELSE
	SELECT	sd.ItemID, sd.ModuleID,
		sd.FileFriendlyName, sd.CreatedByUser, sd.CreatedDate, sd.Category
	FROM	rb_SecureDocuments_st sd
	WHERE	sd.ItemID = @ItemID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Retrieves the roles information for a single 
 *		secure document.
 *
 * In:		@ItemId	- Id of the item to retrieve roles for
 *
 * Out:		Role record(s)
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE   PROCEDURE rb_GetSingleSecureDocumentRoles
(
    @ItemID int
)
AS

SELECT 
	sda.GroupID, r.RoleName
FROM
	rb_SecureDocumentsAccess sda
left JOIN
	rb_Roles r
ON	sda.GroupId = r.RoleID

WHERE	sda.ItemID = @ItemID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Adds or updates a secure documents record.
 *
 * In:		@ItemId	- Id of the item to update, 0 for new
 *		@ModuleID - Id of the module
 *		@FileFriendlyName - Name of secure doc
 *		@UserName - creator of the file
 *		@Category - Category name
 * Out:		@RecordId - Id of the record updated or created
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE PROCEDURE rb_UpdateSecureDocument
(
    	@ItemID           	int,
    	@ModuleID         	int,
    	@FileFriendlyName 	nvarchar(150),
    	@UserName         	nvarchar(100),
    	@Category         	nvarchar(50),
	@RecordId		int	 	output
)
AS

SET NOCOUNT ON

-- default value
SELECT @RecordId = 0

-- is this an add?
IF (@ItemID=0) OR NOT EXISTS (SELECT ItemId FROM rb_SecureDocuments_st WHERE ItemID = @ItemID)
 BEGIN
	-- add the record
	INSERT INTO 
		rb_SecureDocuments_st
	
		(ModuleID, FileFriendlyName, CreatedByUser, CreatedDate, Category)
	VALUES
		(@ModuleID,@FileFriendlyName,@UserName,GetDate(),@Category)

	-- get the id
	SELECT @RecordId = @@IDENTITY
 END
ELSE
 BEGIN
	-- its an update
	SELECT @RecordId = @ItemId

	UPDATE 
		rb_SecureDocuments_st
	SET
		CreatedByUser    = @UserName,
		CreatedDate      = GetDate(),
		Category         = @Category,
		FileFriendlyName = @FileFriendlyName
	WHERE
			ItemID = @ItemID
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

/*****************************************************************
 *
 * By:		brian kierstead
 * Date:	2005.04.14
 *
 * Purpose:	Adds or updates a file for a given secure documents
 *		record.
 *
 * In:		@ItemId	- Id of the item to update, 0 for new
 *		@DocumentID - Id of the secure doc this file is associated with
 *		@ModuleID - Id of the module 
 *		@FileName - Name of the file, no path info
 *		@FileNameUrl - Full URL to the file
 *		@Content - File contents, stored as binary
 *		@ContentType - Type of the file
 *		@ContentSize - Size of the file
 * Out:		none
 *
 * RETURN:	none
 *
 ******************************************************************/
CREATE PROCEDURE rb_UpdateSecureDocumentFile
(
	@ItemId		int,
	@DocumentID	int,
	@ModuleID	int,
	@FileName	nvarchar(250),
	@FileNameUrl	nvarchar(250),
	@Content	image,
	@ContentType	nvarchar(50),
	@ContentSize	int
)
AS

SET NOCOUNT ON

-- is this an add?
IF (@ItemID=0) OR NOT EXISTS (SELECT ItemId FROM rb_SecureDocumentFiles_st WHERE ItemID = @ItemID)
 BEGIN
	-- add the record
	INSERT INTO 
		rb_SecureDocumentFiles_st
	
		(ModuleID, DocumentID, FileName, FileNameUrl, Content, ContentType, ContentSize )
	VALUES
		(@ModuleID, @DocumentID, @FileName, @FileNameUrl, @Content, @ContentType, @ContentSize)

 END
ELSE
 BEGIN
	IF(@ContentSize=0)
		UPDATE 
			rb_SecureDocumentFiles_st
		SET
			FileNameUrl      = @FileNameUrl
		WHERE
			ItemID = @ItemID
	ELSE
		UPDATE
			rb_SecureDocumentFiles_st
		SET
			FileName	  = @FileName,
			FileNameUrl       = @FileNameUrl,
			Content           = @Content,
			ContentType       = @ContentType,
			ContentSize       = @ContentSize
		WHERE
			ItemID = @ItemID
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

/*******************************************************************************/