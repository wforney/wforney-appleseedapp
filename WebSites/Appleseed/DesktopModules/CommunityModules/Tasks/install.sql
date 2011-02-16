/* Install script, Tasks module, Jakob Hansen, 29 april 2003 */
/* EHN - Mike Stone - 19 jan 2005 [mstone@kaskaskia.edu] The Description field is no longer restricted to 3000 char. */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Tasks]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Tasks] (
	[ItemID] [int] IDENTITY (1,1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[ModifiedByUser] [nvarchar] (100) NULL ,
	[ModifiedDate] [datetime] NULL ,
	[AssignedTo] [nvarchar] (50) NULL ,
	[Title] [nvarchar] (100) NOT NULL ,
	[Description] [ntext] NULL ,
	[Status] [nvarchar] (20) NULL ,
	[Priority] [nvarchar] (20) NULL ,
	[PercentComplete] [int] NULL ,
	[StartDate] [datetime] NULL ,
	[DueDate] [datetime] NULL ,
	CONSTRAINT [PK_Tasks] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_Tasks_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
/* Esle my Mike Stone to Change the nvarchar 3000 Description field to ntext field. */
ELSE
  BEGIN
	IF NOT EXISTS (select b.length From sysobjects a inner join  syscolumns b ON a.id = b.id where a.name = 'rb_Tasks' and b.name = 'Description' AND b.length = 16) 
    BEGIN 
     ALTER TABLE [rb_Tasks] ALTER COLUMN [Description] ntext
    END
  END
GO

-- INSERT templated records for tasks module
--INSERT INTO rb_TASKS (ModuleID,Title) VALUES (0,' ')
--GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddTask]
GO

CREATE PROCEDURE rb_AddTask
(
    @ModuleID int,
    @UserName nvarchar(100),
    @AssignedTo     nvarchar(50),
    @Title     nvarchar(100),
    @Description    ntext,
    @Status nvarchar(20),
    @Priority nvarchar(20),
    @PercentComplete int,
    @StartDate datetime,
    @DueDate datetime,
    @ItemID   int OUTPUT
)
AS

INSERT INTO rb_Tasks
(
    CreatedByUser,
    CreatedDate,
    ModifiedByUser,
    ModifiedDate,
    ModuleID,
    AssignedTo,
    Title,
    Description,
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate
)

VALUES
(
    @UserName,
    GETDATE(),
    @UserName,
    GETDATE(),
    @ModuleID,
    @AssignedTo,
    @Title,
    @Description,
    @Status,
    @Priority,
    @PercentComplete,
    @StartDate,
    @DueDate
)

SELECT
    @ItemID = @@IDENTITY
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteTask]
GO

CREATE PROCEDURE rb_DeleteTask
(
    @ItemID int
)
AS

DELETE FROM
    rb_Tasks

WHERE
    ItemID = @ItemID
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetTasks]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetTasks]
GO


CREATE PROCEDURE rb_GetTasks
(
    @ModuleID int
)
AS

SELECT
    ItemID,
    CreatedByUser,
    CreatedDate,
    AssignedTo,
    Title,
    Description,
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate,
    ModifiedDate

FROM
    rb_Tasks

WHERE
    ModuleID = @ModuleID
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleTask]
GO

CREATE PROCEDURE rb_GetSingleTask
(
    @ItemID int
)
AS

SELECT
    ItemID,
    CreatedByUser,
    CreatedDate,
    ModifiedByUser,
    ModifiedDate,
    AssignedTo,
    Title,
    Description,
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate

FROM
    rb_Tasks

WHERE
    ItemID = @ItemID
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateTask]
GO

CREATE PROCEDURE rb_UpdateTask
(
    @ItemID int,
    @UserName nvarchar(100),
    @AssignedTo     nvarchar(50),
    @Title     nvarchar(100),
    @Description    ntext,
    @Status nvarchar(20),
    @Priority nvarchar(20),
    @PercentComplete int,
    @StartDate datetime,
    @DueDate datetime
)
AS

UPDATE
    rb_Tasks

SET
    ModifiedByUser   = @UserName,
    ModifiedDate     = GETDATE(),
    AssignedTo      = @AssignedTo,
    Title           = @Title,
    Description     = @Description,
    Status          = @Status,
    Priority        = @Priority,
    PercentComplete = @PercentComplete,
    StartDate       = @StartDate,
    DueDate         = @DueDate

WHERE
    ItemID = @ItemID
GO

