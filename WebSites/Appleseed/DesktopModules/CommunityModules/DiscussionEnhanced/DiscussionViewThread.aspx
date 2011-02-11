<%@ Page AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.DiscussionEnhanced.DiscussionViewThread"
    Language="c#" CodeBehind="DiscussionViewThread.aspx.cs" %>

<%@ Register Src="~/Design/DesktopLayouts/DesktopFooter.ascx" TagName="Footer" TagPrefix="foot" %>
<%@ Register Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" TagName="Banner"
    TagPrefix="portal" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" name="form1">
    <%-- needed to get site header to render correctly --%>
    <div class="rb_DefaultLayoutDiv">
        <table class="rb_DefaultLayoutTable">
            <tr valign="top">
                <td class="rb_DefaultPortalHeader" colspan="2" valign="top">
                    <portal:Banner ID="Banner1" runat="server" showtabs="false" />
                </td>
            </tr>
            <tr valign="top">
                <td width="5%">
                    &nbsp;
                </td>
                <td width="95%">
                    <br />
                    <%-- DataList of all the threads in this topic --%>
                    <asp:DataList ID="ThreadList" runat="server" DataKeyField="ItemID" EnableViewState="true"
                        ItemStyle-CssClass="Normal" OnItemCommand="ThreadList_Select" OnItemDataBound="OnItemDataBound">
                        <HeaderTemplate>
                            <rbfwebui:Label ID="Label1" runat="server" CssClass="ItemTitle" Text="Displaying all threads for this topic"
                                TextKey="DS_DISPLAY_THREADS">
                            </rbfwebui:Label>
                            <hr>
                            <br>
                        </HeaderTemplate>
                        <FooterTemplate>
                            <hr>
                            <rbfwebui:LinkButton ID="LinkButton1" runat="server" CommandName="return_to_discussion_list"
                                CssClass="Normal" Text="Return to list of threads" TextKey="DS_RETURN_LIST">
                            </rbfwebui:LinkButton>
                            <!-- Known bug:--this link doesn't work corectly if you edit an item on this page.-->
                            <br>
                            <br>
                        </FooterTemplate>
                        <ItemStyle CssClass="Normal" />
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "BlockQuoteStart") %>
                            <rbfwebui:Label ID="Label4" runat="server" CssClass="ItemTitle" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'></rbfwebui:Label>
                            <rbfwebui:HyperLink ID="HyperLink2" runat="server" ImageUrl="<%# GetReplyImage() %>"
                                NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                                Text="Reply to this message_" TextKey="DS_REPLYTHISMSG" Visible="True"></rbfwebui:HyperLink>
                            <rbfwebui:HyperLink ID="HyperLink1" runat="server" ImageUrl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                                NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                                Text="Edit this message" TextKey="EDIT" Visible="True"></rbfwebui:HyperLink>
                            <rbfwebui:ImageButton ID="deleteBtn" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                                CommandName="delete" ImageUrl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                                TextKey="DELETE_THIS_ITEM" />
                            <asp:Panel runat="server" CssClass="NormalDim">
                            </asp:Panel>
                            <rbfwebui:Label ID="Label11" runat="server" CssClass="Normal" Text="Posted by" TextKey="POSTED_BY"></rbfwebui:Label>
                            <rbfwebui:Label ID="Label10" runat="server" CssClass="NormalDim" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                            </rbfwebui:Label>,
                            <rbfwebui:Label ID="Label9" runat="server" CssClass="Normal" Text="posted on" TextKey="POSTED_DATE"></rbfwebui:Label>
                            <rbfwebui:Label ID="Label8" runat="server" CssClass="NormalDim" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                            </rbfwebui:Label><br>
                            <rbfwebui:Label ID="Label3" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'></rbfwebui:Label><%# DataBinder.Eval(Container.DataItem, "BlockQuoteEnd") %>
                        </ItemTemplate>
                    </asp:DataList>
                </td>
            </tr>
            <tr>
                <td class="rb_DefaultPortalFooter" colspan="2">
                    <foot:Footer ID="Footer" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
