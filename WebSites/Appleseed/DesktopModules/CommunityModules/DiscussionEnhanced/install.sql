/* Install script, Discussion */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Discussion]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
 BEGIN
CREATE TABLE [rb_Discussion] (
	[ItemID] [int] IDENTITY (1,1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[Title] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Body] [nvarchar] (3000) NULL ,
	[DisplayOrder] [nvarchar] (750) NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[ViewCount] [int] NULL ,
	CONSTRAINT [PK_rb_Discussion] PRIMARY KEY  NONCLUSTERED 
	(
		[ItemID]
	),
	CONSTRAINT [FK_rb_Discussion_rb_Modules] FOREIGN KEY 
	(
		[ModuleID]
	) REFERENCES [rb_Modules] (
		[ModuleID]
	) ON DELETE CASCADE  NOT FOR REPLICATION 
)
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionAddMessage]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionAddMessage]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionDeleteChildren]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionDeleteChildren]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionDeleteMessage]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionDeleteMessage]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionGetMessage]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionGetMessage]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionGetNextMessageID]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionGetNextMessageID]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionGetPrevMessageID]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionGetPrevMessageID]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionGetThreadMessages]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionGetThreadMessages]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionGetTopLevelMessages]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionGetTopLevelMessages]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DiscussionIncrementViewCount]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DiscussionIncrementViewCount]
GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionAddMessage
(
    @Mode      text,
    @ItemID    int OUTPUT,
    @Title     nvarchar(100),
    @Body      nvarchar(3000),
    @ParentID  int,
    @UserName  nvarchar(100),
    @ModuleID  int
)   

AS 

IF (@Mode LIKE 'EDIT')
BEGIN
UPDATE 
    rb_Discussion
SET
    Title = @Title,
    Body  = @Body
    /* CreatedByUser = @UserName   SHOULD WE LET USER CHANGE ? */
WHERE
    ItemID = @ParentID

SET @ItemID = @ParentID
END

ELSE  /* we are adding a new topic */

BEGIN
/* Find DisplayOrder of parent item */
DECLARE @ParentDisplayOrder AS nvarchar(750)
SET @ParentDisplayOrder = ''

SELECT 
    @ParentDisplayOrder = DisplayOrder
FROM 
    rb_Discussion 
WHERE 
    ItemID = @ParentID

INSERT INTO rb_Discussion
(
    Title,
    Body,
    DisplayOrder,
    CreatedDate, 
    CreatedByUser,
    ModuleID
)

VALUES
(
    @Title,
    @Body,
    @ParentDisplayOrder + CONVERT( nvarchar(24), GETDATE(), 21 ),
    GETDATE(),
    @UserName,
    @ModuleID
)
SELECT 
@ItemID = @@IDENTITY
END


/* update the top most parent's created date with the current date 
This is old logic WHERE I changed the CreatedDate of the parent to get TopLevelMessage
sorts always showing the recent activity at top.  Now TopLevelMessages auto
sorts by threads with recent activity

DECLARE @TopParent AS nvarchar(24)
SET @TopParent = string.Empty 
SET @TopParent = LEFT(@ParentDisplayOrder, 23)

make sure we are not at a top level parent already !
IF @ParentID != 0 
BEGIN
UPDATE Discussion
SET CreatedDate = GETDATE()
WHERE DisplayOrder = @TopParent
END
*/
GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionDeleteChildren
(
   @ItemID int,  /* DELETE this item AND all its children */
   @NumDeletedMessages int OUTPUT
)
AS

DECLARE @CurrentDisplayOrder AS nvarchar(750)
DECLARE @CurrentModule AS int

/* Find DisplayOrder of item to DELETE */
SELECT
    @CurrentDisplayOrder = DisplayOrder,
    @CurrentModule = ModuleID
FROM
    rb_Discussion
WHERE
    ItemID = @ItemID


/* DELETE this item AND all of its children */
DELETE
FROM
    rb_Discussion
WHERE
    LEFT(DisplayOrder,LEN(RTRIM(@CurrentDisplayOrder))) = @CurrentDisplayOrder
    /* DisplayOrder LIKE @CurrentDisplayOrder + '%' */
    AND
    ModuleID = @CurrentModule

SET @NumDeletedMessages = @@ROWCOUNT

GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionDeleteMessage
(
    @ItemID int
)
AS

DELETE FROM
    rb_Discussion
WHERE
    ItemID = @ItemID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


/***********************************************************/
CREATE PROCEDURE rb_DiscussionGetMessage
(
    @ItemID int
)
AS

/*
DECLARE @nextMessageID int
EXECUTE DiscussionGetNextMessageID @ItemID, @nextMessageID OUTPUT
DECLARE @prevMessageID int
EXECUTE DiscussionGetPrevMessageID @ItemID, @prevMessageID OUTPUT
*/

SELECT
    ItemID,
    Title,
    CreatedByUser,
    CreatedDate,
    Body,
    DisplayOrder
/*
    ,
    NextMessageID = @nextMessageID,
    PrevMessageID = @prevMessageID
*/

FROM
    rb_Discussion

WHERE
    ItemID = @ItemID

GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionGetNextMessageID
(
    @ItemID int,
    @NextID int OUTPUT
)
AS

