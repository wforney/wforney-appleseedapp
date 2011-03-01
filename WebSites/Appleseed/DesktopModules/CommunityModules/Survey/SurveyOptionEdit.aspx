<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SurveyOptionEdit"
    language="c#" Codebehind="SurveyOptionEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="form1" runat="server" method="post">
        <div class="rb_AlternateLayoutDiv">
            <table class="rb_AlternateLayoutTable">
                <tr valign="top">
                    <td class="rb_AlternatePortalHeader">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="150">
                                    &nbsp;</td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head">
                                                <rbfwebui:label id="lblTitle" runat="server" cssclass="Head" text="Survey Details - Options"
                                                    textkey="SURVEY_DETAILSOPTIONS"></rbfwebui:label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr noshade="noshade" size="1" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table border="0" cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead">
                                                <rbfwebui:label id="label11" runat="server" cssclass="Title" font-bold="True" text="Question:"
                                                    textkey="SURVEY_QUESTION" width="100px"></rbfwebui:label></td>
                                            <td colspan="2">
                                                <rbfwebui:label id="lblQuestion" runat="server" cssclass="Title" height="48px" width="100%"></rbfwebui:label></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:label id="Label1" runat="server" cssclass="Normal" text="Option Type:"
                                                    textkey="SURVEY_OPTIONTYPE" width="100px"></rbfwebui:label></td>
                                            <td colspan="2">
                                                <rbfwebui:label id="lblTypeOption" runat="server" cssclass="Normal" width="100px"></rbfwebui:label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br/>
                                                <rbfwebui:linkbutton id="AddOptBtn" runat="server" cssclass="CommandButton" onclick="AddOptBtn_Click"
                                                    text="Add new option" textkey="SURVEY_ADDNEWOPTION"></rbfwebui:linkbutton></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br/>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td class="SubHead">
                                                <rbfwebui:label id="lblNewOption" runat="server" cssclass="Normal" height="60px"
                                                    text="New option:" textkey="SURVEY_NEWOPTION" visible="False"></rbfwebui:label></td>
                                            <td>
                                                <asp:textbox id="TxtNewOption" runat="server" cssclass="NormalTextBox" height="48px"
                                                    visible="False" width="100%"></asp:textbox></td>
                                            <td class="Normal">
                                                &nbsp;<asp:requiredfieldvalidator id="ReqNewOpt" runat="server" controltovalidate="TxtNewOption"
                                                    cssclass="Normal" errormessage="Please, insert the option." textkey="SURVEY_OPTION_ERR"
                                                    visible="False" width="266px"></asp:requiredfieldvalidator></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" height="25">
                                                <rbfwebui:linkbutton id="AddOptionBtn" runat="server" cssclass="CommandButton" onclick="AddOptionBtn_Click"
                                                    visible="False"></rbfwebui:linkbutton><rbfwebui:linkbutton id="CancelOptBtn" runat="server"
                                                        cssclass="CommandButton" onclick="CancelOptBtn_Click" text="Cancel" textkey="CANCEL"
                                                        visible="False"></rbfwebui:linkbutton></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br/>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td class="SubHead">
                                                <rbfwebui:label id="lblOption" runat="server" cssclass="Normal" text="Options:" textkey="SURVEY_OPTIONS"
                                                    width="100px"></rbfwebui:label></td>
                                            <td>
                                                <asp:listbox id="OptionList" runat="server" cssclass="NormalTextBox" height="106px"
                                                    rows="5" width="400px"></asp:listbox></td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <rbfwebui:imagebutton id="upBtn" runat="server" commandname="up" onclick="UpDown_Click"
                                                                text="Move up" textkey="MOVEUP" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <rbfwebui:imagebutton id="downBtn" runat="server" commandname="down" onclick="UpDown_Click"
                                                                text="Move down" textkey="MOVEDOWN" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <rbfwebui:imagebutton id="deleteBtn" runat="server" onclick="deleteBtn_Click" text="Delete button"
                                                                textkey="DELETEBTN" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <rbfwebui:linkbutton id="btnReturnCancel" runat="server" cssclass="CommandButton"
                                                    onclick="btnReturnCancel_Click" text="Return" textkey="RETURN"></rbfwebui:linkbutton></td>
                                        </tr>
                                    </table>
                                    <p>
                                    </p>
                                    <hr noshade="noshade" size="1" width="500" />
                                    <span class="Normal">
                                        <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                                        <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label></span>
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
