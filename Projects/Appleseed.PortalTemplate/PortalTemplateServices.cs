// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalTemplateServices.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   The portal template services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.PortalTemplate
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Xml.Serialization;

    using Appleseed.Framework;
    using Appleseed.Framework.DataTypes;
    using Appleseed.Framework.Web.UI.WebControls;
    using Appleseed.PortalTemplate.DTOs;

    using Path = Appleseed.Framework.Settings.Path;

    /// <summary>
    /// The portal template services.
    /// </summary>
    public class PortalTemplateServices : IPortalTemplateServices
    {
        #region Constants and Fields

        /// <summary>
        /// The ipt repository.
        /// </summary>
        private readonly IPortalTemplateRepository iptRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalTemplateServices"/> class.
        /// </summary>
        /// <param name="iptRepository">
        /// The ipt repository.
        /// </param>
        public PortalTemplateServices(IPortalTemplateRepository iptRepository)
        {
            this.iptRepository = iptRepository;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the modules not inserted.
        /// </summary>
        /// <value>
        ///   The modules not inserted.
        /// </value>
        public Dictionary<int, string> ModulesNotInserted { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IPortalTemplateServices

        /// <summary>
        /// Deserializes the portal.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="portalName">
        /// Name of the portal.
        /// </param>
        /// <param name="portalAlias">
        /// The portal alias.
        /// </param>
        /// <param name="portalPath">
        /// The portal path.
        /// </param>
        /// <param name="portalId">
        /// The portal id.
        /// </param>
        /// <returns>
        /// The deserialize portal.
        /// </returns>
        public bool DeserializePortal(
            string file, string portalName, string portalAlias, string portalPath, out int portalId)
        {
            var result = true;
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var xs = new XmlSerializer(typeof(PortalsDTO));
                var portal = (PortalsDTO)xs.Deserialize(fs);
                fs.Close();

                var db = new PortalTemplateDataContext();
                var desktopSources = this.iptRepository.GetDesktopSources();

                var translate = new Translate { DesktopSources = desktopSources, PTDataContext = db };
                var _portal = translate.TranslatePortalDTOIntoRb_Portals(portal);
                this.ModulesNotInserted = translate.ModulesNotInserted;

                _portal.PortalName = portalName;
                _portal.PortalPath = portalPath;
                _portal.PortalAlias = portalAlias;

                db.rb_Portals.InsertOnSubmit(_portal);

                try
                {
                    db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    portalId = _portal.PortalID;
                    this.SaveModuleContent(_portal, desktopSources, translate.ContentModules);
                    AlterModuleSettings(_portal, translate.PageList, desktopSources);
                    AlterModuleDefinitions(_portal.PortalID, translate.ModuleDefinitionsDeserialized);
                }
                catch (Exception e)
                {
                    result = false;
                    portalId = -1;
                    ErrorHandler.Publish(LogLevel.Error, "There was an error on creating the portal", e);
                }
            }
            catch (Exception ex)
            {
                result = false;
                portalId = -1;
                ErrorHandler.Publish(LogLevel.Error, "There was an error on creating the portal", ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the HTML text DTO.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <returns>
        /// </returns>
        public HtmlTextDTO GetHtmlTextDTO(int moduleId)
        {
            return this.iptRepository.GetHtmlTextDTO(moduleId);
        }

        /// <summary>
        /// Saves the HTML text.
        /// </summary>
        /// <param name="moduleId">
        /// The module id.
        /// </param>
        /// <param name="html">
        /// The HTML.
        /// </param>
        /// <returns>
        /// The save html text.
        /// </returns>
        public bool SaveHtmlText(int moduleId, HtmlTextDTO html)
        {
            var result = true;
            try
            {
                var db = new PortalTemplateDataContext();
                var translate = new Translate();
                var htmlText = translate.TranslateHtmlTextDTOIntoRb_HtmlText(html);
                htmlText.ModuleID = moduleId;
                var htmlst = translate.TranslateHtmlTextDTOIntoRb_HtmlText_st(html);
                htmlst.ModuleID = moduleId;
                db.rb_HtmlTexts.InsertOnSubmit(htmlText);
                db.rb_HtmlText_sts.InsertOnSubmit(htmlst);
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.Publish(LogLevel.Error, "There was an error saving the content modules", ex);
            }

            return result;
        }

        /// <summary>
        /// Serializes the portal.
        /// </summary>
        /// <param name="portalId">
        /// The portal ID.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The serialize portal.
        /// </returns>
        public bool SerializePortal(int portalId, string path)
        {
            var result = true;
            try
            {
                var portal = this.iptRepository.GetPortal(portalId);
                var dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                var fs = new FileStream(
                    path + portal.PortalAlias + DateTime.Now.ToString("dd-MM-yyyy") + ".XML", FileMode.Create);
                var xs = new XmlSerializer(typeof(PortalsDTO));
                xs.Serialize(fs, portal);
                fs.Close();
            }
            catch (Exception ex)
            {
                result = false;
                ErrorHandler.Publish(LogLevel.Error, "There was an error on serialize the portal", ex);
            }

            return result;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Alters the module definitions.
        /// </summary>
        /// <param name="portalId">
        /// The portal id.
        /// </param>
        /// <param name="moduleDefinitions">
        /// The module definitions.
        /// </param>
        private static void AlterModuleDefinitions(int portalId, Dictionary<Guid, rb_ModuleDefinition> moduleDefinitions)
        {
            var db = new PortalTemplateDataContext();
            var keys = moduleDefinitions.Keys.ToList();
            try
            {
                foreach (var key in keys)
                {
                    rb_ModuleDefinition oldDef;
                    moduleDefinitions.TryGetValue(key, out oldDef);
                    var def =
                        db.rb_ModuleDefinitions.Where(
                            d =>
                            d.PortalID == oldDef.PortalID && d.GeneralModDefID == key &&
                            d.ModuleDefID != oldDef.ModuleDefID).First();
                    def.PortalID = portalId;
                }

                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (Exception e)
            {
                ErrorHandler.Publish(LogLevel.Error, "There was an error on modifying the module definitions", e);
            }
        }

        /// <summary>
        /// Alters the module settings.
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        /// <param name="pageList">
        /// The page list.
        /// </param>
        /// <param name="desktopSources">
        /// The desktop sources.
        /// </param>
        private static void AlterModuleSettings(
            rb_Portals portal, IDictionary<int, int> pageList, IDictionary<Guid, string> desktopSources)
        {
            var p = new Page();
            var db = new PortalTemplateDataContext();
            var modules = db.rb_Modules.Where(m => m.rb_Pages.PortalID == portal.PortalID).ToList();
            foreach (var module in modules)
            {
                var portalModuleName = string.Concat(
                    Path.ApplicationRoot, "/", desktopSources[module.rb_ModuleDefinition.GeneralModDefID]);
                var portalModule = (PortalModuleControl)p.LoadControl(portalModuleName);
                foreach (var key in
                    portalModule.BaseSettings.Keys.Cast<string>().Where(
                        key =>
                        key.Equals("TARGETURL") ||
                        portalModule.BaseSettings[key] is PageListDataType))
                {
                    try
                    {
                        var setting = module.rb_ModuleSettings.First(s => s.SettingName.Equals(key));
                        var oldPageId =
                            Regex.Match(setting.SettingValue, "(/\\d+/)|(^\\d+$)", RegexOptions.IgnoreCase).Value.
                                Replace("/", string.Empty);
                        var newPageId = portal.rb_Pages[pageList[Convert.ToInt16(oldPageId)]].PageID;
                        setting.SettingValue = setting.SettingValue.Replace(oldPageId, newPageId.ToString());
                    }
                    catch (Exception e)
                    {
                        ErrorHandler.Publish(
                            LogLevel.Error, 
                            string.Format(
                                "There was an error on modifying the module settings for moduleID= {0} and setting= {1}", 
                                module.ModuleID, 
                                key), 
                            e);
                    }
                }
            }

            try
            {
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (Exception e)
            {
                ErrorHandler.Publish(LogLevel.Error, "There was an error on modifying the module settings", e);
            }
        }

        /// <summary>
        /// Saves the content of the module.
        /// </summary>
        /// <param name="portal">
        /// The portal.
        /// </param>
        /// <param name="desktopSources">
        /// The desktop sources.
        /// </param>
        /// <param name="contentModules">
        /// The content modules.
        /// </param>
        private void SaveModuleContent(
            rb_Portals portal, IDictionary<Guid, string> desktopSources, IDictionary<int, string> contentModules)
        {
            var p = new Page();
            var moduleIndex = 0;
            foreach (var module in portal.rb_Pages.SelectMany(page => page.rb_Modules))
            {
                if (contentModules.ContainsKey(moduleIndex))
                {
                    var portalModuleName = string.Concat(
                        Path.ApplicationRoot, "/", desktopSources[module.rb_ModuleDefinition.GeneralModDefID]);
                    var portalModule = (PortalModuleControl)p.LoadControl(portalModuleName);
                    if (portalModule is IModuleExportable)
                    {
                        string content;
                        if (!contentModules.TryGetValue(moduleIndex, out content) ||
                            !((IModuleExportable)portalModule).SetContentData(module.ModuleID, content))
                        {
                            this.ModulesNotInserted.Add(module.ModuleID, module.ModuleTitle);
                        }
                    }
                }

                moduleIndex++;
            }
        }

        #endregion
    }
}