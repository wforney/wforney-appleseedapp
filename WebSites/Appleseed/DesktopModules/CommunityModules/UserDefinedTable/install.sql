/* Install script, UserDefinedTable module, Jakob Hansen, 1 may 2003 */
/* FIX:Bug [ 833693 ] User Defined Tables Procedure Error, 04 June 2004 */
IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UserDefinedData]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
-- Create the table AND add constraints
CREATE TABLE [rb_UserDefinedData] (
	[UserDefinedDataID] [int] IDENTITY (1, 1) NOT NULL ,
	[UserDefinedFieldID] [int] NOT NULL ,
	[UserDefinedRowID] [int] NOT NULL ,
	[FieldValue] [nvarchar] (2000) NOT NULL 
) ON [PRIMARY]

CREATE TABLE [rb_UserDefinedFields] (
	[UserDefinedFieldID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[FieldTitle] [varchar] (50) NOT NULL ,
	[Visible] [bit] NOT NULL ,
	[FieldOrder] [int] NOT NULL ,
	[FieldType] [varchar] (20) NOT NULL 
) ON [PRIMARY]

CREATE TABLE [rb_UserDefinedRows] (
	[UserDefinedRowID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL 
) ON [PRIMARY]

ALTER TABLE [rb_UserDefinedData] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserDefinedData] PRIMARY KEY  CLUSTERED 
	(
		[UserDefinedDataID]
	)  ON [PRIMARY] 

ALTER TABLE [rb_UserDefinedFields] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserDefinedTable] PRIMARY KEY  CLUSTERED 
	(
		[UserDefinedFieldID]
	)  ON [PRIMARY] 

ALTER TABLE [rb_UserDefinedRows] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserDefinedRows] PRIMARY KEY  CLUSTERED 
	(
		[UserDefinedRowID]
	)  ON [PRIMARY] 

ALTER TABLE [rb_UserDefinedFields] WITH NOCHECK ADD 
	CONSTRAINT [DF_UserDefinedFields_FieldOrder] DEFAULT (0) FOR [FieldOrder]

ALTER TABLE [rb_UserDefinedData] ADD 
	CONSTRAINT [FK_UserDefinedData_UserDefinedFields] FOREIGN KEY 
	(
		[UserDefinedFieldID]
	) REFERENCES [rb_UserDefinedFields] (
		[UserDefinedFieldID]
	) ON DELETE CASCADE  NOT FOR REPLICATION ,
	CONSTRAINT [FK_UserDefinedData_UserDefinedRows] FOREIGN KEY 
	(
		[UserDefinedRowID]
	) REFERENCES [rb_UserDefinedRows] (
		[UserDefinedRowID]
	) NOT FOR REPLICATION 

ALTER TABLE [rb_UserDefinedFields] ADD 
	CONSTRAINT [FK_UserDefinedFields_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 

ALTER TABLE [rb_UserDefinedRows] ADD 
	CONSTRAINT [FK_UserDefinedRows_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddUserDefinedField]
GO

CREATE PROCEDURE rb_AddUserDefinedField
@ModuleID     int,
@FieldTitle   varchar(50),
@Visible      bit,
@FieldType    varchar(20)

AS

DECLARE @FieldOrder int

SELECT @FieldOrder = COUNT(*) + 1
FROM   rb_UserDefinedFields
WHERE  ModuleID = @ModuleID

INSERT rb_UserDefinedFields ( 
  ModuleID,
  FieldTitle,
  Visible,
  FieldOrder,
  FieldType
)
VALUES (
  @ModuleID,
  @FieldTitle,
  @Visible,
  @FieldOrder,
  @FieldType
)

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddUserDefinedRow]
GO

CREATE PROCEDURE rb_AddUserDefinedRow

@ModuleID         int,
@UserDefinedRowID int OUTPUT

AS

INSERT rb_UserDefinedRows ( 
  ModuleID
)
VALUES (
  @ModuleID
)

SELECT @UserDefinedRowID = @@IDENTITY
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteUserDefinedField]
GO

CREATE PROCEDURE rb_DeleteUserDefinedField

@UserDefinedFieldID    int 

AS

DELETE 
FROM   rb_UserDefinedData
WHERE  UserDefinedFieldID = @UserDefinedFieldID

DELETE 
FROM   rb_UserDefinedFields
WHERE  UserDefinedFieldID = @UserDefinedFieldID

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteUserDefinedRow]
GO

CREATE PROCEDURE rb_DeleteUserDefinedRow

@UserDefinedRowID    int 

AS

DELETE 
FROM   rb_UserDefinedData
WHERE  UserDefinedRowID = @UserDefinedRowID

DELETE 
FROM   rb_UserDefinedRows
WHERE  UserDefinedRowID = @UserDefinedRowID

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleUserDefinedField]
GO

CREATE PROCEDURE rb_GetSingleUserDefinedField

@UserDefinedFieldID  int

AS

SELECT ModuleID,
       FieldTitle,
       Visible,
       FieldOrder
FROM   rb_UserDefinedFields
WHERE  UserDefinedFieldID = @UserDefinedFieldID

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleUserDefinedRow]
GO

CREATE PROCEDURE rb_GetSingleUserDefinedRow

