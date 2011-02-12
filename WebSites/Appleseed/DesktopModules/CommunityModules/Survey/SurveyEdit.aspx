<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SurveyEdit"
    language="c#" Codebehind="SurveyEdit.aspx.cs" %>

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
                        <br/>
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="150">
                                    &nbsp;</td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head">
                                                <rbfwebui:label id="lblTitle" runat="server" cssclass="Head" text="Survey Details"
                                                    textkey="SURVEY_DETAILS"></rbfwebui:label></td>
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
                                                <rbfwebui:label id="label11" runat="server" cssclass="Title" font-bold="True" text="Survey:"
                                                    textkey="SURVEY_TITLE" width="100px"></rbfwebui:label></td>
                                            <td colspan="2">
                                                <rbfwebui:label id="lblDescSurvey" runat="server" cssclass="Title" height="48px"
                                                    width="100%"></rbfwebui:label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <rbfwebui:linkbutton id="addBtn" runat="server" cssclass="CommandButton" onclick="AddBtn_Click"
                                                    text="Add New Question" textkey="SURVEY_ADDNEWQUESTION"></rbfwebui:linkbutton></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br/>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td class="SubHead">
                                                <rbfwebui:label id="lblNewQuestion" runat="server" cssclass="Normal" height="60px"
                                                    text="New Question:" textkey="SURVEY_NEWQUESTION" visible="False"></rbfwebui:label></td>
                                            <td>
                                                <asp:textbox id="txtNewQuestion" runat="server" cssclass="NormalTextBox" height="48px"
                                                    visible="False" width="100%"></asp:textbox></td>
                                            <td class="Normal">
                                                &nbsp;<asp:requiredfieldvalidator id="ReqQuestion" runat="server" controltovalidate="txtNewQuestion"
                                                    cssclass="Normal" errormessage="Please, insert the question." textkey="SURVEY_QUESTION_ERR"
                                                    visible="False" width="266px"></asp:requiredfieldvalidator></td>
                                        </tr>
                                        <tr>
                                            <td class="SubHead">
                                                <rbfwebui:label id="lblOptionType" runat="server" cssclass="Normal" height="27px"
                                                    text="Option Type:" textkey="SURVEY_OPTIONTYPE" visible="False" width="100px"></rbfwebui:label></td>
                                            <td colspan="2">
                                                <asp:radiobutton id="RdBtnCheck" runat="server" cssclass="Normal" groupname="Type"
                                                    height="33px" text="Checkboxes" visible="False" width="100px" /><asp:radiobutton
                                                        id="RdBtnRadio" runat="server" checked="True" cssclass="Normal" groupname="Type"
                                                        height="33px" text="Radio buttons" visible="False" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" height="25">
                                                <rbfwebui:linkbutton id="AddQuestionBtn" runat="server" cssclass="CommandButton"
                                                    onclick="AddQuestionBtn_Click" visible="False"></rbfwebui:linkbutton>
                                                <rbfwebui:linkbutton id="btnCancel" runat="server" cssclass="CommandButton" onclick="btnCancel_Click"
                                                    text="Cancel" textkey="CANCEL" visible="False"></rbfwebui:linkbutton></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br/>
                                            </td>
                                        </tr>
                                        <tr valign="top">
                                            <td class="SubHead">
                                                <rbfwebui:label id="lblQuestion" runat="server" cssclass="Normal" text="Questions:"
                                                    textkey="SURVEY_QUESTIONS" width="100px"></rbfwebui:label></td>
                                            <td>
                                                <asp:listbox id="QuestionList" runat="server" cssclass="NormalTextBox" height="106px"
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
                                                            <rbfwebui:imagebutton id="editBtn" runat="server" onclick="EditBtn_Click" text="Edit button"
                                                                textkey="EDITBTN" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <rbfwebui:imagebutton id="deleteBtn" runat="server" onclick="DeleteBtn_Click" text="Delete button"
                                                                textkey="DELETEBTN" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <rbfwebui:linkbutton id="btnBackSurvey" runat="server" cssclass="CommandButton" onclick="btnBackSurvey_Click"
                                                    text="Return" textkey="RETURN"></rbfwebui:linkbutton>
                                            </td>
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
