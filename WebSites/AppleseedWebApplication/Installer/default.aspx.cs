using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using Appleseed.Framework.Settings.Cache;
using System.Net;

namespace AppleseedWebApplication.Installer
{
    public partial class Default : System.Web.UI.Page
    {

        /**************************************************************
        Title: Appleseed Web Installer v.1.0
        Author: Rahul Singh (Anant), rahul.singh@anant.us
        Date: 9.8.2006

        Special thanks to the following projects from which I borrowed ideas from.
        1. CommunityServer.org - First ASP.NET Open Source project with a web based installer.
        2. Joomla.org - Really nice open source CMS / Portal written in PHP with a nice installer.
        3. WordPress.org - Great Open Source Blogging Software written in PHP with a nice installer.

        The original file for this installer came from CommunityServer.
        I later used a few functions from it to complete this installer. 


        ***************************************************************/


        /**************************************************************
        To enable the web based installer change the
        line beneath this section to ‘true’.

        After running the installer it is highly recommended that you 
        set this value back to false to disable unauthorized access.
        **************************************************************/
        bool INSTALLER_ENABLED = true;

        string PasswordForDB;

        // flag indicating that the web.config file was successfully updated. This only works if you have write access
        // to your virtual directory.
        bool updatedConfigFile = false;

        // consant string used to allow host to pass the database name to the wizard. If the database can be found in the 
        // list of database returned, the wizard will skip the database selection page.
        private const string QSK_DATABASE = "database"; // query string key

        // arraylist of InstallerMessages. We contruct this on every page request to only keep track of the errors
        // that have occurred during this web request. We don't store it in viewstate because we only want the errors
        // that have happened on each page request
        private ArrayList messages;

        // Class to encapsulate the module (method) along with the error message that occurred within the module(method)
        public class InstallerMessage
        {
            public string Module;
            public string Message;

            public InstallerMessage(string module, string message)
            {
                Module = module;
                Message = message;
            }
        };


        public WizardPanel CurrentWizardPanel
        {
            get
            {
                if (ViewState["WizardPanel"] != null)
                    return (WizardPanel)ViewState["WizardPanel"];

                return WizardPanel.PreInstall;
            }
            set
            {
                ViewState["WizardPanel"] = value;
            }
        }

        /* Site Information */

        public string SmtpServerText;
        public string PortalPrefixText;
        public string EmailFromText;
        public string EncryptPasswordText;


        /* TODO: protected string AdminPassword : randomly created admin password 
            CreateKey(8);

        */


        public enum WizardPanel
        {
            PreInstall,
            License,
            ConnectToDb,
            SelectDb,
            SiteInformation,
            Install,
            Done,
            Errors,
        }


        void HideAllPanels()
        {
            PreInstall.Visible = false;
            License.Visible = false;
            ConnectToDb.Visible = false;
            SiteInformation.Visible = false;
            Install.Visible = false;
            Done.Visible = false;
            Errors.Visible = false;
        }

        public void Page_Load()
        {
            // We use the installer enabled flag to prevent someone from accidentally running the web installer, or
            // someone trying to maliciously trying to run the installer 
            if (!INSTALLER_ENABLED)
            {
                //TODO: make this error display on a nice panel
                Response.Write("<h1>Appleseed Installation Wizard is disabled.</h1>");
                Response.Flush();
                Response.End();
            }
            else
            {
                messages = new ArrayList();

                if (!Page.IsPostBack)
                {
                    SetActivePanel(WizardPanel.PreInstall, PreInstall);
                    CheckEnvironment();
                }
            }
        }

        public void ReportException(string module, Exception e)
        {
            ReportException(module, e.Message);
        }

        public void ReportException(string module, string message)
        {
            messages.Add(new InstallerMessage(module, message));
        }

        void SetActivePanel(WizardPanel panel, Control controlToShow)
        {

            Panel currentPanel = FindControl(CurrentWizardPanel.ToString()) as Panel;
            if (currentPanel != null)
                currentPanel.Visible = false;

            switch (panel)
            {
                case WizardPanel.PreInstall:
                    Previous.Enabled = false;
                    License.Visible = false;
                    break;
                case WizardPanel.Done:
                    Next.Enabled = false;
                    Previous.Enabled = false;
                    break;
                case WizardPanel.Errors:
                    Previous.Enabled = false;
                    Next.Enabled = false;
                    break;
                default:
                    Previous.Enabled = true;
                    Next.Enabled = true;
                    break;
            }

            controlToShow.Visible = true;
            CurrentWizardPanel = panel;

        }

