<%@ Control Inherits="Appleseed.Content.Web.Modules.ArticlesInline" Language="c#"
    CodeBehind="ArticlesInline.ascx.cs" %>
<asp:DataList CellPadding="4" ID="MyDataList" runat="server" Width="100%">
    <ItemTemplate>
        <div class="Normal">
            <div>
                <rbfwebui:HyperLink id="editLink" runat="server" imageurl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    navigateurl='<%# HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Articles/ArticlesEdit.aspx",PageID,string.Format("ItemID={0}&amp;mid={1}", DataBinder.Eval(Container.DataItem,"ItemID"), this.ModuleID) )%>'
                    text="Edit" textkey="EDIT" visible="<%# IsEditable %>">
                </rbfwebui:HyperLink>
                <rbfwebui:Label id="StartDate" runat="server" cssclass="ItemDate" text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:d}") %>'
                    visible="<%# ShowDate %>">
                </rbfwebui:Label>
                <rbfwebui:Label id="Separator" runat="server" visible="<%# ShowDate %>">&#160;-&#160;</rbfwebui:Label>
                <rbfwebui:LinkButton id="Title" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem,"ItemID")%>'
                    commandname='View' cssclass="Head" text='<%# DataBinder.Eval(Container.DataItem,"Title") %>'
                    visible='<%# DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0 %>'>
                </rbfwebui:LinkButton>
                <br />
                <rbfwebui:Label id="SubTitle" runat="server" cssclass="SubHead" text='<%# DataBinder.Eval(Container.DataItem,"SubTitle") %>'
                    visible='<%# ((string)DataBinder.Eval(Container.DataItem,"Subtitle")).Length > 0 %>'>
                </rbfwebui:Label>&nbsp;
                <rbfwebui:Label id="Expired" runat="server" cssclass="Error" text='Expired' visible='<%# DataBinder.Eval(Container.DataItem,"Expired").ToString() == "1"  %>'>
                </rbfwebui:Label>
            </div>
            <div class="NormalItalic" style="margin: 6px 0px 6px 0px;">
                <%# DataBinder.Eval(Container.DataItem,"Abstract").ToString().Replace("\n","<br />") %>
            </div>
        </div>
    </ItemTemplate>
</asp:DataList>
<asp:Panel ID="ArticleDetail" runat="server" Visible="False">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td height="19">
                <rbfwebui:LinkButton id="goBackTop" runat="server" text="Back" textkey="BACK">
                </rbfwebui:LinkButton>
            </td>
            <td align="right" height="19">
                <rbfwebui:HyperLink id="editLinkDetail" runat="server" text="Edit" textkey="EDIT"
                    visible="<%# IsEditable %>">
                </rbfwebui:HyperLink>
            </td>
        </tr>
        <tr>
            <td class="Normal" colspan="2">
                <p>
                    <rbfwebui:localize id="TitleDetail" runat="server" cssclass="Head">
                    </rbfwebui:localize><br />
                    <rbfwebui:localize id="SubtitleDetail" runat="server" cssclass="SubHead">
                    </rbfwebui:localize><br />
                    <rbfwebui:localize id="StartDateDetail" runat="server" cssclass="ItemDate">
                    </rbfwebui:localize></p>
                <p>
                </p>
                <rbfwebui:localize id="Description" runat="server" cssclass="Normal">
                </rbfwebui:localize><p>
                </p>
                <hr noshade="noshade" size="1" />
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <rbfwebui:LinkButton id="goBackBottom" runat="server" text="Back" textkey="BACK">
                            </rbfwebui:LinkButton>
                        </td>
                        <td align="right" class="Normal">
                            <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                            </rbfwebui:localize>&nbsp;
                            <rbfwebui:localize id="CreatedBy" runat="server">
                            </rbfwebui:localize>&nbsp;
                            <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                            </rbfwebui:localize>&nbsp;
                            <rbfwebui:localize id="CreatedDate" runat="server">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
