<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegisterFull.ascx.cs" Inherits="DesktopModules_CoreModules_Register_RegisterFull" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<table border="0" cellpadding="0" cellspacing="0" class="registerTable">
        <tr>
        <td align="left" valign="top" width="30">
            &nbsp;
        </td>
        <td align="left" valign="top" width="661">
            <table border="0" cellpadding="0" cellspacing="0" width="661">
                <tr>
                    <td align="left" valign="top" width="10">
                        &nbsp;
                    </td>
                    <td align="left" class="textogrisinformacion" valign="top">
                        <p>
                            <span class="titulosofertas">
                                <asp:Label ID="lblTitle" runat="server"></asp:Label></span>
                        </p>
                        <asp:Panel ID="pnlForm" runat="server">
                            <table cellpadding="0" cellspacing="0" class="registerForm" style="padding-left: 30px; width:100%">
                                <tbody>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr class="registerRow">
                                        <td width="30%">
                                            <asp:Label ID="lblName" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, NAME%>" textkey="NAME"></asp:Label>
                                        </td>
                                        <td width="69%">
                                            <asp:TextBox ID="tfName" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredName" runat="server" ControlToValidate="tfName"
                                                        Display="Dynamic" ErrorMessage="<%$ Resources:Appleseed, MUST_ENTER_NAME%>" Text="You must enter a name" 
                                                        textkey="MUST_ENTER_NAME" Font-Size="11px"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblBirthDate" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, DATE_OF_BIRTH%>" 
                                                        textkey="DATE_OF_BIRTH"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:PlaceHolder ID="plhBirthDate" runat="server" />
                                            <%--<asp:DropDownList ID="ddlDay" runat="server">
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="ddlMonth" runat="server">
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="ddlYear" runat="server">
                                            </asp:DropDownList>--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCompany" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, COMPANY%>" textkey="COMPANY"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tfCompany" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPhone" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, TELEPHONE%>" 
                                            textkey="TELEPHONE"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tfPhone" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEmail" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, MAIL%>" textkey="MAIL"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tfEmail" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="tfEmail"
                                                  Display="Dynamic" ErrorMessage="MUST_ENTER_MAIL" Text="<%$ Resources:Appleseed, MUST_ENTER_MAIL%>" textkey="MUST_ENTER_MAIL" 
                                                  Font-Size="11px"/>
                                                
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPwd" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, PASSWORD%>" 
                                            textkey="PASSWORD"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="tfPwd" runat="server" TextMode="Password"></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="rfvPwd" runat="server" ControlToValidate="tfPwd" Display="Dynamic"
                                                Text="<%$ Resources:Appleseed, MUST_ENTER_PASSWORD%>"  textkey="MUST_ENTER_PASSWORD" Font-Size="11px"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblChPwd" runat="server" Font-Size="Smaller" Text="<%$ Resources:Appleseed, OVERWRITE_VALUE%>"
                                                textkey="OVERWRITE_VALUE" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCountry" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, COUNTRY%>" 
                                            textkey="COUNTRY"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList Width="70%" ID="ddlCountry" runat="server" DataTextField="Name"
                                                DataValueField="CountryID">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry" Display="Dynamic"
                                                Text="<%$ Resources:Appleseed, MUST_SELECT_COUNTRY%>" textkey="MUST_SELECT_COUNTRY" Font-Size="11px"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblReceiveNews" runat="server" CssClass="textogrisinformacion" Text="<%$ Resources:Appleseed, RECEIVE_NEWS%>" 
                                            textkey="RECEIVE_NEWS"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbReceiveNews" runat="server" Checked="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSendNotification" runat="server" CssClass="textogrisinformacion"
                                                Text="<%$ Resources:Appleseed, SEND_NOTIFICATION%>" textkey="SEND_NOTIFICATION" Visible="false"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chbSendNotification" runat="server" Checked="true" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr id="trCaptcha" runat="server">
                                        <td colspan="2">
                                            <recaptcha:RecaptchaControl
                                                  ID="recaptcha"
                                                  runat="server"
                                                  Theme="clean"
                                                  PrivateKey="some_private_key"
                                                  PublicKey="some_public_key"
                                                  />
                                            <div>
                                                <asp:CustomValidator ID="cvCaptcha" runat="server" Display="Dynamic"
                                                Text="<%$ Resources:Appleseed, USER_SAVING_WRONG_CAPTCHA %>" textkey="USER_SAVING_WRONG_CAPTCHA" Font-Size="11px"
                                                OnServerValidate="cvCaptcha_ServerValidate" ></asp:CustomValidator>
                                            </div>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td>
                                            <asp:Label ID="lblAssignCategory" runat="server" CssClass="textogrisinformacion"
                                                Text="Asignar a la categor&iacute;a:" Visible="false"></asp:Label>
                                        </td>
                                        <td width="200px">
                                            <asp:DropDownList ID="ddlAssignCategory" Visible="false" runat="server" Width="95%" />
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td style="padding-top: 10px;">
                                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Confirm" textkey="CONFIRM"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCategoryId" runat="server" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="pnlSuceeded" runat="server">
                            <asp:Label ID="lblSuceeded" runat="server"></asp:Label>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
