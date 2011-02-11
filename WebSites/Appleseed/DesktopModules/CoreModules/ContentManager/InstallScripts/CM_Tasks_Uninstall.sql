if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_GetSummary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_GetSummary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_CopyItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_MoveItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Tasks_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Tasks_CopyAll]
GO

/*
THIS STEP REQUIRED OR CONTENT MANAGER UI WILL SHOW THE MODULE EVEN THOUGH IT HAS BEEN
UNINSTALLED
*/
DELETE FROM rb_ContentManager WHERE FriendlyName = 'Tasks'
GO