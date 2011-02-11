/* Install script, Contacts module, manudea 27/10/2003  */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Contacts]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Contacts] (
	[ItemID] [int] IDENTITY (1,1)NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Name] [nvarchar] (50) NULL ,
	[Role] [nvarchar] (100) NULL ,
	[Email] [nvarchar] (100) NULL ,
	[Contact1] [nvarchar] (250) NULL ,
	[Contact2] [nvarchar] (250) NULL ,
	CONSTRAINT [PK_rb_Contacts] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Contacts_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC ON SO.id=SC.id WHERE SO.id = OBJECT_ID(N'[rb_Contacts]') AND OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 AND SC.name=N'Fax')
BEGIN
ALTER TABLE [rb_Contacts] WITH NOCHECK ADD 
	[Fax] [nvarchar] (250) NULL 
END
GO


IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC ON SO.id=SC.id WHERE SO.id = OBJECT_ID(N'[rb_Contacts]') AND OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 AND SC.name=N'Address')
BEGIN
ALTER TABLE [rb_Contacts] WITH NOCHECK ADD 
	[Address] [nvarchar] (250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Contacts_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Contacts_st] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Name] [nvarchar] (50) NULL ,
	[Role] [nvarchar] (100) NULL ,
	[Email] [nvarchar] (100) NULL ,
	[Contact1] [nvarchar] (250) NULL ,
	[Contact2] [nvarchar] (250) NULL ,
	CONSTRAINT [PK_rb_Contacts_st] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Contacts_st_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC ON SO.id=SC.id WHERE SO.id = OBJECT_ID(N'[rb_Contacts_st]') AND OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 AND SC.name=N'Address')
BEGIN
ALTER TABLE [rb_Contacts_st] WITH NOCHECK ADD 
	[Address] [nvarchar] (250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects SO INNER JOIN syscolumns SC ON SO.id=SC.id WHERE SO.id = OBJECT_ID(N'[rb_Contacts_st]') AND OBJECTPROPERTY(SO.id, N'IsUserTable') = 1 AND SC.name=N'Fax')
BEGIN
ALTER TABLE [rb_Contacts_st] WITH NOCHECK ADD 
	[Fax] [nvarchar] (250) NULL 
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Contacts_stModified]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Contacts_stModified]
GO

CREATE  TRIGGER [rb_Contacts_stModified]
ON [rb_Contacts_st]
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

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddContact]
GO


CREATE PROCEDURE rb_AddContact
(
    @ModuleID int,
    @UserName nvarchar(100),
    @Name     nvarchar(50),
    @Role     nvarchar(100),
    @Email    nvarchar(100),
    @Contact1 nvarchar(250),
    @Contact2 nvarchar(250),
    @Fax nvarchar(250),
    @Address nvarchar(250),
    @ItemID   int OUTPUT
)
AS

INSERT INTO rb_Contacts_st
(
    CreatedByUser,
    CreatedDate,
    ModuleID,
    Name,
    Role,
    Email,
    Contact1,
    Contact2,
    Fax,
    Address
)

VALUES
(
    @UserName,
    GETDATE(),
    @ModuleID,
    @Name,
    @Role,
    @Email,
    @Contact1,
    @Contact2,
    @Fax,
    @Address
)

SELECT
    @ItemID = @@IDENTITY

GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteContact]
GO


CREATE   PROCEDURE rb_DeleteContact
(
    @ItemID int
)
AS

DELETE FROM
    rb_Contacts_st

WHERE
    ItemID = @ItemID

GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetContacts]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetContacts]
GO


CREATE PROCEDURE rb_GetContacts
(
    @ModuleID int,
    @WorkflowVersion int
)
AS

IF (@WorkflowVersion = 1)
	SELECT
	    ItemID,
	    CreatedDate,
	    CreatedByUser,
	    Name,
	    Role,
	    Email,
	    Contact1,
	    Contact2,
	    Fax,
	    Address
	FROM
	    rb_Contacts
	WHERE
	    ModuleID = @ModuleID
	ORDER BY Name
ELSE
	SELECT
	    ItemID,
	    CreatedDate,
	    CreatedByUser,
	    Name,
	    Role,
	    Email,
	    Contact1,
	    Contact2,
	    Fax,
	    Address
	FROM
	    rb_Contacts_st
	WHERE
	    ModuleID = @ModuleID
	ORDER BY Name

GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleContact]
GO


CREATE PROCEDURE rb_GetSingleContact
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF (@WorkflowVersion = 1)
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Name,
	    Role,
	    Email,
	    Contact1,
	    Contact2,
	    Fax,
	    Address
	FROM
	    rb_Contacts
	WHERE
	    ItemID = @ItemID
ELSE
	SELECT
	    CreatedByUser,
	    CreatedDate,
	    Name,
	    Role,
	    Email,
	    Contact1,
	    Contact2,
	    Fax,
	    Address
	FROM
	    rb_Contacts_st
	WHERE
	    ItemID = @ItemID

GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateContact]
GO


CREATE PROCEDURE rb_UpdateContact
(
    @ItemID   int,
    @UserName nvarchar(100),
    @Name     nvarchar(50),
    @Role     nvarchar(100),
    @Email    nvarchar(100),
    @Contact1 nvarchar(250),
    @Contact2 nvarchar(250),
    @Fax nvarchar(250),
    @Address nvarchar(250)
)
AS

UPDATE
    rb_Contacts_st

SET
    CreatedByUser = @UserName,
    CreatedDate   = GETDATE(),
    Name          = @Name,
    Role          = @Role,
    Email         = @Email,
    Contact1      = @Contact1,
    Contact2      = @Contact2,
    Fax			  = @Fax,
    Address       = @Address

WHERE
    ItemID = @ItemID

GO

