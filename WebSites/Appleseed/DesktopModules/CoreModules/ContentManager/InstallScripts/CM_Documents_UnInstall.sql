if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetDocuments_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetDocuments_Summary]
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_CopyItem]
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_MoveItem]
GO

/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Documents_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Documents_CopyAll]
GO

/*
THIS STEP REQUIRED OR CONTENT MANAGER UI WILL SHOW THE MODULE EVEN THOUGH IT HAS BEEN
UNINSTALLED
*/
DELETE FROM rb_ContentManager WHERE FriendlyName = 'Documents'
GO