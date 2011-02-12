<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.EnhancedLinks"
    language="c#" targetschema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="EnhancedLinks.ascx.cs" %>
<table border="0" cellpadding="4" cellspacing="0" width="100%">
    <tr>
        <td>
            <asp:dropdownlist id="cboLinks" runat="server" autopostback="True" cssclass="NormalTextBox"
                visible="False">
            </asp:dropdownlist></td>
    </tr>
</table>
<asp:panel id="results" runat="server" visible="False">
</asp:panel>
