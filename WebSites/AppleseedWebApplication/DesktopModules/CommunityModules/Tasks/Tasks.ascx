<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Tasks"
    language="c#" Codebehind="Tasks.ascx.cs" %>
<asp:gridview id="myDataGrid" runat="server" allowpaging="False" allowsorting="True"
    alternatingrowstyle-cssclass="Grid_AlternatingItem" autogeneratecolumns="False"
    borderwidth="0" headerstyle-cssclass="Grid_Header" rowstyle-cssclass="Grid_Item"
    width="100%">
    <columns>
        <asp:templatefield>
            <itemtemplate>
                <rbfwebui:hyperlink id="HyperLinkEdit" runat="server" imageurl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    navigateurl='<%# "~/DesktopModules/CommunityModules/Tasks/TasksEdit.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID %>'
                    text="Edit" textkey="EDIT" visible="<%# IsEditable %>"></rbfwebui:hyperlink>
            </itemtemplate>
        </asp:templatefield>
        <asp:boundfield datafield="DueDate" dataformatstring="{0:d}" headertext="<%$ Resources:Appleseed, TASK_DUEDATE %>"
            sortexpression="DueDate">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
        </asp:boundfield>
        <asp:templatefield headertext="<%$ Resources:Appleseed, TASK_TITLE %>" sortexpression="Title">
            <headerstyle cssclass="NormalBold" />
            <itemtemplate>
                <rbfwebui:hyperlink id="HyperLinkView" runat="server" cssclass="Normal" navigateurl='<%# "~/DesktopModules/CommunityModules/Tasks/TasksView.aspx?ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID %>'
                    text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'>
                </rbfwebui:hyperlink>
            </itemtemplate>
        </asp:templatefield>
        <asp:templatefield headertext="<%$ Resources:Appleseed, TASK_STATUS %>" sortexpression="Status">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize1" runat="server" text='' textkey='<%# "TASK_STATE_"+DataBinder.Eval(Container, "DataItem.Status") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </asp:templatefield>
        <asp:templatefield headertext="<%$ Resources:Appleseed, TASK_PRIORITY %>" sortexpression="Priority">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize2" runat="server" textkey='<%# "TASK_PRIORITY_"+DataBinder.Eval(Container, "DataItem.Priority") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </asp:templatefield>
        <asp:boundfield datafield="AssignedTo" headertext="<%$ Resources:Appleseed, TASK_ASSIGNEDTO %>"
            sortexpression="AssignedTo">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
        </asp:boundfield>
        <asp:templatefield headertext="<%$ Resources:Appleseed, TASK_COMPLETION %>" sortexpression="PercentComplete">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <table border="1" cellpadding="0" cellspacing="0" class="Normal" style="border: 1px solid black;"
                    width="100%">
                    <tr>
                    </tr>
                </table>
            </itemtemplate>
        </asp:templatefield>
        <asp:boundfield datafield="ModifiedDate" dataformatstring="{0:d}" headertext="<%$ Resources:Appleseed, DOCUMENT_LAST_UPDATED %>"
            sortexpression="ModifiedDate">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
        </asp:boundfield>
    </columns>
</asp:gridview>
