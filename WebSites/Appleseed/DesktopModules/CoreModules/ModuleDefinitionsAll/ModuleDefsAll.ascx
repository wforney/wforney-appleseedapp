<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ModuleDefsAll"
    language="c#" Codebehind="ModuleDefsAll.ascx.cs" %>
<asp:datalist id="defsList" runat="server" datakeyfield="GeneralModDefID">
    <itemtemplate>
        &#160;
        <rbfwebui:imagebutton id="ImageButton1" runat="server" alternatetext="Edit this item"
            imageurl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
            textkey="EDIT_THIS_ITEM" />&#160;
        <rbfwebui:label id="Label1" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "FriendlyName") %>'>
        </rbfwebui:label>
    </itemtemplate>
</asp:datalist>