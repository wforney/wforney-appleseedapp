using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Framework.Site.Configuration
{
	/// <summary>
	/// ModuleSettingsCustom extends the ModuleSettings class to allow authenticated users
	/// to 'customize' a module to their own preference.
	/// </summary>
	public class ModuleSettingsCustom : ModuleSettings
	{
		const string strDesktopSrc = "DesktopSrc";

		#region Data Access Methods used only by PortalModuleControlCustom

		/// <summary>
		/// The GetModuleSettings Method returns a hashtable of
		/// custom module specific settings from the database. This method is
		/// used by some user control modules to access misc settings.
		/// </summary>
		/// <param name="moduleID">The module ID.</param>
		/// <param name="userID">The user ID.</param>
		/// <param name="page">The page.</param>
		/// <returns></returns>
		public static Hashtable GetModuleUserSettings(int moduleID, Guid userID, Page page)
		{
			string ControlPath = Path.ApplicationRoot + "/";

			using (SqlDataReader dr = GetModuleDefinitionByID(moduleID))
			{
				if (dr.Read())
				{
					ControlPath += dr[strDesktopSrc].ToString();
				}
			}

			PortalModuleControlCustom portalModule;
			Hashtable setting;
			try
			{
				portalModule = (PortalModuleControlCustom)page.LoadControl(ControlPath);
				setting = GetModuleUserSettings(moduleID, (Guid)PortalSettings.CurrentUser.Identity.ProviderUserKey, portalModule.CustomizedUserSettings);
			}
			catch (Exception ex)
			{
				// Appleseed.Framework.Configuration.ErrorHandler.HandleException("There was a problem loading: '" + ControlPath + "'", ex);
				// throw;
				throw new Appleseed.Framework.Exceptions.AppleseedException(Appleseed.Framework.LogLevel.Fatal, "There was a problem loading: '" + ControlPath + "'", ex);
			}
			return setting;
		}

        /// <summary>
        /// Retrieves the custom user settings for the current user for this module
        /// from the database.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="_customSettings">The _custom settings.</param>
        /// <returns></returns>
		public static Hashtable GetModuleUserSettings(int moduleID, Guid userID, Hashtable _customSettings)
		{
			// Get Settings for this module from the database
			Hashtable _settings = new Hashtable();

			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_GetModuleUserSettings", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);

					// Execute the command
					myConnection.Open();
					using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (dr.Read())
						{
							_settings[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
						}
					}
				}
			}


			//foreach (string key in _customSettings.Keys)
			foreach (string key in _customSettings.Keys)
			{
				if (_settings[key] != null)
				{
					SettingItem s = ((SettingItem)_customSettings[key]);
					if (_settings[key].ToString().Length != 0)
						s.Value = _settings[key].ToString();
					//_customSettings[key] = s;
				}
			}

			return _customSettings;
		}

		/// <summary>
		/// The UpdateCustomModuleSetting Method updates a single module setting
		/// for the current user in the rb_ModuleUserSettings database table.
		/// </summary>
		/// <param name="moduleID">The module ID.</param>
		/// <param name="userID">The user ID.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public static void UpdateCustomModuleSetting(int moduleID, Guid userID, string key, string value)
		{
			// Create Instance of Connection and Command Object
			using (SqlConnection myConnection = Config.SqlConnectionString)
			{
				using (SqlCommand myCommand = new SqlCommand("rb_UpdateModuleUserSetting", myConnection))
				{
					// Mark the Command as a SPROC
					myCommand.CommandType = CommandType.StoredProcedure;

					// Add Parameters to SPROC
					SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
					parameterModuleID.Value = moduleID;
					myCommand.Parameters.Add(parameterModuleID);

					SqlParameter parameterUserID = new SqlParameter("@UserID", SqlDbType.Int, 4);
					parameterUserID.Value = userID;
					myCommand.Parameters.Add(parameterUserID);

					SqlParameter parameterKey = new SqlParameter("@SettingName", SqlDbType.NVarChar, 50);
					parameterKey.Value = key;
					myCommand.Parameters.Add(parameterKey);

					SqlParameter parameterValue = new SqlParameter("@SettingValue", SqlDbType.NVarChar, 1500);
					parameterValue.Value = value;
					myCommand.Parameters.Add(parameterValue);

					myConnection.Open();
					myCommand.ExecuteNonQuery();
				}
			}
		}
		#endregion

	}
}