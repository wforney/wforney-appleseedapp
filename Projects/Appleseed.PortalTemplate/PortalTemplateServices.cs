using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using Appleseed.PortalTemplate.DTOs;
using System.Collections;
using System.Data.Linq;
using System.Web.UI;
using Appleseed.Framework.Web.UI.WebControls;
using System.Security.AccessControl;
using Appleseed.Framework.DataTypes;
using System.Text.RegularExpressions;
using Appleseed.Framework;

namespace Appleseed.PortalTemplate
{
    public class PortalTemplateServices : IPortalTemplateServices
    {
        private IPortalTemplateRepository _iptRepository;

        public PortalTemplateServices(IPortalTemplateRepository iptRepository)
        {
            _iptRepository = iptRepository;
        }


        public bool SerializePortal(int portalID, string path)
        {
            bool result = true;
            try {
                PortalsDTO portal = _iptRepository.GetPortal(portalID);
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists) {
                    dir.Create();
                }

                FileStream fs = new FileStream(path + portal.PortalAlias + DateTime.Now.ToString("dd-MM-yyyy") + ".XML", FileMode.Create);
                XmlSerializer xs = new XmlSerializer(typeof(PortalsDTO));
                xs.Serialize(fs, portal);
                fs.Close();
            } catch (Exception ex) {
                result = false;
                ErrorHandler.Publish(LogLevel.Error, "There was an error on serialize the portal", ex);
            }
            return result;
        }

        public bool DeserializePortal(string file, string portalName, string portalAlias, string portalPath, out int portalId)
        {
            bool result = true;
            try {
                FileStream fs = new FileStream(file, FileMode.Open);
                XmlSerializer xs = new XmlSerializer(typeof(PortalsDTO));
                PortalsDTO portal = (PortalsDTO)xs.Deserialize(fs);
                fs.Close();

                PortalTemplateDataContext db = new PortalTemplateDataContext();
                Dictionary<Guid,string> desktopSources = _iptRepository.GetDesktopSources();

                Translate _translate = new Translate();
                _translate.DesktopSources = desktopSources;
                _translate.PTDataContext = db;
                rb_Portals _portal = _translate.TranslatePortalDTOIntoRb_Portals(portal);
                this.ModulesNotInserted = _translate.ModulesNotInserted;

                _portal.PortalName = portalName;
                _portal.PortalPath = portalPath;
                _portal.PortalAlias = portalAlias;
                
                db.rb_Portals.InsertOnSubmit(_portal);

                try {
                    db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    portalId = _portal.PortalID;
                    this.SaveModuleContent(_portal, desktopSources, _translate.ContentModules);
                    this.AlterModuleSettings(_portal, _translate.PageList, desktopSources);
                    this.AlterModuleDefinitions(_portal.PortalID, _translate.ModuleDefinitionsDeserialized);


                } catch (Exception e) {
                    result = false;
                    portalId = -1;
                    ErrorHandler.Publish(LogLevel.Error, "There was an error on creating the portal", e);
                }


            } catch (Exception ex) {
                result = false;
                portalId = -1;
                ErrorHandler.Publish(LogLevel.Error, "There was an error on creating the portal", ex);
            }
            return result;
        }

        private void AlterModuleDefinitions(int portalID, Dictionary<Guid, rb_ModuleDefinition> moduleDefinitions)
        {
            PortalTemplateDataContext db = new PortalTemplateDataContext();
            List<Guid> keys = moduleDefinitions.Keys.ToList<Guid>();
            try {

                foreach (Guid key in keys) {
                    rb_ModuleDefinition _oldDef;
                    moduleDefinitions.TryGetValue(key, out _oldDef);
                    rb_ModuleDefinition _def = (from d in db.rb_ModuleDefinitions
                                                where d.PortalID == _oldDef.PortalID
                                                    && d.GeneralModDefID == key
                                                    && d.ModuleDefID != _oldDef.ModuleDefID
                                                select d).First();
                    _def.PortalID = portalID;
                }
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            } catch (Exception e) {
                ErrorHandler.Publish(LogLevel.Error, "There was an error on modifing the module definitions", e);
            }
        }

