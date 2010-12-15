<%@ Control Language="c#" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        PortalHeaderMenu.DataBind();
        PortalTitle.DataBind();
        PortalImage.DataBind();
        PortalTabs.DataBind();
    }
</script>
<!-- Portal BANNER -->
<table id="TablePortalBanner" border="0" cellpadding="0" cellspacing="0" class="HeadBg"
    width="100%">
    <tbody>
        <tr>
            <td class="DefaultBanner1" colspan="2" height="1">
                <asp:Image ID="IMAGE1" runat="server" ImageUrl="~/design/Themes/default/img/shim.gif"
                    alt="" />
            </td>
        </tr>
        <tr>
            <td align="left" rowspan="2" valign="middle">
                <table cellpadding="0" cellspacing="0">
                    <tbody>
                        <tr>
                            <td>
                                <rbfwebui:HeaderImage ID="PortalImage" runat="server" EnableViewState="false" Style="margin-top: 0px;
                                    margin-right: 0px" />
                            </td>
                            <!--headerimage=images/logo-default.gif-->
                            <td height="50">
                                <rbfwebui:HeaderTitle ID="PortalTitle" runat="server" CssClass="SiteTitle" EnableViewState="false"></rbfwebui:HeaderTitle>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
            <td align="right" valign="top">
                <table cellpadding="0" cellspacing="0">
                    <tbody>
                        <tr>
                            <td valign="top">
                            </td>
                            <td valign="top">
                                <rbfwebui:HeaderMenu ID="PortalHeaderMenu" runat="server" CellPadding="4" CssClass="SiteLink"
                                    RepeatDirection="Horizontal" ShowHelp="True" ShowHome="False">
                                    <SeparatorStyle />
                                    <ItemTemplate>
                                        <span class="SiteLink">
                                            <%# Container.DataItem %>
                                        </span>
                                    </ItemTemplate>
                                    <SeparatorTemplate>
                                        |
                                    </SeparatorTemplate>
                                </rbfwebui:HeaderMenu>
                            </td>
                            <rbfwebui:LinkButton ID="saveConfig" runat="server" Visible="False"></rbfwebui:LinkButton>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr valign="top">
            <td align="right">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="DefaultBanner1" colspan="2" height="1">
                <asp:Image ID="IMAGE2" runat="server" ImageUrl="~/design/Themes/default/img/shim.gif"
                    alt="" />
            </td>
        </tr>
        <tr>
            <td class="DefaultBanner2" colspan="2" height="1">
                <asp:Image ID="IMAGE3" runat="server" ImageUrl="~/design/Themes/default/img/shim.gif"
                    alt="" />
            </td>
        </tr>
        <tr>
            <td class="DefaultTD" colspan="2">
                <rbfwebui:DesktopNavigation ID="PortalTabs" runat="server" EnableViewState="false"
                    HorizontalAlign="left" RepeatDirection="horizontal" UseTabNameInUrl="true">
                    <ItemStyle CssClass="Tabs" />
                    <ItemTemplate>
                        <a href="<%#PortalTabs.giveMeUrl(((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName, ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>">
                            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
                        </a>
                    </ItemTemplate>
                    <SelectedItemStyle CssClass="SelectedTabs" />
                    <SelectedItemTemplate>
                        <a href="<%#PortalTabs.giveMeUrl(((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName, ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>">
                            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
                        </a>
                    </SelectedItemTemplate>
                </rbfwebui:DesktopNavigation>
            </td>
        </tr>
        <tr>
            <td class="DefaultBanner2" colspan="2" height="1">
                <asp:Image ID="IMAGE4" runat="server" ImageUrl="~/design/Themes/default/img/shim.gif"
                    alt="" />
            </td>
        </tr>
    </tbody>
</table>
<!-- END Portal BANNER -->
