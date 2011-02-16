// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlLangModule.ascx.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   XML Language Module v1.1 - based (loosely) on the original XML module with added
//   support for content language selection via the PortalContentLanguage
//   property in PortalSettings. By Jes1111
//   Now supports "Print this..." and "Email this..." buttons
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using Appleseed.Framework;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Web.UI.WebControls;

    /// <summary>
    /// XML Language Module v1.1 - based (loosely) on the original XML module with added
    ///   support for content language selection via the PortalContentLanguage
    ///   property in PortalSettings. By Jes1111
    ///   Now supports "Print this..." and "Email this..." buttons
    /// </summary>
    public partial class XmlLangModule : PortalModuleControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmlLangModule" /> class.
        /// </summary>
        public XmlLangModule()
        {
            var xmlSrc = new SettingItem<string, TextBox>(new PortalUrlDataType()) { Required = true, Order = 1 };
            this.BaseSettings.Add("XMLsrc", xmlSrc);

            var xslSrc = new SettingItem<string, TextBox>(new PortalUrlDataType()) { Required = true, Order = 2 };
            this.BaseSettings.Add("XSLsrc", xslSrc);

            this.SupportsWorkflow = false;
            this.SupportsBack = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get
            {
                return new Guid("{E16DD121-267E-4268-A497-BDA6314E21A5}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Page_Load event handler on this User Control uses
        ///   the Portal configuration system to obtain an xml document
        ///   and xsl/t transform file location.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var xa = new XsltArgumentList();
            var xh = new XslHelper();
            var sb = new StringBuilder();
            var tw = XmlWriter.Create(new StringWriter(sb));

            var pt = new PortalUrlDataType { Value = this.Settings["XMLsrc"].ToString() };
            var xmlsrc = this.Server.MapPath(pt.FullPath);
            pt = new PortalUrlDataType { Value = this.Settings["XSLsrc"].ToString() };
            var xslsrc = this.Server.MapPath(pt.FullPath);

            if (string.IsNullOrEmpty(xmlsrc) || string.IsNullOrEmpty(xslsrc) || !File.Exists(xmlsrc) ||
                !File.Exists(xslsrc))
            {
                return;
            }

            var xd = new XPathDocument(xmlsrc);
            var xs = XslHelper.GetXslt(xslsrc);
            xa.AddParam("Lang", string.Empty, this.PortalSettings.PortalContentLanguage.Name.ToLower());
            xa.AddExtensionObject("urn:Appleseed", xh);
#if FW10
            xs.Transform(xd, xa, tw);
#else
            xs.Transform(xd, xa, tw, new XmlUrlResolver());
#endif
            this.Content = sb.ToString();
            this.ContentHolder.Controls.Add(new LiteralControl(this.Content.ToString()));

            this.ModuleConfiguration.CacheDependency.Add(xslsrc);
            this.ModuleConfiguration.CacheDependency.Add(xmlsrc);
        }

        #endregion
    }
}