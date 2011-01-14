<%@ Control Language="c#" %>
<script runat="server">
    private void Page_Load(object sender, System.EventArgs e)
    {
        ThreePanes.DataBind();
    }
</script>
<rbfwebui:DESKTOPPANES id="ThreePanes" height="100%" ShowLogon="False" ShowLastSeparator="False" ShowFirstSeparator="False" Cellpadding="0" cellspacing="0" runat="server">
    <LEFTPANETEMPLATE>
        <rbfwebui:MENUNAVIGATION id="NavigationMenu" runat="server" Height="20" TopPaddng="1" StartTop="0" StartLeft="0" Width="185px" BorderWidth="0" ImagesPath="/design/Themes/default/img" Bind="BindOptionCurrentChilds" AutoShopDetect="True" UseTabNameInUrl="True" AutoBind="True" LeftPaddng="5" Horizontal="False" visible="True">
            <CONTROLITEMSTYLE cssclass="MenuItem"></CONTROLITEMSTYLE>
            <CONTROLSUBSTYLE cssclass="MenuSub"></CONTROLSUBSTYLE>
            <CONTROLHISTYLE cssclass="MenuItemHi"></CONTROLHISTYLE>
            <CONTROLHISUBSTYLE cssclass="MenuSubHi"></CONTROLHISUBSTYLE>
            <ARROWIMAGE height="9px" width="7px" imageurl="arrow.gif"></ARROWIMAGE>
            <ARROWIMAGELEFT height="9px" width="5px" imageurl="arrow_left.gif"></ARROWIMAGELEFT>
            <ARROWIMAGEDOWN height="5px" width="10px" imageurl="arrow_down.gif"></ARROWIMAGEDOWN></rbfwebui:MENUNAVIGATION>
    </LEFTPANETEMPLATE>
    <LEFTPANESTYLE cssclass="LeftPane" width="190px" verticalalign="Top"></LEFTPANESTYLE>
    <CONTENTPANESTYLE cssclass="ContentPane" verticalalign="Top" horizontalalign="left"></CONTENTPANESTYLE>
    <RIGHTPANESTYLE cssclass="RightPane" verticalalign="Top"></RIGHTPANESTYLE>
</rbfwebui:DESKTOPPANES>
