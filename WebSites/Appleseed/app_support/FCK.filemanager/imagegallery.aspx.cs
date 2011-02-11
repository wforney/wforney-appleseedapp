using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Appleseed.Framework;using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;
using Image = System.Drawing.Image;
using Page = System.Web.UI.Page;
using Path = Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules.FCK.filemanager.browse
{
	/// <summary>
	/// Imagegallery.
	/// </summary>
	public partial class imagegallery : EditItemPage
	{
		#region Declerations
		#region Messages
		private string NoFileMessage = "No file selected";
		private string UploadSuccessMessage = "Uploaded Sucess";
		private string NoImagesMessage = "No Images";
		private string NoFolderSpecifiedMessage = "No folder";
		private string NoFileToDeleteMessage = "No file to delete";
		private string InvalidFileTypeMessage = "Invalid file type";
		#endregion
		private string[] AcceptedFileTypes = new string[] {"jpg","jpeg","jpe","gif","bmp","png"};

		// Configuration
		private bool UploadIsEnabled = true;
		private bool DeleteIsEnabled = true;
		#endregion

        /// <summary>
        /// LoadSettings
        /// Check if user has edit permissions
        /// </summary>
		protected override void LoadSettings()
		{
			if (PortalSecurity.HasEditPermissions(this.portalSettings.ActiveModule) == false)
				PortalSecurity.AccessDeniedEdit();
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, EventArgs e) 
		{
			string isframe = string.Empty + Request["frame"];
	
			if (isframe.Length != 0) 
			{
				MainPage.Visible = true;
				iframePanel.Visible = false;
	
				string rif = string.Empty + Request["rif"];
				string cif = string.Empty + Request["cif"];

				if (cif.Length != 0 && rif.Length != 0)
				{
					RootImagesFolder.Value = rif;
					CurrentImagesFolder.Value = cif;
				} 
				else
				{
					Hashtable ms = ModuleSettings.GetModuleSettings(portalSettings.ActiveModule);
					string DefaultImageFolder = "default";
					if (ms["MODULE_IMAGE_FOLDER"] != null) 
					{
						DefaultImageFolder = ms["MODULE_IMAGE_FOLDER"].ToString();
					}
					else if (portalSettings.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null) 
					{
						DefaultImageFolder = portalSettings.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
					}
					RootImagesFolder.Value = portalSettings.PortalPath + "/images/" + DefaultImageFolder;
					RootImagesFolder.Value = RootImagesFolder.Value.Replace("//", "/");
					CurrentImagesFolder.Value = RootImagesFolder.Value;	
				}

				UploadPanel.Visible = UploadIsEnabled;
				DeleteImage.Visible = DeleteIsEnabled;

				string FileErrorMessage = string.Empty;
				string ValidationString = ".*(";
				//[\.jpg]|[\.jpeg]|[\.jpe]|[\.gif]|[\.bmp]|[\.png])$"
				for (int i=0;i<AcceptedFileTypes.Length; i++) 
				{
					ValidationString += "[\\." + AcceptedFileTypes[i] + "]";
					if (i < (AcceptedFileTypes.Length-1)) ValidationString += "|";
					FileErrorMessage += AcceptedFileTypes[i];
					if (i < (AcceptedFileTypes.Length-1)) FileErrorMessage += ", ";
				}
				FileValidator.ValidationExpression = ValidationString+")$";
				FileValidator.ErrorMessage=FileErrorMessage;

				if (!IsPostBack) 
				{
					DisplayImages();
				}
			} 
			else 
			{
		
			}
		}

		/// <summary>
		/// Upload Image OnClick
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void UploadImage_OnClick(object sender, EventArgs e) 
		{	
			if (Page.IsValid) 
			{
				if (CurrentImagesFolder.Value.Length != 0) 
				{
					if (UploadFile.PostedFile.FileName.Trim().Length != 0) 
					{
						if (IsValidFileType(UploadFile.PostedFile.FileName)) 
						{
							try 
							{
								string UploadFileName = string.Empty;
								string UploadFileDestination = string.Empty;
								UploadFileName = UploadFile.PostedFile.FileName;
								UploadFileName = UploadFileName.Substring(UploadFileName.LastIndexOf("\\")+1);
								UploadFileDestination = HttpContext.Current.Request.PhysicalApplicationPath;
								UploadFileDestination += CurrentImagesFolder.Value;
								UploadFileDestination += "\\";
								UploadFile.PostedFile.SaveAs(UploadFileDestination + UploadFileName);
								ResultsMessage.Text = UploadSuccessMessage;
							} 
							catch
							{
								//ResultsMessage.Text = "Your file could not be uploaded: " + ex.Message;
								ResultsMessage.Text = "There was an error.";
							}
						} 
						else 
						{
							ResultsMessage.Text = InvalidFileTypeMessage;
						}
					} 
					else 
					{
						ResultsMessage.Text = NoFileMessage;
					}
				} 
				else 
				{
					ResultsMessage.Text = NoFolderSpecifiedMessage;
				}
			} 
			else 
			{
				ResultsMessage.Text = InvalidFileTypeMessage;
		
			}
			DisplayImages();
		}

		/// <summary>
		/// Handles the OnClick event of the DeleteImage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void DeleteImage_OnClick(object sender, EventArgs e) 
		{
			if (FileToDelete.Value.Length != 0 && FileToDelete.Value != "undefined") 
			{
				try 
				{
					string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
					File.Delete(AppPath  + CurrentImagesFolder.Value + "\\" + FileToDelete.Value);
					ResultsMessage.Text = "Deleted: " + FileToDelete.Value;
				} 
				catch 
				{			
					ResultsMessage.Text = "There was an error.";
				}
			} 
			else 
			{
				ResultsMessage.Text = NoFileToDeleteMessage;
			}
			DisplayImages();
		}

		/// <summary>
		/// Determines whether [is valid file type] [the specified file name].
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid file type] [the specified file name]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsValidFileType(string FileName) 
		{
			string ext = FileName.Substring(FileName.LastIndexOf(".")+1,FileName.Length-FileName.LastIndexOf(".")-1);
			ext = ext.ToLower();
			for (int i=0; i<AcceptedFileTypes.Length; i++) 
			{		
				if (ext == AcceptedFileTypes[i]) 
				{
					return true;
				}	
			}
			return false;
		}

		/// <summary>
		/// Returns the files array.
		/// </summary>
		/// <returns></returns>
		private string[] ReturnFilesArray() 
		{
			if (CurrentImagesFolder.Value.Length != 0) 
			{
				try 
				{
					string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
					string ImageFolderPath = AppPath + CurrentImagesFolder.Value;
					string[] FilesArray = Directory.GetFiles(ImageFolderPath,"*");
					return FilesArray;
			
			
				} 
				catch 
				{
		
					return null;
				}
			} 
			else 
			{
				return null;
			}

		}

		/// <summary>
		/// Returns the directories array.
		/// </summary>
		/// <returns></returns>
		private string[] ReturnDirectoriesArray() 
		{
			if (CurrentImagesFolder.Value.Length != 0) 
			{
				try 
				{
					string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
					string CurrentFolderPath = AppPath + CurrentImagesFolder.Value;
					string[] DirectoriesArray = Directory.GetDirectories(CurrentFolderPath,"*");
					return DirectoriesArray ;
				} 
				catch 
				{
					return null;
				}
			} 
			else 
			{
				return null;
			}
		}

		/// <summary>
		/// Displays the images.
		/// </summary>
		public void DisplayImages() 
		{
			string[] FilesArray = ReturnFilesArray();
			string[] DirectoriesArray = ReturnDirectoriesArray();
			string AppPath = HttpContext.Current.Request.PhysicalApplicationPath;
			string AppUrl;
	
			//Get the application's URL
			AppUrl = Request.ApplicationPath;
			if (!AppUrl.EndsWith("/")) AppUrl += "/";
			AppUrl = AppUrl.Replace("//", "/");
	
			GalleryPanel.Controls.Clear();
			if ( (FilesArray == null || FilesArray.Length == 0) && (DirectoriesArray == null || DirectoriesArray.Length == 0) ) 
			{
				gallerymessage.Text = NoImagesMessage + ": " + RootImagesFolder.Value;
			} 
			else 
			{
				string ImageFileName = string.Empty;
				string ImageFileLocation = string.Empty;

				int thumbWidth = 94;
				int thumbHeight = 94;
		
				if (CurrentImagesFolder.Value != RootImagesFolder.Value) 
				{
					HtmlImage myHtmlImage = new HtmlImage();
					myHtmlImage.Src = Path.ApplicationRoot + "/DesktopModules/FCK/filemanager/folder.gif";
					myHtmlImage.Attributes["unselectable"]="on"; 
					myHtmlImage.Attributes["align"]="absmiddle"; 
					myHtmlImage.Attributes["vspace"]="36"; 

					string ParentFolder = CurrentImagesFolder.Value.Substring(0,CurrentImagesFolder.Value.LastIndexOf("\\"));

					Panel myImageHolder = new Panel();					
					myImageHolder.CssClass = "imageholder";
					myImageHolder.Attributes["unselectable"]="on"; 
					myImageHolder.Attributes["onclick"]="divClick(this,'');";  
					myImageHolder.Attributes["ondblclick"]="gotoFolder('" + RootImagesFolder.Value + "','" + ParentFolder.Replace("\\","\\\\") + "');";  
					myImageHolder.Controls.Add(myHtmlImage);

					Panel myMainHolder = new Panel();
					myMainHolder.CssClass = "imagespacer";
					myMainHolder.Controls.Add(myImageHolder);

					Panel myTitleHolder = new Panel();
					myTitleHolder.CssClass = "titleHolder";
					myTitleHolder.Controls.Add(new LiteralControl("Up"));
					myMainHolder.Controls.Add(myTitleHolder);

					GalleryPanel.Controls.Add(myMainHolder);		
			
				}
		
				foreach (string _Directory in DirectoriesArray) 
				{
					try 
					{
						string DirectoryName = _Directory.ToString();

						HtmlImage myHtmlImage = new HtmlImage();
						myHtmlImage.Src = Path.ApplicationRoot + "/DesktopModules/FCK/filemanager/folder.gif";
						myHtmlImage.Attributes["unselectable"]="on"; 
						myHtmlImage.Attributes["align"]="absmiddle"; 
						myHtmlImage.Attributes["vspace"]="29"; 

						Panel myImageHolder = new Panel();					
						myImageHolder.CssClass = "imageholder";
						myImageHolder.Attributes["unselectable"]="on"; 
						myImageHolder.Attributes["onclick"]="divClick(this);";  
						myImageHolder.Attributes["ondblclick"]="gotoFolder('" + RootImagesFolder.Value + "','" + DirectoryName.Replace(AppPath,string.Empty).Replace("\\","\\\\") + "');";  
						myImageHolder.Controls.Add(myHtmlImage);

						Panel myMainHolder = new Panel();
						myMainHolder.CssClass = "imagespacer";
						myMainHolder.Controls.Add(myImageHolder);

						Panel myTitleHolder = new Panel();
						myTitleHolder.CssClass = "titleHolder";
						myTitleHolder.Controls.Add(new LiteralControl(DirectoryName.Replace(AppPath + CurrentImagesFolder.Value + "\\",string.Empty)));
						myMainHolder.Controls.Add(myTitleHolder);

						GalleryPanel.Controls.Add(myMainHolder);		
					} 
					catch 
					{
						// nothing for error
					}
				}
		
				foreach (string ImageFile in FilesArray) 
				{
					try 
					{
						ImageFileName = ImageFile.ToString();
						ImageFileName = ImageFileName.Substring(ImageFileName.LastIndexOf("\\")+1);
						ImageFileLocation = AppUrl;
//						ImageFileLocation = ImageFileLocation.Substring(ImageFileLocation.LastIndexOf("\\")+1);
						//galleryfilelocation += "/";
						ImageFileLocation += CurrentImagesFolder.Value;
						ImageFileLocation += "/";
						ImageFileLocation += ImageFileName;
						ImageFileLocation = ImageFileLocation.Replace("//", "/");
						HtmlImage myHtmlImage = new HtmlImage();
						myHtmlImage.Src = ImageFileLocation;
						System.Drawing.Image myImage = Image.FromFile(ImageFile.ToString());
						myHtmlImage.Attributes["unselectable"]="on";  
						//myHtmlImage.border=0;

						// landscape image
						if (myImage.Width > myImage.Height) 
						{
							if (myImage.Width > thumbWidth) 
							{
								myHtmlImage.Width = thumbWidth;
								myHtmlImage.Height = Convert.ToInt32(myImage.Height * thumbWidth/myImage.Width);						
							} 
							else 
							{
								myHtmlImage.Width = myImage.Width;
								myHtmlImage.Height = myImage.Height;
							}
							// portrait image
						} 
						else 
						{
							if (myImage.Height > thumbHeight) 
							{
								myHtmlImage.Height = thumbHeight;
								myHtmlImage.Width = Convert.ToInt32(myImage.Width * thumbHeight/myImage.Height);
							} 
							else 
							{
								myHtmlImage.Width = myImage.Width;
								myHtmlImage.Height = myImage.Height;
							}
						}
				
						if (myHtmlImage.Height < thumbHeight) 
						{
							myHtmlImage.Attributes["vspace"] = Convert.ToInt32((thumbHeight/2)-(myHtmlImage.Height/2)).ToString(); 
						}


						Panel myImageHolder = new Panel();					
						myImageHolder.CssClass = "imageholder";
						myImageHolder.Attributes["onclick"]="divClick(this,'" + ImageFileName + "');";  
						myImageHolder.Attributes["ondblclick"]="returnImage('" + ImageFileLocation.Replace("\\","/") + "','" + myImage.Width.ToString() + "','" + myImage.Height.ToString() + "');";  
						myImageHolder.Controls.Add(myHtmlImage);


						Panel myMainHolder = new Panel();
						myMainHolder.CssClass = "imagespacer";
						myMainHolder.Controls.Add(myImageHolder);

						Panel myTitleHolder = new Panel();
						myTitleHolder.CssClass = "titleHolder";
						myTitleHolder.Controls.Add(new LiteralControl(ImageFileName + "<BR>" + myImage.Width.ToString() + "x" + myImage.Height.ToString()));
						myMainHolder.Controls.Add(myTitleHolder);

						//GalleryPanel.Controls.Add(myImage);
						GalleryPanel.Controls.Add(myMainHolder);
				
						myImage.Dispose();
					} 
					catch 
					{

					}
				}
				gallerymessage.Text = string.Empty;
			}
		}

		#region Código generado por el Diseñador de Web Forms
		/// <summary>
		/// OnInit
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: llamada requerida por el Diseñador de Web Forms ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
