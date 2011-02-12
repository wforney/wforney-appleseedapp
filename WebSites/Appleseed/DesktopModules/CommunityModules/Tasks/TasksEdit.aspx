<%@ register assembly="DateTextBoxControls" namespace="PeterBlum.DateTextBoxControls"
    tagprefix="Date" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>

<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.TasksEdit"
    language="c#" Codebehind="TasksEdit.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0">
                 
                        <tr>
                            <td align="left" class="Head" colspan="5">
                                <rbfwebui:localize id="Literal1" runat="server" text="Task Detail" textkey="TASK_DETAIL">
                                </rbfwebui:localize></td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <hr noshade="noshade" size="1" width="600" />
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal2" runat="server" text="Title" textkey="TASK_TITLE">
                                </rbfwebui:localize>:
                            </td>
                            <td rowspan="8" width="5">
                            </td>
                            <td width="391">
                                <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                    maxlength="150" width="390"></asp:textbox></td>
                            <td rowspan="6" width="25">
                            </td>
                            <td class="Normal" width="218">
                                <asp:requiredfieldvalidator id="RequiredTitle" runat="server" controltovalidate="TitleField"
                                    display="Dynamic" errormessage="You must enter a valid title" textkey="TASK_TITLE_ERROR"></asp:requiredfieldvalidator></td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal3" runat="server" text="Description" textkey="TASK_DESCRIPTION">
                                </rbfwebui:localize>:
                            </td>
                            <td colspan="3" width="391">
                                <asp:placeholder id="DescriptionField" runat="server"></asp:placeholder>
                            </td>
                            <td class="Normal" width="218">
                            </td>
                        </tr>
                        <tr valign="top">
                        </tr><tr>
                        <td class="SubHead" width="179">
                            <rbfwebui:localize id="Literal4" runat="server" text="% Complete" textkey="TASK_COMPLETION">
                            </rbfwebui:localize>:
                        </td>
                        <td colspan="3" width="391">
                            <asp:textbox id="PercentCompleteField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="150" width="50px">0</asp:textbox>&nbsp;%</td>
                        <td class="Normal" width="218">
                            <asp:rangevalidator id="PercentValidator" runat="server" controltovalidate="PercentCompleteField"
                                errormessage="Must be an integer from 0 to 100" maximumvalue="100" minimumvalue="0"
                                textkey="TASK_COMPLETION_ERROR" type="Integer"></asp:rangevalidator></td></tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal5" runat="server" text="Status" textkey="TASK_STATUS">
                                </rbfwebui:localize>:</td>
                            <td width="391">
                                <asp:dropdownlist id="StatusField" runat="server" cssclass="NormalTextBox" width="120px">
                                </asp:dropdownlist></td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal6" runat="server" text="Priority" textkey="TASK_PRIORITY">
                                </rbfwebui:localize>:</td>
                            <td colspan="3" width="391">
                                <asp:dropdownlist id="PriorityField" runat="server" cssclass="NormalTextBox" width="120px">
                                </asp:dropdownlist></td>
                            <td width="218">
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal7" runat="server" text="Assigned To" textkey="TASK_ASSIGNEDTO">
                                </rbfwebui:localize>:
                            </td>
                            <td colspan="3" width="391">
                                <asp:textbox id="AssignedField" runat="server" cssclass="NormalTextBox" width="197px"></asp:textbox></td>
                            <td width="218">
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" width="179">
                                <rbfwebui:localize id="Literal8" runat="server" text="Start Date" textkey="TASK_STARTDATE">
                                </rbfwebui:localize>:
                            </td>
                            <td colspan="3">
                                <date:datetextbox id="StartField" runat="server" designtimedragdrop="292" xendrangecontrolid="DueField"
                                    xpopupwidth="150" xstartrangecontrolid="StartField"></date:datetextbox></td>
                            <td>
                                <asp:requiredfieldvalidator id="RequiredEndDate" runat="server" controltovalidate="StartField"
                                    cssclass="Normal" display="Dynamic" errormessage="You Must Enter a Valid Start Date"
                                    textkey="TASK_STARTDATE_ERROR"></asp:requiredfieldvalidator>
                                <date:datetextboxvalidator id="VerifyEndDate" runat="server" controltovalidate="StartField"
                                    errormessage="You Must Enter a Valid Start Date" textkey="TASK_STARTDATE_ERROR"></date:datetextboxvalidator>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead">
                                <rbfwebui:localize id="Literal9" runat="server" text="Due Date" textkey="TASK_DUEDATE">
                                </rbfwebui:localize>:
                            </td>
                            <td colspan="3" width="391">
                                <date:datetextbox id="DueField" runat="server" xendrangecontrolid="DueField" xpopupwidth="150"
                                    xstartrangecontrolid="StartField"></date:datetextbox></td>
                            <td class="Normal">
                                <asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" controltovalidate="DueField"
                                    cssclass="Normal" display="Dynamic" errormessage="You Must Enter a Valid Due Date"
                                    textkey="TASK_DUEDATE_ERROR"></asp:requiredfieldvalidator>
                                <date:datetextboxvalidator id="Datetextboxvalidator1" runat="server" controltovalidate="DueField"
                                    errormessage="You Must Enter a Valid Due Date" textkey="TASK_DUEDATE_ERROR"></date:datetextboxvalidator>
                            </td>
                        </tr>
                 
                </table>
                <p>
                </p>
                <rbfwebui:linkbutton id="updateButton" runat="server" class="CommandButton" text="Update">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" class="CommandButton"
                    text="Cancel">
                </rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="deleteButton" runat="server" causesvalidation="False" class="CommandButton"
                    text="Delete this item">
                </rbfwebui:linkbutton>
                <br/>
                <hr noshade="noshade" size="1" width="600" />
                <span class="Normal">
                    <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                    </rbfwebui:localize>
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>
                    <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                    <br/>
                    <rbfwebui:localize id="ModifiedLabel" runat="server" text="Modified by" textkey="MODIFIED_BY">
                    </rbfwebui:localize>
                    <rbfwebui:label id="ModifiedBy" runat="server"></rbfwebui:label>
                    <rbfwebui:localize id="ModifiedOnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>
                    <rbfwebui:label id="ModifiedDate" runat="server"></rbfwebui:label>
                </span>
                <p>
                </p>
                <rbfwebui:localize id="PickDateCalendarScript" runat="server">
                </rbfwebui:localize>
               
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
