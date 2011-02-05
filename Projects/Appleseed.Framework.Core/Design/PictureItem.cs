using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.Design
{

	/// <summary>
	/// PictureItem
	/// </summary>
	public class PictureItem : UserControl 
	{

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		protected HyperLink editLink;

		/// <summary>
		///     
		/// </summary>
		/// <remarks>
		///     
		/// </remarks>
		private XmlDocument metadata;

        /// <summary>
        /// Gets the current image from theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="bydefault">The bydefault.</param>
        /// <returns>A string value...</returns>
		public string GetCurrentImageFromTheme (string name, string bydefault) 
		{
			// Obtain PortalSettings from Current Context
			if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
			{
				PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
				return pS.GetCurrentTheme().GetImage(name, bydefault).ImageUrl;
			}
			return bydefault;
		}

	    /// <summary>
	    /// Gets the metadata.
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <returns>A string value...</returns>
	    public string GetMetadata(string key)
	    {
	        XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);

	        if (targetNode == null)
	        {
	            return null;
	        }

	        else
	        {
	            return targetNode.Value;
	        }
	    }

	    /// <summary>
	    /// Gets or sets the metadata.
	    /// </summary>
	    /// <value>The metadata.</value>
	    /// <remarks>
	    /// </remarks>
	    public XmlDocument Metadata
	    {
	        get { return metadata; }
	        set { metadata = value; }
	    }

	    #region Web Form Designer generated code
	    /// <summary>
	    /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
	    /// </summary>
	    /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
	    override protected void OnInit(EventArgs e)
	    {
	        base.OnInit(e);
	    }

	    #endregion

	}
}
