<%@ Control AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.Milestones"
    Language="c#" CodeBehind="Milestones.ascx.cs" %>
<asp:DataGrid ID="myDataGrid" runat="server" AutoGenerateColumns="false" BorderWidth="0"
    CellPadding="5" EnableViewState="true" HeaderStyle-CssClass="Normal" HeaderStyle-Font-Bold="true"
    ItemStyle-CssClass="Normal" Width="100%">
    <Columns>
        <rbfwebui:TemplateColumn>
            <ItemTemplate>
                <rbfwebui:HyperLink ID="editLink" TextKey="EDIT" Text="Edit" ImageUrl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/MileStones/MilestonesEdit.aspx", "ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&Mid=" + ModuleID) %>'
                    Visible="<%# IsEditable %>" runat="server" />
            </ItemTemplate>
        </rbfwebui:TemplateColumn>
        <rbfwebui:BoundColumn runat="server" DataField="Title" HeaderText="Title" TextKey="MILESTONE_TITLE">
        </rbfwebui:BoundColumn>
        <rbfwebui:BoundColumn runat="server" DataField="EstCompleteDate" DataFormatString="{0:d}"
            HeaderText="Compl. Date" TextKey="MILESTONE_COMPL_DATE">
        </rbfwebui:BoundColumn>
        <rbfwebui:BoundColumn runat="server" DataField="Status" HeaderText="Status" TextKey="MILESTONE_STATUS">
        </rbfwebui:BoundColumn>
    </Columns>
</asp:DataGrid>