<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.EnhancedLinksEdit"
    language="c#" targetschema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="EnhancedLinksEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">

    <script type="text/javascript" language="JavaScript">
<!--
function newWindow(file,window) {
    msgWindow=open(file,window,'resizable=yes,width=400,height=400,scrollbars=yes');
    if (msgWindow.opener == null) msgWindow.opener = self;
}
-->
    </script>

    <form id="Form1" runat="server">
        <div class="rb_AlternateLayoutDiv">
            <table class="rb_AlternateLayoutTable">
                <tr valign="top">
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br/>
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="150">
                                    &nbsp;</td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head" width="266">
                                                <rbfwebui:localize id="Literal1" runat="server" text="Link detail" textkey="LINKDETAILS">
                                                </rbfwebui:localize></td>
                                        </tr>
                                    </table>
                                    <table border="0" cellpadding="0" cellspacing="0" width="750">
                                        <tr>
                                            <td class="SubHead" width="100">
                                                <rbfwebui:localize id="Literal10" runat="server" text="Link Group?" textkey="LINKGROUP">
                                                </rbfwebui:localize></td>
                                            <td>
                                                <asp:checkbox id="IsGroup" runat="server" autopostback="True" tooltip="Separator for group links" /></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead" width="100">
                                                <rbfwebui:localize id="Literal11" runat="server" text="Icon File" textkey="ICONFILE">
                                                </rbfwebui:localize>:</td>
                                            <td>
                                                <rbfwebui:uploaddialogtextbox id="Src" runat="server" columns="30" cssclass="NormalTextBox"
                                                    maxlength="150" width="390">
                                                </rbfwebui:uploaddialogtextbox></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead" width="100">
                                                <rbfwebui:localize id="Literal2" runat="server" text="Title" textkey="TITLE">
                                                </rbfwebui:localize>:</td>
                                            <td>
                                                <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                                    maxlength="150" width="390"></asp:textbox></td>
                                            <td class="Normal" width="250">
                                                <asp:requiredfieldvalidator id="Req1" runat="server" controltovalidate="TitleField"
                                                    display="Static" errormessage="You Must Enter a Valid Title" textkey="ERROR_VALID_TITLE"></asp:requiredfieldvalidator></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:localize id="UrlFieldLabel" runat="server" text="Url" textkey="URL">
                                                </rbfwebui:localize></td>
                                            <td>
                                                <asp:textbox id="UrlField" runat="server" columns="30" cssclass="NormalTextBox" maxlength="150"
                                                    width="390"></asp:textbox><rbfwebui:localize id="oldUrl" runat="server" text="string.Empty"
                                                        visible="False">
                                                    </rbfwebui:localize></td>
                                            <td class="Normal">
                                                <asp:requiredfieldvalidator id="Req2" runat="server" controltovalidate="UrlField"
                                                    display="Dynamic" errormessage="You Must Enter a Valid URL" textkey="ERROR_VALID_URL"></asp:requiredfieldvalidator></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:localize id="MobileUrlFieldLabel" runat="server" text="Mobile Url" textkey="MOBILEURL">
                                                </rbfwebui:localize></td>
                                            <td>
                                                <asp:textbox id="MobileUrlField" runat="server" columns="30" cssclass="NormalTextBox"
                                                    maxlength="150" width="390"></asp:textbox></td>
                                            <td>
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:localize id="Literal5" runat="server" text="Description" textkey="DESCRIPTION">
                                                </rbfwebui:localize>:</td>
                                            <td>
                                                <asp:textbox id="DescriptionField" runat="server" columns="30" cssclass="NormalTextBox"
                                                    maxlength="150" width="390"></asp:textbox></td>
                                            <td>
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:localize id="TargetFieldLabel" runat="server" text="Target" textkey="TARGET">
                                                </rbfwebui:localize></td>
                                            <td>
                                                <asp:dropdownlist id="TargetField" runat="server" width="390px">
                                                </asp:dropdownlist></td>
                                            <td>
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:localize id="Literal7" runat="server" text="View Order" textkey="VIEWORDER">
                                                </rbfwebui:localize>:</td>
                                            <td>
                                                <asp:textbox id="ViewOrderField" runat="server" columns="30" cssclass="NormalTextBox"
                                                    maxlength="3" width="390"></asp:textbox></td>
                                            <td class="Normal">
                                                <asp:requiredfieldvalidator id="RequiredViewOrder" runat="server" controltovalidate="ViewOrderField"
                                                    display="Static" errormessage="You Must Enter a Valid View Order" textkey="ERROR_VALID_VIEWORDER"></asp:requiredfieldvalidator><asp:comparevalidator
                                                        id="VerifyViewOrder" runat="server" controltovalidate="ViewOrderField" display="Static"
                                                        errormessage="You Must Enter a Valid View Order" operator="DataTypeCheck" textkey="ERROR_VALID_VIEWORDER"
                                                        type="Integer"></asp:comparevalidator></td>
                                        </tr>
                                    </table>
                                    <p>
                                    </p>
                                    <p>
                                    </p>
                                    <p>
                                    </p>
                                    <rbfwebui:linkbutton id="UpdateButton" runat="server" cssclass="CommandButton" text="Update"></rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="CancelButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                                        text="Cancel"></rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="DeleteButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                                        text="Delete this item"></rbfwebui:linkbutton>&nbsp;
                                    <hr noshade="noshade" size="1" width="500" />
                                    <span class="Normal">
                                        <rbfwebui:localize id="Literal8" runat="server" text="Created by" textkey="CREATEDBY">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                                        <rbfwebui:localize id="Literal9" runat="server" text="on" textkey="ON">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label><br/>
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="rb_AlternatePortalFooter">
                        <foot:footer id="Footer" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
