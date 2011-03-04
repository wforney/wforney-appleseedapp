if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetFAQs_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetFAQs_Summary]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_FAQs_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_FAQs_CopyItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_FAQs_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_FAQs_MoveItem]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[rb_FAQs_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_FAQs_CopyAll]
GO

/*
THIS STEP REQUIRED OR CONTENT MANAGER UI WILL SHOW THE MODULE EVEN THOUGH IT HAS BEEN
UNINSTALLED
*/
DELETE FROM rb_ContentManager WHERE FriendlyName = 'FAQs'
GO