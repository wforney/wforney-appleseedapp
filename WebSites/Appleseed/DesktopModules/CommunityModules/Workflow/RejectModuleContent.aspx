
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DesktopPortalBanner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="EmailForm" Src="EmailForm.ascx" %>
<%@ Page language="c#" AutoEventWireup="false" inherits="Appleseed.Content.Web.Modules.RejectModuleContent" Codebehind="RejectModuleContent.aspx.cs" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title></title>
	</head>
	<body runat="server">
<form id="RequestModuleContentApproval" method="post" runat="server">
			<div class="rb_AlternateLayoutDiv">
			<table class="rb_AlternateLayoutTable">
				<tr>
				<td class="rb_AlternatePortalHeader" valign="top">
				<uc1:DesktopPortalBanner id="SiteHeader" runat="server"></uc1:DesktopPortalBanner>
				</td>
				</tr>
				<tr>
			<td nowrap="nowrap" align="center" width="100%"><uc1:emailform id="emailForm" runat="server"></uc1:emailform></td>
				</tr>
				<tr>
			<td nowrap="nowrap" align="center">
				<rbfwebui:LinkButton TextKey="SWI_REJECTSENDMAIL" Text="Reject &amp; send mail" id="btnRejectAndSendMail" runat="server" CssClass="CommandButton"></rbfwebui:LinkButton>
				&nbsp;
				<rbfwebui:LinkButton TextKey="SWI_REJECT" Text="Reject" id="btnReject" runat="server" CssClass="CommandButton"></rbfwebui:LinkButton>
				&nbsp;
				<rbfwebui:LinkButton TextKey="CANCEL" Text="Cancel" id="cancelButton" runat="server" CssClass="CommandButton"></rbfwebui:LinkButton>
					</td>
				</tr>
				<tr>
				<td class="rb_AlternatePortalFooter"><div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></td>
				</tr>
			</table>
			</div>
</form>
</body>
</html>
