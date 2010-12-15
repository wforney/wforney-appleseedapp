using System;
using System.ComponentModel;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Appleseed.Framework.Web.UI.WebControls Inherits and extends
    /// <see cref="System.Web.UI.WebControls.LinkButton"/>
    /// We add properties for default text, and TextKey which results in a search for resources.
    /// </summary>
    [History("Jonathan F. Minond", "2/2/2006", "Created to extend asp.net 2.0 control Localize")]
    [DefaultProperty("TextKey")]
    [ToolboxData("<{0}:LinkButton TextKey='' runat=server></{0}:LinkButton>")]
    public class LinkButton : System.Web.UI.WebControls.LinkButton
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
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Sets a defeault text value
        /// </summary>
        /// <value></value>
        /// <returns>The text caption displayed on the <see cref="T:System.Web.UI.WebControls.LinkButton"></see> control. The default value is an empty string ("").</returns>
        [ToolboxItem("text")]
        public string Text
        {
            get { return _defaulttext; }
            set { _defaulttext = value; }
        }

        /// <summary>
        /// Before rendering, set the keys for the text
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (_key.Length != 0)
                base.Text = General.GetString(_key, _defaulttext);
            else if (_defaulttext.Length > 0)
                base.Text = _defaulttext;

            base.OnPreRender(e);
        }
    }
}