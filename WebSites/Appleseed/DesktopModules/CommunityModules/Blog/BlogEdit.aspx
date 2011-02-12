<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>

<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.BlogEdit"
    language="c#" Codebehind="BlogEdit.aspx.cs" %>



<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" enctype="multipart/form-data">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:localize id="Literal1" runat="server" text="Blog Entry" textkey="BLOG_ENTRY">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="0" cellspacing="4" class="Normal" width="726">
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal2" runat="server" text="Start date" textkey="START_DATE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            &nbsp;
                            <asp:textbox id="StartField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="150" width="353"></asp:textbox>
                        </td>
                        <td rowspan="5" width="25">
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredStartDate" runat="server" controltovalidate="StartField"
                                display="Dynamic" errormessage="You Must Enter a Valid Start Date" textkey="ERROR_VALID_STARTDATE"></asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal4" runat="server" text="Title" textkey="TITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            &nbsp;
                            <asp:textbox id="TitleField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="150" width="353"></asp:textbox></td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" controltovalidate="TitleField"
                                display="Dynamic" errormessage="You Must Enter a Valid Title" textkey="ERROR_VALID_TITLE"></asp:requiredfieldvalidator></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal6" runat="server" text="Excerpt" textkey="EXCERPT">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            &nbsp;
                            <asp:textbox id="ExcerptField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="5000" rows="4" textmode="MultiLine" width="353"></asp:textbox></td>
                        <td>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal7" runat="server" text="Description" textkey="DESCRIPTION">
                            </rbfwebui:localize>:
                        </td>
                        <td colspan="3">
                            &nbsp;
                            <asp:placeholder id="PlaceHolderHTMLEditor" runat="server"></asp:placeholder>
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
