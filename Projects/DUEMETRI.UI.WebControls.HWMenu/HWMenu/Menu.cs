using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Drawing;
using System.Configuration;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DUEMETRI.UI.WebControls.HWMenu
{
    /// <summary>
    /// HWMenu WebControl.
    /// </summary>
    /// 	[History("mario@hartmann.net", "2003/06/14", "use of named CSS styles added")]

    [
    Designer("DUEMETRI.UI.WebControls.HWMenu.Design.MenuDesigner"),
    ParseChildren(true)
    ]
    public class Menu : WebControl
    {
        #region Private implementation
        MenuTreeNodes _childs;
        Style _controlItemStyle;
        Style _controlSubStyle;
        Style _controlHiStyle;
        Style _controlHiSubStyle;

        void Initialize()
        {
            _childs = new MenuTreeNodes();

            _controlItemStyle = new Style();
            _controlSubStyle = new Style();
            _controlHiStyle = new Style();
            _controlHiSubStyle = new Style();


            CssClass = "";
            _controlItemStyle.CssClass = "";
            _controlSubStyle.CssClass = "";
            _controlHiStyle.CssClass = "";
            _controlHiSubStyle.CssClass = "";

            BackColor = Color.White;
            _controlItemStyle.BackColor = Color.White;
            _controlSubStyle.BackColor = Color.White;
            _controlHiStyle.BackColor = Color.Black;
            _controlHiSubStyle.BackColor = Color.Black;

            ForeColor = Color.Black;
            _controlItemStyle.ForeColor = Color.Black;
            _controlSubStyle.ForeColor = Color.Black;
            _controlHiStyle.ForeColor = Color.White;
            _controlHiSubStyle.ForeColor = Color.White;

            BorderColor = Color.Black;
            _controlItemStyle.BorderColor = Color.Black;
            _controlSubStyle.BorderColor = Color.Black;

            BorderWidth = new Unit(1);

            ControlStyle.Width = new Unit(100);
            ControlStyle.Height = new Unit(20);

            Font.Names = new string[2] { "Arial", "sans-serif" };
            Font.Size = new FontUnit(9);
            Font.Bold = true;
            Font.Italic = false;

            _arrws = new System.Web.UI.WebControls.Image[3];

            _arrws[0] = new System.Web.UI.WebControls.Image();
            _arrws[0].ImageUrl = "tri.gif";
            _arrws[0].Width = 5;
            _arrws[0].Height = 10;

            _arrws[1] = new System.Web.UI.WebControls.Image();
            _arrws[1].ImageUrl = "tridown.gif";
            _arrws[1].Width = 10;
            _arrws[1].Height = 5;

            _arrws[2] = new System.Web.UI.WebControls.Image();
            _arrws[2].ImageUrl = "trileft.gif";
            _arrws[2].Width = 5;
            _arrws[2].Height = 10;

        }
        #endregion


        /// <summary>
        /// Style of this element when the mouse is not over the element.
        /// </summary>
        [
        Category("Style"),
        Description("Style of this element when the mouse is over the element."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        ]
        public Style ControlItemStyle
        {
            get
            {
                if (_controlItemStyle == null)
                {
                    _controlItemStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_controlItemStyle).TrackViewState();
                }
                return _controlItemStyle;
            }
            set
            {
                _controlItemStyle = value;
            }
        }

        /// <summary>
        /// Style of this element when the mouse is over the element.
        /// </summary>
        [
        Category("Style"),
        Description("Style of this element when the mouse is over the element."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public Style ControlHiStyle
        {
            get
            {
                if (_controlHiStyle == null)
                {
                    _controlHiStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_controlHiStyle).TrackViewState();
                }
                return _controlHiStyle;
            }
            set
            {
                _controlHiStyle = value;
            }
        }

        /// <summary>
        /// Style of tSubs element when the mouse is not over the element.
        /// </summary>
        [
        Category("Style"),
        Description("Style of tSubs element when the mouse is over the element."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        ]
        public Style ControlSubStyle
        {
            get
            {
                if (_controlSubStyle == null)
                {
                    _controlSubStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_controlSubStyle).TrackViewState();
                }
                return _controlSubStyle;
            }
            set
            {
                _controlSubStyle = value;
            }
        }

        /// <summary>
        /// Style of the Subs element when the mouse is over the element.
        /// </summary>
        [
        Category("Style"),
        Description("Style of the Subs element when the mouse is over the element."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        ]
        public Style ControlHiSubStyle
        {
            get
            {
                if (_controlHiSubStyle == null)
                {
                    _controlHiSubStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_controlHiSubStyle).TrackViewState();
                }
                return _controlHiSubStyle;
            }
            set
            {
                _controlHiSubStyle = value;
            }
        }

        /// <summary>
        /// Border between elements
        /// </summary>
        public bool BorderBtwnElmnts = true;
        /// <summary>
        /// Item text position 'left', 'center' or 'right'
        /// </summary>
        public HorizontalAlign MenuTextCentered = HorizontalAlign.Left;
        /// <summary>
        /// Menu horizontal position 'left', 'center' or 'right'
        /// </summary>
        public HorizontalAlign MenuCentered = HorizontalAlign.Left;
        /// <summary>
        /// Menu vertical position 'top', 'middle','bottom' or 'static'
        /// </summary>
        public VerticalAlign MenuVerticalCentered = VerticalAlign.Top;
        /// <summary>
        /// horizontal overlap child/ parent
        /// </summary>
        public double ChildOverlap = .1;
        /// <summary>
        /// vertical overlap child/ parent
        /// </summary>
        public double ChildVerticalOverlap = .1;
        /// <summary>
        /// Menu offset x coordinate
        /// </summary>
        public int StartTop = 0;
        /// <summary>
        /// Menu offset y coordinate
        /// </summary>
        public int StartLeft = 0;
        /// <summary>
        /// Multiple frames y correction
        /// </summary>
        public int VerCorrect = 0;
        /// <summary>
        /// Multiple frames x correction
        /// </summary>
        public int HorCorrect = 0;
        /// <summary>
        /// Left padding
        /// </summary>
        public int LeftPaddng = 2;
        /// <summary>
        /// Top padding
        /// </summary>
        public int TopPaddng = 2;

        private bool _horizontal = true;
        /// <summary>
        /// Horizontal or vertical menu
        /// </summary>
        /// 
        [
        Category("Appearance"),
        Description("Horizontal or vertical menu.")
        ]
        public bool Horizontal
        {
            get
            {
                return _horizontal;
            }
            set
            {
                _horizontal = value;
            }
        }

        /// <summary>
        /// Frames in cols or rows 1 or 0
        /// </summary>
        public bool MenuFramesVertical = true;
        /// <summary>
        /// delay before menu folds in (milliseconds)
        /// </summary>
        public int DissapearDelay = 1000;
        /// <summary>
        /// Menu frame takes over background color subitem frame
        /// </summary>
        public bool TakeOverBgColor = true;
        /// <summary>
        /// Frame where first level appears
        /// </summary>
        public string FirstLineFrame = "";
        /// <summary>
        /// Frame where sub levels appear
        /// </summary>
        public string SecLineFrame = "";
        /// <summary>
        /// Frame where target documents appear
        /// </summary>
        public string DocTargetFrame = "_self";
        /// <summary>
        /// span id for relative positioning
        /// </summary>
        public string TargetLoc = "MenuContainer";
        /// <summary>
        /// Hide first level when loading new document 1 or 0
        /// </summary>
        public bool HideTop = false;
        /// <summary>
        /// enables/ disables menu wrap 1 or 0
        /// </summary>
        public bool MenuWrap = true;
        /// <summary>
        /// enables/ disables right to left unfold 1 or 0
        /// </summary>
        public bool RightToLeft = false;
        /// <summary>
        /// Level 1 unfolds onclick/ onmouseover
        /// </summary>
        public bool UnfoldsOnClick = false;
        /// <summary>
        /// menu tree checking on or off 1 or 0
        /// </summary>
        public bool WebMasterCheck = false;
        /// <summary>
        /// Uses arrow gifs when 1
        /// </summary>
        public bool ShowArrow = true;
        /// <summary>
        /// Keep selected path highligthed
        /// </summary>
        public bool KeepHilite = true;

        private System.Web.UI.WebControls.Image[] _arrws;

        /// <summary>
        /// Arrow image
        /// </summary>
        [
        Category("Appearance"),
        Description("Arrow image"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public System.Web.UI.WebControls.Image ArrowImage
        {
            get { return _arrws[0]; }
            set { _arrws[0] = value; }
        }

        /// <summary>
        /// Arrow image down
        /// </summary>
        [
        Category("Appearance"),
        Description("Arrow image down"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public System.Web.UI.WebControls.Image ArrowImageDown
        {
            get { return _arrws[1]; }
            set { _arrws[1] = value; }
        }

        /// <summary>
        /// Arrow image left
        /// </summary>
        [
        Category("Appearance"),
        Description("Arrow image left"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public System.Web.UI.WebControls.Image ArrowImageLeft
        {
            get { return _arrws[2]; }
            set { _arrws[2] = value; }
        }

        /// <summary>
        /// Menu items collection.
        /// </summary>
        [
        Category("Data"),
        Description("Menu items."),
        PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public MenuTreeNodes Childs
        {
            get
            {
                return (_childs);
            }
        }

        /// <summary>
        /// Client script path.
        /// </summary>
        [
        Category("Data"),
        Description("Client script path."),
        PersistenceMode(PersistenceMode.Attribute)
        ]
        public string ClientScriptPath
        {
            get
            {
                object _clientScriptPath = this.ViewState["ClientScriptPath"];
                if (_clientScriptPath != null)
                    return (String)_clientScriptPath;
                return GetClientScriptPath();
            }
            set
            {
                string _clientScriptPath = value;
                if (_clientScriptPath.Length > 0 && !_clientScriptPath.EndsWith("/"))
                    _clientScriptPath = _clientScriptPath + "/";
                this.ViewState["ClientScriptPath"] = _clientScriptPath;
            }
        }

        /// <summary>
        /// Client images path.
        /// </summary>
        [
        Category("Data"),
        Description("Client images path."),
        PersistenceMode(PersistenceMode.Attribute)
        ]
        public string ImagesPath
        {
            get
            {
                object _imagesPath = this.ViewState["ImagesPath"];
                if (_imagesPath != null)
                    return (String)_imagesPath;
                return GetClientScriptPath();
            }
            set
            {
                string _imagesPath = value;
                if (_imagesPath.Length > 0 && !_imagesPath.EndsWith("/"))
                    _imagesPath = _imagesPath + "/";
                this.ViewState["ImagesPath"] = _imagesPath;
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public Menu()
        {
            Initialize();
        }

        /// <summary>
        /// Retrieves a string representing the current menu array
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>The current menu array</returns>
        protected string ToMenuArray(string prefix)
        {
            WebColorConverter wc = new WebColorConverter();

            StringBuilder sb = new StringBuilder();

            sb.Append("<script type = 'text/javascript'>\n");
            sb.Append("  function Go(){return}\n");
            sb.Append("</script>\n");

            sb.Append("<script type = 'text/javascript'>\n");

            sb.Append("var NoOffFirstLineMenus = ");
            sb.Append(Childs.Count);
            sb.Append(";\n");

            //MH:
            sb.Append("var CssItemClassName = ");
            sb.Append("\"");
            sb.Append(ControlItemStyle.CssClass);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var CssHiClassName = ");
            sb.Append("\"");
            sb.Append(ControlHiStyle.CssClass);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var CssSubClassName = ");
            sb.Append("\"");
            sb.Append(ControlSubStyle.CssClass);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var CssHiSubClassName = ");
            sb.Append("\"");
            sb.Append(ControlHiSubStyle.CssClass);
            sb.Append("\"");
            sb.Append(";\n");

            //MH:


            sb.Append("var LowBgColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(BackColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var LowSubBgColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlSubStyle.BackColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var HighBgColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlHiStyle.BackColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var HighSubBgColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlHiSubStyle.BackColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var FontLowColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ForeColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var FontSubLowColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlSubStyle.ForeColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var FontHighColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlHiStyle.ForeColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var FontSubHighColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlHiSubStyle.ForeColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var BorderColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(BorderColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var BorderSubColor = ");
            sb.Append("\"");
            sb.Append(wc.ConvertToString(ControlSubStyle.BorderColor));
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var BorderWidth = ");
            sb.Append(BorderWidth.Value);
            sb.Append(";\n");

            sb.Append("var BorderBtwnElmnts = ");
            sb.Append(BorderBtwnElmnts ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var FontFamily = ");
            sb.Append("\"");
            sb.Append(Font.Name);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var FontSize = ");
            sb.Append(Font.Size.Unit.Value);
            sb.Append(";\n");

            sb.Append("var FontBold = ");
            sb.Append(Font.Bold ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var FontItalic = ");
            sb.Append(Font.Italic ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var MenuTextCentered = ");
            sb.Append("\"");
            sb.Append(MenuTextCentered.ToString().ToLower());
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var MenuCentered = ");
            sb.Append("\"");
            sb.Append(MenuCentered.ToString().ToLower());
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var MenuVerticalCentered = ");
            sb.Append("\"");
            sb.Append(MenuVerticalCentered.ToString().ToLower());
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var ChildOverlap = ");
            sb.Append(ChildOverlap.ToString(new CultureInfo("en-US").NumberFormat));
            sb.Append(";\n");

            sb.Append("var ChildVerticalOverlap = ");
            sb.Append(ChildVerticalOverlap.ToString(new CultureInfo("en-US").NumberFormat));
            sb.Append(";\n");

            sb.Append("var LeftPaddng = ");
            sb.Append(LeftPaddng);
            sb.Append(";\n");

            sb.Append("var TopPaddng = ");
            sb.Append(TopPaddng);
            sb.Append(";\n");

            sb.Append("var StartTop = ");
            sb.Append(StartTop);
            sb.Append(";\n");

            sb.Append("var StartLeft = ");
            sb.Append(StartLeft);
            sb.Append(";\n");

            sb.Append("var VerCorrect = ");
            sb.Append(VerCorrect);
            sb.Append(";\n");

            sb.Append("var HorCorrect = ");
            sb.Append(HorCorrect);
            sb.Append(";\n");

            sb.Append("var FirstLineHorizontal = ");
            sb.Append(Horizontal ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var MenuFramesVertical = ");
            sb.Append(MenuFramesVertical ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var DissapearDelay = ");
            sb.Append(DissapearDelay);
            sb.Append(";\n");

            sb.Append("var TakeOverBgColor = ");
            sb.Append(TakeOverBgColor ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var FirstLineFrame = ");
            sb.Append("\"");
            sb.Append(FirstLineFrame);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var SecLineFrame = ");
            sb.Append("\"");
            sb.Append(SecLineFrame);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var DocTargetFrame = ");
            sb.Append("\"");
            sb.Append(DocTargetFrame);
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var HideTop = ");
            sb.Append(HideTop ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var TargetLoc = ");
            sb.Append("\"");
            //sb.Append(TargetLoc);
            //sb.Append(this.Controls[0].ClientID);
            sb.Append("MenuPos"); //NS4 bug fix
            sb.Append("\"");
            sb.Append(";\n");

            sb.Append("var MenuWrap = ");
            sb.Append(MenuWrap ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var RightToLeft = ");
            sb.Append(RightToLeft ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var UnfoldsOnClick = ");
            sb.Append(UnfoldsOnClick ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var WebMasterCheck = ");
            sb.Append(WebMasterCheck ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var ShowArrow = ");
            sb.Append(ShowArrow ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var KeepHilite = ");
            sb.Append(KeepHilite ? 1 : 0);
            sb.Append(";\n");

            sb.Append("var Arrws = ");
            sb.Append("[");
            for (int i = 0; i <= _arrws.GetUpperBound(0); i++)
            {
                sb.Append("\"");
                sb.Append(ImagesPath + _arrws[i].ImageUrl);
                sb.Append("\", ");
                sb.Append(_arrws[i].Width.Value);
                sb.Append(", ");
                sb.Append(_arrws[i].Height.Value);
                if (i != _arrws.GetUpperBound(0))
                    sb.Append(", ");
            }
            sb.Append("]");
            sb.Append(";\n");

            sb.Append("function BeforeStart(){;}\n");
            sb.Append("function AfterBuild(){;}\n");
            sb.Append("function BeforeFirstOpen(){;}\n");
            sb.Append("function AfterCloseAll(){;}\n");

            sb.Append(Childs.ToMenuArray(prefix));

            sb.Append("</script>\n");
            sb.Append("<script type = 'text/javascript' src = '" + ClientScriptPath + "menu_com.js'></script>\n");
            sb.Append("<noscript>Your browser does not support script</noscript>\n");

            return sb.ToString();
        }

        /// <summary>
        /// Render the control
        /// </summary>
        /// <param name="output"></param>
        override protected void Render(HtmlTextWriter output)
        {
            if (!HasControls()) //HACK
                CreateChildControls(); //If on pane CreateChildControls is not fired

            if (Childs.Count > 0 && HasControls())
            {
                //Added by groskrg@versifit.com: 10/13/2004 to resolve issue with the Menu consuming more width than it should in Netscape browser.
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HWMenuScript", ToMenuArray("Menu"));
                //line made obsolete by the change above 
                //output.Write(ToMenuArray("Menu"));

                this.Controls[0].RenderControl(output);
            }
        }

        /// <summary>
        /// CreateChildControl
        /// </summary>
        protected override void CreateChildControls()
        {
            LiteralControl MenuContainer;

            Unit myWidth = new Unit();
            Unit myHeight = new Unit();

            if (Childs.Count > 0)
            {
                if (this.Horizontal)
                {
                    myHeight = Height;
                    myWidth = Childs.Width;
                }
                else
                {
                    myHeight = Childs.Height;
                    myWidth = Width;
                }
            }
            myHeight = new Unit(myHeight.Value + StartTop); //correction
            myWidth = new Unit(myWidth.Value + StartLeft); //correction

            //MenuContainer.MergeStyle(this.ControlStyle);
            MenuContainer = new LiteralControl(@"<div id='MenuPos' style='padding:0;margin:0;border-width:0;position:relative;width:" + myWidth.Value + "; height:" + myHeight.Value + ";'><img style='padding:0;margin:0;border-width:0' src='" + ClientScriptPath + "1x1.gif' border='0' width='" + myWidth.Value + "' height='" + myHeight.Value + "'></div>"); //ns4 bug fix
            MenuContainer.ID = TargetLoc;
            this.Controls.Add(MenuContainer);
        }

        /// <summary>
        /// GetClientScriptPath() method -- works out the
        /// location of the shared scripts and images.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetClientScriptPath()
        {
            string location = null;

            IDictionary configData = null;


            if (Context != null)
                configData = (IDictionary)Context.GetConfig("system.web/webControls");

            if (configData != null)
                location = (string)configData["clientScriptsLocation"];

            if (location == null)
                return String.Empty;
            else if (location.IndexOf("{0}") >= 0)
            {
                AssemblyName assemblyName = GetType().Assembly.GetName();
                string assembly = assemblyName.Name.Replace('.', '_');
                string version = assemblyName.Version.ToString().Replace('.', '_');
                location = String.Format(location, assembly, version);
            }

            if (location == null)
                return String.Empty;
            else
                return location;
        }

        /// <summary>
        /// Restores the view-state information from a previous 
        /// user control request that was saved by the 
        /// <seealso cref="M:SaveViewState"/> method.
        /// </summary>
        /// <param name="savedState"></param>
        protected override void LoadViewState(object savedState)
        {
            // Customize state management to handle saving state of contained objects.
            if (savedState != null)
            {
                object[] myState = (object[])savedState;

                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    ((IStateManager)_controlItemStyle).LoadViewState(myState[1]);
                if (myState[2] != null)
                    ((IStateManager)_controlHiStyle).LoadViewState(myState[2]);
                if (myState[3] != null)
                    ((IStateManager)_controlSubStyle).LoadViewState(myState[3]);
                if (myState[4] != null)
                    ((IStateManager)_controlHiSubStyle).LoadViewState(myState[4]);
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have 
        /// occurred since the last page postback.
        /// </summary>
        /// <returns></returns>
        protected override object SaveViewState()
        {
            // Customized state management to handle saving state of contained objects  such as styles.

            object baseState = base.SaveViewState();
            object controlStyle = (ControlStyle != null) ? ((IStateManager)_controlItemStyle).SaveViewState() : null;
            object controlHiStyle = (ControlHiStyle != null) ? ((IStateManager)_controlHiStyle).SaveViewState() : null;
            object controlSubStyle = (ControlSubStyle != null) ? ((IStateManager)_controlSubStyle).SaveViewState() : null;
            object controlHiSubStyle = (ControlHiSubStyle != null) ? ((IStateManager)_controlHiSubStyle).SaveViewState() : null;

            object[] myState = new object[5];
            myState[0] = baseState;
            myState[1] = controlStyle;
            myState[2] = controlHiStyle;
            myState[3] = controlSubStyle;
            myState[4] = controlHiSubStyle;

            return myState;
        }

        /// <summary>
        /// Causes tracking of view-state changes to the server control 
        /// so they can be stored in the server control's StateBag object.
        /// This object is accessible through the <seealso cref="P:Control.ViewState"/> property.
        /// </summary>
        protected override void TrackViewState()
        {
            // Customized state management to handle saving state of contained objects such as styles.

            base.TrackViewState();

            if (_controlItemStyle != null)
                ((IStateManager)_controlItemStyle).TrackViewState();
            if (_controlHiStyle != null)
                ((IStateManager)_controlHiStyle).TrackViewState();
            if (_controlSubStyle != null)
                ((IStateManager)_controlSubStyle).TrackViewState();
            if (_controlHiSubStyle != null)
                ((IStateManager)_controlHiSubStyle).TrackViewState();
        }
    }
}
