<%@ Control AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.Tasks"
    Language="c#" CodeBehind="Tasks.ascx.cs" %>
<asp:GridView ID="myDataGrid" runat="server" AllowPaging="False" AllowSorting="True"
    AlternatingRowStyle-CssClass="Grid_AlternatingItem" AutoGenerateColumns="False"
    BorderWidth="0" HeaderStyle-CssClass="Grid_Header" RowStyle-CssClass="Grid_Item"
    Width="100%">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <rbfwebui:HyperLink ID="HyperLinkEdit" runat="server" ImageUrl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    NavigateUrl='<%# "~/DesktopModules/CommunityModules/Tasks/TasksEdit.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID %>'
                    Text="Edit" TextKey="EDIT" Visible="<%# IsEditable %>"></rbfwebui:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="DueDate" DataFormatString="{0:d}" HeaderText="<%$ Resources:Appleseed, TASK_DUEDATE %>"
            SortExpression="DueDate">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$ Resources:Appleseed, TASK_TITLE %>" SortExpression="Title">
            <HeaderStyle CssClass="NormalBold" />
            <ItemTemplate>
                <rbfwebui:HyperLink ID="HyperLinkView" runat="server" CssClass="Normal" NavigateUrl='<%# "~/DesktopModules/CommunityModules/Tasks/TasksView.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID %>'
                    Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'>
                </rbfwebui:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ Resources:Appleseed, TASK_STATUS %>" SortExpression="Status">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
            <ItemTemplate>
                <rbfwebui:Localize ID="Localize1" runat="server" Text='' TextKey='<%# "TASK_STATE_"+DataBinder.Eval(Container, "DataItem.Status") %>'>
                </rbfwebui:Localize>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<%$ Resources:Appleseed, TASK_PRIORITY %>" SortExpression="Priority">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
            <ItemTemplate>
                <rbfwebui:Localize ID="Localize2" runat="server" TextKey='<%# "TASK_PRIORITY_"+DataBinder.Eval(Container, "DataItem.Priority") %>'>
                </rbfwebui:Localize>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="AssignedTo" HeaderText="<%$ Resources:Appleseed, TASK_ASSIGNEDTO %>"
            SortExpression="AssignedTo">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="<%$ Resources:Appleseed, TASK_COMPLETION %>" SortExpression="PercentComplete">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
            <ItemTemplate>
                <table border="1" cellpadding="0" cellspacing="0" class="Normal" style="border: 1px solid black;"
                    width="100%">
                    <tr>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ModifiedDate" DataFormatString="{0:d}" HeaderText="<%$ Resources:Appleseed, DOCUMENT_LAST_UPDATED %>"
            SortExpression="ModifiedDate">
            <HeaderStyle CssClass="NormalBold" />
            <ItemStyle CssClass="Normal" />
        </asp:BoundField>
    </Columns>
</asp:GridView>
