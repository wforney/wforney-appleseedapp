<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.MapQuest"
    language="c#" targetschema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="MapQuest.ascx.cs" %>
<rbfwebui:label id="lblLocation" runat="server" cssclass="NormalBold"></rbfwebui:label><br />
<rbfwebui:label id="lblAddress" runat="server" cssclass="Normal"></rbfwebui:label><br />
<table align="center">
    <tr>
        <td>
            <rbfwebui:hyperlink id="hypMap" runat="server"></rbfwebui:hyperlink></td>
        <td>
            <rbfwebui:localize id="Literal1" runat="server" text="Zoom">
            </rbfwebui:localize><br />
            <asp:radiobuttonlist id="RadioButtonList1" runat="server" autopostback="True">
                <asp:listitem value="1">1</asp:listitem>
                <asp:listitem value="2">2</asp:listitem>
                <asp:listitem value="3">3</asp:listitem>
                <asp:listitem value="4">4</asp:listitem>
                <asp:listitem value="5">5</asp:listitem>
                <asp:listitem value="6">6</asp:listitem>
                <asp:listitem selected="true" value="7">7</asp:listitem>
                <asp:listitem value="8">8</asp:listitem>
                <asp:listitem value="9">9</asp:listitem>
                <asp:listitem value="10">10</asp:listitem>
            </asp:radiobuttonlist></td>
    </tr>
</table>
<!-- WILL ONLY WORK IN US!!
<br> 
<rbfwebui:HyperLink ID="hypDirections" Runat="server" CssClass="CommandButton"></rbfwebui:HyperLink> 
<br> 
-->
