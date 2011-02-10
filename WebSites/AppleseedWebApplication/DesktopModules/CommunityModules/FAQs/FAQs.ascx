<%@ Control AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.FAQs"
    Language="c#" CodeBehind="FAQs.ascx.cs" %>
<%@ Register Assembly="Appleseed.Framework" Namespace="Appleseed.Framework.Web.UI.WebControls"
    TagPrefix="rbfwebui" %>
<asp:DataList ID="myDataList" runat="server">
    <SelectedItemStyle />
    <SelectedItemTemplate>
        <rbfwebui:HyperLink ID="HyperlinkSelected" runat="server" ImageUrl='<%# this.CurrentTheme.GetModuleImageSRC("Edit.gif") %>'
            NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/FAQs/FAQsEdit.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mID=" + ModuleID) %>'
            Text="Edit" TextKey="EDIT" Visible="<%# IsEditable %>">
        </rbfwebui:HyperLink>
        <span class="normalBold">
            <rbfwebui:Localize ID="Literal1" runat="server" Text="Q" TextKey="FAQ_Q">
            </rbfwebui:Localize>:&nbsp;</span>
        <rbfwebui:LinkButton ID="LinkbuttonSelected" runat="server" CommandName="select"
            Text='<%# DataBinder.Eval(Container.DataItem, "Question") %>' title='<%# DataBinder.Eval(Container.DataItem, "CreatedDate") %>'>
        </rbfwebui:LinkButton><br />
        <span class="normalBold">
            <rbfwebui:Localize ID="Literal2" runat="server" Text="A" TextKey="FAQ_A">
            </rbfwebui:Localize>:&nbsp; </span>
        <rbfwebui:Label ID="LabelSelected" runat="server" CssClass="normal" Text='<%# DataBinder.Eval(Container.DataItem, "Answer") %>'>:&nbsp;</span>

        </rbfwebui:Label>
    </SelectedItemTemplate>
    <ItemTemplate>
        <rbfwebui:HyperLink ID="HyperlinkItem" runat="server" ImageUrl='<%# this.CurrentTheme.GetModuleImageSRC("Edit.gif")  %>'
            NavigateUrl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/FAQs/FAQsEdit.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mID=" + ModuleID)%>'
            Text="Edit" TextKey="EDIT" Visible="<%# IsEditable %>">
        </rbfwebui:HyperLink>
        <span class="normalBold">
            <rbfwebui:Localize ID="Literal3" runat="server" Text="Q" TextKey="FAQ_Q">
            </rbfwebui:Localize>:&nbsp;</span>
        <rbfwebui:LinkButton ID="LinkbuttonItem" runat="server" CommandName="select" Text='<%# DataBinder.Eval(Container.DataItem, "Question") %>'
            title='<%# DataBinder.Eval(Container.DataItem, "CreatedDate") %>'>
        </rbfwebui:LinkButton>
    </ItemTemplate>
</asp:DataList>
