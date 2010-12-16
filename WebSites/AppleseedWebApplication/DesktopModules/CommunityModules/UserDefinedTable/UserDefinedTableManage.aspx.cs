using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Web.UI;
using Label = System.Web.UI.WebControls.Label;
using LinkButton = System.Web.UI.WebControls.LinkButton;
using Page = System.Web.UI.Page;

namespace Appleseed.Content.Web.Modules 
{
    using System.Collections.Generic;

    /// <summary>
	/// Users Defined Table module - Manage page part
	/// Written by: Shaun Walker (IbuySpy Workshop)
	/// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
	/// </summary>
	public partial class UserDefinedTableManage : EditItemPage
	{
        /// <summary>
        /// The Page_Load event on this Page is used to ...
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, EventArgs e) 
		{
			if ( Page.IsPostBack == false )
			{				
				BindData();

				// Store URL Referrer to return to portal
				//ViewState("UrlReferrer") = Replace(Request.UrlReferrer.ToString(), "insertrow=true&", string.Empty);
				ViewState["UrlReferrer"] = Request.UrlReferrer.ToString().Replace("insertrow=true&", string.Empty);
			}
		}

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
		protected override List<string> AllowedModules
		{
			get
			{
				List<string> al = new List<string>();
				al.Add ("2502DB18-B580-4F90-8CB4-C15E6E531021");
				return al;
			}
		}

