using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Framework.Web.UI.Design
{
	/// <summary>
	/// Designer support for paging
	/// </summary>
	public class PagingDesigner : ControlDesigner 
	{
		/// <summary>
		/// Component is the instance of the component or control that
		/// this designer object is associated with. This property is
		/// inherited from System.ComponentModel.ComponentDesigner.
		/// </summary>
		/// <returns>
		/// The HTML markup used to represent the control at design time.
		/// </returns>
		public override string GetDesignTimeHtml() 
		{
			Paging paging = (Paging) Component;

			using (StringWriter sw = new StringWriter())
			{
				using (HtmlTextWriter tw = new HtmlTextWriter(sw))
				{
        
					paging.HideOnSinglePage = false;
					paging.RefreshButtons();

					paging.RenderControl(tw);
				}
				return sw.ToString();
			}
		}
	}
}