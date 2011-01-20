<%@ Control Language="c#" %>
<%@ Register Assembly="Appleseed.Framework.Core" Namespace="Appleseed.Framework.Web.UI.WebControls" TagPrefix="rbfwebui" %>
<%@ Register Assembly="Appleseed.Framework.Web.UI.WebControls" Namespace="Appleseed.Framework.Web.UI.WebControls" TagPrefix="rbfwebui" %>
<script runat="server">
    private void Page_Load( object sender, System.EventArgs e ) {
        PortalHeaderMenu.DataBind();
		PortalTitle.DataBind();
        PortalImage.DataBind();
        var signInControl = (Appleseed.Content.Web.Modules.Signin)LoadControl("~/DesktopModules/CoreModules/SignIn/SignIn.ascx");
        loguinPlaceHolder.Controls.Add(signInControl);
    }
</script>

<div id="loguin_dialog" style="display:none">
    <asp:PlaceHolder ID="loguinPlaceHolder" runat="server"></asp:PlaceHolder>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        var $dialog = $('#loguin_dialog')
		.dialog({
		    autoOpen: false,
		    modal: true,
		    width: 350, 
		    open: function (type, data) { $(this).parent().appendTo("form"); }
		});

        $('#logon_link').click(function () {
            $dialog.dialog('open');
            // prevent the default action, e.g., following a link
            return false;
        });

        if ($("#loguin_dialog span.Error").html()) {
            $dialog.dialog('open');
        }
    });
</script>

<div class="header">

	<div class="logo_div">
		<!-- Portal Logo Image Uploaded-->
			<rbfwebui:headerimage id="PortalImage" runat="server" enableviewstate="false"/>
		<!-- End Portal Logo-->
						<!-- Portal Title -->
			<rbfwebui:headertitle id="PortalTitle" runat="server" cssclass="SiteTitle" enableviewstate="false"></rbfwebui:headertitle>
		<!-- End Portal Title -->
	</div>

	<div class="userMenu">
		<!-- begin User Menu at the Top of the Page -->
			<rbfwebui:HeaderMenu 	ID="PortalHeaderMenu" runat="server" 
									CssClass="SiteLink" RepeatDirection="Horizontal" cellspacing="0"
									CellPadding="0" ShowHelp="False" ShowHome="False" 
									ShowLogon="true" ShowRegister="true" ShowDragNDrop="True">
				<ItemStyle Wrap="False"></ItemStyle>
				<ItemTemplate>
					<!-- used to stylize the left border ex: border with images-->
						<div class="SiteLink_Border_Left"></div>
					<!-- End left border -->
							<div class="SiteLink_bg">
								<span class="SiteLink">
									<%# Container.DataItem %>
								</span>
							</div>
					<!-- used to stylize the right border-->
						<div class="SiteLink_Border_Right"></div>
					<!-- End right border -->
				</ItemTemplate>
				
					<SeparatorTemplate>
						<span class="SiteLink">&nbsp;&nbsp;|&nbsp;&nbsp;</span>
					</SeparatorTemplate>
				
			</rbfwebui:HeaderMenu>
		<!-- End User Menu -->
	</div>

	<div class="contenedor_menu">
		<!-- Begin Portal Manu -->
			<div class="menu_border_left"></div>
			<asp:Menu 	ID="biMenu"	runat="server" 
						DataSourceID="biSMDS" 
						Orientation="Horizontal"
						CssClass="menu" 
						DynamicEnableDefaultPopOutImage="False" 
						StaticEnableDefaultPopOutImage="False">                                
			</asp:Menu>
			<div class="menu_border_right"></div>
		<!-- End Portla Menu -->
	</div>
</div>


<asp:SiteMapDataSource ID="biSMDS" ShowStartingNode="false" runat="server" />
