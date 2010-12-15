using System;
using System.ComponentModel;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Appleseed.Framework.Web.UI.WebControls Inherits and extends
    /// <see cref="System.Web.UI.WebControls.Localize"/>
    /// We add properties for default text, and TextKey which results in a search for resources.
    /// </summary>
    [History("Jonathan F. Minond", "2/2/2006", "Created to extend asp.net 2.0 control Localize")]
    [DefaultProperty("TextKey")]
    [ToolboxData("<{0}:Localize TextKey='' runat=server></{0}:Localize>")]
    public class Localize : System.Web.UI.WebControls.Localize
    {
        /// <summary>
        /// Wrap a span with a class around the literal
        /// </summary>
        /// <value>The CSS class.</value>
        public string CssClass
        {
            get
            {
                object txt = ViewState["CssClass"];
                if (txt != null)
                    return (String) txt;
                return String.Empty;
            }
            set { ViewState["CssClass"] = value; }
        }

        /// <summary>
        /// Set the resource key
        /// </summary>
        /// <value>The text key.</value>
        [ToolboxItem("textkey")]
        public string TextKey
        {
            get
            {
                object txt = ViewState["TextKey"];
                if (txt != null)
                    return (String) txt;
                return String.Empty;
            }
            set { ViewState["TextKey"] = value; }
        }

        /// <summary>
        /// Sets a defeault text value
        /// </summary>
        /// <value></value>
        /// <returns>The caption displayed in the <see cref="T:System.Web.UI.WebControls.Literal"></see> control.</returns>
        [ToolboxItem("text")]
        public string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Before rendering, set the keys for the text
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            string str = "";

            if (TextKey.Length != 0)
                str = General.GetString(TextKey, Text);

            if (CssClass.Length > 0)
                str = "<span class=" + CssClass + ">" + str + "</span>";

            if (str.Length > 0)
                base.Text = str;

            base.OnPreRender(e);
        }
    }
}