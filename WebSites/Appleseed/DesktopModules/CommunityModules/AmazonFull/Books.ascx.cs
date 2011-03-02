using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml;
using Appleseed.Framework;
using Appleseed.Framework.Settings;
using Appleseed.Framework.Content.Data;
using Appleseed.Framework.Data;
using Appleseed.Framework.DataTypes;
using Appleseed.Framework.Web.UI.WebControls;
using Path=System.IO.Path;

namespace Appleseed.Content.Web.Modules
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Books module
    /// Load books from amazon
    /// </summary>
    public partial class AmazonBooks : PortalModuleControl
    {
        /// <summary>
        /// Is module searchable
        /// </summary>
        public override bool Searchable
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonBooks"/> class.
        /// </summary>
        public AmazonBooks()
        {
            var Columns = new SettingItem<int, TextBox>()
                { Required = true, Value = 3, MinValue = 1, MaxValue = 10 };
            this.BaseSettings.Add("Columns", Columns);

            var Width = new SettingItem<int, TextBox>()
                { Value = 110, MinValue = 50, MaxValue = 250 };
            this.BaseSettings.Add("Width", Width);

            var PromoCode = new SettingItem<string, TextBox>() { Value = Config.AmazonPromoCode };
            //jes1111
            //if (ConfigurationSettings.AppSettings["AmazonPromoCode"] != null && ConfigurationSettings.AppSettings["AmazonPromoCode"].Length != 0)
            //	PromoCode.Value = ConfigurationSettings.AppSettings["AmazonPromoCode"].ToString();
            //else 
            //	PromoCode.Value = string.Empty;
            this.BaseSettings.Add("Promotion Code", PromoCode);

            var ShowDetails = new SettingItem<string, TextBox>()
                { Value = "ProductName,OurPrice,Author" };
            this.BaseSettings.Add("Show Details", ShowDetails);

            var AmazonDevToken = new SettingItem<string, TextBox>()
                { Value = Config.AmazonDevToken };
            //jes1111
            //if (ConfigurationSettings.AppSettings["AmazonDevToken"] != null && ConfigurationSettings.AppSettings["AmazonDevToken"].Length != 0)
            //	AmazonDevToken.Value = ConfigurationSettings.AppSettings["AmazonDevToken"].ToString();
            //else 
            //	AmazonDevToken.Value = string.Empty;
            this.BaseSettings.Add("Amazon Dev Token", AmazonDevToken);

            //Choose your editor here
            SupportsWorkflow = false;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            BooksDB books = new BooksDB();

            myDataList.DataSource = books.Getrb_BookList(ModuleID);
            myDataList.DataBind();

            myDataList.RepeatColumns = Int32.Parse(Settings["Columns"].ToString());
        }

        /// <summary>
        /// GetTdWidthPercentage
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public string GetTdWidthPercentage(string Columns)
        {
            //Trace.Write("AmazonFullCaption","GetTdWidthPercentage()");
            int tdWidthPercent;
            try
            {
                tdWidthPercent = 100/Int32.Parse(Columns);
                return tdWidthPercent + "%";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GetWebServiceDetails
        /// </summary>
        /// <param name="AmazonDevToken"></param>
        /// <param name="isbn"></param>
        /// <param name="ShowDetails"></param>
        /// <param name="PromoCode"></param>
        /// <returns></returns>
        public string GetWebServiceDetails(string AmazonDevToken, string isbn, string ShowDetails, string PromoCode)
        {
            string BookDetails = string.Empty;
            if (ShowDetails.Length > 0)
            {
                string strAmazonURL = "http://xml.amazon.com/onca/xml2?" +
                                      "t=" + PromoCode +
                                      "&dev-t=" + AmazonDevToken +
                                      "&type=heavy&AsinSearch=" + isbn +
                                      "&f=xml";

                XmlTextReader xmlTr1;
                xmlTr1 = XmlReaderCached_v2(strAmazonURL);

                BookDetails = "<br>";
                ShowDetails = "," + ShowDetails + ",";

                try
                {
                    //Trace.Write("AmazonFullCaption","GetTdWidthPercentage.If.Try()");
                    while (xmlTr1.Read())
                    {
                        //Trace.Write("AmazonFullCaption","GetTdWidthPercentage.If.Try.While()");
                        if (xmlTr1.NodeType.ToString() == "Element")
                        {
                            //Trace.Write("AmazonFullCaption","GetTdWidthPercentage.If.Try.While.If()");
                            string strName = xmlTr1.Name;
                            if ((ShowDetails.IndexOf("," + strName + ",") > -1) || (ShowDetails.ToLower() == ",all,"))
                            {
                                //Trace.Write("AmazonFullCaption","GetTdWidthPercentage.If.Try.While.If.If()");
                                xmlTr1.Read();
                                BookDetails += strName + "&nbsp;&nbsp;<b>" + xmlTr1.Value + "</b><br>";
                            } //END IF
                        } //END IF
                    } //END WHILE	
                }
                catch
                {
                    //Trace.Write("AmazonFullCaption","GetTdWidthPercentage.If.Try.Catch()");
                }
            } //END IF
            //Trace.Write("AmazonFullCaption","GetWebServiceDetails.End()");
            return BookDetails;
        } //END FUNCTION

        /// <summary>
        /// ConvertStr2ByteArray
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public byte[] ConvertStr2ByteArray(string strInput)
        {
            //Trace.Write("AmazonFullCaption","ConvertStr2ByteArray.Begin()");

            int intCounter = 0;
            char[] arrChar;

            arrChar = strInput.ToCharArray();


            byte[] arrByte;
            arrByte = new byte[arrChar.Length - 1];

            for (intCounter = 0; intCounter <= arrByte.Length - 1; intCounter++)
            {
                arrByte[intCounter] = Convert.ToByte(arrChar[intCounter]);
            }

            //Trace.Write("AmazonFullCaption","ConvertStr2ByteArray.End()");
            return arrByte;
        }


        private string MD5checksum(string strParm1)
        {
            //Trace.Write("AmazonFullCaption","MD5checksum.Begin()");
            byte[] arrHashInput;
            byte[] arrHashOutput;
            MD5CryptoServiceProvider objMD5 = new MD5CryptoServiceProvider();

            arrHashInput = ConvertStr2ByteArray(strParm1);
            arrHashOutput = objMD5.ComputeHash(arrHashInput);

            //Trace.Write("AmazonFullCaption","MD5checksum.End()");
            return BitConverter.ToString(arrHashOutput);
        }


//		private XmlTextReader XmlReaderCached_v1(string strXML ){
//			
//			XmlTextReader xmlTrCached = new XmlTextReader(strXML);
//			//Trace.Write("AmazonFullCaption","XmlReaderCached_v1()");
//			return(xmlTrCached);
//		}


        private XmlTextReader XmlReaderCached_v2(string strXML)
        {
            //Trace.Write("AmazonFullCaption","XmlReaderCached_v2.Begin()");

            XmlTextReader xmlTrCached;
            string strChksum = MD5checksum(strXML);
            string strInCache;

            if (Cache[strChksum] == null)
            {
                strInCache = HttpGet(strXML);
                if (strInCache != "error")
                    Cache.Insert(strChksum, strInCache, null, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
                strInCache = Cache[strChksum].ToString();

            StringReader strReader1 = new StringReader(strInCache);
            xmlTrCached = new XmlTextReader(strReader1);
            //Trace.Write("AmazonFullCaption","XmlReaderCached_v2.End()");
            return (xmlTrCached);
        }


        private string HttpGet(string strURL)
        {
            //Response.Write("HttpGet(" + strURL + ")<BR>");
            //Trace.Write("AmazonFullCaption","HttpGet.Begin()");

            StreamReader sr1 = null;
            string strTemp;
            WebResponse webResponse1;
            WebRequest webRequest1;

            try
            {
                //Trace.Write("AmazonFullCaption","HttpGet.Try()");
                webRequest1 = WebRequest.Create(strURL);
                webResponse1 = webRequest1.GetResponse();
                sr1 = new StreamReader(webResponse1.GetResponseStream());
                strTemp = sr1.ReadToEnd();
            }
            catch
            {
                strTemp = "error";
                //Trace.Write("AmazonFullCaption","HttpGet.Catch()");
            }
            finally
            {
                if (sr1 != null)
                    sr1.Close();
                //Trace.Write("AmazonFullCaption","HttpGet.Finally()");
            }

            //Trace.Write("AmazonFullCaption","HttpGet.End()");
            return (strTemp);
        }


        /// <summary>
        /// General Module DEf GUID
        /// </summary>
        public override Guid GuidID
        {
            get { return new Guid("{5A2E8E9C-B9C7-439a-BFF9-54CA78762818}"); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "install.sql");

            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);

            if (errors.Count > 0)
            {
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Uninstall(IDictionary stateSaver)
        {
            string currentScriptName = Path.Combine(Server.MapPath(TemplateSourceDirectory), "uninstall.sql");

            List<string> errors = DBHelper.ExecuteScript(currentScriptName, true);

            if (errors.Count > 0)
            {
                throw new Exception("Error occurred:" + errors[0].ToString());
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        ///	Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();

            // Set here title properties
            // Add support for the edit page
            AddUrl = "~/DesktopModules/CommunityModules/AmazonFull/BooksEdit.aspx";

            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion
    }
}