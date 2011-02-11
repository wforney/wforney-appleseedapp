/* Install script, MileStones, [Mario@Hartmann.net], 27/05/2003 */
/* Updated to allow deletion from a tab, john.mandia@whitelightsolutions.com 03/07/04 */
IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Milestones]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [rb_Milestones] (
		[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
		[ModuleID] [int] NULL ,
		[CreatedByUser] [nvarchar] (100) NULL ,
		[CreatedDate] [datetime] NULL ,
		[Title] [nvarchar] (100) NULL ,
		[EstCompleteDate] [datetime] NULL ,
		[Status] [nvarchar] (100) NULL 
	) ON [PRIMARY]

	ALTER TABLE [rb_Milestones] WITH NOCHECK ADD 
		CONSTRAINT [PK_rb_Milestones] PRIMARY KEY  CLUSTERED 
		(
			[ItemID]
		)  ON [PRIMARY] 

	ALTER TABLE [rb_Milestones] ADD 
		CONSTRAINT [FK_rb_Milestones_rb_Modules] FOREIGN KEY 
		(
			[ModuleID]
		) REFERENCES [rb_Modules] (
			[ModuleID]
		)

	CREATE TABLE [rb_Milestones_st] (
		[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
		[ModuleID] [int] NULL ,
		[CreatedByUser] [nvarchar] (100) NULL ,
		[CreatedDate] [datetime] NULL ,
		[Title] [nvarchar] (100) NULL ,
		[EstCompleteDate] [datetime] NULL ,
		[Status] [nvarchar] (100) NULL 
	) ON [PRIMARY]
END
GO



IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Milestones_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1 AND (deltrig <> 0 OR instrig <> 0 OR updtrig <> 0))
DROP TRIGGER [rb_Milestones_stModified]
GO

CREATE TRIGGER [rb_Milestones_stModified]
ON [rb_Milestones_st]
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

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddMilestones]
GO

/* Procedure rb_AddMilestones*/
CREATE PROCEDURE rb_AddMilestones
	@ItemID int OUTPUT,
	@ModuleID int,
	@CreatedByUser nvarchar(100),
	@CreatedDate datetime,
	@Title nvarchar(100),
	@EstCompleteDate datetime,
	@Status nvarchar(100)
AS
INSERT INTO rb_Milestones_st
(
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	EstCompleteDate,
	Status
)
VALUES
(
	@ModuleID,
	@CreatedByUser,
	@CreatedDate,
	@Title,
	@EstCompleteDate,
	@Status
)
SELECT
	@ItemID = @@IDENTITY
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteMilestones]
GO

/* Procedure rb_DeleteMilestones*/
CREATE PROCEDURE rb_DeleteMilestones
@ItemID int
AS
DELETE
FROM
	rb_Milestones_st
WHERE
	ItemID = @ItemID
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetMilestones]
GO


/* Procedure rb_GetMilestones*/
CREATE PROCEDURE rb_GetMilestones
(
    @ModuleID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
SELECT
	ItemID,
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	EstCompleteDate,
	Status
FROM
	rb_Milestones
WHERE
	ModuleID = @ModuleID
	ORDER BY CreatedDate DESC
ELSE
	SELECT
	ItemID,
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	EstCompleteDate,
	Status
FROM
	rb_Milestones_st
WHERE
	ModuleID = @ModuleID
	ORDER BY CreatedDate DESC
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleMilestones]
GO

/* Procedure rb_GetSingleMilestones*/
CREATE PROCEDURE rb_GetSingleMilestones
(
    @ItemID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
SELECT
	ItemID,
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	EstCompleteDate,
	Status
FROM
	rb_Milestones
	WHERE
	    ItemID = @ItemID
ELSE
SELECT
	ItemID,
	ModuleID,
	CreatedByUser,
	CreatedDate,
	Title,
	EstCompleteDate,
	Status
FROM
	rb_Milestones_st
	WHERE
	    ItemID = @ItemID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateMilestones]
GO

/* Procedure rb_UpdateMilestones*/
CREATE PROCEDURE rb_UpdateMilestones
	@ItemID int,
	@ModuleID int,
	@CreatedByUser nvarchar(100),
	@CreatedDate datetime,
	@Title nvarchar(100),
	@EstCompleteDate datetime,
	@Status nvarchar(100)
AS
UPDATE rb_Milestones_st
SET
	ModuleID = @ModuleID,
	CreatedByUser = @CreatedByUser,
	CreatedDate = @CreatedDate,
	Title = @Title,
	EstCompleteDate = @EstCompleteDate,
	Status = @Status
WHERE
	ItemID = @ItemID
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[FK_rb_Milestones_rb_Modules]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE rb_Milestones
	DROP CONSTRAINT [FK_rb_Milestones_rb_Modules]
GO

ALTER TABLE rb_Milestones WITH NOCHECK ADD CONSTRAINT
	FK_rb_Milestones_rb_Modules FOREIGN KEY
	(
	ModuleID
	) REFERENCES rb_Modules
	(
	ModuleID
	) ON DELETE CASCADE
	
GO

