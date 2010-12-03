using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Security;
using Appleseed.Framework.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using Path = Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules 
{
	/// <summary>
	/// Documents
	/// </summary>
	public partial class Documents : PortalModuleControl 
	{
		/// <summary>
		/// 
		/// </summary>
		protected DataView myDataView;
		/// <summary>
		/// 
		/// </summary>
		protected string sortField;
		/// <summary>
		/// 
		/// </summary>
		protected string sortDirection;

		// Jminond - addded to upgraded extension set
		private string baseImageDIR = string.Empty;
		private Hashtable availExtensions = new Hashtable();

		/// <summary>
		/// Constructor
		/// </summary>
		public Documents() 
		{

			// 17/12/2004 added localization for new settings by José Viladiu (jviladiu@portalServices.net)

			#region Document Setting Items
			// Modified by Hongwei Shen(hongwei.shen@gmail.com) to group the settings
			// 13/9/2005
			SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			int groupBase = (int)group;
			// end of modification

			SettingItem DocumentPath = new SettingItem(new PortalUrlDataType());
			DocumentPath.Required = true;
			DocumentPath.Value = "Documents";
			// Modified by Hongwei Shen
			// DocumentPath.Order = 1;
			DocumentPath.Group = group;
			DocumentPath.Order = groupBase + 20;
			// end of modification
			DocumentPath.EnglishName = "Document path";
			DocumentPath.Description = "Folder for store the documents";
			this._baseSettings.Add("DocumentPath", DocumentPath);

			// Add new functionalities by jviladiu@portalServices.net (02/07/2004)
			SettingItem ShowImages = new SettingItem(new BooleanDataType());
			ShowImages.Value = "true";
			// Modified by Hongwei Shen
			// ShowImages.Order = 5;
			ShowImages.Group = group;
			ShowImages.Order = groupBase + 25;
			// end of modification
			ShowImages.EnglishName = "Show Image Icons?";
			ShowImages.Description = "Mark this if you like see Image Icons";
			this._baseSettings.Add("DOCUMENTS_SHOWIMAGES", ShowImages);

			SettingItem SaveInDataBase = new SettingItem(new BooleanDataType());
			SaveInDataBase.Value = "false";
			// Modified by Hongwei Shen
			// SaveInDataBase.Order = 10;
			SaveInDataBase.Group = group;
			SaveInDataBase.Order = groupBase + 30;
			// end of modification
			SaveInDataBase.EnglishName = "Save files in DataBase?";
			SaveInDataBase.Description = "Mark this if you like save files in DataBase";
			this._baseSettings.Add("DOCUMENTS_DBSAVE", SaveInDataBase);

			// Added sort by fields by Chris Thames [icecold_2@hotmail.com] (11/17/2004)
			SettingItem	SortByField	= new SettingItem(new ListDataType(General.GetString("DOCUMENTS_SORTBY_FIELD_LIST", "File Name;Created Date")));
			SortByField.Required=true;
			SortByField.Value =	"File Name";
			// Modified by Hongwei Shen
			// SortByField.Order = 11;
			SortByField.Group = group;
			SortByField.Order = groupBase + 35;
			// end of modification
			SortByField.EnglishName = "Sort Field?";
			SortByField.Description = "Sort by File Name or by Created Date?";
			this._baseSettings.Add("DOCUMENTS_SORTBY_FIELD", SortByField);

			SettingItem SortByDirection = new SettingItem(new ListDataType(General.GetString("DOCUMENTS_SORTBY_DIRECTION_LIST", "Ascending;Descending")));
			SortByDirection.Value = "Ascending";
			// Modified by Hongwei Shen
			// SortByDirection.Order = 12;
			SortByDirection.Group = group;
			SortByDirection.Order = groupBase + 40;
			// end of modification
			SortByDirection.EnglishName = "Sort ascending or descending?";
			SortByDirection.Description = "Ascending: A to Z or 0 - 9. Descending: Z - A or 9 - 0.";
			this._baseSettings.Add("DOCUMENTS_SORTBY_DIRECTION", SortByDirection);
			// End

			// Added by Jakob Hansen 07/07/2004
			SettingItem showTitle = new SettingItem(new BooleanDataType());
			showTitle.Value = "true";
			// Modified by Hongwei Shen
			// showTitle.Order = 15;
			showTitle.Group = group;
			showTitle.Order = groupBase + 45;
			// end of modification
			showTitle.EnglishName = "Show Title column?";
			showTitle.Description = "Mark this if the title column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWTITLE", showTitle);

			SettingItem showOwner = new SettingItem(new BooleanDataType());
			showOwner.Value = "true";
			// Modified by Hongwei Shen
			// showOwner.Order = 16;
			showOwner.Group = group;
			showOwner.Order = groupBase + 50;
			// end of modification
			showOwner.EnglishName = "Show Owner column?";
			showOwner.Description = "Mark this if the owner column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWOWNER", showOwner);

			SettingItem showArea = new SettingItem(new BooleanDataType());
			showArea.Value = "true";
			// Modified by Hongwei Shen
			// showArea.Order = 17;
			showArea.Group = group;
			showArea.Order = groupBase + 55;
			// end of modification
			showArea.EnglishName = "Show Area column";
			showArea.Description = "Mark this if the area column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWAREA", showArea);

			SettingItem showLastUpdated = new SettingItem(new BooleanDataType());
			showLastUpdated.Value = "true";
			// Modified by Hongwei Shen
			// showLastUpdated.Order = 18;
			showLastUpdated.Group = group;
			showLastUpdated.Order = groupBase + 60;
			// end of modification
			showLastUpdated.EnglishName = "Show Last Updated column";
			showLastUpdated.Description = "Mark this if the Last Updated column should be displayed";
			this._baseSettings.Add("DOCUMENTS_SHOWLASTUPDATED", showLastUpdated);
			// End Change Jakob Hansen

			#endregion

			// Change by Geert.Audenaert@Syntegra.Com
			// Date: 27/2/2003
			SupportsWorkflow = true;
			// End Change Geert.Audenaert@Syntegra.Com
		}

        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// obtain a SqlDataReader of document information from the
        /// Documents table, and then databind the results to a DataGrid
        /// server control.  It uses the Appleseed.DocumentDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, EventArgs e) 
		{
			LoadAvailableImageList();

			if (!Page.IsPostBack) 
			{
				string sortFieldOption		= Settings["DOCUMENTS_SORTBY_FIELD"].ToString();
				string sortDirectionOption	= Settings["DOCUMENTS_SORTBY_DIRECTION"].ToString();
				if (sortFieldOption.Length > 0)
				{
					if (General.GetString("DOCUMENTS_SORTBY_FIELD_LIST", "File Name;Created Date").IndexOf(sortFieldOption) > 0)
						sortField = "CreatedDate";
					else
						sortField = "FileFriendlyName";

					if (General.GetString("DOCUMENTS_SORTBY_DIRECTION_LIST", "Ascending;Descending").IndexOf(sortDirectionOption) > 0)
						sortDirection = "DESC";
					else
						sortDirection = "ASC";
				}
				else
				{
					sortField = "FileFriendlyName";
					sortDirection = "ASC";
					if (sortField == "DueDate")
						sortDirection = "DESC";
				}
				ViewState["SortField"] = sortField;
				ViewState["SortDirection"] = sortDirection;
			}
			else
			{
				sortField = (string) ViewState["SortField"];
				sortDirection = (string) ViewState["sortDirection"];
			}

			myDataView = new DataView();

			// Obtain Document Data from Documents table
			// and bind to the datalist control
			DocumentDB documents = new DocumentDB();

			// DataSet documentsData = documents.GetDocuments(ModuleID, Version);
			// myDataView = documentsData.Tables[0].DefaultView;
			setDataView (documents.GetDocuments(ModuleID, Version));

			if (!Page.IsPostBack)
				myDataView.Sort = sortField + " " + sortDirection;

			BindGrid();
		}

        /// <summary>
        /// Bind the grid
        /// </summary>
		protected void BindGrid()
		{
			myDataGrid.DataSource = myDataView;

			myDataGrid.Columns[0].Visible = IsEditable;
			myDataGrid.Columns[1].Visible = bool.Parse(Settings["DOCUMENTS_SHOWIMAGES"].ToString());
			myDataGrid.Columns[2].Visible = bool.Parse(Settings["DOCUMENTS_SHOWTITLE"].ToString());
			myDataGrid.Columns[3].Visible = bool.Parse(Settings["DOCUMENTS_SHOWOWNER"].ToString());
			myDataGrid.Columns[4].Visible = bool.Parse(Settings["DOCUMENTS_SHOWAREA"].ToString());
			myDataGrid.Columns[5].Visible = bool.Parse(Settings["DOCUMENTS_SHOWLASTUPDATED"].ToString());

			myDataGrid.DataBind();
		}


		#region Image documents

        /// <summary>
        /// Sets the data view.
        /// </summary>
        /// <param name="documentsData">The documents data.</param>
		private void setDataView (DataSet documentsData) 
		{
			myDataView = documentsData.Tables[0].DefaultView;
			foreach (DataRow dr in myDataView.Table.Rows) 
			{
				dr["contentType"] = imageAsign ((string) dr["contentType"]);
			}
		}

        /// <summary>
        /// Loads the available image list.
        /// </summary>
		private void LoadAvailableImageList()
		{
			string bDir = Server.MapPath(this.baseImageDIR);
			DirectoryInfo di = new DirectoryInfo(bDir);
			FileInfo[] rgFiles = di.GetFiles("*.gif");
			string ext = string.Empty;
			string nme = string.Empty;
			string f_Name = string.Empty;

			foreach(FileInfo fi in rgFiles)
			{
				f_Name = fi.Name;
				ext = fi.Extension;
				nme = f_Name.Substring(0, (f_Name.Length - ext.Length));
				availExtensions.Add(nme, f_Name);
			}

		}

        /// <summary>
        /// Images the asign.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
		private string imageAsign (string contentType) 
		{
			// jminond - switched to use extensions pack
			if(availExtensions.ContainsKey(contentType))
			{
				return availExtensions[contentType].ToString();
			}
			else
			{
				return "unknown.gif";
			}
		}
		#endregion

        /// <summary>
        /// GetBrowsePath() is a helper method used to create the url
        /// to the document.  If the size of the content stored in the
        /// database is non-zero, it creates a path to browse that.
        /// Otherwise, the FileNameUrl value is used.
        /// This method is used in the databinding expression for
        /// the browse Hyperlink within the DataGrid, and is called
        /// for each row when DataGrid.DataBind() is called.  It is
        /// defined as a helper method here (as opposed to inline
        /// within the template) to improve code organization and
        /// avoid embedding logic within the content template.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="size">The size.</param>
        /// <param name="documentID">The document ID.</param>
        /// <returns></returns>
		protected string GetBrowsePath(string url, object size, int documentID) 
		{
			if (size != DBNull.Value && (int) size > 0) 
			{
				// if there is content in the database, create an url to browse it
				// Add ModuleID into url for correct security access. jviladiu@portalServices.net (02/07/2004)
                return (HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Documents/DocumentsView.aspx", "ItemID=" + documentID.ToString() + "&MId=" + ModuleID.ToString() + "&wversion=" + Version.ToString()));
			}
			else 
			{
				// otherwise, return the FileNameUrl
				return url;
			}
		}

		#region General Implementation
        /// <summary>
        /// General Module Def GUID
        /// </summary>
        /// <value></value>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{F9645B82-CB45-4C4C-BB2D-72FA42FE2B75}");
			}
		}

		#region Search Implementation

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
		public override bool Searchable
		{
			get
			{
				return true;
			}
		}

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
		public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
		{
			SearchDefinition s = new SearchDefinition("rb_Documents", "FileFriendlyName", "FileNameUrl", "CreatedByUser", "CreatedDate", searchField);
			
			//Add extra search fields here, this way
			s.ArrSearchFields.Add("itm.Category");

			return s.SearchSqlSelect(portalID, userID, searchString);
		}
		#endregion

		# region Install / Uninstall Implementation
        /// <summary>
        /// Install
        /// </summary>
        /// <param name="stateSaver"></param>
		public override void Install(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}

        /// <summary>
        /// Uninstall
        /// </summary>
        /// <param name="stateSaver"></param>
		public override void Uninstall(IDictionary stateSaver)
		{
			string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
			ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
			if (errors.Count > 0)
			{
				// Call rollback
				throw new Exception("Error occurred:" + errors[0].ToString());
			}
		}
		# endregion

		#endregion          

		#region Web Form Designer generated code
        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
            this.myDataGrid.SortCommand += new DataGridSortCommandEventHandler(this.myDataGrid_SortCommand);
            this.myDataGrid.ItemDataBound += new DataGridItemEventHandler(this.myDataGrid_ItemDataBound);
            this.Load += new EventHandler(this.Page_Load);

			this.baseImageDIR = Path.ApplicationRoot + "/aspnet_client/Ext/";

			myDataGrid.EnableViewState = false;
			// Create a new Title the control
//			ModuleTitle = new DesktopModuleTitle();
			// Set here title properties
			// Add support for the edit page
			this.AddText = "ADD_DOCUMENT";
			this.AddUrl = "~/DesktopModules/CommunityModules/Documents/DocumentsEdit.aspx";
			// Add title ad the very beginning of 
			// the control's controls collection
//			Controls.AddAt(0, ModuleTitle);
		
			base.OnInit(e);
		}
		#endregion

        /// <summary>
        /// Handles the SortCommand event of the myDataGrid control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		protected void myDataGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (sortField == e.SortExpression)
			{
				if (sortDirection == "ASC")
					sortDirection = "DESC";
				else
					sortDirection = "ASC";
			}

			ViewState["SortField"] = e.SortExpression;
			ViewState["sortDirection"] = sortDirection;

			myDataView.Sort = e.SortExpression + " " + sortDirection;
			BindGrid();
		}

        /// <summary>
        /// Handles the ItemDataBound event of the myDataGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
        protected void myDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
			if (e.Item.Cells [3].Text == "unknown")
			{
				e.Item.Cells [3].Text = General.GetString ( "UNKNOWN", "unknown");
			}
		}
	}
}
