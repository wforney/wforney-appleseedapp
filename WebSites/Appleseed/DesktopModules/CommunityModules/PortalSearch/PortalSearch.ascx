<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.PortalSearch"
    language="c#" Codebehind="PortalSearch.ascx.cs" %>
<p>
    <asp:datagrid id="DataGrid1" runat="server" alternatingitemstyle-cssclass="Grid_AlternatingItem"
        autogeneratecolumns="False" cellpadding="3" headerstyle-cssclass="Grid_Header"
        itemstyle-cssclass="Grid_Item" width="100%">
    </asp:datagrid>
</p>
<p>
    <asp:textbox id="txtSearchString" runat="server" cssclass="NormalTextBox" width="150px"></asp:textbox>
    <rbfwebui:button id="btnSearch" runat="server" text="Search" textkey="PORTALSEARCH_SEARCH" />
    <rbfwebui:label id="lblHits" runat="server" font-names="Tahoma" width="50px"></rbfwebui:label>
    &nbsp;<rbfwebui:localize id="lblModule" runat="server" text="Module" textkey="PORTALSEARCH_MODULE"></rbfwebui:localize>&nbsp;
    <asp:dropdownlist id="ddSearchModule" runat="server" cssclass="NormalTextBox">
    </asp:dropdownlist>
    <rbfwebui:localize id="lblTopic" runat="server" text="Topic" textkey="PORTALSEARCH_TOPIC">
    </rbfwebui:localize>
    <asp:dropdownlist id="ddTopics" runat="server" cssclass="NormalTextBox">
    </asp:dropdownlist>
    &nbsp;<rbfwebui:localize id="lblField" runat="server" text="Field" textkey="PORTALSEARCH_FIELD"></rbfwebui:localize>&nbsp;
    <asp:dropdownlist id="ddSearchField" runat="server" cssclass="NormalTextBox">
    </asp:dropdownlist></p>
<rbfwebui:label id="lblError" runat="server" visible="false"></rbfwebui:label>
