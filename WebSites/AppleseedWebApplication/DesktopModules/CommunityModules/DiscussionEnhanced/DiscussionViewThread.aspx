<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.DiscussionViewThread"
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
        <%-- needed to get site header to render correctly --%>
        <div class="rb_DefaultLayoutDiv">
            <table class="rb_DefaultLayoutTable">
                <tr valign="top">
                    <td class="rb_DefaultPortalHeader" colspan="2" valign="top">
                        <portal:banner id="Banner1" runat="server" showtabs="false" />
                    </td>
                </tr>
                <tr valign="top">
                    <td width="5%">
                        &nbsp;</td>
                    <td width="95%">
                        <br/>
                        <%-- DataList of all the threads in this topic --%>
                        <asp:datalist id="ThreadList" runat="server" datakeyfield="ItemID" enableviewstate="true"
                            itemstyle-cssclass="Normal" onitemcommand="ThreadList_Select" onitemdatabound="OnItemDataBound">
                            <headertemplate>
                                <rbfwebui:label id="Label1" runat="server" cssclass="ItemTitle" text="Displaying all threads for this topic"
                                    textkey="DS_DISPLAY_THREADS">
                                </rbfwebui:label>
                                <hr>
                                <br>
                            </headertemplate>
                            <footertemplate>
                                <hr>
                                <rbfwebui:linkbutton id="LinkButton1" runat="server" commandname="return_to_discussion_list"
                                    cssclass="Normal" text="Return to list of threads" textkey="DS_RETURN_LIST">
                                </rbfwebui:linkbutton>
                                <!-- Known bug:--this link doesn't work corectly if you edit an item on this page.-->
                                <br>
                                <br>
                            </footertemplate>
                            <itemstyle cssclass="Normal" />
                            <itemtemplate>
                                <%# DataBinder.Eval(Container.DataItem, "BlockQuoteStart") %>
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
                                <asp:panel runat="server" cssclass="NormalDim">
                                </asp:panel>
                                    <rbfwebui:label id="Label11" runat="server" cssclass="Normal" text="Posted by" textkey="POSTED_BY"></rbfwebui:label>
                                    <rbfwebui:label id="Label10" runat="server" cssclass="NormalDim" text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                                    </rbfwebui:label>,
                                    <rbfwebui:label id="Label9" runat="server" cssclass="Normal" text="posted on" textkey="POSTED_DATE"></rbfwebui:label>
                                    <rbfwebui:label id="Label8" runat="server" cssclass="NormalDim" text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                                    </rbfwebui:label><br>
                                    <rbfwebui:label id="Label3" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'></rbfwebui:label><%# DataBinder.Eval(Container.DataItem, "BlockQuoteEnd") %>
                            </itemtemplate>
                        </asp:datalist></td>
                </tr>
                <tr>
                    <td class="rb_DefaultPortalFooter" colspan="2">
                        <foot:footer id="Footer" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
