

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


