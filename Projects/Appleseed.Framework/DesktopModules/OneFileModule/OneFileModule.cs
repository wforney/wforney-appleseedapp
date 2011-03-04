using System.IO;
using System.Web.UI;
using System.Xml;
using Appleseed.Framework;

using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
	using Appleseed.Framework.Web.UI.WebControls;

	/// <summary>
	/// The OneFileModule provides basis for a module that only
	/// consists of a single .ascx file. 
	/// See the OneFileModule Kit for documentation and examples.
	/// Written by: Jakob Hansen, hansen3000@hotmail
	/// </summary>
	public class OneFileModule : Appleseed.Framework.Web.UI.WebControls.PortalModuleControl
	{
        /// <summary>
        /// 
        /// </summary>
		public enum SettingsType { Off, Str, Xml, StrAndXml }   // With StrAndXml SettingsStr are searched first
		/// <summary>
		/// 
		/// </summary>
        protected SettingsType _settingsType = SettingsType.Off;
        /// <summary>
        /// The content of setting "Settings string"
        /// </summary>
		protected string _settingsStr = string.Empty;      // 
        /// <summary>
        /// The filename in setting "XML settings file"
        /// </summary>
		protected string _xmlFileName = string.Empty;      // 
        /// <summary>
        /// The XML in file _xmlFileName
        /// </summary>
		protected XmlDocument _settingsXml;      // 
        /// <summary>
        /// True if setting "Debug Mode" is clicked
        /// </summary>
		protected bool _debugMode = false;       // 
        /// <summary>
        /// False if settings are missing
        /// </summary>
		protected bool _settingsExists = false;  // 


        /// <summary>
        /// Same as "Settings string" in the setting system
        /// </summary>
        /// <value>The settings STR.</value>
		public string SettingsStr
		{
			get { return _settingsStr; }
		}

        /// <summary>
        /// Same as "XML settings file" in the setting system
        /// </summary>
        /// <value>The name of the XML file.</value>
		public string XmlFileName
		{
			get { return _xmlFileName; }
		}

        /// <summary>
        /// The XML in file XmlFileName
        /// </summary>
        /// <value>The settings XML.</value>
		public XmlDocument SettingsXml
		{
			get { return _settingsXml; }
		}

        /// <summary>
        /// Same as "Debug Mode" in the setting system.
        /// </summary>
        /// <value><c>true</c> if [debug mode]; otherwise, <c>false</c>.</value>
		public bool DebugMode
		{
			get { return _debugMode; }
			set { _debugMode = value; }
		}

        /// <summary>
        /// False if settings are missing
        /// </summary>
        /// <value><c>true</c> if [settings exists]; otherwise, <c>false</c>.</value>
		public bool SettingsExists
		{
			get { return _settingsExists; }
		}


        /// <summary>
        /// Fills all settings: SettingsStr, SettingsXml and DebugMode
        /// InitSettings() should be the first line of code in Page_Load().
        /// If settings is missing settingsExists is set to false
        /// Note: It is not mandatory to use InitSettings() in the
        /// Page_Load() - The programmer can just leave it out if he
        /// decides that he is not going to use the setting system that
        /// class OneFileModule provides.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
		protected void InitSettings(SettingsType settingsType)
		{
			_settingsType = settingsType;
			if (_settingsType == SettingsType.Off)
				return;

			_debugMode = "True" == Settings["Debug Mode"].ToString();
			_xmlFileName = Settings["XML settings file"].ToString();

			_settingsExists = true;

			if (_settingsType == SettingsType.Str || _settingsType == SettingsType.StrAndXml)
			{
				_settingsStr = Settings["Settings string"].ToString();
				if (_settingsStr == string.Empty)
				{
					_settingsExists = false;
					Controls.Add(new LiteralControl("<br><span class='Error'>Settings string is missing</span><br>"));
				}
			}

			if (_settingsType == SettingsType.Xml || _settingsType == SettingsType.StrAndXml)
			{
				_settingsXml = new XmlDocument();
				if (GetSettingsXml(ref _settingsXml, _xmlFileName) == false)
				{
					_settingsExists = false;
                    Controls.Add(new LiteralControl("<br>" + "<span class='Error'>XML " + General.GetString("FILE_NOT_FOUND").Replace("%1%", _xmlFileName) + "<br>"));
				}
			}

		}

        /// <summary>
        /// Creates 3 fields in the settings system:
        /// "Settings string", "XML settings file" and "Debug Mode"
        /// </summary>
		public OneFileModule()
		{
			SettingItem setting = new SettingItem(new StringDataType());
			setting.Required = false;
			setting.Order = 1;
			setting.Value = string.Empty;
			setting.EnglishName = "Settings string";
			setting.Description = "Settings are in pairs like: FirstName=Elvis;LastName=Presly;";
			this._baseSettings.Add("Settings string", setting);

			SettingItem xmlFile = new SettingItem(new PortalUrlDataType());
			xmlFile.Required = false;
			xmlFile.Order = 2;
			xmlFile.EnglishName = "XML settings file";
			xmlFile.Description = "Name of file in folder Appleseed\\_Portalfolder (typically _Appleseed). Do not add a path!";
			this._baseSettings.Add("XML settings file", xmlFile);

			SettingItem debugMode = new SettingItem(new BooleanDataType());
			debugMode.Order = 3;
			debugMode.Value = "True";
			debugMode.EnglishName = "Debug Mode";
			debugMode.Description = "Primarily for the developer. Controls property DebugMode";
			this._baseSettings.Add("Debug Mode", debugMode);
		}


        /// <summary>
        /// Get the setting value from SettingsStr.
        /// If not found in SettingsStr then SettingsXml are searched.
        /// This function uses GetStrSetting() and GetXmlSetting
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
		protected string GetSetting(string settingName)
		{
			if (_settingsType == SettingsType.Off)
				return string.Empty;

			string retVal;
			retVal = GetStrSetting(settingName);
			if (retVal == string.Empty)
				retVal = GetXmlSetting(settingName);
			return retVal;
		}


        /// <summary>
        /// Get the setting value from SettingsStr which has the form:
        /// nameA=valueA;nameB=valueB;nameC=valueC
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
		protected string GetStrSetting(string settingName)
		{
			if (_settingsType == SettingsType.Off ||
				_settingsType == SettingsType.Xml ||
				_settingsStr == string.Empty)
				return string.Empty;

			int idxStart = _settingsStr.IndexOf(settingName);
			if (idxStart == -1)
				return string.Empty;

			idxStart = _settingsStr.IndexOf('=', idxStart);
			if (idxStart == -1)
				return string.Empty;

			string val;
			int idxEnd = _settingsStr.IndexOf(';', idxStart);
			if (idxEnd == -1)
			{
				if (_settingsStr.Substring(idxStart).Length == 0)
					val = string.Empty;
				else
					val = _settingsStr.Substring(++idxStart);
			}
			else
			{
				idxStart++;
				val = _settingsStr.Substring(idxStart, (idxEnd - idxStart));
			}

			return val;
		}


        /// <summary>
        /// Fills the settingsXml parameter with the xml from a file
        /// </summary>
        /// <param name="settingsXml">The settings XML.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
		protected bool GetSettingsXml(ref XmlDocument settingsXml, string file)
		{
			bool retValue = true;

			PortalUrlDataType pt = new PortalUrlDataType();
			pt.Value = file;
			string xmlFile = pt.FullPath;

			if ((xmlFile != null) && (xmlFile.Length != 0))
			{
				if (File.Exists(Server.MapPath(xmlFile)))
				{
					settingsXml.Load(Server.MapPath(xmlFile));
				}
				else
				{
					retValue = false;
				}
			}
			return retValue;
		}


        /// <summary>
        /// Get the setting value from the XML Document SettingsXml.
        /// settingName is a XPath expression.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
		protected string GetXmlSetting(string settingName)
		{
			if (_settingsType == SettingsType.Off || _settingsType == SettingsType.Str)
				return string.Empty;

			string val = string.Empty;

			// Add default root to the xpath expression if missing
			if (settingName.IndexOf('/') == -1)
				settingName = "settings/" + settingName;

			XmlNodeReader xmlNodeReader = new XmlNodeReader(_settingsXml.SelectSingleNode(settingName));

			try
			{
				if (xmlNodeReader.Read())
				{
					// If we get here the setting is in the xml file
					// Move to next node (the text node containg the actual setting value)
					if (xmlNodeReader.Read())
						val = xmlNodeReader.Value;
				}
			}
			finally
			{
				xmlNodeReader.Close(); //by Manu, fixed bug 807858
			}


			return val;
		}

	}
}