        private void AlterModuleSettings(rb_Portals _portal, Dictionary<int, int> pageList,Dictionary<Guid, string> desktopSources)
        {
            Page p = new Page();
            PortalTemplateDataContext db = new PortalTemplateDataContext();
            List<rb_Modules> _modules = (from m in db.rb_Modules where m.rb_Pages.PortalID == _portal.PortalID select m).ToList<rb_Modules>();
            foreach (rb_Modules _module in _modules) {
                string portalModuleName = string.Concat(Appleseed.Framework.Settings.Path.ApplicationRoot, "/", desktopSources[_module.rb_ModuleDefinition.GeneralModDefID]);
                PortalModuleControl portalModule = (PortalModuleControl)p.LoadControl(portalModuleName);
                foreach (string key in portalModule.BaseSettings.Keys) {
                    if (key.Equals("TARGETURL") || ((SettingItem) portalModule.BaseSettings[key]).DataType== PropertiesDataType.PageList) {
                        try {
                            rb_ModuleSettings setting = _module.rb_ModuleSettings.First(s => s.SettingName.Equals(key));
                            string oldPageId = Regex.Match(setting.SettingValue, "(/\\d+/)|(^\\d+$)", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Value.Replace("/", string.Empty);
                            int newPageId = _portal.rb_Pages[pageList[Convert.ToInt16(oldPageId)]].PageID;
                            setting.SettingValue = setting.SettingValue.Replace(oldPageId, newPageId.ToString());
                        } catch (Exception e) {
                            ErrorHandler.Publish(LogLevel.Error, "There was an error on modifing the module settings for moduleID= " + _module.ModuleID + " and setting= " + key, e);
                        }
                    }
                }
            }
            try {
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            } catch (Exception e) {
                ErrorHandler.Publish(LogLevel.Error, "There was an error on modifing the module settings", e);
            }
        }

        private void SaveModuleContent(rb_Portals _portal, Dictionary<Guid, string> desktopSources,Dictionary<int, string> contentModules)
        {
            Page p = new Page();
            int moduleIndex = 0;
            foreach (rb_Pages _page in _portal.rb_Pages) {
                foreach (rb_Modules _module in _page.rb_Modules) {
                    if (contentModules.ContainsKey(moduleIndex)) {
                        string portalModuleName = string.Concat(Appleseed.Framework.Settings.Path.ApplicationRoot, "/", desktopSources[_module.rb_ModuleDefinition.GeneralModDefID]);
                        PortalModuleControl portalModule = (PortalModuleControl)p.LoadControl(portalModuleName);
                        if (portalModule is IModuleExportable) {
                            string content;
                            if (!contentModules.TryGetValue(moduleIndex, out content) || !((IModuleExportable)portalModule).SetContentData(_module.ModuleID, content)){
                                ModulesNotInserted.Add(_module.ModuleID, _module.ModuleTitle);
                            }
                        }
                    }
                    moduleIndex++;
                }
            }
            
        }


        public HtmlTextDTO GetHtmlTextDTO(int moduleId)
        {
            return _iptRepository.GetHtmlTextDTO(moduleId);
        }

        public bool SaveHtmlText(int moduleId, HtmlTextDTO html)
        {
            bool result = true;
            try {
                PortalTemplateDataContext db = new PortalTemplateDataContext();
                Translate _translate = new Translate();
                rb_HtmlText _html = _translate.TranslateHtmlTextDTOIntoRb_HtmlText(html);
                _html.ModuleID = moduleId;
                rb_HtmlText_st _htmlst = _translate.TranslateHtmlTextDTOIntoRb_HtmlText_st(html);
                _htmlst.ModuleID = moduleId;
                db.rb_HtmlTexts.InsertOnSubmit(_html);
                db.rb_HtmlText_sts.InsertOnSubmit(_htmlst);
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
            } catch (Exception ex) {
                result = false;
                ErrorHandler.Publish(LogLevel.Error, "There was an error saving the content modules", ex);
            }
            return result;
        }


        #region properties
        public Dictionary<int, string> ModulesNotInserted
        {
            get;
            set;
        }

        #endregion

    }
}
