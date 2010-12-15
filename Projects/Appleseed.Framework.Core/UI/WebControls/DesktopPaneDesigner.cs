using System;
using System.Web.UI.Design;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using System.ComponentModel;


namespace Appleseed.Framework.Web.UI.WebControls
{
    /// <summary>
    /// DesktopPanes design support class for Visual Studio.
    /// </summary>
	public class DesktopPanesDesigner : TemplatedControlDesigner
	{
		private DesktopPanes desktopPanes;

		private TemplateEditingVerb[] templateVerbs;
		private bool templateVerbsDirty;
		
		internal static TraceSwitch DesktopPanesDesignerSwitch;

		private static string[] PaneTemplateNames;
		private static string[] SeparatorTemplateNames;

		private const int PaneTemplates = 0;
		private const int SeparatorTemplates = 1;

		private const int IDX_LEFT_PANE_TEMPLATE = 0;
		private const int IDX_CONTENT_PANE_TEMPLATE = 1;
		private const int IDX_RIGHT_PANE_TEMPLATE = 2;

		private const int IDX_HORIZONTAL_SEPARATOR_TEMPLATE = 0;
		private const int IDX_VERTICAL_SEPARATOR_TEMPLATE = 1;

        /// <summary>
        /// Initialize components
        /// </summary>
        /// <param name="component"></param>
		public override void Initialize(IComponent component) 
		{
			desktopPanes = (DesktopPanes) component;

			base.Initialize(component);
		}

		static DesktopPanesDesigner() 
		{
			string[] _TemplateNames;

			DesktopPanesDesigner.DesktopPanesDesignerSwitch = new TraceSwitch("DESKTOPPANEDESIGNER", "Enable DesktopPanes designer general purpose traces.");

			_TemplateNames = new String[3];
			_TemplateNames[IDX_LEFT_PANE_TEMPLATE] = "Left pane template";
			_TemplateNames[IDX_CONTENT_PANE_TEMPLATE] = "Content pane template";
			_TemplateNames[IDX_RIGHT_PANE_TEMPLATE] = "Right pane template";
			DesktopPanesDesigner.PaneTemplateNames = _TemplateNames;

			_TemplateNames = new String[2];
			_TemplateNames[IDX_HORIZONTAL_SEPARATOR_TEMPLATE] = "Horizontal Separator";
			_TemplateNames[IDX_VERTICAL_SEPARATOR_TEMPLATE] = "Vertical Separator";
			DesktopPanesDesigner.SeparatorTemplateNames = _TemplateNames;
		}

        /// <summary>
        /// Default constructor
        /// </summary>
		public DesktopPanesDesigner() 
		{
			templateVerbsDirty = true;
		}

		/// <summary>
		/// Override the AllowResize property inherited
		/// from ControlDesigner to enable the control 
		/// to be resized. 
		/// It is recommended that controls allow resizing 
		/// when in template mode even if they normally 
		/// do not allow resizing. 
		/// </summary>
		public override bool AllowResize 
		{
			get 
			{
				// When templates are not defined, render a read-only fixed-size block. 
				// Once templates are defined or are being edited, the control should allow resizing.
				if (!(desktopPanes.ContentPaneTemplate == null))
					return this.InTemplateMode;
				else
					return true;
			}
		}

		/// <summary>
		/// Override the GetCachedTemplateEditingVerbs method. 
		/// A control overrides this method to return the list 
		/// of template editing verbs applicable to the control. 
		/// </summary>
		/// <returns></returns>
		protected override TemplateEditingVerb[] GetCachedTemplateEditingVerbs() 
		{
			if (templateVerbsDirty)
			{
				DisposeTemplateVerbs();

				templateVerbs = new TemplateEditingVerb[2];
				templateVerbs[PaneTemplates] = new TemplateEditingVerb("PaneTemplates", PaneTemplates, this);
				templateVerbs[SeparatorTemplates] = new TemplateEditingVerb("SeparatorTemplates", SeparatorTemplates, this);
				templateVerbsDirty = false;
			}
			return templateVerbs;
		}

