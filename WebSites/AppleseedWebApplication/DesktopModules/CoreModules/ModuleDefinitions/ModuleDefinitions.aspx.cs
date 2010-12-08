using System;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Core.Model;
using Appleseed.Framework.Web.UI.WebControls;
using io=System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Appleseed.AdminAll
{
    /// <summary>
    /// Add/Remove modules, assign modules to portals
    /// </summary>
    public partial class ModuleDefinitions : EditItemPage
    {
        private Guid defID;

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to populate the role information for the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Verify that the current user has access to access this page
            // Removed by Mario Endara <mario@softworks.com.uy> (2004/11/04)
            //			if (PortalSecurity.IsInRoles("Admins") == false) 
            //				PortalSecurity.AccessDeniedEdit();

            // Calculate security defID
            if (Request.Params["defID"] != null)
                defID = new Guid(Request.Params["defID"]);

            ModulesDB modules = new ModulesDB();
           

            // If this is the first visit to the page, bind the definition data 
            if (!Page.IsPostBack) {
                if (defID.Equals(Guid.Empty)) {
                    ChangeInstallMode(EditMode.Installer);
                    // new module definition
                    InstallerFileName.Text = "DesktopModules/[ModuleFolder]/install.xml";
                    FriendlyName.Text = "";
                    DesktopSrc.Text = "";
                    MobileSrc.Text = "";
                } else {
                    
                    // Obtain the module definition to edit from the database
                    SqlDataReader dr = modules.GetSingleModuleDefinition(defID);

                    // Read in first row from database
                    string friendlyName = string.Empty;
                    string desktopSrc = string.Empty;
                    while (dr.Read())
                    {
                        friendlyName = dr["FriendlyName"].ToString();
                        FriendlyName.Text = friendlyName;
                        desktopSrc = dr["DesktopSrc"].ToString();
                        DesktopSrc.Text = desktopSrc;
                        MobileSrc.Text = dr["MobileSrc"].ToString();
                        lblGUID.Text = dr["GeneralModDefID"].ToString();
                    }
                    dr.Close(); //by Manu, fixed bug 807858

                    if (DesktopSrc.Text.Contains(".aspx") || DesktopSrc.Text.Contains(".ascx"))
                    {
                        ChangeInstallMode(EditMode.Manually);
                    }
                    else
                    {
                        this.FriendlyNameMVC.Text = friendlyName;

                        ChangeInstallMode(EditMode.MVC);
                        Dictionary<string, string> items = ModelServices.GetMVCActionModules();
                        foreach (ListItem item in GetPortableAreaModules())
                        {
                            items.Add(item.Text, item.Value);
                        }
                        this.ddlAction.DataSource = items;
                        this.ddlAction.DataBind();

                        string val = ddlAction.Items[0].Value;
                        foreach (ListItem item in ddlAction.Items)
                        {
                            if (item.Text.Contains(desktopSrc.Replace("/", "\\")))
                            {
                                val = item.Value;
                                break;
                            }
                        }

                        this.ddlAction.SelectedValue = val;
                    }
                }

                // Populate checkbox list with all portals
                // and "check" the ones already configured for this tab
                SqlDataReader portals = modules.GetModuleInUse(defID);

                // Clear existing items in checkboxlist
                PortalsName.Items.Clear();

                while (portals.Read()) {
                    if (Convert.ToInt32(portals["PortalID"]) >= 0) {
                        ListItem item = new ListItem();
                        item.Text = (string)portals["PortalName"];
                        item.Value = portals["PortalID"].ToString();

                        if ((portals["checked"].ToString()) == "1")
                            item.Selected = true;
                        else
                            item.Selected = false;

                        PortalsName.Items.Add(item);
                    }
                }
                portals.Close(); //by Manu, fixed bug 807858

              
            }
        }

        private ListItem[] GetPortableAreaModules()
        {
            List<ListItem> result = new List<ListItem>();
            foreach (io.FileInfo file in new io.DirectoryInfo(Server.MapPath("~/bin/")).GetFiles("*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                string areaName = assembly.GetName().Name;
                try
                {
                    var modules = from t in assembly.GetTypes()
                                  where t.IsClass && t.Namespace == areaName + ".Controllers"
                                  && t.GetMethods().FirstOrDefault(d => d.Name == "Module") != null
                                  select new
                                  {
                                      AssemblyFullName = assembly.FullName,
                                      AreaName = areaName,
                                      Module = t.Name
                                  };


                    foreach (var module in modules)
                    {
                        string moduleName = module.Module.Replace("Controller", "");
                        //ModelServices.RegisterPortableAreaModule(module.AreaName, module.AssemblyFullName, moduleName);
                        string itemValue = String.Format("Areas\\{0}\\Views\\{1}\\Module", module.AreaName, moduleName);
                        string itemText = String.Format("[PortableArea] Areas\\{0}\\Views\\{1}\\Module", module.AreaName, moduleName);

                        result.Add(new ListItem(itemText, itemValue));

                    }
                }
                catch (Exception exc) { ErrorHandler.Publish(LogLevel.Debug, exc); }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("D04BB5EA-A792-4E87-BFC7-7D0ED3ADD582");
                return al;
            }
        }

        /// <summary>
        /// OnUpdate installs or refresh module definiton on db
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            if (Page.IsValid) {
                try {

                    if (chbMVCAction.Visible || chbPortableAreas.Visible) {
                        //Es un módulo clásico

                        if (!btnUseInstaller.Visible) {
                            ModuleInstall.InstallGroup(Server.MapPath(Path.ApplicationRoot + "/" + InstallerFileName.Text),
                                                       lblGUID.Text == string.Empty);
                        } else {
                            ModuleInstall.Install(FriendlyName.Text, DesktopSrc.Text, MobileSrc.Text,
                                                  lblGUID.Text == string.Empty);
                        }
                       
                        // Update the module definition
                        

                    } else {
                        //Es una acción MVC
                        string path = this.ddlAction.SelectedValue;


                        path = path.Substring(path.IndexOf("Areas"));
                        path = path.Replace("\\", "/");
                        path = path.Replace(".aspx", string.Empty);
                        path = path.Replace(".ascx", string.Empty);

                        string name = this.FriendlyNameMVC.Text;

                        defID = Appleseed.Framework.Core.Model.ModelServices.AddMVCActionModule(name, path);

                    }
                    ModulesDB modules = new ModulesDB();

                    for (int i = 0; i < PortalsName.Items.Count; i++) {
                        modules.UpdateModuleDefinitions(defID, Convert.ToInt32(PortalsName.Items[i].Value),
                                                        PortalsName.Items[i].Selected);
                    }


                    // Redirect back to the portal admin page
                    RedirectBackToReferringPage();
                } catch (ThreadAbortException) {
                    //normal with redirect 
                } catch (Exception ex) {
                    lblErrorDetail.Text =
                        General.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) +
                        "<br>";
                    lblErrorDetail.Text += ex.Message + "<br>";
                    if (!btnUseInstaller.Visible)
                        lblErrorDetail.Text += " Installer: " +
                                               Server.MapPath(Path.ApplicationRoot + "/" + InstallerFileName.Text);
                    else
                        lblErrorDetail.Text += " Module: '" + FriendlyName.Text + "' - Source: '" + DesktopSrc.Text +
                                               "' - Mobile: '" + MobileSrc.Text + "'";
                    lblErrorDetail.Visible = true;

                    ErrorHandler.Publish(LogLevel.Error, lblErrorDetail.Text, ex);
                }
            }
        }


        /// <summary>
        /// Delete a Module definition
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            try {
                if (!btnUseInstaller.Visible)
                    ModuleInstall.UninstallGroup(Server.MapPath(Path.ApplicationRoot + "/" + InstallerFileName.Text));
                else
                    ModuleInstall.Uninstall(DesktopSrc.Text, MobileSrc.Text);

                // Redirect back to the portal admin page
                RedirectBackToReferringPage();
            } catch (ThreadAbortException) {
                //normal with redirect 
            } catch (Exception ex) {
                lblErrorDetail.Text =
                    General.GetString("MODULE_DEFINITIONS_DELETE_ERROR", "An error occurred deleting module.", this);
                lblErrorDetail.Visible = true;
                ErrorHandler.Publish(LogLevel.Error, lblErrorDetail.Text, ex);
            }
        }

        /// <summary>
        /// Handles the Click event of the selectAllButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PortalsName.Items.Count; i++) {
                PortalsName.Items[i].Selected = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the selectNoneButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void selectNoneButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PortalsName.Items.Count; i++) {
                PortalsName.Items[i].Selected = false;
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.btnUseInstaller.Click += new EventHandler(this.btnUseInstaller_Click);
            this.btnDescription.Click += new EventHandler(this.btnDescription_Click);
            this.chbMVCAction.Click += new EventHandler(chbMVCAction_Click);
            this.chbPortableAreas.Click += new EventHandler(chbPortableAreas_Click);
            this.selectAllButton.Click += new EventHandler(this.selectAllButton_Click);
            this.selectNoneButton.Click += new EventHandler(this.selectNoneButton_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }



        #endregion

        /// <summary>
        /// Handles the Click event of the btnUseInstaller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void btnUseInstaller_Click(object sender, EventArgs e)
        {
            ChangeInstallMode(EditMode.Installer);
        }


        void chbMVCAction_Click(object sender, EventArgs e)
        {
            ChangeInstallMode(EditMode.MVC);
        }

         void chbPortableAreas_Click(object sender, EventArgs e)
        {
            ChangeInstallMode(EditMode.PortableAreas);
        }

         protected void btnRegisterPortableAreas_Click(object sender, EventArgs e)
         {

             List<String> modulesRegistered = new List<string>();
             foreach (io.FileInfo file in new io.DirectoryInfo(Server.MapPath("~/bin/")).GetFiles("*.dll"))
             {
                 Assembly assembly = Assembly.LoadFile(file.FullName);
                 string areaName = assembly.GetName().Name;
                 try
                 {
                     var modules = from t in assembly.GetTypes()
                                   where t.IsClass && t.Namespace == areaName + ".Controllers"
                                   && t.GetMethods().FirstOrDefault(d => d.Name == "Module") != null
                                   select new
                                   {
                                       AssemblyFullName = assembly.FullName,
                                       AreaName = areaName,
                                       Module = t.Name
                                   };


                     foreach (var module in modules)
                     {
                         string moduleName = module.Module.Replace("Controller", "");
                         ModelServices.RegisterPortableAreaModule(module.AreaName, module.AssemblyFullName, moduleName);
                         modulesRegistered.Add(module.AreaName + " - " + moduleName);
                     }
                 }
                 catch (Exception exc) { ErrorHandler.Publish(LogLevel.Debug, exc); }
             }

             
             registeredAreas.DataSource = modulesRegistered;
             registeredAreas.DataBind();
         }

        public void ChangeInstallMode(EditMode mode)
        {
            portalsDiv.Visible = true;

            switch (mode) {
                case EditMode.Installer:
                    tableInstaller.Visible = true;
                    tableManual.Visible = false;
                    tableMVC.Visible = false;
                    tablePortableAreas.Visible = false;

                    btnUseInstaller.Visible = false;
                    btnDescription.Visible = true;
                    chbMVCAction.Visible = true;
                    deleteButton.Visible = true;
                    chbPortableAreas.Visible = true;

                    break;
                case EditMode.Manually:
                    tableInstaller.Visible = false;
                    tableManual.Visible = true;
                    tableMVC.Visible = false;
                    tablePortableAreas.Visible = false;

                    btnUseInstaller.Visible = true;
                    btnDescription.Visible = false;
                    chbMVCAction.Visible = true;
                    deleteButton.Visible = true;
                    chbPortableAreas.Visible = true;

                    break;
                case EditMode.MVC:
                    tableInstaller.Visible = false;
                    tableManual.Visible = false;
                    tableMVC.Visible = true;
                    tablePortableAreas.Visible = false;

                    btnUseInstaller.Visible = true;
                    btnDescription.Visible = true;
                    chbMVCAction.Visible = false;
                    deleteButton.Visible = false;
                    chbPortableAreas.Visible = true;

                    break;
                case EditMode.PortableAreas:
                    tableInstaller.Visible = false;
                    tableManual.Visible = false;
                    tableMVC.Visible = false;
                    tablePortableAreas.Visible = true;

                    btnUseInstaller.Visible = true;
                    btnDescription.Visible = true;
                    chbMVCAction.Visible = true;
                    chbPortableAreas.Visible = false;

                    deleteButton.Visible = false;
                    portalsDiv.Visible = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDescription control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void btnDescription_Click(object sender, EventArgs e)
        {
            ChangeInstallMode(EditMode.Manually);
        }
       
}

    public enum EditMode : int
    {
        Installer = 0,
        Manually,
        MVC,
        PortableAreas
    }
}
