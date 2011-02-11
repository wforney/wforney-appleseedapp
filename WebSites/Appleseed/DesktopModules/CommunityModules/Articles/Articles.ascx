
<%@ Control Language="c#" inherits="Appleseed.Content.Web.Modules.Articles" autoeventwireup="false" %>
<asp:datalist id="myDataList" runat="server" CellPadding="4" Width="100%">
	<ItemTemplate>
		<div class="Normal">
			<div>
				<rbfwebui:HyperLink TextKey="EDIT" Text="Edit" id="editLink" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>' NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>' Visible="<%# IsEditable %>" runat="server" />
				<rbfwebui:label id=StartDate Visible="<%# ShowDate %>" runat="server" CssClass="ItemDate" Text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:d}") %>'/>
				<rbfwebui:label id=Separator Visible="<%# ShowDate %>" runat="server">&#160;-&#160;</rbfwebui:label>
				<rbfwebui:HyperLink id=Title runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Title") %>' Visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0) %>' NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesView.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mid=" + ModuleID + "&wversion=" + Version)%>'>
				</rbfwebui:HyperLink>&#160;
				<rbfwebui:label id=SubTitle Text='<%# "(" + DataBinder.Eval(Container.DataItem,"SubTitle") + ")" %>' Visible='<%# ((string)DataBinder.Eval(Container.DataItem,"Subtitle")).Length > 0 %>' runat="server"/>&#160;
				<rbfwebui:label id="Expired" Visible='<%# DataBinder.Eval(Container.DataItem,"Expired").ToString() == "1"  %>' runat="server" CssClass="Error" Text='Expired'/>
			</div>
			<div class="NormalItalic" style="MARGIN-TOP: 6px; margin-botton: 6px"><%# DataBinder.Eval(Container.DataItem,"Abstract").ToString().Replace("\n","<br>") %></div>
		</div>
	</ItemTemplate>
</asp:datalist>