        private void CheckEnvironment()
        {

            string configFile = HttpContext.Current.Server.MapPath("~/web.config");
            string logsDir = HttpContext.Current.Server.MapPath("~/rb_Logs");
            string portalsDir = HttpContext.Current.Server.MapPath("~/Portals");

            lblAspNetVersion.Text = "<span style='color:green;' >" + System.Environment.Version.ToString() + "</span>";
            lblWebConfigWritable.Text = IfFileWritable(configFile);
            lblLogsDirWritable.Text = IfDirectoryWritable(logsDir);
            lblPortalsDirWritable.Text = IfDirectoryWritable(portalsDir);
        }

        private string IfFileWritable(string FileName)
        {

            string returnInfo = "";

            FileInfo FileNameInfo = new FileInfo(FileName);

            if (FileNameInfo.Exists)
            {
                returnInfo = "<span style='color:green;' >Exists</span>";

                try
                {
                    StreamWriter sw = File.AppendText(FileName);
                    sw.Write(" ");
                    sw.Close();
                    returnInfo += ",<span style='color:green;' >Writable</span>";

                }
                catch (Exception e)
                {

                    returnInfo += ",<span style='color:red;' >Un-Writable: " + e.Message + "</span>";
                }
            }
            else
            {
                returnInfo = "File Doesn't Exist";
            }

            return returnInfo;
        }

        private string IfDirectoryWritable(string DirectoryName)
        {
            string returnInfo = "";
            string FileName = "TempFile.txt";

            DirectoryInfo DirectoryNameInfo = new DirectoryInfo(DirectoryName);

            if (DirectoryNameInfo.Exists)
            {
                returnInfo = "<span style='color:green;' >Exists</span>";

                try
                {
                    StreamWriter sw = File.AppendText(DirectoryName + "\\" + FileName);
                    sw.Write("-");
                    sw.Close();
                    File.Delete(DirectoryName + "\\" + FileName);
                    returnInfo += ",<span style='color:green;' >Writable</span>";

                }
                catch (Exception e)
                {
                    returnInfo += ",<span style='color:red;' >Un-Writable: " + e.Message + "</span>";
                }

            }
            else
            {
                returnInfo = "Directory Doesn't Exist";
            }
            return returnInfo;
        }

        /*******
        private bool InstallDatabase() Deleted
        TODO: do some initial database create here, let DB installer take care of rest.
        take care of any aspnetdb role user assignments here using AddToRole
 
        *******/

        private bool InstallConfig()
        {
            using (Appleseed.Framework.Core.Update.Services s = new Appleseed.Framework.Core.Update.Services())
            {
               
                if (s.RunDBUpdate(GetDatabaseConnectionString()))
                {
                    UpdateWebConfig();
                }
                
            }


            return true;
        }

