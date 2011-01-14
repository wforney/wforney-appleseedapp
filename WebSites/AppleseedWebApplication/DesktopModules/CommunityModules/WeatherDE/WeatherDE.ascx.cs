using System;
using System.Collections;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// WeatherDE (German Weather) using german zipcodes.
    /// Adapted from original version by: Mario Hartmann, Mario@Hartmann.net
    ///
    /// USWeather
    /// Written by: Jason Schaitel, Jason_Schaitel@hotmail.com
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class WeatherDE : PortalModuleControl
    {
        public enum WeatherOption
        {
            Today,
            Forecast
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeatherDE"/> class.
        /// </summary>
        public WeatherDE()
        {
            SettingItem setZip = new SettingItem(new DoubleDataType());
            setZip.MinValue = 0;
            setZip.MaxValue = 99999;
            setZip.Required = true;
            setZip.Value = "88045";
            setZip.Order = 1;
            _baseSettings.Add("WeatherZip", setZip);


            // Module Weather Options
            ArrayList ModuleWeatherOption = new ArrayList();
            ModuleWeatherOption.Add(new SettingOption((int) WeatherOption.Today, "Today"));
            ModuleWeatherOption.Add(new SettingOption((int) WeatherOption.Forecast, "Forecast"));

            SettingItem setOption = new SettingItem(new CustomListDataType(ModuleWeatherOption, "Name", "Val"));
            setOption.Required = true;
            setOption.Value = ((int) WeatherOption.Today).ToString();
            setOption.Order = 2;
            _baseSettings.Add("WeatherOption", setOption);


            // Module Weather Design
            ArrayList ModuleWeatherDesignValue = new ArrayList();

            ModuleWeatherDesignValue.Add(new SettingOption(0, "1"));
            ModuleWeatherDesignValue.Add(new SettingOption(1, "1b"));
            ModuleWeatherDesignValue.Add(new SettingOption(2, "1c"));
            ModuleWeatherDesignValue.Add(new SettingOption(3, "2"));
            ModuleWeatherDesignValue.Add(new SettingOption(4, "2b"));
            ModuleWeatherDesignValue.Add(new SettingOption(5, "3"));

            SettingItem setDesign = new SettingItem(new CustomListDataType(ModuleWeatherDesignValue, "Name", "Name"));
            setDesign.Required = true;
            setDesign.Value = "1";
            setDesign.Order = 3;
            _baseSettings.Add("WeatherDesign", setDesign);

            // Module Weather CityIndex		
            SettingItem setCityIndex = new SettingItem(new DoubleDataType());
            setCityIndex.MinValue = 0;
            setCityIndex.MaxValue = 99999;
            setCityIndex.Required = false;
            setCityIndex.Value = "0";
            setCityIndex.Order = 4;
            _baseSettings.Add("WeatherCityIndex", setCityIndex);
        }


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            string strZip = "88045";
            string strCityIndex = "0";
            string strDesign = "1";
            string strTempOption = "0";
            string strOption = "C";

            if (Settings["WeatherZip"] != null)
                strZip = Settings["WeatherZip"].ToString();

            if (Settings["WeatherCityIndex"] != null)
                strCityIndex = Settings["WeatherCityIndex"].ToString();

            if (Settings["WeatherDesign"] != null)
                strDesign = Settings["WeatherDesign"].ToString();

            if (Settings["WeatherSetting"] != null)
                strTempOption = Settings["WeatherSetting"].ToString();

            if (strTempOption == "1")
                strOption = "F";
            //			else 
            //				strOption = "C";


            string MyHTML = string.Empty;
            MyHTML += "<!-- BEGIN wetter.com-Button -->";
            MyHTML += "<a href='http://www.wetter.com/home/extern/ex_search.php?ms=1&ss=1&sss=2&search=" + strZip +
                      "' target='_new'>";
            MyHTML += "<img src='http://www.wetter.com/home/woys/woys.php?," + strOption + "," + strDesign + ",DEPLZ," +
                      strZip + "," + strCityIndex + "' border='0'></a>";
            MyHTML += "<!-- END wetter.com-Button -->";

            pWeatherDE.InnerHtml = MyHTML;
        }

        #region general Module Implementation

        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{D3182CD6-DAFF-4E72-AD9E-0B28CB44F000}"); }
        }

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises OnInit event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            this.EditText = "EDIT";
            this.EditUrl = "~/DesktopModules/CommunityModules/WeatherDE/WeatherDEEdit.aspx";
            base.OnInit(e);
        }

        #endregion
    }
}