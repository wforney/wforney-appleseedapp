// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DailyDilbertImage.aspx.cs" company="--">
//   Copyright © -- 2011. All Rights Reserved.
// </copyright>
// <summary>
//   DailyDilbert Module
//   Based on VB Module Written by SnowCovered.com
//   Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
//   email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
//   Modifications and conversion for Appleseed Jakob hansen
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.DesktopModules.CommunityModules.DailyDilbert
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Caching;

    using Appleseed.Framework;
    using Appleseed.Framework.Web.UI;

    /// <summary>
    /// DailyDilbert Module
    ///   Based on VB Module Written by SnowCovered.com
    ///   Modifications and conversion for C# IBS Portal (c)2002 by Christopher S Judd, CDP
    ///   email- dotNet@HorizonsLLC.com    web- www.HorizonsLLC.com/IBS
    ///   Modifications and conversion for Appleseed Jakob hansen
    /// </summary>
    public partial class DailyDilbertImage : ViewItemPage
    {
        #region Properties

        /// <summary>
        ///   Set the module guids with free access to this page
        /// </summary>
        /// <value>The allowed modules.</value>
        protected override List<string> AllowedModules
        {
            get
            {
                var al = new List<string> { "2502DB18-B580-4F90-8CB4-C15E6E531031" };
                return al;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles OnInit event
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"></see> that contains the event data.
        /// </param>
        /// <remarks>
        /// The Page_Load server event handler on this User Control is used
        ///   to stream the current comic from the DailyDilbert web service
        ///   and output an image for use on the web control
        ///   The image returned is a thumbnail based on a % of
        ///   the original picture size. If no % reduction then
        ///   the image will be viewed at 100%. if (ModuleID cannot
        ///   be determined, then the image will be viewed at 100%.
        ///   Since when clicking on the comic from the module, ModuleID
        ///   is not sent, it displays the image in a new window at 100%.
        /// </remarks>
        protected override void OnInit(EventArgs e)
        {
            this.Response.ContentType = "image/gif";

            // Covert image percent to an integer, else set it to 100%
            double dblImagePercent;
            try
            {
                var imagePercent = this.ModuleSettings["ImagePercent"].ToString();
                dblImagePercent = Convert.ToDouble(imagePercent);
            }
            catch
            {
                dblImagePercent = 100;
            }

            if (dblImagePercent == 0)
            {
                dblImagePercent = 100;
            }

            dblImagePercent = dblImagePercent * 0.01;

            const string CacheKey = "DAILY_DILBERT";
            Image thumbnailImage = null;

            if (this.Cache[CacheKey] == null)
            {
                using (var objHttpReq = new WebClient())
                {
                    try
                    {
                        string strAddress = "http://www.dilbert.com/comics/dilbert/archive/";

                        using (var rs = objHttpReq.OpenRead(strAddress))
                        {
                            if (rs != null)
                            {
                                using (var objStream = new StreamReader(rs, Encoding.ASCII))
                                {
                                    strAddress = objStream.ReadToEnd();
                                }
                            }
                        }

                        if (strAddress.IndexOf("/comics/dilbert/archive/images/dilbert") > 0)
                        {
                            // Setup the URL of the image to capture
                            string strImageAddress = "http://www.dilbert.com";
                            strImageAddress +=
                                strAddress.Substring(strAddress.IndexOf("/comics/dilbert/archive/images/dilbert"), 56);

                            // Remove the & if it was added to the URL to prevent errors
                            strImageAddress = strImageAddress.Replace("&", string.Empty);

                            // Create the bitmap based on the image address
                            using (var imgStream = objHttpReq.OpenRead(strImageAddress))
                            {
                                if (imgStream != null)
                                {
                                    using (var objDilbertImg = new Bitmap(imgStream))
                                    {
                                        var width = Convert.ToInt32(objDilbertImg.Width * dblImagePercent);
                                        var height = Convert.ToInt32(objDilbertImg.Height * dblImagePercent);

                                        // Create a thumbnail using the new size
                                        thumbnailImage = objDilbertImg.GetThumbnailImage(width, height, null, IntPtr.Zero);
                                    }
                                }
                            }

                            // Set the output type and send image
                            if (thumbnailImage != null)
                            {
                                this.Cache.Insert(
                                    CacheKey, thumbnailImage, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Publish(LogLevel.Warn, "Daily Dilbert error", ex);
                    }
                }
            }
            else
            {
                thumbnailImage = (Image)this.Cache[CacheKey];
            }

            if (thumbnailImage != null)
            {
                thumbnailImage.Save(this.Response.OutputStream, ImageFormat.Gif);
            }

            base.OnInit(e);
        }

        #endregion
    }
}