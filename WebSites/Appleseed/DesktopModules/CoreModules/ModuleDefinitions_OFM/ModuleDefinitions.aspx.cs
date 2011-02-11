using System;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI;
using Label=Appleseed.Framework.Web.UI.WebControls.Label;

namespace Appleseed.AdminAll
{
    using System.Collections.Generic;

    /// <summary>
    /// Add/Remove modules, assign OneFileModules to portals
    /// </summary>
    public partial class ModuleDefinitions_OFM : EditItemPage
    {
        protected Label Label7;
        protected TextBox InstallerFileName;
        protected RequiredFieldValidator Requiredfieldvalidator1;

        private Guid defID;
        private bool addModule = false;

        /// <summary>
        /// The Page_Load server event handler on this page is used
        /// to populate the role information for the page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            //if (PortalSecurity.IsInRoles("Admins") == false) 
            //	PortalSecurity.AccessDeniedEdit();

            // Calculate security defID
            if (Request.Params["DefID"] != null)
                defID = new Guid(Request.Params["DefID"]);

            ModulesDB modules = new ModulesDB();

            // If this is the first visit to the page, bind the definition data 

            if (Page.IsPostBack)
            {
                addModule = bool.Parse(ViewState["addModule"].ToString());
            }
            else
            {
                if (Request.Params["defID"] == null)
                    addModule = true;
                ViewState["addModule"] = addModule;
                if (addModule)
                    deleteButton.Visible = false;

                if (addModule)
                {
                    defID = Guid.NewGuid();
                    // new module definition
                    FriendlyName.Text = "My New Module";
                    DesktopSrc.Text = "DesktopModules/MyNewModule.ascx";
                    MobileSrc.Text = "";
                    ModuleGuid.Text = defID.ToString();
                }
                else
                {
                    // Obtain the module definition to edit from the database
                    SqlDataReader dr = modules.GetSingleModuleDefinition(defID);

                    // Read in first row from database
                    while (dr.Read())
                    {
                        FriendlyName.Text = dr["FriendlyName"].ToString();
                        DesktopSrc.Text = dr["DesktopSrc"].ToString();
                        MobileSrc.Text = dr["MobileSrc"].ToString();
                        ModuleGuid.Text = dr["GeneralModDefID"].ToString();
                        ModuleGuid.Enabled = false;
                    }
                    dr.Close();
                }

                // Clear existing items in checkboxlist
                PortalsName.Items.Clear();

                // Populate checkbox list with all portals
                // and "check" the ones already configured for this tab
                SqlDataReader portals = modules.GetModuleInUse(defID);
                while (portals.Read())
                {
                    if (Convert.ToInt32(portals["PortalID"]) >= 0)
                    {
                        ListItem item = new ListItem();
                        item.Text = (string) portals["PortalName"];
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

        /// <summary>
        /// Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                List<string> al = new List<string>();
                al.Add("D04BB5EA-A792-4E87-BFC7-7D0ED3AD1234");
                return al;
            }
        }

        /// <summary>
        /// OnUpdate installs or refresh module definiton on db
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    ModulesDB modules = new ModulesDB();
                    modules.AddGeneralModuleDefinitions(new Guid(ModuleGuid.Text), FriendlyName.Text, DesktopSrc.Text,
                                                        MobileSrc.Text,
                                                        "Appleseed.Modules.OneFileModule.dll",
                                                        "Appleseed.Content.Web.ModulesOneFileModule", false, false);

                    // Update the module definition
                    for (int i = 0; i < PortalsName.Items.Count; i++)
                    {
                        modules.UpdateModuleDefinitions(defID, Convert.ToInt32(PortalsName.Items[i].Value),
                                                        PortalsName.Items[i].Selected);
                    }

                    // Redirect back to the portal admin page
                    RedirectBackToReferringPage();
                }
                catch (ThreadAbortException)
                {
                    //normal with redirect 
                }
                catch (Exception ex)
                {
                    lblErrorDetail.Text =
                        General.GetString("MODULE_DEFINITIONS_INSTALLING", "An error occurred installing.", this) +
                        "<br>";
                    lblErrorDetail.Text += ex.Message + "<br>";
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
            try
            {
                ModulesDB modules = new ModulesDB();
                modules.DeleteModuleDefinition(new Guid(ModuleGuid.Text));

                // Redirect back to the portal admin page
                RedirectBackToReferringPage();
            }
            catch (ThreadAbortException)
            {
                //normal with redirect 
            }
            catch (Exception ex)
            {
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
            for (int i = 0; i < PortalsName.Items.Count; i++)
            {
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
            for (int i = 0; i < PortalsName.Items.Count; i++)
            {
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
            this.updateButton.Click += new EventHandler(updateButton_Click);
            this.deleteButton.Click += new EventHandler(deleteButton_Click);
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            this.selectNoneButton.Click += new System.EventHandler(this.selectNoneButton_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Click event of the deleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void deleteButton_Click(object sender, EventArgs e)
        {
            OnDelete(e);
        }

        /// <summary>
        /// Handles the Click event of the updateButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void updateButton_Click(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        #endregion
    }
}