@UserDefinedRowID   int,
@ModuleID           int

AS

SELECT rb_UserDefinedFields.FieldTitle,
       rb_UserDefinedData.FieldValue
FROM   rb_UserDefinedData
INNER JOIN rb_UserDefinedFields ON rb_UserDefinedData.UserDefinedFieldID = rb_UserDefinedFields.UserDefinedFieldID
WHERE  rb_UserDefinedData.UserDefinedRowID = @UserDefinedRowID
AND    rb_UserDefinedFields.ModuleID = @ModuleID
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetUserDefinedFields]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetUserDefinedFields]
GO

CREATE PROCEDURE rb_GetUserDefinedFields

@ModuleID  int

AS

SELECT UserDefinedFieldID,
       FieldTitle,
       Visible,
       FieldType
FROM   rb_UserDefinedFields
WHERE  ModuleID = @ModuleID
ORDER BY FieldOrder

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetUserDefinedRows]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetUserDefinedRows]
GO

CREATE PROCEDURE rb_GetUserDefinedRows

@ModuleID    int 

AS

SELECT rb_UserDefinedRows.UserDefinedRowID,
       rb_UserDefinedFields.FieldTitle,
       rb_UserDefinedData.FieldValue
FROM   rb_UserDefinedRows
LEFT OUTER JOIN rb_UserDefinedData ON rb_UserDefinedRows.UserDefinedRowID = rb_UserDefinedData.UserDefinedRowID
INNER JOIN rb_UserDefinedFields ON rb_UserDefinedData.UserDefinedFieldID = rb_UserDefinedFields.UserDefinedFieldID 
WHERE  rb_UserDefinedRows.ModuleID = @ModuleID
ORDER BY rb_UserDefinedData.UserDefinedRowID, 
rb_UserDefinedData.UserDefinedFieldID


RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedData]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedData]
GO

CREATE PROCEDURE rb_UpdateUserDefinedData

@UserDefinedRowID    int,
@UserDefinedFieldID  int,
@FieldValue          nvarchar(2000) = null

AS

IF @FieldValue is null
BEGIN
  IF EXISTS ( SELECT 1 FROM rb_UserDefinedData WHERE UserDefinedFieldID = @UserDefinedFieldID AND UserDefinedRowID = @UserDefinedRowID )
  BEGIN
    DELETE
    FROM rb_UserDefinedData
    WHERE UserDefinedFieldID = @UserDefinedFieldID
    AND UserDefinedRowID = @UserDefinedRowID
  END
END
ELSE
BEGIN
  IF NOT EXISTS ( SELECT 1 FROM rb_UserDefinedData WHERE UserDefinedFieldID = @UserDefinedFieldID AND UserDefinedRowID = @UserDefinedRowID )
  BEGIN
    INSERT rb_UserDefinedData ( 
      UserDefinedFieldID,
      UserDefinedRowID,
      FieldValue
    )
    VALUES (
      @UserDefinedFieldID,
      @UserDefinedRowID,
      @FieldValue
    )
  END
  ELSE
  BEGIN
    update rb_UserDefinedData
    SET    FieldValue = @FieldValue
    WHERE UserDefinedFieldID = @UserDefinedFieldID
    AND UserDefinedRowID = @UserDefinedRowID
  END
END

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedField]
GO

CREATE PROCEDURE rb_UpdateUserDefinedField

@UserDefinedFieldID   int,
@FieldTitle           varchar(50),
@Visible              bit,
@FieldType            varchar(20)

AS

update rb_UserDefinedFields
SET    FieldTitle = @FieldTitle,
       Visible = @Visible,
       FieldType = @FieldType
WHERE  UserDefinedFieldID = @UserDefinedFieldID

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedFieldOrder]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedFieldOrder]
GO

CREATE PROCEDURE rb_UpdateUserDefinedFieldOrder

@UserDefinedFieldID  int,
@Direction           int

AS

DECLARE @ModuleID int
DECLARE @FieldOrder int

SELECT @ModuleID = ModuleID,
       @FieldOrder = FieldOrder
FROM   rb_UserDefinedFields
WHERE  UserDefinedFieldID = @UserDefinedFieldID

IF (@Direction = -1 AND @FieldOrder > 0) or (@Direction = 1 AND @FieldOrder < ( SELECT (COUNT(*) - 1) FROM rb_UserDefinedFields WHERE ModuleID = @ModuleID ))
BEGIN
  update rb_UserDefinedFields
  SET    FieldOrder = @FieldOrder
  WHERE  ModuleID = @ModuleID
  AND    FieldOrder = @FieldOrder + @Direction

  update rb_UserDefinedFields
  SET    FieldOrder = @FieldOrder + @Direction
  WHERE  UserDefinedFieldID = @UserDefinedFieldID
END

RETURN 1
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedRow]
GO

CREATE PROCEDURE rb_UpdateUserDefinedRow

@UserDefinedRowID int

AS

IF NOT EXISTS ( SELECT 1 FROM rb_UserDefinedData WHERE UserDefinedRowID = @UserDefinedRowID )
BEGIN
  DELETE
  FROM   rb_UserDefinedRows
  WHERE  UserDefinedRowID = @UserDefinedRowID
END
GO
