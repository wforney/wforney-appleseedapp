<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ComponentModuleEdit"
    language="c#" Codebehind="ComponentModuleEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left" class="Head">
                            Details
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="750">
                    <tr valign="top">
                        <td class="SubHead" width="100">
                            Title
                        </td>
                        <td rowspan="4">
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="150" width="490">
                            </asp:textbox>
                        </td>
                        <td rowspan="4" width="25">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredTitle" runat="server" controltovalidate="TitleField"
                                display="Dynamic">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            Component
                        </td>
                        <td>
                            <asp:textbox id="ComponentField" runat="server" columns="44" rows="10" textmode="Multiline"
                                width="490">
                            </asp:textbox>
                        </td>
                        <td class="Normal">
                            <asp:requiredfieldvalidator id="RequiredComponent" runat="server" controltovalidate="ComponentField"
                                display="Dynamic">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                </table>
                <p>
                    <rbfwebui:linkbutton id="updateButton" runat="server" class="CommandButton" text="UPDATE">
                    </rbfwebui:linkbutton>
                    &nbsp;
                    <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" class="CommandButton"
                        text="CANCEL">
                    </rbfwebui:linkbutton>
                </p>
                <hr noshade="noshade" size="1" width="600" />
                <span class="Normal">
                    <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                    <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                </span>
                <div class="rb_AlternatePortalFooter">
                    <foot:footer id="Footer" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
