/*
SUGGESTIONS:
    -if the module has workflow, use the workflow table!!!!
    -you can copy the list of fields for the _CopyItem sproc from
        the Add method of that module.  Ex get the fields for
        rb_Blogs_CopyItem from rb_AddAnnouncement.  Just
        copy+paste the list twice, add the rest and use the
        @ModuleID field.
    -the _Summary function MUST, MUST,MUST return both the
        ItemID field(named ItemID, you can alias it for others)
        ItemDesc(alias + concatenate the fields you want into this one)

    -Add a record into the rb_ContentManager for the sprocs you made.
*/
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetBlogs_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetBlogs_Summary]
GO

CREATE PROCEDURE rb_GetBlogs_Summary
(
    @ModuleID           int
)
AS
    SELECT ItemID, Title As ItemDesc  FROM rb_Blogs WHERE ModuleID = @ModuleID
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Blogs_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Blogs_CopyItem]
GO

CREATE PROCEDURE rb_Blogs_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Blogs(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Excerpt,
	[Description],
	StartDate,
	IsInNewsletter,
    CommentCount,
    TrackBackCount
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Excerpt,
	[Description],
	StartDate,
	IsInNewsletter,
    CommentCount,
    TrackBackCount
FROM rb_Blogs WHERE ItemID = @ItemID

DECLARE @DestinationItemID int

SELECT @DestinationItemID = @@IDENTITY


INSERT INTO rb_BlogComments(
    ModuleID,
    ItemID,
    Comment,
    Title,
    [Name],
    URL,
    DateCreated)
SELECT
    @TargetModuleID,
    @DestinationItemID,
    Comment,
    Title,
    [Name],
    URL,
    DateCreated
FROM rb_BlogComments WHERE ItemID = @ItemID

DECLARE @EntryCount int
DECLARE @CommentCount int
DECLARE @TrackBackCount int
DECLARE @ModuleID int

SET @ModuleID = (SELECT ModuleID FROM rb_Blogs WHERE ItemID = @ItemID)

SELECT
    @EntryCount = EntryCount,
    @CommentCount = CommentCount,
    @TrackBackCount = TrackBackCount
FROM rb_BlogStats
WHERE ModuleID = @ModuleID

UPDATE rb_BlogStats
    SET
      EntryCount = EntryCount + @EntryCount,
      CommentCount = CommentCount + @CommentCount,
      TrackBackCount = TrackBackCount + @TrackBackCount
    WHERE ModuleID = @TargetModuleID

UPDATE rb_BlogStats
    SET
      EntryCount = EntryCount - @EntryCount,
      CommentCount = CommentCount - @CommentCount,
      TrackBackCount = TrackBackCount - @TrackBackCount
    WHERE ModuleID = @ModuleID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Blogs_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Blogs_MoveItem]
GO

CREATE PROCEDURE rb_Blogs_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Blogs
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID

DECLARE @EntryCount int
DECLARE @CommentCount int
DECLARE @TrackBackCount int
DECLARE @ModuleID int
SET @ModuleID = (SELECT ModuleID FROM rb_Blogs WHERE ItemID = @ItemID)

UPDATE rb_BlogStats
    SET
      EntryCount = EntryCount + @EntryCount,
      CommentCount = CommentCount + @CommentCount,
      TrackBackCount = TrackBackCount + @TrackBackCount
    WHERE ModuleID = @TargetModuleID

UPDATE rb_BlogStats
    SET
      EntryCount = EntryCount - @EntryCount,
      CommentCount = CommentCount - @CommentCount,
      TrackBackCount = TrackBackCount - @TrackBackCount
    WHERE ModuleID = @ModuleID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Blogs_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Blogs_CopyAll]
GO

CREATE PROCEDURE rb_Blogs_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS

INSERT INTO rb_Blogs(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Excerpt,
	[Description],
	StartDate,
	IsInNewsletter,
    CommentCount,
    TrackBackCount
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Title,
    Excerpt,
	[Description],
	StartDate,
	IsInNewsletter,
    CommentCount,
    TrackBackCount
FROM rb_Blogs WHERE ModuleID = @SourceModuleID

INSERT INTO rb_BlogComments(
    ModuleID,
    ItemID,
    Comment,
    Title,
    [Name],
    URL,
    DateCreated)
SELECT
    @TargetModuleID,
    ItemID,
    Comment,
    Title,
    [Name],
    URL,
    DateCreated
FROM rb_BlogComments WHERE ModuleID = @SourceModuleID

DECLARE @EntryCount int
DECLARE @CommentCount int
DECLARE @TrackBackCount int

SELECT
    @EntryCount = EntryCount,
    @CommentCount = CommentCount,
    @TrackBackCount = TrackBackCount
FROM rb_BlogStats
WHERE ModuleID = @SourceModuleID

UPDATE rb_BlogStats
    SET
    EntryCount = EntryCount + @EntryCount,
    CommentCount = CommentCount + @CommentCount,
    TrackBackCount = TrackBackCount + @TrackBackCount
    WHERE ModuleID = @TargetModuleID
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
VALUES('55EF407B-C9D6-47e3-B627-EFA6A5EEF4B2',
	'55EF407B-C9D6-47e3-B627-EFA6A5EEF4B2',
    'Blogs',
    'rb_GetBlogs_Summary',
    'rb_GetBlogs_Summary',
    'rb_Blogs_CopyItem',
    'rb_Blogs_MoveItem',
    'rb_Blogs_MoveItem',
    'rb_Blogs_CopyAll',
    'rb_DeleteBlog',
    'rb_DeleteBlog')
GO
