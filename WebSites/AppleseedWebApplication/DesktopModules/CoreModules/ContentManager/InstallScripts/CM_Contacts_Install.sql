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

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetContacts_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetContacts_Summary]
GO

CREATE PROCEDURE rb_GetContacts_Summary
(
    @ModuleID   int
)
AS
SELECT ItemID, Name + '::    ' + Role + '::    ' + Email As ItemDesc FROM rb_Contacts_st WHERE ModuleID = @ModuleID

GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_CopyItem]
GO

CREATE PROCEDURE rb_Contacts_CopyItem
(
    @ItemID          int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Contacts_st
(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Name,
    Role,
    Email,
    Contact1,
    Contact2
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Name,
    Role,
    Email,
    Contact1,
    Contact2
FROM
    rb_Contacts_st WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_MoveItem]
GO

CREATE PROCEDURE rb_Contacts_MoveItem
(
    @ItemID         int,
    @TargetModuleID int
)
AS
UPDATE rb_Contacts_st
    SET ModuleID = @TargetModuleID
        WHERE ItemID = @ItemID
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_CopyAll]
GO

CREATE PROCEDURE rb_Contacts_CopyAll
(
    @SourceModuleID  int,        /*Item that will be copied*/
    @TargetModuleID  int         /*Where to copy it to*/
)
AS
INSERT INTO rb_Contacts_st
(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    Name,
    Role,
    Email,
    Contact1,
    Contact2
)
SELECT
    @TargetModuleID,
    CreatedByUser,
    CreatedDate,
    Name,
    Role,
    Email,
    Contact1,
    Contact2
FROM
    rb_Contacts_st WHERE ModuleID = @SourceModuleID
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
VALUES('2502db18-b580-4f90-8cb4-c15e6e5339ef',
	'2502db18-b580-4f90-8cb4-c15e6e5339ef',
    'Contacts',
    'rb_GetContacts_Summary',
    'rb_GetContacts_Summary',
    'rb_Contacts_CopyItem',
    'rb_Contacts_MoveItem',
    'rb_Contacts_MoveItem',
    'rb_Contacts_CopyAll',
    'rb_DeleteContact',
    'rb_DeleteContact')
GO
