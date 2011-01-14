<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="DesktopPortalBanner"
    tagprefix="uc1" %>

<%@ page autoeventwireup="false"
    inherits="Appleseed.Content.Web.Modules.RequestModuleContentApproval" language="c#" Codebehind="RequestModuleContentApproval.aspx.cs" %>

<%@ register src="EmailForm.ascx" tagname="EmailForm" tagprefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server"><title></title>
   
</head>
<body id="Body1" runat="server">
    <form id="RequestModuleContentApprovalForm" runat="server" method="post">
        <div class="rb_AlternateLayoutDiv">
            <table class="rb_AlternateLayoutTable">
                <tr>
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <uc1:desktopportalbanner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="center" nowrap="nowrap" width="100%">
                        <uc1:emailform id="emailForm" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="center" nowrap="nowrap">
                        <rbfwebui:linkbutton id="btnRequestApprovalAndSendMail" runat="server" cssclass="CommandButton"
                            text="Request approve &amp; send mail" textkey="SWI_READYTOAPPROVESENDMAIL"></rbfwebui:linkbutton>
                        &nbsp;
                        <rbfwebui:linkbutton id="btnRequestApproval" runat="server" cssclass="CommandButton"
                            text="Request approval" textkey="SWI_READYTOAPPROVE"></rbfwebui:linkbutton>
                        &nbsp;
                        <rbfwebui:linkbutton id="cancelButton" runat="server" cssclass="CommandButton" text="Cancel"
                            textkey="CANCEL"></rbfwebui:linkbutton>
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
