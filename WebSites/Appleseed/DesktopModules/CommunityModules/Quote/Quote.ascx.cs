using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web;
using Appleseed.Framework;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Web.UI.WebControls;

namespace Appleseed.Content.Web.Modules
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// Display some text from a xml file. 
    /// </summary>
    public partial class Quote : PortalModuleControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            bool MyQuote = "My Quote" == Settings["Quote source"].ToString();
            string textSize = Settings["Text size"].ToString();
            bool displayInItalic = "True" == Settings["Display in italic"].ToString();
            bool displayInBold = "True" == Settings["Display in bold"].ToString();
            string startTag = Settings["Start tag"].ToString();
            string endTag = Settings["End tag"].ToString();
            string quoteText;

            if (MyQuote)
            {
                quoteText = Settings["My Quote"].ToString();
            }
            else
            {
                Random objRan = new Random();
                ArrayList col = new ArrayList();
                string quoteFile = Settings["Quote file"].ToString();

                if (ConfigurationManager.AppSettings.Get("QuoteFileFolder") != null)
                {
                    if (ConfigurationManager.AppSettings.Get("QuoteFileFolder").ToString().Length > 0)
                        quoteFile = ConfigurationManager.AppSettings.Get("QuoteFileFolder").ToString() + quoteFile;
                    else
                        quoteFile = "~/DesktopModules/CommunityModules/Quote/" + quoteFile;
                }
                else
                {
                    quoteFile = "~/DesktopModules/CommunityModules/Quote/" + quoteFile;
                }

                if (!quoteFile.EndsWith(".quote"))
                    quoteFile += ".quote";

                if (File.Exists(Server.MapPath(quoteFile)))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(Server.MapPath(quoteFile)))
                        {
                            String line;
                            while ((line = sr.ReadLine()) != null)
                                col.Add(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        col.Add("Problems reading quotes file! <br> --- Jakob Hansen");
                        ErrorHandler.Publish(LogLevel.Error, "Problems reading Quotes file.", ex);
                    }
                }
                else
                {
                    col.Add("Quotes file missing! <br> --- Jakob Hansen"); // hehe...
                }

                /* These are now in file demo.quote
                col.Add("Service is the rent we pay for being. It is the very purpose of life, and not something you do in your spare time. <br> --- Marion Wright Edelman");
                col.Add("You must be the change you wish to see in the world. <br> --- Mahatma Ghandi");
                col.Add("Make others happy and joyful. Your happiness will multiply a thousand fold. <br> --- Swami Sivananda");
                col.Add("The influence of each human being on others in this life is a kind of immortality. <br> --- John Quincy Adams");
                col.Add("Love sought is good, but given unsought is better. <br> --- Shakespeare");
                col.Add("Here is a test to find out whether your mission in life is complete. If you're alive, it isn't. <br> --- Richard Bach");
                col.Add("There is no such thing in anyone's life as an unimportant day. <br> --- Alexander Woollcott");
                col.Add("How far you go in life depends on your being tender with the young, compassionate with the aged, sympathetic with the striving and tolerant of the weak and the strong. Because someday in life you will have been all of these. <br> --- George Washington Carver");
                col.Add("People rarely succeed unless they have fun in what they are doing. <br> --- Dale Carnegie");
                col.Add("Sit on a baby's bib and SPIT HAPPENS <br> --- Anonymous");
                col.Add("May the smile on your face Come straight from your heart <br> --- Anonymous");
                col.Add("Most good judgment comes from experience. Most experience comes from bad judgment. <br> --- Anonymous");
                col.Add("The true \"final frontier\" is in the minds and the will of people. <br> -- Gen. Michael E. Ryan, U.S. Air Force Chief of Staff");
                col.Add("I don't pretend to understand the Universe - it's a great deal bigger than I am. <br> -- Thomas Carlyle");
                col.Add("When we try to pick out anything else in the Universe, we find it hitched to everything else in the Universe. <br> -- John Muir");
                col.Add("Not all who wander are lost. <br> -- Tolkien");
                */

                quoteText = (string) col[objRan.Next(col.Count)];
            }

            if (displayInItalic)
                quoteText = "<i>" + quoteText + "</i>";
            if (displayInBold)
                quoteText = "<b>" + quoteText + "</b>";
            if (textSize != "Default")
                quoteText = "<H" + textSize[1] + ">" + quoteText + "</H" + textSize[1] + ">";
            if (startTag != "")
                quoteText = startTag + quoteText;
            if (endTag != "")
                quoteText += endTag;

            lblQuote.Text = quoteText;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Quote"/> class.
        /// </summary>
        public Quote()
        {
            var setQuoteSource =
                new SettingItem<string, ListControl>(new ListDataType<string, ListControl>("File;My Quote"))
                    {
                        Value = "File",
                        Order = 1,
                        EnglishName = "Quote source?",
                        Description = "Get quotes from a file or display the text from field My Quote"
                    };
            this.BaseSettings.Add("Quote source", setQuoteSource);

            var fileList = new ListDataType<string, ListControl>(this.GetListOfQuoteFiles());
            var setQuoteFile = new SettingItem<string, ListControl>(fileList)
                {
                    Value = "demo.quote",
                    Order = 2,
                    EnglishName = "Quote file",
                    Description = "The name of the file containing quotes"
                };
            this.BaseSettings.Add("Quote file", setQuoteFile);

            var setMyQuote = new SettingItem<string, TextBox>
                {
                    Value = "Enter your a quote here!",
                    Order = 3,
                    EnglishName = "My Quote",
                    Description = "Enter any quote here and set Quote source to My Quote"
                };
            this.BaseSettings.Add("My Quote", setMyQuote);

            var setTextSize =
                new SettingItem<string, ListControl>(
                    new ListDataType<string, ListControl>("Default;H1 (largest);H2;H3;H4;H5;H6 (smallest)"))
                    {
                        Value = "Default",
                        Order = 4,
                        EnglishName = "Text size",
                        Description = "Text size of the quote text. The 6 build-in heading sizes (HTML tag <H1>,<H2>,etc)"
                    };
            this.BaseSettings.Add("Text size", setTextSize);

            var setDisplayInItalic = new SettingItem<bool, CheckBox>
                {
                    Value = true,
                    Order = 5,
                    EnglishName = "Display in italic?",
                    Description = "Display all the quote text in italic style (HTML tag <i>)"
                };
            this.BaseSettings.Add("Display in italic", setDisplayInItalic);

            var setDisplayInBold = new SettingItem<bool, CheckBox>
                {
                    Value = false,
                    Order = 6,
                    EnglishName = "Display in bold?",
                    Description = "Display all the quote text in bold/fat letters (HTML tag <b>)"
                };
            this.BaseSettings.Add("Display in bold", setDisplayInBold);

            var setStartTag = new SettingItem<string, TextBox>
                {
                    Value = string.Empty,
                    Order = 7,
                    EnglishName = "Start tag",
                    Description =
                        "Enter any special customizing HTML start tag here, e.g. a marquee tag make the text scroll"
                };
            this.BaseSettings.Add("Start tag", setStartTag);

            var setEndTag = new SettingItem<string, TextBox>
                {
                    Value = string.Empty,
                    Order = 8,
                    EnglishName = "End tag",
                    Description = "Must correspond to the Start tag"
                };
            this.BaseSettings.Add("End tag", setEndTag);
        }

        /// <summary>
        /// Author:		Joe Audette
        /// Added:		7/31/2003
        /// Allows you to add files with queries without compiling
        /// Any query file placed in the folder specified in the web.config willshow up
        /// in the dropdown list
        /// </summary>
        /// <returns>FileInfo[]</returns>
        public FileInfo[] GetListOfQuoteFiles()
        {
            string quoteFilePath = string.Empty;

            //jes1111 - if (ConfigurationSettings.AppSettings["QuoteFileFolder"] != null && ConfigurationSettings.AppSettings["QuoteFileFolder"].Length > 0)
            if (Config.QuoteFileFolder.Length != 0)
                quoteFilePath = Config.QuoteFileFolder;
            else
            {
                //this will default to the folder where the .query files are located by default
                quoteFilePath = HttpContext.Current.Server.MapPath(TemplateSourceDirectory);
            }

            try
            {
                if (Directory.Exists(quoteFilePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(quoteFilePath);
                    return dir.GetFiles("*.quote");
                }
                else
                {
                    ErrorHandler.Publish(LogLevel.Warn,
                                         "Default Quote file folder/location not found: '" + quoteFilePath + "'");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Publish(LogLevel.Error, "Quote file folder/location not found: " + quoteFilePath, ex);
            }
            return null;
        }


        /// <summary>
        /// GUID of module (mandatory)
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{2502DB18-B580-4F90-8CB4-C15E6E531053}"); }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Raises Init event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += new EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        #endregion
    }
}