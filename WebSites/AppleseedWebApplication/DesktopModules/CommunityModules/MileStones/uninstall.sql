/* Uninstall script, MileStones module, [Mario@Hartmann.net], 27/05/2003 */

/* No we do not DELETE the tables!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Milestones]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_Milestones]
*/
--GO

/*
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Milestones_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_Milestones_st]
*/
--GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Milestones_st]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1 AND (deltrig <> 0 OR instrig <> 0 OR updtrig <> 0))
DROP TRIGGER [rb_Milestones_stModified]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddMilestones]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteMilestones]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetMilestones]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleMilestones]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateMilestones]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateMilestones]
GO


