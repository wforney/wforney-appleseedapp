using System;
using System.Web.UI;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// Localized TemplateColumn
    /// </summary>
    [ToolboxData("<{0}:TemplateColumn TextKey='' runat=server></{0}:TemplateColumn>")]
    public class TemplateColumn : System.Web.UI.WebControls.TemplateColumn
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
                object o = ViewState["HeaderText"];
                if (o != null)
                    return General.GetString(TextKey, o.ToString());
                else
                    return General.GetString(TextKey);
            }
            set
            {
                ViewState["HeaderText"] = value;
                OnColumnChanged();
            }
        }
    }
}