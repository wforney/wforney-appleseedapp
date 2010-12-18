/*
SUGGESTIONS:
    -if the module has workflow, use the workflow table!!!!
    -you can copy the list of fields for the _CopyItem sproc from
        the Add method of that module.  Ex get the fields for
        rb_Announcements_CopyItem from rb_AddAnnouncement.  Just
        copy+paste the list twice, add the rest and use the
        @ModuleID field.
    -the _Summary function MUST, MUST,MUST return both the
        ItemID field(named ItemID, you can alias it for others)
        ItemDesc(alias + concatenate the fields you want into this one)

    -Add a record into the rb_ContentManager for the sprocs you made.
*/
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_GetSummary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_GetSummary]
GO

CREATE PROCEDURE rb_Tasks_GetSummary
(
    @ModuleID           int
)
AS
    SELECT ItemID, Title As ItemDesc  FROM rb_Tasks WHERE ModuleID = @ModuleID
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_CopyItem]
GO

CREATE PROCEDURE [rb_Tasks_CopyItem]
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Tasks(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    ModifiedByUser,
    ModifiedDate,
    AssignedTo,
    Title,
    [Description],
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate)
SELECT  
	@TargetModuleID,
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
FROM rb_Tasks WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_MoveItem]
GO

CREATE PROCEDURE [rb_Tasks_MoveItem]
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Tasks
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_CopyAll]
GO

CREATE PROCEDURE [rb_Tasks_CopyAll]
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Tasks(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    ModifiedByUser,
    ModifiedDate,
    AssignedTo,
    Title,
    [Description],
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate)
SELECT  
	@TargetModuleID,
    CreatedByUser,
    CreatedDate,
    ModifiedByUser,
    ModifiedDate,
    AssignedTo,
    Title,
    [Description],
    Status,
    Priority,
    PercentComplete,
    StartDate,
    DueDate
FROM rb_Tasks WHERE ModuleID = @SourceModuleID

GO

INSERT INTO rb_ContentManager(
    SourceGeneralModDefID,
    DestGeneralModDefID,
    FriendlyName,
    SourceSummarySproc,
    DestSummarySproc,
    CopyItemSproc,
    MoveItemLeftSproc,
    MoveItemRightSproc,
    CopyAllSproc,
    DeleteItemLeftSproc,
    DeleteItemRightSproc
)
VALUES('2502DB18-B580-4F90-8CB4-C15E6E531012',
	'2502DB18-B580-4F90-8CB4-C15E6E531012',
    'Tasks',
    'rb_Tasks_GetSummary',
    'rb_Tasks_GetSummary',
    'rb_Tasks_CopyItem',
    'rb_Tasks_MoveItem',
    'rb_Tasks_MoveItem',
    'rb_Tasks_CopyAll',
    'rb_DeleteTask',
    'rb_DeleteTask')
GO

/*delete sprocs with old naming convention*/
if exists (select * from dbo.sysobjects where id = object_id(N'[CM_GetTasks_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [CM_GetTasks_Summary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[CM_Tasks_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [CM_Tasks_CopyItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[CM_Tasks_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [CM_Tasks_MoveItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[CM_Tasks_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [CM_Tasks_CopyAll]
GO
