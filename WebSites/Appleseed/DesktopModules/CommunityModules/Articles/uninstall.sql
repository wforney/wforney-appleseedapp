/* Uninstall script, Articles module, mario@hartmann.net, 07/09/03 */


/* No we do not DELETE the table!
IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_Articles]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [rb_Articles]
*/
--GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_AddArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_AddArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_DeleteArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_DeleteArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetArticles]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetArticles]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetSingleArticleWithImages]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetSingleArticleWithImages]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_UpdateArticle]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_UpdateArticle]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[rb_GetArticlesAll]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rb_GetArticlesAll]
GO
