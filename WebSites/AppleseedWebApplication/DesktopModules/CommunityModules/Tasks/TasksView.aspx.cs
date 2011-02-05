using System;
using System.Collections;
using System.Data.SqlClient;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// IBS Portal Tasks Module - Display all info about single task
    /// Writen by: ?
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    public partial class TaskView : ViewItemPage
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        protected string EditLink = string.Empty;

        #endregion

        /// <summary>
        /// The Page_Load event on this Page is used to obtain the ModuleID
        /// and ItemID of the task to display.
        /// It then uses the Appleseed.TasksDB() data component
        /// to populate the page's edit controls with the task details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Verify that the current user has access to edit this module
            if (PortalSecurity.HasEditPermissions(ModuleID))
            {
                EditLink = "<a href= \"TasksEdit.aspx?ItemID=" + ItemID;
                EditLink += "&mID=" + ModuleID + "\" class=\"Normal\">Edit</a>";
            }

            if (Page.IsPostBack == false)
            {
                //Chris Farrell, chris@cftechconsulting.com, 5/28/04.
                //Improper Identity seed in the ItemID means that there may be tasks
                //with a ItemID = 0.  This is not the way it should be, but there is no
                //reason to NOT show the task with ItemID = 0 and that helps reduce
                //the pains from this bug for users who already have data present.

                // Obtain a single row of Task information
                TasksDB Tasks = new TasksDB();
                SqlDataReader dr = Tasks.GetSingleTask(ItemID);

                try
                {
                    // Read first row from database
                    if (dr.Read())
                    {
                        TitleField.Text = (string) dr["Title"];
                        longdesc.Text = (string) dr["Description"];
                        StartField.Text = ((DateTime) dr["StartDate"]).ToShortDateString();
                        DueField.Text = ((DateTime) dr["DueDate"]).ToShortDateString();
                        CreatedBy.Text = (string) dr["CreatedByUser"];
                        ModifiedBy.Text = (string) dr["ModifiedByUser"];
                        PercentCompleteField.Text = ((Int32) dr["PercentComplete"]).ToString();
                        AssignedField.Text = (string) dr["AssignedTo"];
                        CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToString();
                        ModifiedDate.Text = ((DateTime) dr["ModifiedDate"]).ToString();
                        StatusField.Text =
                            General.GetString("TASK_STATE_" + (string) dr["Status"], (string) dr["Status"], StatusField);
                        PriorityField.Text =
                            General.GetString("TASK_PRIORITY_" + (string) dr["Priority"], (string) dr["Priority"],
                                              PriorityField);
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
                    }
                }
                finally
                {
                    dr.Close();
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
                var al = new List<string>();
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531012");
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531030"); // Access from portalSearch
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531052"); // Access from serviceItemList				
                return al;
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
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