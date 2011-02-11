using System;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Weather (Using USA zipcodes)
    /// Written by: Jason Schaitel, Jason_Schaitel@hotmail.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class WeatherUS : PortalModuleControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherUS"/> class.
        /// </summary>
        [
            History("mario@hartmann.net", "2003/06/11",
                "changed DataType of setZip to StringdataType to enable leading zeros in zip.")]
        public WeatherUS()
        {
            SettingItem setZip = new SettingItem(new StringDataType());
            //setZip.MinValue = 0;
            //setZip.MaxValue = 99999;
            setZip.Required = true;
            setZip.Value = "10001";
            setZip.Order = 1;
            _baseSettings.Add("Zip", setZip);

            SettingItem setOption = new SettingItem(new StringDataType());
            setOption.Required = true;
            setOption.Value = "0";
            setOption.Order = 2;
            _baseSettings.Add("Option", setOption);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string Zip = "10001";
            string Option = "0";

            if (Settings["Zip"] != null)
                Zip = Settings["Zip"].ToString();
            if (Settings["Option"] != null)
                Option = Settings["Option"].ToString();

            string MyHTML = string.Empty;

            MyHTML += "<a href='http://www.wx.com/myweather.cfm?ZIP=" + Zip + "' target='_new'>";
            if (Option == "0")
            {
                MyHTML += "<img src='http://www.wx.com/partnership/sticker.cfm?zip=" + Zip + "' border='0'></a>";
            }
            else
            {
                MyHTML += "<img src='http://www4.wx.com/partnership/miniradar_servlet.cfm?zip=" + Zip +
                          "&size=0' border='0'></a>";
            }

            pWeather.InnerHtml = MyHTML;
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531006}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            this.EditText = "EDIT";
            this.EditUrl = "~/DesktopModules/CommunityModules/WeatherUS/WeatherUSEdit.aspx";
            base.OnInit(e);
        }

        #endregion
    }
}