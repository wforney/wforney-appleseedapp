using System;
using System.IO;
using System.Net;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// MapQuest module
    /// Written by: Shaun Walker (released the module in VB.NET)
    /// Moved into Appleseed by Jakob Hansen, hansen3000@hotmail.com
    /// </summary>
    public partial class MapQuest : PortalModuleControl
    {
        protected bool showMap;
        protected bool showAddress;

        /// <summary>
        /// The Page_Load event handler on this User Control uses
        /// the Portal configuration system to obtain the MapQuest picture
        /// using the string "mqmapgend" starting point for the image URL.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            showMap = "True" == Settings["ShowMap"].ToString();
            showAddress = "True" == Settings["ShowAddress"].ToString();

            if (!Page.IsPostBack)
            {
                lblLocation.Text = Settings["Location"].ToString();
                if (showAddress)
                {
                    if (Settings["Street"].ToString().Length != 0)
                        lblAddress.Text += Settings["Street"].ToString() + "<br>";

                    if (Settings["Region"].ToString().Length != 0)
                        lblAddress.Text += Settings["City"].ToString() + ", " + Settings["Region"].ToString() + "<br>";
                    else
                        lblAddress.Text += Settings["City"].ToString() + "<br>";

                    if (Settings["PostalCode"].ToString().Length != 0)
                        lblAddress.Text += Settings["PostalCode"].ToString() + "<br>";

                    if (Settings["Country"].ToString().Length != 0)
                        lblAddress.Text += Settings["Country"].ToString();
                }

                int zoom;
                try
                {
                    zoom = int.Parse(Settings["Zoom"].ToString()) - 1;
                }
                catch
                {
                    zoom = 7;
                }
                RadioButtonList1.Items[zoom].Selected = true;
                if (!bool.Parse(Settings["ShowZoom"].ToString()))
                {
                    RadioButtonList1.Visible = false;
                    Literal1.Visible = false;
                }
                show();
            }
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        private void show()
        {
            hypMap.NavigateUrl = BuildMapURL();
            hypMap.Target = "_" + Settings["Target"].ToString();

            if (showMap)
                hypMap.ImageUrl = GetMapImageURL(hypMap.NavigateUrl);
            else
                hypMap.Text = "Show Map";

            hypDirections.Text = "Get Directions";
            hypDirections.NavigateUrl = BuildDirectionsURL();
        }

        /// <summary>
        /// Encodes the value.
        /// </summary>
        /// <param name="strValue">The STR value.</param>
        /// <returns></returns>
        private string EncodeValue(string strValue)
        {
            strValue = strValue.Replace("\n", string.Empty);
            strValue = strValue.Replace("\r", string.Empty);
            strValue = strValue.Replace("%", "%25");
            strValue = strValue.Replace("&", "%26");
            strValue = strValue.Replace("+", "%30");
            strValue = strValue.Replace(" ", "+");
            return strValue;
        }

        /// <summary>
        /// Builds the map URL.
        /// </summary>
        /// <returns></returns>
        private string BuildMapURL()
        {
            string strURL;
            strURL = "http://www.mapquest.com/maps/map.adp";
            strURL += "?address=" + EncodeValue(Settings["Street"].ToString());
            strURL += "&city=" + EncodeValue(Settings["City"].ToString());
            strURL += "&state=" + EncodeValue(Settings["Region"].ToString());
            strURL += "&country=" + EncodeValue(Settings["Country"].ToString());
            strURL += "&zip=" + EncodeValue(Settings["PostalCode"].ToString());
            strURL += "&zoom=" + RadioButtonList1.SelectedItem.Value;
            return strURL;
        }

        /// <summary>
        /// Gets the map image URL.
        /// </summary>
        /// <param name="strURL">The STR URL.</param>
        /// <returns></returns>
        private string GetMapImageURL(string strURL)
        {
            try
            {
                HttpWebRequest objRequest;
                objRequest = (HttpWebRequest) WebRequest.Create(strURL);
                HttpWebResponse objResponse;
                objResponse = (HttpWebResponse) objRequest.GetResponse();
                StreamReader sr;
                sr = new StreamReader(objResponse.GetResponseStream());
                string strResponse = sr.ReadToEnd();
                sr.Close();

                int intPos1;
                int intPos2;

                intPos1 = strResponse.IndexOf("mqmapgend");
                intPos1 = strResponse.LastIndexOf("http://", intPos1);
                intPos2 = strResponse.IndexOf("\"", intPos1);

                return strResponse.Substring(intPos1, intPos2 - intPos1);
            }
            catch
            {
                //error accessing MapQuest website 
                return string.Empty;
            }
        }


        /// <summary>
        /// Builds the directions URL.
        /// </summary>
        /// <returns></returns>
        private string BuildDirectionsURL()
        {
            string strURL;
            strURL = "http://www.mapquest.com/directions/main.adp?go=1";
            strURL += "&2a=" + EncodeValue(Settings["Street"].ToString());
            strURL += "&2c=" + EncodeValue(Settings["City"].ToString());
            strURL += "&2s=" + EncodeValue(Settings["Region"].ToString());
            strURL += "&2y=" + EncodeValue(Settings["Country"].ToString());
            strURL += "&2z=" + EncodeValue(Settings["PostalCode"].ToString());
            /*
            If Request.IsAuthenticated Then 
            ' if you have expanded the Users table in IBuySpy to include more demographic fields you can use them 
            ' here to automate the Get Directions feature 

            ' Obtain PortalSettings from Current Context 
            'Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings) 

            'Dim objUser As New ASPNetPortal.UsersDB() 

            'Dim drUser As SqlDataReader = objUser.GetSingleUser(_portalSettings.PortalID, Int32.Parse(context.User.Identity.Name)) 
            'If drUser.Read Then 
            ' strURL += "&1a=" & EncodeValue(drUser("Street").ToString) 
            ' strURL += "&1c=" & EncodeValue(drUser("City").ToString) 
            ' strURL += "&1s=" & EncodeValue(drUser("Region").ToString) 
            ' strURL += "&1y=" & EncodeValue(drUser("Country").ToString) 
            ' strURL += "&1z=" & EncodeValue(drUser("PostalCode").ToString) 
            'End If 
            'drUser.Close() 
            End If
            */

            return strURL;
        }


        /// <summary>
        /// Contstructor
        /// </summary>
        public MapQuest()
        {
            // modified by Hongwei Shen
            SettingItemGroup group = SettingItemGroup.MODULE_SPECIAL_SETTINGS;
            int groupBase = (int) group;

            var setLocation = new SettingItem<string, TextBox>();
            setLocation.Required = true;
            setLocation.Group = group;
            setLocation.Order = groupBase + 20; //10;
            this.BaseSettings.Add("Location", setLocation);

            var setStreet = new SettingItem<string, TextBox>();
            setStreet.Required = false;
            setStreet.Group = group;
            setStreet.Order = groupBase + 25; //20;
            this.BaseSettings.Add("Street", setStreet);

            var setCity = new SettingItem<string, TextBox>();
            setCity.Required = true;
            setCity.Group = group;
            setCity.Order = groupBase + 30;
            this.BaseSettings.Add("City", setCity);

            var setRegion = new SettingItem<string, TextBox>();
            setRegion.Required = false;
            setRegion.Group = group;
            setRegion.Order = groupBase + 35; //40;
            setRegion.Value = string.Empty; //Same as State for US
            this.BaseSettings.Add("Region", setRegion);

            var setCountry = new SettingItem<string, TextBox>();
            setCountry.Required = false;
            setCountry.Group = group;
            setCountry.Order = groupBase + 40; //50;
            this.BaseSettings.Add("Country", setCountry);

            var setPostalCode = new SettingItem<string, TextBox>();
            setPostalCode.Required = false;
            setPostalCode.Group = group;
            setPostalCode.Order = groupBase + 45; //60;
            this.BaseSettings.Add("PostalCode", setPostalCode);

            var setShowMap = new SettingItem<bool, CheckBox>();
            setShowMap.Group = group;
            setShowMap.Order = groupBase + 50; //70;
            setShowMap.Value = true;
            this.BaseSettings.Add("ShowMap", setShowMap);

            var setShowAddress = new SettingItem<bool, CheckBox>();
            setShowAddress.Group = group;
            setShowAddress.Order = groupBase + 55; //80;
            setShowAddress.Value = false;
            this.BaseSettings.Add("ShowAddress", setShowAddress);

            var setZoom = new SettingItem<int, TextBox>();
            setZoom.Required = true;
            setZoom.Group = group;
            setZoom.Order = groupBase + 60; //90;
            setZoom.Value = 7;
            setZoom.MinValue = 1;
            setZoom.MaxValue = 10;
            this.BaseSettings.Add("Zoom", setZoom);

            var setShowZoom = new SettingItem<bool, CheckBox>();
            setShowZoom.Group = group;
            setShowZoom.Order = groupBase + 65; //100;
            setShowAddress.Value = false;
            this.BaseSettings.Add("ShowZoom", setShowZoom);

            var setTarget = new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("blank;parent;self;top"));
            setTarget.Required = true;
            setTarget.Group = group;
            setTarget.Order = groupBase + 70; //110;
            setTarget.Value = "blank";
            this.BaseSettings.Add("Target", setTarget);
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531016}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// On init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.RadioButtonList1.SelectedIndexChanged += new EventHandler(this.zoom_Click);
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the zoom control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void zoom_Click(object sender, EventArgs e)
        {
            show();
        }
    }
}