		//private void cmdCancel_Click( object sender,  EventArgs e)  cmdCancel.Click, cmdCancel.Click 
        /// <summary>
        /// Handles the Click event of the cmdCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			// Redirect back to the portal home page
			this.RedirectBackToReferringPage();
		}


        /// <summary>
        /// Handles the CancelEdit event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		protected void grdFields_CancelEdit(object sender, DataGridCommandEventArgs e)
		{
			grdFields.EditItemIndex = -1;
			BindData();
		}


        /// <summary>
        /// Handles the Edit event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		public void grdFields_Edit(object sender, DataGridCommandEventArgs e)
		{
			grdFields.EditItemIndex = e.Item.ItemIndex;
			grdFields.SelectedIndex = -1;
			BindData();
		}


        /// <summary>
        /// Handles the Update event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		public void grdFields_Update(object sender, DataGridCommandEventArgs e)
		{
			CheckBox chkVisible = (CheckBox) e.Item.Cells[1].Controls[1];
			TextBox txtFieldTitle = (TextBox) e.Item.Cells[2].Controls[1];
			DropDownList cboFieldType = (DropDownList) e.Item.Cells[3].Controls[1];

			if ( txtFieldTitle.Text.Length != 0 )
			{
				UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();

				if ( int.Parse(grdFields.DataKeys[e.Item.ItemIndex].ToString()) == -1 )
					objUserDefinedTable.AddUserDefinedField(ModuleID, txtFieldTitle.Text, chkVisible.Checked, cboFieldType.SelectedItem.Value);
				else
					objUserDefinedTable.UpdateUserDefinedField(int.Parse(grdFields.DataKeys[e.Item.ItemIndex].ToString()), txtFieldTitle.Text, chkVisible.Checked, cboFieldType.SelectedItem.Value);

				grdFields.EditItemIndex = -1;
				BindData();
			} 
			else 
			{
				grdFields.EditItemIndex = -1;
				BindData();
			}
		}


        /// <summary>
        /// Handles the Delete event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		public void grdFields_Delete(object sender, DataGridCommandEventArgs e)
		{
			UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();
			objUserDefinedTable.DeleteUserDefinedField(int.Parse(grdFields.DataKeys[e.Item.ItemIndex].ToString()));

			grdFields.EditItemIndex = -1;
			BindData();
		}


        /// <summary>
        /// Handles the Move event of the grdFields control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		public void grdFields_Move(object source, DataGridCommandEventArgs e)
		{
			UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();

			switch (e.CommandArgument.ToString())
			{
				case "Up":
					objUserDefinedTable.UpdateUserDefinedFieldOrder(int.Parse(grdFields.DataKeys[e.Item.ItemIndex].ToString()), -1);
					BindData();
					break;
				case "Down":
					objUserDefinedTable.UpdateUserDefinedFieldOrder(int.Parse(grdFields.DataKeys[e.Item.ItemIndex].ToString()), 1);
					BindData();
					break;
			}
		}


        /// <summary>
        /// Handles the Click event of the cmdAddField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void cmdAddField_Click(object sender,  EventArgs e)
		{
			grdFields.EditItemIndex = 0;
			BindData(true);
		}


        /// <summary>
        /// Converts the data reader to data set.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
		public DataSet ConvertDataReaderToDataSet( SqlDataReader reader)
		{

			DataSet dataSet = new DataSet();

			DataTable schemaTable = reader.GetSchemaTable();

			DataTable dataTable = new DataTable();

			int intCounter;

			for ( intCounter = 0 ; intCounter <= schemaTable.Rows.Count - 1; intCounter++)
			{
				DataRow dataRow = schemaTable.Rows[intCounter];
				string columnName = dataRow["ColumnName"].ToString();
				DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
				dataTable.Columns.Add(column);
			} 

			dataSet.Tables.Add(dataTable);

			try
			{
				while ( reader.Read())
				{
					DataRow dataRow = dataTable.NewRow();

					for (intCounter = 0; intCounter <= reader.FieldCount - 1; intCounter++)
						dataRow[intCounter] = reader.GetValue(intCounter);

					dataTable.Rows.Add(dataRow);
				}
			}
			finally
			{
				reader.Close(); //by Manu, fixed bug 807858
			}
			return dataSet;
		}


        /// <summary>
        /// Binds the data.
        /// </summary>
		protected void BindData()
		{
			BindData(false);
		}


        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="blnInsertField">if set to <c>true</c> [BLN insert field].</param>
		protected void BindData(bool blnInsertField)
		{
			UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();
			SqlDataReader dr = objUserDefinedTable.GetUserDefinedFields(ModuleID);

			DataSet ds;
			ds = ConvertDataReaderToDataSet(dr);

			// inserting a new field
			if ( blnInsertField )
			{
				DataRow row;
				row = ds.Tables[0].NewRow();
				row["UserDefinedFieldID"] = "-1";
				row["FieldTitle"] = string.Empty;
				row["Visible"] = true;
				row["FieldType"] = "String";
				ds.Tables[0].Rows.InsertAt(row, 0);
				grdFields.EditItemIndex = 0;
			}

			grdFields.DataSource = ds;
			grdFields.DataBind();
		}


        /// <summary>
        /// Handles the ItemCreated event of the grdFields control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
		protected void grdFields_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			Control cmdDeleteUserDefinedField = e.Item.FindControl("cmdDeleteUserDefinedField");

			if (cmdDeleteUserDefinedField != null )
			{
				ImageButton imgBut = (ImageButton) cmdDeleteUserDefinedField;
				imgBut.Attributes.Add("onClick", "javascript: return confirm('Are you sure you wish to delete this field?')");
			}
		}


        /// <summary>
        /// Gets the name of the field type.
        /// </summary>
        /// <param name="strFieldType">Type of the STR field.</param>
        /// <returns></returns>
		public string GetFieldTypeName(string strFieldType)
		{
			switch (strFieldType)
			{
				case "String" 	: return General.GetString("USERTABLE_TYPE_STRING", "Text",null);
				case "Int32" 	: return General.GetString("USERTABLE_TYPE_INT32", "Integer",null);
				case "Decimal" 	: return General.GetString("USERTABLE_TYPE_DECIMAL", "Decimal",null);
				case "DateTime" : return General.GetString("USERTABLE_TYPE_DATETIME", "Date",null);
				case "Boolean" 	: return General.GetString("USERTABLE_TYPE_BOOLEAN", "True/False",null);
				case "File" 	: return General.GetString("USERTABLE_TYPE_FILE", "File",null);
				case "Image" 	: return General.GetString("USERTABLE_TYPE_IMAGE", "Image",null);
				default			: return General.GetString("USERTABLE_TYPE_STRING", "Text",null);
			}
		}


        /// <summary>
        /// Gets the index of the field type.
        /// </summary>
        /// <param name="strFieldType">Type of the STR field.</param>
        /// <returns></returns>
		public int GetFieldTypeIndex(string strFieldType)
		{
			switch (strFieldType)
			{
				case "String" : return 0;
				case "Int32" : return 1;
				case "Decimal" : return 2;
				case "DateTime" : return 3;
				case "Boolean" : return 4;
				case "File" : return 5;
				case "Image" : return 6;
				default: return 0;
			}
		}

        /// <summary>
        /// Ifs the visible.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="trueStr">The true STR.</param>
        /// <param name="falseStr">The false STR.</param>
        /// <returns></returns>
		public string IfVisible(object data, string trueStr, string falseStr)
		{
			bool check = bool.Parse(DataBinder.Eval(data, "Visible").ToString());
			return (check ? this.CurrentTheme.GetImage(trueStr, trueStr + ".gif").ImageUrl : 
							this.CurrentTheme.GetImage(falseStr, falseStr + ".gif").ImageUrl);
		}

        /// <summary>
        /// Gets the table types.
        /// </summary>
        /// <returns></returns>
		public UserDefinedTableType[] GetTableTypes()
		{
			UserDefinedTableType[] tableTypes = new UserDefinedTableType[7];
			tableTypes[0] = new UserDefinedTableType();
			tableTypes[0].TypeText =General.GetString("USERTABLE_TYPE_STRING", "Text",null);
			tableTypes[0].TypeValue = "String";			
			tableTypes[1] = new UserDefinedTableType();
			tableTypes[1].TypeText =General.GetString("USERTABLE_TYPE_INT32", "Integer",null);
			tableTypes[1].TypeValue = "Int32";	
			tableTypes[2] = new UserDefinedTableType();
			tableTypes[2].TypeText =General.GetString("USERTABLE_TYPE_DECIMAL", "Decimal",null);
			tableTypes[2].TypeValue = "Decimal";	
			tableTypes[3] = new UserDefinedTableType();
			tableTypes[3].TypeText =General.GetString("USERTABLE_TYPE_DATETIME", "Date",null);
			tableTypes[3].TypeValue = "DateTime";	
			tableTypes[4] = new UserDefinedTableType();
			tableTypes[4].TypeText =General.GetString("USERTABLE_TYPE_BOOLEAN", "True/False",null);
			tableTypes[4].TypeValue = "Boolean";	
			tableTypes[5] = new UserDefinedTableType();
			tableTypes[5].TypeText =General.GetString("USERTABLE_TYPE_FILE", "File",null);
			tableTypes[5].TypeValue = "File";	
			tableTypes[6] = new UserDefinedTableType();
			tableTypes[6].TypeText =General.GetString("USERTABLE_TYPE_IMAGE", "Image",null);
			tableTypes[6].TypeValue = "Image";	
			return tableTypes;
		}

		#region Web Form Designer generated code
        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
            this.Load += new EventHandler(this.Page_Load);

			base.OnInit(e);
		}
		#endregion
    }
}