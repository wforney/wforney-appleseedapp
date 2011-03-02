<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.TaskView" language="c#" Codebehind="TasksView.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<html>
<head id="Head1" runat="server">
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td align="left" class="Head" colspan="2" width="120">
                            <rbfwebui:localize id="Literal1" runat="server" text="Task Detail" textkey="TASK_DETAIL">
                            </rbfwebui:localize>
                        </td>
                        <td align="right">
                            <%= EditLink%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <hr noshade="noshade" size="1">
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal2" runat="server" text="Title" textkey="TASK_TITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="TitleField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal3" runat="server" text="Description" textkey="TASK_DESCRIPTION">
                            </rbfwebui:localize>:
                        </td>
                        <td colspan="2" valign="top">
                            <rbfwebui:label id="longdesc" runat="server">No Description Available</rbfwebui:label>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal4" runat="server" text="% Complete" textkey="TASK_COMPLETION">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="PercentCompleteField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal5" runat="server" text="Status" textkey="TASK_STATUS">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="StatusField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal6" runat="server" text="Priority" textkey="TASK_PRIORITY">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="PriorityField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal7" runat="server" text="Assigned To" textkey="TASK_ASSIGNEDTO">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="AssignedField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                            <rbfwebui:localize id="Literal8" runat="server" text="Start Date" textkey="TASK_STARTDATE">
                            </rbfwebui:localize>:
                        </td>
                        <td class="Normal" colspan="2" valign="top">
                            <rbfwebui:label id="StartField" runat="server"></rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" nowrap="nowrap" valign="top" width="170">
                        </td>
                            <rbfwebui:localize id="Literal9" runat="server" text="Due Date" textkey="TASK_DUEDATE">
                            </rbfwebui:localize>
                            :
                            <td class="Normal" colspan="2" valign="top">
                                <rbfwebui:label id="DueField" runat="server"></rbfwebui:label>
                            </td>
                    </tr>
                </table>
                <p>
                    <rbfwebui:LinkButton id="CancelButton" runat="server" causesvalidation="False" class="CommandButton"
                        text="Cancel">
                    </rbfwebui:LinkButton>
                    <br>
                    <hr noshade="noshade" size="1" width="600">
                    <span class="Normal">
                        <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                        </rbfwebui:localize>
                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>
                        <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                        </rbfwebui:localize>
                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                        <br>
                        <rbfwebui:localize id="ModifiedLabel" runat="server" text="Modified by" textkey="MODIFIED_BY">
                        </rbfwebui:localize>
                        <rbfwebui:label id="ModifiedBy" runat="server"></rbfwebui:label>
                        <rbfwebui:localize id="ModifiedOnLabel" runat="server" text="on" textkey="ON">
                        </rbfwebui:localize>
                        <rbfwebui:label id="ModifiedDate" runat="server"></rbfwebui:label>
                    </span>
                </p>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
            </foot:Footer>
        </div>
    </form>
</body>
</html>
