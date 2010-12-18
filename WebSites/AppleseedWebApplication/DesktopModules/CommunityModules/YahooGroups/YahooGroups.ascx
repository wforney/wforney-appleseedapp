<%@ control inherits="Appleseed.Content.Web.Modules.OneFileModule" language="c#" %>
<%@ import namespace="System.Web.Mail" %>
<%@ import namespace="Appleseed.Framework.Web.UI" %>
<%@ import namespace="Appleseed.Framework.Web.UI.WebControls" %>
<%@ import namespace="Appleseed.Framework.Site.Configuration" %>

<script runat="server" language="C#">

    string yahooGroupName;
    string yahooServerName = "yahoogroups.com";


    void Page_Load(Object sender, EventArgs e)
    {
        InitSettings(SettingsType.Str);
        // Note you have these variables available everywhere in your code:
        // string SettingsStr  -- The content of setting "Settings string"
        // bool DebugMode      -- true if setting "Debug Mode" is clicked
        // bool SettingsExists -- false if settings are missing
        // string GetStrSetting(settingName) -- Returns the setting from SettingsStr
        // string GetXmlSetting(settingName) -- Returns the setting from XML file
        // string GetSetting(settingName)    -- Returns the setting in search order: 
        //                                      1)SettingsStr, 2)XML file

        if (SettingsExists)
        {
            yahooGroupName = GetSetting("YahooGroupName");

            if (DebugMode)
                Message.Text = "Debug info: " + yahooGroupName + " - " + yahooServerName;
        }

        email.Text = PortalSettings.CurrentUser.Identity.Email;
    }

    void SubscribeBtn_Click(Object sender, EventArgs e)
    {
        JoinList(email.Text, yahooGroupName);
        Message.Text = email.Text + " subscribed!";
    }

    void LeaveBtn_Click(Object sender, EventArgs e)
    {
        LeaveList(email.Text, yahooGroupName);
        Message.Text = email.Text + " unsubscribed!";
    }

    void JoinList(string email, string listname)
    {
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress(email);
        mail.To.Add(listname + "-subscribe " + yahooServerName);
        mail.Subject = "subscribe";
        mail.Body = "subscribe";
        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Appleseed.Framework.Settings.Config.SmtpServer);
        smtp.Send(mail);

        //MailMessage Mailer = new MailMessage();
        //Mailer.From = email;
        //Mailer.To = listname + "-subscribe " + yahooServerName;
        //Mailer.Subject = "subscribe";
        //Mailer.Body = "subscribe";
        //SmtpMail.SmtpServer = Appleseed.Framework.Settings.Config.SmtpServer;
        //SmtpMail.Send(Mailer);
    }

    void LeaveList(string email, string listname)
    {
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress(email);
        mail.To.Add(listname + "-unsubscribe " + yahooServerName);
        mail.Subject = "unsubscribe";
        mail.Body = "unsubscribe";
        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Appleseed.Framework.Settings.Config.SmtpServer);
        smtp.Send(mail);

        //MailMessage Mailer = new MailMessage();
        //Mailer.From = email;
        //Mailer.To = listname + "-unsubscribe " + yahooServerName;
        //Mailer.Subject = "unubscribe";
        //Mailer.Body = "unsubscribe";
        //SmtpMail.SmtpServer = Appleseed.Framework.Settings.Config.SmtpServer;
        //SmtpMail.Send(Mailer);
    }
</script>

<rbfwebui:desktopmoduletitle id="ModuleTitle" runat="server" edittext="Edit" editurl="~/DesktopModules/CoreModules/Admin/PropertyPage.aspx"
    propertiestext="PROPERTIES" propertiesurl="~/DesktopModules/CoreModules/Admin/PropertyPage.aspx">
</rbfwebui:desktopmoduletitle>
<hr noshade="noshade" size="1pt" width="98%" />
<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <span class="SubSubHead" style="height: 20">YahooGroups Signup:</span></td>
    </tr>
    <tr>
        <td>
            <span class="Normal">Email:</span></td>
    </tr>
    <tr>
        <td>
            <asp:textbox id="email" runat="server" columns="9" cssclass="NormalTextBox" width="130">
            </asp:textbox>
            <div class="SubHead">
                <asp:regularexpressionvalidator id="validEMailRegExp" runat="server" controltovalidate="email"
                    display="Dynamic" errormessage="Please enter a valid email address." validationexpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+">
                </asp:regularexpressionvalidator>
                <asp:requiredfieldvalidator id="rfvEMail" runat="server" controltovalidate="email"
                    display="Dynamic" errormessage="'Email' must not be left blank.">
                </asp:requiredfieldvalidator>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <rbfwebui:button id="SubscribeBtn" runat="server" onclick="SubscribeBtn_Click" text="Join" />
            <rbfwebui:button id="LeaveBtn" runat="server" onclick="LeaveBtn_Click" text="Leave" />
        </td>
    </tr>
    <tr>
        <td>
            <rbfwebui:label id="Message" runat="server" cssclass="NormalRed">
            </rbfwebui:label>
        </td>
    </tr>
</table>
