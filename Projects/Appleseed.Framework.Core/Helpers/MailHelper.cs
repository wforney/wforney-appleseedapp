using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Security.Principal;
using System.Web;
using System.Web.Mail;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Exceptions;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Site.Configuration;
using MailMessage=System.Net.Mail.MailMessage;

namespace Appleseed.Framework.Helpers
{
    /// <summary>
    /// This class contains functions for mailing to 
    /// Appleseed users
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// Gets the email addresses in roles.
        /// </summary>
        /// <param name="Roles">The roles.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>A string[] value...</returns>
        public static string[] GetEmailAddressesInRoles(string[] Roles, int portalID)
        {
            if (Config.UseSingleUserBase) portalID = 0;

            if (HttpContext.Current.User is WindowsPrincipal)
            {
                ArrayList addresses = new ArrayList();

                for (int i = 0; i < Roles.Length; i++)
                {
                    string account = Roles[i];
                    EmailAddressList eal = ADHelper.GetEmailAddresses(account);

                    for (int j = 0; j < eal.Count; j++)

                        if (!addresses.Contains(eal[j]))
                            addresses.Add(eal[j]);
                }
                return (string[]) addresses.ToArray(typeof (string));
            }

            else
            {
                // No roles --> no email addresses
                if (Roles.Length == 0)
                    return new string[0];
                // Build the sql select
                string[] adaptedRoles = new string[Roles.Length];

                for (int i = 0; i < Roles.Length; i++)
                    adaptedRoles[i] = Roles[i].Replace("'", "''");
                string delimitedRoleList = "N'" + string.Join("', N'", adaptedRoles) + "'";
                string sql = "SELECT DISTINCT rb_Users.Email " +
                             "FROM rb_UserRoles INNER JOIN " +
                             " rb_Users ON rb_UserRoles.UserID = rb_Users.UserID INNER JOIN " +
                             " rb_Roles ON rb_UserRoles.RoleID = rb_Roles.RoleID " +
                             "WHERE (rb_Users.PortalID = " + portalID.ToString() + ") " +
                             " AND (rb_Roles.RoleName IN (" + delimitedRoleList + "))";
                // Execute the sql
                EmailAddressList eal = new EmailAddressList();
                IDataReader myReader = DBHelper.GetDataReader(sql);

                try
                {
                    while (myReader.Read())
                    {
                        if (!myReader.IsDBNull(0))

                            try
                            {
                                string email = myReader.GetString(0);

                                if (email.Trim().Length != 0)
                                    eal.Add(email);
                            }

                            catch
                            {
                            }
                    }
                }

                finally
                {
                    myReader.Close();
                }
                // Return the result
                return (string[]) eal.ToArray(typeof (string));
            }
        }

        /// <summary>
        /// This function return's the email address of the current logged on user.
        /// If its email-address is not valid or not found,
        /// then an empty string is returned.
        /// </summary>
        /// <returns>string</returns>
        public static string GetCurrentUserEmailAddress()
        {
            return GetCurrentUserEmailAddress(string.Empty);
        }

        /// <summary>
        /// Gets the current user email address.
        /// </summary>
        /// <param name="Validated">if set to <c>true</c> [validated].</param>
        /// <returns>A string value...</returns>
        public static string GetCurrentUserEmailAddress(bool Validated)
        {
            return GetCurrentUserEmailAddress(string.Empty, Validated);
        }

        /// <summary>
        /// Gets the current user email address.
        /// </summary>
        /// <param name="DefaultEmail">The default email.</param>
        /// <returns>A string value...</returns>
        public static string GetCurrentUserEmailAddress(string DefaultEmail)
        {
            return GetCurrentUserEmailAddress(DefaultEmail, true);
        }

        /// <summary>
        /// This function return's the email address of the current logged on user.
        /// If its email-address is not valid or not found,
        /// then the Default address is returned
        /// </summary>
        /// <param name="DefaultEmail">The default email.</param>
        /// <param name="Validated">if set to <c>true</c> [validated].</param>
        /// <returns></returns>
        public static string GetCurrentUserEmailAddress(string DefaultEmail, bool Validated)
        {
            if (HttpContext.Current.User is WindowsPrincipal)
            {
                // windows user
                EmailAddressList eal = ADHelper.GetEmailAddresses(HttpContext.Current.User.Identity.Name);

                if (eal.Count == 0)
                    return DefaultEmail;

                else
                    return (string) eal[0];
            }

            else
            {
                // Get the logged on email address from the context
                //string email = System.Web.HttpContext.Current.User.Identity.Name;
                string email = PortalSettings.CurrentUser.Identity.Email;

                if (!Validated)
                    return email;
                // Check if its email address is valid
                EmailAddressList eal = new EmailAddressList();

                try
                {
                    eal.Add(email);
                    return email;
                }

                catch
                {
                    return DefaultEmail;
                }
            }
        }