		/// <summary>
		/// Override the CreateTemplateEditingFrame method. 
		/// This method takes a TemplateEditingVerb instance as an argument. 
		/// TemplateEditingVerb is a designer verb (a class that derives from DesignerVerb) 
		/// that enables the template editor to add a command to the control at design time. 
		/// </summary>
		/// <param name="verb"></param>
		/// <returns></returns>
		protected override ITemplateEditingFrame CreateTemplateEditingFrame(TemplateEditingVerb verb) 
		{
			ITemplateEditingService teService = (ITemplateEditingService) GetService(typeof(ITemplateEditingService));
			
			Trace.Assert(teService != null, "How did we get this far without an ITemplateEditingService?");
			Trace.Assert(verb.Index == 0 || verb.Index == SeparatorTemplates);

			string[] templateNames = null;
			System.Web.UI.WebControls.Style[] templateStyles = null;
			ITemplateEditingFrame editingFrame;
			System.Web.UI.WebControls.Style[] outputTemplateStyles;

			switch (verb.Index) 
			{
				case PaneTemplates:
					templateNames = DesktopPanesDesigner.PaneTemplateNames;
					outputTemplateStyles = new Style[3];
					outputTemplateStyles[IDX_LEFT_PANE_TEMPLATE] = desktopPanes.LeftPaneStyle;
					outputTemplateStyles[IDX_CONTENT_PANE_TEMPLATE] = desktopPanes.ControlStyle;
					outputTemplateStyles[IDX_RIGHT_PANE_TEMPLATE] = desktopPanes.RightPaneStyle;
					templateStyles = outputTemplateStyles;
					break;
				case SeparatorTemplates:
					templateNames = DesktopPanesDesigner.SeparatorTemplateNames;
					outputTemplateStyles = new Style[2];
					outputTemplateStyles[IDX_HORIZONTAL_SEPARATOR_TEMPLATE] = desktopPanes.HorizontalSeparatorStyle;
					outputTemplateStyles[IDX_VERTICAL_SEPARATOR_TEMPLATE] = desktopPanes.VerticalSeparatorStyle;
					templateStyles = outputTemplateStyles;
					break;
			}
			editingFrame = teService.CreateFrame(this, verb.Text, templateNames, desktopPanes.ControlStyle, templateStyles);
			//editingFrame = teService.CreateFrame(this, verb.Text, templateNames);
			return editingFrame;
		}

		/// <summary>
		/// A type's Dispose method should release all the resources 
		/// that it owns. It should also release all resources owned 
		/// by its base types by calling its parent type's Dispose 
		/// method. 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) 
		{
			if (disposing) 
			{
				DisposeTemplateVerbs();
				desktopPanes = null;
			}
			this.Dispose(disposing);
		}

		private void DisposeTemplateVerbs() 
		{
			if (templateVerbs != null) 
			{
				for(int i = 0; i < templateVerbs.Length; i++) 
					templateVerbs[i].Dispose();

				templateVerbs = null;
				templateVerbsDirty = true;
			}
		}

		/// <summary>
		/// As with any other designer, override the GetDesignTimeHtml.
		/// Gets the HTML that is used to represent the control at design time.
		/// </summary>
		public override string GetDesignTimeHtml() 
		{
			StringBuilder designTimeHTML = new StringBuilder();

		    designTimeHTML.Append("<TABLE");
            if(!(desktopPanes.Width.IsEmpty))
            {
		        designTimeHTML.Append(" width='");
		        designTimeHTML.Append(desktopPanes.Width.Value);
		        designTimeHTML.Append("'");
            }
            if(!(desktopPanes.Height.IsEmpty))
            {
                designTimeHTML.Append(" height='");
                designTimeHTML.Append(desktopPanes.Height.Value);
                designTimeHTML.Append("'");
            }
		    designTimeHTML.Append(" BORDER='1'>");
		    designTimeHTML.Append("<TR>");

		    designTimeHTML.Append("<TD>");
			if (desktopPanes.VerticalSeparatorTemplate != null)
			{
				designTimeHTML.Append(GetTextFromTemplate(desktopPanes.VerticalSeparatorTemplate));
			}
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
			if (desktopPanes.LeftPaneTemplate != null)
			{
				designTimeHTML.Append(GetTextFromTemplate(desktopPanes.LeftPaneTemplate));
			}
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
			if (desktopPanes.VerticalSeparatorTemplate != null)
			{
				designTimeHTML.Append(GetTextFromTemplate(desktopPanes.VerticalSeparatorTemplate));
			}
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
            if (desktopPanes.ContentPaneTemplate != null)
            {
                designTimeHTML.Append(GetTextFromTemplate(desktopPanes.ContentPaneTemplate));
            }
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
			if (desktopPanes.VerticalSeparatorTemplate != null)
			{
				designTimeHTML.Append(GetTextFromTemplate(desktopPanes.VerticalSeparatorTemplate));
			}
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
            if (desktopPanes.RightPaneTemplate != null)
            {
                designTimeHTML.Append(GetTextFromTemplate(desktopPanes.RightPaneTemplate));
            }
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("<TD>");
			if (desktopPanes.VerticalSeparatorTemplate != null)
			{
				designTimeHTML.Append(GetTextFromTemplate(desktopPanes.VerticalSeparatorTemplate));
			}
		    designTimeHTML.Append("</TD>");

		    designTimeHTML.Append("</TR>");
		    designTimeHTML.Append("</TABLE>");

			return designTimeHTML.ToString();
		}

