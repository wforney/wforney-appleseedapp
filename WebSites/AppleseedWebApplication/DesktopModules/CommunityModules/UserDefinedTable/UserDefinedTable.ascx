<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.UserDefinedTable"
    language="c#" Codebehind="UserDefinedTable.ascx.cs" %>
<asp:placeholder id="PlaceHolderOutput" runat="server"></asp:placeholder>
<br />
<rbfwebui:linkbutton id="cmdManage" runat="server" cssclass="CommandButton" onclick="cmdManage_Click"
    text="Manage Table" textkey="USERTABLE_MANAGETABLE" visible="False"></rbfwebui:linkbutton>
