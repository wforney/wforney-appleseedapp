<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>

<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.DiscussionEdit"
    language="c#" Codebehind="DiscussionEdit.aspx.cs" %>

<html>
<head id="Head1" runat="server">
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" name="form1">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="Banner1" runat="server" showtabs="false" />
            </div>
            <div class="div_ev_Table">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left">
                            <rbfwebui:label id="DiscussionEditInstructions" runat="Server" class="Head" text="Add a new thread"
                                textkey="DS_NEWTHREAD">
                            </rbfwebui:label>
                            <%-- <rbfwebui:label class="Head" TextKey="DS_NEWTHREAD" Text="Add a new thread" id="DiscussionEditInstructions" runat="Server" /> --%>
                        </td>
                        <td align="right">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                            </p>
                            <hr noshade="noshade" size="1" />
                            <p>
                            </p>
                            <p>
                                &nbsp;</p>
                        </td>
                    </tr>
                </table>
                <asp:panel id="EditPanel" runat="server" visible="true">
                    <table border="0" cellpadding="4" cellspacing="0" width="600">
                        <tr valign="top">
                            <td class="SubHead" width="150">
                                <rbfwebui:label id="title_label" runat="Server" text="Subject" textkey="SUBJECT"></rbfwebui:label></td>
                            <td rowspan="4">
                                &nbsp;
                            </td>
                            <td width="*">
                                <asp:textbox id="TitleField" runat="server" columns="40" cssclass="NormalTextBox"
                                    maxlength="100" width="500"></asp:textbox><%-- translation needed --%>
                                <asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" controltovalidate="TitleField"
                                    display="Dynamic" errormessage="Please enter a summary of your reply."></asp:requiredfieldvalidator></td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead">
                                <rbfwebui:label id="body_label" runat="Server" text="Body" textkey="DS_BODY"></rbfwebui:label></td>
                            <td width="*">
                                <asp:placeholder id="DescriptionField" runat="server"></asp:placeholder>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <rbfwebui:linkbutton id="submitButton" runat="server" class="CommandButton" text="Submit"></rbfwebui:linkbutton>&nbsp;
                                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" class="CommandButton"
                                    text="Cancel"></rbfwebui:linkbutton>&nbsp;
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead">
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:panel>
                <table border="0" cellpadding="4" cellspacing="0" width="600">
                    <tbody>
                        <itemtemplate>
								<tr vAlign="top">
									<td align="left">
										<asp:Panel id="OriginalMessagePanel" runat="server">
											<table id="Table1" cellSpacing="0" cellPadding="0" border="0">
												<tr>
													<td colSpan="2">
														<rbfwebui:label class="SubHead" id="Label1" runat="server" Text="Original Message" TextKey="DS_ORIGINALMSG"></rbfwebui:label><br />
														<br />
													</td>
												</tr>
												<tr>
													<td>
														<rbfwebui:label class="SubHead" id="Label2" runat="server" Text="Subject" TextKey="SUBJECT"></rbfwebui:label></td>
													<td>&nbsp;
														<rbfwebui:label id="Title" runat="server"></rbfwebui:label></td>
												</tr>
												<tr>
													<td>
														<rbfwebui:label class="SubHead" id="Label3" runat="server" Text="Author" TextKey="AUTHOR"></rbfwebui:label></td>
													<td>&nbsp;
														<rbfwebui:label id="CreatedByUser" runat="server"></rbfwebui:label></td>
												</tr>
												<tr>
													<td>
														<rbfwebui:label class="SubHead" id="Label4" runat="server" Text="Created Date" TextKey="DATE"></rbfwebui:label></td>
													<td>&nbsp;
														<rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label></td>
												</tr>
											</table>
											<br />
											<rbfwebui:label id="Body" runat="server"></rbfwebui:label>
											<p></p>
										</asp:Panel>
									</td>
								</tr>
						</itemtemplate>
                    </tbody>
                </table>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
