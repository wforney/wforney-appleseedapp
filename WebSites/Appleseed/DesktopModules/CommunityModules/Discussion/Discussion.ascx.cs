// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Discussion.ascx.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The discussion module allows for simple threaded discussion
//   with full HTML support in the discussion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules.Discussion
{
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

    using Resources;

    using HyperLink = Appleseed.Framework.Web.UI.WebControls.HyperLink;
    using ImageButton = Appleseed.Framework.Web.UI.WebControls.ImageButton;
    using Label = Appleseed.Framework.Web.UI.WebControls.Label;

    /// <summary>
    /// The discussion module allows for simple threaded discussion 
    ///   with full HTML support in the discussion.
    /// </summary>
    public partial class DiscussionModule : PortalModuleControl
    {
        #region Constants and Fields

        /// <summary>
        ///   details of thread list
        /// </summary>
        protected DataList DetailList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DiscussionModule" /> class.
        /// </summary>
        public DiscussionModule()
        {
            // Jminond - added editor support
            HtmlEditorDataType.HtmlEditorSettings(this.BaseSettings, SettingItemGroup.MODULE_SPECIAL_SETTINGS);

            /*
             * SettingItem setSortField = new SettingItem(new ListDataType("CreatedDate;Title"));
            setSortField.Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            setSortField.Required = true;
            setSortField.Value = "DueDate";
            this._baseSettings.Add("DISCUSSION_SORT_FIELD", setSortField);
            */
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets GUID of module (mandatory)
        /// </summary>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{2D86166C-4BDC-4A6F-A028-D17C2BB177C8}");
            }
        }

        /// <summary>
        ///   Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Installs the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        public override void Install(IDictionary stateSaver)
        {
            var currentScriptName = Path.Combine(this.Server.MapPath(this.TemplateSourceDirectory), "install.sql");
            var errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception(string.Format("Error occurred:{0}", errors[0]));
            }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalId">
        /// The portal ID
        /// </param>
        /// <param name="userId">
        /// ID of the user is searching
        /// </param>
        /// <param name="searchString">
        /// The text to search
        /// </param>
        /// <param name="searchField">
        /// The fields where perfoming the search
        /// </param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
        public override string SearchSqlSelect(int portalId, int userId, string searchString, string searchField)
        {
            var s = new SearchDefinition("rb_Discussion", "Title", "Body", "CreatedByUser", "CreatedDate", searchField);
            return s.SearchSqlSelect(portalId, userId, searchString);
        }

        /// <summary>
        /// The TopLevelList_Select server event handler is used to
        ///   expand/collapse a selected discussion topic and delete individual items
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// DataListCommandEventAargs e
        /// </param>
        public void TopLevelListOrDetailListSelect(object sender, DataListCommandEventArgs e)
        {
            // Update asp:datalist selection index depending upon the type of command
            // and then rebind the asp:datalist with content
            switch (e.CommandName)
            {
                case "CollapseThread":
                    {
                        this.TopLevelList.SelectedIndex = -1; // nothing is selected
                        break;
                    }

                case "ExpandThread":
                    {
                        this.TopLevelList.SelectedIndex = e.Item.ItemIndex;
                        var discuss = new DiscussionDB();
                        var itemId = Int32.Parse(e.CommandArgument.ToString());
                        discuss.IncrementViewCount(itemId);
                        break;
                    }

                case "ShowThreadNewWindow":
                    {
                        // open up the entire thread in a new window
                        this.TopLevelList.SelectedIndex = e.Item.ItemIndex;
                        var discuss = new DiscussionDB();
                        var itemId = Int32.Parse(e.CommandArgument.ToString());
                        discuss.IncrementViewCount(itemId);
                        this.Response.Redirect(this.FormatUrlShowThread(itemId));
                        break;
                    }

                    /*
                case "SelectTitle":
                TopLevelList.SelectedIndex = e.Item.ItemIndex;
                Response.Redirect(FormatUrlShowThread((int)DataBinder.Eval(Container.DataItem, "ItemID")));
                break;
                */
                case "delete":
                    {
                        // the "delete" command can come from the TopLevelList or the DetailList
                        var discuss = new DiscussionDB();
                        var itemId = Int32.Parse(e.CommandArgument.ToString());
                        discuss.DeleteChildren(itemId);

                        // DetailList.DataBind(); // synchronize the control and database after deletion
                        break;
                    }

                default:
                    break;
            }

            this.BindList();
        }

        /// <summary>
        /// Uninstalls the specified state saver.
        /// </summary>
        /// <param name="stateSaver">
        /// The state saver.
        /// </param>
        public override void Uninstall(IDictionary stateSaver)
        {
            var currentScriptName = Path.Combine(this.Server.MapPath(this.TemplateSourceDirectory), "uninstall.sql");
            var errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception(string.Format("Error occurred:{0}", errors[0]));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The FormatUrl method is a helper messages called by a
        ///   data binding statement within the &lt;asp:DataList&gt; server
        ///   control template.  It is defined as a helper method here
        ///   (as opposed to inline within the template) to improve
        ///   code organization and avoid embedding logic within the
        ///   content template.
        /// </summary>
        /// <param name="itemId">
        /// ID of the currently selected topic
        /// </param>
        /// <param name="mode">
        /// The mode of the edit item.
        /// </param>
        /// <returns>
        /// Returns a properly formatted URL to call the DiscussionEdit page
        /// </returns>
        protected string FormatUrlEditItem(int itemId, string mode)
        {
            return HttpUrlBuilder.BuildUrl(
                "~/DesktopModules/CommunityModules/Discussion/DiscussionEdit.aspx", 
                string.Format("ItemID={0}&Mode={1}&mID={2}&edit=1", itemId, mode, this.ModuleID));
        }

        /// <summary>
        /// Format URL Show Thread
        /// </summary>
        /// <param name="itemId">
        /// The item ID.
        /// </param>
        /// <returns>
        /// The URL for the thread.
        /// </returns>
        protected string FormatUrlShowThread(int itemId)
        {
            return HttpUrlBuilder.BuildUrl(
                "~/DesktopModules/CommunityModules/Discussion/DiscussionViewThread.aspx", 
                string.Format("ItemID={0}&mID={1}", itemId, this.ModuleID));
        }

        /// <summary>
        /// Gets the delete image.
        /// </summary>
        /// <param name="itemId">
        /// The item ID.
        /// </param>
        /// <param name="itemUserEmail">
        /// The item user email.
        /// </param>
        /// <returns>
        /// The get delete image.
        /// </returns>
        protected string GetDeleteImage(int itemId, string itemUserEmail)
        {
            return
                this.GetLocalImage(
                    DiscussionPermissions.HasDeletePermissions(this.ModuleID, itemId, itemUserEmail)
                        ? "delete.png"
                        : "1x1.gif");
        }

        /// <summary>
        /// Gets the edit image.
        /// </summary>
        /// <param name="itemUserEmail">
        /// The item user email.
        /// </param>
        /// <returns>
        /// The get edit image.
        /// </returns>
        protected string GetEditImage(string itemUserEmail)
        {
            return
                this.GetLocalImage(
                    DiscussionPermissions.HasEditPermissions(this.ModuleID, itemUserEmail) ? "edit.png" : "1x1.gif");
        }

        /// <summary>
        /// Gets the reply image.
        /// </summary>
        /// <returns>
        /// The get reply image.
        /// </returns>
        protected string GetReplyImage()
        {
            return this.GetLocalImage(DiscussionPermissions.HasAddPermissions(this.ModuleID) ? "reply.png" : "1x1.gif");
        }

        /// <summary>
        /// The GetThreadMessages method is used to obtain the list
        ///   of messages contained within a sub-topic of the
        ///   a top-level discussion message thread.  This method is
        ///   used to populate the "DetailList" asp:datalist server control
        ///   in the SelectedItemTemplate of "TopLevelList".
        /// </summary>
        /// <returns>
        /// returns a SqlDataReader object
        /// </returns>
        protected SqlDataReader GetThreadMessages()
        {
            var discuss = new DiscussionDB();
            var itemId = Int32.Parse(this.TopLevelList.DataKeys[this.TopLevelList.SelectedIndex].ToString());
            var dr = discuss.GetThreadMessages(itemId, 'N');
            return dr;
        }

        /// <summary>
        /// The NodeImage method is a helper method called by a
        ///   databinding statement within the &lt;asp:datalist&gt; server
        ///   control template.  It controls whether or not an item
        ///   in the list should be rendered as an expandable topic
        ///   or just as a single node within the list.
        /// </summary>
        /// <param name="count">
        /// Number of replys to the selected topic
        /// </param>
        /// <returns>
        /// The node image.
        /// </returns>
        protected string NodeImage(int count)
        {
            return this.GetLocalImage("Thread.png");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.TopLevelList.ItemCommand += this.TopLevelListOrDetailListSelect;

            // this.DetailList.ItemCommand   += new System.Web.UI.WebControls.DataListCommandEventHandler(this.TopLevelListOrDetailList_Select);
            if (!this.Page.IsCssFileRegistered("Mod_Discussion"))
            {
                this.Page.RegisterCssFile("Mod_Discussion");
            }

            this.AddText = "DS_NEWTHREAD";
            this.AddUrl = "~/DesktopModules/CommunityModules/Discussion/DiscussionEdit.aspx";

            this.BindList();

            base.OnInit(e);
        }

        /// <summary>
        /// set up a client-side JavaScript dialog to confirm deletions
        ///   the 'confirm' dialog is called when onClick is triggered
        ///   if the dialog returns false the server never gets the delete request
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.
        /// </param>
        protected void OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            // 13/7/2004 Added Localization Mario Endara mario@softworks.com.uy
            if (e.Item.FindControl("deleteBtn") != null)
            {
                var response = General.GetString(
                    "DISCUSSION_DELETE_RESPONSE",
                    "Are you sure you want to delete the selected response message and ALL of its children ?");
                ((ImageButton)e.Item.FindControl("deleteBtn")).Attributes.Add(
                    "onClick",
                    string.Format("return confirm('{0}');", response));

                ((ImageButton)e.Item.FindControl("deleteBtn")).Attributes.Add(
                    "title", General.GetString("DELETE", "Delete this thread"));
            }

            if (e.Item.FindControl("deleteBtnExpanded") != null)
            {
                var response = General.GetString(
                    "DISCUSSION_DELETE_RESPONSE",
                    "Are you sure you want to delete the selected response message and ALL of its children ?");
                ((ImageButton)e.Item.FindControl("deleteBtnExpanded")).Attributes.Add(
                    "onClick",
                    string.Format("return confirm('{0}');", response));

                ((ImageButton)e.Item.FindControl("deleteBtnExpanded")).Attributes.Add(
                    "title", General.GetString("DELETE", "Delete this thread"));
            }

            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
            if (e.Item.FindControl("Label4") != null)
            {
                if (((Label)e.Item.FindControl("Label4")).Text == Appleseed.Discussion_unknown)
                {
                    ((Label)e.Item.FindControl("Label4")).Text = General.GetString("UNKNOWN", Appleseed.Discussion_unknown);
                }
            }

            if (e.Item.FindControl("Label10") != null)
            {
                if (((Label)e.Item.FindControl("Label10")).Text == Appleseed.Discussion_unknown)
                {
                    ((Label)e.Item.FindControl("Label10")).Text = General.GetString("UNKNOWN", Appleseed.Discussion_unknown);
                }
            }

            if (e.Item.FindControl("Label6") != null)
            {
                ((Label)e.Item.FindControl("Label6")).ToolTip = General.GetString(
                    "DISCUSSION_REPLYS", "Number of replys to this topic");
            }

            if (e.Item.FindControl("Label5") != null)
            {
                ((Label)e.Item.FindControl("Label5")).ToolTip = General.GetString(
                    "DISCUSSION_VIEWED", "Number of times this topic has been viewed");
            }

            if (e.Item.FindControl("Label1") != null)
            {
                ((Label)e.Item.FindControl("Label1")).ToolTip = General.GetString(
                    "DISCUSSION_REPLYS", "Number of replys to this topic");
            }

            if (e.Item.FindControl("Label2") != null)
            {
                ((Label)e.Item.FindControl("Label2")).ToolTip = General.GetString(
                    "DISCUSSION_VIEWED", "Number of times this topic has been viewed");
            }

            if (e.Item.FindControl("btnCollapse") != null)
            {
                ((ImageButton)e.Item.FindControl("btnCollapse")).ToolTip = General.GetString(
                    "DISCUSSION_MSGCOLLAPSE", "Collapse the thread of this topic");
            }

            if (e.Item.FindControl("btnSelect") != null)
            {
                ((ImageButton)e.Item.FindControl("btnSelect")).ToolTip = General.GetString(
                    "DISCUSSION_MSGEXPAND", "Expand the thread of this topic inside this browser page");
            }

            if (e.Item.FindControl("btnNewWindow") != null)
            {
                ((ImageButton)e.Item.FindControl("btnNewWindow")).ToolTip = General.GetString(
                    "DISCUSSION_MSGSELECT", "Open the thread of this topic in a new browser page");
            }

            // jminond - add tooltip support for firefox
            // Relpy
            if (e.Item.FindControl("HyperLink2") != null)
            {
                ((HyperLink)e.Item.FindControl("HyperLink2")).Attributes.Add(
                    "title", General.GetString("DS_REPLYTHISMSG", "Reply to this message"));
            }

            if (e.Item.FindControl("HyperLink1") != null)
            {
                ((HyperLink)e.Item.FindControl("HyperLink1")).ToolTip = General.GetString("EDIT", "Edit this message");
            }

            // End FireFox Tooltip Support
        }

        /// <summary>
        /// Gets the local image.
        /// </summary>
        /// <param name="img">
        /// The image.
        /// </param>
        /// <returns>
        /// The source path of the image.
        /// </returns>
        protected string GetLocalImage(string img)
        {
            return this.CurrentTheme.GetModuleImageSRC(img);
        }

        /// <summary>
        /// The BindList method obtains the list of top-level messages
        ///   from the Discussion table and then data binds them against
        ///   the "TopLevelList" asp:datalist server control.  It uses
        ///   the Appleseed.DiscussionDB() data component to encapsulate
        ///   all data access functionality.
        /// </summary>
        private void BindList()
        {
            // Obtain a list of discussion messages for the module and bind to data list
            var discuss = new DiscussionDB();

            this.TopLevelList.DataSource = discuss.GetTopLevelMessages(this.ModuleID);
            this.TopLevelList.DataBind();
        }

        #endregion
    }
}