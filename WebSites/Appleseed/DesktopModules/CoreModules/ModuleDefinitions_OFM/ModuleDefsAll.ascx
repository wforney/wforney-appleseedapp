<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.ModuleDefsAll_OFM"
    language="c#" Codebehind="ModuleDefsAll.ascx.cs" %>
<asp:datalist id="defsList" runat="server" datakeyfield="GeneralModDefID">
    <itemtemplate>
        <span nowrap="">&nbsp;<rbfwebui:imagebutton id="ImageButton1" runat="server" alternatetext="Edit this item"
            imageurl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
            textkey="EDIT_THIS_ITEM" />
            &nbsp;<rbfwebui:label id="Label1" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "FriendlyName") %>'>
            </rbfwebui:label>
            &nbsp;&nbsp;&nbsp;File:
            <%# DataBinder.Eval(Container.DataItem, "DesktopSrc") %>
        </span>
    </itemtemplate>
</asp:datalist>