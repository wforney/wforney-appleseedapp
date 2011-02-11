using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Appleseed.Framework;
using Appleseed.Framework.Data;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Users.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{

	/// <summary>
	/// Xml Module
	/// </summary>
	public partial class XmlModule : PortalModuleControl
	{
        /// <summary>
        /// The Page_Load event handler on this User Control uses
        /// the Portal configuration system to obtain an xml document
        /// and xsl/t transform file location.  It then sets these
        /// properties on an &lt;asp:Xml&gt; server control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, EventArgs e)
		{
			PortalUrlDataType pt;

			pt = new PortalUrlDataType();
			pt.Value = Settings["XMLsrc"].ToString();
			string xmlsrc = pt.FullPath;

			if ((xmlsrc != null) && (xmlsrc.Length != 0))
			{
				if (File.Exists(Server.MapPath(xmlsrc)))
				{
					xml1.DocumentSource = xmlsrc;
					// Change - 28/Feb/2003 - Jeremy Esland
					// Builds cache dependency files list
					this.ModuleConfiguration.CacheDependency.Add(Server.MapPath(xmlsrc));
				}
				else
				{
					Controls.Add(new LiteralControl("<br>" + "<span class='Error'>" +General.GetString("FILE_NOT_FOUND").Replace("%1%", xmlsrc) + "<br>"));
				}
			}

			pt = new PortalUrlDataType();
			pt.Value = Settings["XSLsrc"].ToString();
			string xslsrc = pt.FullPath;

			if ((xslsrc != null) && (xslsrc.Length != 0))
			{
				if (File.Exists(Server.MapPath(xslsrc)))
				{
					xml1.TransformSource = xslsrc;
					// Change - 28/Feb/2003 - Jeremy Esland
					// Builds cache dependency files list
					this.ModuleConfiguration.CacheDependency.Add(Server.MapPath(xslsrc));
				}
				else
				{
					Controls.Add(new LiteralControl("<br>" + "<span class='Error'>" +General.GetString("FILE_NOT_FOUND").Replace("%1%", xslsrc) + "<br>"));
				}
			}
		}

        /// <summary>
        /// Contsructor
        /// </summary>
		public XmlModule()
		{
			SettingItem XMLsrc = new SettingItem(new PortalUrlDataType());
			XMLsrc.Required = true;
			XMLsrc.Order = 1;
			this._baseSettings.Add("XMLsrc", XMLsrc);

			SettingItem XSLsrc = new SettingItem(new PortalUrlDataType());
			XSLsrc.Required = true;
			XSLsrc.Order = 2;
			this._baseSettings.Add("XSLsrc", XSLsrc);
		}

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
		public override Guid GuidID
		{
			get
			{
				return new Guid("{BE224332-03DE-42B7-B127-AE1F1BD0FADC}");
			}
		}

		#region Web Form Designer generated code
        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
            this.Load += new EventHandler(this.Page_Load);
			base.OnInit(e);
		}
		#endregion
	}
}
