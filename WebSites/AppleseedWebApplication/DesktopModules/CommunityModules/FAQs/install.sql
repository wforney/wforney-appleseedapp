/* Install script, FAQs module, Jakob Hansen, 25 april 2003 */

IF NOT EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_FAQs]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [rb_FAQs] (
	[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
	[ModuleID] [int] NOT NULL ,
	[CreatedByUser] [nvarchar] (100) NULL ,
	[CreatedDate] [datetime] NULL ,
	[Question] [nvarchar] (500) NULL ,
	[Answer] [ntext] NULL 
    ) ON [PRIMARY] 
END
/* Alter Table must be used when Table exists. */
ELSE
BEGIN
  IF NOT EXISTS (select b.length From sysobjects a inner join  syscolumns b ON a.id = b.id where a.name = 'rb_FAQs' and b.name = 'Answer' AND b.length = 16) 
    BEGIN 
	   ALTER TABLE [rb_FAQs] ALTER COLUMN [Answer][ntext] NULL
	END 
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddFAQ]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddFAQ]
GO
CREATE PROCEDURE [rb_AddFAQ]
	(@ItemID 	[int] OUTPUT,
	 @ModuleID 	[int],
	 @UserName	[nvarchar] (100),
	 @Question 	[nvarchar] (500),
	 @Answer 	[ntext])

AS INSERT INTO [rb_FAQs]
	([ModuleID],
	 [CreatedByUser],
	 [CreatedDate],
	 [Question],
	 [Answer]) 
 
VALUES 
	 (@ModuleID,
	  @UserName,
	  GETDATE(),
	  @Question,
	  @Answer)

SELECT 
	@ItemID = @@IDENTITY

GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteFAQ]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteFAQ]
GO

CREATE PROCEDURE [rb_DeleteFAQ]
	(@ItemID 	[int])

AS DELETE FROM [rb_FAQs]

WHERE 
	( [ItemID] = @ItemID)

GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetFAQ]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetFAQ]
GO

CREATE PROCEDURE rb_GetFAQ

(@ModuleID int)

AS

SELECT ItemID, CreatedByUser, CreatedDate, Question, Answer
FROM rb_FAQs 
WHERE ModuleID = @ModuleID
ORDER BY Question
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleFAQ]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleFAQ]
GO

CREATE PROCEDURE rb_GetSingleFAQ 
(@ItemID int)

AS

SELECT CreatedByUser, CreatedDate, Question, Answer
FROM rb_FAQs
WHERE ItemID = @ItemID

GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateFAQ]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateFAQ]
GO

CREATE PROCEDURE [rb_UpdateFAQ]
	(@ItemID 	[int],
	 @UserName	[nvarchar] (100),
	 @Question 	[nvarchar] (500),
	 @Answer 	[ntext] )

AS UPDATE [rb_FAQs]

SET  
	 [CreatedByUser] = @UserName,
	 [Question]	 = @Question,
	 [Answer]	 = @Answer 

WHERE 
	( [ItemID]	 = @ItemID)

GO
