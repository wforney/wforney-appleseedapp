<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register assembly="Appleseed.Framework" namespace="Appleseed.Framework.Web.UI.WebControls"
    tagprefix="rbfwebui" %>

<%@ page autoeventwireup="True" inherits="Appleseed.Content.Web.Modules.ArchiveView" language="c#"
    CodeBehind="ArchiveView.aspx.cs" %>

 <html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="ArchiveView" runat="server" method="post">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <div class="div_blog">
                    <div class="div_blog_stats">
                        <b>
                            <rbfwebui:localize id="SyndicationLabel" runat="server" text="Syndication" textkey="BLOG_SYNDICATION">
                            </rbfwebui:localize></b><br />
                        <a id="lnkRSS" runat="server" href="/DesktopModules/Blog/rss.aspx">
                            <img alt="imgRss" id="imgRSS" runat="server" border="0" src="/Appleseed/DesktopModules/Blog/xml.gif" /></a>
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
                                    <rbfwebui:HyperLink id="Hyperlink4" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/ArchiveView.aspx",PageID,
									"&month=" + DataBinder.Eval(Container.DataItem,"Month") 
									+ "&year=" + DataBinder.Eval(Container.DataItem,"Year")
									+ "&mid=" + ModuleID )%>' text='<%# DataBinder.Eval(Container.DataItem,"MonthName") 
																+ ", " +  DataBinder.Eval(Container.DataItem,"Year")
																+ " (" + DataBinder.Eval(Container.DataItem,"Count") + ")"%>'
                                        visible='True'>
                                    </rbfwebui:HyperLink>&nbsp;</li>
                            </itemtemplate>
                            <footertemplate>
                                </ul>
                            </footertemplate>
                        </asp:repeater>
                    </div>
                    <div class="div_blog_messeges">
                        <rbfwebui:label id="lblHeader" runat="server" cssclass="BlogTitle" text='' visible="True">
                        </rbfwebui:label>
                        <br />
                        <br />
                        <asp:datalist id="myDataList" runat="server" cellpadding="4" enableviewstate="False"
                            width="100%">
                            <headertemplate>
                                <div style="border-bottom: 4px dotted blue; padding: 2px;">
                                    <table border="0" class="normalbold">
                                        <tr>
                                            <td width="99%">
                                                Title</td>
                                            <td nowrap="nowrap" width="200">
                                                <rbfwebui:label id="Label1" runat="server" textkey="BLOG_POSTED">posted</rbfwebui:label></td>
                                            <td nowrap="nowrap" width="120">
                                                FeedBack</td>
                                        </tr>
                                    </table>
                                </div>
                            </headertemplate>
                            <itemtemplate>
                                <div class="div_blog_archive_row">
                                    <table border="0">
                                        <tr>
                                            <td width="99%">
                                                <rbfwebui:HyperLink id="Title" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                                    text='<%# DataBinder.Eval(Container.DataItem,"Title") %>' visible="True">
                                                </rbfwebui:HyperLink>
                                            </td>
                                            <td nowrap="nowrap" width="200">
                                                <rbfwebui:HyperLink id="Hyperlink1" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                                    text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:dddd MMMM d yyyy hh:mm tt}") %>'
                                                    visible="True">
                                                </rbfwebui:HyperLink></td>
                                            <td nowrap="nowrap" width="120">
                                                <rbfwebui:HyperLink id="Hyperlink2" runat="server" navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                                                    text='<%# Feedback + DataBinder.Eval(Container.DataItem,"CommentCount") + ")" %>'
                                                    visible="True">
                                                </rbfwebui:HyperLink></td>
                                        </tr>
                                    </table>
                                </div>
                            </itemtemplate>
                            <footertemplate>
                                </table>
                            </footertemplate>
                        </asp:datalist>
                    </div>
                    <div class="copyright">
                        <rbfwebui:label id="lblCopyright" runat="server" cssclass="Normal"></rbfwebui:label>
                    </div>
                </div>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
      
        </div>
    </form>
</body>
</html>
