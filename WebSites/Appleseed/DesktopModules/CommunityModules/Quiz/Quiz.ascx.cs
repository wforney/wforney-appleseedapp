using System;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// Quiz Module
    /// </summary>
    public partial class Quiz : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            lnkQuiz.Text = Settings["QuizName"].ToString();
            lnkQuiz.NavigateUrl = HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Quiz/QuizPage.aspx", "mID=" + ModuleID);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quiz"/> class.
        /// </summary>
        public Quiz()
        {
            var quizName = new SettingItem<string, TextBox>
                {
                    Required = true, Order = 1, Value = "About Australia (Demo1)" 
                };
            this.BaseSettings.Add("QuizName", quizName);

            var xmlSrc = new SettingItem<string, TextBox>(new PortalUrlDataType())
                {
                    Required = true, Order = 2, Value = "/Quiz/Demo1.xml" 
                };
            this.BaseSettings.Add("XMLsrc", xmlSrc);
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531050}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}