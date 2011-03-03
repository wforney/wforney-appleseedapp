// Esperantus - The Web translator
// Copyright (C) 2003 Emmanuele De Andreis
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Emmanuele De Andreis (manu-dea@hotmail dot it)

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework.Settings;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Summary description for LanguageSwitcher.
    /// </summary>
    [ToolboxData("<{0}:LanguageSwitcher runat='server'></{0}:LanguageSwitcher>")]
    [Designer("Esperantus.Design.WebControls.LanguageSwitcherDesigner")]
    [DefaultProperty("LanguageListString")]
    //[PermissionSet(SecurityAction.LinkDemand, XML="<PermissionSet class=\"System.Security.PermissionSet\"\r\n version=\"1\">\r\n <IPermission class=\"System.Web.AspNetHostingPermission, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n version=\"1\"\r\n Level=\"Minimal\"/>\r\n</PermissionSet>\r\n"), PermissionSet(SecurityAction.InheritanceDemand, XML="<PermissionSet class=\"System.Security.PermissionSet\"\r\n version=\"1\">\r\n <IPermission class=\"System.Web.AspNetHostingPermission, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n version=\"1\"\r\n Level=\"Minimal\"/>\r\n</PermissionSet>\r\n")]
    public class LanguageSwitcher : WebControl, IPostBackEventHandler
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public LanguageSwitcher()
        {
            //Trace.WriteLine("LanguageSwitcher() constructor *****************************************");
            if (Context != null && Context.Request != null)
                ChangeLanguageUrl = Context.Request.Path;
        }

        #endregion

        #region Control creation

        private const string SWITCHER_COOKIE_NAME = "languageSwitcher"; //TODO: do not hardcode cookie name
        private const string SWITCHER_COOKIE_PREFIX = "Esperantus_Language_";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            //Trace.WriteLine("RenderingContents");
            EnsureChildControls();
            foreach (Control ctrl in Controls)
                ctrl.RenderControl(output);
        }

        private DropDownList langDropDown;

        /// <summary>
        /// Override CreateChildControls to create the control tree.
        /// </summary>
        protected override void CreateChildControls()
        {
            //Trace.WriteLine("Creating controls: Type: " + Type.ToString() + " / ChangeLanguageAction: " + ChangeLanguageAction.ToString() + " / Labels: " + Labels.ToString() + " / Flags: " + Flags.ToString() + " / LanguageListString: " + LanguageListString);

            ProcessCultures(LanguageListString, SWITCHER_COOKIE_NAME, this);

            Controls.Clear();
            Table myTable = new Table();
            myTable.CellPadding = 3;
            myTable.CellSpacing = 0;
            TableRowCollection myRows = myTable.Rows;

            switch (Type)
            {
                    //Drop down list
                case LanguageSwitcherType.DropDownList:

                    TableRow myTableRowDD = new TableRow();

                    if (Flags != LanguageSwitcherDisplay.DisplayNone)
                    {
                        Image myImage = new Image();
                        myImage.ImageUrl = GetFlagImg(GetCurrentLanguage());

                        TableCell myTableCellFlag = new TableCell();
                        myTableCellFlag.Controls.Add(myImage);

                        myTableRowDD.Controls.Add(myTableCellFlag);
                    }

                    TableCell myTableCellDropDown = new TableCell();
                    langDropDown = new DropDownList();
                    langDropDown.CssClass = "rb_LangSw_dd"; //TODO make changeable

                    LanguageCultureItem myCurrentLanguage = GetCurrentLanguage();

                    // bind the dropdownlist
                    langDropDown.Items.Clear();
                    foreach (LanguageCultureItem i in LanguageList)
                    {
                        langDropDown.Items.Add(new ListItem(GetName(i), i.UICulture.Name));
                        if (i.UICulture.ToString() == myCurrentLanguage.UICulture.ToString()) //Select current language
                            langDropDown.Items[langDropDown.Items.Count - 1].Selected = true;
                    }

                    langDropDown.Attributes.Add("OnChange",
                                                GetLangAction().Replace("''", "this[this.selectedIndex].value"));

                    myTableCellDropDown.Controls.Add(langDropDown);
                    myTableRowDD.Controls.Add(myTableCellDropDown);
                    myRows.Add(myTableRowDD);
                    break;

                    // Links
                case LanguageSwitcherType.VerticalLinksList:

                    foreach (LanguageCultureItem l in LanguageList)
                    {
                        TableRow myTableRowLinks = new TableRow();

                        if (Flags != LanguageSwitcherDisplay.DisplayNone)
                            myTableRowLinks.Controls.Add(GetFlagCell(l));
                        if (Labels != LanguageSwitcherDisplay.DisplayNone)
                            myTableRowLinks.Controls.Add(GetLabelCell(l));
                        myRows.Add(myTableRowLinks);
                    }
                    break;

                    // Horizontal links
                case LanguageSwitcherType.HorizontalLinksList:

                    TableRow myTableRowLinksHorizontal = new TableRow();

                    foreach (LanguageCultureItem l in LanguageList)
                    {
                        if (Flags != LanguageSwitcherDisplay.DisplayNone)
                            myTableRowLinksHorizontal.Controls.Add(GetFlagCell(l));
                        if (Labels != LanguageSwitcherDisplay.DisplayNone)
                            myTableRowLinksHorizontal.Controls.Add(GetLabelCell(l));
                    }

                    myRows.Add(myTableRowLinksHorizontal);
                    break;
            }

            Controls.Add(myTable);
        }

        #endregion

        #region Events and delegates

        /// <summary>
        /// The ChangeLanguage event is defined using the event keyword.
        /// The type of ChangeLanguage is EventHandler.
        /// </summary>
        public event LanguageSwitcherEventHandler ChangeLanguage;

        /// <summary>
        /// Examines/combines all the variables involved and sets
        /// CurrentUICulture and CurrentCulture.
        /// Can be overridden.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChangeLanguage(LanguageSwitcherEventArgs e)
        {
            // Updates current cultures
            SetCurrentLanguage(e.CultureItem, SWITCHER_COOKIE_NAME, this);

            if (ChangeLanguage != null)
                ChangeLanguage(this, e); //Invokes the delegates
        }

        /// <summary>
        /// Implement the RaisePostBackEvent method 
        /// from the IPostBackEventHandler interface. 
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaisePostBackEvent(string eventArgument)
        {
            //Trace.WriteLine("RaisingPostBackEvent: eventArgument = '" + eventArgument + "'");

            if (ChangeLanguageAction == LanguageSwitcherAction.LinkRedirect)
            {
                Context.Response.Redirect(GetLangUrl(eventArgument));
            }
            else
            {
                LanguageCultureItem myItem = LanguageList.GetBestMatching(new CultureInfo(eventArgument));
                OnChangeLanguage(new LanguageSwitcherEventArgs(myItem));
            }
        }

        #endregion

        #region Private Implementation

        private TableCell GetFlagCell(LanguageCultureItem l)
        {
            TableCell myTableCellFlag = new TableCell();

            if (l.UICulture.ToString() == GetCurrentLanguage().UICulture.ToString())
                myTableCellFlag.CssClass = "rb_LangSw_sel"; // TODO: not hardcode
            else
                myTableCellFlag.CssClass = "rb_LangSw_tbl"; // TODO: not hardcode

            HyperLink myImage = new HyperLink();
            myImage.NavigateUrl = GetLangUrl(l.UICulture.Name);
            myImage.ImageUrl = GetFlagImg(l);
            myImage.Text = GetName(l);
            myTableCellFlag.Controls.Add(myImage);
            return myTableCellFlag;
        }

        private TableCell GetLabelCell(LanguageCultureItem l)
        {
            TableCell myTableCellLabel = new TableCell();

            if (l.UICulture.ToString() == GetCurrentLanguage().UICulture.ToString())
                myTableCellLabel.CssClass = "rb_LangSw_sel"; // TODO: not hardcode
            else
                myTableCellLabel.CssClass = "rb_LangSw_tbl"; // TODO: not hardcode

            HyperLink myLabel = new HyperLink();
            myLabel.NavigateUrl = GetLangUrl(l.UICulture.Name);
            myLabel.Text = GetName(l);
            myTableCellLabel.Controls.Add(myLabel);

            return myTableCellLabel;
        }

        private string GetFlagImg(LanguageCultureItem languageItem)
        {
            CultureInfo myCulture;

            switch (Flags)
            {
                default:
                case LanguageSwitcherDisplay.DisplayNone:
                    return string.Empty;

                case LanguageSwitcherDisplay.DisplayUICultureList:
                    myCulture = languageItem.UICulture;
                    break;

                case LanguageSwitcherDisplay.DisplayCultureList:
                    myCulture = languageItem.Culture;
                    break;
            }

            //Flag must be specific
            if (myCulture.IsNeutralCulture)
                myCulture = CultureInfo.CreateSpecificCulture(myCulture.Name);

            string flagImgUrl;
            if (myCulture.Name.Length > 0)
                flagImgUrl = ImagePath + "flags_" + myCulture.Name + ".gif";
            else
                flagImgUrl = ImagePath + "flags_unknown.gif";

            return flagImgUrl;
        }

        /// <summary>
        /// Used by flags and label
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private string GetLangUrl(string language)
        {
            if (ChangeLanguageAction == LanguageSwitcherAction.LinkRedirect)
                return ChangeLanguageUrl + "?lang=" + language; //TODO replace lang if present
            else
                return "javascript:" + Page.GetPostBackEventReference(this, language);
        }

        /// <summary>
        /// Used by dropdownlist
        /// </summary>
        /// <returns></returns>
        private string GetLangAction()
        {
            return Page.GetPostBackEventReference(this);
        }

        private string GetName(LanguageCultureItem languageItem)
        {
            CultureInfo myCulture;

            switch (Labels)
            {
                default:
                case LanguageSwitcherDisplay.DisplayNone:
                    return string.Empty;

                case LanguageSwitcherDisplay.DisplayUICultureList:
                    myCulture = languageItem.UICulture;
                    break;

                case LanguageSwitcherDisplay.DisplayCultureList:
                    myCulture = languageItem.Culture;
                    break;
            }

            switch (ShowNameAs)
            {
                default:
                case LanguageSwitcherName.NativeName:
                    return languageItem.Culture.TextInfo.ToTitleCase(myCulture.NativeName);

                case LanguageSwitcherName.DisplayName:
                    return languageItem.Culture.TextInfo.ToTitleCase(myCulture.DisplayName);

                case LanguageSwitcherName.EnglishName:
                    return languageItem.Culture.TextInfo.ToTitleCase(myCulture.EnglishName);
            }
        }

        #endregion

        #region Static Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langItem"></param>
        public static void SetCurrentLanguage(LanguageCultureItem langItem)
        {
            SetCurrentLanguage(langItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langItem"></param>
        /// <param name="cookieAlias">Cookie name used for persist Language</param>
        public static void SetCurrentLanguage(LanguageCultureItem langItem, string cookieAlias)
        {
            SetCurrentLanguage(langItem, cookieAlias, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langItem"></param>
        /// <param name="cookieAlias">Cookie name used for persist Language</param>
        /// <param name="switcher"></param>
        internal static void SetCurrentLanguage(LanguageCultureItem langItem, string cookieAlias,
                                                LanguageSwitcher switcher)
        {
            Thread.CurrentThread.CurrentUICulture = langItem.UICulture;
            Thread.CurrentThread.CurrentCulture = langItem.Culture;

            //Persists choice
            InternalSetViewState(langItem, switcher);
            InternalSetCookie(langItem, cookieAlias);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static LanguageCultureItem GetCurrentLanguage()
        {
            return new LanguageCultureItem(Thread.CurrentThread.CurrentUICulture, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Get current Language from Querystring
        /// </summary>
        /// <param name="myLanguagesCultureList"></param>
        /// <returns></returns>
        private static LanguageCultureItem InternalGetQuerystring(LanguageCultureCollection myLanguagesCultureList)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Params["Lang"] != null &&
                HttpContext.Current.Request.Params["Lang"].Length > 0)
            {
                try
                {
                    return
                        myLanguagesCultureList.GetBestMatching(
                            new CultureInfo(HttpContext.Current.Request.Params["Lang"]));
                }
                catch (ArgumentException) //Maybe an invalid CultureInfo
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Get current Language from User language list
        /// </summary>
        /// <param name="myLanguagesCultureList"></param>
        /// <returns></returns>
        private static LanguageCultureItem InternalGetUserLanguages(LanguageCultureCollection myLanguagesCultureList)
        {
            //Get userLangs
            CultureInfo[] userLangs;

            if (HttpContext.Current != null && HttpContext.Current.Request.UserLanguages != null &&
                HttpContext.Current.Request.UserLanguages.Length > 0)
            {
                ArrayList arrUserLangs = new ArrayList(HttpContext.Current.Request.UserLanguages);
                if (arrUserLangs.Count > 0)
                {
                    for (Int32 i = 0; i <= arrUserLangs.Count - 1; i++)
                    {
                        string currentLanguage;
                        if (arrUserLangs[i].ToString().IndexOf(';') >= 0)
                            currentLanguage =
                                arrUserLangs[i].ToString().Substring(0, arrUserLangs[i].ToString().IndexOf(';'));
                        else
                            currentLanguage = arrUserLangs[i].ToString();
                        try
                        {
                            // We try the full one... if this fails we catch it
                            arrUserLangs[i] = new CultureInfo(currentLanguage);
                        }
                        catch (ArgumentException)
                        {
                            try
                            {
                                // Some browsers can send an invalid language
                                // we try to get first two letters.. this is usually valid
                                arrUserLangs[i] = new CultureInfo(currentLanguage.Substring(2));
                            }
                            catch (ArgumentException)
                            {
                            }
                            return null;
                        }
                    }
                }
                userLangs = (CultureInfo[]) arrUserLangs.ToArray(typeof (CultureInfo));

                // Try to match browser "accept languages" list
                return myLanguagesCultureList.GetBestMatching(userLangs);
            }
            return null;
        }

        /// <summary>
        /// Get current Language from Cookie
        /// </summary>
        /// <param name="myLanguagesCultureList"></param>
        /// <param name="cookieAlias"></param>
        /// <returns></returns>
        private static LanguageCultureItem InternalGetCookie(LanguageCultureCollection myLanguagesCultureList,
                                                             string cookieAlias)
        {
            if (HttpContext.Current != null && cookieAlias != null &&
                HttpContext.Current.Request.Cookies[SWITCHER_COOKIE_PREFIX + cookieAlias] != null &&
                HttpContext.Current.Request.Cookies[SWITCHER_COOKIE_PREFIX + cookieAlias].Value.Length > 0)
            {
                try
                {
                    return
                        myLanguagesCultureList.GetBestMatching(
                            new CultureInfo(
                                HttpContext.Current.Request.Cookies[SWITCHER_COOKIE_PREFIX + cookieAlias].Value));
                }
                catch (ArgumentException)
                {
                    //Maybe an invalid CultureInfo
                }
            }
            return null;
        }

        /// <summary>
        /// Set current Cookie from Language
        /// </summary>
        /// <param name="myLanguageCultureItem"></param>
        /// <param name="cookieAlias"></param>
        /// <returns></returns>
        private static void InternalSetCookie(LanguageCultureItem myLanguageCultureItem, string cookieAlias)
        {
            // Set language cookie  --- hack.. do not set cookie if cookieAlias is languageSwitcher
            if (HttpContext.Current != null && cookieAlias != null && cookieAlias != SWITCHER_COOKIE_NAME)
            {
                //Trace.WriteLine("Persisting in cookie: '" + SWITCHER_COOKIE_PREFIX + cookieAlias + "'");
                HttpCookie langCookie = HttpContext.Current.Response.Cookies[SWITCHER_COOKIE_PREFIX + cookieAlias];
                langCookie.Value = myLanguageCultureItem.UICulture.Name;
                langCookie.Path = "/";

                // Keep the cookie?
                //if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    langCookie.Expires = DateTime.Now.AddYears(50);
            }
        }

        /// <summary>
        /// Get current Language from ViewState
        /// </summary>
        /// <param name="myLanguagesCultureList"></param>
        /// <returns></returns>
        private static LanguageCultureItem InternalGetViewState(LanguageCultureCollection myLanguagesCultureList,
                                                                LanguageSwitcher switcher)
        {
            if (switcher != null && switcher.ViewState["RB_Language_CurrentUICulture"] != null &&
                switcher.ViewState["RB_Language_CurrentCulture"] != null)
                return
                    new LanguageCultureItem((CultureInfo) switcher.ViewState["RB_Language_CurrentUICulture"],
                                            (CultureInfo) switcher.ViewState["RB_Language_CurrentCulture"]);
            else
                return null;
        }

        /// <summary>
        /// Get current Language from ViewState
        /// </summary>
        /// <param name="myLanguageCultureItem"></param>
        /// <returns></returns>
        private static void InternalSetViewState(LanguageCultureItem myLanguageCultureItem, LanguageSwitcher switcher)
        {
            if (switcher != null)
            {
                //Trace.WriteLine("Persisting in viewstate");
                switcher.ViewState["RB_Language_CurrentUICulture"] = myLanguageCultureItem.UICulture;
                switcher.ViewState["RB_Language_CurrentCulture"] = myLanguageCultureItem.Culture;
            }
        }

        /// <summary>
        /// Get default
        /// </summary>
        /// <param name="myLanguagesCultureList"></param>
        /// <returns></returns>
        private static LanguageCultureItem InternalGetDefault(LanguageCultureCollection myLanguagesCultureList)
        {
            return myLanguagesCultureList[0];
        }

        /// <summary>
        /// Examines/combines all the variables involved and sets
        /// CurrentUICulture and CurrentCulture
        /// </summary>
        /// <param name="langList">Languages list. Something like it=it-IT;en=en-US</param>
        public static void ProcessCultures(string langList)
        {
            ProcessCultures(langList, null);
        }

        /// <summary>
        /// Examines/combines all the variables involved and sets
        /// CurrentUICulture and CurrentCulture
        /// </summary>
        /// <param name="langList">Languages list. Something like it=it-IT;en=en-US</param>
        /// <param name="cookieAlias">Alias used to make this cookie unique. Use null is you do not want cookies.</param>
        public static void ProcessCultures(string langList, string cookieAlias)
        {
            ProcessCultures(langList, cookieAlias, null);
        }

        /// <summary>
        /// Examines/combines all the variables involved and sets
        /// CurrentUICulture and CurrentCulture
        /// </summary>
        /// <param name="langList">Languages list. Something like it=it-IT;en=en-US</param>
        /// <param name="cookieAlias">Alias used to make this cookie unique. Use null is you do not want cookies.</param>
        /// <param name="switcher">A referenct to a Switcher control for accessing viewstate</param>
        internal static void ProcessCultures(string langList, string cookieAlias, LanguageSwitcher switcher)
        {
            LanguageCultureCollection myLanguagesCultureList = (LanguageCultureCollection) langList;

            //Verify that at least on language is provided
            if (myLanguagesCultureList.Count <= 0)
                throw new ArgumentException("Please provide at least one language in the list.", "langList");

            // Language Item
            LanguageCultureItem langItem;

            // Querystring
            langItem = InternalGetQuerystring(myLanguagesCultureList);
            //Trace.WriteLine("Evaluated InternalGetQuerystring: '" + (langItem == null ? "null" : langItem) + "'");
            if (langItem != null) goto setLanguage;

            // Viewstate
            langItem = InternalGetViewState(myLanguagesCultureList, switcher);
            //Trace.WriteLine("Evaluated InternalGetViewState: '" + (langItem == null ? "null" : langItem) + "'");
            if (langItem != null) goto setLanguage;

            // Cookie
            langItem = InternalGetCookie(myLanguagesCultureList, cookieAlias);
            //Trace.WriteLine("Evaluated InternalGetCookie: '" + (langItem == null ? "null" : langItem) + "'");
            if (langItem != null) goto setLanguage;

            // UserLanguageList
            langItem = InternalGetUserLanguages(myLanguagesCultureList);
            //Trace.WriteLine("Evaluated InternalGetUserLanguages: '" + (langItem == null ? "null" : langItem) + "'");
            if (langItem != null) goto setLanguage;

            // Default
            langItem = InternalGetDefault(myLanguagesCultureList);
            //Trace.WriteLine("Evaluated InternalGetDefault: '" + (langItem == null ? "null" : langItem) + "'");

            setLanguage:
            // Updates current cultures
            SetCurrentLanguage(langItem, cookieAlias);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Normally the Language part is shown as Native name (the name in the proper language).
        /// Set ShowNative to false for showing names in english.
        /// </summary>
        [DefaultValue(LanguageSwitcherName.NativeName)]
        public LanguageSwitcherName ShowNameAs
        {
            get
            {
                if (ViewState["ShowNameAs"] != null)
                    return (LanguageSwitcherName) ViewState["ShowNameAs"];
                return LanguageSwitcherName.NativeName;
            }
            set
            {
                ViewState["ShowNameAs"] = value;
                ChildControlsCreated = false;
                //EnsureChildControls();
            }
        }

        /// <summary>
        /// Normally the Language part is shown (UI).
        /// Choose DisplayCultureList to show the Culture part.
        /// Choose DisplayNone for hide labels.
        /// </summary>
        [DefaultValue(LanguageSwitcherDisplay.DisplayUICultureList)]
        public LanguageSwitcherDisplay Labels
        {
            get
            {
                if (ViewState["Labels"] != null)
                    return (LanguageSwitcherDisplay) ViewState["Labels"];
                return LanguageSwitcherDisplay.DisplayUICultureList;
            }
            set
            {
                ViewState["Labels"] = value;
                ChildControlsCreated = false;
                //EnsureChildControls();
            }
        }


        /// <summary>
        /// Normally the Language part is shown (UI).
        /// Choose DisplayCultureList to show the Culture part.
        /// Choose DisplayNone for hide Flags.
        /// </summary>
        [DefaultValue(LanguageSwitcherDisplay.DisplayCultureList)]
        public LanguageSwitcherDisplay Flags
        {
            get
            {
                if (ViewState["Flags"] != null)
                    return ((LanguageSwitcherDisplay) ViewState["Flags"]);
                return LanguageSwitcherDisplay.DisplayCultureList;
            }
            set
            {
                ViewState["Flags"] = value;
                ChildControlsCreated = false;
                //EnsureChildControls();
            }
        }

        /// <summary>
        /// LanguageSwitcher Type
        /// </summary>
        [DefaultValue(LanguageSwitcherType.DropDownList)]
        public LanguageSwitcherType Type
        {
            get
            {
                if (ViewState["Type"] != null)
                    return (LanguageSwitcherType) ViewState["Type"];
                return LanguageSwitcherType.DropDownList;
            }
            set
            {
                ViewState["Type"] = value;
                ChildControlsCreated = false;
                //EnsureChildControls();
            }
        }

        /// <summary>
        /// Image path
        /// </summary>
        [DefaultValue("images/flags/")]
        public string ImagePath
        {
            get
            {
                string imagePath = ((string) ViewState["ImagePath"]);
                if (imagePath != null)
                    return imagePath;
                return "images/flags/"; //TODO: point to aspnet
            }
            set { ViewState["ImagePath"] = value; }
        }

        /// <summary>
        /// Url where redirecting language changes.
        /// An empty walue reload current page.
        /// </summary>
        [DefaultValue("")]
        public string ChangeLanguageUrl
        {
            get
            {
                string changeLanguageUrl = ((string) ViewState["ChangeLanguageUrl"]);
                if (changeLanguageUrl != null)
                    return changeLanguageUrl;
                return string.Empty;
            }
            set { ViewState["ChangeLanguageUrl"] = value; }
        }

        /// <summary>
        /// Choose how language switcher change language.
        /// </summary>
        /// <remarks>
        /// In LanguageSwitcherAction.LinkRedirect mode 
        /// NO EVENT is raised because no posback occurs.
        /// </remarks>
        [DefaultValue(LanguageSwitcherAction.PostBack)]
        public LanguageSwitcherAction ChangeLanguageAction
        {
            get
            {
                if (ViewState["ChangeLanguageAction"] != null)
                    return (LanguageSwitcherAction) ViewState["ChangeLanguageAction"];
                return LanguageSwitcherAction.PostBack;
            }
            set { ViewState["ChangeLanguageAction"] = value; }
        }

        //		private LanguageCultureCollection LanguageList;
        //		/// <summary>
        //		/// LanguageCultureCollection
        //		/// </summary>
        //		[DefaultValue("en=en-US;it=it-IT")]
        //		[PersistenceMode(PersistenceMode.Attribute)]
        //		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //		[TypeConverter(typeof(TypeConverterLanguageCultureCollection))]
        //		[Description("The language list.")]
        //		[MergableProperty(false)]
        private LanguageCultureCollection LanguageList
        {
            get { return (LanguageCultureCollection) LanguageListString; }
            //			set
            //			{
            //				Trace.WriteLine("LanguageList");
            //				LanguageListString = (string) value;
            //			}
        }

        //
        //		public void SetLanguagesCultureList(string languageList)
        //		{
        //			m_languageList = (LanguageCultureCollection) languageList;
        //			ChildControlsCreated = false;
        //			EnsureChildControls();
        //		}

        //		private string strLanguageList = "en=en-US;it=it-IT";
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("en=en-US;it=it-IT")]
        [PersistenceMode(PersistenceMode.Attribute)]
        public string LanguageListString
        {
            get
            {
                if (ViewState["LanguageList"] != null)
                    return (string) ViewState["LanguageList"];
                return Config.DefaultLanguageList;
                //return strLanguageList;
            }
            set
            {
                //strLanguageList = value;
                ViewState["LanguageList"] = (string) value;
                ChildControlsCreated = false;
                //EnsureChildControls();
            }
        }

        #endregion
    }
}