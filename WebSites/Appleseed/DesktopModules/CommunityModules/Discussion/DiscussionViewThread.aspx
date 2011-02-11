<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Discussion.DiscussionViewThread"
    language="c#" Codebehind="DiscussionViewThread.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" name="form1">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="Banner1" runat="server" showtabs="false" />
            </div>
            <div class="div_ev_Table">
                <%-- DataList of all the threads in this topic --%>
                <asp:datalist id="ThreadList" runat="server" datakeyfield="ItemID" enableviewstate="true"
                    itemstyle-cssclass="Normal" onitemcommand="ThreadList_Select" onitemdatabound="OnItemDataBound">
                    <headertemplate>
                        <rbfwebui:label id="Label1" runat="server" cssclass="ItemTitle" text="Displaying all threads for this topic"
                            textkey="DS_DISPLAY_THREADS">
                        </rbfwebui:label>
                        <hr />
                        <br />
                    </headertemplate>
                    <footertemplate>
                        <hr />
                        <rbfwebui:linkbutton id="LinkButton1" runat="server" commandname="return_to_discussion_list"
                            cssclass="Normal" text="Return to list of threads" textkey="DS_RETURN_LIST">
                        </rbfwebui:linkbutton>
                        <br />
                        <br />
                    </footertemplate>
                    <itemstyle cssclass="Normal" />
                    <itemtemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteStart") %>
                        <div class="discuss_reply">
                            <div class="discuss_reply_controls">
                                <rbfwebui:label id="Label4" runat="server" cssclass="ItemTitle" text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'></rbfwebui:label>
                                <rbfwebui:hyperlink id="HyperLink2" runat="server" imageurl="<%# GetReplyImage() %>"
                                    navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                                    text="Reply to this message_" textkey="DS_REPLYTHISMSG" visible="True"></rbfwebui:hyperlink>
                                <rbfwebui:hyperlink id="HyperLink1" runat="server" imageurl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                                    navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                                    text="Edit this message" textkey="EDIT" visible="True"></rbfwebui:hyperlink>
                                <rbfwebui:imagebutton id="deleteBtn" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                                    commandname="delete" imageurl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                                    textkey="DELETE_THIS_ITEM" />
                            </div>
                            <div class="discuss_author">
                                <rbfwebui:label id="Label11" runat="server" cssclass="Normal" text="Posted by" textkey="POSTED_BY"></rbfwebui:label>
                                <rbfwebui:label id="Label10" runat="server" text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                                </rbfwebui:label>,
                                <rbfwebui:label id="Label9" runat="server" cssclass="Normal" text="posted on" textkey="POSTED_DATE"></rbfwebui:label>
                                <rbfwebui:label id="Label8" runat="server" text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                                </rbfwebui:label>
                            </div>
                            <div class="discuss_reply_body">
                                <rbfwebui:label id="Label3" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'></rbfwebui:label>
                            </div>
                        </div>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteEnd") %>
                    </itemtemplate>
                </asp:datalist>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
