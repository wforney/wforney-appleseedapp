<%@ Control Inherits="Appleseed.Content.Web.Modules.DiscussionEnhanced.Discussion" Language="c#" CodeBehind="Discussion.ascx.cs" %>
<%-- list of threads, URL must have a Parent="parent from database" --%>
<asp:DataList ID="TopLevelList" runat="server" DataKeyField="ItemID" EnableViewState="false"
    ItemStyle-CssClass="Normal" OnItemDataBound="OnItemDataBound" Width="98%">
    <HeaderTemplate>
        <tr>
            <td width="50%">
                <rbfwebui:Label ID="Label14" runat="server" CssClass="NormalBold" TextKey="DISCUSION_TITLE">Titulo</rbfwebui:Label>
            </td>
            <td align="right" width="25%">
                &nbsp;
                <rbfwebui:Label ID="Label13" runat="server" CssClass="NormalBold" TextKey="DISCUSION_AUTHOR">Autor</rbfwebui:Label>
            </td>
            <td align="right" width="25%">
                <rbfwebui:Label ID="Label12" runat="server" CssClass="NormalBold" TextKey="DISCUSSION_LAST_UPDATE">Ultima Actualizacion</rbfwebui:Label>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <hr>
            </td>
        </tr>
    </HeaderTemplate>
    <SelectedItemTemplate>
        <tr bgcolor="WhiteSmoke">
            <td colspan="3">
                <rbfwebui:ImageButton ID="btnCollapse" runat="server" CommandName="CollapseThread"
                    ImageUrl='<%# this.GetLocalImage("minus.gif") %>' />
                <rbfwebui:Label ID="Label1" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "ChildCount") %>'
                    ToolTip="Number of replys to this topic">
                </rbfwebui:Label>/
                <rbfwebui:Label ID="Label2" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "ViewCount") %>'
                    ToolTip="Number of times this topic has been viewed">
                </rbfwebui:Label>&nbsp;
                <rbfwebui:LinkButton ID="LinkButton2" runat="server" CommandName="CollapseThread"
                    CssClass="ItemTitle" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                </rbfwebui:LinkButton><%-- add the property 'Target="_new"' to the following hyperlink to have edits occur up in a new browser window --%>
                <rbfwebui:HyperLink ID="HyperLink2" runat="server" ImageUrl="<%# GetReplyImage() %>"
                    NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                    Text="Reply to this message_" TextKey="DS_REPLYTHISMSG" Visible="True">
                </rbfwebui:HyperLink>
                <rbfwebui:HyperLink ID="HyperLink1" runat="server" ImageUrl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                    NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                    Text="Edit this message" TextKey="EDIT" Visible="True">
                </rbfwebui:HyperLink>
                <rbfwebui:ImageButton ID="deleteBtn" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    CommandName="delete" ImageUrl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                    TextKey="DELETE_THIS_ITEM" />
                <rbfwebui:Label ID="Label11" runat="server" CssClass="Normal" Text="Posted by" TextKey="POSTED_BY"></rbfwebui:Label>
                <rbfwebui:Label ID="Label10" runat="server" CssClass="NormalBI" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                </rbfwebui:Label>,
                <rbfwebui:Label ID="Label9" runat="server" CssClass="Normal" Text="posted on" TextKey="POSTED_DATE"></rbfwebui:Label>
                <rbfwebui:Label ID="Label8" runat="server" CssClass="NormalBI" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                </rbfwebui:Label><br>
                <rbfwebui:Label ID="Label7" runat="server" CssClass="NormalBold" Text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'>
                </rbfwebui:Label>
                <p>
                </p>
                <%-- expanded responses to main thread --%>
                <asp:DataList ID="DetailList" runat="server" DataSource="<%# GetThreadMessages() %>"
                    OnItemCommand="TopLevelListOrDetailListSelect" OnItemDataBound="OnItemDataBound">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteStart") %>
                        <rbfwebui:Label ID="Label3" runat="server" CssClass="ItemTitle" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                        </rbfwebui:Label>
                        <rbfwebui:HyperLink ID="Hyperlink1" runat="server" ImageUrl='<%# GetReplyImage() %>'
                            NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                            Text='Reply to this message_' TextKey="DS_REPLYTHISMSG" Visible="True">
                        </rbfwebui:HyperLink>
                        <rbfwebui:HyperLink ID="Hyperlink2" runat="server" ImageUrl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                            NavigateUrl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                            Text="Edit this message" TextKey="EDIT" Visible="True">
                        </rbfwebui:HyperLink>
                        <rbfwebui:ImageButton ID="deleteBtnExpanded" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                            CommandName="delete" ImageUrl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                            TextKey="DELETE_THIS_ITEM" />
                        <rbfwebui:Label ID="Label4" runat="server" CssClass="Normal" Text="Posted by" TextKey="POSTED_BY">
                        </rbfwebui:Label>
                        <rbfwebui:Label ID="Label5" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                        </rbfwebui:Label>
                        ,
                        <rbfwebui:Label ID="Label6" runat="server" CssClass="Normal" Text="posted on" TextKey="POSTED_DATE">
                        </rbfwebui:Label>
                        <rbfwebui:Label ID="Label15" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                        </rbfwebui:Label>
                        <br>
                        <rbfwebui:Label ID="Label16" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'>
                        </rbfwebui:Label>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteEnd") %>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
    </SelectedItemTemplate>
    <ItemStyle CssClass="Normal" />
    <ItemTemplate>
        <tr>
            <td width="60%">
                <rbfwebui:ImageButton ID="btnSelect" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    CommandName="ExpandThread" ImageUrl='<%# NodeImage((int)DataBinder.Eval(Container.DataItem, "ChildCount")) %>'
                    ToolTip="Expand the thread of this topic inside this browser page" />
                <rbfwebui:ImageButton ID="btnNewWindow" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    CommandName="ShowThreadNewWindow" ImageUrl='<%# this.GetLocalImage("new_window.gif") %>'
                    ToolTip="Open the thread of this topic in a new browser page" />
                <rbfwebui:Label ID="Label6" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "ChildCount") %>'
                    ToolTip="Number of replys to this topic">
                </rbfwebui:Label>/
                <rbfwebui:Label ID="Label5" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem, "ViewCount") %>'
                    ToolTip="Number of times this topic has been viewed">
                </rbfwebui:Label>&nbsp;
                <rbfwebui:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    CommandName="ExpandThread" CssClass="ItemTitle" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                </rbfwebui:LinkButton>
            </td>
            <td align="right" width="20%">
                <rbfwebui:Label ID="Label4" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'
                    ToolTip="Author of this post">
                </rbfwebui:Label>&nbsp;
            </td>
            <td align="right" width="20%">
                <rbfwebui:Label ID="Label3" runat="server" CssClass="Normal" Text='<%# DataBinder.Eval(Container.DataItem,"DateofLastReply", "{0:g}") %>'
                    ToolTip="Date of most recent reply">
                </rbfwebui:Label>
            </td>
        </tr>
    </ItemTemplate>
</asp:DataList>
