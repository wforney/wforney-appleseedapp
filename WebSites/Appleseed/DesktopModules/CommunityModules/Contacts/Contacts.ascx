<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.Contacts"
    language="c#" Codebehind="Contacts.ascx.cs" %>
<asp:datagrid id="myDataGrid" runat="server" allowsorting="True" autogeneratecolumns="False"
    enableviewstate="False" width="100%">
    <columns>
        <rbfwebui:templatecolumn>
            <itemtemplate>
                <rbfwebui:HyperLink id="HyperLink1" runat="server" imageurl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                    navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/Contacts/ContactsEdit.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mid=" + ModuleID)%>'
                    text="Edit" textkey="EDIT" visible='<%# IsEditable %>'>
                </rbfwebui:HyperLink>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_NAME%>" sortexpression="Name">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize1" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Name") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_ROLE%>" sortexpression="Role">
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize2" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Role") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_EMAIL%>" sortexpression="Email">
            <itemstyle wrap="False" />
            <itemtemplate>
                <rbfwebui:HyperLink id="HyperLinkView" runat="server" cssclass="Normal" navigateurl='<%# "mailto:" + DataBinder.Eval(Container.DataItem,"Email")%>'
                    text='<%# DataBinder.Eval(Container, "DataItem.Email") %>'>
                </rbfwebui:HyperLink>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_CONTACT1%>"
            sortexpression="Contact1">
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize3" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Contact1") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_CONTACT2%>"
            sortexpression="Contact2">
            <itemstyle cssclass="Normal" wrap="False" />
            <itemtemplate>
                <rbfwebui:localize id="Localize4" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Contact2") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_FAX%>" sortexpression="Fax">
            <itemstyle cssclass="Normal" wrap="False" />
            <itemtemplate>
                <rbfwebui:localize id="Localize5" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Fax") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
        <rbfwebui:templatecolumn headertext="<%$ Resources:Appleseed, CONTACTS_ADDRESS%>" sortexpression="Address">
            <headerstyle cssclass="NormalBold" />
            <itemstyle cssclass="Normal" />
            <itemtemplate>
                <rbfwebui:localize id="Localize6" runat="server" text='<%# DataBinder.Eval(Container, "DataItem.Address") %>'>
                </rbfwebui:localize>
            </itemtemplate>
        </rbfwebui:templatecolumn>
    </columns>
</asp:datagrid>
