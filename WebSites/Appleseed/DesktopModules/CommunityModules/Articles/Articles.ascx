<%@ Control Language="c#" Inherits="Appleseed.Content.Web.Modules.Articles" %>
<asp:DataList ID="MyDataList" runat="server" CellPadding="4" Width="100%">
    <ItemTemplate>
        <div class="Normal">
            <div>
                <rbfwebui:HyperLink TextKey="EDIT" Text="Edit" id="editLink" ImageUrl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    NavigateUrl='<%# HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx",PageID,string.Format("ItemID={0}&amp;mid={1}", DataBinder.Eval(Container.DataItem,"ItemID"), this.ModuleID) )%>'
                    Visible="<%# IsEditable %>" runat="server" />
                <rbfwebui:label id="StartDate" Visible="<%# ShowDate %>" runat="server" CssClass="ItemDate"
                    Text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:d}") %>' />
                <rbfwebui:label id="Separator" Visible="<%# ShowDate %>" runat="server">&#160;-&#160;</rbfwebui:label>
                <rbfwebui:HyperLink id="Title" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Title") %>'
                    Visible='<%# DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0 %>'
                    NavigateUrl='<%# HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesView.aspx",string.Format("ItemID={0}&mid={1}&wversion={2}", DataBinder.Eval(Container.DataItem,"ItemID"), this.ModuleID, this.Version))%>'>
                </rbfwebui:HyperLink>&#160;
                <rbfwebui:label id="SubTitle" Text='<%# "(" + DataBinder.Eval(Container.DataItem,"SubTitle") + ")" %>'
                    Visible='<%# ((string)DataBinder.Eval(Container.DataItem,"Subtitle")).Length > 0 %>'
                    runat="server" />&#160;
                <rbfwebui:label id="Expired" Visible='<%# DataBinder.Eval(Container.DataItem,"Expired").ToString() == "1"  %>'
                    runat="server" CssClass="Error" Text='Expired' />
            </div>
            <div class="NormalItalic" style="margin-top: 6px; margin-botton: 6px">
                <%# DataBinder.Eval(Container.DataItem,"Abstract").ToString().Replace("\n","<br />") %></div>
        </div>
    </ItemTemplate>
</asp:DataList>
