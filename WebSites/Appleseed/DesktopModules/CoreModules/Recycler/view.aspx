<%@ page autoeventwireup="false" inherits="Appleseed.recyclerViewPage"
    language="c#" Codebehind="view.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="htmlHead" runat="server"><title></title>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:panel id="pnlMain" runat="server" visible="True">
            <table cellpadding="0" cellspacing="0" class="PrintPage" width="100%">
                <tr>
                    <td>
                        <asp:placeholder id="PrintPlaceHolder" runat="server"></asp:placeholder>
                    </td>
                </tr>
            </table>
            <rbfwebui:linkbutton id="UpdateButton" runat="server" cssclass="CommandButton" text="Update"></rbfwebui:linkbutton>
            <asp:dropdownlist id="ddTabs" runat="server" autopostback="True" datasource="<%# portalTabs %>"
                datatextfield="PageName" datavaluefield="PageID" onselectedindexchanged="ddTabs_SelectedIndexChanged">
            </asp:dropdownlist>&nbsp;
            <rbfwebui:linkbutton id="restoreButton" runat="server" cssclass="CommandButton" onclick="restoreButton_Click"
                text="Restore"></rbfwebui:linkbutton>&nbsp;
            <rbfwebui:linkbutton id="CancelButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                text="Cancel"></rbfwebui:linkbutton>&nbsp;
            <rbfwebui:linkbutton id="DeleteButton" runat="server" causesvalidation="False" cssclass="CommandButton"
                text="Delete this item"></rbfwebui:linkbutton>
            <hr noshade="noshade" size="1" width="500" />
        </asp:panel>
        <asp:panel id="pnlError" runat="server" visible="False">
            <rbfwebui:label id="Label1" runat="server" cssclass="Head" textkey="ERROR_403">Access / Edit rights have been denied</rbfwebui:label>
        </asp:panel>
    </form>
</body>
</html>
