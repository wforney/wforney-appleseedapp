<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.WeatherUSEdit"
    language="c#" Codebehind="WeatherUSEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
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
                                    &nbsp;
                                </td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head">
                                                United States Weather information&nbsp;(<a href="http://www.wx.com" target="_blank">www.wx.com</a>)<br/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="SubHead">
                                                <a href="http://www.wx.com/link/index.cfm?link=instruct" target="_blank">Please have
                                                    a look at: WX.COM Custom Link Pages Terms &amp; Conditions</a></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr noshade="noshade" size="1"/>
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                Zip code:
                                            </td>
                                            <td rowspan="3">
                                                &nbsp;
                                            </td>
                                            <td class="Normal">
                                                <asp:textbox id="Zip" runat="server" columns="30" cssclass="NormalTextBox" width="390"></asp:textbox>
                                                <div class="SubHead">
                                                    <asp:requiredfieldvalidator id="rfvZip" runat="server" controltovalidate="Zip" display="Dynamic"
                                                        errormessage="'Zip' must not be left blank."></asp:requiredfieldvalidator></div>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                Option:
                                            </td>
                                            <td class="Normal">
                                                <asp:radiobuttonlist id="Option" runat="server" repeatdirection="Horizontal" width="250px">
                                                    <asp:listitem selected="True" value="0">current conditions</asp:listitem>
                                                    <asp:listitem value="1">local radar</asp:listitem>
                                                </asp:radiobuttonlist>
                                            </td>
                                        </tr>
                                    </table>
                                    <p>
                                        &nbsp;
                                        <asp:placeholder id="PlaceHolderButtons" runat="server"></asp:placeholder>
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
