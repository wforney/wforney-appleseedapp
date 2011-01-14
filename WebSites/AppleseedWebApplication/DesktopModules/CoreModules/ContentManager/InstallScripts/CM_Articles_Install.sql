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

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetArticles_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetArticles_Summary]
GO

CREATE PROCEDURE rb_GetArticles_Summary
(
    @ModuleID   int
)
AS
SELECT ItemID, Title + '::    ' + CAST(Abstract As NVARCHAR(200)) As ItemDesc FROM rb_Articles WHERE ModuleID = @ModuleID
GO


/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Articles_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Articles_CopyItem]
GO

CREATE PROCEDURE rb_Articles_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Articles(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
	Subtitle,
    Abstract,
	Description,
	StartDate,
	ExpireDate,
	IsInNewsletter,
	MoreLink
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
	Subtitle,
    Abstract,
	Description,
	StartDate,
	ExpireDate,
	IsInNewsletter,
	MoreLink
FROM
    rb_Articles WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Articles_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Articles_MoveItem]
GO

CREATE PROCEDURE rb_Articles_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Articles
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Articles_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Articles_CopyAll]
GO

CREATE PROCEDURE rb_Articles_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Articles(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
	Subtitle,
    Abstract,
	Description,
	StartDate,
	ExpireDate,
	IsInNewsletter,
	MoreLink
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
	Subtitle,
    Abstract,
	Description,
	StartDate,
	ExpireDate,
	IsInNewsletter,
	MoreLink
FROM
    rb_Articles WHERE ModuleID = @SourceModuleID
GO
/***************************************************************************************/
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
VALUES('87303cf7-76d0-49b1-a7e7-a5c8e26415ba',	
	'87303cf7-76d0-49b1-a7e7-a5c8e26415ba',
    'Articles',
    'rb_GetArticles_Summary',
    'rb_GetArticles_Summary',
    'rb_Articles_CopyItem',
    'rb_Articles_MoveItem',
    'rb_Articles_MoveItem',
    'rb_Articles_CopyAll',
    'rb_DeleteArticle',
    'rb_DeleteArticle')
GO
