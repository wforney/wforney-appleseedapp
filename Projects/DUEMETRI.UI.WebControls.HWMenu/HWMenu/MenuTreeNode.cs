using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls;

namespace DUEMETRI.UI.WebControls.HWMenu
{
    /// <summary>
    /// Summary description for MenuTreeNode.
    /// </summary>
    public class MenuTreeNode : WebControl, INamingContainer
    {
        #region Private implementation
        string _text;
        string _link;
        string _backgroundImage;
        MenuTreeNodes _childs;
        Style _controlHiStyle;

        void Initialize()
        {
            this.Text = "Untitled menu";
            _childs = new MenuTreeNodes();
            ControlStyle.Width = new Unit(100);
            ControlStyle.Height = new Unit(20);
        }
        #endregion

        /// <summary>
        /// Style of this element when the mouse is over the element.
        /// </summary>
        [
        Category("Style"),
        Description("Style of this element when the mouse is over the element."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true),
        PersistenceMode(PersistenceMode.InnerProperty),
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
                    ((IStateManager)_controlHiStyle).LoadViewState(myState[1]);
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
            object controlHiStyle = (ControlHiStyle != null) ? ((IStateManager)_controlHiStyle).SaveViewState() : null;

            object[] myState = new object[2];
            myState[0] = baseState;
            myState[1] = controlHiStyle;

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

            if (_controlHiStyle != null)
                ((IStateManager)_controlHiStyle).TrackViewState();
        }

        /// <summary>
        /// Public constructor. Initializes text property.
        /// </summary>
        /// <param name="text"></param>
        public MenuTreeNode(string text)
        {
            Initialize();
            this.Text = text;
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public MenuTreeNode()
        {
            Initialize();
        }

        /// <summary>
        /// What you want to show in the element. It can be text, an image or html.
        /// To show an image Text will be look like "&lt;img src='MyImage'&gt;"
        /// To use roll over images use "rollover:MyImage1:MyImage2"
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Text string- Where you want to go when you click the element.
        /// Looks like "MyLink" 
        /// Can also be used to execute javascript statements. 
        /// For instance when you want the link to open in the top window use 
        /// "javascript:top.document.location.href='Link.htm';"
        /// You can in fact start a whole script when the element is clicked 
        /// with the help of javascript:. "javascript:{your script; another function;}"
        /// </summary>
        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }

        /// <summary>
        /// Background image for the element. 
        /// Is not supported for NS4 when the menu is across frames. 
        /// I had to disable this for NS4 in frame setup because 
        /// I could not get it to work properly. 
        /// (Everybody who wants to try and find a solution for this is very welcome. 
        /// Enable in menu_com.js)
        /// </summary>
        [
        Category("Appearance"),
        Description("Background image for the element.")
        ]
        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
            }
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
                return _childs;
            }
        }

        /// <summary>
        /// Retrieves a string representing the current menu array
        /// </summary>
        /// <returns>The current menu array</returns>
        public string ToMenuArray()
        {
            WebColorConverter wc = new WebColorConverter();

            StringBuilder sb = new StringBuilder();

            sb.Append("=new Array(");
            sb.Append(CleanForJavascript(Text));
            sb.Append(", ");
            sb.Append(CleanForJavascript(Link));
            sb.Append(", ");
            sb.Append(CleanForJavascript(BackgroundImage));
            sb.Append(", ");
            sb.Append(Childs.Count);
            sb.Append(", ");
            if (Height.Value > 0)
                sb.Append(Height.Value);
            else
                sb.Append(20);
            sb.Append(", ");
            if (Width.Value > 0)
                sb.Append(Width.Value);
            else
                sb.Append(Text.Length * 7 + 15);
            sb.Append(", ");
            sb.Append(CleanForJavascript(wc.ConvertToString(ControlStyle.BackColor)));
            sb.Append(", ");
            sb.Append(CleanForJavascript(wc.ConvertToString(ControlHiStyle.BackColor)));
            sb.Append(", ");
            sb.Append(CleanForJavascript(wc.ConvertToString(ControlStyle.ForeColor)));
            sb.Append(", ");
            sb.Append(CleanForJavascript(wc.ConvertToString(ControlHiStyle.ForeColor)));
            sb.Append(", ");
            sb.Append(CleanForJavascript(wc.ConvertToString(ControlStyle.BorderColor)));
            sb.Append(");");
            sb.Append("\n");

            return sb.ToString();
        }

        private string CleanForJavascript(string value)
        {
            StringBuilder sb = new StringBuilder(value);
            sb.Replace("'", "\'");
            sb.Replace("\"", "\\\"");
            sb.Insert(0, "\"");
            sb.Append("\"");
            return sb.ToString();
        }
    }
}