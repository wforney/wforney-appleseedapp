<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ArticlesEdit" language="c#" Codebehind="ArticlesEdit.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="HEAD1" runat="server"> <title></title></head>
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
                            <rbfwebui:localize id="Literal1" runat="server" text="Article Detail" textkey="ARTICLE_DETAIL">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
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
                            <asp:comparevalidator id="VerifyStartDate" runat="server" controltovalidate="StartField"
                                display="Dynamic" errormessage="You Must Enter a Valid Start Date" operator="DataTypeCheck"
                                textkey="ERROR_VALID_STARTDATE" type="Date"></asp:comparevalidator>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal3" runat="server" text="Expire date" textkey="EXPIRE_DATE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            &nbsp;
                            <asp:textbox id="ExpireField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="150" width="353"></asp:textbox></td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="RequiredExpireDate" runat="server" controltovalidate="ExpireField"
                                display="Dynamic" errormessage="You Must Enter a Valid Expiration Date" textkey="ERROR_VALID_EXPIRE_DATE"></asp:requiredfieldvalidator>
                            <asp:comparevalidator id="VerifyExpireDate" runat="server" controltovalidate="ExpireField"
                                display="Dynamic" errormessage="You Must Enter a Valid Expiration Date" operator="DataTypeCheck"
                                textkey="ERROR_VALID_EXPIRE_DATE" type="Date"></asp:comparevalidator>
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
                            <rbfwebui:localize id="Literal5" runat="server" text="Subtitle" textkey="SUBTITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td>
                            &nbsp;
                            <asp:textbox id="SubtitleField" runat="server" columns="28" cssclass="NormalTextBox"
                                maxlength="50" width="353"></asp:textbox></td>
                        <td>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal6" runat="server" text="Abstract" textkey="ABSTRACT">
                            </rbfwebui:localize>:
                        </td>
                        <td colspan="2">
                            <asp:placeholder id="PlaceHolderAbstractHTMLEditor" runat="server"></asp:placeholder>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal7" runat="server" text="Description" textkey="DESCRIPTION">
                            </rbfwebui:localize>:
                        </td>
                        <td colspan="2">
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
