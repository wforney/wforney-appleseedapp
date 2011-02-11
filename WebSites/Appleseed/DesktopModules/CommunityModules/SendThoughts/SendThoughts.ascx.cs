using System;
using System.Net.Mail;
using System.Text;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// This module sends an email with some input from the portal user
    /// Written by: Vlado
    ///
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class SendThoughts : PortalModuleControl
    {
        protected string strServerVariables;
        protected string EMailAddress;


        /// <summary>
        /// Page_Load reads setting items "Email" and "Description".
        /// All messages will be sent to "Email".
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            EMailAddress = Settings["EMail"].ToString();
            string DescText = Settings["Description"].ToString();

            if (Page.IsPostBack == false)
            {
                // Set Image EMailAddress and Desc Properties
                if (EMailAddress == null || EMailAddress.Length == 0)
                {
                    Label1.Text =
                        General.GetString("SENDTHTS_RECIPIENT", "Recipient's EMail address not set.", Label1) + "<br>";
                    EditPanel.Visible = false;
                }

                txtEMail.Text = PortalSettings.CurrentUser.Identity.Email;
            }

            if (!(DescText == null) && DescText.Length != 0)
                Label2.Text = DescText;

            strServerVariables += "HTTP_USER_AGENT: " + Request.ServerVariables["HTTP_USER_AGENT"] + "<br>";
            strServerVariables += "HTTP_HOST: " + Request.ServerVariables["HTTP_HOST"] + "<br>";
            strServerVariables += "REMOTE_HOST: " + Request.ServerVariables["REMOTE_HOST"] + "<br>";
            strServerVariables += "REMOTE_ADDR: " + Request.ServerVariables["REMOTE_ADDR"] + "<br>";
            strServerVariables += "LOCAL_ADDR: " + Request.ServerVariables["LOCAL_ADDR"] + "<br>";
            strServerVariables += "HTTP_REFERER: " + Request.ServerVariables["HTTP_REFERER"] + "<br>";
        }


        /// <summary>
        /// The SendBtn_Click server event handler on this page is
        /// used to handle the scenario where a user clicks the "Send"
        /// button after entering a response to a message post.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void SendBtn_Click(object sender, EventArgs e)
        {
            MailMessage mail = new MailMessage();
            UTF8Encoding utf8 = new UTF8Encoding();
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.From = new MailAddress(txtEMail.Text);
            mail.To.Add(EMailAddress);
            Byte[] enc = utf8.GetBytes(txtSubject.Text);
            mail.Subject = utf8.GetString(enc);
            mail.Body =
                txtBody.Text + "<br><br>" +
                General.GetString("SENDTHTS_NAME", "Name", this) + ": " + txtName.Text + "<br>" +
                General.GetString("SENDTHTS_REMAIL", "Real EMail Address", this) + ": " +
                PortalSettings.CurrentUser.Identity.Email + "<br><br>" +
                strServerVariables;

            SmtpClient smtp = new SmtpClient(Config.SmtpServer);
            smtp.Send(mail);

            Label2.Text =
                General.GetString("SENDTHTS_SENT", "The message was sent - thank you for your message!", Label2);
            EditPanel.Visible = false;
        }


        /// <summary>
        /// The ClearBtn_Click server event handler on this page is used
        /// to handle the scenario where a user clicks the "cancel"
        /// button to discard a message post and toggle out of edit mode.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void ClearBtn_Click(object sender, EventArgs e)
        {
            txtSubject.Text = string.Empty;
            txtBody.Text = string.Empty;
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531003}"); }
        }


        /// <summary>
        /// The Constructor adds the Setting items "Email" and "Description"
        /// All messages will be sent to "Email"
        /// </summary>
        public SendThoughts()
        {
            SettingItem setEMail = new SettingItem(new StringDataType());
            setEMail.Required = true;
            setEMail.Value = string.Empty;
            setEMail.Order = 1;
            _baseSettings.Add("EMail", setEMail);

            SettingItem setDescription = new SettingItem(new StringDataType());
            setDescription.Required = true;
            setDescription.Value = General.GetString("SENDTHTS_DES_TXT", "Write a description here...", this);
            setDescription.Order = 2;
            _baseSettings.Add("Description", setDescription);
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
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