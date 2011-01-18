using System;
using System.Data;
using System.Data.SqlClient;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Database Table Edit Module
    /// Based on control from TripelASP (code is free)
    /// Original programmer: ? (Manu knows)
    /// Modifications by Jakob hansen
    /// </summary>
    public partial class DatabaseTableEdit : PortalModuleControl
    {
        protected bool Connected = false;
        protected string ConnectionString;
        protected bool Trusted_Connection;
        protected string ServerName;
        protected string DatabaseName;
        protected string UserID;
        protected string Password;

        protected int MaxStringLength;
        protected bool AllowPaging;
        protected int PageSize;


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            Trusted_Connection = "True" == Settings["Trusted Connection"].ToString();
            ServerName = Settings["ServerName"].ToString();
            DatabaseName = Settings["DatabaseName"].ToString();
            UserID = Settings["UserID"].ToString();
            Password = Settings["Password"].ToString();
            MaxStringLength = int.Parse(Settings["MaxStringLength"].ToString());
            AllowPaging = "True" == Settings["AllowPaging"].ToString();
            PageSize = int.Parse(Settings["PageSize"].ToString());

            if (Trusted_Connection)
                ConnectionString = "Server=" + ServerName + ";Trusted_Connection=true;database=" + DatabaseName;
            else
                ConnectionString = "Server=" + ServerName + ";database=" + DatabaseName + ";uid=" + UserID + ";pwd=" +
                                   Password + ";";

            Connected = Connect(ConnectionString);

            panConnected.Visible = Connected;
            if (Connected)
            {
                lblConnectedError.Visible = false;

                tableeditor.ConnectionString = ConnectionString;
                tableeditor.ConfigConnectionString = ConnectionString;
                tableeditor.MaxStringLength = MaxStringLength;
                tableeditor.AllowPaging = AllowPaging;
                tableeditor.PageSize = PageSize;
            }
            else
            {
                lblConnectedError.Visible = true;
                // Added EsperantusKeys for Localization --%>
                // Mario Endara mario@softworks.com.uy 11/05/2004 --%>
                lblConnectedError.Text = General.GetString("TABLEEDIT_MSG_CONNECT");
            }

            if (!Page.IsPostBack)
            {
                BindTables();
            }
        }


        /// <summary>
        /// Connects the specified con STR.
        /// </summary>
        /// <param name="ConStr">The con STR.</param>
        /// <returns></returns>
        protected bool Connect(string ConStr)
        {
            bool retValue;
            try
            {
                SqlConnection SqlCon = new SqlConnection(ConStr);
                SqlDataAdapter DA = new SqlDataAdapter("SELECT NULL", SqlCon);
                SqlCon.Open();

                SqlCon.Close();
                SqlCon.Dispose();
                retValue = true;
            }
            catch (Exception)
            {
                retValue = false;
            }
            return retValue;
        }


        /// <summary>
        /// Binds the tables.
        /// </summary>
        private void BindTables()
        {
            string sql =
                @"	Select so.name, so.id From sysobjects so 
							where xtype = 'U' AND so.name <> 'dtproperties'
							order by so.name";

            SqlCommand cmd = new SqlCommand(sql, new SqlConnection(ConnectionString));
            try
            {
                cmd.Connection.Open();

                tablelist.DataSource = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                tablelist.DataValueField = "name";
                tablelist.DataTextField = "name";
                tablelist.DataBind();
                // Added EsperantusKeys for Localization --%>
                // Mario Endara mario@softworks.com.uy 11/05/2004 --%>
                tablelist.Items.Insert(0, General.GetString("TABLEEDIT_SELECT_TABLE"));
            }
            catch (Exception ex)
            {
                lblConnectedError.Visible = true;
                lblConnectedError.Text = ex.Message;
            }
        }


        /// <summary>
        /// Handles the SelectedIndexChanged event of the tablelist control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void tablelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tablelist.SelectedIndex > 0)
            {
                tableeditor.Table = tablelist.SelectedItem.Value.ToString();
            }
        }


        /// <summary>
        /// Public constructor. Sets base settings for module.
        /// </summary>
        public DatabaseTableEdit()
        {
            SettingItem Trusted_Connection = new SettingItem(new BooleanDataType());
            Trusted_Connection.Order = 1;
            //Trusted_Connection.Required = true;   // hmmm... problem here! Dont set to true!" 
            Trusted_Connection.Value = "True";
            _baseSettings.Add("Trusted Connection", Trusted_Connection);

            SettingItem ServerName = new SettingItem(new StringDataType());
            ServerName.Order = 2;
            ServerName.Required = true;
            ServerName.Value = "localhost";
            _baseSettings.Add("ServerName", ServerName);

            SettingItem DatabaseName = new SettingItem(new StringDataType());
            DatabaseName.Order = 3;
            DatabaseName.Required = true;
            DatabaseName.Value = "Appleseed";
            _baseSettings.Add("DatabaseName", DatabaseName);

            SettingItem UserID = new SettingItem(new StringDataType());
            UserID.Order = 4;
            UserID.Required = false;
            UserID.Value = "sa";
            _baseSettings.Add("UserID", UserID);

            SettingItem Password = new SettingItem(new StringDataType());
            Password.Order = 5;
            Password.Required = false;
            Password.Value = string.Empty;
            _baseSettings.Add("Password", Password);

            SettingItem MaxStringLength = new SettingItem(new IntegerDataType());
            MaxStringLength.Order = 6;
            MaxStringLength.Required = true;
            MaxStringLength.Value = "100";
            _baseSettings.Add("MaxStringLength", MaxStringLength);

            SettingItem AllowPaging = new SettingItem(new BooleanDataType());
            AllowPaging.Order = 7;
            //AllowPaging.Required = true;   // hmmm... problem here! Dont set to true!" 
            AllowPaging.Value = "True";
            _baseSettings.Add("AllowPaging", AllowPaging);

            SettingItem PageSize = new SettingItem(new IntegerDataType());
            PageSize.Order = 8;
            PageSize.Required = true;
            PageSize.Value = "10";
            _baseSettings.Add("PageSize", PageSize);
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{AB02A3F4-A0A4-45e0-96ED-8450C19166C5}"); }
        }

        /// <summary>
        /// Admin Module
        /// </summary>
        /// <value></value>
        public override bool AdminModule
        {
            get { return true; }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.tablelist.SelectedIndexChanged += new EventHandler(this.tablelist_SelectedIndexChanged);
            this.Load += new EventHandler(this.Page_Load);

            // Create a new Title the control
//			ModuleTitle = new DesktopModuleTitle();
            // Add title ad the very beginning of 
            // the control's controls collection
//			Controls.AddAt(0, ModuleTitle);

            base.OnInit(e);
        }

        #endregion
    }
}