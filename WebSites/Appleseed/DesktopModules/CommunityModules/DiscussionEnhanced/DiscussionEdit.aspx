<%@ Page AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.DiscussionEnhanced.DiscussionEdit"
    Language="c#" CodeBehind="DiscussionEdit.aspx.cs" %>

<%@ Register Src="~/Design/DesktopLayouts/DesktopFooter.ascx" TagName="Footer" TagPrefix="foot" %>
<%@ Register Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" TagName="Banner"
    TagPrefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" name="form1">
    <div id="zenpanes" class="zen-main">
        <portal:Banner ID="Banner1" runat="server" showtabs="false" />
        <div class="div_ev_Table">
            <table cellpadding="0" cellspacing="0" width="600">
                <tr>
                    <td align="left">
                        <rbfwebui:Label ID="DiscussionEditInstructions" runat="Server" class="Head" Text="Add a new thread"
                            TextKey="DS_NEWTHREAD">
                        </rbfwebui:Label>
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
            <asp:Panel ID="EditPanel" runat="server" Visible="true">
                <table border="0" cellpadding="4" cellspacing="0" width="600">
                    <tr valign="top">
                        <td class="SubHead" width="150">
                            <rbfwebui:Label ID="title_label" runat="Server" Text="Subject" TextKey="SUBJECT"></rbfwebui:Label>
                        </td>
                        <td rowspan="4">
                            &nbsp;
                        </td>
                        <td width="*">
                            <asp:TextBox ID="TitleField" runat="server" Columns="40" CssClass="NormalTextBox"
                                MaxLength="100" Width="500"></asp:TextBox><%-- translation needed --%>
                            <asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server" ControlToValidate="TitleField"
                                Display="Dynamic" ErrorMessage="Please enter a summary of your reply."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead">
                            <rbfwebui:Label ID="body_label" runat="Server" Text="Body" TextKey="DS_BODY"></rbfwebui:Label>
                        </td>
                        <td width="*">
                            <asp:TextBox ID="BodyField" runat="server" Columns="59" CssClass="NormalTextBox"
                                Rows="15" TextMode="Multiline" Width="500"></asp:TextBox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <rbfwebui:LinkButton ID="submitButton" runat="server" class="CommandButton" Text="Submit"></rbfwebui:LinkButton>&nbsp;
                            <rbfwebui:LinkButton ID="CancelButton" runat="server" CausesValidation="False" class="CommandButton"
                                Text="Cancel"></rbfwebui:LinkButton>&nbsp;
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
            </asp:Panel>
            <table border="0" cellpadding="4" cellspacing="0" width="600">
                <tr valign="top">
                    <td align="left">
                        <asp:Panel ID="OriginalMessagePanel" runat="server">
                            <table id="Table1" border="0">
                                <tr>
                                    <td colspan="2">
                                        <rbfwebui:Label class="SubHead" ID="Label1" runat="server" Text="Original Message"
                                            TextKey="DS_ORIGINALMSG"></rbfwebui:Label><br />
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <rbfwebui:Label class="SubHead" ID="Label2" runat="server" Text="Subject" TextKey="SUBJECT"></rbfwebui:Label>
                                    </td>
                                    <td>
                                        &nbsp;
                                        <rbfwebui:Label ID="Title" runat="server"></rbfwebui:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <rbfwebui:Label class="SubHead" ID="Label3" runat="server" Text="Author" TextKey="AUTHOR"></rbfwebui:Label>
                                    </td>
                                    <td>
                                        &nbsp;
                                        <rbfwebui:Label ID="CreatedByUser" runat="server"></rbfwebui:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <rbfwebui:Label class="SubHead" ID="Label4" runat="server" Text="Created Date" TextKey="DATE"></rbfwebui:Label>
                                    </td>
                                    <td>
                                        &nbsp;
                                        <rbfwebui:Label ID="CreatedDate" runat="server"></rbfwebui:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <rbfwebui:Label ID="Body" runat="server"></rbfwebui:Label>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <foot:Footer ID="Footer" runat="server" />
    </div>
    </form>
</body>
</html>
