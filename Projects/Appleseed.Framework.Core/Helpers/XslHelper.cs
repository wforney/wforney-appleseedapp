using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Appleseed.Framework.Security;
using Appleseed.Framework.Site.Configuration;
using System.Web.Security;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// XslHelper object, designed to be imported into an XSLT transform
    /// via XsltArgumentList.AddExtensionObject(...). Provides transform with 
    /// access to various Appleseed functions, such as BuildUrl(), IsInRoles(), data 
    /// formatting, etc. (Jes1111)
    /// </summary>
    public class XslHelper
    {
        private PortalSettings portalSettings;
        private MembershipUser user;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XslHelper"/> class.
        /// </summary>
        /// <returns>
        /// A void value...
        /// </returns>
        public XslHelper() {
            if ( HttpContext.Current != null ) {
                portalSettings = ( PortalSettings )HttpContext.Current.Items["PortalSettings"];

                Appleseed.Framework.Users.Data.UsersDB users = new Appleseed.Framework.Users.Data.UsersDB();
                user = users.GetSingleUser( HttpContext.Current.User.Identity.Name, portalSettings.PortalAlias );
            }
        }

        /// <summary>
        /// Localizes the specified text key.
        /// </summary>
        /// <param name="textKey">The text key.</param>
        /// <param name="translation">The translation.</param>
        /// <returns>A string value...</returns>
        public string Localize(string textKey, string translation)
        {
            return General.GetString(textKey, translation);
        }

        # region Data Formatting

        /// <summary>
        /// Formats the money.
        /// </summary>
        /// <param name="myAmount">My amount.</param>
        /// <param name="myCurrency">My currency.</param>
        /// <returns>A string value...</returns>
        public string FormatMoney(string myAmount, string myCurrency)
        {
            try
            {
                // Jonathan - im not sure what namesppace this comes from?
                // TODO: FIX TIHS
                return myAmount;
                //return new Money(Decimal.Parse(myAmount, CultureInfo.InvariantCulture.NumberFormat), myCurrency).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formats the temp.
        /// </summary>
        /// <param name="tempStr">The temp STR.</param>
        /// <param name="dataScale">The data scale.</param>
        /// <param name="outputScale">The output scale.</param>
        /// <returns>A string value...</returns>
        public string FormatTemp(string tempStr, string dataScale, string outputScale)
        {
            try
            {
                double conv;

                if (dataScale == outputScale)
                {
                    conv = double.Parse(tempStr, new CultureInfo(string.Empty));
                    return conv.ToString("F0") + Convert.ToChar(176) + outputScale;
                }

                else if (outputScale.ToUpper() == "C")
                {
                    conv = F2C(double.Parse(tempStr, new CultureInfo(string.Empty)));
                    return conv.ToString("F0") + Convert.ToChar(176) + "C";
                }

                else
                {
                    conv = C2F(double.Parse(tempStr, new CultureInfo(string.Empty)));
                    return conv.ToString("F0") + Convert.ToChar(176) + "F";
                }
            }

            catch
            {
                return tempStr;
            }
        }

        /// <summary>
        /// C2s the F.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>A double value...</returns>
        public double C2F(double c)
        {
            return (1.8*c) + 32;
        }

        /// <summary>
        /// F2s the C.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>A double value...</returns>
        public double F2C(double f)
        {
            return (f - 32)/1.8;
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">The date STR.</param>
        /// <param name="dataCulture">The data culture.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns>A string value...</returns>
        public string FormatDateTime(string dateStr, string dataCulture, string formatStr)
        {
            try
            {
                return FormatDateTime(dateStr, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatStr);
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">The date STR.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns>A string value...</returns>
        public string FormatDateTime(string dateStr, string formatStr)
        {
            try
            {
                return
                    FormatDateTime(dateStr, portalSettings.PortalDataFormattingCulture.Name,
                                   portalSettings.PortalDataFormattingCulture.Name, formatStr);
            }
            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">The date STR.</param>
        /// <returns>A string value...</returns>
        public string FormatDateTime(string dateStr)
        {
            try
            {
                return DateTime.Parse(dateStr).ToLongDateString();
            }

            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="dateStr">The date STR.</param>
        /// <param name="dataCulture">The data culture.</param>
        /// <param name="outputCulture">The output culture.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns>A string value...</returns>
        public string FormatDateTime(string dateStr, string dataCulture, string outputCulture, string formatStr)
        {
            try
            {
                DateTime conv;

                if (dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
                {
                    conv =
                        DateTime.ParseExact(dateStr, "mm/dd/yyyy hh:mm:ss", new CultureInfo(dataCulture, false),
                                            DateTimeStyles.AdjustToUniversal);
                }

                else
                {
                    conv = DateTime.Parse(dateStr, new CultureInfo(dataCulture, false), DateTimeStyles.None);
                }

                if (outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
                    return conv.ToString(formatStr);

                else
                    return conv.ToString(formatStr, new CultureInfo(outputCulture, false));
            }

            catch
            {
                return dateStr;
            }
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="numberStr">The number STR.</param>
        /// <param name="dataCulture">The data culture.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns>A string value...</returns>
        public string FormatNumber(string numberStr, string dataCulture, string formatStr)
        {
            try
            {
                return FormatNumber(numberStr, dataCulture, portalSettings.PortalDataFormattingCulture.Name, formatStr);
            }

            catch
            {
                return numberStr;
            }
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="numberStr">The number STR.</param>
        /// <param name="dataCulture">The data culture.</param>
        /// <param name="outputCulture">The output culture.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns>A string value...</returns>
        public string FormatNumber(string numberStr, string dataCulture, string outputCulture, string formatStr)
        {
            try
            {
                Double conv;

                if (dataCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
                {
                    conv = Double.Parse(numberStr);
                }

                else
                {
                    conv = Double.Parse(numberStr, new CultureInfo(dataCulture, false));
                }

                if (outputCulture.ToLower() == portalSettings.PortalDataFormattingCulture.Name.ToLower())
                    return conv.ToString(formatStr);

                else
                    return conv.ToString(formatStr, new CultureInfo(outputCulture, false));
            }

            catch
            {
                return numberStr;
            }
        }

        # endregion

        # region Security

        /// <summary>
        /// Checks the roles.
        /// </summary>
        /// <param name="authRoles">The auth roles.</param>
        /// <returns>A bool value...</returns>
        public bool CheckRoles(string authRoles)
        {
            return PortalSecurity.IsInRoles(authRoles);
        }

        # endregion

        # region Url Builder

        /// <summary>
        /// Adds to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paramKey">The param key.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns>A string value...</returns>
        public string AddToUrl(string url, string paramKey, string paramValue)
        {
            if (url.IndexOf(paramKey) == -1)
            {
                if (url.IndexOf("?") > 0)
                {
                    url = url.Trim() + "&" + paramKey.Trim() + "=" + paramValue.Trim();
                }

                else
                {
                    url = url.Trim();
                    url = url.Substring(0, url.LastIndexOf("/")) + "/" + paramKey.Trim() + "_" + paramValue.Trim() +
                          url.Substring(url.LastIndexOf("/"));
                }
            }
            return url;
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="targetPage">The target page.</param>
        /// <param name="pageID">The page ID.</param>
        /// <returns>A string value...</returns>
        public string BuildUrl(string targetPage, int pageID)
        {
//			targetPage = System.Text.RegularExpressions.Regex.Replace(targetPage,@"[\.\$\^\{\[\(\|\)\*\+\?!'""]",string.Empty);
//			targetPage = targetPage.Replace(" ","_").ToLower();
//			return Appleseed.HttpUrlBuilder.BuildUrl("~/" + targetPage + ".aspx", tabID);

            return HttpUrlBuilder.BuildUrl(string.Concat("~/", Clean(targetPage), ".aspx"), pageID);
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="targetPage">The target page.</param>
        /// <param name="pageID">The page ID.</param>
        /// <param name="pathTrace">The path trace.</param>
        /// <returns></returns>
        public string BuildUrl(string targetPage, int pageID, string pathTrace)
        {
            return HttpUrlBuilder.BuildUrl(string.Concat("~/", Clean(targetPage), ".aspx"), pageID, Clean(pathTrace));
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <param name="pathTrace">The path trace.</param>
        /// <returns>A string value...</returns>
        public string BuildUrl(int pageID, string pathTrace)
        {
            return HttpUrlBuilder.BuildUrl(pageID, Clean(pathTrace));
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="pageID">The page ID.</param>
        /// <returns></returns>
        public string BuildUrl(int pageID)
        {
            return HttpUrlBuilder.BuildUrl(pageID);
        }

        /// <summary>
        /// Cleans the specified my text.
        /// </summary>
        /// <param name="myText">My text.</param>
        /// <returns></returns>
        private string Clean(string myText)
        {
            // is this faster/slower than using iteration over string?
            char mySeparator = '_';
            string singleSeparator = "_";
            string doubleSeparator = "__";
            //myText = Regex.Replace(myText.ToLower(), @"[^-'/\p{L}\p{N}]",singleSeparator);
            myText = Regex.Replace(myText.ToLower(), @"[^-\p{L}\p{N}]", singleSeparator);

            return myText.Replace(doubleSeparator, singleSeparator).Trim(mySeparator);
        }

        #endregion

        # region Member Access - portalSettings

        /// <summary>
        /// Portals the alias.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalAlias()
        {
            return portalSettings.PortalAlias;
        }

        /// <summary>
        /// Portals the ID.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalID()
        {
            return portalSettings.PortalID.ToString();
        }

        /// <summary>
        /// Pages the ID.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PageID()
        {
            return portalSettings.ActivePage.PageID.ToString();
        }

        /// <summary>
        /// Tabs the title.
        /// </summary>
        /// <returns>A string value...</returns>
        public string TabTitle()
        {
            return portalSettings.ActivePage.PageName;
        }

        /// <summary>
        /// Portals the content language.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalContentLanguage()
        {
            return portalSettings.PortalContentLanguage.Name;
        }

        /// <summary>
        /// Portals the UI language.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalUILanguage()
        {
            return portalSettings.PortalUILanguage.Name;
        }

        /// <summary>
        /// Portals the data formatting culture.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalDataFormattingCulture()
        {
            return portalSettings.PortalDataFormattingCulture.Name;
        }

        /// <summary>
        /// Portals the layout path.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalLayoutPath()
        {
            return portalSettings.PortalLayoutPath;
        }

        /// <summary>
        /// Portals the full path.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalFullPath()
        {
            return portalSettings.PortalFullPath;
        }

        /// <summary>
        /// Portals the name.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalName()
        {
            return portalSettings.PortalName;
        }

        /// <summary>
        /// Portals the title.
        /// </summary>
        /// <returns>A string value...</returns>
        public string PortalTitle()
        {
            return portalSettings.PortalTitle;
        }

        /// <summary>
        /// Users the name.
        /// </summary>
        /// <returns>A string value...</returns>
        public string UserName()
        {
            return user.UserName;
        }

        /// <summary>
        /// Users the email.
        /// </summary>
        /// <returns>A string value...</returns>
        public string UserEmail()
        {
            return user.Email;
        }

        /// <summary>
        /// Users the ID.
        /// </summary>
        /// <returns>A string value...</returns>
        public Guid UserID()
        {
            return (Guid)user.ProviderUserKey;
        }

        /// <summary>
        /// Desktops the tabs XML.
        /// </summary>
        /// <returns>
        /// A System.Xml.XPath.XPathNodeIterator value...
        /// </returns>
        public XPathNodeIterator DesktopTabsXml()
        {
            return portalSettings.PortalPagesXml.CreateNavigator().Select("*");
        }

        #endregion

        #region Member Access - moduleSettings

        #endregion

        #region Selected Options for Products module (ECommerce receipt)

        /// <summary>
        /// Selecteds the options.
        /// </summary>
        /// <param name="metadataXml">The metadata XML.</param>
        /// <returns>A string value...</returns>
        public string SelectedOptions(string metadataXml)
        {
            string selectedOptions = string.Empty;
            //Create a xml Document
            XmlDocument myXmlDoc = new XmlDocument();

            if (metadataXml != null && metadataXml.Length > 0)
            {
                try
                {
                    myXmlDoc.LoadXml(metadataXml);
                    XmlNode foundNode1 = myXmlDoc.SelectSingleNode("options/option1/selected");

                    if (foundNode1 != null)
                    {
                        selectedOptions += foundNode1.InnerText;
                    }
                    XmlNode foundNode2 = myXmlDoc.SelectSingleNode("options/option2/selected");

                    if (foundNode2 != null)
                    {
                        selectedOptions += " - " + foundNode2.InnerText;
                    }
                    XmlNode foundNode3 = myXmlDoc.SelectSingleNode("options/option3/selected");

                    if (foundNode3 != null)
                    {
                        selectedOptions += " - " + foundNode3.InnerText;
                    }
                }

                catch (Exception ex)
                {
                    ErrorHandler.Publish(LogLevel.Warn, "XSL failed. Metadata Was: '" + metadataXml + "'", ex);
                }
            }
            return selectedOptions;
        }

        #endregion
    }
}