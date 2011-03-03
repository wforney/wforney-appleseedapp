<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.UploadFlash"
    language="c#" Codebehind="UploadFlash.aspx.cs" %>

<html>
<head id="Head1" runat="server">
</head>
<body id="Body1" runat="server">
    <form id="Form" runat="server" enctype="multipart/form-data">
        <asp:panel id="uploadpanel" runat="server">
            <table height="50" width="100%">
                <tr>
                    <td class="HeadBg">
                        <rbfwebui:label id="Label5" runat="server" cssclass="SiteTitle" text="Flash Gallery"
                            textkey="FLASH_GALLERY"></rbfwebui:label>:</td>
                </tr>
                <tr>
                    <td>
                        <rbfwebui:label id="gallerymessage" runat="server" cssclass="Message"></rbfwebui:label></td>
                </tr>
            </table>
            <asp:table id="flashTable" runat="server" enableviewstate="True" height="300" style="overflow: scroll"
                width="100%">
            </asp:table>
            <hr />
            <rbfwebui:label id="Label1" runat="server" class="Head" text="Upload Flash File"
                textkey="FLASH_UPLOAD_TITLE"></rbfwebui:label>
            <table>
                <tr>
                    <td class="SubHead">
                        <rbfwebui:label id="Label2" runat="server" text="1. Select the image" textkey="FLASH_SELECT_FILE"></rbfwebui:label>:</td>
                    <td class="SubHead">
                        <rbfwebui:label id="uploadlabel" runat="server"></rbfwebui:label></td>
                    <td>
                        <input id="uploadfile" runat="server" cssclass="CommandButton" name="uploadfile"
                            type="file"></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <rbfwebui:label id="uploadmessage" runat="server" cssclass="Message"></rbfwebui:label></td>
                </tr>
                <tr>
                    <td class="SubHead">
                        <rbfwebui:label id="Label3" runat="server" text="2. Upload the (*.swf)File" textkey="FLASH_UPLOAD_FILE"></rbfwebui:label>:</td>
                    <td>
                    </td>
                    <td>
                        <rbfwebui:button id="uploadButton" runat="server" cssclass="CommandButton" text="Upload"
                            textkey="UPLOAD_FILE" /></td>
                </tr>
            </table>
        </asp:panel>
    </form>
</body>
</html>
