How workflow works

1. the editor switches to the staging view and edit's the content
2. the editor has finished editing it's content so he clicks the 
   "Request approval button"
   Then he's shown a page where he can (not must) send an email 
   to the guy who has to approve to new content.
3. The approval guy get's it's email and navigates opens his browser 
   and navigates to the specific module the approval guy has two option's:
   approve & reject
   when he clicks the approve button he's shown a page where he can (not must) 
   send an email to the guy who has to publish the new content


Add support for workflow in your module

Workflow at the moment work’s only for modules which persist there content 
in the Appleseed database. 

The table in which the content is persisted must be linked through a foreign 
key to the modules table. 
If this table contains child tables itself then the the workflow will fail. 
I can support this too, but then I have to make the publish stored proc recursive.

Do the following steps

1) Database modifications
- Create an identical table and name it as rb_Tablename_st in the database 
  to the table in which you store the modules content. 
- Modify your stored procedure so updates the staging table instead of the 
  “production” table (delete, add and update). 
- Add a trigger to the staging table ('rb_Tablename_st'). 
  For example see to the trigger on the rb_HtmlText_st table. 

CREATE TRIGGER [rb_Tablename_stModified]
ON [rb_Tablename_st]
FOR DELETE, INSERT, UPDATE 
AS 
BEGIN
	DECLARE ChangedModules CURSOR FOR
		SELECT ModuleID
		FROM inserted
		UNION
		SELECT ModuleID
		FROM deleted

	DECLARE @ModID	int

	OPEN ChangedModules	

	FETCH NEXT FROM ChangedModules
	INTO @ModID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC rb_ModuleEdited @ModID

		FETCH NEXT FROM ChangedModules
		INTO @ModID
	END

	CLOSE ChangedModules
	DEALLOCATE ChangedModules

END

- Add a parameter to the stored procedure you use for retrieving the content 
  to indicate if you want the staging data or the production data. 
  For example see rb_GetHtmlText :
  
CREATE   PROCEDURE rb_GetHtmlText
(
    @ModuleID int,
    @WorkflowVersion int
)
AS

IF ( @WorkflowVersion = 1 )
	SELECT *
	FROM
	    rb_HtmlText
	WHERE
	    ModuleID = @ModuleID
ELSE
	SELECT *
	FROM
	    rb_HtmlText_st
	WHERE
	    ModuleID = @ModuleID
GO
  

2) C# source code modifications
- Add a parameter to indicate the version you want to retrieve 
  to the retrieve function in your DB component corresponding to your module.
  
  public SqlDataReader GetHtmlText(int moduleID, Appleseed.Framework.WorkFlowVersion version)


  SqlParameter parameterWorkflowVersion = new SqlParameter("@WorkflowVersion", SqlDbType.Int, 4);
  parameterWorkflowVersion.Value = (int)version;
  myCommand.Parameters.Add(parameterWorkflowVersion);

  
- In the constructor of your module set the “SupportsWorkflow” property = true;

	public HtmlModule()
	{
		//...
		SupportsWorkflow = true;
	}
	
- In the databinding procedure of your module use the “Version” property of 
  the PortalModuleControl with the retrieving procedure of your DB component 
  corresponding to your module.
  
	// Obtain the selected item from the HtmlText table
	HtmlTextDB text = new HtmlTextDB();
	SqlDataReader dr = text.GetHtmlText(ModuleID, Version);

  
- In the databinding of the edit page of your module always use WorkflowVersion.Staging, 
  because you always have to bind your editing env to the staging environment.
  
	// Obtain a single row of text information
	HtmlTextDB text = new HtmlTextDB();
	SqlDataReader dr = text.GetHtmlText(ModuleID, Appleseed.Framework.WorkFlowVersion.Staging);
