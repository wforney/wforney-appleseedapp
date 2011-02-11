<%@ register assembly="Appleseed.Framework" namespace="Appleseed.Framework.Web.UI.WebControls"
    tagprefix="rbfwebui" %>
<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.XmlModule"
    language="c#" Codebehind="XmlModule.ascx.cs" %>
<rbfwebui:desktopmoduletitle id="ModuleTitle" runat="server" propertiestext="PROPERTIES"
    propertiesurl="~/DesktopModules/CoreModules/Admin/PropertyPage.aspx">
</rbfwebui:desktopmoduletitle>
<span class="Normal"><asp:xml id="xml1" runat="server"></asp:xml></span>