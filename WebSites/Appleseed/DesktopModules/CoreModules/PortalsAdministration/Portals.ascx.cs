using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using Appleseed.Framework;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI.WebControls;
//using Abtour.PortalTemplate;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Module to manage portals (AdminAll)
    /// </summary>
    public partial class Portals : PortalModuleControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected ArrayList portals;

        /// <summary>
        /// Admin Module
        /// </summary>
        /// <value></value>
        public override bool AdminModule
        {
            get { return true; }
        }

        /// <summary>
        /// The Page_Load server event handler on this user control is used
        /// to populate the current portals list from the database
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            portals = new ArrayList();
            PortalsDB portalsDb = new PortalsDB();
            SqlDataReader dr = portalsDb.GetPortals();
            try
            {
                while (dr.Read())
                {
                    PortalItem p = new PortalItem();
                    p.Name = dr["PortalName"].ToString();
                    p.Path = dr["PortalPath"].ToString();
                    p.ID = Convert.ToInt32(dr["PortalID"].ToString());
                    portals.Add(p);
                }
            }
            finally
            {
                dr.Close(); //by Manu, fixed bug 807858
            }

            // If this is the first visit to the page, bind the tab data to the page listbox
            if (Page.IsPostBack == false)
            {
                portalList.DataBind();
            }
            EditBtn.ImageUrl = this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
            DeleteBtn.ImageUrl = this.CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
            DeleteBtn.Attributes.Add("onclick", "return confirmDelete();");
        }

        

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{366C247D-4CFB-451D-A7AE-649C83B05841}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
            // Add a link for the edit page
            this.AddText = "ADD_PORTAL";
            this.AddUrl = "~/DesktopModules/CoreModules/PortalsAdministration/AddNewPortal.aspx";
        }

        #endregion

        /// <summary>
        /// OnDelete
        /// </summary>
        protected override void OnDelete()
        {
            if (portalList.SelectedIndex != -1)
            {
                try
                {
                    // must delete from database too
                    PortalItem p = (PortalItem) portals[portalList.SelectedIndex];
                    PortalsDB portalsdb = new PortalsDB();
                    //Response.Write("Will delete " + p.Name);
                    portalsdb.DeletePortal(p.ID);

                    // remove item from list
                    portals.RemoveAt(portalList.SelectedIndex);
                    // rebind list
                    portalList.DataBind();
                }
                catch (SqlException sqlex)
                {
                    string aux =
                        General.GetString("DELETE_PORTAL_ERROR", "There was an error on deleting the portal", this);
                    Appleseed.Framework.ErrorHandler.Publish(Appleseed.Framework.LogLevel.Error, aux, sqlex);
                    Controls.Add(new LiteralControl("<br><span class=NormalRed>" + aux + "<br>"));
                }
            }
            base.OnDelete();
        }

        /// <summary>
        /// OnEdit
        /// </summary>
        protected override void OnEdit()
        {
            if (portalList.SelectedIndex != -1)
            {
                // must delete from database too
                PortalItem p = (PortalItem) portals[portalList.SelectedIndex];

                //Add new portal
                // added mID by Mario Endara <mario@softworks.com.uy> to support security check (2004/11/09)
                Response.Redirect(HttpUrlBuilder.BuildUrl("~/DesktopModules/CoreModules/PortalsAdministration/EditPortal.aspx", 0,
                                                          "PortalID=" + p.ID + "&mID=" + ModuleID.ToString()));
            }
            base.OnEdit();
        }
        protected void EditBtn_Click(object sender, ImageClickEventArgs e)
        {
            OnEdit();
        }
        protected void DeleteBtn_Click(object sender, ImageClickEventArgs e)
        {
            OnDelete();
        }

        private string GetPhysicalPackageTemplatesPath()
        {
            string path = Appleseed.Framework.Settings.Path.ApplicationPhysicalPath;
            path = string.Format(@"{0}{1}\PortalTemplates", path, this.PortalSettings.PortalFullPath.Substring(1));
            path = path.Replace("/", @"\");
            return path;
        }


        protected void SerializeBtn_Click(object sender, EventArgs e)
        {
            //if (portalList.SelectedIndex != -1) {
            //    IPortalTemplateServices services = PortalTemplateFactory.GetPortalTemplateServices(new PortalTemplateRepository());
            //    PortalItem p = (PortalItem)portals[portalList.SelectedIndex];
            //    bool ok = services.SerializePortal(p.ID, GetPhysicalPackageTemplatesPath() + "\\");
            //    if (!ok) {
            //        ErrorMessage.Visible = true;
            //        ErrorMessage.Text = "There was an error on serialize the portal <br>";
            //    } else {
            //        ErrorMessage.Visible = false;
            //    }
            //} else {
            //    ErrorMessage.Visible = true;
            //    ErrorMessage.Text = "You must select a portal <br>";
            //}
        }
    }
}
