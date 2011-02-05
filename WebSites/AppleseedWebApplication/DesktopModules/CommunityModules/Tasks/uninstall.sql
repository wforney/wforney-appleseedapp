/* Uninstall script, Tasks module, Jakob Hansen, 29 april 2003 */


/* No we do not DELETE the table!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Tasks]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_Tasks] */
--GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddTask]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteTask]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetTasks]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetTasks]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleTask]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateTask]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateTask]
GO
