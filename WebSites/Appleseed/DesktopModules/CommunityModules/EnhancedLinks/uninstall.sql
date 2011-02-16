/* Uninstall script */

--if exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
--DROP TABLE [rb_EnhancedLinks]
--GO

--if exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
---DROP TABLE [rb_EnhancedLinks_st]
--GO

if exists (select * from sysobjects where id = object_id(N'[rb_EnhancedLinks_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_EnhancedLinks_stModified]
GO

if exists (select * from sysobjects where id = object_id(N'[rb_AddEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_AddEnhancedLink]
GO

if exists (select * from sysobjects where id = object_id(N'[rb_DeleteEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_DeleteEnhancedLink]
GO

if exists (select * from sysobjects where id = object_id(N'[rb_GetEnhancedLinks]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetEnhancedLinks]
GO

if exists (select * from sysobjects where id = object_id(N'[rb_GetSingleEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_GetSingleEnhancedLink]
GO

if exists (select * from sysobjects where id = object_id(N'[rb_UpdateEnhancedLink]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [rb_UpdateEnhancedLink]
GO
