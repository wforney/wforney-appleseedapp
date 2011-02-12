<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.BlogView"
    language="c#" Codebehind="BlogView.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="BlogViewform" runat="server" method="post">
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
                            <img id="imgRSS" runat="server" alt="xml" src="/Appleseed/DesktopModules/Blog/xml.gif"
                                style="border: none 0 #000" /></a>
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
																+ " (" + DataBinder.Eval(Container.DataItem,"Count") + ")" %>'
                                        visible='True'>
                                    </rbfwebui:hyperlink>&nbsp;</li>
                            </itemtemplate>
                            <footertemplate>
                                </ul>
                            </footertemplate>
                        </asp:repeater>
                    </div>
                    <div class="div_blog_messeges">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td colspan="2">
                                    <p>
                                        <rbfwebui:label id="Title" runat="server" cssclass="BlogTitle">Title</rbfwebui:label>&nbsp;</p>
                                    <p>
                                        <rbfwebui:label id="Description" runat="server" cssclass="Normal">Description</rbfwebui:label></p>
                                    <p>
                                        <rbfwebui:label id="StartDate" runat="server" cssclass="ItemDate">StartDate</rbfwebui:label></p>
                                    <hr noshade="noshade" size="1" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="div_blog_comments">
                        <b>
                            <rbfwebui:localize id="Literal6" runat="server" text="Feedback" textkey="BLOG_TITLE_FEEDBACK">
                            </rbfwebui:localize></b>
                        <asp:datalist id="dlComments" runat="server" cellpadding="4" enableviewstate="False"
                            onitemcommand="dlComments_ItemCommand" width="100%">
                            <itemtemplate>
                                <div>
                                    <div class="BlogTitle">
                                        <rbfwebui:imagebutton id="btnDelete" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem,"BlogCommentID")%>'
                                            commandname="DeleteComment" imageurl='<%# CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl %>'
                                            text="Delete" textkey="DELETE" visible="<% # IsDeleteable %>" />
                                        <%# DataBinder.Eval(Container.DataItem,"Title") %>
                                        &#160;
                                    </div>
                                    <br />
                                    <rbfwebui:label id="Label3" runat="server" cssclass="BlogCommentName" visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "URL").ToString().Length == 0) %>'>
												<%# Server.HtmlDecode((String) DataBinder.Eval(Container.DataItem,"Name")) %>
                                    </rbfwebui:label>
                                    <rbfwebui:hyperlink id="Hyperlink2" runat="server" cssclass="BlogCommentName" navigateurl='<%# DataBinder.Eval(Container.DataItem,"URL")%>'
                                        target="_blank" text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "URL").ToString().Length != 0) %>'>
                                    </rbfwebui:hyperlink>&#160;
                                    <br />
                                    <rbfwebui:label id="Label2" runat="server" cssclass="BlogItemDate" text='<%# DataBinder.Eval(Container.DataItem,"DateCreated", "{0:dddd MMMM d yyyy h:mm tt}") %>'
                                        visible="True">
                                    </rbfwebui:label>
                                    <br />
                                    <%# Server.HtmlDecode((String) DataBinder.Eval(Container.DataItem,"Comment")) %>
                                    <br />
                                    <br />
                                    <div class="BlogFooter">
                                    </div>
                                </div>
                                <hr />
                            </itemtemplate>
                        </asp:datalist>
                    </div>
                    <div class="div_blog_comment_form">
                        <!-- begin comments form -->
                        <table border="0">
                            <tr>
                                <td align="left">
                                    <rbfwebui:localize id="Literal4" runat="server" text="Title" textkey="BLOG_TITLE">
                                    </rbfwebui:localize>&nbsp;
                                </td>
                                <td>
                                    <asp:textbox id="txtTitle" runat="server" maxlength="100" width="300"></asp:textbox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <rbfwebui:localize id="Literal1" runat="server" text="Name" textkey="BLOG_NAME">
                                    </rbfwebui:localize>&nbsp;
                                </td>
                                <td>
                                    <asp:textbox id="txtName" runat="server" maxlength="100" width="300"></asp:textbox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <rbfwebui:localize id="Literal2" runat="server" text="URL" textkey="BLOG_URL">
                                    </rbfwebui:localize>&nbsp;
                                </td>
                                <td>
                                    <asp:textbox id="txtURL" runat="server" maxlength="200" width="300"></asp:textbox>
                                </td>
                            </tr>
                        </table>
                        <rbfwebui:localize id="Literal3" runat="server" text="Comments" textkey="BLOG_COMMENTS">
                        </rbfwebui:localize>
                        <br />
                        <asp:textbox id="txtComments" runat="server" height="200" textmode="MultiLine" width="400"></asp:textbox>
                        <br />
                        <asp:checkbox id="chkRememberMe" runat="server" />&nbsp;
                        <rbfwebui:localize id="Literal5" runat="server" text="Remember Me?" textkey="BLOG_REMEMBER_ME">
                        </rbfwebui:localize>
                        <br />
                        <rbfwebui:button id="btnPostComment" runat="server" text="Submit" textkey="SUBMIT" />
                        <!-- end comments form -->
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
