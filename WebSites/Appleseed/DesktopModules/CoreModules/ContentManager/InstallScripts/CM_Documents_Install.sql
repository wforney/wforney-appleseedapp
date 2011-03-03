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

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetDocuments_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetDocuments_Summary]
GO

CREATE PROCEDURE rb_GetDocuments_Summary
(
    @ModuleID   int
)
AS
SELECT ItemID,
    FileFriendlyName + '::    ' +
    CreatedByUser + '::    ' +
    Category As ItemDesc
FROM rb_Documents_st WHERE ModuleID = @ModuleID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_CopyItem]
GO

CREATE PROCEDURE rb_Documents_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Documents_st
(
    ModuleID,
    FileFriendlyName,
    FileNameUrl,
    CreatedByUser,
    CreatedDate,
    Category,
    Content,
    ContentType,
    ContentSize
)
SELECT
    @TargetModuleID,
    FileFriendlyName,
    FileNameUrl,
    CreatedByUser,
    CreatedDate,
    Category,
    Content,
    ContentType,
    ContentSize
FROM
    rb_Documents_st WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_MoveItem]
GO

CREATE PROCEDURE rb_Documents_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Documents_st
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_CopyAll]
GO

CREATE PROCEDURE rb_Documents_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Documents_st
(
    ModuleID,
    FileFriendlyName,
    FileNameUrl,
    CreatedByUser,
    CreatedDate,
    Category,
    Content,
    ContentType,
    ContentSize
)
SELECT
    @TargetModuleID,
    FileFriendlyName,
    FileNameUrl,
    CreatedByUser,
    CreatedDate,
    Category,
    Content,
    ContentType,
    ContentSize
FROM
    rb_Documents_st WHERE ModuleID = @SourceModuleID
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
VALUES('f9645b82-cb45-4c4c-bb2d-72fa42fe2b75',
	'f9645b82-cb45-4c4c-bb2d-72fa42fe2b75',
    'Documents',
    'rb_GetDocuments_Summary',
    'rb_GetDocuments_Summary',
    'rb_Documents_CopyItem',
    'rb_Documents_MoveItem',
    'rb_Documents_MoveItem',
    'rb_Documents_CopyAll',
    'rb_DeleteDocument',
    'rb_DeleteDocument')
GO
