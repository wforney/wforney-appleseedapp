/* Uninstall script, Contacts module, manudea 27/10/2003  */

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Contacts_stModified]') AND OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [rb_Contacts_stModified]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetContacts]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetContacts]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleContact]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateContact]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddContact]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteContact]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteContact]
GO
