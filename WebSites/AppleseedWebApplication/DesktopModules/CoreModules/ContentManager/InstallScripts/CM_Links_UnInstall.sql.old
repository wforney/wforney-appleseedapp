if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_GetSummary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_GetSummary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_CopyItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_MoveItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Links_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Links_CopyAll]
GO
/*
THIS STEP REQUIRED OR CONTENT MANAGER UI WILL SHOW THE MODULE EVEN THOUGH IT HAS BEEN
UNINSTALLED
*/
DELETE FROM rb_ContentManager WHERE FriendlyName = 'Links'
GO