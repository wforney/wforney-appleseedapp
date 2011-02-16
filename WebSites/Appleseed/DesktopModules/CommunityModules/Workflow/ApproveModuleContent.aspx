<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ApproveModuleContent"
    language="c#" Codebehind="ApproveModuleContent.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="DesktopPortalBanner"
    tagprefix="uc1" %>
<%@ register src="EmailForm.ascx" tagname="EmailForm" tagprefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
  
</head>
<body >
    <form id="RequestModuleContentApproval" runat="server" method="post">
        <div class="rb_AlternateLayoutDiv">
            <table class="rb_AlternateLayoutTable">
                <tr>
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <uc1:desktopportalbanner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="center" nowrap="nowrap" width="100%"><uc1:emailform id="emailForm" runat="server" /></td>
                </tr>
                <tr>
                    <td align="center" nowrap="nowrap">
                        <rbfwebui:linkbutton id="btnApproveAndSendMail" runat="server" cssclass="CommandButton"
                            text="Approve &amp; send mail" textkey="SWI_APPROVESENDMAIL"></rbfwebui:linkbutton>
                        &nbsp;
                        <rbfwebui:linkbutton id="btnApprove" runat="server" cssclass="CommandButton" text="Approve"
                            textkey="SWI_APPROVE"></rbfwebui:linkbutton>
                        &nbsp;
                        <rbfwebui:linkbutton id="CancelButton" runat="server" cssclass="CommandButton" text="Cancel"
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
