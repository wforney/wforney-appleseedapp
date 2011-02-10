// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlEditorDataType.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   List of available HTML editors
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.DataTypes
{
    using System.Collections;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web.UI.WebControls;
    using Appleseed.Framework.UI.WebControls.CodeMirror;
    using Appleseed.Framework.UI.WebControls.TinyMCE;

    using FreeTextBoxControls;

    using Syrinx.Gui.AspNet;

    using FreeTextBox = Appleseed.Framework.Web.UI.WebControls.FreeTextBox;

    /// <summary>
    /// List of available HTML editors
    /// </summary>
    public class HtmlEditorDataType : ListDataType<string, DropDownList>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "HtmlEditorDataType" /> class.
        /// </summary>
        public HtmlEditorDataType()
        {
            this.Type = PropertiesDataType.List;
            this.InitializeComponents();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the data source.
        /// </summary>
        public override object DataSource
        {
            get
            {
                return "Plain Text;FCKeditor;SyrinxCkEditor;FreeTextBox".Split(';');
            }
        }

        /// <summary>
        ///   Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "HtmlEditor List";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// HTMLs the editor settings.
        /// </summary>
        /// <param name="editorSettings">
        /// The editor settings.
        /// </param>
        /// <param name="group">
        /// The group.
        /// </param>
        public static void HtmlEditorSettings(Hashtable editorSettings, SettingItemGroup group)
        {
            var pS = (PortalSettings)HttpContext.Current.Items["PortalSettings"];

            var editor = new SettingItem<string, DropDownList>(new HtmlEditorDataType())
                {
                    // 1; modified by Hongwei Shen(hongwei.shen@gmail.com) 11/9/2005
                    Order = (int)group + 1, 
                    Group = group, 
                    EnglishName = "Editor", 
                    Description = "Select the Html Editor for Module"
                };

            var controlWidth = new SettingItem<int, TextBox>(new IntegerDataType())
                {
                    Value = 700,
                    // 2; modified by Hongwei Shen
                    Order = (int)group + 2, 
                    Group = group, 
                    EnglishName = "Editor Width", 
                    Description = "The width of editor control"
                };

            var controlHeight = new SettingItem<int, TextBox>(new IntegerDataType())
                {
                    Value = 400, 
                    // 3; modified by Hongwei Shen
                    Order = (int)group + 3, 
                    Group = group, 
                    EnglishName = "Editor Height", 
                    Description = "The height of editor control"
                };

            var showUpload = new SettingItem<bool, CheckBox>(new BooleanDataType())
                {
                    Value = true, 
                    // 4;  modified by Hongwei Shen
                    Order = (int)group + 4, 
                    Group = group, 
                    EnglishName = "Upload?", 
                    Description = "Only used if Editor is ActiveUp HtmlTextBox"
                };

            SettingItem<string, Panel> moduleImageFolder = null;
            if (pS != null)
            {
                if (pS.PortalFullPath != null)
                {
                    moduleImageFolder =
                        new SettingItem<string, Panel>(
                            new FolderDataType(
                                HttpContext.Current.Server.MapPath(string.Format("{0}/images", pS.PortalFullPath)), 
                                "default"))
                            {
                                Value = "default", 
                                // 5;  modified by Hongwei Shen
                                Order = (int)group + 5, 
                                Group = group, 
                                EnglishName = "Default Image Folder", 
                                Description =
                                    "This folder is used for editor in this module to take and upload images"
                            };
                }

                // Set the portal default values
                if (pS.CustomSettings != null)
                {
                    if (pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"] != null)
                    {
                        editor.Value = (string)pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"];
                    }

                    if (pS.CustomSettings["SITESETTINGS_EDITOR_WIDTH"] != null)
                    {
                        controlWidth.Value = (int)pS.CustomSettings["SITESETTINGS_EDITOR_WIDTH"];
                    }

                    if (pS.CustomSettings["SITESETTINGS_EDITOR_HEIGHT"] != null)
                    {
                        controlHeight.Value = (int)pS.CustomSettings["SITESETTINGS_EDITOR_HEIGHT"];
                    }

                    if (pS.CustomSettings["SITESETTINGS_EDITOR_HEIGHT"] != null)
                    {
                        controlHeight.Value = (int)pS.CustomSettings["SITESETTINGS_EDITOR_HEIGHT"];
                    }

                    if (pS.CustomSettings["SITESETTINGS_SHOWUPLOAD"] != null)
                    {
                        showUpload.Value = (bool)pS.CustomSettings["SITESETTINGS_SHOWUPLOAD"];
                    }

                    if (pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null)
                    {
                        if (moduleImageFolder != null)
                        {
                            moduleImageFolder.Value = (string)pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"];
                        }
                    }
                }
            }

            editorSettings.Add("Editor", editor);
            editorSettings.Add("Width", controlWidth);
            editorSettings.Add("Height", controlHeight);
            editorSettings.Add("ShowUpload", showUpload);
            if (moduleImageFolder != null)
            {
                editorSettings.Add("MODULE_IMAGE_FOLDER", moduleImageFolder);
            }
        }

        /// <summary>
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            // Default
            Value = "FreeTextBox";
            // Change the default value to Portal Default Editor Value by jviladiu@portalServices.net 13/07/2004

            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                if (pS.CustomSettings != null)
                {
                    if (pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"] != null)
                        Value = pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"].ToString();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public override object DataSource
        {
            get { return "Code Mirror Plain Text;TinyMCE Editor;FCKeditor;Syrinx CkEditor;FreeTextBox".Split(';'); }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "HtmlEditor List"; }
        }

        /// <summary>
        /// Gets the FTB language.
        /// </summary>
        /// <param name="AppleseedLanguage">The Appleseed language.</param>
        /// <returns></returns>
        private static string getFtbLanguage(string AppleseedLanguage)
        {
            switch (AppleseedLanguage.Substring(AppleseedLanguage.Length - 2).ToLower())
            {
                case "en":
                    return "en-US";
                case "us":
                    return "en-US";
                case "es":
                    return "es-ES";
                case "cn":
                    return "zh-cn";
                case "cz":
                    return "cz-CZ";
                case "fi":
                    return "fi-fi";
                case "nl":
                    return "nl-NL";
                case "de":
                    return "de-de";
                case "il":
                    return "he-IL";
                case "it":
                    return "it-IT";
                case "jp":
                    return "ja-JP";
                case "kr":
                    return "ko-kr";
                case "no":
                    return "nb-NO";
                case "pt":
                    return "pt-pt";
                case "ro":
                    return "ro-RO";
                case "ru":
                    return "ru-ru";
                case "se":
                    return "sv-se";
                case "tw":
                    return "zh-TW";
                default:
                    return "en-US";
            }
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <param name="placeHolderHtmlEditor">
        /// The place holder HTML editor.
        /// </param>
        /// <param name="moduleId">
        /// The module ID.
        /// </param>
        /// <param name="showUpload">
        /// if set to <c>true</c> [show upload].
        /// </param>
        /// <param name="portalSettings">
        /// The portal settings.
        /// </param>
        /// <returns>
        /// An html editor interface.
        /// </returns>
        /// </summary>
        /// <param name="PlaceHolderHTMLEditor">The place holder HTML editor.</param>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="showUpload">if set to <c>true</c> [show upload].</param>
        /// <param name="portalSettings">The portal settings.</param>
        /// <returns></returns>
        public IHtmlEditor GetEditor(Control PlaceHolderHTMLEditor, int moduleID, bool showUpload,
                                     PortalSettings portalSettings)
        {
            IHtmlEditor DesktopText;
            string moduleImageFolder = ModuleSettings.GetModuleSettings(moduleID)["MODULE_IMAGE_FOLDER"].ToString();

            // Grabs ID from the place holder so that a unique editor is on the page if more than one
            // But keeps same ID so that the information can be submitted to be saved. [CDT]
            string uniqueID = PlaceHolderHTMLEditor.ID;

            switch (Value)
            {
                case "TinyMCE Editor":
                    var tinyMCE = new TinyMCETextBox();
                    tinyMCE.ImageFolder = moduleImageFolder;
                    DesktopText = tinyMCE;
                    break;

                case "FCKeditor": // 9/8/2010
                    FCKTextBoxV2 fckv2 = new FCKTextBoxV2();
                    fckv2.ImageFolder = moduleImageFolder;
                    fckv2.BasePath = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/FCKeditorV2.6.6/");
                    fckv2.AutoDetectLanguage = false;
                    fckv2.DefaultLanguage = portalSettings.PortalUILanguage.Name.Substring(0, 2);
//					fckv2.EditorAreaCSS = portalSettings.GetCurrentTheme().CssFile;
                    fckv2.ID = string.Concat("FCKTextBox", uniqueID);
                    string conector = Path.ApplicationRootPath("/app_support/FCKconnectorV2.aspx");
                    fckv2.ImageBrowserURL =
                        Path.WebPathCombine(Path.ApplicationRoot,
                                            "aspnet_client/FCKeditorV2.6.6/editor/filemanager/browser/default/" +
                                            "browser.html?Type=Image%26Connector=" + conector);
                    fckv2.LinkBrowserURL =
                        Path.WebPathCombine(Path.ApplicationRoot,
                                            "aspnet_client/FCKeditorV2.6.6/editor/filemanager/browser/default/" +
                                            "browser.html?Connector=" + conector);
                    DesktopText = ((IHtmlEditor) fckv2);
                    break;


                case "Syrinx CkEditor":
                    SyrinxCkTextBox.CkEditorJS = Path.WebPathCombine(Path.ApplicationRoot,
                                            "aspnet_client/ckeditor/ckeditor.js");
                    
                    SyrinxCkTextBox sckvtb = new SyrinxCkTextBox();
                    sckvtb.ImageFolder = moduleImageFolder;
                    sckvtb.BaseContentUrl = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/ckeditor/");
                    sckvtb.Resizable = false;
                    sckvtb.Language = portalSettings.PortalUILanguage.TwoLetterISOLanguageName;

                    DesktopText = ((IHtmlEditor)sckvtb);
                    break;

                case "FreeTextBox":
                    FreeTextBox freeText = new FreeTextBox();
                    //freeText.ToolbarLayout =
                    //    "ParagraphMenu,FontFacesMenu,FontSizesMenu,FontForeColorPicker,FontBackColorPicker,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat;CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;InsertRule|Delete,Cut,Copy,Paste;Undo,Redo,Print;InsertTable,InsertTableColumnAfter,InsertTableColumnBefore,InsertTableRowAfter,InsertTableRowBefore,DeleteTableColumn,DeleteTableRow,InsertImageFromGallery";
                    freeText.ImageGalleryUrl =
                        Path.WebPathCombine(Path.ApplicationFullPath,
                                            "app_support/ftb.imagegallery.aspx?rif={0}&cif={0}&mID=" +
                                            moduleID.ToString());
                    freeText.ImageFolder = moduleImageFolder;
                    freeText.ImageGalleryPath = Path.WebPathCombine(portalSettings.PortalFullPath, freeText.ImageFolder);
                    freeText.ID = string.Concat("FreeText", uniqueID);
                    freeText.Language = getFtbLanguage(portalSettings.PortalUILanguage.Name);
                    freeText.JavaScriptLocation = FreeTextBoxControls.ResourceLocation.ExternalFile;
                    freeText.ButtonImagesLocation = FreeTextBoxControls.ResourceLocation.ExternalFile;
                    freeText.ToolbarImagesLocation = FreeTextBoxControls.ResourceLocation.ExternalFile;
                    freeText.SupportFolder = Path.WebPathCombine(Path.ApplicationFullPath,"aspnet_client/FreeTextBox");

                    DesktopText = ((IHtmlEditor) freeText);
                    break;

                case "Code Mirror Plain Text":
                default: 
                    var codeMirrorTextBox = new CodeMirrorTextBox();
                    DesktopText = codeMirrorTextBox;
                    break;
            }
            PlaceHolderHTMLEditor.Controls.Add(((Control) DesktopText));
            return DesktopText;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the components.
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Default
            this.Value = "FreeTextBox";

            // Change the default value to Portal Default Editor Value by jviladiu@portalServices.net 13/07/2004
            if (HttpContext.Current == null || HttpContext.Current.Items["PortalSettings"] == null)
            {
                return;
            }

            var pS = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (pS.CustomSettings == null)
            {
                return;
            }

            if (pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"] != null)
            {
                this.Value = pS.CustomSettings["SITESETTINGS_DEFAULT_EDITOR"].ToString();
            }
        }

        /// <summary>
        /// Gets the FTB language.
        /// </summary>
        /// <param name="appleseedLanguage">
        /// The Appleseed language.
        /// </param>
        /// <returns>
        /// The language.
        /// </returns>
        private static string GetFtbLanguage(string appleseedLanguage)
        {
            switch (appleseedLanguage.Substring(appleseedLanguage.Length - 2).ToLower())
            {
                case "en":
                    return "en-US";
                case "us":
                    return "en-US";
                case "es":
                    return "es-ES";
                case "cn":
                    return "zh-cn";
                case "cz":
                    return "cz-CZ";
                case "fi":
                    return "fi-fi";
                case "nl":
                    return "nl-NL";
                case "de":
                    return "de-de";
                case "il":
                    return "he-IL";
                case "it":
                    return "it-IT";
                case "jp":
                    return "ja-JP";
                case "kr":
                    return "ko-kr";
                case "no":
                    return "nb-NO";
                case "pt":
                    return "pt-pt";
                case "ro":
                    return "ro-RO";
                case "ru":
                    return "ru-ru";
                case "se":
                    return "sv-se";
                case "tw":
                    return "zh-TW";
                default:
                    return "en-US";
            }
        }

        #endregion
    }
}