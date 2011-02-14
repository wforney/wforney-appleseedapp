using System;
using System.Collections;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

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
            var setZip = new SettingItem<double, TextBox>
                {
                    MinValue = 0, MaxValue = 99999, Required = true, Value = 88045, Order = 1 
                };
            this.BaseSettings.Add("WeatherZip", setZip);

            // Module Weather Options
            var moduleWeatherOption = new List<SettingOption>
                {
                    new SettingOption((int)WeatherOption.Today, "Today"),
                    new SettingOption((int)WeatherOption.Forecast, "Forecast")
                };

            var setOption =
                new SettingItem<string, ListControl>(new CustomListDataType(moduleWeatherOption, "Name", "Val"))
                    {
                        Required = true, Value = ((int)WeatherOption.Today).ToString(), Order = 2 
                    };
            this.BaseSettings.Add("WeatherOption", setOption);

            // Module Weather Design
            var moduleWeatherDesignValue = new List<SettingOption>
                {
                    new SettingOption(0, "1"),
                    new SettingOption(1, "1b"),
                    new SettingOption(2, "1c"),
                    new SettingOption(3, "2"),
                    new SettingOption(4, "2b"),
                    new SettingOption(5, "3")
                };

            var setDesign =
                new SettingItem<string, ListControl>(new CustomListDataType(moduleWeatherDesignValue, "Name", "Name"))
                    {
                        Required = true, Value = "1", Order = 3 
                    };
            this.BaseSettings.Add("WeatherDesign", setDesign);

            // Module Weather CityIndex
            var setCityIndex = new SettingItem<double, TextBox>
                {
                    MinValue = 0, MaxValue = 99999, Required = false, Value = 0, Order = 4 
                };
            this.BaseSettings.Add("WeatherCityIndex", setCityIndex);
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
            MyHTML += string.Format("<a href='http://www.wetter.com/home/extern/ex_search.php?ms=1&ss=1&sss=2&search={0}' target='_new'>", strZip);
            MyHTML += string.Format("<img src='http://www.wetter.com/home/woys/woys.php?,{0},{1},DEPLZ,{2},{3}' border='0'></a>", strOption, strDesign, strZip, strCityIndex);
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