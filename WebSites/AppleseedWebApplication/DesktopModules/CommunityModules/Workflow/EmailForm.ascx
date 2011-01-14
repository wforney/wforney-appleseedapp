<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.EmailForm" language="c#" %>
<table width="100%">
    <tr>
        <td align="right" width="20%">
            <span class="Normal">
                <rbfwebui:localize id="Literal1" runat="server" text="To" textkey="EMF_TO">
                </rbfwebui:localize>:</span></td>
        <td align="left">
            <asp:textbox id="txtTo" runat="server" width="80%"></asp:textbox></td>
    </tr>
    <tr>
        <td align="right" width="20%">
            <span class="Normal">
                <rbfwebui:localize id="Literal2" runat="server" text="Cc" textkey="EMF_CC">
                </rbfwebui:localize>:</span></td>
        <td align="left">
            <asp:textbox id="txtCc" runat="server" width="80%"></asp:textbox></td>
    </tr>
    <tr>
        <td align="right" width="20%string.Empty">
            <span class="Normal">
                <rbfwebui:localize id="Literal3" runat="server" text="Bcc" textkey="EMF_BCC">
                </rbfwebui:localize>:</span></td>
        <td align="left">
            <asp:textbox id="txtBcc" runat="server" width="80%"></asp:textbox></td>
    </tr>
    <tr>
        <td align="right" width="20%">
            <span class="Normal">
                <rbfwebui:localize id="Literal4" runat="server" text="Subject" textkey="EMF_SUBJECT">
                </rbfwebui:localize></span>:</td>
        <td align="left">
            <asp:textbox id="txtSubject" runat="server" width="80%"></asp:textbox></td>
    </tr>
    <tr>
    </tr><tr>
    <td align="center" colspan="2">
        <asp:placeholder id="PlaceHolderHTMLEditor" runat="server"></asp:placeholder>
    </td></tr>
    <tr>
        <td align="center" colspan="2">
            <rbfwebui:label id="lblEmailAddressesNotOk" runat="server" cssclass="Normal" forecolor="Red"
                visible="False"></rbfwebui:label></td>
    </tr>
</table>
