//Add Appleseed Namespaces
using System;
using System.Collections;
using System.Data.SqlClient;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// IBS Portal Milestone Module - Edit page part
    /// Writen by: Elaine Ossipov  - 9/11/2002 - admin@sbsc.net
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// Updated by Manu as Appleseed Tutorial
    /// </summary>
    public partial class MilestonesEdit : AddEditItemPage
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // If the page is being requested the first time, determine if a
            // Milestone itemID value is specified, and if so, 
            // populate the page contents with the Milestone details.
            if (Page.IsPostBack == false)
            {
                //Item id is defined in base class
                if (ItemID > 0)
                {
                    //Obtain a single row of Milestone information.
                    MilestonesDB milestonesDB = new MilestonesDB();
                    SqlDataReader dr = milestonesDB.GetSingleMilestones(ItemID, WorkFlowVersion.Staging);

                    try
                    {
                        //Load the first row into the DataReader
                        if (dr.Read())
                        {
                            TitleField.Text = (string) dr["Title"];
                            EstCompleteDate.Text = ((DateTime) dr["EstCompleteDate"]).ToShortDateString();
                            StatusBox.Text = (string) dr["Status"];
                            CreatedBy.Text = (string) dr["CreatedByUser"];
                            CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();
                            // 15/7/2004 added localization by Mario Endara mario@softworks.com.uy
                            if (CreatedBy.Text == "unknown" || CreatedBy.Text == string.Empty)
                            {
                                CreatedBy.Text = General.GetString("UNKNOWN", "unknown");
                            }
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
                else
                {
                    //Provide defaults
                    EstCompleteDate.Text = DateTime.Now.AddDays(60).ToShortDateString();
                }
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
                al.Add("B8784E32-688A-4b8a-87C4-DF108BF12DBE");
                return al;
            }
        }

        /// <summary>
        /// This procedure is automaticall
        /// called on Update
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnUpdate(EventArgs e)
        {
            // Calling base we check if the user has rights on updating
            base.OnUpdate(e);

            // Update onlyif the entered data is Valid
            if (Page.IsValid == true)
            {
                MilestonesDB milestonesDb = new MilestonesDB();
                if (ItemID <= 0)
                    milestonesDb.AddMilestones(ItemID, ModuleID, PortalSettings.CurrentUser.Identity.Email, DateTime.Now,
                                               TitleField.Text, DateTime.Parse(EstCompleteDate.Text), StatusBox.Text);
                else
                    milestonesDb.UpdateMilestones(ItemID, ModuleID, PortalSettings.CurrentUser.Identity.Email,
                                                  DateTime.Now, TitleField.Text, DateTime.Parse(EstCompleteDate.Text),
                                                  StatusBox.Text);

                // Redirects to the referring page
                // This method is provided by the base class
                RedirectBackToReferringPage();
            }
        }

        /// <summary>
        /// This procedure is automaticall
        /// called on Update
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnDelete(EventArgs e)
        {
            // Calling base we check if the user has rights on deleting
            base.OnUpdate(e);

            if (ItemID > 0)
            {
                MilestonesDB milestonesDb = new MilestonesDB();
                milestonesDb.DeleteMilestones(ItemID);
            }

            // This method is provided by the base class
            RedirectBackToReferringPage();
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}