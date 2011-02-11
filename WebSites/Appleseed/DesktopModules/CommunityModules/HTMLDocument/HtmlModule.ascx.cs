using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Appleseed.Framework;
using Appleseed.Framework.BLL.MergeEngine;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Security;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;
//using Abtour.PortalTemplate;
//using Abtour.PortalTemplate.DTOs;
using System.Xml.Serialization;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// HTML Document Module
    /// Represents any text that can contain HTML
    /// Edited with HTMLeditors
    /// </summary>
    public partial class HtmlModule : PortalModuleControl//, IModuleExportable
    {
        #region Member variables

        // Added by Hongwei Shen(Hongwei.shen@gmail.com) 10/9/2005
        // for supporting version compare
        private const string COMPARE_BUTTON = "MODULESETTINGS_HTMLDOCUMENT_SHOW_COMPARE_BUTTON";

        // private variables
        private ModuleButton _btnCompare;
        // end of addition

        /// <summary>
        /// PlaceHolder for the HTML
        /// </summary>
        // protected PlaceHolder HtmlHolder;
        /// <summary>
        /// Takes the html text and does a path correction before loading 
        /// to the HTML document placeholder
        /// </summary>
        protected LiteralControl HtmlLiteral;

        #endregion

        #region Constructor

        /// <summary>
        /// HtmlModule Constructor
        /// Set the Module Settings
        /// <list type="">
        /// 		<item>ShowMobile</item>
        /// 	</list>
        /// </summary>
        public HtmlModule()
        {
            int _groupOrderBase;
            SettingItemGroup _Group;

            #region Module Special Settings

            _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            _groupOrderBase = (int) SettingItemGroup.MODULE_SPECIAL_SETTINGS;

            HtmlEditorDataType.HtmlEditorSettings(_baseSettings, _Group);

            //If false the input box for mobile content will be hidden
            SettingItem ShowMobileText = new SettingItem(new BooleanDataType());
            ShowMobileText.Value = "true";
            ShowMobileText.Order = _groupOrderBase + 10;
            ShowMobileText.Group = _Group;
            _baseSettings.Add("ShowMobile", ShowMobileText);

            #endregion

            #region Button Display Settings for this module

            // added by Hongwei Shen(Hongwei.shen@gmail.com) 10/9/2005

            _Group = SettingItemGroup.BUTTON_DISPLAY_SETTINGS;
            _groupOrderBase = (int) SettingItemGroup.BUTTON_DISPLAY_SETTINGS;

            //If false the compare button will be hidden
            SettingItem ShowCompareButton = new SettingItem(new BooleanDataType());
            ShowCompareButton.Value = "true";
            ShowCompareButton.Order = _groupOrderBase + 60;
            ShowCompareButton.Group = _Group;
            ShowCompareButton.EnglishName = "Show Compare Button?";
            ShowCompareButton.Description = "Compare the working version with the live one";
            _baseSettings.Add(COMPARE_BUTTON, ShowCompareButton);

            // end of addition

            #endregion

            SupportsWorkflow = true;

            // No need for view state on view. - jminond
            // Need viewstate to toggle the compare button Hongwei Shen 10/9/2005
            EnableViewState = true; // false;
        }

        #endregion

        #region Event handling

        /// <summary>
        /// The Page_Load event handler on this User Control is
        /// used to render a block of HTML or text to the page.
        /// The text/HTML to render is stored in the HtmlText
        /// database table.  This method uses the Appleseed.HtmlTextDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [History("Mark Gregory", "mgregory@gt.com.au", "added use of HtmlLiteral_DataBinding")]
        [
            History("William Forney", "bill@improvdesign.com",
                "Moved data reader code to the DB class where it belongs & commented it out.")]
        private void Page_Load(object sender, EventArgs e)
        {
            HtmlTextDB text = new HtmlTextDB();
            Content = Server.HtmlDecode(text.GetHtmlTextString(ModuleID, Version));
            HtmlLiteral = new LiteralControl(Content.ToString());
            HtmlLiteral.DataBinding += new EventHandler(HtmlLiteral_DataBinding);
            HtmlLiteral.DataBind();
            HtmlHolder.Controls.Add(HtmlLiteral);
        }

        /// <summary>
        /// Handles the DataBinding event of the HtmlLiteral control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HtmlLiteral_DataBinding(object sender, EventArgs e)
        {
            ((LiteralControl) sender).Text = ((LiteralControl) sender).Text.Replace("~/", Path.ApplicationRoot + "/");
        }

        // Added by Hongwei Shen(Hongwei.shen@gmail.com) 10/9/2005
        // for supporting version comparison

        /// <summary>
        /// Handle the request for comparison
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CompareButton_Click(Object sender, EventArgs e)
        {
            HtmlTextDB text = new HtmlTextDB();
            HtmlHolder.Controls.Clear();

            if (IsComparing == 0)
            {
                Content = Server.HtmlDecode(text.GetHtmlTextString(ModuleID, Version));
                HtmlLiteral = new LiteralControl(Content.ToString());
                HtmlLiteral.DataBinding += new EventHandler(HtmlLiteral_DataBinding);
                HtmlLiteral.DataBind();
                HtmlHolder.Controls.Add(HtmlLiteral);
                IsComparing = 1;
            }
            else
            {
                string prod = Server.HtmlDecode(text.GetHtmlTextString(ModuleID, WorkFlowVersion.Production));
                string stag = Server.HtmlDecode(text.GetHtmlTextString(ModuleID, WorkFlowVersion.Staging));
                Merger merger = new Merger(prod, stag);
                Content = Server.HtmlDecode(merger.merge());
                HtmlLiteral = new LiteralControl(Content.ToString());
                HtmlLiteral.DataBinding += new EventHandler(HtmlLiteral_DataBinding);
                HtmlLiteral.DataBind();
                HtmlHolder.Controls.Add(HtmlLiteral);
                IsComparing = 0;
            }
        }

        // end of addition

        #endregion

        #region Overrriden parent methods

        // Added by Hongwei Shen(Hongwei.shen@gmail.com) 10/9/2005
        // for supporting version comparison
        /// <summary>
        /// Override to add the Compare button to the button list
        /// </summary>
        protected override void BuildButtonLists()
        {
            // add Compare button
            if (CompareButton != null)
            {
                ButtonListAdmin.Add(_btnCompare);
                if (IsComparing == -1)
                {
                    // it is the time to toggle the buttons
                    // manully
                    IsComparing = 1;
                }
            }
            base.BuildButtonLists();
        }

        // end of addition

        #endregion

        #region Properties

        // Added by Hongwei. Shen(hongwei.shen@gmail.com) to support
        // version comparison. 10/9/2005
        /// <summary>
        /// Check if setting allows version Compare button
        /// </summary>
        /// <value><c>true</c> if [support compare]; otherwise, <c>false</c>.</value>
        public bool SupportCompare
        {
            get
            {
                object o = Settings[COMPARE_BUTTON];
                if (o != null)
                    return bool.Parse(o.ToString());
                return false;
            }
        }

        /// <summary>
        /// Permission for Compare and BackToStage buttons
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can compare; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual bool CanCompare
        {
            get
            {
                if (ModuleConfiguration == null || !SupportCompare)
                {
                    return false;
                }

                if (SupportsWorkflow
                    && (PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedApproveRoles) ||
                        PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedEditRoles) ||
                        PortalSecurity.IsInRoles(ModuleConfiguration.AuthorizedDeleteRoles))
                    && Version == WorkFlowVersion.Staging)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The button toggles between displaying the staging content
        /// and displaying the comparison of the staging with the
        /// production version with the differences hightlighted.
        /// </summary>
        /// <value>The compare button.</value>
        public ModuleButton CompareButton
        {
            get
            {
                if (_btnCompare == null && HttpContext.Current != null)
                {
                    if (CanCompare)
                    {
                        // create the button
                        _btnCompare = new ModuleButton();
                        _btnCompare.Group = ModuleButton.ButtonGroup.Admin;
                        _btnCompare.ServerClick += new EventHandler(CompareButton_Click);
                        _btnCompare.RenderAs = ButtonsRenderAs;
                    }
                }

                if (_btnCompare != null)
                {
                    if (IsComparing == 1)
                    {
                        // if it is in comparing status, clicking the button
                        // will bring the content back to staging
                        _btnCompare.TranslationKey = "BackToStaging";
                        _btnCompare.EnglishName = "Back to staging";
                        _btnCompare.Image = CurrentTheme.GetImage("Buttons_Stage", "stage.gif");
                    }
                    else
                    {
                        // otherwise, clicking will do comparison
                        _btnCompare.TranslationKey = "Compare";
                        _btnCompare.EnglishName = "Compare staging with production";
                        _btnCompare.Image = CurrentTheme.GetImage("Buttons_Compare", "Compare.gif");
                    }
                }

                return _btnCompare;
            }
        }

        /// <summary>
        /// Remember the status of the Compare Button to allow
        /// toggling between displaying comparison and staging
        /// content.
        /// </summary>
        /// <value>The is comparing.</value>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual int IsComparing
        {
            get
            {
                object o = ViewState["NewHtmlModuleIsComparing"];
                if (o == null)
                {
                    // the first time is special because the compare
                    // button click event handler is not called, thus,
                    // we need to toggle IsCompare manually
                    return -1;
                }
                else
                {
                    return (int) o;
                }
            }
            set { ViewState["NewHtmlModuleIsComparing"] = value; }
        }

        // end of addition

        /// <summary>
        /// Searchable module
        /// </summary>
        /// <value></value>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// General Module Def GUID
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{0B113F51-FEA3-499A-98E7-7B83C192FDBB}"); }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>
        /// The SELECT sql to perform a search on the current module
        /// </returns>
        public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            SearchDefinition s = new SearchDefinition("rb_HtmlText", "DesktopHtml", "DesktopHtml", searchField);
            return s.SearchSqlSelect(portalID, userID, searchString, false);
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            this.HtmlHolder.EnableViewState = false;
            // Add title
            //			ModuleTitle = new DesktopModuleTitle();
            this.EditUrl = "~/DesktopModules/CommunityModules/HTMLDocument/HtmlEdit.aspx";
            //			Controls.AddAt(0, ModuleTitle);

            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion

        # region Install / Uninstall Implementation

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        #endregion

        //#region IModuleExportable Members

        //public string GetContentData(int moduleId)
        //{
        //    IPortalTemplateServices services = PortalTemplateFactory.GetPortalTemplateServices(new PortalTemplateRepository());
        //    HtmlTextDTO _html = services.GetHtmlTextDTO(moduleId);
        //    if (_html == null) {
        //        return string.Empty;
        //    } else {
        //        System.IO.StringWriter xout = new System.IO.StringWriter();
        //        XmlSerializer xs = new XmlSerializer(typeof(HtmlTextDTO));
        //        xs.Serialize(xout, _html);
        //        return xout.ToString();
        //    }
        //}

        //public bool SetContentData(int moduleId, string content)
        //{
        //    if (content == null || content.Equals(string.Empty)) {
        //        //si el contenido es nullo es porque no existe ningun registro en htmltext para el modulo
        //        return true;
        //    } else {
        //        IPortalTemplateServices services = PortalTemplateFactory.GetPortalTemplateServices(new PortalTemplateRepository());
        //        HtmlTextDTO _html = new HtmlTextDTO();
        //        System.IO.StringReader xin = new System.IO.StringReader(content);
        //        XmlSerializer xs = new XmlSerializer(typeof(HtmlTextDTO));
        //        _html = (HtmlTextDTO)xs.Deserialize(xin);

        //        return services.SaveHtmlText(moduleId, _html);
        //    }
        //}

        //#endregion
    }
}