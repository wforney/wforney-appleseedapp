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
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_GetSummary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_GetSummary]
GO

CREATE PROCEDURE rb_Links_GetSummary
(
    @ModuleID           int
)
AS
    SELECT ItemID, Title + '::    ' + Url  As ItemDesc  FROM rb_Links_st WHERE ModuleID = @ModuleID
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_CopyItem]
GO

CREATE PROCEDURE rb_Links_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Links_st(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Url,
    MobileUrl,
    ViewOrder,
    Description,
    Target)
SELECT  @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Url,
    MobileUrl,
    ViewOrder,
    Description,
    Target
FROM rb_Links_st WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_MoveItem]
GO

CREATE PROCEDURE rb_Links_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Links_st
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_CopyAll]
GO

CREATE PROCEDURE rb_Links_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Links_st(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Url,
    MobileUrl,
    ViewOrder,
    Description,
    Target)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Url,
    MobileUrl,
    ViewOrder,
    Description,
    Target
FROM rb_Links_st WHERE ModuleID = @SourceModuleID

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
VALUES('476cf1cc-8364-479d-9764-4b3abd7ffabd',
	'476cf1cc-8364-479d-9764-4b3abd7ffabd',
    'Links',
    'rb_Links_GetSummary',
    'rb_Links_GetSummary',
    'rb_Links_CopyItem',
    'rb_Links_MoveItem',
    'rb_Links_MoveItem',
    'rb_Links_CopyAll',
    'rb_DeleteLink',
    'rb_DeleteLink')
GO
