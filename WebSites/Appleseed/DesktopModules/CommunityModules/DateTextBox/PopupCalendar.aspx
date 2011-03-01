<%@ page autoeventwireup="false" clienttarget="Uplevel" inherits="PeterBlum.DateTextBoxWebForms.PopupCalendar"
    language="c#" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="Form1" runat="server">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="center" valign="middle">
                    <asp:calendar id="Calendar1" runat="server" backcolor="#ffffff" bordercolor="#000000"
                        daynameformat="FirstTwoLetters" firstdayofweek="Monday" font-names="Verdana,Arial"
                        font-size="8pt" forecolor="#00000" height="60" ondayrender="Calendar1_DayRender"
                        onselectionchanged="Calendar1_SelectionChanged" selectionmode="Day" showtitle="true"
                        width="150">
                        <nextprevstyle backcolor="Navy" forecolor="White" />
                        <titlestyle backcolor="Navy" forecolor="White" />
                        <othermonthdaystyle forecolor="Silver" />
                    </asp:calendar>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
