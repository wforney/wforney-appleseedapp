namespace Appleseed.Framework
{
    /// <summary>
    /// SettingItemGroups, used to sort and group site and module
    /// settings in SettingsTable.
    /// </summary>
    public enum SettingItemGroup : int
    {
        /// <summary>
        /// 
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 
        /// </summary>
        THEME_LAYOUT_SETTINGS = 1000,
        /// <summary>
        /// 
        /// </summary>
        SECURITY_USER_SETTINGS = 2000,
        /// <summary>
        /// 
        /// </summary>
        CULTURE_SETTINGS = 3000,
        /// <summary>
        /// 
        /// </summary>
        BUTTON_DISPLAY_SETTINGS = 6000,
        /// <summary>
        /// 
        /// </summary>
        MODULE_SPECIAL_SETTINGS = 7000,
        /// <summary>
        /// 
        /// </summary>
        META_SETTINGS = 8000,
        /// <summary>
        /// 
        /// </summary>
        MISC_SETTINGS = 9000,
        /// <summary>
        /// 
        /// </summary>
        NAVIGATION_SETTINGS = 10000,
        /// <summary>
        /// 
        /// </summary>
        CUSTOM_USER_SETTINGS = 15000,
        /// <summary>Module Data Filter (aka. MDF).</summary>
        MDF_SETTINGS = 20000
    }
}