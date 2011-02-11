using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// XML Language Module v1.1 - based (loosely) on the original XML module with added
    /// support for content language selection via the PortalContentLanguage
    /// property in PortalSettings. By Jes1111
    /// Now supports "Print this..." and "Email this..." buttons
    /// </summary>
    public partial class XmlLangModule : PortalModuleControl
    {
        /// <summary>
        /// The Page_Load event handler on this User Control uses
        /// the Portal configuration system to obtain an xml document
        /// and xsl/t transform file location.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            XslTransform xs;
            XPathDocument xd;
            XsltArgumentList xa = new XsltArgumentList();
            XslHelper xh = new XslHelper();
            StringBuilder sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            PortalUrlDataType pt;

            pt = new PortalUrlDataType();
            pt.Value = Settings["XMLsrc"].ToString();
            string xmlsrc = Server.MapPath(pt.FullPath);
            pt = new PortalUrlDataType();
            pt.Value = Settings["XSLsrc"].ToString();
            string xslsrc = Server.MapPath(pt.FullPath);

            if ((xmlsrc != null) && (xmlsrc.Length != 0)
                && (xslsrc != null) && (xslsrc.Length != 0)
                && File.Exists(xmlsrc)
                && File.Exists(xslsrc))
            {
                xd = new XPathDocument(xmlsrc);
                xs = new XslTransform();
                xs.Load(xslsrc);
                xa.AddParam("Lang", string.Empty, portalSettings.PortalContentLanguage.Name.ToLower());
                xa.AddExtensionObject("urn:Appleseed", xh);
#if FW10
				xs.Transform(xd, xa, tw);
#else
                xs.Transform(xd, xa, tw, new XmlUrlResolver());
#endif
                Content = sb.ToString();
                ContentHolder.Controls.Add(new LiteralControl(Content.ToString()));

                ModuleConfiguration.CacheDependency.Add(xslsrc);
                ModuleConfiguration.CacheDependency.Add(xmlsrc);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public XmlLangModule()
        {
            SettingItem XMLsrc = new SettingItem(new PortalUrlDataType());
            XMLsrc.Required = true;
            XMLsrc.Order = 1;
            _baseSettings.Add("XMLsrc", XMLsrc);

            SettingItem XSLsrc = new SettingItem(new PortalUrlDataType());
            XSLsrc.Required = true;
            XSLsrc.Order = 2;
            _baseSettings.Add("XSLsrc", XSLsrc);

            SupportsWorkflow = false;
            SupportsBack = false;
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{E16DD121-267E-4268-A497-BDA6314E21A5}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}