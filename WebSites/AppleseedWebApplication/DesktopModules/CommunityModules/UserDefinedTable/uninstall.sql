/* Uninstall script, UserDefinedTable module, Jakob Hansen, 1 may 2003 */


/* We do not DELETE the tables!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UserDefinedData]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_UserDefinedData] */
--GO

/* We do not DELETE the tables!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UserDefinedFields]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_UserDefinedFields] */
--GO

/* We do not DELETE the tables!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UserDefinedRows]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_UserDefinedRows] */
--GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddUserDefinedField]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddUserDefinedRow]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteUserDefinedField]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteUserDefinedRow]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleUserDefinedField]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleUserDefinedRow]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_getuserdefinedfields]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_getuserdefinedfields]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetUserDefinedRows]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetUserDefinedRows]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedData]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedData]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedField]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedField]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedFieldOrder]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedFieldOrder]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateUserDefinedRow]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateUserDefinedRow]
GO
