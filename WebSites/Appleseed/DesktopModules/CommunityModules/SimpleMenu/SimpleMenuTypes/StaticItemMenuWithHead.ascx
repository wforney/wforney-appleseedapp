<%@ Control AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.SimpleMenuType"
    Language="c#" %>
<!--Simple Item Menu-->
<rbfwebui:DesktopNavigation ID="NavigationMenu" runat="server" AutoBind="false" Bind="<%#MenuBindOption%>"
    CssClass="sm_SimpleMenu" EnableViewState="false" ParentPageID="<%#ParentPageID%>"
    RepeatDirection="<%#MenuRepeatDirection%>" ShowFooter="false" ShowHeader="true"
    Visible="true">
    <SelectedItemStyle CssClass="sm_SelectedTab" />
    <SelectedItemTemplate>
        <!-- 
            &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , this.GlobalPortalSettings.ActivePage.PageID,"ItemId="  + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
                <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
                &#160;
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </SelectedItemTemplate>
    <AlternatingItemStyle CssClass="sm_OtherSubTabsAlt" />
    <AlternatingItemTemplate>
        <!--  
            &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , this.GlobalPortalSettings.ActivePage.PageID,"ItemId=" + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
                <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>&#160; 
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </AlternatingItemTemplate>
    <ItemStyle CssClass="sm_OtherSubTabs" />
    <ItemTemplate>
        <!-- 
            &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" ,this.GlobalPortalSettings.ActivePage.PageID, "ItemId=" + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
                <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
            </a>&#160;
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx", ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </ItemTemplate>
    <FooterStyle CssClass="sm_Footer" />
    <FooterTemplate>
    </FooterTemplate>
    <HeaderStyle CssClass="sm_Header" />
    <HeaderTemplate>
        &#160;<a href='<%=Appleseed.Framework.HttpUrlBuilder.BuildUrl(this.GlobalPortalSettings.ActivePage.PageID)%>'><%=this.GlobalPortalSettings.ActivePage.PageName%></a>&#160;</HeaderTemplate>
</rbfwebui:DesktopNavigation>
<!--END:Simple Item Menu-->
