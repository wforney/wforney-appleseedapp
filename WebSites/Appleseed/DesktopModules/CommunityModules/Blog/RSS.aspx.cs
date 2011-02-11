// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RSS.aspx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   Author:					Joe Audette
//   Created:				1/18/2004
//   Last Modified:			2/7/2004
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Content.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    using Appleseed.Framework;
    using Appleseed.Framework.Content.Data;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;

    /// <summary>
    /// Author:					Joe Audette
    ///   Created:				1/18/2004
    ///   Last Modified:			2/7/2004
    /// </summary>
    public partial class RSS : Page
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += this.Page_Load;
            base.OnInit(e);
        }

        /// <summary>
        /// The module title as it will be displayed on the page. Handles cultures automatically.
        /// </summary>
        /// <param name="moduleSettings">
        /// The module settings.
        /// </param>
        /// <returns>
        /// The title text.
        /// </returns>
        private static string TitleText(Dictionary<string, ISettingItem> moduleSettings)
        {
            var titleText = "Appleseed Blog";

            if (HttpContext.Current != null)
            {
                // if it is not design time (and not overridden - Jes1111)
                var portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                if (portalSettings.PortalContentLanguage != CultureInfo.InvariantCulture &&
                    moduleSettings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name] != null &&
                    moduleSettings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name].ToString().
                        Length > 0)
                {
                    titleText =
                        moduleSettings["MODULESETTINGS_TITLE_" + portalSettings.PortalContentLanguage.Name].ToString();
                }
            }

            return titleText;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> instance containing the event data.
        /// </param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Params.Get("mID") != null)
            {
                try
                {
                    var ModuleID = int.Parse(this.Request.Params.Get("mID"));
                    this.RenderRSS(ModuleID);
                }
                catch (Exception ex)
                {
                    this.RenderError(ex.Message);
                }
            }
            else
            {
                this.RenderError("Invalid ModuleID");
            }
        }

        /// <summary>
        /// Renders the error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void RenderError(string message)
        {
            this.Response.Write(message);
            this.Response.End();
        }

        /// <summary>
        /// Renders the RSS.
        /// </summary>
        /// <param name="moduleID">
        /// The module ID.
        /// </param>
        private void RenderRSS(int moduleID)
        {
            /*
             
            For more info on RSS 2.0
            http://www.feedvalidator.org/docs/rss2.html
            
            Fields not implemented yet:
            <blogChannel:blogRoll>http://radio.weblogs.com/0001015/userland/scriptingNewsLeftLinks.opml</blogChannel:blogRoll>
            <blogChannel:mySubscriptions>http://radio.weblogs.com/0001015/gems/mySubscriptions.opml</blogChannel:mySubscriptions>
            <blogChannel:blink>http://diveintomark.org/</blogChannel:blink>
            <lastBuildDate>Mon, 30 Sep 2002 11:00:00 GMT</lastBuildDate>
            <docs>http://backend.userland.com/rss</docs>
             
            */
            this.Response.ContentType = "text/xml";

            var moduleSettings = ModuleSettings.GetModuleSettings(moduleID);
            Encoding encoding = new UTF8Encoding();

            var xmlTextWriter = new XmlTextWriter(this.Response.OutputStream, encoding);
            xmlTextWriter.Formatting = Formatting.Indented;

            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteComment(
                "RSS generated by Appleseed Portal Blog Module V 1.0 on " + DateTime.Now.ToLongDateString());
            xmlTextWriter.WriteStartElement("rss");

            xmlTextWriter.WriteStartAttribute("version", "http://Appleseedportal.net/blogmodule");
            xmlTextWriter.WriteString("2.0");
            xmlTextWriter.WriteEndAttribute();

            xmlTextWriter.WriteStartElement("channel");

            /*  
                RSS 2.0
                Required elements for channel are title link and description
            */
            xmlTextWriter.WriteStartElement("title");

            // try
            // {
            // xmlTextWriter.WriteString(moduleSettings["MODULESETTINGS_TITLE_en-US"].ToString());
            // }
            // catch
            // {
            // HACK: Get MODULESETTINGS_TITLE from where?
            xmlTextWriter.WriteString(TitleText(moduleSettings));

            // }
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("link");
            xmlTextWriter.WriteString(
                this.Request.Url.ToString().Replace("DesktopModules/Blog/RSS.aspx", "DesktopDefault.aspx"));
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("description");
            xmlTextWriter.WriteString(moduleSettings["Description"].ToString());
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("copyright");
            xmlTextWriter.WriteString(moduleSettings["Copyright"].ToString());
            xmlTextWriter.WriteEndElement();

            // begin optional RSS 2.0 fields

            // ttl = time to live in minutes, how long a channel can be cached before refreshing from the source
            xmlTextWriter.WriteStartElement("ttl");
            xmlTextWriter.WriteString(moduleSettings["RSS Cache Time In Minutes"].ToString());
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("managingEditor");
            xmlTextWriter.WriteString(moduleSettings["Author Email"].ToString());
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("language");
            xmlTextWriter.WriteString(moduleSettings["Language"].ToString());
            xmlTextWriter.WriteEndElement();

            // jes1111 - if(ConfigurationSettings.AppSettings.Get("webMaster") != null)
            if (Config.WebMaster.Length != 0)
            {
                xmlTextWriter.WriteStartElement("webMaster");
                xmlTextWriter.WriteString(ConfigurationManager.AppSettings.Get("webMaster"));
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteStartElement("generator");
            xmlTextWriter.WriteString("Appleseed Portal Blog Module V 1.0");
            xmlTextWriter.WriteEndElement();

            var blogDb = new BlogDB();
            var dr = blogDb.GetBlogs(moduleID);

            try
            {
                // write channel items
                while (dr.Read())
                {
                    // beginning of blog entry
                    xmlTextWriter.WriteStartElement("item");

                    /*  
                    RSS 2.0
                    All elements of an item are optional, however at least one of title or description 
                    must be present.
                    */
                    xmlTextWriter.WriteStartElement("title");
                    xmlTextWriter.WriteString(dr["Title"].ToString());
                    xmlTextWriter.WriteEndElement();

                    xmlTextWriter.WriteStartElement("link");
                    xmlTextWriter.WriteString(
                        this.Request.Url.ToString().Replace("RSS.aspx", "blogview.aspx") + "&ItemID=" + dr["ItemID"]);
                    xmlTextWriter.WriteEndElement();

                    xmlTextWriter.WriteStartElement("pubDate");
                    xmlTextWriter.WriteString(
                        DateTime.Parse(dr["StartDate"].ToString()).ToString("dddd MMMM d yyyy hh:mm:ss tt zzz"));
                    xmlTextWriter.WriteEndElement();

                    xmlTextWriter.WriteStartElement("guid");
                    xmlTextWriter.WriteString(
                        this.Request.Url.ToString().Replace("RSS.aspx", "blogview.aspx") + "&ItemID=" + dr["ItemID"]);
                    xmlTextWriter.WriteEndElement();

                    xmlTextWriter.WriteStartElement("comments");
                    xmlTextWriter.WriteString(
                        this.Request.Url.ToString().Replace("RSS.aspx", "blogview.aspx") + "&ItemID=" + dr["ItemID"]);
                    xmlTextWriter.WriteEndElement();

                    xmlTextWriter.WriteStartElement("description");
                    xmlTextWriter.WriteCData(this.Server.HtmlDecode(dr["Description"].ToString()));
                    xmlTextWriter.WriteEndElement();

                    // end blog entry
                    xmlTextWriter.WriteEndElement();
                }
            }
            finally
            {
                dr.Close();
            }

            // end of document
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.Close();
        }

        #endregion
    }
}