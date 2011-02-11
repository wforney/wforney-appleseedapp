using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using BoundColumn = Appleseed.Framework.Web.UI.WebControls.BoundColumn;
using Path = Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules 
{

	/// <summary>
	/// Users Defined Table module
	/// Written by: Shaun Walker (IbuySpy Workshop)
	/// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
	/// </summary>
	public partial class UserDefinedTable : PortalModuleControl 
	{
		protected DataGrid grdData;
		protected UserDefinedTableXML xmlControl;
		protected IHtmlEditor xmlText;

        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, EventArgs e) 
		{
			if (!Page.IsPostBack )
			{
				ViewState["SortField"] = string.Empty;
				ViewState["SortOrder"] = string.Empty;

				if ( IsEditable )
					cmdManage.Visible = true;
				else
					cmdManage.Visible = false;
			}
 
			if(Settings["XSLsrc"].ToString().Length >0 || bool.Parse(Settings["DisplayAsXML"].ToString())==true)
			{
				xmlControl = new UserDefinedTableXML(XmlDataView(),PageID,ModuleIDsrc(),IsEditable,GetSortField(),GetSortOrder());
				xmlControl.SortCommand += new UDTXSLSortEventHandler(xmlData_Sort);

				if(bool.Parse(Settings["DisplayAsXML"].ToString())==true)
				{
					//Show Raw XML
					xmlText = new TextEditor();
					xmlText.Text = xmlControl.InnerXml;
					xmlText.Width = 450;
					xmlText.Height = 400;				
					PlaceHolderOutput.Controls.Add((Control) xmlText);
				}
				else
				{
					//Show XSL data
					PlaceHolderOutput.Controls.Add(xmlControl);
					BindXSL();
				}	
			}
			else
			{
				//Show datagrid
				grdData = new DataGrid();
				grdData.BorderWidth = 0;
				grdData.CellPadding = 4;
				grdData.AutoGenerateColumns = false;
				grdData.HeaderStyle.CssClass = "NormalBold";
				grdData.ItemStyle.CssClass = "Normal";
				grdData.AllowSorting = true;
				grdData.SortCommand += new DataGridSortCommandEventHandler(grdData_Sort);
				PlaceHolderOutput.Controls.Add(grdData);
				BindGrid();
			}
			//Rob Siera - 04 nov 2004 - Show ManageButton only when having Permission to edit Properties
			cmdManage.Visible=ArePropertiesEditable;
        }


        /// <summary>
        /// XMLs the data view.
        /// </summary>
        /// <returns></returns>
		private DataView XmlDataView()
		{
			UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();
			DataSet ds;

			ds = objUserDefinedTable.GetUserDefinedRows(ModuleIDsrc());

			// create a dataview to process the sort and filter options
			return new DataView(ds.Tables[0]);
		}

        /// <summary>
        /// Binds the XSL.
        /// </summary>
		public void BindXSL()
		{
			PortalUrlDataType pt = new PortalUrlDataType();
			pt.Value = Settings["XSLsrc"].ToString();
			string xslsrc = pt.FullPath;

			if ((xslsrc != null) && (xslsrc.Length != 0)) 
			{
				if  (File.Exists(Server.MapPath(xslsrc))) 
				{
					xmlControl.TransformSource = xslsrc;
					// Change - 28/Feb/2003 - Jeremy Esland
					// Builds cache dependency files list
					this.ModuleConfiguration.CacheDependency.Add(Server.MapPath(xslsrc));
				}
				else 
				{
					xmlControl.TransformSource = Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/default.xslt");
					Controls.Add(new LiteralControl("<br>" + "<span class='Error'>" +General.GetString("FILE_NOT_FOUND").Replace("%1%", xslsrc) + "<br>"));
				}
			}

		}


        /// <summary>
        /// Modules the I DSRC.
        /// </summary>
        /// <returns></returns>
		private int ModuleIDsrc()
		{
			//Rob Siera - 04 nov 2004 - Adding possibility to use data of other UDT when XSL specified
			if(int.Parse(Settings["UDTsrc"].ToString())>0)
			{
				return int.Parse(Settings["UDTsrc"].ToString());
			}
			else
			{
				return ModuleID;
			}
		}
        /// <summary>
        /// Gets the sort field.
        /// </summary>
        /// <returns></returns>
		public string GetSortField()
		{
			if(xmlControl!=null && xmlControl.SortField.Length != 0)
			{
				return xmlControl.SortField;
			}
			else if ( ViewState["SortField"].ToString().Length != 0  )
			{
				return ViewState["SortField"].ToString();
			} 
			else if ( Settings["SortField"].ToString().Length != 0 )
			{
				return Settings["SortField"].ToString();
			}
			return string.Empty;
		}
        /// <summary>
        /// Gets the sort order.
        /// </summary>
        /// <returns></returns>
		public string GetSortOrder()
		{

			if(xmlControl!=null && xmlControl.SortOrder.Length != 0)
			{
				return xmlControl.SortOrder;
			}
			else if (  ViewState["SortOrder"].ToString().Length != 0 )
			{
				return ViewState["SortOrder"].ToString();
			} 
			else if ( Settings["SortField"].ToString().Length != 0 )
			{
				return Settings["SortOrder"].ToString();
			}
			return "ASC";
		}


        /// <summary>
        /// Binds the grid.
        /// </summary>
		protected void BindGrid()
		{
            UserDefinedTableDB  objUserDefinedTable = new UserDefinedTableDB();

            string strSortField = string.Empty;
            string strSortOrder = string.Empty;

			SqlDataReader dr;

            if ( ViewState["SortField"].ToString().Length != 0 && ViewState["SortOrder"].ToString().Length != 0 )
			{
                strSortField = ViewState["SortField"].ToString();
                strSortOrder = ViewState["SortOrder"].ToString();
            } 
			else
			{
                if ( Settings["SortField"].ToString().Length != 0 )
					strSortField = Settings["SortField"].ToString();

				if ( Settings["SortOrder"].ToString().Length != 0 )
                    strSortOrder = Settings["SortOrder"].ToString();
                else 
                    strSortOrder = "ASC";
            }

			grdData.Columns.Clear();

            dr = objUserDefinedTable.GetUserDefinedFields(ModuleID);
			try
			{
				while ( dr.Read())
				{
					DataGridColumn colField = null;
					if(dr["FieldType"].ToString() == "Image")
					{
						colField = new BoundColumn();
						((BoundColumn)colField).DataField = dr["FieldTitle"].ToString();
						((BoundColumn)colField).DataFormatString = "<img src=\"" +((SettingItem)Settings["ImagePath"]).FullPath + "/{0}" + "\" alt=\"{0}\" border =0>" ;
					}
					else if(dr["FieldType"].ToString() == "File")
					{
						colField = new HyperLinkColumn();
						((HyperLinkColumn)colField).DataTextField = dr["FieldTitle"].ToString();
						((HyperLinkColumn)colField).DataTextFormatString = "{0}";
						((HyperLinkColumn)colField).DataNavigateUrlFormatString = ((SettingItem) Settings["DocumentPath"]).FullPath + "/{0}";
						((HyperLinkColumn)colField).DataNavigateUrlField = dr["FieldTitle"].ToString();
					}
					else
					{
						colField = new BoundColumn();
						((BoundColumn)colField).DataField = dr["FieldTitle"].ToString();
						switch (dr["FieldType"].ToString())
						{ 
							case "DateTime":
								//Changed to Italian format as it is sayed to be the default (see intro of history.txt)
								//Better would be to make this follow the current culture - Rob Siera, 15 jan 2005
								((BoundColumn)colField).DataFormatString = "{0:dd MMM yyyy}";
								break;
							case "Int32":
								((BoundColumn)colField).DataFormatString = "{0:#,###,##0}";
								colField.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
								colField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
								break;
							case "Decimal":
								((BoundColumn)colField).DataFormatString = "{0:#,###,##0.00}";
								colField.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
								colField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
								break;
						}
					}

					colField.HeaderText = dr["FieldTitle"].ToString();
					if ( dr["FieldTitle"].ToString() == strSortField)
					{
						//  2004/07/04 by Ozan Sirin, FIX: It does not show sort images when running root site instead of Appleseed virtual folder.
						if ( strSortOrder == "ASC" )
							colField.HeaderText += "<img src='" + Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortascending.gif") + "' border='0' alt='" +General.GetString("USERTABLE_SORTEDBY", "Sorted By", null) + " " + strSortField + " " +General.GetString("USERTABLE_INASCORDER", "In Ascending Order", null) + "'>";
						else
							colField.HeaderText += "<img src='" + Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortdescending.gif") +  "' border='0' alt='" +General.GetString("USERTABLE_SORTEDBY", "Sorted By", null) + " " + strSortField + " " +General.GetString("USERTABLE_INDSCORDER", "In Descending Order", null) + "'>";
					}
					colField.Visible = bool.Parse(dr["Visible"].ToString());
					colField.SortExpression = dr["FieldTitle"].ToString() + "|ASC";

					grdData.Columns.Add(colField);
				}
			}
			finally
			{
				dr.Close();
			}

			if(IsEditable)
			{
				HyperLinkColumn hc = new HyperLinkColumn();
				hc.Text = "Edit";
				hc.DataNavigateUrlField = "UserDefinedRowID";
                hc.DataNavigateUrlFormatString = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/UserDefinedTable/UserDefinedTableEdit.aspx", PageID, "&mID=" + ModuleID + "&UserDefinedRowID={0}"); 
				grdData.Columns.Add(hc);
			}

            DataSet ds;
            ds = objUserDefinedTable.GetUserDefinedRows(ModuleID);

            // create a dataview to process the sort and filter options
			DataView dv;
			dv = new DataView(ds.Tables[0]);

            // sort data view
            if ( strSortField.Length != 0 && strSortOrder.Length != 0 )
                dv.Sort = strSortField + " " + strSortOrder;

            grdData.DataSource = dv;
            grdData.DataBind();
        }

        /// <summary>
        /// Handles the Sort event of the grdData control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		protected void grdData_Sort(Object source, DataGridSortCommandEventArgs e)
		{

			string[] strSort = e.SortExpression.Split('|');

            if ( strSort[0] == ViewState["SortField"].ToString() )
			{
                if ( ViewState["SortOrder"].ToString() == "ASC" )
                    ViewState["SortOrder"] = "DESC";
                else
                    ViewState["SortOrder"] = "ASC";
            } 
			else
			{
                ViewState["SortOrder"] = strSort[1];
            }

            ViewState["SortField"] = strSort[0];

            BindGrid();

        }

        /// <summary>
        /// Handles the Sort event of the xmlData control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="T:Appleseed.Content.Web.Modules.UDTXSLSortEventArgs"/> instance containing the event data.</param>
		protected void xmlData_Sort(Object source, UDTXSLSortEventArgs e)
		{
			xmlControl.SortField=e.SortField;
			xmlControl.SortOrder=e.SortOrder;

			BindXSL();

			ViewState["SortField"] = e.SortField;
			ViewState["SortOrder"] = e.SortOrder;
		}


        /// <summary>
        /// Handles the Click event of the cmdManage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void cmdManage_Click(object sender, EventArgs e)
		{
            Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/UserDefinedTable/UserDefinedTableManage.aspx", PageID, "&mID=" + ModuleIDsrc() + "&def=Manage UDT"));
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
		public override Guid GuidID 
		{
			get
			{
				return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531021}");
			}
		}

        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
		public UserDefinedTable() 
		{
			SettingItem setSortField = new SettingItem(new StringDataType());
			setSortField.Required = false;
			setSortField.Value = string.Empty;
			setSortField.Order = 1;
			this._baseSettings.Add("SortField", setSortField);

			SettingItem setSortOrder = new SettingItem(new ListDataType("ASC;DESC"));
			setSortOrder.Required = true;
			setSortOrder.Value = "ASC";
			setSortOrder.Order = 2;
			this._baseSettings.Add("SortOrder", setSortOrder);

			SettingItem DocumentPath = new SettingItem(new PortalUrlDataType());
			DocumentPath.Required = true;
			DocumentPath.Value = "Documents";
			DocumentPath.Order = 3;
			this._baseSettings.Add("DocumentPath", DocumentPath);

			SettingItem ImagePath = new SettingItem(new PortalUrlDataType());
			ImagePath.Required = true;
			ImagePath.Value = "Images\\Default";
			ImagePath.Order = 4;
			this._baseSettings.Add("ImagePath", ImagePath);

			SettingItem XSLsrc = new SettingItem(new PortalUrlDataType());
			XSLsrc.Required = false;
			XSLsrc.Order = 5;
			this._baseSettings.Add("XSLsrc", XSLsrc);

			//Rob Siera - 04 nov 2004 - Adding possibility to use data of other UDT
			SettingItem UDTsrc = new SettingItem(new IntegerDataType());
			UDTsrc.Required = false;
			UDTsrc.Value = ModuleID.ToString();
			UDTsrc.EnglishName="XSL data";
			UDTsrc.Description="Specify ModuleID of a UserDefinedTable to be used as data source for XSL (see 'mID' parameter in edit URL). Specify 0 to reset to current module data.";
			UDTsrc.Order = 6;
			this._baseSettings.Add("UDTsrc", UDTsrc);

			//Rob Siera - 04 nov 2004 - Adding possibility to view data as raw XML
			SettingItem DisplayAsXML = new SettingItem(new BooleanDataType());
			DisplayAsXML.Required = false;
			DisplayAsXML.EnglishName="Display XML";
			DisplayAsXML.Description="Toggle to display data as XML. Helpfull to develop XSL file.";
			DisplayAsXML.Order = 7;
			this._baseSettings.Add("DisplayAsXML", DisplayAsXML);
		}


        /// <summary>
        /// Unknown
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
        /// Unknown
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


		#region Web Form Designer generated code
        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
            this.Load += new EventHandler(this.Page_Load);
            this.AddUrl = "~/DesktopModules/CommunityModules/UserDefinedTable/UserDefinedTableEdit.aspx";
			base.OnInit(e);
		}
		#endregion

    }

}