		/// <summary>
		/// As with any other designer, 
		/// override the GetEmptyDesignTimeHtml.
		/// Gets the HTML used to represent 
		/// an empty template-based control at design time.
		/// </summary>
		protected override string GetEmptyDesignTimeHtml() 
		{
			string text;

			if (CanEnterTemplateMode) 
			{
				text = "Right click and choose a set of templates to edit their content.";
			}
			else 
			{
				text = "Switch to HTML view to edit the control's templates.";
			}
			return CreatePlaceHolderDesignTimeHtml(text);
		}

		/// <summary>
		/// Override the GetTemplateContent method that gets 
		/// the template content. 
		/// </summary>
		public override string GetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, out bool allowEditing) 
		{
			Trace.Assert(editingFrame.Verb.Index >= 0 && editingFrame.Verb.Index <= 2);

            allowEditing = true;
			ITemplate template = null;
			string templateContent = String.Empty;

			switch (editingFrame.Verb.Index) 
			{
				case PaneTemplates:
					if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_LEFT_PANE_TEMPLATE])
					{
						template = desktopPanes.LeftPaneTemplate;
						break;
					}
					if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_CONTENT_PANE_TEMPLATE])
					{
						template = desktopPanes.ContentPaneTemplate;
						break;
					}
					if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_RIGHT_PANE_TEMPLATE])
					{
						template = desktopPanes.RightPaneTemplate;
						break;
					}
					break;

				case SeparatorTemplates:
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_HORIZONTAL_SEPARATOR_TEMPLATE])
                    {
                        template = desktopPanes.HorizontalSeparatorTemplate;
                        break;
                    }
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_VERTICAL_SEPARATOR_TEMPLATE])
                    {
                        template = desktopPanes.VerticalSeparatorTemplate;
                        break;
                    }
                    break;
			}
			if (template != null)
				templateContent = GetTextFromTemplate(template);
			return templateContent;
		}

		/// <summary>
		/// Override the SetTemplateContent method that sets 
		/// the template content. 
		/// </summary>
		public override void SetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, string templateContent) 
		{
			Trace.Assert(editingFrame.Verb.Index >= 0 && editingFrame.Verb.Index <= 2);

			ITemplate template = null;

			if (templateContent != null && templateContent.Length != 0) 
			{
				template = GetTemplateFromText(templateContent);
			}

			switch (editingFrame.Verb.Index) 
			{
				case PaneTemplates:
					if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_LEFT_PANE_TEMPLATE]) 
					{
						desktopPanes.LeftPaneTemplate = template;
						return;
					}
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_CONTENT_PANE_TEMPLATE]) 
                    {
                        desktopPanes.ContentPaneTemplate = template;
                        return;
                    }
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_RIGHT_PANE_TEMPLATE]) 
                    {
                        desktopPanes.RightPaneTemplate = template;
                        return;
                    }
                    break;
				case SeparatorTemplates:
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_HORIZONTAL_SEPARATOR_TEMPLATE]) 
                    {
                        desktopPanes.HorizontalSeparatorTemplate = template;
                        return;
                    }
                    if (templateName == DesktopPanesDesigner.PaneTemplateNames[IDX_VERTICAL_SEPARATOR_TEMPLATE]) 
                    {
                        desktopPanes.VerticalSeparatorTemplate = template;
                        return;
                    }
					return;

				default:
					return;
			}
		}
	}
}