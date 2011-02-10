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
/*WE DO NOT NEED SUMMARY SPROCS FOR CROSS MODULE TRANSFERS.  THE SPROC FROM THE SINGLE
ITEM IS RE-USED.

EX.  Links -> Enhanced Links re-uses the GetSummaryData sprocs from the
CM_Links_Install.sql file
AND
CM_EnhancedLinks_Install.sql

This does introduce some file dependancy issues!!!*/

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_EnhancedLinks_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_EnhancedLinks_CopyItem]
GO

CREATE PROCEDURE rb_Links_EnhancedLinks_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
DECLARE @BLANK nvarchar(1)
DECLARE @ZERO int

SET @BLANK = ''
SET @ZERO = 0

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
    @BLANK,
    @ZERO,
    Target
    
FROM rb_Links_st WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_EnhancedLinks_MoveItemLeft]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_EnhancedLinks_MoveItemLeft]
GO

CREATE PROCEDURE rb_Links_EnhancedLinks_MoveItemLeft
(
    @ItemID         int,
    @TargetModuleID int
)
AS

/*this is actually a duplicate sproc, moving item from EnhancedLinks to Links.  Same sproc found
in the CM_EnhancedLinks_Links.sql file.  Duplicated with different name so as to not break the CM
when one is uninstalled and they try to use the other */
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
    
FROM rb_EnhancedLinks_st WHERE ItemID = @ItemID

DELETE FROM rb_EnhancedLinks_st WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_EnhancedLinks_MoveItemRight]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_EnhancedLinks_MoveItemRight]
GO
CREATE PROCEDURE rb_Links_EnhancedLinks_MoveItemRight
(
    @ItemID         int,
    @TargetModuleID int
)
AS
/*this will need to be an insert instead of a update statment.*/
DECLARE @BLANK nvarchar(1)
DECLARE @ZERO int

SET @BLANK = ''
SET @ZERO = 0

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
    @BLANK,
    @ZERO,
    Target
  
FROM rb_Links_st WHERE ItemID = @ItemID

DELETE FROM rb_Links_st WHERE ItemID = @ItemID


GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_EnhancedLinks_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_EnhancedLinks_CopyAll]
GO

CREATE PROCEDURE rb_Links_EnhancedLinks_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

DECLARE @BLANK nvarchar(1)
DECLARE @ZERO int

SET @BLANK = ''
SET @ZERO = 0

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
    @BLANK,
    @ZERO,
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
	'96BC3CE0-0409-4AB1-A0C2-67D6C4D68193',
    'Links -> EnhancedLinks',
    'rb_Links_GetSummary',
    'rb_EnhancedLinks_GetSummary',
    'rb_Links_EnhancedLinks_CopyItem',
    'rb_Links_EnhancedLinks_MoveItemLeft',
    'rb_Links_EnhancedLinks_MoveItemRight',
    'rb_Links_EnhancedLinks_CopyAll',
    'rb_DeleteLink',
    'rb_DeleteEnhancedLink')
GO