        // by Rob Siera 4 dec 2004
        // modified again by Bill Forney 4 dec 2004
        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        public static void SendMailNoAttachment(string From, string sendTo, string Subject, string Body, string CC,
                                                string BCC, string SMTPServer)
        {
            SendEMail(From, sendTo, Subject, Body, CC, BCC, SMTPServer);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="AttachmentFile">Optional attachment file name</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        public static void SendMailOneAttachment(string From, string sendTo, string Subject, string Body,
                                                 string AttachmentFile, string CC, string BCC, string SMTPServer)
        {
            SendEMail(From, sendTo, Subject, Body, AttachmentFile, CC, BCC, SMTPServer);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        public static void SendMailMultipleAttachments(string From, string sendTo, string Subject, string Body,
                                                       ArrayList AttachmentFiles, string CC, string BCC,
                                                       string SMTPServer)
        {
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC, SMTPServer);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        public static void SendEMail(string From, string sendTo, string Subject, string Body, string CC, string BCC,
                                     string SMTPServer)
        {
            ArrayList AttachmentFiles;
            AttachmentFiles = null;
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC, SMTPServer);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        /// <param name="AttachmentFile">Optional attachment file name</param>
        public static void SendEMail(string From, string sendTo, string Subject, string Body, string AttachmentFile,
                                     string CC, string BCC, string SMTPServer)
        {
            ArrayList AttachmentFiles = new ArrayList();

            if (AttachmentFile != null && AttachmentFile.Length != 0)
            {
                AttachmentFiles.Add(AttachmentFile);
            }

            else
            {
                AttachmentFiles = null;
            }
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC, SMTPServer);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        public static void SendEMail(string From, string sendTo, string Subject, string Body, ArrayList AttachmentFiles,
                                     string CC, string BCC, string SMTPServer)
        {
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC, SMTPServer, MailFormat.Text);
        }

        /// <summary>
        /// Sends an email to specified address.
        /// </summary>
        /// <param name="From">Email address from</param>
        /// <param name="sendTo">Email address to</param>
        /// <param name="Subject">Email subject line</param>
        /// <param name="Body">Email body content</param>
        /// <param name="AttachmentFiles">Optional, list of attachment file names in form of an array list</param>
        /// <param name="CC">Email carbon copy to</param>
        /// <param name="BCC">Email blind carbon copy to</param>
        /// <param name="SMTPServer">SMTP Server to send mail thru (optional, if not specified local machine is used)</param>
        /// <param name="mf">Optional, mail format (text/html)</param>
        public static void SendEMail(string From, string sendTo, string Subject, string Body, ArrayList AttachmentFiles,
                                     string CC, string BCC, string SMTPServer, MailFormat mf)
        {
            MailMessage myMessage;

            try
            {
                myMessage = new MailMessage();
                myMessage.To.Add(sendTo);
                myMessage.From = new MailAddress(From);
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = (mf == MailFormat.Html ? true : false);

                if (CC.Length != 0) myMessage.CC.Add(CC);

                if (BCC.Length != 0) myMessage.Bcc.Add(BCC);

                if (AttachmentFiles != null)
                {
                    foreach (string x in AttachmentFiles)
                    {
                        if (File.Exists(x)) myMessage.Attachments.Add(new Attachment(x));
                    }
                }

                if (SMTPServer.Length != 0)
                {
                    SmtpClient smtp = new SmtpClient(SMTPServer);

                    smtp.Send(myMessage);
                }
                else
                {
                    throw new AppleseedException("SMTPServer configuration error");
                }
            }

            catch (Exception myexp)
            {
                throw myexp;
            }
        }

        /// <summary>
        /// It writes emailaddresse only, if javascript is enabled (needs a really user-agent),
        /// if not, an address like meyert[at]geschichte.hu-berlin.de is returned.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string FormatEmail(string email)
        {
            string user = email.Substring(0, email.IndexOf('@'));
            string dom = email.Substring(email.IndexOf('@') + 1);
            return
                "<script language=\"javascript\">var name = \"" + user + "\"; var domain = \"" + dom +
                "\"; document.write('<a href=\"mailto:' + name + String.fromCharCode(64) + domain + '\">' + name + String.fromCharCode(64) + domain + '</a>')</script><noscript>" +
                user + " at " + dom + "</noscript>";
        }
    }
}