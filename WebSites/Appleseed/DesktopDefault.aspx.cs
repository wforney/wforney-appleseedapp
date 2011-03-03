using System;
using System.IO;
using Appleseed.Framework;
using Appleseed.Framework.Design;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using History = Appleseed.Framework.History;
using System.Web;
using Appleseed.Framework.Core.Model;
using System.Collections.Generic;
using System.Collections;
using WebUI = System.Web.UI;
using WebUIControls = System.Web.UI.WebControls;
using Html = System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Web.Security;
using Appleseed.Framework.Providers.AppleseedMembershipProvider;
using System.Security.Cryptography;
using System.Text;
using Appleseed.Framework.Providers.AppleseedRoleProvider;
using System.Linq;

namespace Appleseed
{
 
	/// <summary> 
    /// The DesktopDefault.aspx page is used 
    /// to load and populate each Portal View.
    /// It accomplishes this by reading the layout configuration 
    /// of the portal from the Portal Configuration	system, 
    /// and then using this information to dynamically 
    /// instantiate portal modules (each implemented 
    /// as an ASP.NET User Control), and then inject them into the page.
    /// </summary>
    public partial class DesktopDefault : Page
    {
        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //this.Load += new EventHandler(this.DesktopDefault_Load);
            base.OnInit(e);
            this.DesktopDefault_Load(this, null);
        }

        #endregion

