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
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_EnhancedLinks_GetSummary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_EnhancedLinks_GetSummary]
GO

CREATE PROCEDURE rb_EnhancedLinks_GetSummary
(
    @ModuleID           int
)
AS
    SELECT ItemID, Title + '::    ' + Url  As ItemDesc  FROM rb_EnhancedLinks_st WHERE ModuleID = @ModuleID
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_EnhancedLinks_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_EnhancedLinks_CopyItem]
GO

CREATE PROCEDURE rb_EnhancedLinks_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_EnhancedLinks_st(
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
    Target)
SELECT  @TargetModuleID,
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
FROM rb_EnhancedLinks_st WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_EnhancedLinks_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_EnhancedLinks_MoveItem]
GO

CREATE PROCEDURE rb_EnhancedLinks_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_EnhancedLinks_st
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_EnhancedLinks_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_EnhancedLinks_CopyAll]
GO

CREATE PROCEDURE rb_EnhancedLinks_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_EnhancedLinks_st(
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
    ImageUrl,
    Clicks,
    Target
FROM rb_EnhancedLinks_st WHERE ModuleID = @SourceModuleID
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

VALUES('96BC3CE0-0409-4AB1-A0C2-67D6C4D68193',
      '96BC3CE0-0409-4AB1-A0C2-67D6C4D68193',
    'EnhancedLinks',
    'rb_EnhancedLinks_GetSummary',
    'rb_EnhancedLinks_GetSummary',
    'rb_EnhancedLinks_CopyItem',
    'rb_EnhancedLinks_MoveItem',
    'rb_EnhancedLinks_MoveItem',
    'rb_EnhancedLinks_CopyAll',
    'rb_DeleteEnhancedLink',
    'rb_DeleteEnhancedLink')

GO


--enhanced links fix
UPDATE rb_ContentManager 
SET MoveItemLeftSproc = 'rb_EnhancedLinks_MoveItem', MoveItemRightSproc = 'rb_EnhancedLinks_MoveItem' 
WHERE SourceGeneralModDefID = '{96BC3CE0-0409-4AB1-A0C2-67D6C4D68193}'
AND DestGeneralModDefID = '{96BC3CE0-0409-4AB1-A0C2-67D6C4D68193}'
GO


