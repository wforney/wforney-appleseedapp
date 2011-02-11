using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Content.Security;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;
using HyperLink=Appleseed.Framework.Web.UI.WebControls.HyperLink;
using ImageButton=Appleseed.Framework.Web.UI.WebControls.ImageButton;
using Label=Appleseed.Framework.Web.UI.WebControls.Label;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// The discussion module allwos for simple threaded discussion 
    /// with full HTML support in the discussion.
    /// </summary>
    public partial class Discussion : PortalModuleControl
    {
        /// <summary>
        /// details of thread list
        /// </summary>
        protected DataList DetailList;

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// On the first invocation of Page_Load, the data is bound using BindList();
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            /*if (Page.IsPostBack == false) 
			{
				BindList();
			}*/
            BindList();
        }

        /// <summary>
        /// The BindList method obtains the list of top-level messages
        /// from the Discussion table and then databinds them against
        /// the "TopLevelList" asp:datalist server control.  It uses
        /// the Appleseed.DiscussionDB() data component to encapsulate
        /// all data access functionality.
        /// </summary>
        private void BindList()
        {
            // Obtain a list of discussion messages for the module and bind to datalist
            DiscussionDB discuss = new DiscussionDB();

            TopLevelList.DataSource = discuss.GetTopLevelMessages(ModuleID);
            TopLevelList.DataBind();
        }

        /// <summary>
        /// The GetThreadMessages method is used to obtain the list
        /// of messages contained within a sub-topic of the
        /// a top-level discussion message thread.  This method is
        /// used to populate the "DetailList" asp:datalist server control
        /// in the SelectedItemTemplate of "TopLevelList".
        /// </summary>
        /// <returns>returns a SqlDataReader object</returns>
        protected SqlDataReader GetThreadMessages()
        {
            DiscussionDB discuss = new DiscussionDB();
            int itemID = Int32.Parse(TopLevelList.DataKeys[TopLevelList.SelectedIndex].ToString());
            SqlDataReader dr = discuss.GetThreadMessages(itemID, 'N');
            return dr;
        }

        /// <summary>
        /// The TopLevelList_Select server event handler is used to
        /// expand/collapse a selected discussion topic and delte individual items
        /// </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="e">DataListCommandEventAargs e</param>
        public void TopLevelListOrDetailList_Select(object Sender, DataListCommandEventArgs e)
        {
            // Determine the command of the button
            string command = ((CommandEventArgs) (e)).CommandName;

            // Update asp:datalist selection index depending upon the type of command
            // and then rebind the asp:datalist with content
            switch (command)
            {
                case "CollapseThread":
                    {
                        TopLevelList.SelectedIndex = -1; // nothing is selected
                        break;
                    }
                case "ExpandThread":
                    {
                        TopLevelList.SelectedIndex = e.Item.ItemIndex;
                        DiscussionDB discuss = new DiscussionDB();
                        int ItemID = Int32.Parse(e.CommandArgument.ToString());
                        discuss.IncrementViewCount(ItemID);
                        break;
                    }
                case "ShowThreadNewWindow": // open up the entire thread in a new window
                    {
                        TopLevelList.SelectedIndex = e.Item.ItemIndex;
                        DiscussionDB discuss = new DiscussionDB();
                        int ItemID = Int32.Parse(e.CommandArgument.ToString());
                        discuss.IncrementViewCount(ItemID);
                        Response.Redirect(FormatUrlShowThread(ItemID));
                        break;
                    }
                    /*
					case "SelectTitle":
					TopLevelList.SelectedIndex = e.Item.ItemIndex;
					Response.Redirect(FormatUrlShowThread((int)DataBinder.Eval(Container.DataItem, "ItemID")));
					break;
				*/
                case "delete": // the "delete" command can come from the TopLevelList or the DetailList
                    {
                        DiscussionDB discuss = new DiscussionDB();
                        int ItemID = Int32.Parse(e.CommandArgument.ToString());
                        discuss.DeleteChildren(ItemID);
                        // DetailList.DataBind();	// synchronize the control and database after deletion
                        break;
                    }
                default:
                    break;
            }
            BindList();
        }

        /// <summary>
        /// set up a client-side javascript dialog to confirm deletions
        /// the 'confirm' dialog is called when onClick is triggered
        /// if the dialog returns false the server never gets the delete request
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.</param>
        protected void OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            // 13/7/2004 Added Localization Mario Endara mario@softworks.com.uy
            if (e.Item.FindControl("deleteBtn") != null)
            {
                ((ImageButton) e.Item.FindControl("deleteBtn")).Attributes.Add("onClick", "return confirm('" +
                                                                                          General.GetString(
                                                                                              "DISCUSSION_DELETE_RESPONSE",
                                                                                              "Are you sure you want to delete the selected response message and ALL of its children ?") +
                                                                                          "');");

                ((ImageButton) e.Item.FindControl("deleteBtn")).Attributes.Add("title",
                                                                               General.GetString("DELETE",
                                                                                                 "Delete this thread"));
            }
            if (e.Item.FindControl("deleteBtnExpanded") != null)
            {
                ((ImageButton) e.Item.FindControl("deleteBtnExpanded")).Attributes.Add("onClick", "return confirm('" +
                                                                                                  General.GetString(
                                                                                                      "DISCUSSION_DELETE_RESPONSE",
                                                                                                      "Are you sure you want to delete the selected response message and ALL of its children ?") +
                                                                                                  "');");

                ((ImageButton) e.Item.FindControl("deleteBtnExpanded")).Attributes.Add("title",
                                                                                       General.GetString("DELETE",
                                                                                                         "Delete this thread"));
            }
            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
            if (e.Item.FindControl("Label4") != null)
            {
                if (((Label) e.Item.FindControl("Label4")).Text == "unknown")
                {
                    ((Label) e.Item.FindControl("Label4")).Text = General.GetString("UNKNOWN", "unknown");
                }
            }

            if (e.Item.FindControl("Label10") != null)
            {
                if (((Label) e.Item.FindControl("Label10")).Text == "unknown")
                {
                    ((Label) e.Item.FindControl("Label10")).Text = General.GetString("UNKNOWN", "unknown");
                }
            }
            if (e.Item.FindControl("Label6") != null)
            {
                ((Label) e.Item.FindControl("Label6")).ToolTip =
                    General.GetString("DISCUSSION_REPLYS", "Number of replys to this topic");
            }
            if (e.Item.FindControl("Label5") != null)
            {
                ((Label) e.Item.FindControl("Label5")).ToolTip =
                    General.GetString("DISCUSSION_VIEWED", "Number of times this topic has been viewed");
            }
            if (e.Item.FindControl("Label1") != null)
            {
                ((Label) e.Item.FindControl("Label1")).ToolTip =
                    General.GetString("DISCUSSION_REPLYS", "Number of replys to this topic");
            }
            if (e.Item.FindControl("Label2") != null)
            {
                ((Label) e.Item.FindControl("Label2")).ToolTip =
                    General.GetString("DISCUSSION_VIEWED", "Number of times this topic has been viewed");
            }
            if (e.Item.FindControl("btnCollapse") != null)
            {
                ((ImageButton) e.Item.FindControl("btnCollapse")).ToolTip =
                    General.GetString("DISCUSSION_MSGCOLLAPSE", "Collapse the thread of this topic");
            }
            if (e.Item.FindControl("btnSelect") != null)
            {
                ((ImageButton) e.Item.FindControl("btnSelect")).ToolTip =
                    General.GetString("DISCUSSION_MSGEXPAND", "Expand the thread of this topic inside this browser page");
            }
            if (e.Item.FindControl("btnNewWindow") != null)
            {
                ((ImageButton) e.Item.FindControl("btnNewWindow")).ToolTip =
                    General.GetString("DISCUSSION_MSGSELECT", "Open the thread of this topic in a new browser page");
            }

            // jminond - add tooltip support for firefox
            // Relpy
            if (e.Item.FindControl("HyperLink2") != null)
            {
                ((HyperLink) e.Item.FindControl("HyperLink2")).Attributes.Add("title",
                                                                              General.GetString("DS_REPLYTHISMSG",
                                                                                                "Reply to this message"));
            }

            if (e.Item.FindControl("HyperLink1") != null)
            {
                ((HyperLink) e.Item.FindControl("HyperLink1")).ToolTip = General.GetString("EDIT", "Edit this message");
            }

            // End FireFox Tooltip Support
        }

        /// <summary>
        /// GetReplyImage
        /// </summary>
        /// <returns></returns>
        protected string GetReplyImage()
        {
            if (DiscussionPermissions.HasAddPermissions(ModuleID) == true)
                return getLocalImage("reply.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// GetEditImage
        /// </summary>
        /// <param name="itemUserEmail">The item user email.</param>
        /// <returns></returns>
        protected string GetEditImage(string itemUserEmail)
        {
            if (DiscussionPermissions.HasEditPermissions(ModuleID, itemUserEmail))
                return getLocalImage("edit.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// GetDeleteImage
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <param name="itemUserEmail">The item user email.</param>
        /// <returns></returns>
        protected string GetDeleteImage(int itemID, string itemUserEmail)
        {
            if (DiscussionPermissions.HasDeletePermissions(ModuleID, itemID, itemUserEmail) == true)
                return getLocalImage("delete.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// The FormatUrl method is a helper messages called by a
        /// databinding statement within the &lt;asp:DataList&gt; server
        /// control template.  It is defined as a helper method here
        /// (as opposed to inline within the template) to improve
        /// code organization and avoid embedding logic within the
        /// content template.
        /// </summary>
        /// <param name="itemID">ID of the currently selected topic</param>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// Returns a properly formatted URL to call the DiscussionEdit page
        /// </returns>
        protected string FormatUrlEditItem(int itemID, string mode)
        {
            return
                (HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Discussion/DiscussionEdit.aspx",
                                         "ItemID=" + itemID + "&Mode=" + mode + "&mID=" + ModuleID + "&edit=1"));
        }

        /// <summary>
        /// FormatUrlShowThread
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <returns></returns>
        protected string FormatUrlShowThread(int itemID)
        {
            return
                (HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Discussion/DiscussionViewThread.aspx",
                                         "ItemID=" + itemID + "&mID=" + ModuleID));
        }

        /// <summary>
        /// The NodeImage method is a helper method called by a
        /// databinding statement within the &lt;asp:datalist&gt; server
        /// control template.  It controls whether or not an item
        /// in the list should be rendered as an expandable topic
        /// or just as a single node within the list.
        /// </summary>
        /// <param name="count">Number of replys to the selected topic</param>
        /// <returns></returns>
        protected string NodeImage(int count)
        {
            return getLocalImage("Thread.png");
        }

        /// <summary>
        /// Gets the local image.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        protected string getLocalImage(string img)
        {
            return CurrentTheme.GetModuleImageSRC(img);
        }

        /// <summary>
        /// Module GUID
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2D86166C-4BDC-4A6F-A028-D17C2BB177C8}"); }
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
            SearchDefinition s =
                new SearchDefinition("rb_Discussion", "Title", "Body", "CreatedByUser", "CreatedDate", searchField);
            return s.SearchSqlSelect(portalID, userID, searchString);
        }


        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
        public Discussion()
        {
            // Jminond - added editor support
            HtmlEditorDataType.HtmlEditorSettings(_baseSettings, SettingItemGroup.MODULE_SPECIAL_SETTINGS);

            /*
			 * SettingItem setSortField = new SettingItem(new ListDataType("CreatedDate;Title"));
			setSortField.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
			setSortField.Required = true;
			setSortField.Value = "DueDate";
			this._baseSettings.Add("DISCUSSION_SORT_FIELD", setSortField);
			*/
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.TopLevelList.ItemCommand += new DataListCommandEventHandler(this.TopLevelListOrDetailList_Select);
            // this.DetailList.ItemCommand   += new System.Web.UI.WebControls.DataListCommandEventHandler(this.TopLevelListOrDetailList_Select);
            this.Load += new EventHandler(this.Page_Load);
            if (!this.Page.IsCssFileRegistered("Mod_Discussion"))
                this.Page.RegisterCssFile("Mod_Discussion");

            this.AddText = "DS_NEWTHREAD";
            this.AddUrl = "~/DesktopModules/CommunityModules/Discussion/DiscussionEdit.aspx";

            base.OnInit(e);
        }

        #endregion

        # region Install / Uninstall Implementation

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
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
            string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        # endregion
    }
}