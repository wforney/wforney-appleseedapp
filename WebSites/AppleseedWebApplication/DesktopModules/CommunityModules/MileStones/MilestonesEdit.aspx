<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.MilestonesEdit"
    language="c#" Codebehind="MilestonesEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
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
                            <rbfwebui:localize id="Literal1" runat="server" text="Milestones Details" textkey="MILESTONES_DETAIL">
                            </rbfwebui:localize></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr valign="top">
                        <td class="Subhead" nowrap="nowrap" width="100">
                            <rbfwebui:localize id="Literal3" runat="server" text="Title" textkey="MILESTONES_TITLE">
                            </rbfwebui:localize>:
                        </td>
                        <td rowspan="5">
                            &nbsp;
                        </td>
                        <td>
                            <asp:textbox id="TitleField" runat="server" columns="30" cssclass="NormalTextBox"
                                maxlength="100" width="390"></asp:textbox></td>
                        <td rowspan="5" width="1">
                            &nbsp;
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="Req1" runat="server" controltovalidate="TitleField"
                                display="Dynamic" errormessage="You Must Enter a Valid Title" textkey="ERROR_VALID_MILESTONE_TITLE"></asp:requiredfieldvalidator></td>
                    </tr>
                    <tr valign="top">
                        <td class="SubHead" nowrap="nowrap">
                            <rbfwebui:localize id="Literal2" runat="server" text="Estimated Completion Date"
                                textkey="MILESTONES_COMPLETION_DATE">
                            </rbfwebui:localize>:</td>
                        <td width="25">
                            <p>
                                <asp:textbox id="EstCompleteDate" runat="server" columns="8" cssclass="NormalTextBox"
                                    width="100"></asp:textbox>
                            </p>
                        </td>
                        <td class="Normal" width="250">
                            <asp:requiredfieldvalidator id="Req2" runat="server" controltovalidate="EstCompleteDate"
                                display="Dynamic" errormessage="Enter The Estimated Completion Date" textkey="ERROR_COMPLETION_DATE"></asp:requiredfieldvalidator><asp:comparevalidator
                                    id="VerifyCompleteDate" runat="server" controltovalidate="EstCompleteDate" display="Dynamic"
                                    errormessage="You must enter a valid date." operator="DataTypeCheck" textkey="ERROR_VALID_COMPLETION_DATE"
                                    type="Date"></asp:comparevalidator></td>
                    </tr>
                    <tr valign="top">
                        <td class="Subhead" nowrap="nowrap" valign="top" width="100">
                            <rbfwebui:localize id="Literal4" runat="server" text="Status of Milestone" textkey="MILESTONES_STATUS">
                            </rbfwebui:localize>:</td>
                        <td>
                            <asp:textbox id="StatusBox" runat="server" columns="44" cssclass="NormalTextBox"
                                height="101px" rows="6" textmode="MultiLine" width="387px"></asp:textbox></td>
                        <td rowspan="5">
                            <asp:requiredfieldvalidator id="Req3" runat="server" controltovalidate="StatusBox"
                                cssclass="Error" display="Dynamic" errormessage="Enter The Status of this Milestone"
                                textkey="ERROR_VALID_MILESTONE_STATUS"></asp:requiredfieldvalidator></td>
                        <td class="Normal">
                        </td>
                    </tr>
                </table>
                <p>
                </p>
                <rbfwebui:linkbutton id="updateButton" runat="server" cssclass="CommandButton" text="Update"></rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="cancelButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="Cancel"></rbfwebui:linkbutton>
                &nbsp;
                <rbfwebui:linkbutton id="deleteButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                    text="Delete this Item"></rbfwebui:linkbutton>
                <hr noshade="noshade" size="1" width="520" />
                <span class="Normal">
                    <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>
                    <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                    </rbfwebui:localize>&nbsp;
                    <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                    <br/>
                </span>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
