/* Uninstall script, Documents module, manudea 27/10/2003  */
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_AddSecureDocumentAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_AddSecureDocumentAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocumentAccess]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocumentAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_DeleteSecureDocumentFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_DeleteSecureDocumentFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocumentContent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocumentContent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocumentFiles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocumentFiles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSecureDocuments]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSecureDocuments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSingleSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSingleSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_GetSingleSecureDocumentRoles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_GetSingleSecureDocumentRoles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_UpdateSecureDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_UpdateSecureDocument]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_UpdateSecureDocumentFile]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rb_UpdateSecureDocumentFile]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[rb_SecureDocumentFiles_stModified]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments_stModified]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[rb_SecureDocuments_stModified]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentFiles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentFiles_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentFiles_st]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocuments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocumentsAccess]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocumentsAccess]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rb_SecureDocuments_st]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[rb_SecureDocuments_st]
GO
