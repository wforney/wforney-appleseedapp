<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SimpleMenuType"
    language="c#" %>
<!--Simple Item Menu-->
<rbfwebui:desktopnavigation id="NavigationMenu" runat="server" autobind="false" bind="<%#MenuBindOption%>"
    cssclass="sm_SimpleMenu" enableviewstate="false" parentpageid="<%#ParentPageID%>"
    repeatdirection="<%#MenuRepeatDirection%>" showfooter="false" showheader="true"
    visible="true">
    <selecteditemstyle cssclass="sm_SelectedTab" />
    <selecteditemtemplate>
        <!-- 
			&#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , GlobalPortalSettings.ActivePage.PageID,"ItemId="  + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
				&#160;
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </selecteditemtemplate>
    <alternatingitemstyle cssclass="sm_OtherSubTabsAlt" />
    <alternatingitemtemplate>
        <!--  
			&#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , GlobalPortalSettings.ActivePage.PageID,"ItemId=" + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>&#160; 
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" , ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </alternatingitemtemplate>
    <itemstyle cssclass="sm_OtherSubTabs" />
    <itemtemplate>
        <!-- 
			&#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx" ,GlobalPortalSettings.ActivePage.PageID, "ItemId=" + ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
				<%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
			</a>&#160;
-->
        <!-- Modified by Hongwei Shen(hongwei.shen@gmail.com) in fixing the url not correct problem, 05/07/2005 -->
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/Default.aspx", ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID, "")%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
        <!-- end of modification -->
    </itemtemplate>
    <footerstyle cssclass="sm_Footer" />
    <footertemplate>
    </footertemplate>
    <headerstyle cssclass="sm_Header" />
    <headertemplate>
        &#160;<a href='<%=Appleseed.Framework.HttpUrlBuilder.BuildUrl(GlobalPortalSettings.ActivePage.PageID)%>'><%=GlobalPortalSettings.ActivePage.PageName%></a>&#160;</headertemplate>
</rbfwebui:desktopnavigation>
<!--END:Simple Item Menu-->
