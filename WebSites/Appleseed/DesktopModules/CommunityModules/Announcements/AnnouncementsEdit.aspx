<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>

<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.AnnouncementsEdit"
    language="c#" Codebehind="AnnouncementsEdit.aspx.cs" %>


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
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:localize id="Literal1" runat="server" text="Announce details" textkey="ANNOUNCES_DETAILS">
                            </rbfwebui:localize>&nbsp;
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
                        <td class="SubHead" width="100">
                            <rbfwebui:localize id="Literal2" runat="server" text="Announce Title" textkey="ANNOUNCES_TITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td rowspan="5">
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390">
                            </asp:textbox>
                        </td>
                        <td rowspan="5" width="25">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredTitle" runat="server" controltovalidate="TitleField"
                                display="Dynamic" errormessage="Please insert a valid title" textkey="ERROR_VALID_TITLE">
                            </asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal3" runat="server" text="Read More Link" textkey="ANNOUNCES_READ_MORE_LINK">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="MoreLinkField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal4" runat="server" text="Read More Mobile Link" textkey="ANNOUNCES_READ_MORE_MOBILE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="MobileMoreField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390">
                            </asp:textbox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal5" runat="server" text="Read Descriptions" textkey="ANNOUNCES_DESCRIPTION">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:placeholder id="PlaceHolderHTMLEditor" runat="server"></asp:placeholder>
                        </td>
                        <td class="Normal">
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:localize id="Literal6" runat="server" text="Expires on" textkey="ANNOUNCES_EXPIRES">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            <asp:textbox id="ExpireField" runat="server" columns="8" cssclass="NormalTextBox"
                                width="100">
                            </asp:textbox>
                        </td>
                        <td class="Normal">
                            <asp:requiredfieldvalidator id="RequiredExpireDate" runat="server" controltovalidate="ExpireField"
                                display="Dynamic" errormessage="Please insert a valid expire date" textkey="ERROR_VALID_EXPIRE_DATE">
                            </asp:requiredfieldvalidator>
                            <asp:comparevalidator id="VerifyExpireDate" runat="server" controltovalidate="ExpireField"
                                display="Dynamic" errormessage="Please insert a valid expire date" operator="DataTypeCheck"
                                textkey="ERROR_VALID_EXPIRE_DATE" type="Date">
                            </asp:comparevalidator>
                        </td>
                    </tr>
                </table>
                <p>
                    <asp:placeholder id="PlaceHolderButtons" runat="server"></asp:placeholder>
                </p>
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
