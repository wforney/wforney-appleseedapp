<%@ Control AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.Documents"
    Language="c#" Codebehind="Documents.ascx.cs" %>
<asp:DataGrid ID="myDataGrid" runat="server" AllowSorting="True" AutoGenerateColumns="false"
    BorderWidth="0px" CellPadding="3" EnableViewState="false">
    <Columns>
        <rbfwebui:TemplateColumn>
            <ItemTemplate>
                <rbfwebui:HyperLink TextKey="EDIT" Text="Edit" ID="editLink" ImageUrl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Documents/DocumentsEdit.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID")  + "&mid=" + ModuleID) %>'
                    runat="server" />
            </ItemTemplate>
        </rbfwebui:TemplateColumn>
        <rbfwebui:TemplateColumn>
            <ItemTemplate>
                <rbfwebui:HyperLink TextKey="CONTENTTYPE" Text="Content Type" ID="contentType" ImageUrl='<%# "~/aspnet_client/Ext/" + DataBinder.Eval(Container.DataItem,"contentType")%>'
                    NavigateUrl='<%# GetBrowsePath(DataBinder.Eval(Container.DataItem,"FileNameUrl").ToString(), DataBinder.Eval(Container.DataItem,"ContentSize"), (int) DataBinder.Eval(Container.DataItem,"ItemID")) %>'
                    Target="_new" runat="server" />
            </ItemTemplate>
        </rbfwebui:TemplateColumn>
        <rbfwebui:TemplateColumn HeaderStyle-CssClass="NormalBold" HeaderText="Title" SortExpression="FileFriendlyName">
            <ItemTemplate>
                <rbfwebui:HyperLink ID="docLink" Text='<%# DataBinder.Eval(Container.DataItem,"FileFriendlyName") %>'
                    NavigateUrl='<%# GetBrowsePath(DataBinder.Eval(Container.DataItem,"FileNameUrl").ToString(), DataBinder.Eval(Container.DataItem,"ContentSize"), (int) DataBinder.Eval(Container.DataItem,"ItemID")) %>'
                    CssClass="Normal" Target="_new" runat="server" />
            </ItemTemplate>
        </rbfwebui:TemplateColumn>
        <rbfwebui:BoundColumn DataField="CreatedByUser" HeaderStyle-CssClass="NormalBold"
            HeaderText="<%$ Resources:Appleseed, DOCUMENT_OWNER %>" ItemStyle-CssClass="Normal"
            SortExpression="CreatedByUser">
        </rbfwebui:BoundColumn>
        <rbfwebui:BoundColumn DataField="Category" HeaderStyle-CssClass="NormalBold" HeaderText="<%$ Resources:Appleseed, DOCUMENT_AREA %>"
            ItemStyle-CssClass="Normal" ItemStyle-Wrap="false" SortExpression="Category">
        </rbfwebui:BoundColumn>
        <rbfwebui:BoundColumn DataField="CreatedDate" DataFormatString="{0:d}" HeaderStyle-CssClass="NormalBold"
            HeaderText="<%$ Resources:Appleseed, DOCUMENT_LAST_UPDATED %>" ItemStyle-CssClass="Normal"
            SortExpression="CreatedDate">
        </rbfwebui:BoundColumn>
    </Columns>
</asp:DataGrid>