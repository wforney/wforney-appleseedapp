using System;
using System.Web;
using System.Web.UI;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;

namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// This user control will render the current list of languages.
    /// by José Viladiu
    /// </summary>
    [ToolboxData("<{0}:Selector runat='server'></{0}:Selector>")]
    [History("jviladiu@portalservices.net", "2004/06/15", "First Implementation Selector webcontrol for Appleseed")]
    public class Selector : LanguageSwitcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        public Selector()
        {
            ImagePath = Path.WebPathCombine(Path.ApplicationRoot, "aspnet_client/flags/");
            ChangeLanguageAction = LanguageSwitcherAction.PostBack;
            // Obtain PortalSettings from Current Context
            if (HttpContext.Current != null && HttpContext.Current.Items["PortalSettings"] != null)
            {
                PortalSettings pS = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                if (pS.CustomSettings != null)
                {
                    if (pS.CustomSettings["SITESETTINGS_LANGLIST"] != null)
                        LanguageListString = pS.CustomSettings["SITESETTINGS_LANGLIST"].ToString();
                    if (pS.CustomSettings["LANGUAGESWITCHER_CUSTOMFLAGS"] != null)
                        if (bool.Parse(pS.CustomSettings["LANGUAGESWITCHER_CUSTOMFLAGS"].ToString()))
                            ImagePath = pS.PortalFullPath + "/images/flags/";
                }
            }
            else LanguageListString = "en-US";
        }

        /// <summary>
        /// Examines/combines all the variables involved and sets
        /// CurrentUICulture and CurrentCulture.  instance containing the event data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnChangeLanguage(LanguageSwitcherEventArgs e)
        {
            if (Context != null)
            {
                int mID = 0;
                if (Context.Request.Params["Mid"] != null)
                    mID = Int32.Parse(Context.Request.Params["Mid"]);

                int tID = 0;
                if (Context.Request.Params["PageID"] != null)
                    tID = Int32.Parse(Context.Request.Params["PageID"]);
                else if (Context.Request.Params["TabID"] != null)
                    tID = Int32.Parse(Context.Request.Params["TabID"]);

                string auxUrl = Context.Request.Url.AbsolutePath;
                string auxApplication = Context.Request.ApplicationPath;
                int index = auxUrl.ToLower().IndexOf(auxApplication.ToLower());
                if (index != -1)
                {
                    auxUrl = auxUrl.Substring(index + auxApplication.Length);
                }
                if (auxUrl.StartsWith("/"))
                    auxUrl = "~" + auxUrl;
                else
                    auxUrl = "~/" + auxUrl;

                string customParams = string.Empty;

                foreach (string key in Context.Request.QueryString.Keys)
                {
                    if (!key.ToLower().Equals("mid") && !key.ToLower().Equals("tabid") && !key.ToLower().Equals("lang"))
                        customParams += "&" + key + "=" + Context.Request.Params[key];
                }

                string returnUrl =
                    HttpUrlBuilder.BuildUrl(auxUrl, tID, mID, e.CultureItem.Culture, customParams, string.Empty,
                                            string.Empty);
                if (returnUrl.ToLower().IndexOf("lang") == -1)
                {
                    customParams += "&Lang=" + e.CultureItem.Culture.Name;
                    returnUrl =
                        HttpUrlBuilder.BuildUrl(auxUrl, tID, mID, e.CultureItem.Culture, customParams, string.Empty,
                                                string.Empty);
                }
                //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(e.CultureItem
                //LanguageCultureItem lci = new LanguageCultureItem(e.CultureItem.Culture.Name, e.CultureItem.Culture.Name)
                SetCurrentLanguage(e.CultureItem);
                Context.Response.Redirect(returnUrl);
            }
        }
    }
}