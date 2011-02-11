using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Content.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// DiscussionViewThread
    /// </summary>
    /// <remarks>
    /// known bugs
    /// 1) if you delete the entire thread while viewing in this window, 
    /// it should automatically take you back to the Dicussion.ascx view
    /// </remarks>
    public partial class DiscussionViewThread : EditItemPage
    {
        /// <summary>
        /// On the first invocation of Page_Load, the data is bound using BindList();
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                BindList(GetItemID());
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
                al.Add("2D86166C-4BDC-4A6F-A028-D17C2BB177C8");
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531030"); // Access from portalSearch
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531052"); // Access from serviceItemList				
                return al;
            }
        }

        /// <summary>
        /// extracts this threads ItemID from the URL
        /// </summary>
        /// <returns></returns>
        private int GetItemID()
        {
            if (HttpContext.Current != null && Request.Params["ItemID"] != null)
            {
                return Int32.Parse(Request.Params["ItemID"]);
            }
            else
            {
                /* throw up an error dialog here */
                Response.Write("Error, invalid <ItemID> given to DiscussionViewThread");
                return 0;
            }
        }


        /// <summary>
        /// Binds the threads from the database to the list control
        /// </summary>
        /// <param name="ItemID">itemID of ANY of the topics in the thread</param>
        private void BindList(int ItemID)
        {
            // Obtain a list of discussion messages for the module and bind to datalist
            DiscussionDB discuss = new DiscussionDB();
            ThreadList.DataSource = discuss.GetThreadMessages(ItemID, 'Y'); // 'Y' means include rootmessage
            ThreadList.DataBind();
        }

        /// <summary>
        /// ThreadList_Select processes user events to add, edit, and delete topics
        /// </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="e">DataListCommandEventAargs e</param>
        public void ThreadList_Select(object Sender, DataListCommandEventArgs e)
        {
            // Determine the command of the button
            string command = ((CommandEventArgs) (e)).CommandName;

            switch (command)
            {
                case "delete":
                    DiscussionDB discuss = new DiscussionDB();
                    int ItemID = Int32.Parse(e.CommandArgument.ToString());
                    discuss.DeleteChildren(ItemID);
                    break;
                case "return_to_discussion_list":
                    RedirectBackToReferringPage();
                    break;
                default:
                    break;
            }
            BindList(GetItemID());
            return;
        }

        /// <summary>
        /// Invoked when each data row is bound to the list.
        /// Adds a clientside Java script to confirm row deltes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">DataListCommandEventAargs e</param>
        protected void OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.FindControl("deleteBtn") != null)
            {
                // 13/7/2004 Added Localization Mario Endara mario@softworks.com.uy
                ((ImageButton) e.Item.FindControl("deleteBtn")).Attributes.Add("onClick", "return confirm('" +
                                                                                          General.GetString(
                                                                                              "DISCUSSION_DELETE_RESPONSE",
                                                                                              "Are you sure you want to delete the selected response message and ALL of its children ?") +
                                                                                          "');");
            }
            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
            if (e.Item.FindControl("Label10") != null)
            {
                if (((Label) e.Item.FindControl("Label10")).Text == "unknown")
                {
                    ((Label) e.Item.FindControl("Label10")).Text = General.GetString("UNKNOWN", "unknown");
                }
            }
        }

        /// <summary>
        /// GetReplyImage check to see whether the current user has permissions to contribute to the discussion thread
        /// Users with proper permission see an image they can click  on to post a reply, otherwise they see nothing.
        /// </summary>
        /// <returns>
        /// Returns either a 1x1 image or the reply.gif icon
        /// </returns>
        protected string GetReplyImage()
        {
            // leave next commented statement in for testing back doors 
            // return "~/images/reply.gif";
            if (DiscussionPermissions.HasAddPermissions(ModuleID) == true)
                return getLocalImage("reply.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// Gets the edit image.
        /// </summary>
        /// <param name="itemUserEmail">The item user email.</param>
        /// <returns></returns>
        protected string GetEditImage(string itemUserEmail)
        {
            if (DiscussionPermissions.HasEditPermissions(ModuleID, itemUserEmail))
                return getLocalImage("edit.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// Gets the delete image.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <param name="itemUserEmail">The item user email.</param>
        /// <returns></returns>
        protected string GetDeleteImage(int itemID, string itemUserEmail)
        {
            if (DiscussionPermissions.HasDeletePermissions(ModuleID, itemID, itemUserEmail) == true)
                return getLocalImage("delete.png");
            else
                return getLocalImage("1x1.gif");
        }

        /// <summary>
        /// The FormatUrl method is a helper messages called by a
        /// databinding statement within the &lt;asp:DataList&gt; server
        /// control template.  It is defined as a helper method here
        /// (as opposed to inline within the template) to improve
        /// code organization and avoid embedding logic within the
        /// content template.
        /// </summary>
        /// <param name="item">ID of the currently selected topic</param>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// Returns a properly formatted URL to call the DiscussionEdit page
        /// </returns>
        protected string FormatUrlEditItem(int item, string mode)
        {
            return
                (HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Discussion/DiscussionEdit.aspx",
                                         "ItemID=" + item + "&Mode=" + mode + "&mID=" + ModuleID + "&edit=1"));
        }

        /// <summary>
        /// The NodeImage method is a helper method called by a
        /// databinding statement within the &lt;asp:datalist&gt; server
        /// control template.  It controls whether or not an item
        /// in the list should be rendered as an expandable topic
        /// or just as a single node within the list.
        /// </summary>
        /// <param name="count">Number of replys to the selected topic</param>
        /// <returns></returns>
        protected string NodeImage(int count)
        {
            return getLocalImage("plus.gif");
        }

        /// <summary>
        /// Gets the local image.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        protected string getLocalImage(string img)
        {
            return CurrentTheme.GetModuleImageSRC(img);
        }


        /// <summary>
        /// Load settings
        /// </summary>
        protected override void LoadSettings()
        {
            // Verify that the current user has proper permissions for this module

            // need to reanable this code as second level check in case users hack URLs to come to this page
            // if (PortalSecurity.HasEditPermissions(ModuleID) == false && PortalSecurity.IsInRoles("Admins") == false)
            //	PortalSecurity.AccessDeniedEdit();

            // base.LoadSettings();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            if (!((Page) this.Page).IsCssFileRegistered("Mod_Discussion"))
                ((Page) this.Page).RegisterCssFile("Mod_Discussion");

            this.ThreadList.EnableViewState = false;
            base.OnInit(e);
        }

        #endregion
    }
}