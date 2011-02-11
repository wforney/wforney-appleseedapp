<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.WeatherDEEdit"
    language="c#" Codebehind="WeatherDEEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div class="rb_AlternateLayoutDiv">
            <table class="rb_AlternateLayoutTable">
                <tr valign="top">
                    <td class="rb_AlternatePortalHeader" valign="top">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br/>
                        <table border="0" cellpadding="4" cellspacing="0" width="98%">
                            <tr valign="top">
                                <td width="150">
                                    &nbsp;
                                </td>
                                <td width="*">
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr>
                                            <td align="left" class="Head" height="18">
                                                German Weather information obtained from '
                                                <rbfwebui:hyperlink id="HyperLink" runat="server" navigateurl="http://wetter.com"
                                                    target="_blank">Wetter.com</rbfwebui:hyperlink>'
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr noshade="noshade" size="1" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                Zip code:
                                            </td>
                                            <td rowspan="3">
                                                &nbsp;
                                            </td>
                                            <td class="Normal">
                                                <asp:textbox id="WeatherZip" runat="server" columns="30" cssclass="NormalTextBox"
                                                    width="390">
                                                </asp:textbox>
                                                <div class="SubHead">
                                                    <asp:requiredfieldvalidator id="rfvWeatherZip" runat="server" controltovalidate="WeatherZip"
                                                        display="Dynamic" errormessage="'Zip' must not be left blank.">
                                                    </asp:requiredfieldvalidator>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                City index:
                                            </td>
                                            <td rowspan="3">
                                                &nbsp;
                                            </td>
                                            <td class="Normal">
                                                <asp:textbox id="WeatherCityIndex" runat="server" columns="30" cssclass="NormalTextBox"
                                                    width="390">
												0</asp:textbox>
                                                <div class="SubHead">
                                                    <asp:requiredfieldvalidator id="rfvWeatherCityIndex" runat="server" controltovalidate="WeatherCityIndex"
                                                        display="Dynamic" errormessage="'CityIndex' must be a numer."></asp:requiredfieldvalidator>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                Setting:
                                            </td>
                                            <td rowspan="3">
                                                &nbsp;
                                            </td>
                                            <td class="Normal">
                                                <div class="SubHead">
                                                    <asp:radiobuttonlist id="WeatherSetting" runat="server" repeatdirection="Horizontal"
                                                        width="390px">
                                                        <asp:listitem selected="True" value="0">Today</asp:listitem>
                                                        <asp:listitem value="1">Forecast</asp:listitem>
                                                    </asp:radiobuttonlist>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" cellspacing="0" width="500">
                                        <tr valign="top">
                                            <td class="SubHead" width="100">
                                                Design:
                                            </td>
                                            <td rowspan="3">
                                                &nbsp;
                                            </td>
                                            <td class="Normal">
                                                <div class="SubHead">
                                                    <asp:radiobuttonlist id="WeatherDesign" runat="server" width="390px">
                                                        <asp:listitem selected="True" value="1">Size: 150 X 90 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;transparent backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;black font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                        <asp:listitem value="1b">Size: 150 X 90 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;transparent backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;white font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                        <asp:listitem value="1c">Size: 130 X 113 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;transparent backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;black font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                        <asp:listitem value="2">Size: 130 X 113 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;orange backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;black font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                        <asp:listitem value="2b">Size: 150 X 94 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;orange backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;black font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                        <asp:listitem value="3">Size: 150 X 90 px
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;blue backgroung
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;black font
																		&lt;br&gt;&#160;&#160;&#160;&#160;&#160;&#160;Format: PNG
                                                        </asp:listitem>
                                                    </asp:radiobuttonlist>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <p>
                                        <asp:placeholder id="PlaceHolderButtons" runat="server"></asp:placeholder>
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
