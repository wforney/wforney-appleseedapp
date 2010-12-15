using System;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Localized ButtonColumn
    /// </summary>
    [ToolboxData("<{0}:ButtonColumn TextKey='' runat=server></{0}:ButtonColumn>")]
    public class ButtonColumn : System.Web.UI.WebControls.ButtonColumn
    {
        /// <summary>
        /// Gets or sets the text key.
        /// </summary>
        /// <value>The text key.</value>
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
        /// Gets or sets the caption that is displayed in the buttons of the <see cref="T:System.Web.UI.WebControls.ButtonColumn"></see> object.
        /// </summary>
        /// <value></value>
        /// <returns>The caption displayed in the buttons of the <see cref="T:System.Web.UI.WebControls.ButtonColumn"></see>. The default is an empty string ("").</returns>
        public override string Text
        {
            get
            {
                string text = ViewState["Text"].ToString();
                return General.GetString(TextKey, text);
            }
            set
            {
                ViewState["Text"] = value;
                OnColumnChanged();
            }
        }
    }
}