using System;
using System.Web.Mail;
using Appleseed.Framework;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Summary description for ApproveModuleContent.
    /// </summary>
    [History("Jes1111", "2003/03/04", "Added OnUpdate call to base page to handle cache flushing")]
    [
        History("Geert.Audenaert@Syntegra.Com", "2003/03/10",
            "Commented call from Jes, because it caused an error, and it wasn't necessary too")]
    [History("Geert.Audenaert@Syntegra.Com", "2003/03/11", "Added default destinators and text in the email form")]
    public partial class ApproveModuleContent : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is authorized
            if (!(PortalSecurity.HasApprovePermissions(ModuleID)))
                PortalSecurity.AccessDeniedEdit();

            // Fill email form with default 
            if (!IsPostBack)
            {
                // Destinators
                ModuleSettings ms = null;
                for (int i = 0; i < this.PortalSettings.ActivePage.Modules.Count; i++)
                {
                    ms = (ModuleSettings) this.PortalSettings.ActivePage.Modules[i];
                    if (ms.ModuleID == ModuleID)
                        break;
                }
                string[] emails =
                    MailHelper.GetEmailAddressesInRoles(ms.AuthorizedPublishingRoles.Split(";".ToCharArray()),
                                                        this.PortalSettings.PortalID);
                for (int i = 0; i < emails.Length; i++)
                    emailForm.To.Add(emails[i]);
                // Subject
                emailForm.Subject =
                    General.GetString("SWI_REQUEST_PUBLISH_SUBJECT", "Request publishing for the new content of '") +
                    ms.ModuleTitle + "'";
                // Message
                emailForm.HtmlBodyText = General.GetString("SWI_REQUEST_BODY", "You can find the new content at:") +
                                         "<br><br><a href='" + UrlReferrer + "'>" + UrlReferrer + "</a>";
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Handles the OnInit event at Page level<br/>
        /// Performs OnInit events that are common to all Pages<br/>
        /// Can be overridden
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.btnApproveAndSendMail.Click += new EventHandler(this.btnApproveAndSendMail_Click);
            this.btnApprove.Click += new EventHandler(this.btnApprove_Click);
            this.CancelButton.Click += new EventHandler(this.cancelButton_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.RedirectBackToReferringPage();
        }

        /// <summary>
        /// Handles the Click event of the btnApprove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApprove_Click(object sender, EventArgs e)
        {
            Approve(e);
        }

        /// <summary>
        /// Handles the Click event of the btnApproveAndSendMail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApproveAndSendMail_Click(object sender, EventArgs e)
        {
            if (emailForm.AllEmailAddressesOk)
            {
                // Send mail
                MailMessage mm = new MailMessage();
                // jes1111 - mm.From = MailHelper.GetCurrentUserEmailAddress(ConfigurationSettings.AppSettings["EmailFrom"]);
                mm.From = MailHelper.GetCurrentUserEmailAddress(Config.EmailFrom);
                mm.To = string.Join(";", (string[]) emailForm.To.ToArray(typeof (string)));
                mm.Cc = string.Join(";", (string[]) emailForm.Cc.ToArray(typeof (string)));
                mm.Bcc = string.Join(";", (string[]) emailForm.Bcc.ToArray(typeof (string)));
                mm.BodyFormat = MailFormat.Html;
                mm.Body = emailForm.BodyText;
                mm.Subject = emailForm.Subject;

                //jes1111 - SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["SmtpServer"];
                SmtpMail.SmtpServer = Config.SmtpServer;
                SmtpMail.Send(mm);

                // Request approval
                Approve(e);
            }
        }

        /// <summary>
        /// Approves the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Approve(EventArgs e)
        {
            // Geert.Audenaert
            // 10/03/2003
            // This is not necessary
            //base.OnUpdate(e);

            WorkFlowDB.Approve(ModuleID);
            this.RedirectBackToReferringPage();
        }
    }
}