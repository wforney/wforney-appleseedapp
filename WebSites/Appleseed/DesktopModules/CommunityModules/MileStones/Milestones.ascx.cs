//Add Appleseed Namespaces
using System;
using System.Collections;
using System.IO;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    ///	Summary description for Milestones.
    ///	Notice we have changed base class from System.Web.UI.UserControl
    ///	to Appleseed.Framework.Web.UI.WebControls.PortalModuleControl
    /// </summary>
    /// Remove abstract, searchable classes cannot be abstract
    public partial class Milestones : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Added EsperantusKeys for Localization 
            // Mario Endara mario@softworks.com.uy 11/05/2004 

            foreach (DataGridColumn column in myDataGrid.Columns)
            {
                switch (myDataGrid.Columns.IndexOf(column))
                {
                    case 1:
                        column.HeaderText = General.GetString("MILESTONES_TITLE");
                        break;
                    case 2:
                        column.HeaderText = General.GetString("MILESTONES_COMPLETION_DATE");
                        break;
                    case 3:
                        column.HeaderText = General.GetString("MILESTONES_STATUS");
                        break;
                }
            }

            if (!Page.IsPostBack)
            {
                // Create an instance of MilestonesDB class
                MilestonesDB milestones = new MilestonesDB();

                // Get the Milstones data for the current module.
                // ModuleID is defined on base class and contains
                // a reference to the current module.
                myDataGrid.DataSource = milestones.GetMilestones(ModuleID, Version);

                // Bind the milestones data to the grid.
                myDataGrid.DataBind();
            }
        }

        /// <summary>
        /// Override base Guid implementation
        /// to provide an unique id for your control
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{B8784E32-688A-4b8a-87C4-DF108BF12DBE}"); }
        }

        /// <summary>
        /// If the module is searchable you
        /// must override the property to return true
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        # region Install / Uninstall Implementation

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);
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
            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        # endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Milestones"/> class.
        /// </summary>
        public Milestones()
        {
            // Change by David.Verberckmoes@syntegra.com
            // Date: 20030324
            SupportsWorkflow = true;
            // End Change David.Verberckmoes@syntegra.com
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
            // Parameters:
            // Table Name: the table that holds the data
            // Title field: the field that contains the title for result, must be a field in the table
            // Abstract field: the field that contains the text for result, must be a field in the table
            // Search field: pass the searchField parameter you recieve.

            SearchDefinition s =
                new SearchDefinition("rb_Milestones", "Title", "Status", "CreatedByUser", "CreatedDate", searchField);

            //Add here extra search fields, this way
            //s.ArrSearchFields.Add("itm.ExtraFieldToSearch");

            // Builds and returns the SELECT query
            return s.SearchSqlSelect(portalID, userID, searchString);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            this.AddUrl = "~/DesktopModules/CommunityModules/MileStones/MilestonesEdit.aspx";
            base.OnInit(e);
        }

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}