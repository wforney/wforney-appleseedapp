using System;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Localized ButtonColumn
    /// </summary>
    [ToolboxData("<{0}:ButtonColumn TextKey='' runat=server></{0}:ButtonColumn>")]
    public class BoundColumn : System.Web.UI.WebControls.BoundColumn
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
        /// Gets or sets the text displayed in the header section of the column.
        /// </summary>
        /// <value></value>
        /// <returns>The text displayed in the header section of the column. The default value is <see cref="F:System.String.Empty"></see>.</returns>
        public override string HeaderText
        {
            get
            {
                string text = ViewState["HeaderText"].ToString();
                return General.GetString(TextKey, text);
            }
            set
            {
                ViewState["HeaderText"] = value;
                OnColumnChanged();
            }
        }
    }
}