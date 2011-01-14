using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Security;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;
using Localize=Appleseed.Framework.Web.UI.WebControls.Localize;
using Path=Appleseed.Framework.Settings.Path;
using Appleseed.Framework.Core.Model;
using System.Collections.Generic;

namespace Appleseed.Content.Web.Modules.AddModule
{
    /// <summary>
    /// This module has been built by John Mandia (www.whitelightsolutions.com)
    /// It allows administrators to give permission to selected roles to add modules to pages
    /// </summary>
    [History("jminond", "2006/03/25", "Converted to partial class")]
    [History("jminond", "2006/03/19", "Corrected adding module to root page for site")]
    [History("jminond", "2005/03/10", "Changes for moving Tab to Page")]
    public partial class Viewer : PortalModuleControl
    {
        /// <summary>
        /// Localized label for add module
        /// </summary>
        protected Localize addmodule;

        #region Page Load

        /// <summary>
        /// The Page_Load event handler on this User Control is used to
        /// load all the modules that are currently on this tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If first visit to the page, update all entries
            if (Page.IsPostBack == false)
            {
                BindData();
                SetHelpPath();
                SetModuleName();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The BindData helper method is used to update the tab's
        /// layout panes with the current configuration information
        /// </summary>
        private void BindData()
        {
            // Populate the "Add Module" Data
            ModulesDB m = new ModulesDB();

            SqlDataReader drCurrentModuleDefinitions = m.GetCurrentModuleDefinitions(portalSettings.PortalID);

            try
            {
//				if(this.ArePropertiesEditable)
//				{
//					while(drCurrentModuleDefinitions.Read())
//					{
//						moduleType.Items.Add(new ListItem(drCurrentModuleDefinitions["FriendlyName"].ToString(),drCurrentModuleDefinitions["ModuleDefID"].ToString() + "|" + GetHelpPath(drCurrentModuleDefinitions["DesktopSrc"].ToString())));
//					}
//				}
//				else
//				{
                while (drCurrentModuleDefinitions.Read())
                {
                    // Added by Mario Endara <mario@softworks.com.uy> 2004/11/04
                    // only users members of the "Amins" role can add Admin modules to a Tab
                    if (PortalSecurity.IsInRoles("Admins") == true ||
                        !(bool.Parse(drCurrentModuleDefinitions["Admin"].ToString())))
                    {
                        moduleType.Items.Add(
                            new ListItem(drCurrentModuleDefinitions["FriendlyName"].ToString(),
                                         drCurrentModuleDefinitions["ModuleDefID"].ToString() + "|" +
                                         GetHelpPath(drCurrentModuleDefinitions["DesktopSrc"].ToString())));


                        Dictionary<string,string> actions = ModelServices.GetMVCActionModules();
                        foreach (string key in actions.Keys) {
                            moduleType.Items.Add(new ListItem(key, actions[key]));
                        }
                    }
                }
//				}
            }
            finally
            {
                drCurrentModuleDefinitions.Close();
            }
        }


        /// <summary>
        /// Gets the folder help path.
        /// </summary>
        /// <param name="desktopSrc">Desktop SRC.</param>
        /// <returns>The name of the help folder in the correct format</returns>
        private string GetHelpPath(string desktopSrc)
        {
            string helpPath = desktopSrc.Replace(".", "_").ToString();
            return "Appleseed/" + helpPath;
        }

        private void SetModuleName()
        {
            //by Manu, set title like module name
            if (moduleType.SelectedItem != null)
                moduleTitle.Text = moduleType.SelectedItem.Text;
        }

        /// <summary>
        /// Sets the help path. This method checks to see whether the currently selected module has a
        /// help file associated with it. If it does then it shows the help icon. If it doesn't then it
        /// hides it.
        /// </summary>
        private void SetHelpPath()
        {
            SetDatata(moduleType.SelectedValue.ToString());
        }

        private void SetDatata(string modulePath)
        {
            string folderName = modulePath;
            int start = folderName.IndexOf("|");
            folderName = folderName.Substring(start + 1);
            int fileNameStart = folderName.LastIndexOf("/");
            string fileName = folderName.Substring(fileNameStart + 1);
            string completePath = folderName + "/" + fileName;


            if (
                File.Exists(
                    HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/rb_documentation/" + completePath +
                                                       ".xml")))
            {
                AddModuleHelp.Visible = true;
                string javaScript = "HelpWindow=window.open('"
                                    + Path.ApplicationRoot + "/rb_documentation/Viewer.aspx?loc=" + folderName + "&src=" +
                                    fileName
                                    +
                                    "','HelpWindow','toolbar=no,location=no,directories=no,status=no,menubar=yes,scrollbars=yes,resizable=yes,width=640,height=480,left=15,top=15'); return false;";
                AddModuleHelp.Attributes.Add("onclick", javaScript);
                AddModuleHelp.Attributes.Add("style", "cursor: hand;");
                AddModuleHelp.NavigateUrl = string.Empty;
                AddModuleHelp.ImageUrl = CurrentTheme.GetImage("Buttons_Help", "Help.gif").ImageUrl;
                AddModuleHelp.ToolTip = moduleType.SelectedItem.Text.ToString() + " Help";
            }
            else
            {
                AddModuleHelp.Visible = false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Each time the module selection is changed it checks to see if that particular module has a help file.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void moduleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHelpPath();
            SetModuleName();
        }

        /*[Ajax.AjaxMethod]*/
        /*
        public System.Collections.Specialized.StringCollection ModuleChangeStrings(string moduleType, string moduleName)
        {
            SetDatata(moduleType);
            moduleTitle.Text = moduleName;

            System.Collections.Specialized.StringCollection s = new System.Collections.Specialized.StringCollection();

            s.Add(moduleTitle.Text);

            if (AddModuleHelp.Visible)
            {
                s.Add(AddModuleHelp.Attributes["onclick"].ToString());
                s.Add(AddModuleHelp.NavigateUrl);
                s.Add(AddModuleHelp.ImageUrl);
                s.Add(AddModuleHelp.ToolTip);
            }

            return s;
        }
         * */

        /// <summary>
        /// The AddModule_Click server event handler 
        /// on this page is used to add a new portal module 
        /// into the tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddModule_Click(Object sender, EventArgs e)
        {
            // TODO: IF PAGE ID = 0 Then we know it's home page, cant we get from db the id?
            //PagesDB _d = new PagesDB();
            int pid = PageID;
            if (pid == 0)
                pid = PagesDB.PortalHomePageID(PortalID);

            if (pid != 0)
            {
                // All new modules go to the end of the contentpane
                string selectedModule = moduleType.SelectedItem.Value.ToString();
                int start = selectedModule.IndexOf("|");
                int moduleID = Convert.ToInt32(selectedModule.Substring(0, start).Trim());

                // Hide error message in case there was a previous error.
                moduleError.Visible = false;

                // This allows the user to pick what type of people can view the module being added.
                // If Authorised Roles is selected from the dropdown then every role that has view permission for the
                // Add Role module will be added to the view permissions of the module being added.
                string viewPermissionRoles = viewPermissions.SelectedValue.ToString();
                if (viewPermissionRoles == "Authorised Roles")
                {
                    viewPermissionRoles = PortalSecurity.GetViewPermissions(ModuleID);
                }

                try
                {
                    ModuleItem m = new ModuleItem();
                    m.Title = moduleTitle.Text;
                    m.ModuleDefID = moduleID;
                    m.Order = 999;

                    // save to database
                    ModulesDB _mod = new ModulesDB();
                    m.ID =
                        _mod.AddModule(pid, m.Order, paneLocation.SelectedValue.ToString(), m.Title, m.ModuleDefID, 0,
                                       PortalSecurity.GetEditPermissions(ModuleID), viewPermissionRoles,
                                       PortalSecurity.GetAddPermissions(ModuleID),
                                       PortalSecurity.GetDeletePermissions(ModuleID),
                                       PortalSecurity.GetPropertiesPermissions(ModuleID),
                                       PortalSecurity.GetMoveModulePermissions(ModuleID),
                                       PortalSecurity.GetDeleteModulePermissions(ModuleID), false,
                                       PortalSecurity.GetPublishPermissions(ModuleID), false, false, false);
                }
                catch (Exception ex)
                {
                    moduleError.Visible = true;
                    ErrorHandler.Publish(LogLevel.Error,
                                         "There was an error with the Add Module Module while trying to add a new module.",
                                         ex);
                }
                finally
                {
                    if (moduleError.Visible == false)
                    {
                        // Reload page to pick up changes
                        Response.Redirect(Request.RawUrl, false);
                    }
                }
            }
            else
            {
                //moduleError.TextKey = "ADDMODULE_HOMEPAGEERROR";
                moduleError.Text =
                    General.GetString("ADDMODULE_HOMEPAGEERROR",
                                      "You are currently on the homepage using the default virtual ID (The default ID is set when no specific page is selected. e.g. www.yourdomain.com. Please select your homepage from the Navigation menu e.g. 'Home' so that you can add a module against the page's actual ID.");
                moduleError.Visible = true;
            }
        }

        #endregion

        #region General Implementation

        /// <summary>
        /// Gets the GUID for this module.
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{350CED6F-6739-43f3-8BF1-1D95187CA0BF}"); }
        }

        /// <summary>
        /// Marks This Module To Be An Admin Module
        /// </summary>
        public override bool AdminModule
        {
            get { return true; }
        }

        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
        public Viewer()
        {
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();

            // Create a new Title the control
//			ModuleTitle = new DesktopModuleTitle();
            // Set here title properties
            // Add title ad the very beginning of
            // the control's controls collection
//			Controls.AddAt(0, ModuleTitle);

            // Call base init procedure
            base.OnInit(e);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.moduleType.SelectedIndexChanged += new EventHandler(this.moduleType_SelectedIndexChanged);
            this.AddModuleBtn.Click += new EventHandler(this.AddModule_Click);
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}