DECLARE @CurrentDisplayOrder AS nvarchar(750)
DECLARE @CurrentModule AS int

/* Find DisplayOrder of current item */
SELECT
    @CurrentDisplayOrder = DisplayOrder,
    @CurrentModule = ModuleID
FROM
    rb_Discussion
WHERE
    ItemID = @ItemID

/* Get the next message in the same module */
SELECT Top 1
    @NextID = ItemID

FROM
    rb_Discussion

WHERE
    DisplayOrder > @CurrentDisplayOrder
    AND
    ModuleID = @CurrentModule

ORDER BY
    DisplayOrder ASC

/* END of this thread? */
IF @@ROWCOUNT < 1
    SET @NextID = null

GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionGetPrevMessageID
(
    @ItemID int,
    @PrevID int OUTPUT
)
AS

DECLARE @CurrentDisplayOrder AS nvarchar(750)
DECLARE @CurrentModule AS int

/* Find DisplayOrder of current item */
SELECT
    @CurrentDisplayOrder = DisplayOrder,
    @CurrentModule = ModuleID
FROM
    rb_Discussion
WHERE
    ItemID = @ItemID

/* Get the previous message in the same module */
SELECT Top 1
    @PrevID = ItemID

FROM
    rb_Discussion

WHERE
    DisplayOrder < @CurrentDisplayOrder
    AND
    ModuleID = @CurrentModule

ORDER BY
    DisplayOrder DESC

/* already at the beginning of this module? */
IF @@ROWCOUNT < 1
    SET @PrevID = null

GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionGetThreadMessages
(
    @ItemID		int,    
    @IncludeRoot char   /* IF = 'Y', then RETURN the root message AND all children */
						/* IF = 'N', only RETURN the children of the root */ 
)
AS

DECLARE @CurrentDisplayOrder AS nvarchar(750)
DECLARE @CurrentModule AS int

/* Find DisplayOrder of current item */
SELECT
    @CurrentDisplayOrder = DisplayOrder,
    @CurrentModule = ModuleID
FROM
    rb_Discussion
WHERE
    ItemID = @ItemID


IF (@IncludeRoot = 'N') /* retrieve all thread topics including original post */
BEGIN
SELECT
    ItemID,
    DisplayOrder,
    /* REPLICATE( '&nbsp;', ( ( LEN( DisplayOrder ) / 23 ) - 2 ) * 5 ) AS Indent, */
    REPLICATE( '<blockquote>', ( ( LEN( DisplayOrder ) / 23 ) -1 )) AS BlockQuoteStart,
    REPLICATE( '</blockquote>', ( ( LEN( DisplayOrder ) / 23 ) -1 )) AS BlockQuoteEnd,
    Title,  
    CreatedByUser,
    CreatedDate,
    Body

FROM 
    rb_Discussion

WHERE
    LEFT(DisplayOrder, 23) = @CurrentDisplayOrder
  AND
    (LEN( DisplayOrder ) / 23 ) > 1
 AND
    ModuleID = @CurrentModule
    
ORDER BY
    DisplayOrder
END
ELSE
BEGIN
SELECT
    ItemID,
    DisplayOrder,
    /* REPLICATE( '&nbsp;', ( ( LEN( DisplayOrder ) / 23 ) - 2 ) * 5 ) AS Indent, */
    REPLICATE( '<blockquote>', ( ( LEN( DisplayOrder ) / 23 ) )) AS BlockQuoteStart,
    REPLICATE( '</blockquote>', ( ( LEN( DisplayOrder ) / 23 ) )) AS BlockQuoteEnd,
    Title,  
    CreatedByUser,
    CreatedDate,
    Body

FROM 
    rb_Discussion

WHERE
    LEFT(DisplayOrder, 23) = @CurrentDisplayOrder
 AND
    ModuleID = @CurrentModule    
ORDER BY
    DisplayOrder
END

GO

/***********************************************************/
CREATE PROCEDURE rb_DiscussionGetTopLevelMessages
(
    @ModuleID int
)
AS

SELECT
    ItemID,
    DisplayOrder,
    LEFT(DisplayOrder, 23) AS Parent,    
    (SELECT COUNT(*) -1  FROM rb_Discussion Disc2 WHERE LEFT(Disc2.DisplayOrder,LEN(RTRIM(Disc.DisplayOrder))) = Disc.DisplayOrder) AS ChildCount,
    Title,  
    CreatedByUser,
    CreatedDate,
    Body,
    ViewCount,
    (SELECT MAX(CreatedDate) FROM rb_Discussion Disc3 WHERE LEFT(Disc3.DisplayOrder, 23) = Disc.DisplayOrder)
    AS DateofLastReply
FROM rb_Discussion Disc
WHERE ModuleID=@ModuleID
AND (LEN( DisplayOrder ) / 23 ) = 1

ORDER BY
    DateofLastReply DESC

GO

/**************************************************************/
CREATE PROCEDURE rb_DiscussionIncrementViewCount
(
    @ItemID  int
)
AS

/* handle case of old database WHERE ViewCOunt is NULL */
SELECT 
	ViewCount
FROM
	rb_Discussion
WHERE
	  ItemID = @ItemID
	
UPDATE
    rb_Discussion
SET
    ViewCount = ISNULL(ViewCount,0) + 1
WHERE
    ItemID = @ItemID
GO

