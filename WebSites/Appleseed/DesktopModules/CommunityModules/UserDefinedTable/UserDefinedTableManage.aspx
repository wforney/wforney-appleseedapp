<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.UserDefinedTableManage"
    language="c#" Codebehind="UserDefinedTableManage.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div class="rb_AlternateLayoutDiv">
            <table border="0" cellpadding="0" cellspacing="0" class="rb_AlternateLayoutTable">
                <tr valign="top">
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="100">
                                    &nbsp;</td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head">
                                                <rbfwebui:label id="ManageTableLabel" runat="Server" text="Manage Table" textkey="USERTABLE_MANAGETABLE">
                                                </rbfwebui:label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr noshade="noshade" size="1" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:datagrid id="grdFields" runat="server" autogeneratecolumns="False" border="0"
                                        cellpadding="2" cellspacing="0" cssclass="Normal" datakeyfield="UserDefinedFieldID"
                                        oncancelcommand="grdFields_CancelEdit" ondeletecommand="grdFields_Delete" oneditcommand="grdFields_Edit"
                                        onitemcommand="grdFields_Move" onupdatecommand="grdFields_Update">
                                        <columns>
                                            <rbfwebui:templatecolumn>
                                                <itemtemplate>
                                                    <rbfwebui:imagebutton ID="cmdEditUserDefinedField" runat="server" CausesValidation="false"
                                                        CommandName="Edit" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                                                        AlternateText="Edit" TextKey="EDIT"></rbfwebui:imagebutton>
                                                    <rbfwebui:imagebutton ID="cmdDeleteUserDefinedField" runat="server" CausesValidation="false"
                                                        CommandName="Delete" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl %>'
                                                        AlternateText="Delete" TextKey="DELETE"></rbfwebui:imagebutton>
                                                </itemtemplate>
                                                <edititemtemplate>
                                                    <rbfwebui:imagebutton ID="cmdSaveUserDefinedField" runat="server" CausesValidation="false"
                                                        CommandName="Update" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Save", "Save.gif").ImageUrl %>'
                                                        AlternateText="Save" TextKey="SAVE"></rbfwebui:imagebutton>
                                                    <rbfwebui:imagebutton ID="cmdCancelUserDefinedField" runat="server" CausesValidation="false"
                                                        CommandName="Cancel" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl %>'
                                                        AlternateText="Cancel" TextKey="CANCEL"></rbfwebui:imagebutton>
                                                </edititemtemplate>
                                            </rbfwebui:templatecolumn>
                                            <rbfwebui:templatecolumn headerstyle-cssclass="NormalBold" headerstyle-horizontalalign="Center"
                                                headertext="Visible" itemstyle-cssclass="Normal" itemstyle-horizontalalign="Center"
                                                textkey="USERTABLE_VISIBLE">
                                                <itemtemplate>
														<asp:Image runat="server" ImageUrl='<%# IfVisible(Container.DataItem, "Checked", "Unchecked") %>' ID="Image2"/>
													</itemtemplate>
                                                <edititemtemplate>
														<asp:CheckBox runat="server" id="Checkbox2" Checked='True' />
													</edititemtemplate>
                                            </rbfwebui:templatecolumn>
                                            <rbfwebui:templatecolumn headerstyle-cssclass="NormalBold" headertext="Title" itemstyle-cssclass="Normal"
                                                textkey="USERTABLE_TITLE">
                                                <itemtemplate>
														<rbfwebui:label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FieldTitle") %>' ID="Label1"/>
													</itemtemplate>
                                                <edititemtemplate>
														<asp:TextBox runat="server" id="txtFieldTitle" Columns="30" MaxLength="50" Text='<%# DataBinder.Eval(Container.DataItem, "FieldTitle") %>' CssClass="NormalTextBox" />
													</edititemtemplate>
                                            </rbfwebui:templatecolumn>
                                            <rbfwebui:templatecolumn headerstyle-cssclass="NormalBold" headertext="Type" itemstyle-cssclass="Normal"
                                                textkey="USERTABLE_TYPE">
                                                <itemtemplate>
														<rbfwebui:label runat="server" Text='<%# GetFieldTypeName(DataBinder.Eval(Container.DataItem, "FieldType").ToString()) %>' ID="Label2"/>
													</itemtemplate>
                                                <edititemtemplate>
														<asp:DropDownList ID="cboFieldType" Runat="server" CssClass="NormalTextBox" SelectedIndex='<%# GetFieldTypeIndex(DataBinder.Eval(Container.DataItem, "FieldType").ToString()) %>' DataSource="<%# GetTableTypes() %>" DataTextField="TypeText" DataValueField="TypeValue">
														</asp:DropDownList>
													</edititemtemplate>
                                            </rbfwebui:templatecolumn>
                                            <rbfwebui:templatecolumn>
                                                <itemtemplate>
                                                    <rbfwebui:imagebutton ID="cmdMoveUserDefinedFieldUp" runat="server" CausesValidation="false"
                                                        CommandName="Item" CommandArgument="Up" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Up", "Up.gif").ImageUrl %>'
                                                        AlternateText="Move Field Up" TextKey="USERTABLE_MOVEFIELDUP"></rbfwebui:imagebutton>
                                                </itemtemplate>
                                            </rbfwebui:templatecolumn>
                                            <rbfwebui:templatecolumn>
                                                <itemtemplate>
                                                    <rbfwebui:imagebutton ID="cmdMoveUserDefinedFieldDown" runat="server" CausesValidation="false"
                                                        CommandName="Item" CommandArgument="Down" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Down", "Down.gif").ImageUrl %>'
                                                        AlternateText="Move Field Down" TextKey="USERTABLE_MOVEFIELDDOWN"></rbfwebui:imagebutton>
                                                </itemtemplate>
                                            </rbfwebui:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <hr noshade="noshade" size="1" width="500" />
                                    <p>
                                        <rbfwebui:linkbutton id="cmdAddField" runat="server" causesvalidation="False" class="CommandButton"
                                            onclick="cmdAddField_Click" text="Add New Field" textkey="USERTABLE_NEWFIELD">
                                        </rbfwebui:linkbutton>&nbsp;
                                        <rbfwebui:linkbutton id="cmdCancel" runat="server" causesvalidation="False" class="CommandButton"
                                            onclick="cmdCancel_Click" text="Back" textkey="USERTABLE_BACK">
                                        </rbfwebui:linkbutton>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="rb_AlternatePortalFooter">
                        <div class="rb_AlternatePortalFooter">
                            <foot:footer id="Footer" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
