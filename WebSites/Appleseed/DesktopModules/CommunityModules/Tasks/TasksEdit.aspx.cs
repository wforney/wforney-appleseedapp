// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TasksEdit.aspx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   Tasks Module - Edit page part
//   Writen by: ?
//   Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// Tasks Module - Edit page part
    ///   Writen by: ?
    ///   Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class TasksEdit : AddEditItemPage
    {
        #region Constants and Fields

        /// <summary>
        /// The desktop text.
        /// </summary>
        protected IHtmlEditor DesktopText;

        #endregion

        #region Properties

        /// <summary>
        ///   Set the module guids with free access to this page
        /// </summary>
        protected override List<string> AllowedModules
        {
            get
            {
                var al = new List<string>();
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531012");
                return al;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add list item.
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="translation">
        /// </param>
        /// <param name="sender">
        /// </param>
        protected void AddListItem(string key, string translation, DropDownList sender)
        {
            var Item = new ListItem();
            Item.Value = key.Substring(key.Length - 1, 1);
            Item.Text = General.GetString(key, translation, sender);
            sender.Items.Add(Item);
        }

        /// <summary>
        /// The OnDelete event handler on this Page is used to delete
        ///   an task. It uses the Appleseed.TasksDB() data component to
        ///   encapsulate all data functionality.
        ///   Note:This procedure is automaticall called on Update
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnDelete(EventArgs e)
        {
            // Calling base we check if the user has rights on deleting
            base.OnUpdate(e);

            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)
            if (this.ItemID != 0)
            {
                var tasks = new TasksDB();
                tasks.DeleteTask(this.ItemID);
            }

            // Redirect back to the portal home page
            this.RedirectBackToReferringPage();
        }

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            // Translate! Jakob Hansen says: TBD!!
            // RequiredTitle.ErrorMessage = Esperantus.General.GetString("TASKS_VALID_TITLE");
            // PercentValidator.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_PERCENT");
            // VerifyStartDate.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_STARTDATE");
            // VerifyDueDate.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_DUEDATE");
            InitializeComponent();

            // Jes1111
            if (!((Page)this.Page).IsCssFileRegistered("Mod_Tasks"))
            {
                ((Page)this.Page).RegisterCssFile("Mod_Tasks");
            }

            base.OnInit(e);
        }

        /// <summary>
        /// The OnUpdate event handler on this Page is used to either
        ///   create or update an task. It uses the Appleseed.TasksDB()
        ///   data component to encapsulate all data functionality.
        ///   Note: This procedure is automaticall called on Update
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnUpdate(EventArgs e)
        {
            // Calling base we check if the user has rights on updating
            base.OnUpdate(e);

            // Only Update if the Entered Data is Valid
            if (this.Page.IsValid)
            {
                // Create an instance of the Task DB component
                var tasks = new TasksDB();

                if (this.ItemID == 0)
                {
                    // Add the task within the Tasks table

                    // by Manu
                    // First get linked task modules
                    var linkedModules = this.ModuleSettings["TASKS_LINKED_MODULES"].ToString().Split(';');

                    for (var i = 0; i < linkedModules.Length; i++)
                    {
                        var linkedModuleID = int.Parse(linkedModules[i]);

                        // If not module is null or current
                        if (linkedModuleID != 0 && linkedModuleID != this.ModuleID)
                        {
                            // Add to linked

                            // Get default assignee from module setting
                            var linkedModuleSettings = Framework.Site.Configuration.ModuleSettings.GetModuleSettings(linkedModuleID, this);
                            var linkedModuleAssignee = linkedModuleSettings["TASKS_DEFAULT_ASSIGNEE"].ToString();

                            tasks.AddTask(
                                linkedModuleID, 
                                this.ItemID, 
                                PortalSettings.CurrentUser.Identity.Email, 
                                this.TitleField.Text, 
                                DateTime.Parse(this.StartField.Text), 
                                this.DesktopText.Text, 
                                this.StatusField.SelectedItem.Value, 
                                this.PriorityField.SelectedItem.Value, 
                                linkedModuleAssignee, 
                                DateTime.Parse(this.DueField.Text), 
                                Int16.Parse(this.PercentCompleteField.Text));
                        }
                    }

                    // Add to current
                    tasks.AddTask(
                        this.ModuleID, 
                        this.ItemID, 
                        PortalSettings.CurrentUser.Identity.Email, 
                        this.TitleField.Text, 
                        DateTime.Parse(this.StartField.Text), 
                        this.DesktopText.Text, 
                        this.StatusField.SelectedItem.Value, 
                        this.PriorityField.SelectedItem.Value, 
                        this.AssignedField.Text, 
                        DateTime.Parse(this.DueField.Text), 
                        Int16.Parse(this.PercentCompleteField.Text));
                }
                else
                {
                    // Update the task within the Tasks table
                    tasks.UpdateTask(
                        this.ModuleID, 
                        this.ItemID, 
                        PortalSettings.CurrentUser.Identity.Email, 
                        this.TitleField.Text, 
                        DateTime.Parse(this.StartField.Text), 
                        this.DesktopText.Text, 
                        this.StatusField.SelectedItem.Value, 
                        this.PriorityField.SelectedItem.Value, 
                        this.AssignedField.Text, 
                        DateTime.Parse(this.DueField.Text), 
                        Int16.Parse(this.PercentCompleteField.Text));
                }

                // Redirect back to the portal home page
                this.RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += this.Page_Load;
        }

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        ///   and ItemID of the task to edit.
        ///   It then uses the Appleseed.TasksDB() data component
        ///   to populate the page's edit controls with the task details.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // task itemID value is specified, and if so populate page
            // contents with the task details

            // Chris Farrell, chris@cftechconsulting.com, 5/28/04
            // Added support for Appleseed WYSIWYG editors.
            // Editor placeholder setup
            var h = new HtmlEditorDataType();
            h.Value = this.ModuleSettings["Editor"].ToString();
            this.DesktopText = h.GetEditor(
                this.DescriptionField, 
                this.ModuleID, 
                bool.Parse(this.ModuleSettings["ShowUpload"].ToString()), 
                this.PortalSettings);

            this.DesktopText.Width = new Unit(this.ModuleSettings["Width"].ToString());
            this.DesktopText.Height = new Unit(this.ModuleSettings["Height"].ToString());

            // end Chris Farrell changes, 5/28/04

            // Set right popup url
            this.StartField.xPopupURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/popupcalendar.aspx";
            this.StartField.xImageURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/calendar.jpg";
            this.DueField.xPopupURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/popupcalendar.aspx";
            this.DueField.xImageURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/calendar.jpg";

            if (this.Page.IsPostBack == false)
            {
                this.StartField.Text = DateTime.Now.ToShortDateString();
                this.DueField.Text = DateTime.Now.ToShortDateString();
                this.AddListItem("TASK_STATE_0", "Not Started", this.StatusField);
                this.AddListItem("TASK_STATE_1", "In Progress", this.StatusField);
                this.AddListItem("TASK_STATE_2", "Complete", this.StatusField);
                this.StatusField.SelectedIndex = 0;
                this.AddListItem("TASK_PRIORITY_0", "High", this.PriorityField);
                this.AddListItem("TASK_PRIORITY_1", "Normal", this.PriorityField);
                this.AddListItem("TASK_PRIORITY_2", "Low", this.PriorityField);

                this.PriorityField.SelectedIndex = 1;
                if (this.ItemID != 0)
                {
                    // Obtain a single row of Task information
                    var Tasks = new TasksDB();
                    var dr = Tasks.GetSingleTask(this.ItemID);

                    try
                    {
                        // Read first row from database
                        if (dr.Read())
                        {
                            this.TitleField.Text = (string)dr["Title"];
                            this.StartField.Text = ((DateTime)dr["StartDate"]).ToShortDateString();
                            this.DueField.Text = ((DateTime)dr["DueDate"]).ToShortDateString();
                            this.CreatedBy.Text = (string)dr["CreatedByUser"];
                            this.ModifiedBy.Text = (string)dr["ModifiedByUser"];
                            this.PercentCompleteField.Text = ((Int32)dr["PercentComplete"]).ToString();
                            this.AssignedField.Text = (string)dr["AssignedTo"];
                            this.CreatedDate.Text = ((DateTime)dr["CreatedDate"]).ToString();
                            this.ModifiedDate.Text = ((DateTime)dr["ModifiedDate"]).ToString();
                            this.StatusField.SelectedIndex = Convert.ToInt16((string)dr["Status"]);
                            this.PriorityField.SelectedIndex = Convert.ToInt16((string)dr["Priority"]);

                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (this.CreatedBy.Text == "unknown")
                            {
                                this.CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }

                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (this.ModifiedBy.Text == "unknown")
                            {
                                this.ModifiedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }

                            // Chris Farrell, chris@cftechconsulting.com, 5/28/04
                            // DescriptionField.Text = (string) dr["Description"];
                            this.DesktopText.Text = (string)dr["Description"];
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
                else
                {
                    // default for new
                    this.AssignedField.Text = this.ModuleSettings["TASKS_DEFAULT_ASSIGNEE"].ToString();
                }
            }
        }

        #endregion
    }
}