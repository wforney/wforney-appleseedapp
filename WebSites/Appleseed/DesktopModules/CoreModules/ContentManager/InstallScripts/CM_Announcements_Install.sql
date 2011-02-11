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
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetAnnoucements_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetAnnoucements_Summary]
GO

CREATE PROCEDURE rb_GetAnnoucements_Summary
(
    @ModuleID           int
)
AS
    SELECT ItemID, Title + '::    ' + LEFT(Description,200)  As ItemDesc  FROM rb_Announcements_st WHERE ModuleID = @ModuleID
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Announcements_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Announcements_CopyItem]
GO

CREATE PROCEDURE rb_Announcements_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Announcements_st(
        ModuleID,
        CreatedByUser,
        CreatedDate,
        Title,
        MoreLink,
        MobileMoreLink,
        ExpireDate,
        Description)
SELECT  @TargetModuleID,
        CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
FROM rb_Announcements_st WHERE ItemID = @ItemID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Announcements_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Announcements_MoveItem]
GO

CREATE PROCEDURE rb_Announcements_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Announcements_st
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Announcements_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Announcements_CopyAll]
GO

CREATE PROCEDURE rb_Announcements_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Announcements_st(
        ModuleID,
        CreatedByUser,
        CreatedDate,
        Title,
        MoreLink,
        MobileMoreLink,
        ExpireDate,
        Description)
SELECT  @TargetModuleID,
        CreatedByUser,
	    CreatedDate,
	    Title,
	    MoreLink,
	    MobileMoreLink,
	    ExpireDate,
	    Description
FROM rb_Announcements_st WHERE ModuleID = @SourceModuleID

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
VALUES('ce55a821-2449-4903-ba1a-ec16db93f8db',
	'ce55a821-2449-4903-ba1a-ec16db93f8db',
    'Announcements',
    'rb_GetAnnoucements_Summary',
    'rb_GetAnnoucements_Summary',
    'rb_Announcements_CopyItem',
    'rb_Announcements_MoveItem',
    'rb_Announcements_MoveItem',
    'rb_Announcements_CopyAll',
    'rb_DeleteAnnouncement',
    'rb_DeleteAnnouncement')
GO
