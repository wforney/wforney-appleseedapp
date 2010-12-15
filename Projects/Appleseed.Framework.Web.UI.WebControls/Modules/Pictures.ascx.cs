using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using Appleseed.Framework;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Design;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Web.UI.WebControls;
using Label=Appleseed.Framework.Web.UI.WebControls.Label;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Appleseed Portal Pictures module
    /// (c)2002 by Ender Malkoc
    /// </summary>
    [History("Mario Hartmann", "mario@hartmann.net", "2.3 beta", "2003/10/08", "moved to seperate folder")]
    public class Pictures : PortalModuleControl
    {
        /// <summary>
        /// Datalist for pictures
        /// </summary>
        protected DataList dlPictures;

        /// <summary>
        /// Error label
        /// </summary>
        protected Label lblError;

        /// <summary>
        /// Paging for the pictures
        /// </summary>
        protected Paging pgPictures;

        /// <summary>
        /// Resize Options
        /// NoResize : Do not resize the picture
        /// FixedWidthHeight : Use the width and height specified. 
        /// MaintainAspectWidth : Use the specified height and calculate height using the original aspect ratio
        /// MaintainAspectHeight : Use the specified width and calculate width using the original aspect ration
        /// </summary>
        public enum ResizeOption
        {
            /// <summary>
            /// No resizing
            /// </summary>
            NoResize,
            /// <summary>
            /// FixedWidthHeight : Use the width and height specified. 
            /// </summary>
            FixedWidthHeight,
            /// <summary>
            /// MaintainAspectWidth : Use the specified height and calculate height using the original aspect ratio
            /// </summary>
            MaintainAspectWidth,
            /// <summary>
            /// MaintainAspectHeight : Use the specified width and calculate width using the original aspect ration
            /// </summary>
            MaintainAspectHeight
        }

        /// <summary>
        /// The Page_Load event on this page calls the BindData() method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            dlPictures.RepeatDirection = (Settings["RepeatDirectionSetting"] == null
                                              ? RepeatDirection.Horizontal
                                              :
                                          (RepeatDirection)
                                          Int32.Parse(((SettingItem) _baseSettings["RepeatDirectionSetting"])));
            dlPictures.RepeatColumns = Int32.Parse(((SettingItem) Settings["RepeatColumns"]));
            dlPictures.ItemDataBound += new DataListItemEventHandler(Pictures_ItemDataBound);
            pgPictures.RecordsPerPage = Int32.Parse(Settings["PicturesPerPage"].ToString());
            BindData(pgPictures.PageNumber);
        }

        private void Page_Changed(object sender, EventArgs e)
        {
            BindData(pgPictures.PageNumber);
        }

        private void Pictures_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            PictureItem pictureItem;
            try
            {
                pictureItem =
                    (PictureItem)
                    Page.LoadControl(Path.ApplicationRoot + "/Design/PictureLayouts/" + Settings["ThumbnailLayout"]);
            }
            catch
            {
                lblError.Visible = true;
                dlPictures.Visible = false;
                pgPictures.Visible = false;
                return;
            }

            DataRowView di = (DataRowView) e.Item.DataItem;

            XmlDocument metadata = new XmlDocument();
            metadata.LoadXml((string) di["MetadataXml"]);

            XmlAttribute albumPath = metadata.CreateAttribute("AlbumPath");
            albumPath.Value = ((SettingItem) Settings["AlbumPath"]).FullPath;

            XmlAttribute itemID = metadata.CreateAttribute("ItemID");
            itemID.Value = ((int) di["ItemID"]).ToString();

            XmlAttribute moduleID = metadata.CreateAttribute("ModuleID");
            moduleID.Value = ModuleID.ToString();

            XmlAttribute wVersion = metadata.CreateAttribute("WVersion");
            wVersion.Value = Version.ToString();

            XmlAttribute isEditable = metadata.CreateAttribute("IsEditable");
            isEditable.Value = IsEditable.ToString();

            metadata.DocumentElement.Attributes.Append(albumPath);
            metadata.DocumentElement.Attributes.Append(itemID);
            metadata.DocumentElement.Attributes.Append(moduleID);
            metadata.DocumentElement.Attributes.Append(isEditable);
            metadata.DocumentElement.Attributes.Append(wVersion);

            if (Version == WorkFlowVersion.Production)
            {
                XmlNode modifiedFilenameNode = metadata.DocumentElement.SelectSingleNode("@ModifiedFilename");
                XmlNode thumbnailFilenameNode = metadata.DocumentElement.SelectSingleNode("@ThumbnailFilename");

                modifiedFilenameNode.Value = modifiedFilenameNode.Value.Replace(".jpg", ".Production.jpg");
                thumbnailFilenameNode.Value = thumbnailFilenameNode.Value.Replace(".jpg", ".Production.jpg");
            }

            pictureItem.Metadata = metadata;
            pictureItem.DataBind();
            e.Item.Controls.Add(pictureItem);
        }

        /// <summary>
        /// The Binddata method on this User Control is used to
        /// obtain a DataReader of picture information from the Pictures
        /// table, and then databind the results to a templated DataList
        /// server control. It uses the Appleseed.PictureDB()
        /// data component to encapsulate all data functionality.
        /// </summary>
        private void BindData(int page)
        {
            PicturesDB pictures = new PicturesDB();
            DataSet dsPictures =
                pictures.GetPicturesPaged(ModuleID, page, Int32.Parse(Settings["PicturesPerPage"].ToString()), Version);

            if (dsPictures.Tables.Count > 0 && dsPictures.Tables[0].Rows.Count > 0)
            {
                pgPictures.RecordCount = (int) (dsPictures.Tables[0].Rows[0]["RecordCount"]);
            }

            dlPictures.DataSource = dsPictures;
            dlPictures.DataBind();
        }

        /// <summary>
        /// Overriden from PortalModuleControl, this override deletes unnecessary picture files from the system
        /// </summary>
        protected override void Publish()
        {
            string pathToDelete = Server.MapPath(((SettingItem) Settings["AlbumPath"]).FullPath) + "\\";

            DirectoryInfo albumDirectory = new DirectoryInfo(pathToDelete);

            foreach (FileInfo fi in albumDirectory.GetFiles(ModuleID.ToString() + "m*.Production.jpg"))
            {
                try
                {
                    File.Delete(fi.FullName);
                }
                catch
                {
                }
            }

            foreach (FileInfo fi in albumDirectory.GetFiles(ModuleID.ToString() + "m*"))
            {
                try
                {
                    File.Copy(fi.FullName, fi.FullName.Replace(".jpg", ".Production.jpg"), true);
                }
                catch
                {
                }
            }

            base.Publish();
        }

        /// <summary>
        /// Given a key returns the value
        /// </summary>
        /// <param name="MetadataXml">XmlDocument containing key value pairs in attributes</param>
        /// <param name="key">key of the pair</param>
        /// <returns>value</returns>
        protected string GetMetadata(object MetadataXml, string key)
        {
            XmlDocument Metadata = new XmlDocument();
            Metadata.LoadXml((string) MetadataXml);

            XmlNode targetNode = Metadata.SelectSingleNode("/Metadata/@" + key);
            if (targetNode == null)
            {
                return null;
            }
            else
            {
                return targetNode.Value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        [
            History("Tim Capps", "tim@cappsnet.com", "2.4 beta", "2004/02/18",
                "fixed order on settings and added ShowBulkLoad")]
        public Pictures()
        {
            // Add support for workflow
            SupportsWorkflow = true;

            // Album Path Setting
            SettingItem AlbumPath = new SettingItem(new PortalUrlDataType());
            AlbumPath.Required = true;
            AlbumPath.Value = "Album";
            AlbumPath.Order = 3;
            _baseSettings.Add("AlbumPath", AlbumPath);

            // Thumbnail Resize Options
            ArrayList thumbnailResizeOptions = new ArrayList();
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.FixedWidthHeight,
                           General.GetString("PICTURES_FIXED_WIDTH_AND_HEIGHT", "Fixed width and height", this)));
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectWidth,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_WIDTH", "Maintain aspect fixed width", this)));
            thumbnailResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectHeight,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_HEIGHT", "Maintain aspect fixed height",
                                             this)));

            // Thumbnail Resize Settings
            SettingItem ThumbnailResize = new SettingItem(new CustomListDataType(thumbnailResizeOptions, "Name", "Val"));
            ThumbnailResize.Required = true;
            ThumbnailResize.Value = ((int) ResizeOption.FixedWidthHeight).ToString();
            ThumbnailResize.Order = 4;
            _baseSettings.Add("ThumbnailResize", ThumbnailResize);

            // Thumbnail Width Setting
            SettingItem ThumbnailWidth = new SettingItem(new IntegerDataType());
            ThumbnailWidth.Required = true;
            ThumbnailWidth.Value = "100";
            ThumbnailWidth.Order = 5;
            ThumbnailWidth.MinValue = 2;
            ThumbnailWidth.MaxValue = 9999;
            _baseSettings.Add("ThumbnailWidth", ThumbnailWidth);

            // Thumbnail Height Setting
            SettingItem ThumbnailHeight = new SettingItem(new IntegerDataType());
            ThumbnailHeight.Required = true;
            ThumbnailHeight.Value = "75";
            ThumbnailHeight.Order = 6;
            ThumbnailHeight.MinValue = 2;
            ThumbnailHeight.MaxValue = 9999;
            _baseSettings.Add("ThumbnailHeight", ThumbnailHeight);

            // Original Resize Options
            ArrayList originalResizeOptions = new ArrayList();
            originalResizeOptions.Add(
                new Option((int) ResizeOption.NoResize, General.GetString("PICTURES_DONT_RESIZE", "Don't Resize", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.FixedWidthHeight,
                           General.GetString("PICTURES_FIXED_WIDTH_AND_HEIGHT", "Fixed width and height", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectWidth,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_WIDTH", "Maintain aspect fixed width", this)));
            originalResizeOptions.Add(
                new Option((int) ResizeOption.MaintainAspectHeight,
                           General.GetString("PICTURES_MAINTAIN_ASPECT_FIXED_HEIGHT", "Maintain aspect fixed height",
                                             this)));

            // Original Resize Settings
            SettingItem OriginalResize = new SettingItem(new CustomListDataType(originalResizeOptions, "Name", "Val"));
            OriginalResize.Required = true;
            OriginalResize.Value = ((int) ResizeOption.MaintainAspectWidth).ToString();
            OriginalResize.Order = 7;
            _baseSettings.Add("OriginalResize", OriginalResize);

            // Original Width Settings
            SettingItem OriginalWidth = new SettingItem(new IntegerDataType());
            OriginalWidth.Required = true;
            OriginalWidth.Value = "800";
            OriginalWidth.Order = 8;
            OriginalWidth.MinValue = 2;
            OriginalWidth.MaxValue = 9999;
            _baseSettings.Add("OriginalWidth", OriginalWidth);

            // Original Width Settings
            SettingItem OriginalHeight = new SettingItem(new IntegerDataType());
            OriginalHeight.Required = true;
            OriginalHeight.Value = "600";
            OriginalHeight.Order = 9;
            OriginalHeight.MinValue = 2;
            OriginalHeight.MaxValue = 9999;
            _baseSettings.Add("OriginalHeight", OriginalHeight);

            // Repeat Direction Options
            ArrayList repeatDirectionOptions = new ArrayList();
            repeatDirectionOptions.Add(
                new Option((int) RepeatDirection.Horizontal,
                           General.GetString("PICTURES_HORIZONTAL", "Horizontal", this)));
            repeatDirectionOptions.Add(
                new Option((int) RepeatDirection.Vertical, General.GetString("PICTURES_VERTICAL", "Vertical", this)));

            // Repeat Direction Setting
            SettingItem RepeatDirectionSetting =
                new SettingItem(new CustomListDataType(repeatDirectionOptions, "Name", "Val"));
            RepeatDirectionSetting.Required = true;
            RepeatDirectionSetting.Value = ((int) RepeatDirection.Horizontal).ToString();
            RepeatDirectionSetting.Order = 10;
            _baseSettings.Add("RepeatDirection", RepeatDirectionSetting);

            // Repeat Columns Setting
            SettingItem RepeatColumns = new SettingItem(new IntegerDataType());
            RepeatColumns.Required = true;
            RepeatColumns.Value = "6";
            RepeatColumns.Order = 11;
            RepeatColumns.MinValue = 1;
            RepeatColumns.MaxValue = 200;
            _baseSettings.Add("RepeatColumns", RepeatColumns);

            // Layouts
            Hashtable layouts = new Hashtable();
            foreach (
                string layoutControl in
                    Directory.GetFiles(
                        HttpContext.Current.Server.MapPath(Path.ApplicationRoot + "/Design/PictureLayouts"), "*.ascx"))
            {
                string layoutControlDisplayName =
                    layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1,
                                            layoutControl.LastIndexOf(".") - layoutControl.LastIndexOf("\\") - 1);
                string layoutControlName = layoutControl.Substring(layoutControl.LastIndexOf("\\") + 1);
                layouts.Add(layoutControlDisplayName, layoutControlName);
            }

            // Thumbnail Layout Setting
            SettingItem ThumbnailLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
            ThumbnailLayoutSetting.Required = true;
            ThumbnailLayoutSetting.Value = "DefaultThumbnailView.ascx";
            ThumbnailLayoutSetting.Order = 12;
            _baseSettings.Add("ThumbnailLayout", ThumbnailLayoutSetting);

            // Thumbnail Layout Setting
            SettingItem ImageLayoutSetting = new SettingItem(new CustomListDataType(layouts, "Key", "Value"));
            ImageLayoutSetting.Required = true;
            ImageLayoutSetting.Value = "DefaultImageView.ascx";
            ImageLayoutSetting.Order = 13;
            _baseSettings.Add("ImageLayout", ImageLayoutSetting);

            // PicturesPerPage
            SettingItem PicturesPerPage = new SettingItem(new IntegerDataType());
            PicturesPerPage.Required = true;
            PicturesPerPage.Value = "9999";
            PicturesPerPage.Order = 14;
            PicturesPerPage.MinValue = 1;
            PicturesPerPage.MaxValue = 9999;
            _baseSettings.Add("PicturesPerPage", PicturesPerPage);

            //If false the input box for bulk loads will be hidden
            SettingItem AllowBulkLoad = new SettingItem(new BooleanDataType());
            AllowBulkLoad.Value = "false";
            AllowBulkLoad.Order = 15;
            _baseSettings.Add("AllowBulkLoad", AllowBulkLoad);
        }

        #region Global Implementation

        /// <summary>
        /// GuidID
        /// </summary>
        public override Guid GuidID
        {
            get { return new Guid("{B29CB86B-AEA1-4E94-8B77-B4E4239258B0}"); }
        }

        #region Search Implementation

        /// <summary>
        /// Searchable module
        /// </summary>
        public override bool Searchable
        {
            get { return true; }
        }

        /// <summary>
        /// Searchable module implementation
        /// </summary>
        /// <param name="portalID">The portal ID</param>
        /// <param name="userID">ID of the user is searching</param>
        /// <param name="searchString">The text to search</param>
        /// <param name="searchField">The fields where perfoming the search</param>
        /// <returns>The SELECT sql to perform a search on the current module</returns>
        public override string SearchSqlSelect(int portalID, int userID, string searchString, string searchField)
        {
            // Parameters:
            // Table Name: the table that holds the data
            // Title field: the field that contains the title for result, must be a field in the table
            // Abstract field: the field that contains the text for result, must be a field in the table
            // Search field: pass the searchField parameter you recieve.

            SearchDefinition s =
                new SearchDefinition("rb_Pictures", "ShortDescription", "Keywords", "CreatedByUser", "CreatedDate",
                                     "Keywords");

            //Add here extra search fields, this way
            s.ArrSearchFields.Add("itm.ShortDescription");

            // Builds and returns the SELECT query
            return s.SearchSqlSelect(portalID, userID, searchString);
        }

        #endregion

        #region Install / Uninstall Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = System.IO.Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");
            ArrayList errors = DBHelper.ExecuteScript(currentScriptName, true);
            if (errors.Count > 0)
            {
                // Call rollback
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        #endregion

        #endregion

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            this.dlPictures.EnableViewState = false;
            pgPictures.OnMove += new EventHandler(Page_Changed);
            this.AddText = "ADD"; //"Add New Picture"
            this.AddUrl = "~/DesktopModules/CommunityModules/Pictures/PicturesEdit.aspx";
            base.OnInit(e);
        }

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion

        /// <summary>
        /// Structure used for list settings
        /// </summary>
        public struct Option
        {
            private int val;
            private string name;

            /// <summary>
            /// 
            /// </summary>
            public int Val
            {
                get { return this.val; }
                set { this.val = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="aVal"></param>
            /// <param name="aName"></param>
            public Option(int aVal, string aName)
            {
                val = aVal;
                name = aName;
            }
        }
    }
}