<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Discussion"
    language="c#" Codebehind="Discussion.ascx.cs" %>
<%-- list of threads, URL must have a Parent="parent from dataabse" --%>
<asp:datalist id="TopLevelList" runat="server" datakeyfield="ItemID" enableviewstate="false"
    itemstyle-cssclass="Normal" onitemdatabound="OnItemDataBound" width="98%">
    <headertemplate>
        <tr>
            <td width="50%">
                <rbfwebui:label id="Label14" runat="server" cssclass="NormalBold" textkey="DISCUSION_TITLE">Titulo</rbfwebui:label></td>
            <td align="right" width="25%">
                &nbsp;
                <rbfwebui:label id="Label13" runat="server" cssclass="NormalBold" textkey="DISCUSION_AUTHOR">Autor</rbfwebui:label></td>
            <td align="right" width="25%">
                <rbfwebui:label id="Label12" runat="server" cssclass="NormalBold" textkey="DISCUSSION_LAST_UPDATE">Ultima Actualizacion</rbfwebui:label></td>
        </tr>
        <tr>
            <td colspan="3">
                <hr>
            </td>
        </tr>
    </headertemplate>
    <selecteditemtemplate>
        <tr bgcolor="WhiteSmoke">
            <td colspan="3">
                <rbfwebui:imagebutton id="btnCollapse" runat="server" commandname="CollapseThread"
                    imageurl='<%# getLocalImage("minus.gif") %>' />
                <rbfwebui:label id="Label1" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "ChildCount") %>'
                    tooltip="Number of replys to this topic">
                </rbfwebui:label>/
                <rbfwebui:label id="Label2" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "ViewCount") %>'
                    tooltip="Number of times this topic has been viewed">
                </rbfwebui:label>&nbsp;
                <rbfwebui:linkbutton id="LinkButton2" runat="server" commandname="CollapseThread"
                    cssclass="ItemTitle" text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                </rbfwebui:linkbutton><%-- add the property 'Target="_new"' to the following hyperlink to have edits occur up in a new browser window --%>
                <rbfwebui:hyperlink id="HyperLink2" runat="server" imageurl="<%# GetReplyImage() %>"
                    navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                    text="Reply to this message_" textkey="DS_REPLYTHISMSG" visible="True">
                </rbfwebui:hyperlink>
                <rbfwebui:hyperlink id="HyperLink1" runat="server" imageurl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                    navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                    text="Edit this message" textkey="EDIT" visible="True">
                </rbfwebui:hyperlink>
                <rbfwebui:imagebutton id="deleteBtn" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    commandname="delete" imageurl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                    textkey="DELETE_THIS_ITEM" />
                <rbfwebui:label id="Label11" runat="server" cssclass="Normal" text="Posted by" textkey="POSTED_BY"></rbfwebui:label>
                <rbfwebui:label id="Label10" runat="server" cssclass="NormalBI" text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                </rbfwebui:label>,
                <rbfwebui:label id="Label9" runat="server" cssclass="Normal" text="posted on" textkey="POSTED_DATE"></rbfwebui:label>
                <rbfwebui:label id="Label8" runat="server" cssclass="NormalBI" text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                </rbfwebui:label><br>
                <rbfwebui:label id="Label7" runat="server" cssclass="NormalBold" text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'>
                </rbfwebui:label>
                <p>
                </p>
                <%-- expanded responses to main thread --%>
                <asp:datalist id="DetailList" runat="server" datasource="<%# GetThreadMessages() %>"
                    onitemcommand="TopLevelListOrDetailList_Select" onitemdatabound="OnItemDataBound">
                    <itemtemplate>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteStart") %>
                        <rbfwebui:label id="Label3" runat="server" cssclass="ItemTitle" text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                        </rbfwebui:label>
                        <rbfwebui:hyperlink id="Hyperlink1" runat="server" imageurl='<%# GetReplyImage() %>'
                            navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "REPLY") %>'
                            text='Reply to this message_' textkey="DS_REPLYTHISMSG" visible="True">
                        </rbfwebui:hyperlink>
                        <rbfwebui:hyperlink id="Hyperlink2" runat="server" imageurl='<%# GetEditImage((string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                            navigateurl='<%# FormatUrlEditItem((int)DataBinder.Eval(Container.DataItem, "ItemID"), "EDIT") %>'
                            text="Edit this message" textkey="EDIT" visible="True">
                        </rbfwebui:hyperlink>
                        <rbfwebui:imagebutton id="deleteBtnExpanded" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                            commandname="delete" imageurl='<%# GetDeleteImage((int)DataBinder.Eval(Container.DataItem, "ItemID"),(string)DataBinder.Eval(Container.DataItem,"CreatedByUser")) %>'
                            textkey="DELETE_THIS_ITEM" />
                        <rbfwebui:label id="Label4" runat="server" cssclass="Normal" text="Posted by" textkey="POSTED_BY">
                        </rbfwebui:label>
                        <rbfwebui:label id="Label5" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'>
                        </rbfwebui:label>
                        ,
                        <rbfwebui:label id="Label6" runat="server" cssclass="Normal" text="posted on" textkey="POSTED_DATE">
                        </rbfwebui:label>
                        <rbfwebui:label id="Label15" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"CreatedDate", "{0:g}") %>'>
                        </rbfwebui:label>
                        <br>
                        <rbfwebui:label id="Label16" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"Body") %>'>
                        </rbfwebui:label>
                        <%# DataBinder.Eval(Container.DataItem, "BlockQuoteEnd") %>
                    </itemtemplate>
                </asp:datalist></td>
        </tr>
    </selecteditemtemplate>
    <itemstyle cssclass="Normal" />
    <itemtemplate>
        <tr>
            <td width="60%">
                <rbfwebui:imagebutton id="btnSelect" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    commandname="ExpandThread" imageurl='<%# NodeImage((int)DataBinder.Eval(Container.DataItem, "ChildCount")) %>'
                    tooltip="Expand the thread of this topic inside this browser page" />
                <rbfwebui:imagebutton id="btnNewWindow" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    commandname="ShowThreadNewWindow" imageurl='<%# getLocalImage("new_window.gif") %>'
                   tooltip="Open the thread of this topic in a new browser page" />
                <rbfwebui:label id="Label6" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "ChildCount") %>'
                    tooltip="Number of replys to this topic">
                </rbfwebui:label>/
                <rbfwebui:label id="Label5" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem, "ViewCount") %>'
                    tooltip="Number of times this topic has been viewed">
                </rbfwebui:label>&nbsp;
                <rbfwebui:linkbutton id="LinkButton1" runat="server" commandargument='<%# DataBinder.Eval(Container.DataItem, "ItemID") %>'
                    commandname="ExpandThread" cssclass="ItemTitle" text='<%# DataBinder.Eval(Container.DataItem, "Title") %>'>
                </rbfwebui:linkbutton></td>
            <td align="right" width="20%">
                <rbfwebui:label id="Label4" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"CreatedByUser") %>'
                    tooltip="Author of this post">
                </rbfwebui:label>&nbsp;
            </td>
            <td align="right" width="20%">
                <rbfwebui:label id="Label3" runat="server" cssclass="Normal" text='<%# DataBinder.Eval(Container.DataItem,"DateofLastReply", "{0:g}") %>'
                    tooltip="Date of most recent reply">
                </rbfwebui:label></td>
        </tr>
    </itemtemplate>
</asp:datalist>
