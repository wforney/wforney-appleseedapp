using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// by Jose Viladiu
    /// Moved into Appleseed by Jakob Hansen
    /// </summary>
    [History("jminond", "2006/2/23", "Converted to partial class")]
    public partial class ComponentModule : PortalModuleControl
    {
        // protected PlaceHolder ComponentHolder;

        /// <summary>
        /// The Page_Load event handler on this User Control is
        /// used to load and execute a user control block.
        /// The user control to execute is stored in the HtmlText
        /// database table.  This method uses the Appleseed.HtmlTextDB()
        /// data component to encapsulate all data functionality.
        /// Is a simple variation from HtmlModule.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Obtain the selected item from the HtmlText table
            ComponentModuleDB comp = new ComponentModuleDB();
            SqlDataReader dr = comp.GetComponentModule(ModuleID);

            try
            {
                if (dr.Read())
                {
                    // Dynamically add the file content into the page
                    string content = (string) dr["Component"];
                    try
                    {
                        ComponentHolder.Controls.Add(ParseControl(content));
                    }
                    catch (Exception controlError)
                    {
                        ComponentHolder.Controls.Add
                            (new LiteralControl("<p>Error in control: " + controlError + "<p>" + content));
                    }
                }
            }
            finally
            {
                // Close the datareader
                dr.Close();
            }
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
                new SearchDefinition("rb_ComponentModule", "Title", "Component", "CreatedByUser", "CreatedDate",
                                     searchField);

            //Add here extra search fields, this way
            //s.ArrSearchFields.Add("itm.ExtraFieldToSearch");

            // Builds and returns the SELECT query
            return s.SearchSqlSelect(portalID, userID, searchString, false);
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2B113F51-FEA3-499A-98E7-7B83C192FDBC}"); }
        }

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

        #region Web Form Designer generated code

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);

            // Create a new Title the control
//			ModuleTitle = new DesktopModuleTitle();
            // Set here title properties
            // Add support for the edit page
            this.EditText = "EDIT";
            this.AddUrl = "~/DesktopModules/CommunityModules/ComponentModule/ComponentModuleEdit.aspx";
            // Add title ad the very beginning of 
            // the control's controls collection
//			Controls.AddAt(0, ModuleTitle);

            // Call base init procedure
            base.OnInit(e);
        }

        #endregion
    }
}