        protected bool UpdateWebConfig()
        {
            bool returnValue = false;
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                if (doc == null)
                    return false;

                doc.PreserveWhitespace = true;

                string configFile = HttpContext.Current.Server.MapPath("~/web.config");

                doc.Load(configFile);
                bool dirty = false;

                // for Appleseed 2.0
                var ns = new System.Xml.XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("x", "http://schemas.microsoft.com/.NetConfiguration/v2.0");

                System.Xml.XmlNode connectionStrings = doc.SelectSingleNode("/x:configuration/x:connectionStrings", ns);
                foreach (System.Xml.XmlNode connString in connectionStrings)
                {
                    if (connString.Name == "add")
                    {
                        System.Xml.XmlAttribute attrName = connString.Attributes["name"];
                        if (attrName != null)
                        {
                            if (attrName.Value == "ConnectionString")
                            {
                                System.Xml.XmlAttribute attrCSTRValue = connString.Attributes["connectionString"];
                                if (attrCSTRValue != null)
                                {
                                    attrCSTRValue.Value = GetDatabaseConnectionString();
                                    dirty = true;
                                }
                            }
                            else if (attrName.Value == "Providers.ConnectionString")
                            {
                                System.Xml.XmlAttribute attrPCSTRValue = connString.Attributes["connectionString"];
                                if (attrPCSTRValue != null)
                                {
                                    attrPCSTRValue.Value = GetDatabaseConnectionString();
                                    dirty = true;
                                }
                            }
                            else if (attrName.Value == "AppleseedProviders.ConnectionString")
                            {
                                System.Xml.XmlAttribute attrRPCSTRValue = connString.Attributes["connectionString"];
                                if (attrRPCSTRValue != null)
                                {
                                    attrRPCSTRValue.Value = GetDatabaseConnectionString();
                                    dirty = true;
                                }
                            }
                            else if (attrName.Value == "Main.ConnectionString")
                            {
                                System.Xml.XmlAttribute attrMCSTRValue = connString.Attributes["connectionString"];
                                if (attrMCSTRValue != null)
                                {
                                    attrMCSTRValue.Value = GetDatabaseConnectionString();
                                    dirty = true;
                                }
                            }
                            else if (attrName.Value == "AppleseedDBContext")
                            {
                                System.Xml.XmlAttribute attrMCSTRValue = connString.Attributes["connectionString"];
                                if (attrMCSTRValue != null)
                                {
                                    attrMCSTRValue.Value = GetEntityModelConnectionString();
                                    dirty = true;
                                }
                                System.Xml.XmlAttribute attrPVValue = connString.Attributes["providerName"];
                                if (attrPVValue != null)
                                {
                                    attrPVValue.Value = "System.Data.EntityClient";
                                    dirty = true;
                                }
                            }
                            else if (attrName.Value == "AppleseedMembershipEntities")
                            {
                                System.Xml.XmlAttribute attrMCSTRValue = connString.Attributes["connectionString"];
                                if (attrMCSTRValue != null)
                                {
                                    attrMCSTRValue.Value = GetMembershipModelConectionString();
                                    dirty = true;
                                }
                                System.Xml.XmlAttribute attrPVValue = connString.Attributes["providerName"];
                                if (attrPVValue != null)
                                {
                                    attrPVValue.Value = "System.Data.EntityClient";
                                    dirty = true;
                                }
                            }
                        }

                    }

                }


                System.Xml.XmlNode appSettings = doc.SelectSingleNode("/x:configuration/x:appSettings", ns);
                foreach (System.Xml.XmlNode setting in appSettings)
                {
                    if (setting.Name == "add")
                    {
                        System.Xml.XmlAttribute attrKey = setting.Attributes["key"];
                        if (attrKey != null)
                        {                           
                            if (attrKey.Value == "SmtpServer")
                            {
                                System.Xml.XmlAttribute attrSMTPValue = setting.Attributes["value"];
                                if (attrSMTPValue != null)
                                {
                                    attrSMTPValue.Value = SmtpServerText;
                                    dirty = true;

                                }

                            }
                            else if (attrKey.Value == "EmailFrom")
                            {
                                System.Xml.XmlAttribute attrEFROMValue = setting.Attributes["value"];
                                if (attrEFROMValue != null)
                                {
                                    attrEFROMValue.Value = EmailFromText;
                                    dirty = true;

                                }

                            }
                            else if (attrKey.Value == "PortalTitlePrefix")
                            {
                                System.Xml.XmlAttribute attrPREFIXValue = setting.Attributes["value"];
                                if (attrPREFIXValue != null)
                                {
                                    attrPREFIXValue.Value = PortalPrefixText;
                                    dirty = true;

                                }

                            }

                            else if (attrKey.Value == "EncryptPassword")
                            {
                                System.Xml.XmlAttribute attrENCPASSValue = setting.Attributes["value"];
                                if (attrENCPASSValue != null)
                                {
                                    attrENCPASSValue.Value = EncryptPasswordText;
                                    dirty = true;

                                }

                            }

                        }
                    }
                }

                if (dirty)
                {
                    // Save the document to a file and auto-indent the output.
                    System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(configFile, System.Text.Encoding.UTF8);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    doc.Save(writer);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                ReportException("UpdateWebConfig", e);
            }

            return returnValue;
        }

