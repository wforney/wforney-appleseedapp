if exists (select * from dbo.sysobjects where id = object_id(N'[rb_GetContacts_Summary]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetContacts_Summary]
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_CopyItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_CopyItem]
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_MoveItem]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_MoveItem]
GO
/***************************************************************************************/
if exists (select * from dbo.sysobjects where id = object_id(N'[rb_Contacts_CopyAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_Contacts_CopyAll]
GO

/*
THIS STEP REQUIRED OR CONTENT MANAGER UI WILL SHOW THE MODULE EVEN THOUGH IT HAS BEEN
UNINSTALLED
*/
DELETE FROM rb_ContentManager WHERE FriendlyName = 'Contacts'
GO
