using System;
using System.Collections;
using System.Web;
using Appleseed.Framework;using Appleseed.Framework.Site.Data;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Security;
using Appleseed.Framework.Web.UI;

namespace Appleseed.Content.Web.Modules.FCK.filemanager.upload.aspx
{
	/// <summary>
	/// upload files to server.
	/// </summary>
	public partial class upload : EditItemPage 
	{

		/// <summary>
		/// Load settings
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
			if (Request.Files.Count > 0)
			{
				HttpPostedFile oFile = Request.Files.Get("FCKeditor_File") ;
	
				string fileName = oFile.FileName.Substring(oFile.FileName.LastIndexOf("\\") + 1);
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
				string sFileURL  = portalSettings.PortalFullPath + "/images/" + DefaultImageFolder + "/" + fileName;
				string sFilePath = Server.MapPath(sFileURL) ;
			
				oFile.SaveAs(sFilePath) ;
			
				Response.Write("<SCRIPT language=javascript>window.opener.setImage('" + sFileURL + "') ; window.close();</" + "SCRIPT>") ;
			}
		}

		#region Código generado por el Diseñador de Web Forms
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
