using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Tasks Module - Edit page part
    /// Writen by: ?
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class TasksEdit : AddEditItemPage
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        protected IHtmlEditor DesktopText;

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the task to edit.
        /// It then uses the Appleseed.TasksDB() data component
        /// to populate the page's edit controls with the task details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if an
            // task itemID value is specified, and if so populate page
            // contents with the task details

            //Chris Farrell, chris@cftechconsulting.com, 5/28/04
            //Added support for Appleseed WYSIWYG editors.
            //Editor placeholder setup
            HtmlEditorDataType h = new HtmlEditorDataType();
            h.Value = moduleSettings["Editor"].ToString();
            DesktopText =
                h.GetEditor(DescriptionField, ModuleID, bool.Parse(moduleSettings["ShowUpload"].ToString()),
                            portalSettings);

            DesktopText.Width = new Unit(moduleSettings["Width"].ToString());
            DesktopText.Height = new Unit(moduleSettings["Height"].ToString());
            //end Chris Farrell changes, 5/28/04


            //Set right popup url
            StartField.xPopupURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/popupcalendar.aspx";
            StartField.xImageURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/calendar.jpg";
            DueField.xPopupURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/popupcalendar.aspx";
            DueField.xImageURL = Path.ApplicationRoot + "/DesktopModules/DateTextBox/calendar.jpg";

            if (Page.IsPostBack == false)
            {
                StartField.Text = DateTime.Now.ToShortDateString();
                DueField.Text = DateTime.Now.ToShortDateString();
                AddListItem("TASK_STATE_0", "Not Started", StatusField);
                AddListItem("TASK_STATE_1", "In Progress", StatusField);
                AddListItem("TASK_STATE_2", "Complete", StatusField);
                StatusField.SelectedIndex = 0;
                AddListItem("TASK_PRIORITY_0", "High", PriorityField);
                AddListItem("TASK_PRIORITY_1", "Normal", PriorityField);
                AddListItem("TASK_PRIORITY_2", "Low", PriorityField);

                PriorityField.SelectedIndex = 1;
                if (ItemID != 0)
                {
                    // Obtain a single row of Task information
                    TasksDB Tasks = new TasksDB();
                    SqlDataReader dr = Tasks.GetSingleTask(ItemID);

                    try
                    {
                        // Read first row from database
                        if (dr.Read())
                        {
                            TitleField.Text = (string) dr["Title"];
                            StartField.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
                            DueField.Text = ((DateTime) dr["DueDate"]).ToShortDateString();
                            CreatedBy.Text = (string) dr["CreatedByUser"];
                            ModifiedBy.Text = (string) dr["ModifiedByUser"];
                            PercentCompleteField.Text = ((Int32) dr["PercentComplete"]).ToString();
                            AssignedField.Text = (string) dr["AssignedTo"];
                            CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToString();
                            ModifiedDate.Text = ((DateTime) dr["ModifiedDate"]).ToString();
                            StatusField.SelectedIndex = Convert.ToInt16((string) dr["Status"]);
                            PriorityField.SelectedIndex = Convert.ToInt16((string) dr["Priority"]);
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown")
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (ModifiedBy.Text == "unknown")
                            {
                                ModifiedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }

                            //Chris Farrell, chris@cftechconsulting.com, 5/28/04
                            //DescriptionField.Text = (string) dr["Description"];
                            DesktopText.Text = (string) dr["Description"];
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
                else //default for new
                {
                    AssignedField.Text = moduleSettings["TASKS_DEFAULT_ASSIGNEE"].ToString();
                }
            }
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531012");
                return al;
            }
        }

        /// <summary>
        /// The OnUpdate event handler on this Page is used to either
        /// create or update an task. It uses the Appleseed.TasksDB()
        /// data component to encapsulate all data functionality.
        /// Note: This procedure is automaticall called on Update
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdate(EventArgs e)
        {
            // Calling base we check if the user has rights on updating
            base.OnUpdate(e);

            // Only Update if the Entered Data is Valid
            if (Page.IsValid == true)
            {
                // Create an instance of the Task DB component
                TasksDB tasks = new TasksDB();

                if (ItemID == 0)
                {
                    // Add the task within the Tasks table

                    //by Manu
                    //First get linked task modules
                    string[] linkedModules = moduleSettings["TASKS_LINKED_MODULES"].ToString().Split(';');

                    for (int i = 0; i < linkedModules.Length; i++)
                    {
                        int linkedModuleID = int.Parse(linkedModules[i]);

                        //If not module is null or current
                        if (linkedModuleID != 0 && linkedModuleID != ModuleID)
                        {
                            //Add to linked

                            //Get default assignee from module setting
                            Hashtable linkedModuleSettings = ModuleSettings.GetModuleSettings(linkedModuleID, this);
                            string linkedModuleAssignee = linkedModuleSettings["TASKS_DEFAULT_ASSIGNEE"].ToString();

                            tasks.AddTask(linkedModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email,
                                          TitleField.Text, DateTime.Parse(StartField.Text), DesktopText.Text,
                                          StatusField.SelectedItem.Value, PriorityField.SelectedItem.Value,
                                          linkedModuleAssignee, DateTime.Parse(DueField.Text),
                                          Int16.Parse(PercentCompleteField.Text));
                        }
                    }

                    //Add to current
                    tasks.AddTask(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, TitleField.Text,
                                  DateTime.Parse(StartField.Text), DesktopText.Text, StatusField.SelectedItem.Value,
                                  PriorityField.SelectedItem.Value, AssignedField.Text, DateTime.Parse(DueField.Text),
                                  Int16.Parse(PercentCompleteField.Text));
                }
                else
                {
                    // Update the task within the Tasks table
                    tasks.UpdateTask(ModuleID, ItemID, PortalSettings.CurrentUser.Identity.Email, TitleField.Text,
                                     DateTime.Parse(StartField.Text), DesktopText.Text, StatusField.SelectedItem.Value,
                                     PriorityField.SelectedItem.Value, AssignedField.Text, DateTime.Parse(DueField.Text),
                                     Int16.Parse(PercentCompleteField.Text));
                }

                // Redirect back to the portal home page
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// The OnDelete event handler on this Page is used to delete
        /// an task. It uses the Appleseed.TasksDB() data component to
        /// encapsulate all data functionality.
        /// Note:This procedure is automaticall called on Update
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDelete(EventArgs e)
        {
            // Calling base we check if the user has rights on deleting
            base.OnUpdate(e);

            // Only attempt to delete the item if it is an existing item
            // (new items will have "ItemID" of 0)

            if (ItemID != 0)
            {
                TasksDB tasks = new TasksDB();
                tasks.DeleteTask(ItemID);
            }

            // Redirect back to the portal home page
            RedirectBackToReferringPage();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="translation"></param>
        /// <param name="sender"></param>
        protected void AddListItem(string key, string translation, DropDownList sender)
        {
            ListItem Item = new ListItem();
            Item.Value = key.Substring(key.Length - 1, 1);
            Item.Text = General.GetString(key, translation, sender);
            sender.Items.Add(Item);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Translate! Jakob Hansen says: TBD!!
            //RequiredTitle.ErrorMessage = Esperantus.General.GetString("TASKS_VALID_TITLE");
            //PercentValidator.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_PERCENT");
            //VerifyStartDate.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_STARTDATE");
            //VerifyDueDate.ErrorMessage = Esperantus.General.GetString("TASKS_INVALID_DUEDATE");

            InitializeComponent();

            // Jes1111
            if (!((Page) this.Page).IsCssFileRegistered("Mod_Tasks"))
                ((Page) this.Page).RegisterCssFile("Mod_Tasks");

            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}