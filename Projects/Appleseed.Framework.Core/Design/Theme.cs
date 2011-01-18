using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appleseed.Framework.Design
{
    /// <summary>
    /// The Theme class encapsulates all the settings
    /// of the currently selected theme
    /// </summary>
    [History("bja", "2003/04/26", "C1: [Future] Added minimize color for title bar")]
    public class Theme
    {
        #region Declarations

        /// <summary>
        ///     
        /// </summary>
        public const string DefaultButtonPath = "~/Design/Themes/Default/icon";
        /// <summary>
        ///     
        /// </summary>
        public const string DefaultModuleImagePath = "~/Design/Themes/Default/img";
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultModuleCSSPath = "~/Design/Themes/Default/mod";

        /// <summary>
        ///     
        /// </summary>
        public Hashtable ThemeImages = new Hashtable();

        /// <summary>
        ///     
        /// </summary>
        public Hashtable ThemeParts = new Hashtable();

        /// <summary>
        ///     
        /// </summary>
        private string _Css = "Portal.css";

        /// <summary>
        ///     
        /// </summary>
        private string _minimize_color = string.Empty; //(FUTURE) [bja:C1]

        /// <summary>
        ///     
        /// </summary>
        private string _name;

        /// <summary>
        ///     
        /// </summary>
        private string _webPath;

        /// <summary>
        ///     
        /// </summary>
        private string type = "classic";

        #endregion

        /// <summary>
        /// Gets the HTML part.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A string value...</returns>
        public string GetHTMLPart(string name)
        {
            //			string html = GetThemePart(name);
            //			string w = string.Concat(WebPath, "/");
            //			html = html.Replace("src='", string.Concat("src='", w));
            //			html = html.Replace("src=\"", string.Concat("src=\"", w));
            //			html = html.Replace("background='", string.Concat("background='", w));
            //			html = html.Replace("background=\"", string.Concat("background=\"", w));
            //			return html;
            return GetThemePart(name);
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultImagePath">The default image path.</param>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        public Image GetImage(string name, string defaultImagePath)
        {
            Image img;

            if (ThemeImages.ContainsKey(name))
            {
                img = ((ThemeImage) ThemeImages[name]).GetImage();
                img.ImageUrl = Settings.Path.WebPathCombine(WebPath, img.ImageUrl);
            }
            else
            {
                img = new Image();
                img.ImageUrl =
                    Settings.Path.WebPathCombine(DefaultButtonPath.Replace("~", Settings.Path.ApplicationRoot),
                                                 defaultImagePath);
            }
            return img;
        }

        /// <summary>
        /// Get module specific image
        /// </summary>
        /// <param name="image_file_name">The image_file_name.</param>
        /// <returns></returns>
        public string GetModuleImageSRC(string image_file_name)
        {
            string imagePath;

            // check if image file exists in current theme img folder
            // else fall back to default theme img folder
            // else fall back to module img folder
            // else use default spacer img
            if (File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/img/" + image_file_name)))
            {
                imagePath = Settings.Path.WebPathCombine(WebPath, "/img/" + image_file_name);
            }
            else if (File.Exists(HttpContext.Current.Server.MapPath(DefaultModuleImagePath + image_file_name)))
            {
                imagePath =
                    Settings.Path.WebPathCombine(DefaultModuleImagePath.Replace("~", Settings.Path.ApplicationRoot),
                                                 image_file_name);
            }
                // TODO: Not Sure how to get current module path here
                //else if(File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/img/" + image_file_name)))
                //{
                // DefaultModuleImagePath = "~/Design/Themes/Default/img";
                // Not Sure how to get current module path here
                // imagePath = Settings.Path.WebPathCombine(Settings.Path.ApplicationRoot, "/desktopmodules/"+   ;			
                //}
            else
            {
                imagePath =
                    Settings.Path.WebPathCombine(DefaultModuleImagePath.Replace("~", Settings.Path.ApplicationRoot),
                                                 "1x1.gif");
            }

            return imagePath;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// A System.Web.UI.WebControls.Image value...
        /// </returns>
        [Obsolete("You are strongly invited to use the new overload the takes default as parameter")]
        public Image GetImage(string name)
        {
            return GetImage(name, "NoImage.gif");
        }

        /// <summary>
        /// Gets the literal control.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A System.Web.UI.LiteralControl value...</returns>
        public LiteralControl GetLiteralControl(string name)
        {
            return new LiteralControl(GetHTMLPart(name));
        }

        /// <summary>
        /// Gets the literal image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultImagePath">The default image path.</param>
        /// <returns>A string value...</returns>
        public string GetLiteralImage(string name, string defaultImagePath)
        {
            Image img = GetImage(name, defaultImagePath);
            return
                "<img src='" + img.ImageUrl + "' width='" + img.Width.ToString() + "' height='" + img.Height.ToString() +
                "'>";
        }

        /// <summary>
        /// Gets the theme part.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks>
        /// added: Jes1111 - 2004/08/27
        /// Part of Zen support
        /// </remarks>
        public string GetThemePart(string name)
        {
            if (ThemeParts.ContainsKey(name))
            {
                ThemePart part = (ThemePart) ThemeParts[name];
                return part.Html;
            }

            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the Css file name without any path.
        /// </summary>
        /// <value>The CSS.</value>
        public string Css
        {
            get { return _Css; }
            set { _Css = value; }
        }

        /// <summary>
        /// Get the Css phisical file name.
        /// Set at runtime using Web Path.
        /// </summary>
        public string CssFile
        {
            get { return Settings.Path.WebPathCombine(WebPath, _Css); }
        }

        /// <summary>
        /// Get the Css phisical file name.
        /// Set at runtime using Web Path.
        /// </summary>
        /// <param name="cssfilename">The cssfilename.</param>
        /// <returns></returns>
        public string Module_CssFile(string cssfilename)
        {
            string cssfilPath = string.Empty;

            if (File.Exists(HttpContext.Current.Server.MapPath(WebPath + "/mod/" + cssfilename)))
                cssfilPath = Settings.Path.WebPathCombine(WebPath, "/mod/" + cssfilename);
            else if (File.Exists(HttpContext.Current.Server.MapPath(DefaultModuleCSSPath + "/" + cssfilename)))
                cssfilPath =
                    Settings.Path.WebPathCombine(DefaultModuleCSSPath.Replace("~", Settings.Path.ApplicationRoot),
                                                 cssfilename);

            return cssfilPath;
        }


        /// <summary>
        /// [START FUTURE bja:C1]
        /// The Theme minimize color
        /// </summary>
        /// <value>The color of the minimize.</value>
        public string MinimizeColor
        {
            get { return _minimize_color; }
            set { _minimize_color = value; }
        } //end of MinimizeColor

        /// <summary>
        /// The Theme Name (must be the directory in which is located)
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Current Phisical Path. Readonly.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return (HttpContext.Current.Server.MapPath(WebPath)); }
        }

        /// <summary>
        /// Get the Theme physical file name.
        /// Set at runtime using Physical Path. NonSerialized.
        /// </summary>
        /// <value>The name of the theme file.</value>
        public string ThemeFileName
        {
            get
            {
                if (WebPath == string.Empty)
                    throw new ArgumentNullException("Path", "Value cannot be null!");
                //Try to get current theme from public folder
                return System.IO.Path.Combine(Path, "Theme.xml");
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>
        /// </remarks>
        public string Type
        {
            get { return type.ToLower(); }
            set { type = value.ToLower(); }
        }

        /// <summary>
        /// Current Web Path.
        /// It is set at runtime and therefore is not serialized
        /// </summary>
        /// <value>The web path.</value>
        public string WebPath
        {
            get { return _webPath; }
            set { _webPath = value; }
        }
    }
}