<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.FAQsEdit"
    language="c#" Codebehind="FAQsEdit.aspx.cs" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr>
                        <td align="left" class="Head">
                            <rbfwebui:localize id="Literal1" runat="server" text="FAQ Details" textkey="FAQ_DETAILS">
                            </rbfwebui:localize>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="2" cellspacing="0"  width="749">
                    <tr>
                        <td class="SubHead" valign="top" width="100">
                            <rbfwebui:localize id="Literal2" runat="server" text="Question" textkey="FAQ_QUESTION">
                            </rbfwebui:localize>
                        </td>
                        <td>
                            <asp:textbox id="Question" runat="server" cssclass="NormalTextBox" height="60px"
                                maxlength="500" textmode="MultiLine" width="401px"></asp:textbox>
                        </td>
                        <td class="Normal" width="266">
                            <asp:requiredfieldvalidator id="RequiredFieldValidatorQuestion" runat="server" controltovalidate="Question"
                                errormessage="Please enter a question!" textkey="FAQ_QUESTION_ERR"></asp:requiredfieldvalidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="SubHead" valign="top" width="100">
                            <rbfwebui:localize id="Literal3" runat="server" text="Answer" textkey="FAQ_ANSWER">
                            </rbfwebui:localize>
                        </td>
                        <td>
                            <asp:placeholder id="PlaceHolderHTMLEditor" runat="server"></asp:placeholder>
                        </td>
                        <td class="Normal" width="266">
                            &nbsp;
                        </td>
                    </tr>
                </table>
                
                    <rbfwebui:linkbutton id="UpdateButton" runat="server" class="CommandButton" text="Update">
                    </rbfwebui:linkbutton>
                    &nbsp;
                    <rbfwebui:linkbutton id="CancelButton" runat="server" causesvalidation="False" class="CommandButton"
                        text="Cancel">
                    </rbfwebui:linkbutton>
                    &nbsp;
                    <rbfwebui:linkbutton id="DeleteButton" runat="server" causesvalidation="False" class="CommandButton"
                        text="Delete this item">
                    </rbfwebui:linkbutton>
                    <br />
                    <hr noshade="noshade" size="1" width="90%" />
                    <span class="Normal">
                        <rbfwebui:localize id="CreatedLabel" runat="server" text="Created by" textkey="CREATED_BY">
                        </rbfwebui:localize>&nbsp;
                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                        <rbfwebui:localize id="OnLabel" runat="server" text="on" textkey="ON">
                        </rbfwebui:localize>&nbsp;
                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label>
                    </span>
                
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
