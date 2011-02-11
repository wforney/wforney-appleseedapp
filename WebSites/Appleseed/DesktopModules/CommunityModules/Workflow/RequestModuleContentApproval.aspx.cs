using System;
using System.Web.Mail;
using System.Web.UI.WebControls;
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
    /// Summary description for RequestModuleContentApproval.
    /// </summary>
    [History("Geert.Audenaert@Syntegra.Com", "2003/03/11", "Added default content in emfailform.")]
    public partial class RequestModuleContentApproval : Page
    {
        /// <summary>
        /// 
        /// </summary>
        protected Label lblEmailAddressesNotOk;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is authorized
            if (!(PortalSecurity.HasAddPermissions(ModuleID)
                  || PortalSecurity.HasEditPermissions(ModuleID)
                  || PortalSecurity.HasDeletePermissions(ModuleID)))
                PortalSecurity.AccessDeniedEdit();

            // Fill email form with default 
            if (!IsPostBack)
            {
                // Destinators
                ModuleSettings ms = null;
                for (int i = 0; i < portalSettings.ActivePage.Modules.Count; i++)
                {
                    ms = (ModuleSettings) portalSettings.ActivePage.Modules[i];
                    if (ms.ModuleID == ModuleID)
                        break;
                }
                string[] emails =
                    MailHelper.GetEmailAddressesInRoles(ms.AuthorizedApproveRoles.Split(";".ToCharArray()),
                                                        portalSettings.PortalID);
                for (int i = 0; i < emails.Length; i++)
                    emailForm.To.Add(emails[i]);
                // Subject
                emailForm.Subject =
                    General.GetString("SWI_REQUEST_APPROVAL_SUBJECT", "Request approval of the new content of '") +
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
            this.btnRequestApprovalAndSendMail.Click += new EventHandler(this.btnRequestApprovalAndSendMail_Click);
            this.btnRequestApproval.Click += new EventHandler(this.btnRequestApproval_Click);
            this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
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
        /// Handles the Click event of the btnRequestApproval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRequestApproval_Click(object sender, EventArgs e)
        {
            RequestApproval();
        }

        /// <summary>
        /// Handles the Click event of the btnRequestApprovalAndSendMail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRequestApprovalAndSendMail_Click(object sender, EventArgs e)
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
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Config.SmtpServer);
                //smtp.Send(mm);
                SmtpMail.SmtpServer = Config.SmtpServer;
                SmtpMail.Send(mm);

                // Request approval
                RequestApproval();
            }
        }

        /// <summary>
        /// Requests the approval.
        /// </summary>
        private void RequestApproval()
        {
            WorkFlowDB.RequestApproval(ModuleID);
            this.RedirectBackToReferringPage();
        }
    }
}