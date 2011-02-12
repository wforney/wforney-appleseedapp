<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.SendThoughts"
    language="c#" Codebehind="SendThoughts.ascx.cs" %>
<table border="0" cellpadding="4" cellspacing="0" width="98%">
    <tr valign="top">
        <td align="left">
            <rbfwebui:label id="Label1" runat="server" border="0"></rbfwebui:label>
            <br />
            <rbfwebui:label id="Label2" runat="server" border="0"></rbfwebui:label>
            <br />
            &nbsp;
        </td>
    </tr>
</table>
<br />
<asp:panel id="EditPanel" runat="server" visible="true">
    <div align="center">
        <table border="0" cellpadding="4" cellspacing="0" width="600">
            <tr valign="top">
                <td class="SubHead" width="200">
                    <rbfwebui:localize id="Literal1" runat="server" text="Your EMail Address:" textkey="SENDTHTS_YREMAIL">
                    </rbfwebui:localize></td>
                <td width="*">
                    <asp:textbox id="txtEMail" runat="server" columns="40" cssclass="NormalTextBox" maxlength="100"
                        width="450"></asp:textbox>
                    <div class="SubHead">
                        <asp:regularexpressionvalidator id="validEMailRegExp" runat="server" controltovalidate="txtEMail"
                            display="Dynamic" errormessage="Please enter a valid email address." textkey="SENDTHTS_EMAILVALID"
                            validationexpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:regularexpressionvalidator>
                        <asp:requiredfieldvalidator id="rfvEMail" runat="server" controltovalidate="txtEMail"
                            display="Dynamic" errormessage="'EMail' must not be left blank." textkey="SENDTHTS_EMAILBLANK"></asp:requiredfieldvalidator></div>
                </td>
            </tr>
            <tr valign="top">
                <td class="SubHead" width="200">
                    <rbfwebui:localize id="Literal2" runat="server" text="Your Name: (optional)" textkey="SENDTHTS_YRNAME">
                    </rbfwebui:localize></td>
                
                <td width="*">
                    <asp:textbox id="txtName" runat="server" columns="40" cssclass="NormalTextBox" maxlength="100"
                        width="450"></asp:textbox></td>
            </tr>
            <tr valign="top">
                <td class="SubHead">
                    <rbfwebui:localize id="Literal3" runat="server" text="Subject: (optional)" textkey="SENDTHTS_SUBJECT">
                    </rbfwebui:localize></td>
                
                <td width="*">
                    <asp:textbox id="txtSubject" runat="server" columns="40" cssclass="NormalTextBox"
                        maxlength="100" width="450"></asp:textbox></td>
            </tr>
            <tr valign="top">
                <td class="SubHead">
                    <rbfwebui:localize id="Literal4" runat="server" text="Message Body:" textkey="SENDTHTS_BODY">
                    </rbfwebui:localize></td>
                <td width="*">
                    <asp:textbox id="txtBody" runat="server" columns="59" cssclass="NormalTextBox" rows="15"
                        textmode="Multiline" width="450"></asp:textbox>
                    <div class="SubHead">
                        <asp:requiredfieldvalidator id="rfvMessageBody" runat="server" controltovalidate="txtBody"
                            display="Dynamic" errormessage="Please enter message text." textkey="SENDTHTS_BODY_ERR"></asp:requiredfieldvalidator></div>
                </td>
            </tr>
            <tr valign="top">
                <td>
                    &nbsp;
                </td>
                <td>
                    <rbfwebui:linkbutton id="SendBtn" runat="server" class="CommandButton" onclick="SendBtn_Click"
                        text="Send" textkey="SENDTHTS_SEND"></rbfwebui:linkbutton>&nbsp;
                    <rbfwebui:linkbutton id="ClearBtn" runat="server" causesvalidation="False" class="CommandButton"
                        onclick="ClearBtn_Click" text="Clear" textkey="SENDTHTS_CLEAR"></rbfwebui:linkbutton>&nbsp;
                </td>
            </tr>
        </table>
    </div>
</asp:panel>
