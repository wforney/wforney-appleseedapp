<%@ Control Language="C#" AutoEventWireup="true" Inherits="DesktopModules_CoreModules_MVC_MVCModule" Codebehind="MVCModule.ascx.cs" %>
<span runat="server">
    <!--<link rel="stylesheet" type="text/css" href="/Content/Themes/<%=this.PortalSettings.PortalAlias %>/jquery-ui-1.7.1.custom.css" />
    <link rel="stylesheet" type="text/css" href="/Content/Themes/<%=this.PortalSettings.PortalAlias %>/jquery-ui-1.7.2.custom.css" />-->
	<link rel="stylesheet" type="text/css" href="/Content/Themes/<%=this.PortalSettings.PortalAlias %>/jquery-ui-1.8.1.custom.css" />
    <script type="text/javascript">



        $(document).ready(function () {
            /*$('#switcher').themeswitcher();*/
            $(document).bind('click', function () {
                $("[adv='adv']").hide('slow');
                $(document.body).unbind('click', '');
            });



            $('.desktopmodules_coremodules_mvc_mvcmodule_ascx ._timeEntry').timeEntry({ spinnerImage: '', show24Hours: true, defaultTime: new Date(0, 0, 0, 0, 0) });
            initTasks();
            $('.desktopmodules_coremodules_mvc_mvcmodule_ascx #loading').loading({ onAjax: true, img: '' });
            $("ul.ui-tabs-nav li").css('list-style-type', 'none')
            $(document).ajaxComplete(ajaxCompleteCallback);



        });
    </script> 
		
		  <% 
        try
        {
            System.Web.Routing.RouteValueDictionary dict = new System.Web.Routing.RouteValueDictionary();

            foreach (DictionaryEntry de in this.Settings)
            {

                dict.Add(de.Key.ToString(), de.Value);
            }
            dict.Add("area", this.AreaName.ToString());
            dict.Add("moduleId", this.ModuleID);
            Html.RenderAction(this.ActionName, this.ControllerName, dict);
        }
        catch (Exception exc)
        {
            ErrorHandler.Publish(LogLevel.Error, exc);
    %>
    Error
    <%} %>
</span>