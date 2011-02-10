using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    public partial class SurveyOptionEdit : EditItemPage
    {
        protected ArrayList portalOption = new ArrayList();


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Set the ImageUrl for controls from current Theme
                upBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl;
                downBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl;
                deleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
            }
            //TBD: Create a sproc that gets these fields:
            //CreatedBy.Text = (string) dr["CreatedByUser"];
            //CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();

            // get { QuestionID from querystring
            if (! (Request.Params["QuestionID"] == null))
            {
                int QuestionID = 0;
                QuestionID = int.Parse(Request.Params["QuestionID"]);

                SurveyDB OptList = new SurveyDB();
                SqlDataReader OList = OptList.GetOptionList(QuestionID);

                try
                {
                    while (OList.Read())
                    {
                        OptionItem o = new OptionItem();
                        o.OptionName = OList["OptionDesc"].ToString();
                        o.OptionID = (int) OList["OptionID"];
                        o.OptionOrder = (int) OList["ViewOrder"];
                        portalOption.Add(o);
                    }
                }
                finally
                {
                    OList.Close(); //by Manu, fixed bug 807858
                }

                if (! Page.IsPostBack)
                {
                    OptionList.DataTextField = "OptionName";
                    OptionList.DataValueField = "OptionID";
                    OptionList.DataSource = portalOption;
                    OptionList.DataBind();
                    lblQuestion.Text = Request.Params["Question"];
                    if (Request.Params["TypeOption"] == "RD")
                        lblTypeOption.Text = General.GetString("SURVEY_RADIOBUTTONS", "Radio buttons", this);
                    else
                        lblTypeOption.Text = General.GetString("SURVEY_CHECKBOXES", "Checkboxes", this);
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
                al.Add("2502DB18-B580-4F90-8CB4-C15E6E531018");
                return al;
            }
        }

        //*******************************************************
        //
        // The UpDown_Click server event handler on this page is
        // used to move an option up o down
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the UpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void UpDown_Click(object sender, ImageClickEventArgs e)
        {
            string cmd = ((ImageButton) sender).CommandName;

            if (OptionList.SelectedIndex != -1)
            {
                int delta;

                // Determine the delta to apply in the order number for the module
                // within the list.  +3 moves down one item; -3 moves up one item
                if (cmd == "down")
                    delta = 3;
                else
                    delta = -3;

                OptionItem o;
                o = (OptionItem) portalOption[OptionList.SelectedIndex];
                o.OptionOrder += delta;

                // Reset the order numbers for the questions
                OrderOptions();

                // Redirect to the same page to pick up changes
                Response.Redirect(Request.RawUrl);
            }
        }


        //*******************************************************
        //
        // The OrderOptions helper method is used to reset the display
        // order for the options
        //
        //*******************************************************//
        /// <summary>
        /// Orders the options.
        /// </summary>
        private void OrderOptions()
        {
            int i = 1;

            // sort the arraylist
            portalOption.Sort();

            // renumber the order and save to database
            foreach (OptionItem o in portalOption)
            {
                // number the items 1, 3, 5, etc. to provide an empty order
                // number when moving items up and down in the list.
                o.OptionOrder = i;
                i += 2;

                // rewrite tab to database
                SurveyDB Order = new SurveyDB();
                Order.UpdateOptionOrder(o.OptionID, o.OptionOrder);
            }
        }


        //*******************************************************
        //
        // The AddOptBtn_Click server event handler is used to add
        // a new option.
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the AddOptBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddOptBtn_Click(object sender, EventArgs e)
        {
            lblNewOption.Visible = true;
            TxtNewOption.Visible = true;
            TxtNewOption.Text = General.GetString("SURVEY_NEWOPTION", "New option", this);
            ReqNewOpt.Visible = true;
            AddOptionBtn.Text = "<img src='" + CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl +
                                "' border=0>" + General.GetString("SURVEY_ADDOPTBELOW", "Add to 'Options' Below", this);
            AddOptionBtn.Visible = true;
            CancelOptBtn.Visible = true;
        }


        //private void AddOptionBtn_Click( System.object sender,  System.EventArgs e)  AddOptionBtn.Click {
        /// <summary>
        /// Handles the Click event of the AddOptionBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddOptionBtn_Click(object sender, EventArgs e)
        {
            // Determine QuestioID 
            int QuestionID = 0;

            // get { QuestionID from querystring
            if (! (Request.Params["QuestionID"] == null))
                QuestionID = int.Parse(Request.Params["QuestionID"]);


            // new option go to the end of the list
            OptionItem o = new OptionItem();
            o.OptionName = TxtNewOption.Text;
            o.OptionID = -1;
            o.OptionOrder = 999;
            portalOption.Add(o);

            // write tab to database
            SurveyDB NewOption = new SurveyDB();
            o.OptionID = NewOption.AddOption(QuestionID, o.OptionName, o.OptionOrder);

            // Reset the order numbers for the tabs within the list
            OrderOptions();

            // Redirect to edit page
            Response.Redirect(Request.RawUrl);
        }


        /// <summary>
        /// Handles the Click event of the CancelOptBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void CancelOptBtn_Click(object sender, EventArgs e)
        {
            TxtNewOption.Visible = false;
            ReqNewOpt.Visible = false;
            AddOptionBtn.Visible = false;
            CancelOptBtn.Visible = false;
        }


        /// <summary>
        /// Handles the Click event of the deleteBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void deleteBtn_Click(object sender, ImageClickEventArgs e)
        {
            OptionItem o;
            o = (OptionItem) portalOption[OptionList.SelectedIndex];

            SurveyDB DelOpt = new SurveyDB();
            DelOpt.DelOption(o.OptionID);

            // remove item from list
            portalOption.RemoveAt(OptionList.SelectedIndex);

            // reorder list
            OrderOptions();

            // Redirect to this site to refresh
            Response.Redirect(Request.RawUrl, false);
        }


        /// <summary>
        /// Handles the Click event of the btnReturnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void btnReturnCancel_Click(object sender, EventArgs e)
        {
            int tabID = int.Parse(Request.Params["tabID"].ToString());
            Response.Redirect("SurveyEdit.aspx?mID=" + ModuleID + "&tabID=" + tabID);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInitEvent
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            // jes1111
            if (!this.IsCssFileRegistered("Mod_Survey"))
                this.RegisterCssFile("Mod_Survey");

            base.OnInit(e);
        }

        #endregion
    }
}