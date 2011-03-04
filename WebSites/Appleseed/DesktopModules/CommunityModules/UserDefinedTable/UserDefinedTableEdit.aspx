<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.UserDefinedTableEdit"
    language="c#" Codebehind="UserDefinedTableEdit.aspx.cs" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" enctype="multipart/form-data">
        <div class="rb_AlternateLayoutDiv">
            <table border="0" cellpadding="0" cellspacing="0" class="rb_AlternateLayoutTable">
                <tr valign="top">
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="NoPane">
                        <br />
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="100">
                                    &nbsp;
                                </td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="600">
                                        <tr>
                                            <td align="left" class="Head">
                                                <rbfwebui:label id="EditTableRow" runat="Server" text="Edit Table Row" textkey="USERTABLE_EDITROW">
                                                </rbfwebui:label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr noshade="noshade" size="1" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:table id="tblFields" runat="server">
                                    </asp:table>
                                    <rbfwebui:label id="lblMessage" runat="server" cssclass="NormalRed"></rbfwebui:label>
                                    <hr noshade="noshade" size="1" width="600" />
                                    <p>
                                        <rbfwebui:linkbutton id="UpdateButton" runat="server" class="CommandButton" text="Update">
                                        </rbfwebui:linkbutton>
                                        &nbsp;
                                        <rbfwebui:linkbutton id="CancelButton" runat="server" causesvalidation="False" class="CommandButton"
                                            text="Cancel">
                                        </rbfwebui:linkbutton>
                                        &nbsp;
                                        <rbfwebui:linkbutton id="DeleteButton" runat="server" causesvalidation="False" class="CommandButton"
                                            text="Delete this item">
                                        </rbfwebui:linkbutton>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="rb_AlternatePortalFooter">
                        <div class="rb_AlternatePortalFooter">
                            <foot:footer id="Footer" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
