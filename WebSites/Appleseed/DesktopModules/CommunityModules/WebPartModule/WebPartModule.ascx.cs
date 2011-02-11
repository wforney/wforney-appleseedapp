using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// WebPart module - Digital Dashboard WebPart Wrapper
    /// Written by: damacco, damacco@hotmail.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class WebPartModule : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string ModuleIDent = ModuleID.ToString();
            string WebPartFile = Settings["WebPartFile"].ToString();
            if ((WebPartFile == null) || (WebPartFile == string.Empty))
            {
                ContentPane.Text = "<font color=red><b>WebPart file setting is missing!</b></font>";
                ContentPane.Visible = true;
                return;
            }

            string filename = Server.MapPath(WebPartFile);
            WebPart partData = WebPartParser.Load(filename);

            if (partData == null)
                return;

            if (partData.RequiresIsolation == 1)
            {
                InnerFrame.Attributes["src"] = partData.ContentLink;
                InnerFrame.Attributes["width"] = partData.Width;
                InnerFrame.Attributes["height"] = partData.Height;
                InnerFrame.Visible = true;
                return;
            }

            string content = ObtainWebPartContent(partData);
            if (content != null)
            {
                ContentPane.Text = UpdateContentWithTokens(content);
                ContentPane.Visible = true;
            }
        }


        /// <summary>
        /// Fetches the content of the network.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private string FetchNetworkContent(string url)
        {
            WebRequest netRequest = WebRequest.Create(url);
            WebResponse netResponse = netRequest.GetResponse();

            try
            {
                Stream receiveStream = netResponse.GetResponseStream();
                byte[] read = new Byte[512];
                string content = string.Empty;
                int bytes = 0;

                try
                {
                    do
                    {
                        bytes = receiveStream.Read(read, 0, 512);
                        content += Encoding.ASCII.GetString(read, 0, bytes);
                    } while (bytes > 0);
                }
                finally
                {
                    receiveStream.Close();
                }
                return content;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Obtains the content of the web part.
        /// </summary>
        /// <param name="partData">The part data.</param>
        /// <returns></returns>
        private string ObtainWebPartContent(WebPart partData)
        {
            string content = null;

            if ((partData.ContentLink != null) && (partData.ContentLink.Length > 0))
                content = FetchNetworkContent(partData.ContentLink);
            if (content == null)
                content = partData.Content;

            if ((partData.ContentType == 1) || (partData.ContentType == 2))
                return "<font color=red><b>Unsupported Web Part Content	Format</b></font>";

            if (partData.ContentType == 3)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(content);

                string xslContent = null;
                if ((partData.XSLLink != null) && (partData.XSLLink.Length > 0))
                    xslContent = FetchNetworkContent(partData.XSLLink);

                if (xslContent == null)
                    xslContent = partData.XSL;

                StringWriter output = new StringWriter();

#if FW10
				XslTransform transform = new XslTransform();
				
				transform.Load(new XmlTextReader(new StringReader(xslContent)));
				transform.Transform(document, null, output);
#else
#if FW11
				XslTransform transform = new XslTransform();

				transform.Load(new XmlTextReader(new StringReader(xslContent)), new XmlUrlResolver(), new Evidence());
				transform.Transform(document, null, output, new XmlUrlResolver());
#else
                // setup and perform the XSLT transformation
                XslCompiledTransform xslt = new XslCompiledTransform();

                xslt.Load(new XmlTextReader(new StringReader(xslContent)));
                xslt.Transform(document, null, output);
#endif
#endif

                content = output.ToString();
            }

            return content;
        }


        /// <summary>
        /// Updates the content with tokens.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        private string UpdateContentWithTokens(string content)
        {
            int startIndex = 0;

            while ((startIndex = content.IndexOf("_WPQ_", startIndex)) != -1)
            {
                content = content.Substring(0, startIndex) +
                          ModuleID.ToString() + content.Substring(startIndex + 5);
                startIndex += 5;
            }
            return content;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WebPartModule"/> class.
        /// </summary>
        public WebPartModule()
        {
            SettingItem setWebPartFile = new SettingItem(new StringDataType());
            setWebPartFile.Required = true;
            setWebPartFile.Value = "_Appleseed/WebParts/sales.dwp";
            setWebPartFile.Order = 1;
            _baseSettings.Add("WebPartFile", setWebPartFile);
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531009}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
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