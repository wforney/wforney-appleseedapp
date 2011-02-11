<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SimpleMenuType"
    language="c#" %>
<!--simple static menu-->
<rbfwebui:desktopnavigation id="NavigationMenu" runat="server" autobind="False" bind="<%#MenuBindOption%>"
    cssclass="sm_SimpleMenu" enableviewstate="False" parentpageid="<%#ParentPageID%>"
    repeatdirection="<%#MenuRepeatDirection%>" showfooter="False" showheader="False"
    visible="True">
    <selecteditemstyle cssclass="sm_SelectedTab" />
    <selecteditemtemplate>
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl(((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'></a>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        &#160;
    </selecteditemtemplate>
    <alternatingitemstyle cssclass="sm_OtherSubTabsAlt" />
    <alternatingitemtemplate>
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl(((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'></a>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        &#160;
    </alternatingitemtemplate>
    <itemstyle cssclass="sm_OtherSubTabs" />
    <itemtemplate>
        &#160;<a href='<%#Appleseed.Framework.HttpUrlBuilder.BuildUrl(((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageID)%>'>
            <%# ((Appleseed.Framework.Site.Configuration.PageStripDetails) Container.DataItem).PageName %>
        </a>&#160;
    </itemtemplate>
    <footerstyle cssclass="sm_Footer" />
    <footertemplate>
    </footertemplate>
    <headerstyle cssclass="sm_Header" />
    <headertemplate>
    </headertemplate>
</rbfwebui:desktopnavigation>
<!--END: simple static menu-->
