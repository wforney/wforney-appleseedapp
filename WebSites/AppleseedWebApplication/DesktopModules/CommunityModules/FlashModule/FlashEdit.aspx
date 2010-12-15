<%@ Page AutoEventWireup="false" Inherits="Appleseed.Content.Web.Modules.FlashEdit"
    Language="c#" MasterPageFile="~/Shared/SiteMasterDefault.master" Codebehind="FlashEdit.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <div class="div_ev_Table">
        <table cellpadding="0" cellspacing="0" width="80%">
            <tr>
                <td align="left" class="Head">
                    <rbfwebui:Localize ID="Literal2" runat="server" Text="Flash Settings" TextKey="FLASH_SETTINGS">
                    </rbfwebui:Localize>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr noshade="noshade" size="1" />
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="80%">
            <tr valign="top">
                <td class="SubHead" width="100">
                    <rbfwebui:Localize ID="Literal1" runat="server" Text="Swf-File Path" TextKey="FLASH_PATH">
                    </rbfwebui:Localize>
                </td>
                <td rowspan="3" width="251">
                    &nbsp;
                </td>
                <td class="Normal">
                    <asp:TextBox ID="Src" runat="server" Columns="30" CssClass="NormalTextBox" Width="390"></asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead">
                    <rbfwebui:Localize ID="Literal3" runat="server" Text="Width" TextKey="WIDTH">
                    </rbfwebui:Localize>
                </td>
                <td>
                    <asp:TextBox ID="Width" runat="server" Columns="30" CssClass="NormalTextBox" Width="390">
                    </asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead">
                    <rbfwebui:Localize ID="Literal4" runat="server" Text="Height" TextKey="HEIGHT">
                    </rbfwebui:Localize>
                </td>
                <td>
                    <asp:TextBox ID="Height" runat="server" Columns="30" CssClass="NormalTextBox" Width="390">
                    </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="SubHead" width="100">
                    <rbfwebui:Localize ID="Literal5" runat="server" Text="Background Color" TextKey="FLASH_BACKGROUNDCOLOR">
                    </rbfwebui:Localize>
                    (rrggbb)
                </td>
                <td rowspan="3" width="251">
                    &nbsp;
                </td>
                <td class="Normal">
                    <asp:TextBox ID="BackgroundCol" runat="server" Columns="30" CssClass="NormalTextBox"
                        Width="390"></asp:TextBox>
                </td>
            </tr>
        </table>
        <p>
            <rbfwebui:LinkButton ID="updateButton" runat="server" class="CommandButton" Text="Update"
                TextKey="UPDATE">Update</rbfwebui:LinkButton>
            &nbsp;
            <rbfwebui:LinkButton ID="cancelButton" runat="server" CausesValidation="False" class="CommandButton"
                Text="Cancel" TextKey="CANCEL">Cancel</rbfwebui:LinkButton>&nbsp;
            <rbfwebui:HyperLink ID="showGalleryButton" runat="server" CssClass="CommandButton"
                Text="Show Gallery" TextKey="SHOW_FLASH_GALLERY">Show Gallery</rbfwebui:HyperLink>
        </p>
    </div>
</asp:Content>
