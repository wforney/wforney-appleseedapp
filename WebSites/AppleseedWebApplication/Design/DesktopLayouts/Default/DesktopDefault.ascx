<%@ Control Language="c#" %>
<%@ Register TagPrefix="pane" TagName="DesktopThreePanes" Src="DesktopThreePanes.ascx" %>
<%@ Register TagPrefix="head" TagName="Banner" Src="DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="DesktopFooter.ascx" %>
<table border="0" cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <td align="center">
            <table>
                <tr>
                    <td align="left">
                        <div class="rb_DefaultLayoutDiv">
                            <table class="rb_DefaultLayoutTable" cellspacing="0" cellpadding="0" id="Table1"
                                width="979px">
                                <tbody>
                                    <tr valign="top">
                                        <td class="rb_DefaultPortalHeader" valign="top">
                                            <head:Banner ID="Banner" SelectedTabIndex="0" runat="server"></head:Banner>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <pane:DesktopThreePanes ID="DesktopThreePanes1" runat="server"></pane:DesktopThreePanes>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="rb_DefaultPortalFooter">
                                            <foot:Footer ID="Footer" runat="server"></foot:Footer>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
