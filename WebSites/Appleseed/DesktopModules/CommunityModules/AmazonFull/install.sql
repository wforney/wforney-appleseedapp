if NOT exists (select * from dbo.sysobjects where id = object_id(N'[rb_BookList]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [rb_BookList] (
		[ItemID] [int] IDENTITY (1, 1) NOT NULL ,
		[ModuleID] [int] NOT NULL ,
		[CreatedByUser] [nvarchar] (100) NULL ,
		[CreatedDate] [datetime] NULL ,
		[ISBN] [nvarchar] (10) NULL ,
		[Caption] [ntext] NOT NULL,
		CONSTRAINT [PK_rbBookList] PRIMARY KEY  NONCLUSTERED 
		(
			[ItemID]
		)  ON [PRIMARY] ,
		CONSTRAINT [FK_rbBookListModules] FOREIGN KEY 
		(
			[ModuleID]
		) REFERENCES [rb_Modules] (
			[ModuleID]
		) ON DELETE CASCADE  NOT FOR REPLICATION 
	) ON [PRIMARY]
END
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_AddBook]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_AddBook]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_DeleteBook]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_DeleteBook]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetBooks]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetBooks]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetSingleBook]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetSingleBook]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_UpdateBook]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_UpdateBook]
GO

CREATE PROCEDURE rb_AddBook
(
    @ModuleID       int,
    @CreatedByUser  nvarchar(100),
    @ISBN           nvarchar(10),
    @ItemID         int OUTPUT,
    @Caption		ntext
)
AS

INSERT INTO rb_BookList
(
    ModuleID,
    CreatedByUser,
    CreatedDate,
    ISBN,
    Caption
)

VALUES
(
    @ModuleID,
    @CreatedByUser,
    GetDate(),
    @ISBN,
    @Caption
)

SELECT
    @ItemID = @@Identity
GO

CREATE PROCEDURE rb_DeleteBook
(
    @ItemID int
)
AS

DELETE FROM
    rb_BookList

WHERE
    ItemID = @ItemID
GO


CREATE PROCEDURE rb_GetBooks
(
    @ModuleID int
)
AS

SELECT
    ItemID,
    CreatedByUser,
    CreatedDate,
    ISBN,
    Caption
FROM 
    rb_BookList

WHERE
    ModuleID = @ModuleID
GO

CREATE PROCEDURE rb_GetSingleBook
(
    @ItemID int
)
AS

SELECT
    CreatedByUser,
    CreatedDate,
    ISBN,
    Caption
FROM 
    rb_BookList

WHERE
    ItemID = @ItemID
GO

CREATE PROCEDURE rb_UpdateBook
(
    @ItemID         int,
    @CreatedByUser  nvarchar(100),
    @ISBN           nvarchar(10),
    @Caption        ntext
)
AS

UPDATE
    rb_BookList

SET
    CreatedByUser   = @CreatedByUser,
    ISBN            = @ISBN,
    Caption         = @Caption

WHERE
    ItemID = @ItemID
GO
