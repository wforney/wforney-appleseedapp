<%@ page autoeventwireup="True" inherits="Appleseed.Content.Web.Modules.ArticlesView" language="c#"
    CodeBehind="ArticlesView.aspx.cs" %>

<%@ register assembly="Appleseed.Framework" namespace="Appleseed.Framework.Web.UI.WebControls"
    tagprefix="rbfwebui" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="ArticlesView" runat="server" method="post">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table cellpadding="0" cellspacing="0" width="80%">
                    <tr>
                        <td rowspan="4" width="10">
                            &nbsp;</td>
                        <td>
                            <a class="Normal" href="javascript:history.go(-1)">&lt;&lt;
                                <rbfwebui:localize id="goBackTop" runat="server" text="Back" textkey="BACK">
                                </rbfwebui:localize></a></td>
                        <td align="right">
                            <rbfwebui:HyperLink id="editLink" runat="server" text="Edit" textkey="EDIT" visible="<%# IsEditable %>"></rbfwebui:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td class="Normal" colspan="2">
                            <p>
                                <rbfwebui:label id="Title" runat="server" cssclass="ItemTitle">&nbsp;</rbfwebui:label>&nbsp;
                                <rbfwebui:label id="Subtitle" runat="server" cssclass="ItemTitle">&nbsp;</rbfwebui:label>&nbsp;
                                <rbfwebui:label id="StartDate" runat="server" cssclass="ItemDate">&nbsp;</rbfwebui:label></p>
                            <p>
                                <rbfwebui:label id="Description" runat="server" cssclass="Normal">&nbsp;</rbfwebui:label></p>
                            <hr noshade="noshade" size="1" />
                            <p>
                            </p>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <a class="Normal" href="javascript:history.go(-1)">&lt;&lt;
                                            <rbfwebui:localize id="goback" runat="server" text="Back" textkey="BACK">
                                            </rbfwebui:localize></a></td>
                                    <td align="right" class="Normal">
                                        <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                                        <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label></td>
                                </tr>
                            </table>
                            <p>
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