        /*
        TODO: use this to generate passwords as well.
        protected string CreateKey(int len)
        {
                    byte[] bytes = new byte[len];
                    new RNGCryptoServiceProvider().GetBytes(bytes);
			
                    StringBuilder sb = new StringBuilder();
                    for(int i = 0; i < bytes.Length; i++)
                    {	
                        sb.Append(string.Format("{0:X2}",bytes[i]));
                    }
			
                    return sb.ToString();
        }
        */
        private string GetConnectionString()
        {
            return String.Format("server={0};uid={1};pwd={2};Trusted_Connection={3}", db_server.Text, db_login.Text, db_password.Text, (db_Connect.SelectedIndex == 0 ? "yes" : "no"));
        }

        private string GetDatabaseConnectionString()
        {
            return String.Format("{0};database={1}", GetConnectionString(), db_name_list.SelectedValue);
        }

        private string GetEntityModelConnectionString()
        {
            return string.Format("metadata=res://*/Models.AppleseedModel.csdl|res://*/Models.AppleseedModel.ssdl|res://*/Models.AppleseedModel.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={0};Initial Catalog={1};User ID={2};pwd={3};Trusted_Connection={4};MultipleActiveResultSets=True\"", db_server.Text, db_name_list.SelectedValue, db_login.Text, db_password.Text, (db_Connect.SelectedIndex == 0 ? "yes" : "no"));
        }

        private string GetMembershipModelConectionString() 
        {
            return string.Format("metadata=res://*/AppleseedMembershipModel.csdl|res://*/AppleseedMembershipModel.ssdl|res://*/AppleseedMembershipModel.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source={0};Initial Catalog={1};User ID={2};pwd={3};Trusted_Connection={4};MultipleActiveResultSets=True\"", db_server.Text, db_name_list.SelectedValue, db_login.Text, db_password.Text, (db_Connect.SelectedIndex == 0 ? "yes" : "no"));
        }


