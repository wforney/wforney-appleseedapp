using System;
using System.Web.Mail;
using Appleseed.Framework;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Summary description for RejectModuleContent.
    /// </summary>
    public partial class RejectModuleContent : Page
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
                string tmp = ms.AuthorizedAddRoles.Trim();
                tmp += ms.AuthorizedEditRoles.Trim();
                tmp += ms.AuthorizedDeleteRoles.Trim();
                string[] emails =
                    MailHelper.GetEmailAddressesInRoles(tmp.Split(";".ToCharArray()), this.PortalSettings.PortalID);
                for (int i = 0; i < emails.Length; i++)
                    emailForm.To.Add(emails[i]);
                // Subject
                emailForm.Subject = General.GetString("SWI_REJECT_SUBJECT1", "The new content of ") + "'" +
                                    ms.ModuleTitle + "'" +
                                    General.GetString("SWI_REJECT_SUBJECT2", " has been rejected");
                // Message
                emailForm.HtmlBodyText = General.GetString("SWI_REJECT_BODY", "You can find the rejected content at:") +
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
            this.btnRejectAndSendMail.Click += new EventHandler(this.btnRejectAndSendMail_Click);
            this.btnReject.Click += new EventHandler(this.btnReject_Click);
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
        /// Handles the Click event of the btnReject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnReject_Click(object sender, EventArgs e)
        {
            Reject();
        }

        /// <summary>
        /// Handles the Click event of the btnRejectAndSendMail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRejectAndSendMail_Click(object sender, EventArgs e)
        {
            if (emailForm.AllEmailAddressesOk)
            {
                // Send mail
                MailMessage mm = new MailMessage();
                //jes1111 - mm.From = MailHelper.GetCurrentUserEmailAddress(ConfigurationSettings.AppSettings["EmailFrom"]);
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
                Reject();
            }
        }

        /// <summary>
        /// Rejects this instance.
        /// </summary>
        private void Reject()
        {
            WorkFlowDB.Reject(ModuleID);
            this.RedirectBackToReferringPage();
        }
    }
}