        /// <summary>
        /// Handles the Load event of the DesktopDefault control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DesktopDefault_Load(object sender, EventArgs e)
        {
            if (!Request.Url.PathAndQuery.Contains("site"))
            {
                int pageId = portalSettings.ActivePage.PageID;
                if (pageId == 0)
                {
                    pageId = Convert.ToInt32(SiteMap.RootNode.ChildNodes[0].Key);
                }
                Response.Redirect(HttpUrlBuilder.BuildUrl(pageId));
            }


            if (!PortalSecurity.IsInRoles(portalSettings.ActivePage.AuthorizedRoles) && !User.IsInRole("Admins"))
            {
               
                 
                PortalSecurity.AccessDenied();
            } else {
                if (Request.Params["r"] == null || Request.Params["r"].ToString() != "0") {
                    MembershipUser user = Membership.GetUser();
                }
                string userName = Request.Params["u"];
                string pass = Request.Params["p"];
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pass)) {
                    //PortalSecurity.SignOn(userName, pass, false, "~/DesktopDefault.aspx");
					bool rem = (Request.Params["rem"] ?? "0").ToString().Equals("1") ? true : false;
                    PortalSecurity.SignOn(userName, pass, rem, "~/DesktopDefault.aspx");                    
					Response.Redirect("~/DesktopDefault.aspx");
                }
                LoadPage();
            }
        }


        //private bool isMasterPageLayout = false;
        const string LAYOUT_BASE_PAGE = "DesktopDefault.ascx";

        protected override void OnPreInit(EventArgs e)
        {
            MASTERPAGE_BASE_PAGE = "PanesMaster.master";
            base.OnPreInit(e);
        }


        /// <summary>
        /// Loads the page.
        /// </summary>
        private void LoadPage()
        {
         

            if (IsMasterPageLayout) {

                //Obtain page modules by placeholder
                Dictionary<string, ArrayList> pageModulesByPlaceHolder = ModelServices.GetCurrentPageModules();

                //Obtain top masterpage
                WebUI.MasterPage mp = GetTopMasterPage();

                List<WebUI.Control> controllist = new List<WebUI.Control>();

                //Obtain top masterpage placeholders
                foreach (WebUI.Control control in mp.Controls){
                    if (control is System.Web.UI.HtmlControls.HtmlForm)
                    {
                        foreach (WebUI.Control control2 in control.Controls)
                            if (control2 is WebUIControls.ContentPlaceHolder)
                                controllist.Add(control2);
                    }
                }

                //foreach top placeholder
                foreach (WebUIControls.ContentPlaceHolder placeHolder in controllist)
                {

                    var insidePlaceHolders = AllPlaceHoldersInControl(placeHolder);

                    //foreach pane placeholder in the page
                    foreach (KeyValuePair<string, ArrayList> pageModuleInPlaceHolder in pageModulesByPlaceHolder)
                    {
                        //find out if current top placeholder contains current pane
                        System.Web.UI.Control container = placeHolder.FindControl(pageModuleInPlaceHolder.Key);
                        if (container != null)
                        {
                            //wrap current pane modules them inside custom span (to dragndrop)
                            container.Controls.Clear();
                            Html.HtmlGenericControl span = new Html.HtmlGenericControl("div");
                            span.Attributes.Add("id", pageModuleInPlaceHolder.Key);
                            span.Attributes.Add("class", "draggable-container");
                            foreach (WebUI.Control control in pageModuleInPlaceHolder.Value)
                            {
                                span.Controls.Add(control);
                            }
                            container.Controls.Add(span);

                            insidePlaceHolders.RemoveAll(d => d.ID.ToLower() == pageModuleInPlaceHolder.Key.ToLower());
                        }
                    }

                    foreach (var v in insidePlaceHolders)
                    {
                        var container = placeHolder.FindControl(v.ID);
                        container.Controls.Clear();
                        Html.HtmlGenericControl span = new Html.HtmlGenericControl("div");
                        span.Attributes.Add("id", v.ID);
                        span.Attributes.Add("class", "draggable-container");
                        span.Style["display"] = "none";
                        container.Controls.Add(span);
                    }

                    //then, hide empty top placeholders
                    HideNotFilled(placeHolder, pageModulesByPlaceHolder, insidePlaceHolders);
                }
            } else {

                string defaultLayoutPath = string.Concat(LayoutManager.WebPath, "/Default/", LAYOUT_BASE_PAGE);

                try {
                    string layoutPath = string.Concat(portalSettings.PortalLayoutPath, LAYOUT_BASE_PAGE);
                    System.Web.UI.Control layoutControl = Page.LoadControl(layoutPath);
                    if (layoutControl != null) {
                        LayoutPlaceHolder.Controls.Add(layoutControl);
                    } else {
                        throw new FileNotFoundException(string.Format("While loading {1} layoutControl is null, control not found in path {0}!!", layoutPath, Request.RawUrl));
                    }
                } catch (System.Web.HttpException ex) {
                    Appleseed.Framework.ErrorHandler.Publish(Appleseed.Framework.LogLevel.Error, "FileOrDirectoryNotFound", ex);
                    LayoutPlaceHolder.Controls.Add(Page.LoadControl(defaultLayoutPath));
                } catch (System.IO.DirectoryNotFoundException ex) {
                    Appleseed.Framework.ErrorHandler.Publish(Appleseed.Framework.LogLevel.Error, "DirectoryNotFound", ex);
                    LayoutPlaceHolder.Controls.Add(Page.LoadControl(defaultLayoutPath));
                } catch (FileNotFoundException ex) {
                    Appleseed.Framework.ErrorHandler.Publish(Appleseed.Framework.LogLevel.Error, "FileNotFound", ex);
                    LayoutPlaceHolder.Controls.Add(Page.LoadControl(defaultLayoutPath));
                }
            }
        }

        private List<WebUIControls.ContentPlaceHolder> AllPlaceHoldersInControl(WebUI.Control placeHolder)
        {
            List<WebUIControls.ContentPlaceHolder> result = new List<WebUIControls.ContentPlaceHolder>();
            foreach (WebUI.Control control in placeHolder.Controls)
            {
                if (control is WebUIControls.ContentPlaceHolder)
                    result.Add(control as WebUIControls.ContentPlaceHolder);
                else
                    result.AddRange(AllPlaceHoldersInControl(control));
            }


            return result;
        }



        private void HideNotFilled(WebUIControls.ContentPlaceHolder topPlaceHolder, Dictionary<string, ArrayList> pageModulesByPlaceHolder, List<WebUIControls.ContentPlaceHolder> emptyHolders)
        {
            foreach (WebUI.Control control in topPlaceHolder.Controls)
            {
                if (control is Html.HtmlControl)
                {
                    Html.HtmlControl htmlControl = (Html.HtmlControl)control;
                    if (htmlControl.Attributes["hideWhenEmpty"] != null)
                    {
                        //htmlControl.Visible = !(Convert.ToBoolean(htmlControl.Attributes["hideWhenEmpty"])
                        //                        && !HasChilds(htmlControl, pageModulesByPlaceHolder));
                        if (Convert.ToBoolean(htmlControl.Attributes["hideWhenEmpty"])
                                                && !HasChilds(htmlControl, pageModulesByPlaceHolder, emptyHolders))
                        {
                            htmlControl.Style["display"] = "none";
                        }
                    }
                }
            }
        }


        private bool HasChilds(Html.HtmlControl htmlControl, Dictionary<string, ArrayList> pageModules, List<WebUIControls.ContentPlaceHolder> emptyHolders)
        {
            bool res = false;
            foreach (WebUI.Control control in htmlControl.Controls) {
                if (control is WebUIControls.ContentPlaceHolder) {
                    if (pageModules.ContainsKey(control.ID.ToLower()) 
                        /*|| emptyHolders.Exists(d => d.ID == control.ID)*/)
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        private WebUI.MasterPage GetTopMasterPage()
        {
            WebUI.MasterPage mp = this.Master;
            while (mp.Master != null) {
                mp = mp.Master;
            }

            return mp;
        }

    }
}