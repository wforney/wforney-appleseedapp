using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.UI;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using ImageButton=System.Web.UI.WebControls.ImageButton;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Portal Survey module - Edit page part
    /// Written by: www.sysdatanet.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class SurveyEdit : EditItemPage
    {
        #region Controls

        protected LinkButton btnRtnSurvey;

        #endregion

        protected ArrayList portalQuestion = new ArrayList();


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
                editBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl;
                deleteBtn.ImageUrl = CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl;
            }

            //*********************************************************
            // Checks whether the survey exist, if not it creates one
            //*********************************************************
            SurveyDB SurveyCheck = new SurveyDB();
            // puts the desc of Survey in the title
            lblDescSurvey.Text = SurveyCheck.ExistAddSurvey(ModuleID, PortalSettings.CurrentUser.Identity.Email);

            //TBD: Create a sproc that gets these fields:
            //CreatedBy.Text = (string) dr["CreatedByUser"];
            //CreatedDate.Text = ((DateTime) dr["CreatedDate"]).ToShortDateString();


            // Fill the Question Listbox
            SurveyDB QuestionList = new SurveyDB();
            SqlDataReader QList = QuestionList.GetQuestionList(ModuleID);

            try
            {
                while (QList.Read())
                {
                    QuestionItem t = new QuestionItem();
                    t.QuestionName = QList["Question"].ToString();
                    t.QuestionID = (int) QList["QuestionID"];
                    t.QuestionOrder = (int) QList["ViewOrder"];
                    t.TypeOption = QList["TypeOption"].ToString();
                    portalQuestion.Add(t);
                }
            }
            finally
            {
                QList.Close(); //by Manu, fixed bug 807858
            }

            // if ( this is the first visit to the page, bind the tab data to the page listbox
            if (Page.IsPostBack == false)
            {
                this.QuestionList.DataTextField = "QuestionName";
                this.QuestionList.DataValueField = "QuestionID";
                this.QuestionList.DataSource = portalQuestion;
                this.QuestionList.DataBind();
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
                var al = new List<string> { "2502DB18-B580-4F90-8CB4-C15E6E531018" };
                return al;
            }
        }


        //*******************************************************
        //
        // The UpDown_Click server event handler on this page is
        // used to move a portal module up or down on a question//s layout pane
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the UpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void UpDown_Click(object sender, ImageClickEventArgs e)
        {
            string cmd = ((ImageButton) (sender)).CommandName;

            if (QuestionList.SelectedIndex != -1)
            {
                int delta;

                // Determine the delta to apply in the order number for the module
                // within the list.  +3 moves down one item; -3 moves up one item
                if (cmd == "down")
                    delta = 3;
                else
                    delta = -3;

                QuestionItem t;
                t = (QuestionItem) portalQuestion[QuestionList.SelectedIndex];
                t.QuestionOrder += delta;

                // Reset the order numbers for the questions
                OrderQuestions();

                // Redirect to the same page to pick up changes
                Response.Redirect(Request.RawUrl);
            }
        }


        //*******************************************************
        //
        // The EditBtn_Click server event handler is used to edit
        // the selected question
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the EditBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void EditBtn_Click(object sender, ImageClickEventArgs e)
        {
            // Redirect to edit page of currently selected tab
            if (QuestionList.SelectedIndex != -1)
            {
                // Determine the QuestionID
                QuestionItem t = (QuestionItem) portalQuestion[QuestionList.SelectedIndex];
                int tabID = int.Parse(Request.Params["tabID"]);
                Response.Redirect("SurveyOptionEdit.aspx?mID=" + ModuleID + "&QuestionID=" + t.QuestionID + "&Question=" +
                                  t.QuestionName + "&TypeOption=" + t.TypeOption + "&tabID=" + tabID);
            }
        }


        //*******************************************************
        //
        // The AddBtn_Click server event handler is used to add
        // a new question.
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the AddBtn control.
        /// </summary>
        /// <param name="Sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddBtn_Click(object Sender, EventArgs e)
        {
            lblNewQuestion.Visible = true;
            txtNewQuestion.Visible = true;
            txtNewQuestion.Text = General.GetString("SURVEY_NEWQUESTION", "New question", this);
            RdBtnCheck.Text = General.GetString("SURVEY_CHECKBOXES", "Checkboxes", this);
            RdBtnCheck.Visible = true;
            RdBtnRadio.Text = General.GetString("SURVEY_RADIOBUTTONS", "Radio buttons", this);
            RdBtnRadio.Visible = true;
            AddQuestionBtn.Text = "<img src='" + CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl +
                                  "' border=0>" + General.GetString("SURVEY_ADDBELOW", "Add to 'Questions' Below", this);
            AddQuestionBtn.Visible = true;
            ReqQuestion.Visible = true;
            lblOptionType.Visible = true;
            btnCancel.Visible = true;
        }


        //*******************************************************
        //
        // The OrderQuestions helper method is used to reset the display
        // order for the questions
        //
        //*******************************************************//
        /// <summary>
        /// Orders the questions.
        /// </summary>
        private void OrderQuestions()
        {
            int i = 1;

            // sort the arraylist
            portalQuestion.Sort();

            // renumber the order and save to database
            foreach (QuestionItem t in portalQuestion)
            {
                // number the items 1, 3, 5, etc. to provide an empty order
                // number when moving items up and down in the list.
                t.QuestionOrder = i;
                i += 2;

                // rewrite tab to database
                SurveyDB Order = new SurveyDB();
                Order.UpdateQuestionOrder(t.QuestionID, t.QuestionOrder);
            } // t
        }


        //*******************************************************
        //
        // The DeleteBtn_Click server event handler is used to delete
        // the selected question
        //
        //*******************************************************
        /// <summary>
        /// Handles the Click event of the DeleteBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void DeleteBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (QuestionList.SelectedIndex != -1)
            {
                // must delete from database too
                QuestionItem t = (QuestionItem) portalQuestion[QuestionList.SelectedIndex];
                SurveyDB DelQuestion = new SurveyDB();
                DelQuestion.DelQuestion(t.QuestionID);

                // remove item from list
                portalQuestion.RemoveAt(QuestionList.SelectedIndex);

                // reorder list
                OrderQuestions();

                // Redirect to this site to refresh
                Response.Redirect(Request.RawUrl);
            }
        }


        /// <summary>
        /// Handles the Click event of the AddQuestionBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddQuestionBtn_Click(object sender, EventArgs e)
        {
            // Determine the TypeOption
            string TypeOption;
            if (RdBtnRadio.Checked)
                TypeOption = "RD";
            else
                TypeOption = "CH";

            // new question go to the end of the list
            QuestionItem t = new QuestionItem();
            t.QuestionName = txtNewQuestion.Text;
            t.QuestionID = -1;
            t.QuestionOrder = 999;
            portalQuestion.Add(t);

            // write tab to database
            SurveyDB NewQuestion = new SurveyDB();
            t.QuestionID = NewQuestion.AddQuestion(ModuleID, t.QuestionName, t.QuestionOrder, TypeOption);

            // Reset the order numbers for the tabs within the list
            OrderQuestions();

            // Redirect to edit page
            Response.Redirect(Request.RawUrl);
        }


        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            lblNewQuestion.Visible = false;
            txtNewQuestion.Visible = false;
            txtNewQuestion.Text = "new Question";
            RdBtnCheck.Visible = false;
            RdBtnRadio.Visible = false;
            AddQuestionBtn.Visible = false;
            ReqQuestion.Visible = false;
            lblOptionType.Visible = false;
            btnCancel.Visible = false;
        }


        /// <summary>
        /// Handles the Click event of the btnBackSurvey control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void btnBackSurvey_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/" + Portal.PageID.ToString() + ".aspx");
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