<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ContactsEdit"
    language="c#" Codebehind="ContactsEdit.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
                            <rbfwebui:localize id="Literal1" runat="server" text="Conctact details" textkey="CONTACTS_DETAILS">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal2" runat="server" text="Name" textkey="CONTACTS_NAME">
                            </rbfwebui:localize>:
                        </td>
                        <td rowspan="7">
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:textbox id="NameField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="50" width="390">
                            </asp:textbox>
                        </td>
                        <td rowspan="7" width="25">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredName" runat="server" controltovalidate="NameField"
                                display="Dynamic" errormessage="Please enter a vaild name" textkey="ERROR_VALID_NAME">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal3" runat="server" text="Role" textkey="CONTACTS_ROLE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="RoleField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal4" runat="server" text="Email" textkey="CONTACTS_EMAIL">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="EmailField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390">
                            </asp:textbox>
                        </td>
                        <td class="Normal" width="250">
                            <asp:regularexpressionvalidator id="ValidEmail" runat="server" controltovalidate="EmailField"
                                display="Dynamic" errormessage="You must use a valid email address" textkey="VALID_EMAIL"
                                validationexpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:regularexpressionvalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal5" runat="server" text="Office" textkey="CONTACTS_CONTACT1">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="Contact1Field" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="250" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal6" runat="server" text="Mobile" textkey="CONTACTS_CONTACT2">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="Contact2Field" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="250" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal7" runat="server" text="Fax" textkey="CONTACTS_FAX">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="FaxField" runat="server" columns="30" cssclass="NormalTextBox" maxlength="250"
                                width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal8" runat="server" text="Address" textkey="CONTACTS_ADDRESS">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="AddressField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="250" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                </table>
                <rbfwebui:linkbutton id="updateButton" runat="server" cssclass="CommandButton" text="UPDATE">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="CANCEL">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="deleteButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="Delete this item">
                </rbfwebui:linkbutton>
                <hr noshade="noshade" size="1" />
                <span class="Normal">
                    <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                    <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                </span>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
