<%@ Page AutoEventWireup="false" Inherits="Appleseed.Admin.PageLayout" Language="c#" 
MasterPageFile="~/Shared/SiteMasterDefault.master" Codebehind="PageLayout.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <table border="0" cellpadding="2" cellspacing="1" class="ModuleWrap">
        <tr>
            <td colspan="4">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:Localize ID="tab_name" runat="server" Text="Page Layouts" TextKey="AM_TABNAME" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="Normal" width="100">
                <rbfwebui:Localize ID="tab_name1" runat="server" Text="Page Name" TextKey="AM_TABNAME1">
                </rbfwebui:Localize>
            </td>
            <td colspan="3">
                <asp:TextBox ID="tabName" runat="server" CssClass="NormalTextBox" MaxLength="50"
                    Width="300" OnTextChanged="PageSettings_Change" />
            </td>
        </tr>
        <tr>
            <td class="Normal" nowrap="nowrap">
                <rbfwebui:Localize ID="roles_auth" runat="server" Text="Authorized Roles" TextKey="AM_ROLESAUTH">
                </rbfwebui:Localize>
            </td>
            <td colspan="3">
                <asp:CheckBoxList ID="authRoles" runat="server" CssClass="Normal" RepeatColumns="2"
                    Width="300" OnSelectedIndexChanged="PageSettings_Change" />
            </td>
        </tr>
        <tr>
            <td class="Normal" nowrap="nowrap">
                <rbfwebui:Localize ID="tab_parent" runat="server" Text="Parent Page" TextKey="TAB_PARENT">
                </rbfwebui:Localize>
            </td>
            <td colspan="3">
                <asp:DropDownList ID="parentPage" runat="server" CssClass="NormalTextBox" Width="300px"
                    DataTextField="Name" DataValueField="ID">
                </asp:DropDownList>
                <rbfwebui:Label ID="lblErrorNotAllowed" runat="server" CssClass="Error" EnableViewState="False"
                    TextKey="ERROR_NOT_ALLOWED_PARENT" Visible="False">Not allowed to choose that parent</rbfwebui:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td colspan="3">
                <hr noshade="noshade" size="1" />
            </td>
        </tr>
        <tr>
            <td class="Normal" nowrap="nowrap">
                <rbfwebui:Localize ID="show_mobile" runat="server" Text="Show to mobile users" TextKey="AM_SHOWMOBILE">
                </rbfwebui:Localize>
            </td>
            <td colspan="3">
                <asp:CheckBox ID="showMobile" runat="server" CssClass="Normal" OnCheckedChanged="PageSettings_Change" />
            </td>
        </tr>
        <tr>
            <td class="Normal" nowrap="nowrap">
                <rbfwebui:Localize ID="mobiletab" runat="server" Text="Mobile Page Name" TextKey="AM_MOBILETAB">
                </rbfwebui:Localize>
            </td>
            <td colspan="3">
                <asp:TextBox ID="mobilePageName" runat="server" CssClass="NormalTextBox" Width="300"
                    OnTextChanged="PageSettings_Change" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <hr noshade="noshade" size="1" />
            </td>
        </tr>
        <tr>
            <td class="Normal">
                <rbfwebui:Localize ID="addmodule" runat="server" Text="Add module" TextKey="AM_ADDMODULE">
                </rbfwebui:Localize>
            </td>
            <td class="Normal">
                <rbfwebui:Localize ID="module_type" runat="server" Text="Module type" TextKey="AM_MODULETYPE">
                </rbfwebui:Localize>
            </td>
            <td colspan="2">
                <asp:DropDownList ID="moduleType" runat="server" CssClass="NormalTextBox" DataTextField="key"
                    DataValueField="value">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td class="Normal">
                <rbfwebui:Localize ID="moduleLocationLabel" runat="server" Text="Module Location:"
                    TextKey="AM_MODULELOCATION">
                </rbfwebui:Localize>
            </td>
            <td colspan="2" valign="top">
                <asp:DropDownList ID="paneLocation" runat="server">
                    <asp:listitem value="TopPane">Header</asp:listitem>
                    <asp:listitem value="LeftPane">Left Column</asp:listitem>
                    <asp:listitem selected="True" value="ContentPane">Center Column</asp:listitem>
                    <asp:listitem value="RightPane">Right Column</asp:listitem>
                    <asp:listitem value="BottomPane">Footer</asp:listitem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td class="Normal" valign="top">
                <rbfwebui:Localize ID="moduleVisibleLabel" runat="server" Text="Module Visible To:"
                    TextKey="AM_MODULEVISIBLETO">
                </rbfwebui:Localize>
            </td>
            <td colspan="2" valign="top">
                <asp:DropDownList ID="viewPermissions" runat="server">
                    <asp:ListItem Selected="True" Value="All Users;">All Users</asp:ListItem>
                    <asp:ListItem Value="Authenticated Users;">Authenticated Users</asp:ListItem>
                    <asp:ListItem Value="Unauthenticated Users;">Unauthenticated Users</asp:ListItem>
                    <asp:ListItem Value="Admins;">Admins Role</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td class="Normal">
                <rbfwebui:Localize ID="module_name" runat="server" Text="Module Name" TextKey="AM_MODULENAME">
                </rbfwebui:Localize>
            </td>
            <td colspan="2">
                <asp:TextBox ID="moduleTitle" runat="server" CssClass="NormalTextBox" EnableViewState="false"
                    Text="New Module Name" Width="250"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td colspan="3">
                <rbfwebui:LinkButton ID="AddModuleBtn" runat="server" CssClass="CommandButton" Text="Add to 'Organize Modules' Below"
                    TextKey="AM_ADDMODULEBELOW" OnClick="AddModuleToPane_Click" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td colspan="3">
                <hr noshade="noshade" size="1" />
            </td>
        </tr>
        <tr valign="top">
            <td class="Normal" rowspan="3">
                <rbfwebui:Localize ID="organizemodule" runat="server" Text="Organize Module" TextKey="AM_ORGANIZEMODULE">
                </rbfwebui:Localize>
            </td>
            <td width="*" colspan="3">
                <table border="0" cellpadding="2" cellspacing="0" width="100%">
                    <tr>
                        <td class="NormalBold">
                            &nbsp;
                            <rbfwebui:Localize ID="topPanel" runat="server" Text="Top Pane" TextKey="AM_TOPPANEL">
                            </rbfwebui:Localize>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table border="0" cellpadding="0" cellspacing="2">
                                <tr valign="top">
                                    <td rowspan="2">
                                        <asp:ListBox ID="topPane" runat="server" CssClass="NormalTextBox" DataSource="<%# topList %>"
                                            DataTextField="Title" DataValueField="ID" Rows="8" Width="690"></asp:ListBox>
                                    </td>
                                    <td nowrap="nowrap" valign="top">
                                        <rbfwebui:ImageButton ID="TopUpBtn" runat="server" CommandArgument="topPane"
                                            CommandName="up" text="Move Up" TextKey="MOVEUP" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="TopDownBtn" runat="server" CommandArgument="topPane"
                                            CommandName="down" text="Move Down" TextKey="MOVEDOWN" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="TopRightBtn" runat="server" CommandName="right" sourcepane="topPane"
                                            targetpane="contentPane" text="Move Right" TextKey="MOVERIGHT" OnClick="RightLeft_Click" />&nbsp;&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" valign="Top">
                                        <rbfwebui:ImageButton ID="TopEditBtn" runat="server" CommandArgument="topPane"
                                            CommandName="edit" text="Edit" TextKey="EDIT" OnClick="EditBtn_Click" /><br />
                                        <br />
                                        <rbfwebui:ImageButton ID="TopDeleteBtn" runat="server" CommandArgument="topPane"
                                            CommandName="delete" text="Delete" TextKey="DELETE" OnClick="DeleteBtn_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>

        <tr valign="top">
            <td width="120">
                <table border="0" cellpadding="2" cellspacing="0" width="100%">
                    <tr>
                        <td class="NormalBold">
                            <rbfwebui:Localize ID="LeftPanel" runat="server" Text="Left Pane" TextKey="AM_LEFTPANEL">
                            </rbfwebui:Localize>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <table border="0" cellpadding="0" cellspacing="2">
                                <tr valign="top">
                                    <td rowspan="2">
                                        <asp:ListBox ID="leftPane" runat="server" CssClass="NormalTextBox" DataSource="<%# leftList %>"
                                            DataTextField="Title" DataValueField="ID" Rows="8" Width="110"></asp:ListBox>
                                    </td>
                                    <td nowrap="nowrap" valign="top">
                                        <rbfwebui:ImageButton ID="LeftUpBtn" runat="server" CommandArgument="leftPane" CommandName="up"
                                            text="Move Up" TextKey="MOVEUP" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="LeftRightBtn" runat="server" CommandName="right" sourcepane="leftPane"
                                            targetpane="contentPane" text="Move Right" TextKey="MOVERIGHT" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="LeftDownBtn" runat="server" CommandArgument="leftPane"
                                            CommandName="down" text="Move Down" TextKey="MOVEDOWN" OnClick="UpDown_Click" />&nbsp;&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" valign="bottom">
                                        <rbfwebui:ImageButton ID="LeftEditBtn" runat="server" CommandArgument="leftPane"
                                            CommandName="edit" text="Edit" TextKey="EDIT" OnClick="EditBtn_Click" /><br />
                                        <br />
                                        <rbfwebui:ImageButton ID="LeftDeleteBtn" runat="server" CommandArgument="leftPane"
                                            CommandName="delete" text="Delete" TextKey="DELETE" OnClick="DeleteBtn_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td >
                <table border="0" cellpadding="2" cellspacing="0">
                    <tr>
                        <td class="NormalBold">
                            &nbsp;
                            <rbfwebui:Localize ID="contentpanel" runat="server" Text="Content Pane" TextKey="AM_CENTERPANEL">
                            </rbfwebui:Localize>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                <tr valign="top">
                                    <td rowspan="2">
                                        <asp:ListBox ID="contentPane" runat="server" CssClass="NormalTextBox" DataSource="<%# contentList %>"
                                            DataTextField="Title" DataValueField="ID" Rows="8" Width="290"></asp:ListBox>
                                    </td>
                                    <td nowrap="nowrap" valign="top">
                                        <rbfwebui:ImageButton ID="ContentTopBtn" runat="server" sourcepane="contentPane"
                                            targetpane="topPane" text="Move Top" TextKey="MOVETOP" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="ContentUpBtn" runat="server" CommandArgument="contentPane"
                                            CommandName="up" text="Move Up" TextKey="MOVEUP" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="ContentLeftBtn" runat="server" sourcepane="contentPane"
                                            targetpane="leftPane" text="Move Left" TextKey="MOVELEFT" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="ContentRightBtn" runat="server" sourcepane="contentPane"
                                            targetpane="rightPane" text="Move Right" TextKey="MOVERIGHT" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="ContentDownBtn" runat="server" CommandArgument="contentPane"
                                            CommandName="down" text="Move Down" TextKey="MOVEDOWN" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="ContentBottomBtn" runat="server" sourcepane="contentPane"
                                            targetpane="BottomPane" text="Move Bottom" TextKey="MOVEBOTTOM" OnClick="RightLeft_Click" /><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" valign="bottom">
                                        <rbfwebui:ImageButton ID="ContentEditBtn" runat="server" CommandArgument="contentPane"
                                            CommandName="edit" text="Edit" TextKey="EDIT" OnClick="EditBtn_Click" /><br />
                                        <br />
                                        <rbfwebui:ImageButton ID="ContentDeleteBtn" runat="server" CommandArgument="contentPane"
                                            CommandName="delete" text="Delete" TextKey="DELETE" OnClick="DeleteBtn_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td width="120">
                <table border="0" cellpadding="2" cellspacing="0" width="100%">
                    <tr>
                        <td class="NormalBold">
                            &nbsp;
                            <rbfwebui:Localize ID="rightpanel" runat="server" Text="Right Pane" TextKey="AM_RIGHTPANEL">
                            </rbfwebui:Localize>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table border="0" cellpadding="0" cellspacing="2">
                                <tr valign="top">
                                    <td rowspan="2">
                                        <asp:ListBox ID="rightPane" runat="server" CssClass="NormalTextBox" DataSource="<%# rightList %>"
                                            DataTextField="Title" DataValueField="ID" Rows="8" Width="110"></asp:ListBox>
                                    </td>
                                    <td nowrap="nowrap" valign="top">
                                        <rbfwebui:ImageButton ID="RightUpBtn" runat="server" CommandArgument="rightPane"
                                            CommandName="up" text="Move Up" TextKey="MOVEUP" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="RightLeftBtn" runat="server" sourcepane="rightPane" targetpane="contentPane"
                                            text="Move Left" TextKey="MOVELEFT" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="RightDownBtn" runat="server" CommandArgument="rightPane"
                                            CommandName="down" text="Move Down" TextKey="MOVEDOWN" OnClick="UpDown_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" valign="bottom">
                                        <rbfwebui:ImageButton ID="RightEditBtn" runat="server" CommandArgument="rightPane"
                                            CommandName="edit" text="Edit" TextKey="EDIT" OnClick="EditBtn_Click" /><br />
                                        <br />
                                        <rbfwebui:ImageButton ID="RightDeleteBtn" runat="server" CommandArgument="rightPane"
                                            CommandName="delete" text="Delete" TextKey="DELETE" OnClick="DeleteBtn_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr valign="top">
            <td width="*" colspan="3">
                <table border="0" cellpadding="2" cellspacing="0" width="100%">
                    <tr>
                        <td class="NormalBold">
                            &nbsp;
                            <rbfwebui:Localize ID="bottomPanel" runat="server" Text="Bottom Pane" TextKey="AM_BOTOMPANEL">
                            </rbfwebui:Localize>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table border="0" cellpadding="0" cellspacing="2">
                                <tr valign="top">
                                    <td rowspan="2">
                                        <asp:ListBox ID="bottomPane" runat="server" CssClass="NormalTextBox" DataSource="<%# bottomList %>"
                                            DataTextField="Title" DataValueField="ID" Rows="8" Width="690"></asp:ListBox>
                                    </td>
                                    <td nowrap="nowrap" valign="top">
                                        <rbfwebui:ImageButton ID="BottomLeftBtn" runat="server" sourcepane="bottomPane" targetpane="contentPane"
                                            text="Move Left" TextKey="MOVELEFT" OnClick="RightLeft_Click" /><br />
                                        <rbfwebui:ImageButton ID="BottomUpBtn" runat="server" CommandArgument="bottomPane"
                                            CommandName="up" text="Move Up" TextKey="MOVEUP" OnClick="UpDown_Click" /><br />
                                        <rbfwebui:ImageButton ID="BottomDownBtn" runat="server" CommandArgument="bottomPane"
                                            CommandName="down" text="Move Down" TextKey="MOVEDOWN" OnClick="UpDown_Click" />&nbsp;&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap" valign="bottom">
                                        <rbfwebui:ImageButton ID="BottomEditBtn" runat="server" CommandArgument="bottomPane"
                                            CommandName="edit" text="Edit" TextKey="EDIT" OnClick="EditBtn_Click" /><br />
                                        <br />
                                        <rbfwebui:ImageButton ID="BottomDeleteBtn" runat="server" CommandArgument="bottomPane"
                                            CommandName="delete" text="Delete" TextKey="DELETE" OnClick="DeleteBtn_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
		</tr>
        <tr>
            <td align="center" class="Error" colspan="4">
                <rbfwebui:Localize ID="msgError" runat="server" Text="You do not have the appropriate permissions to delete or move this module"
                    TextKey="AM_NO_RIGHTS">
                </rbfwebui:Localize>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <hr noshade="noshade" size="1" />
                <rbfwebui:SettingsTable ID="EditTable" runat="server" OnUpdateControl="EditTable_UpdateControl" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <rbfwebui:LinkButton ID="updateButton" runat="server" class="CommandButton" Text="Apply Changes"
                    TextKey="APPLY_CHANGES"></rbfwebui:LinkButton>&nbsp;
                <rbfwebui:LinkButton ID="cancelButton" runat="server" class="CommandButton" Text="Cancel"
                    TextKey="CANCEL"></rbfwebui:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