        private bool Validate_ConnectToDb(out string errorMessage)
        {

            //	ConnectionString = "server=" + db_server.Text + ";uid="+ db_login.Text +";pwd=" + db_password.Text + ";Trusted_Connection=" + (db_Connect.SelectedIndex == 0 ? "yes" : "no");

            try
            {

                SqlConnection connection = new SqlConnection(GetConnectionString());
                connection.Open();
                connection.Close();

                errorMessage = "";
                return true;

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

        }

        private bool Validate_SelectDb(out string errorMessage)
        {

            try
            {

                using (SqlConnection connection = new SqlConnection(GetDatabaseConnectionString()))
                {
                    connection.Open();
                    connection.Close();
                }

                errorMessage = "";

                return true;
            }
            catch (SqlException se)
            {
                switch (se.Number)
                {
                    case 4060:	// login fails
                        if (db_Connect.SelectedIndex == 0)
                        {
                            errorMessage = "The installer is unable to access the specified database using the Windows credentials that the web server is running under. Contact your system administrator to have them add	" + Environment.UserName + " to the list of authorized logins";
                        }
                        else
                        {
                            errorMessage = "You can't login to that database. Please select another one<br />" + se.Message;
                        }
                        break;
                    default:
                        errorMessage = String.Format("Number:{0}:<br/>Message:{1}", se.Number, se.Message) + "<br/>" + GetConnectionString();
                        break;
                }
                return false;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool Validate_SelectDb_ListDatabases(out string errorMessage)
        {

            try
            {
                SqlConnection connection = new SqlConnection(GetConnectionString());
                SqlDataReader dr;
                SqlCommand command = new SqlCommand("select name from master..sysdatabases order by name asc", connection);

                connection.Open();

                // Change to the master database
                //
                connection.ChangeDatabase("master");

                dr = command.ExecuteReader();

                db_name_list.Items.Clear();

                while (dr.Read())
                {
                    string dbName = dr["name"] as String;
                    if (dbName != null)
                    {
                        if (dbName == "master" ||
                            dbName == "msdb" ||
                            dbName == "tempdb" ||
                            dbName == "model")
                        {

                            // skip the system databases
                            continue;
                        }
                        else
                        {
                            db_name_list.Items.Add(dbName);
                        }
                    }
                }

                connection.Close();

                errorMessage = "";

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool CheckSiteInfoValid()
        {
            if (rb_smtpserver.Text.Trim().Length == 0)
            {
                req_rb_smtpserver.IsValid = false;
                return false;
            }

            if (rb_portalprefix.Text.Trim().Length == 0)
            {
                req_rb_portalprefix.IsValid = false;
                return false;
            }

            if (rb_emailfrom.Text.Trim().Length == 0)
            {
                req_rb_emailfrom.IsValid = false;
                return false;
            }
            return true;
        }



        public void NextPanel(Object sender, EventArgs e)
        {
            string errorMessage = "";

            switch (CurrentWizardPanel)
            {

                case WizardPanel.PreInstall:
                    SetActivePanel(WizardPanel.License, License);
                    break;

                case WizardPanel.License:
                    if (chkIAgree.Checked)
                    {
                        SetActivePanel(WizardPanel.ConnectToDb, ConnectToDb);
                        errIAgree.Visible = false;
                    }
                    else
                        errIAgree.Visible = true;
                    break;

                case WizardPanel.ConnectToDb:
                    if (Validate_ConnectToDb(out errorMessage))
                    {
                        if (Validate_SelectDb_ListDatabases(out errorMessage))
                        {
                            if (this.Request.QueryString[QSK_DATABASE] != null &&
                                this.Request.QueryString[QSK_DATABASE] != String.Empty)
                            {

                                try
                                {
                                    db_name_list.SelectedValue = HttpUtility.UrlDecode(this.Request.QueryString[QSK_DATABASE]);

                                    SetActivePanel(WizardPanel.SiteInformation, SiteInformation);
                                }
                                catch
                                {
                                    // an error occured setting the database, lets let the user select the database
                                    SetActivePanel(WizardPanel.SelectDb, SelectDb);
                                }
                            }
                            else
                                SetActivePanel(WizardPanel.SelectDb, SelectDb);
                        }
                        else
                        {
                            lblErrMsgConnect.Text = errorMessage;
                        }
                    }
                    else
                    {
                        lblErrMsgConnect.Text = errorMessage;
                    }
                    break;

                case WizardPanel.SelectDb:
                    if (Validate_SelectDb(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.SiteInformation, SiteInformation);

                    }
                    else
                    {
                        lblErrMsg.Text = errorMessage;
                    }

                    break;
                
                case WizardPanel.SiteInformation:
                    
                    if (CheckSiteInfoValid())
                    {
                        PortalPrefixText = rb_portalprefix.Text;
                        SmtpServerText = rb_smtpserver.Text;
                        EmailFromText = rb_emailfrom.Text;
                        EncryptPasswordText = rb_encryptpassword.Checked.ToString();

                        SetActivePanel(WizardPanel.Install, Install);
                    }
                    break;
                case WizardPanel.Install:
                    if (InstallConfig())
                    {
                        SetActivePanel(WizardPanel.Done, Done);
                    }
                    else
                    {
                        lstMessages.DataSource = messages;
                        lstMessages.DataBind();

                        SetActivePanel(WizardPanel.Errors, Errors);
                    }

                    break;

                case WizardPanel.Done:
                    System.Threading.Thread.Sleep(3000);
                    break;

            }
        }



        public void PreviousPanel(Object sender, EventArgs e)
        {
            switch (CurrentWizardPanel)
            {

                case WizardPanel.PreInstall:
                    break;

                case WizardPanel.License:
                    SetActivePanel(WizardPanel.PreInstall, PreInstall);
                    break;

                case WizardPanel.ConnectToDb:
                    SetActivePanel(WizardPanel.License, License);
                    break;

                case WizardPanel.SelectDb:
                    SetActivePanel(WizardPanel.ConnectToDb, ConnectToDb);
                    break;

                case WizardPanel.SiteInformation:
                    if (Page.Request.QueryString[QSK_DATABASE] != null &&
                        Page.Request.QueryString[QSK_DATABASE] != String.Empty)
                    {

                        SetActivePanel(WizardPanel.ConnectToDb, ConnectToDb);
                    }
                    else
                    {
                        SetActivePanel(WizardPanel.SelectDb, SelectDb);
                    }
                    break;
                case WizardPanel.Install:
                    SetActivePanel(WizardPanel.SiteInformation, SiteInformation);
                    break;
                case WizardPanel.Done:
                    SetActivePanel(WizardPanel.Install, Install);
                    break;

            }
        }

        public string StepClass(WizardPanel panelName)
        {
            string returnValue = "";

            if (CurrentWizardPanel != panelName)
            {
                returnValue = "stepnotselected";
            }
            else
            {
                returnValue = "stepselected";
            }

            return returnValue;
        }


    }
}