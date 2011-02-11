<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.DocumentsEdit"
    language="c#" Codebehind="DocumentsEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
    
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" enctype="multipart/form-data">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:label id="PageTitleLabel" runat="server" height="22"></rbfwebui:label></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr valign="top">
                        <td class="SubHead" width="100">
                            <rbfwebui:label id="FileNameLabel" runat="server" height="22" textkey="FILE_NAME">File name</rbfwebui:label></td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="NameField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="150" width="353"></asp:textbox></td>
                        <td rowspan="6" width="25">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" controltovalidate="NameField"
                                cssclass="Error" display="Static" errormessage="You Must Enter a Valid Name"
                                textkey="ERROR_VALID_NAME"></asp:requiredfieldvalidator></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:label id="CategoryLabel" runat="server" height="22" textkey="FILE_CATEGORY">Category</rbfwebui:label></td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="CategoryField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="50" width="353"></asp:textbox></td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" width="100%" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" width="100">
                            <rbfwebui:label id="UrlLabel" runat="server" height="22" textkey="URL">Url</rbfwebui:label></td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="PathField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="250" width="353"></asp:textbox></td>
                    </tr>
                    <tr>
                        <td class="SubHead">
                            <rbfwebui:label id="OrLabel" runat="server" height="22" textkey="OR">or</rbfwebui:label></td>
                        <td colspan="2">
                            &nbsp;
                            <br />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap" valign="middle">
                            <rbfwebui:label id="UploadLabel" runat="server" height="22" textkey="FILE_UPLOAD">Upload File</rbfwebui:label>&nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <input id="FileUpload" runat="server" name="FileUpload" style="width: 353px; font-family: verdana"
                                type="file" width="300" />
                        </td>
                    </tr>
                </table>
                <p>
                </p>
                <rbfwebui:linkbutton id="updateButton" runat="server" class="CommandButton" text="Update"></rbfwebui:linkbutton>&nbsp;
                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" class="CommandButton"
                    text="Cancel"></rbfwebui:linkbutton>&nbsp;
                <rbfwebui:linkbutton id="deleteButton" runat="server" causesvalidation="False" class="CommandButton"
                    text="Delete this item"></rbfwebui:linkbutton>
                <hr noshade="noshade" size="1" />
                <span class="Normal">
                    <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                    <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                </span>
                <p>
                    <rbfwebui:label id="Message" runat="server" cssclass="NormalRed" forecolor="Red"></rbfwebui:label>
                </p>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
