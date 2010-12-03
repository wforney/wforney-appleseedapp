using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;
using Label=Appleseed.Framework.Web.UI.WebControls.Label;
using Localize=Appleseed.Framework.Web.UI.WebControls.Localize;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    ///	Summary description for EmailForm.
    /// </summary>
    public class EmailForm : UserControl
    {
        #region Declerations

        /// <summary>
        /// CC List
        /// </summary>
        protected TextBox txtCc;

        /// <summary>
        /// BCC List
        /// </summary>
        protected TextBox txtBcc;

        /// <summary>
        /// Subject Textbox
        /// </summary>
        protected TextBox txtSubject;

        /// <summary>
        /// BOdy Area
        /// </summary>
        protected IHtmlEditor txtBody;

        /// <summary>
        /// To List
        /// </summary>
        protected TextBox txtTo;

        /// <summary>
        /// 
        /// </summary>
        private EmailAddressList _to;

        /// <summary>
        /// 
        /// </summary>
        private EmailAddressList _cc;

        /// <summary>
        /// 
        /// </summary>
        private EmailAddressList _bcc;

        /// <summary>
        /// 
        /// </summary>
        protected Label lblEmailAddressesNotOk;

        /// <summary>
        /// 
        /// </summary>
        protected Localize Literal1;

        /// <summary>
        /// 
        /// </summary>
        protected Localize Literal2;

        /// <summary>
        /// 
        /// </summary>
        protected Localize Literal3;

        /// <summary>
        /// 
        /// </summary>
        protected Localize Literal4;

        /// <summary>
        /// 
        /// </summary>
        protected PlaceHolder PlaceHolderHTMLEditor;

        /// <summary>
        /// /
        /// </summary>
        private bool _allAddressesOk = true;

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                _allAddressesOk = true;
                // Initialize To addresses
                foreach (string em in txtTo.Text.Split(";".ToCharArray()))
                {
                    try
                    {
                        if (em.Trim().Length != 0)
                            To.Add(em);
                    }
                    catch (ArgumentException ae)
                    {
                        string message = ae.Message;
                        _allAddressesOk = false;
                    }
                }
                // Initialize Cc addresses
                foreach (string em in txtCc.Text.Split(";".ToCharArray()))
                {
                    try
                    {
                        if (em.Trim().Length != 0)
                            Cc.Add(em);
                    }
                    catch (ArgumentException ae)
                    {
                        string message = ae.Message;
                        _allAddressesOk = false;
                    }
                }
                // Initialize To addresses
                foreach (string em in txtBcc.Text.Split(";".ToCharArray()))
                {
                    try
                    {
                        if (em.Trim().Length != 0)
                            Bcc.Add(em);
                    }
                    catch (ArgumentException ae)
                    {
                        string message = ae.Message;
                        _allAddressesOk = false;
                    }
                }
                // Show error
                lblEmailAddressesNotOk.Visible = ! AllEmailAddressesOk;
            }
            else
            {
                txtTo.Text = string.Join(";", (string[]) To.ToArray(typeof (string)));
                txtCc.Text = string.Join(";", (string[]) Cc.ToArray(typeof (string)));
                txtBcc.Text = string.Join(";", (string[]) Bcc.ToArray(typeof (string)));
            }
        }

        /// <summary>
        /// Collection containing all to email addresses
        /// </summary>
        public EmailAddressList To
        {
            get { return _to; }
        }

        /// <summary>
        /// Collection containing all cc email addresses
        /// </summary>
        public EmailAddressList Cc
        {
            get { return _cc; }
        }

        /// <summary>
        /// Collection containing all bcc email addresses
        /// </summary>
        public EmailAddressList Bcc
        {
            get { return _bcc; }
        }

        /// <summary>
        /// Contains subject
        /// </summary>
        public string Subject
        {
            get { return txtSubject.Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Subject", "Subject can not contain null values!");
                txtSubject.Text = value;
            }
        }

        /// <summary>
        /// Contains text for the body of the email in html format
        /// </summary>
        public string HtmlBodyText
        {
            get { return txtBody.Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("HtmlBodyText", "HtmlBodyText can not contain null values!");
                txtBody.Text = value;
            }
        }

        /// <summary>
        /// Contains text for the body of the email in plain text format
        /// </summary>
        public string BodyText
        {
            get { return txtBody.Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("BodyText", "BodyText can not contain null values!");
                txtBody.Text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllEmailAddressesOk
        {
            get { return _allAddressesOk; }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            _to = new EmailAddressList();
            _cc = new EmailAddressList();
            _bcc = new EmailAddressList();

            HtmlEditorDataType h = new HtmlEditorDataType();
            PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            try
            {
                h.Value = pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"].ToString();
                txtBody =
                    h.GetEditor(PlaceHolderHTMLEditor, int.Parse(Context.Request["mID"]),
                                bool.Parse(pS.CustomSettings["SITESETTINGS_SHOWUPLOAD"].ToString()), pS);
            }
            catch
            {
                txtBody = h.GetEditor(PlaceHolderHTMLEditor, int.Parse(Context.Request["mID"]), true, pS);
            }

            lblEmailAddressesNotOk.Text =
                General.GetString("EMF_ADDRESSES_NOT_OK", "The emailaddresses are not ok.", lblEmailAddressesNotOk);
        }

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}