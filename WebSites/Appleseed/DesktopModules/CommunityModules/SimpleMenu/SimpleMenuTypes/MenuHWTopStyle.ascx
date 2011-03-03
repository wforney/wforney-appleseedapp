<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SimpleMenuType"
    language="c#" %>
<!-- START DHTML Menu -->
<rbfwebui:menunavigation id="NavigationMenu" runat="server" autobind="False" bind="<%#MenuBindOption%>"
    borderwidth="0" horizontal="<%#(MenuRepeatDirection==0)%>" parenttabid="<%#ParentPageID%>"
    visible="True" width="190px">
    <controlitemstyle cssclass="HWMenuItem" />
    <controlsubstyle cssclass="HWMenuSubItem" />
    <controlhistyle cssclass="HWMenuHiItem" />
    <controlhisubstyle cssclass="HWMenuHiSubItem" />
    <arrowimage height="10px" imageurl="tri.gif" width="5px" />
    <arrowimageleft height="10px" imageurl="trileft.gif" width="5px" />
    <arrowimagedown height="5px" imageurl="tridown.gif" width="10px" />
</rbfwebui:menunavigation>
<!-- END DHTML Menu -->
