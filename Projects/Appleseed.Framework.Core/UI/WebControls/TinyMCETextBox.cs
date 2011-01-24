using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Framework.UI.WebControls.TinyMCE
{
    public class TinyMCETextBox: TextBox, IHtmlEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TinyMCETextBox"/> class.
        /// </summary>
        public TinyMCETextBox()
        {
            //in order to render the textbox as a textarea
            TextMode = TextBoxMode.MultiLine;
        }

        private string _imageFolder = string.Empty;

        /// <summary>
        /// Control Image Folder
        /// </summary>
        /// <value></value>
        public string ImageFolder
        {
            get
            {
                if (_imageFolder == string.Empty)
                {
                    var pS = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
                    if (pS.CustomSettings != null)
                    {
                        if (pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"] != null)
                        {
                            _imageFolder = pS.CustomSettings["SITESETTINGS_DEFAULT_IMAGE_FOLDER"].ToString();
                        }
                    }
                }
                return "/images/" + _imageFolder;
            }
            set { _imageFolder = value; }
        }


        /// <summary>
        /// Raised when the pages is loading. Here the TinyMCETextBox will register its scripts references.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(this.GetType(), "TinyMCE"))
            {
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "TinyMCE", HttpUrlBuilder.BuildUrl("~/aspnet_client/tiny_mce/tiny_mce.js"));
            }
            base.OnLoad(e);
        }



        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            var specificEditorClass =  string.Concat(this.ClientID,"_tinymce");
            this.Attributes.Add("class", specificEditorClass);
            writer.Write("<script type=\"text/javascript\">");

            var width = (this.Width.IsEmpty) ? string.Empty : string.Concat("width : \"", this.Width.Value + 3, "\",");
            var height = (this.Height.IsEmpty) ? string.Empty : string.Concat("height : \"", this.Height.Value, "\",");

            writer.Write(string.Concat(@"
                tinyMCE.init({
		            // General options
		            mode : ""specific_textareas"",
		            editor_selector : """, specificEditorClass, @""",
                    theme : ""advanced"",
		            plugins : ""pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount,advlist,autosave"",

		            // Theme options
		            theme_advanced_buttons1 : ""newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,styleselect,formatselect,fontselect,fontsizeselect,|,help"",
		            theme_advanced_buttons2 : ""cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,code,|,insertdate,inserttime,preview,|,forecolor,backcolor"",
		            theme_advanced_buttons3 : ""tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen"",
		            theme_advanced_buttons4 : ""insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak,restoredraft"",
		            theme_advanced_toolbar_location : ""top"",
		            theme_advanced_toolbar_align : ""left"",
		            theme_advanced_statusbar_location : ""bottom"",
		            theme_advanced_resizing : false,
                    ", width , @"
                    ", height, @"
		            // Example content CSS (should be your site CSS)
		            content_css : ""css/content.css"",

		            // Drop lists for link/image/media/template dialogs
		            template_external_list_url : ""lists/template_list.js"",
		            external_link_list_url : ""lists/link_list.js"",
		            external_image_list_url : ""lists/image_list.js"",
		            media_external_list_url : ""lists/media_list.js"",

		            // Style formats
		            style_formats : [
			            {title : 'Normal', inline : 'span', classes : 'Normal'},
			            {title : 'Red text', inline : 'span', styles : {color : '#ff0000'}}
		            ]
                });
                "));

            writer.Write("</script>");
            base.Render(writer);
        }

    }
}
