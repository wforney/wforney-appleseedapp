//added 7/27/2003 by Joe Audette to read proxy setting from the web.config
//end addition
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Xml;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// XmlFeed Module
    /// </summary>
    public partial class XmlFeed : PortalModuleControl
    {
        /// <summary>
        /// The Page_Load event handler on this User Control obtains
        /// an xml document and xsl/t transform file location.
        /// It then sets these properties on an &lt;asp:Xml&gt; server control.
        /// Patch 11/11/2003 by Manu: Errors are logged.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string xmlsrc = string.Empty;
            string xmlsrcType = Settings["XML Type"].ToString();
            if (xmlsrcType == "File")
                xmlsrc = Settings["XML File"].ToString();
            else
                xmlsrc = Settings["XML URL"].ToString();


            string xslsrc = string.Empty;
            string xslsrcType = Settings["XSL Type"].ToString();
            if (xslsrcType == "File")
                xslsrc = Settings["XSL File"].ToString();
            else
                xslsrc = Settings["XSL Predefined"].ToString();


            //Timeout
            int timeout = int.Parse(Settings["Timeout"].ToString());

            if ((xmlsrc != null) && (xmlsrc.Length != 0))
            {
                if (xmlsrcType == "File")
                {
                    PortalUrlDataType pathXml = new PortalUrlDataType();
                    pathXml.Value = xmlsrc;
                    xmlsrc = pathXml.FullPath;

                    if (File.Exists(Server.MapPath(xmlsrc)))
                        xml1.DocumentSource = xmlsrc;
                    else
                        Controls.Add(
                            new LiteralControl("<br><div class='error'>File " + xmlsrc + " not found.<br></div>"));
                }
                else
                {
                    try
                    {
                        LogHelper.Log.Warn("XMLFeed - This should not done more than once in 30 minutes: '" + xmlsrc +
                                           "'");

                        // handle on the remote ressource
                        HttpWebRequest wr = (HttpWebRequest) WebRequest.Create(xmlsrc);
                        //jes1111 - not needed: global proxy is set in Global class Application Start
                        //						if (ConfigurationSettings.AppSettings.Get("UseProxyServerForServerWebRequests") == "true")
                        //							wr.Proxy = PortalSettings.GetProxy();

                        // set the HTTP properties
                        wr.Timeout = timeout*1000; // milliseconds to seconds
                        // Read the response
                        WebResponse resp = wr.GetResponse();
                        // Stream read the response
                        Stream stream = resp.GetResponseStream();
                        // Read XML data from the stream
                        XmlTextReader reader = new XmlTextReader(stream);
                        // ignore the DTD
                        reader.XmlResolver = null;
                        // Create a new document object
                        XmlDocument doc = new XmlDocument();
                        // Create the content of the XML Document from the XML data stream
                        doc.Load(reader);
                        // the XML control to hold the generated XML document
                        xml1.Document = doc;
                    }
                    catch (Exception ex)
                    {
                        // connectivity issues
                        Controls.Add(
                            new LiteralControl("<br><div class='error'>Error loading: " + xmlsrc + ".<br>" + ex.Message +
                                               "</div>"));
                        ErrorHandler.Publish(LogLevel.Error, "Error loading: " + xmlsrc + ".", ex);
                    }
                }
            }

            if (xslsrcType == "File")
            {
                PortalUrlDataType pathXsl = new PortalUrlDataType();
                pathXsl.Value = xslsrc;
                xslsrc = pathXsl.FullPath;
            }
            else
            {
                //				if (ConfigurationSettings.AppSettings.Get("XMLFeedXSLFolder") != null)
                //				{
                //					if (ConfigurationSettings.AppSettings.Get("XMLFeedXSLFolder").ToString().Length > 0)
                //						xslsrc = ConfigurationSettings.AppSettings.Get("XMLFeedXSLFolder").ToString() + xslsrc;
                //					else
                //						xslsrc = "~/DesktopModules/CommunityModules/XmlFeed/" + xslsrc;
                //				}
                //				else
                //				{
                //					xslsrc = "~/DesktopModules/CommunityModules/XmlFeed/" + xslsrc;
                //				}
                if (Config.XMLFeedXSLFolder.Length == 0)
                    xslsrc = Path.WebPathCombine(TemplateSourceDirectory, xslsrc);
                else
                    xslsrc = Path.WebPathCombine(Config.XMLFeedXSLFolder, xslsrc);

                if (!xslsrc.EndsWith(".xslt"))
                    xslsrc += ".xslt";
            }

            if ((xslsrc != null) && (xslsrc.Length != 0))
            {
                if (File.Exists(Server.MapPath(xslsrc)))
                    xml1.TransformSource = xslsrc;
                else
                    Controls.Add(new LiteralControl("<br><div class='error'>File " + xslsrc + " not found.<br></div>"));
            }
        }


        /// <summary>
        /// Contsructor
        /// </summary>
        public XmlFeed()
        {
            int _groupOrderBase;
            SettingItemGroup _Group;

            #region Module Special Settings

            _Group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            _groupOrderBase = (int) SettingItemGroup.MODULE_SPECIAL_SETTINGS;

            SettingItem XMLsrcType = new SettingItem(new ListDataType("URL;File"));
            XMLsrcType.Required = true;
            XMLsrcType.Value = "URL";
            XMLsrcType.Group = _Group;
            XMLsrcType.Order = _groupOrderBase + 1;
            _baseSettings.Add("XML Type", XMLsrcType);

            SettingItem XMLsrcUrl = new SettingItem(new UrlDataType());
            XMLsrcUrl.Required = false;
            XMLsrcUrl.Group = _Group;
            XMLsrcUrl.Order = _groupOrderBase + 2;
            _baseSettings.Add("XML URL", XMLsrcUrl);

            SettingItem XMLsrcFile = new SettingItem(new PortalUrlDataType());
            XMLsrcFile.Required = false;
            XMLsrcFile.Group = _Group;
            XMLsrcFile.Order = _groupOrderBase + 3;
            _baseSettings.Add("XML File", XMLsrcFile);

            SettingItem XSLsrcType = new SettingItem(new ListDataType("Predefined;File"));
            XSLsrcType.Required = true;
            XSLsrcType.Value = "Predefined";
            XSLsrcType.Order = _groupOrderBase + 4;
            XSLsrcType.Group = _Group;
            _baseSettings.Add("XSL Type", XSLsrcType);


            ListDataType xsltFileList = new ListDataType(GetXSLListForFeedTransformations());
            SettingItem XSLsrcPredefined = new SettingItem(xsltFileList);
            XSLsrcPredefined.Required = true;
            XSLsrcPredefined.Value = "RSS91";
            XSLsrcPredefined.Group = _Group;
            XSLsrcPredefined.Order = _groupOrderBase + 5;
            _baseSettings.Add("XSL Predefined", XSLsrcPredefined);

            SettingItem XSLsrcFile = new SettingItem(new PortalUrlDataType());
            XSLsrcFile.Required = false;
            XSLsrcFile.Group = _Group;
            XSLsrcFile.Order = _groupOrderBase + 6;
            _baseSettings.Add("XSL File", XSLsrcFile);

            SettingItem Timeout = new SettingItem(new IntegerDataType());
            Timeout.Required = true;
            Timeout.Group = _Group;
            Timeout.Order = _groupOrderBase + 7;
            Timeout.Value = "15";
            _baseSettings.Add("Timeout", Timeout);

            #endregion
        }


        /// <summary>
        /// Author:		Joe Audette
        /// Added:		7/31/2003
        /// Allows you to add stylesheets for new feed formats without recompiling.
        /// Any xslt stylesheets placed in the folder specified in the web.config willshow up
        /// in the dropdown list
        /// Patch 11/11/2003 by Manu: Errors are logged.
        /// </summary>
        /// <returns>FileInfo[]</returns>
        public FileInfo[] GetXSLListForFeedTransformations()
        {
            string xsltPath = string.Empty;

            //jes1111 - if (ConfigurationSettings.AppSettings["XMLFeedXSLFolder"] != null && ConfigurationSettings.AppSettings["XMLFeedXSLFolder"].Length > 0)
            if (Config.XMLFeedXSLFolder.Length != 0)
                //jes1111 - xsltPath = ConfigurationSettings.AppSettings["XMLFeedXSLFolder"];
                xsltPath = Config.XMLFeedXSLFolder;
            else
            {
                //this will default to the xmlfeed folder where the .xslt files are located by default
                xsltPath = HttpContext.Current.Server.MapPath(TemplateSourceDirectory);
            }

            try
            {
                if (Directory.Exists(xsltPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(xsltPath);
                    return dir.GetFiles("*.xslt");
                }
                else
                {
                    LogHelper.Log.Warn("Default XSLT location not found: '" + xsltPath + "'");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Error, "XSLT location not found: " + xsltPath, ex);
            }
            return null;
        }

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531020}"); }
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