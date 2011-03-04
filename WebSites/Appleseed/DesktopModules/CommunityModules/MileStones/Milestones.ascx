<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Milestones"
    language="c#" Codebehind="Milestones.ascx.cs" %>
<asp:datagrid id="myDataGrid" runat="server" autogeneratecolumns="false" borderwidth="0"
    cellpadding="5" enableviewstate="true" headerstyle-cssclass="Normal" headerstyle-font-bold="true"
    itemstyle-cssclass="Normal" width="100%">
    <columns>
        <rbfwebui:templatecolumn>
            <itemtemplate>
                <rbfwebui:HyperLink ID="editLink" TextKey="EDIT" Text="Edit" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/MileStones/MilestonesEdit.aspx", "ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&Mid=" + ModuleID) %>'
                    Visible="<%# IsEditable %>" runat="server" />
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:boundcolumn runat="server" datafield="Title" headertext="Title" textkey="MILESTONE_TITLE">
        </rbfwebui:boundcolumn>
        <rbfwebui:boundcolumn runat="server" datafield="EstCompleteDate" dataformatstring="{0:d}"
            headertext="Compl. Date" textkey="MILESTONE_COMPL_DATE">
        </rbfwebui:boundcolumn>
        <rbfwebui:boundcolumn runat="server" datafield="Status" headertext="Status" textkey="MILESTONE_STATUS">
        </rbfwebui:boundcolumn>
    </columns>
</asp:datagrid>