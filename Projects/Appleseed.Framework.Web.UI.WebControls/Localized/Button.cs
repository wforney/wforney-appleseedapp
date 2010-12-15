using System;
using System.ComponentModel;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Appleseed.Framework.Web.UI.WebControls Inherits and extends
    /// <see cref="System.Web.UI.WebControls.Button"/>
    /// We add properties for default text, and TextKey which results in a search for resources.
    /// </summary>
    [History("Jonathan F. Minond", "2/2/2006", "Created to extend asp.net 2.0 control Localize")]
    [DefaultProperty("TextKey")]
    [ToolboxData("<{0}:Button TextKey='' runat=server></{0}:Button>")]
    public class Button : System.Web.UI.WebControls.Button
    {
        private string _defaulttext = "";
        private string _key = "";

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
        /// Before rendering, set the keys for the text
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (TextKey.Length != 0)
                base.Text = General.GetString(TextKey, base.Text);

            base.OnPreRender(e);
        }
    }
}