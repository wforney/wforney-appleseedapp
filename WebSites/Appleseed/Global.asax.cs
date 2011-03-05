// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   The global.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Profile;
    using System.Web.Routing;
    using System.Web.Security;

    using Appleseed.Context;
    using Appleseed.Framework;
    using Appleseed.Framework.Exceptions;
    using Appleseed.Framework.Helpers;
    using Appleseed.Framework.Scheduler;
    using Appleseed.Framework.Security;
    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;

    using MvcContrib.UI.InputBuilder;

    using Path = System.IO.Path;
    using Reader = Appleseed.Context.Reader;

    /// <summary>
    /// The global.
    /// </summary>
    public class Global : HttpApplication
    {
        #region Public Methods

        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.IgnoreRoute("{*allasmx}", new { allasmx = @".*\.asmx(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(string.Empty);

            // routes.MapRoute(
            // "Default",                                             // Route name
            // "{controller}/{action}/{id}",                          // URL with parameters
            // new { controller = "Home", action = "Index", id = "" } // Parameter defaults
            // );
        }

        /// <summary>
        /// Runs on application end.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        public void Application_OnEnd(object sender, EventArgs e)
        {
            ErrorHandler.Publish(LogLevel.Info, "Application Ended");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the BeginRequest event of the AppleseedApplication control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void AppleseedApplication_BeginRequest(object sender, EventArgs e)
        {
            var contextReader = new Reader(new WebContextReader());
            var context = contextReader.Current;

            var currentUrl = context.Request.Path.ToLower();

            if (Debugger.IsAttached && currentUrl.Contains("trace.axd"))
            {
                return;
            }

            context.Trace.Warn("Application_BeginRequest :: " + currentUrl);
            if (Portal.PageID > 0)
            {
                var physicalPath = context.Server.MapPath(currentUrl.Substring(currentUrl.LastIndexOf("/") + 1));

                if (!File.Exists(physicalPath))
                {
                    // Rewrites the path
                    context.RewritePath("~/default.aspx?" + context.Request.ServerVariables["QUERY_STRING"]);
                }
            }
            else
            {
                var pname = currentUrl.Substring(currentUrl.LastIndexOf("/") + 1);

                // if the request was not caused by an MS Ajax Client script invoking a WS.
                if (!currentUrl.ToLower().EndsWith(".asmx/js"))
                {
                    if (!String.IsNullOrEmpty(pname) && pname.Length > 5)
                    {
                        pname = pname.Substring(0, pname.Length - 5);
                    }

                    if (Regex.IsMatch(pname, @"^\d+$"))
                    {
                        context.RewritePath(
                            string.Format(
                                "~/default.aspx?pageid={0}{1}", pname, context.Request.ServerVariables["QUERY_STRING"]));
                    }
                }
            }

            // 1st Check: is it a dangerously malformed request?
            // Important patch http://support.microsoft.com/?kbid=887459
            if (context.Request.Path.IndexOf('\\') >= 0 ||
                Path.GetFullPath(context.Request.PhysicalPath) != context.Request.PhysicalPath)
            {
                throw new AppleseedRedirect(LogLevel.Warn, HttpStatusCode.NotFound, "Malformed request", null);
            }

            // 2nd Check: is the AllPortals Lock switched on?
            // let the user through if client IP address is in LockExceptions list, otherwise throw...
            if (Config.LockAllPortals)
            {
                var rawUrl = context.Request.RawUrl.ToLower(CultureInfo.InvariantCulture);
                var lockRedirect = Config.LockRedirect;
                if (!rawUrl.EndsWith(lockRedirect))
                {
                    // construct IPList
                    var lockKeyHolders = Config.LockKeyHolders.Split(new[] { ';' });
                    var ipList = new IPList();
                    foreach (var lockKeyHolder in lockKeyHolders)
                    {
                        if (lockKeyHolder.IndexOf("-") > -1)
                        {
                            ipList.AddRange(
                                lockKeyHolder.Substring(0, lockKeyHolder.IndexOf("-")), 
                                lockKeyHolder.Substring(lockKeyHolder.IndexOf("-") + 1));
                        }
                        else
                        {
                            ipList.Add(lockKeyHolder);
                        }
                    }

                    // check if requestor's IP address is in allowed list
                    if (!ipList.CheckNumber(context.Request.UserHostAddress))
                    {
                        throw new PortalsLockedException();
                    }
                }
            }

            // 3rd Check: is database/code version correct?
            var requestUri = context.Request.Url;
            var requestPath = requestUri.AbsolutePath.ToLower(CultureInfo.InvariantCulture);
            var databaseUpdateRedirect = Config.DatabaseUpdateRedirect;
            if (databaseUpdateRedirect.StartsWith("~/"))
            {
                databaseUpdateRedirect = databaseUpdateRedirect.TrimStart(new[] { '~' });
            }

            if (requestPath.EndsWith(databaseUpdateRedirect.ToLower(CultureInfo.InvariantCulture)))
            {
                return; // this is DB Update page... so skip creation of PortalSettings
            }

            var installRedirect = Config.InstallerRedirect;
            if (installRedirect.StartsWith("~/"))
            {
                installRedirect = installRedirect.TrimStart(new[] { '~', '/' });
            }

            installRedirect = installRedirect.ToLower(CultureInfo.InvariantCulture);
            if (requestPath.EndsWith(installRedirect) || requestPath.Contains(installRedirect.Split(new[] { '/' })[0]))
            {
                return; // this is Install page... so skip creation of PortalSettings
            }

            var smartErrorRedirect = Config.SmartErrorRedirect;
            if (smartErrorRedirect.StartsWith("~/"))
            {
                smartErrorRedirect = smartErrorRedirect.TrimStart(new[] { '~' });
            }

            if (requestPath.EndsWith(smartErrorRedirect.ToLower(CultureInfo.InvariantCulture)))
            {
                return; // this is SmartError page... so skip creation of PortalSettings
            }

            var versionDelta = Database.DatabaseVersion.CompareTo(Portal.CodeVersion);

            // if DB and code versions do not match
            if (versionDelta != 0)
            {
                // ...and this is not DB Update page
                var errorMessage = string.Format(
                    "Database version: {0} Code version: {1}", Database.DatabaseVersion, Portal.CodeVersion);

                if (versionDelta < 0)
                {
                    // DB Version is behind Code Version
                    ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                    this.Response.Redirect(Framework.Settings.Path.ApplicationRoot + databaseUpdateRedirect, true);
                }
                else
                {
                    // DB version is ahead of Code Version
                    ErrorHandler.Publish(LogLevel.Warn, errorMessage);
                }
            }

            // ************ 'calculate' response to this request ************
            // Test 1 - try requested Alias and requested PageID
            // Test 2 - try requested Alias and PageID 0
            // Test 3 - try default Alias and requested PageID
            // Test 4 - try default Alias and PageID 0
            // The UrlToleranceLevel determines how many times the test is allowed to fail before the request is considered
            // to be "an error" and is therefore redirected:
            // UrlToleranceLevel 1 
            // - requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
            // - if requested PageID is found, it is shown
            // - if requested PageID is not found, InvalidPageIdRedirect page is shown
            // UrlToleranceLevel 2 
            // - requested Alias must be valid - if invalid, InvalidAliasRedirect page on default portal will be shown
            // - if requested PageID is found, it is shown
            // - if requested PageID is not found, PageID 0 (Home page) is shown
            // UrlToleranceLevel 3 - <<<<<< not working?
            // - if requested Alias is invalid, default Alias will be used
            // - if requested PageID is found, it is shown
            // - if requested PageID is not found, InvalidPageIdRedirect page is shown
            // UrlToleranceLevel 4 - 
            // - if requested Alias is invalid, default Alias will be used
            // - if requested PageID is found, it is shown
            // - if requested PageID is not found, PageID 0 (Home page) is shown
            PortalSettings portalSettings = null;

            var pageId = Portal.PageID; // Get PageID from QueryString
            var portalAlias = Portal.UniqueID; // Get requested alias from querystring, cookies or hostname
            var defaultAlias = Config.DefaultPortal; // get default portal from config

            // load arrays with values to test
            var testAlias = new[] { portalAlias, portalAlias, defaultAlias, defaultAlias };
            var testPageId = new[] { pageId, pageId, pageId, pageId };

            var testsAllowed = Config.UrlToleranceLevel;
            var testsToRun = testsAllowed > 2 ? 4 : 2;

            // if requested alias is default alias, limit UrlToleranceLevel to max value of 2 and limit tests to 2
            if (portalAlias == defaultAlias)
            {
                testsAllowed = testsAllowed % 2;
                testsToRun = 2;
            }

            var testsCounter = 1;
            while (testsCounter <= testsToRun)
            {
                // try with current values from arrays
                try
                {
                    portalSettings = new PortalSettings(testPageId[testsCounter - 1], testAlias[testsCounter - 1]);
                }
                catch (DatabaseUnreachableException dexc)
                {
                    // If no database, must update
                    ErrorHandler.Publish(LogLevel.Error, dexc);
                    this.Response.Redirect(Config.DatabaseUpdateRedirect);
                }

                // test returned result
                if (portalSettings == null)
                {
                    continue;
                }

                if (portalSettings.PortalAlias != null)
                {
                    break; // successful hit
                }

                testsCounter++; // increment the test counter and continue
            }

            if (portalSettings != null)
            {
                if (portalSettings.PortalAlias == null)
                {
                    // critical error - neither requested alias nor default alias could be found in DB
                    throw new AppleseedRedirect(
                        Config.NoPortalErrorRedirect, 
                        LogLevel.Fatal, 
                        Config.NoPortalErrorResponse, 
                        "Unable to load any portal - redirecting request to ErrorNoPortal page.", 
                        null);
                }

                Membership.Provider.ApplicationName = portalSettings.PortalAlias;
                ProfileManager.Provider.ApplicationName = portalSettings.PortalAlias;
                Roles.ApplicationName = portalSettings.PortalAlias;

                if (testsCounter <= testsAllowed)
                {
                    // success
                    // Portal Settings has passed the test so add it to Context
                    context.Items.Add("PortalSettings", portalSettings);
                    context.Items.Add("PortalID", portalSettings.PortalID); // jes1111
                }
                else
                {
                    // need to redirect
                    if (portalSettings.PortalAlias != portalAlias)
                    {
                        // we didn't get the portal we asked for
                        throw new AppleseedRedirect(
                            Config.InvalidAliasRedirect, 
                            LogLevel.Info, 
                            HttpStatusCode.NotFound, 
                            "Invalid Alias specified in request URL - redirecting (404) to InvalidAliasRedirect page.", 
                            null);
                    }

                    if (portalSettings.ActivePage.PageID != pageId)
                    {
                        // we didn't get the page we asked for
                        throw new AppleseedRedirect(
                            Config.InvalidPageIdRedirect, 
                            LogLevel.Info, 
                            HttpStatusCode.NotFound, 
                            "Invalid PageID specified in request URL - redirecting (404) to InvalidPageIdRedirect page.", 
                            null);
                    }
                }
            }

            // WLF: This was backwards before so it would always set refreshSite true because the cookie was changed before it was checked.
            // WLF: REVIEW: This whole section needs a code review.
            // Try to get alias from cookie to determine if alias has been changed
            var refreshSite = false;
            var portalAliasCookie = context.Request.Cookies["PortalAlias"];
            if (portalAliasCookie != null && portalAliasCookie.Value.ToLower() != Portal.UniqueID)
            {
                refreshSite = true; // Portal has changed since last page request
            }

            if (portalSettings != null)
            {
                portalAliasCookie = new HttpCookie("PortalAlias") { Path = "/", Value = portalSettings.PortalAlias };
                if (context.Response.Cookies["PortalAlias"] == null)
                {
                    context.Response.Cookies.Add(portalAliasCookie);
                }
                else
                {
                    context.Response.Cookies.Set(portalAliasCookie);
                }
            }

            // if switching portals then clean parameters [TipTopWeb]
            // Must be the last instruction in this method 
            var refreshedCookie = context.Request.Cookies["refreshed"];

            // 5/7/2006 Ed Daniel
            // Added hack for Http 302 by extending condition below to check for more than 3 cookies
            if (refreshSite && context.Request.Cookies.Keys.Count > 3)
            {
                // Sign out and force the browser to refresh only once to avoid any dead-lock
                if (refreshedCookie == null || refreshedCookie.Value == "false")
                {
                    var rawUrl = context.Request.RawUrl;
                    var newRefreshedCookie = new HttpCookie("refreshed", "true")
                        {
                           Path = "/", Expires = DateTime.Now.AddMinutes(1) 
                        };
                    if (refreshedCookie == null)
                    {
                        context.Response.Cookies.Add(newRefreshedCookie);
                    }
                    else
                    {
                        context.Response.Cookies.Set(newRefreshedCookie);
                    }

                    var msg =
                        string.Format(
                            "User logged out on global.asax line 423. Values -> refreshsite: {0}, context.Request.Cookies.Keys.count: {1}, rawurl: {2}",
                            refreshSite,
                            context.Request.Cookies.Keys.Count,
                            rawUrl);

                    ErrorHandler.Publish(
                        LogLevel.Warn, 
                        msg);

                    // sign-out, if refreshed parameter on the command line we will not call it again
                    PortalSecurity.SignOut(rawUrl, false);
                }
            }

            // invalidate cookie, so the page can be refreshed when needed
            refreshedCookie = context.Request.Cookies["refreshed"];
            if (refreshedCookie != null && context.Request.Cookies.Keys.Count > 3)
            {
                var newRefreshedCookie = new HttpCookie("refreshed", "false")
                    {
                       Path = "/", Expires = DateTime.Now.AddMinutes(1) 
                    };
                context.Response.Cookies.Set(newRefreshedCookie);
            }

            // This is done in order to allow the sitemap to reference a page that is outside this website.
            var targetPage = this.Request.Params["sitemapTargetPage"];
            if (!string.IsNullOrEmpty(targetPage))
            {
                int mvcPageId;
                if (int.TryParse(targetPage, out mvcPageId))
                {
                    var url = HttpUrlBuilder.BuildUrl(mvcPageId);
                    this.Response.Redirect(url);
                }
            }
        }

        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!this.Request.Path.ToLower().Contains("images.ashx"))
            {
                this.AppleseedApplication_BeginRequest(sender, e);
            }
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
            ErrorHandler.ProcessUnhandledException();
        }

        /// <summary>
        /// Runs when the application starts.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        /// <remarks>
        /// </remarks>
        protected void Application_Start(object sender, EventArgs e)
        {
            var context = HttpContext.Current;

            // moved from PortalSettings
            var f = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(Portal)).Location);
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["CodeVersion"] = f.FilePrivatePart;
            HttpContext.Current.Application.UnLock();

            ErrorHandler.Publish(
                LogLevel.Info, string.Format("Application Started: code version {0}", Portal.CodeVersion));

            if (Config.CheckForFilePermission)
            {
                try
                {
                    var newDir = Path.Combine(Framework.Settings.Path.ApplicationPhysicalPath, "_createnewdir");

                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }

                    if (Directory.Exists(newDir))
                    {
                        Directory.Delete(newDir);
                    }
                }
                catch (Exception ex)
                {
                    throw new AppleseedException(
                        LogLevel.Fatal, 
                        HttpStatusCode.ServiceUnavailable, 
                        "ASPNET Account does not have rights to the file system", 
                        ex); // Jes1111
                }
            }

            // Start scheduler
            if (Config.SchedulerEnable)
            {
                PortalSettings.Scheduler =
                    CachedScheduler.GetScheduler(
                        context.Server.MapPath(Framework.Settings.Path.ApplicationRoot), 
                        Config.SqlConnectionString, 
                        Config.SchedulerPeriod, 
                        Config.SchedulerCacheSize);
                PortalSettings.Scheduler.Start();
            }

            // Start proxy
            if (Config.UseProxyServerForServerWebRequests)
            {
                WebRequest.DefaultWebProxy = PortalSettings.GetProxy();
            }

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            InputBuilder.BootStrap();
        }

        #endregion
    }
}