<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.QuizPage"
    language="c#" Codebehind="QuizPage.aspx.cs" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div class="rb_DefaultLayoutDiv">
            <table class="rb_DefaultLayoutTable">
                <tr valign="top">
                    <td class="rb_DefaultPortalHeader">
                        <portal:banner id="SiteHeader" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <br/>
                        <h2>
                            Quiz:&nbsp;<rbfwebui:label id="lblQuiz" runat="server">
                            </rbfwebui:label></h2>
                        <div id="QuizScreen" runat="server">
                            <table border="0" cellpadding="2" cellspacing="0" width="50%">
                                <tr>
                                    <td align="center">
                                        <b>
                                            <rbfwebui:label id="lblQuestion" runat="server">
                                            </rbfwebui:label></b><br/>
                                        <asp:radiobuttonlist id="rblAnswer" runat="server" repeatdirection="vertical" repeatlayout="table"
                                            textalign="right">
                                        </asp:radiobuttonlist><br/>
                                        <asp:requiredfieldvalidator id="Requiredfieldvalidator1" runat="server" controltovalidate="rblAnswer"
                                            errormessage="Please pick an answer!">
                                        </asp:requiredfieldvalidator><br/>
                                        <rbfwebui:button id="btnSubmit" runat="server" class="CommandButton" onclick="btnSubmit_Click"
                                            text=" Next >> " />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <b>
                                            <br/>
                                            Total questions:
                                            <rbfwebui:label id="lblTotalQuestion" runat="server">
                                            </rbfwebui:label>
                                            &nbsp;&nbsp;&nbsp; Time spent:
                                            <rbfwebui:label id="lblTimeSpent" runat="server">
                                            </rbfwebui:label>
                                        </b>
                                    </td>
                                </tr>
                            </table>
                        </div><span id="ResultScreen" runat="server">
                            <rbfwebui:label id="lblResult" runat="server">
                            </rbfwebui:label>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="rb_DefaultPortalFooter">
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
