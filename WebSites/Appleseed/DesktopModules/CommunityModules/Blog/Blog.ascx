<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Blog"
    language="c#" Codebehind="Blog.ascx.cs" %>
<table>
    <tr>
        <td>
            <div class="div_blog_messeges">
                <asp:datalist id="myDataList" runat="server" cellpadding="4" enableviewstate="False"
                    width="100%">
                    <itemtemplate>
                        <div class="BlogTitle">
                            <rbfwebui:hyperlink id="editLink" runat="server" imageurl='<%# this.CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                                navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogEdit.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                text="Edit" textkey="EDIT" visible="<%# IsEditable %>">
                            </rbfwebui:hyperlink>
                            <rbfwebui:hyperlink id="Title" runat="server" cssclass="BlogTitle" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                text='<%# DataBinder.Eval(Container.DataItem,"Title") %>' visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0) %>'>
                            </rbfwebui:hyperlink>&#160;
                        </div>
                        <br />
                        <rbfwebui:label id="Description" runat="server" cssclass="Normal">
							<%# Server.HtmlDecode((String) DataBinder.Eval(Container.DataItem,"Description")) %>
                        </rbfwebui:label>
                        <br />
                        <br />
                        <div class="BlogFooter">
                            <rbfwebui:hyperlink id="Hyperlink1" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:dddd MMMM d yyyy hh:mm tt}") %>'
                                visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0) %>'>
                            </rbfwebui:hyperlink>&#160;|
                            <rbfwebui:hyperlink id="Hyperlink2" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                text='<%# Feedback + DataBinder.Eval(Container.DataItem,"CommentCount") + ")" %>'
                                visible='True'>
                            </rbfwebui:hyperlink>&#160;
                        </div>
                        <hr />
                    </itemtemplate>
                </asp:datalist>
            </div>
            <div class="div_blog_stats">
                <b>
                    <rbfwebui:localize id="SyndicationLabel" runat="server" text="Syndication" textkey="BLOG_SYNDICATION">
                    </rbfwebui:localize></b><br />
                <a id="lnkRSS" runat="server" href="/DesktopModules/Blog/rss.aspx">
                    <img alt="imgRSS" id="imgRSS" runat="server" border="0" src="/Appleseed/DesktopModules/Blog/xml.gif" /></a>
                <br />
                <br />
                <b>
                    <rbfwebui:localize id="StatisticsLabel" runat="server" text="Statistics" textkey="BLOG_STATISTICS">
                    </rbfwebui:localize></b><br />
                <rbfwebui:label id="lblEntryCount" runat="server"></rbfwebui:label><br />
                <rbfwebui:label id="lblCommentCount" runat="server"></rbfwebui:label><br />
                <br />
                <b>
                    <rbfwebui:localize id="ArchivesLabel" runat="server" text="Archives" textkey="BLOG_ARCHIVES">
                    </rbfwebui:localize></b><br />
                <asp:repeater id="dlArchive" runat="server" enableviewstate="False">
                    <headertemplate>
                        <ul>
                        </ul>
                    </headertemplate>
                    <itemtemplate>
                        <li>
                            <rbfwebui:hyperlink id="Hyperlink4" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/ArchiveView.aspx",PageID,
									"&month=" + DataBinder.Eval(Container.DataItem,"Month") 
									+ "&year=" + DataBinder.Eval(Container.DataItem,"Year")
									+ "&mid=" + ModuleID )%>' text='<%# DataBinder.Eval(Container.DataItem,"MonthName") 
																+ ", " +  DataBinder.Eval(Container.DataItem,"Year")
																+ " (" + DataBinder.Eval(Container.DataItem,"Count") + ")"%>'
                                visible='True'>
                            </rbfwebui:hyperlink>&nbsp;</li>
                    </itemtemplate>
                    <footertemplate>
                        </ul>
                    </footertemplate>
                </asp:repeater>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="copyright">
                <rbfwebui:label id="lblCopyright" runat="server" cssclass="Normal"></rbfwebui:label>
            </div>
        </td>
    </tr>
</table>
