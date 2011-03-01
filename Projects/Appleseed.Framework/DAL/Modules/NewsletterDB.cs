using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mail;
using Appleseed.Framework.Site.Configuration;
using Appleseed.Framework.Helpers;
using Appleseed.Framework.Settings;
using Appleseed.Framework;
using Appleseed.Framework.Web.UI.WebControls;
using Appleseed.Framework.Data;
namespace Appleseed.Framework.Content.Data
{
    /// <summary>
    /// Class that encapsulates all data logic
    /// necessary to send newsletters.
    /// </summary>
    public class NewsletterDB
    {
        /// <summary>
        /// Get only the records with SendNewsletter enabled from the "Users" database table.
        /// Uses GetUsersNewsletter stored procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="MaxUsers">The max users.</param>
        /// <param name="MinSend">The min send.</param>
        /// <returns></returns>
        public SqlDataReader GetUsersNewsletter(int portalID, int MaxUsers, int MinSend) 
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetUsersNewsletter", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            SqlParameter parameterMaxUsers = new SqlParameter("@MaxUsers", SqlDbType.Int);
            parameterMaxUsers.Value = MaxUsers;
            myCommand.Parameters.Add(parameterMaxUsers);

            SqlParameter parameterMinSend = new SqlParameter("@MinSend", SqlDbType.Int);
            parameterMinSend.Value = MinSend;
            myCommand.Parameters.Add(parameterMinSend);

            // Open the database connection and execute the command
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // Return the datareader
            return dr;
        }

        /// <summary>
        /// The GetUsersNewsletterCount method returns the users count.
        /// Uses GetUsersNewsletter Stored Procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="MaxUsers">The max users.</param>
        /// <param name="MinSend">The min send.</param>
        /// <returns></returns>
        public int GetUsersNewsletterCount(int portalID, int MaxUsers, int MinSend) 
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_GetUsersNewsletter", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            SqlParameter parameterUserCount = new SqlParameter("@UserCount", SqlDbType.Int, 4);
            parameterUserCount.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterUserCount);

            SqlParameter parameterMaxUsers = new SqlParameter("@MaxUsers", SqlDbType.Int);
            parameterMaxUsers.Value = MaxUsers;
            myCommand.Parameters.Add(parameterMaxUsers);

            SqlParameter parameterMinSend = new SqlParameter("@MinSend", SqlDbType.Int);
            parameterMinSend.Value = MinSend;
            myCommand.Parameters.Add(parameterMinSend);

            // Open the database connection and execute the command
            myConnection.Open();
			try
			{
				myCommand.ExecuteNonQuery();
			}
			finally
			{
				myConnection.Close();
			}

            return (int)parameterUserCount.Value;
        }

        /// <summary>
        /// The SendNewsletterTo marks the provided email with the last
        /// send date, this avoids multiple send to the same email within specified frame.
        /// Uses SendNewsletterTo Stored Procedure.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="EMail">The E mail.</param>
        public void SendNewsletterTo(int portalID, string EMail) 
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = Config.SqlConnectionString;
            SqlCommand myCommand = new SqlCommand("rb_SendNewsletterTo", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterPortalID = new SqlParameter("@PortalID", SqlDbType.Int, 4);
            parameterPortalID.Value = portalID;
            myCommand.Parameters.Add(parameterPortalID);

            SqlParameter parameterEMail = new SqlParameter("@EMail", SqlDbType.NVarChar, 100);
            parameterEMail.Value = EMail;
            myCommand.Parameters.Add(parameterEMail);

            // Open the database connection and execute the command
            myConnection.Open();
			try
			{
				myCommand.ExecuteNonQuery();
			}
			finally
			{
				myConnection.Close();
			}
		}

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="From">From.</param>
        /// <param name="To">To.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Pwd">The PWD.</param>
        /// <param name="LoginPage">The login page.</param>
        /// <param name="Subject">The subject.</param>
        /// <param name="Body">The body.</param>
        /// <param name="Send">if set to <c>true</c> [send].</param>
        /// <param name="HtmlMode">if set to <c>true</c> [HTML mode].</param>
        /// <param name="breakLines">if set to <c>true</c> [break lines].</param>
        /// <returns></returns>
        public string SendMessage(string From, string To, string Name, string Pwd, string LoginPage, string Subject, string Body, bool Send, bool HtmlMode, bool breakLines)
        {
			// Obtain PortalSettings from Current Context
			PortalSettings portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];

            string LoginUrl;
            //If an alternate home is given use this
            if (LoginPage.Length > 0)
                LoginUrl = LoginPage + "?Usr=" + To + "&Pwd=" + Pwd;
            else
                LoginUrl = Path.WebPathCombine(Path.ApplicationFullPath, "/DesktopModules/Admin/Logon.aspx?Usr=" + To + "&Pwd=" + Pwd + "&Alias=" + portalSettings.PortalAlias);

			System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        	//MailFormat format;
            
            //Interprets TAGS
            //{NAME} = UserName
            //{PASSWORD} = Password
            //{EMAIL} = UserEmail
            //{LOGINURL} = A direct url that can be used to logon automatically
            Body = Body.Replace("{NAME}" , Name);
            Body = Body.Replace("{PASSWORD}" , Pwd);
            if (HtmlMode)
            {
				mail.IsBodyHtml = true;
                //format = MailFormat.Html;
                Body = Body.Replace("{EMAIL}", "<A Href=\"mailto:" + To + "\">" + To + "</A>");
                Body = Body.Replace("{LOGINURL}", "<A Href=\"" + LoginUrl + "\">" + LoginUrl + "</A>");
                
				//This option is useful is you type the text and you want to send as html.
				if (breakLines)
					Body = Body.Replace("\n", "<br>");
            }
            else
            {
				mail.IsBodyHtml = false;
                //format = MailFormat.Text;
                Body = Body.Replace("{EMAIL}", To);
                Body = Body.Replace("{LOGINURL}", LoginUrl);

                //Break rows - must be the last
                Body = ((HTMLText) Body).GetBreakedText(78);
            }

            // Send only if true
            if (Send)
            {
				mail.From = new System.Net.Mail.MailAddress(From);
				mail.To.Add(To);
				//MailMessage mail = new MailMessage();
				//mail.From = From;
				//mail.To = To;
                mail.Subject = Subject;
                mail.Body = Body;
                //mail.BodyFormat = format;
                mail.Priority = System.Net.Mail.MailPriority.Low;
				System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Config.SmtpServer);
				smtp.Send(mail);
				//SmtpMail.SmtpServer = Config.SmtpServer;     
				//SmtpMail.Send(mail);
            }
            return Body;
        }
    }
}
