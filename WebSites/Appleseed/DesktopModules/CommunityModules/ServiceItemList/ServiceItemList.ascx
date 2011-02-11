<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ServiceItemList"
    language="c#" Codebehind="ServiceItemList.ascx.cs" %>
<asp:gridview id="DataGrid1" runat="server" allowpaging="True" allowsorting="False"
    alternatingrowstyle-cssclass="Grid_AlternatingItem" autogeneratecolumns="true"
    cellpadding="3" enablesortingandpagingcallbacks="True" headerstyle-cssclass="Grid_Header"
    pagesize="5" rowstyle-cssclass="Grid_Item" width="100%">
    <rowstyle cssclass="Grid_Item" />
    <headerstyle cssclass="Grid_Header" />
    <alternatingrowstyle cssclass="Grid_AlternatingItem" />
</asp:gridview>
<div class='error'>
    <asp:label id="lblStatus" runat="server"></asp:label></div>
