using System;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
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
        /// Contstructor
        /// </summary>
        public Quiz()
        {
            SettingItem QuizName = new SettingItem(new StringDataType());
            QuizName.Required = true;
            QuizName.Order = 1;
            QuizName.Value = "About Australia (Demo1)";
            _baseSettings.Add("QuizName", QuizName);

            SettingItem XMLsrc = new SettingItem(new PortalUrlDataType());
            XMLsrc.Required = true;
            XMLsrc.Order = 2;
            XMLsrc.Value = "/Quiz/Demo1.xml";
            _baseSettings.Add("XMLsrc", XMLsrc);
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