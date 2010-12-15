using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Appleseed.Framework;
using Path=Appleseed.Framework.Settings.Path;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// Summary description for UserDefinedTableXSLctl.
    /// </summary>
    [DefaultProperty("Text"), ToolboxData("<{0}:UserDefinedTableXSLctl runat=server></{0}:UserDefinedTableXSLctl>")]
    public class UserDefinedTableXML : Xml, IPostBackEventHandler
    {
        private DataView dv;
        private string sortField;
        private string sortOrder;
        private bool isEditable;
        private int pageID;
        private int moduleID;
        private int showdetailID = 0;

        #region Constructors

        public UserDefinedTableXML(DataView XMLdataview, int PageID, int ModuleID, bool IsEditable, string SortField,
                                   string SortOrder)
        {
            dv = XMLdataview;
            pageID = PageID;
            moduleID = ModuleID;
            isEditable = IsEditable;
            sortField = SortField;
            sortOrder = SortOrder;
        }

        #endregion

        #region Control creation

        /// <summary> 
        /// Render this control to Esperantus.Esperantus.Localize. output parameter specified.
        /// </summary>
        /// <param name="output"> Esperantus.Esperantus.Localize. HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            // *** Write it back to Esperantus.Esperantus.Localize. server
            output.Write(RenderedXML());
        }

        private string RenderedXML()
        {
            base.Document = XmlData();

            // *** Write Esperantus.Esperantus.Localize. HTML into this string builder
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            HtmlTextWriter hWriter = new HtmlTextWriter(sw);
            base.Render(hWriter);

            // *** insert Sorting links (if minimal one is used in output)
            if (sb.ToString().IndexOf("@@sort.") > -1)
            {
                foreach (DataColumn fldname in dv.Table.Columns)
                {
                    if (fldname.ColumnName == sortField)
                    {
                        if (SortOrder == "ASC")
                        {
                            sb.Replace("@@sort." + SortField + "@@", GetSortingUrl(SortField, "DESC"));
                            sb.Replace("@@imgsortorder." + SortField + "@@", GetSortOrderImg("ASC"));
                        }
                        else
                        {
                            sb.Replace("@@sort." + SortField + "@@", GetSortingUrl(SortField, "ASC"));
                            sb.Replace("@@imgsortorder." + SortField + "@@", GetSortOrderImg("DESC"));
                        }
                    }
                    else
                    {
                        sb.Replace("@@sort." + fldname.ColumnName + "@@", GetSortingUrl(fldname.ColumnName, "ASC"));
                        sb.Replace("@@imgsortorder." + fldname.ColumnName + "@@", string.Empty);
                    }
                }
            }

            // *** insert ShowDetail links 
            int cmdPos;
            cmdPos = sb.ToString().IndexOf("@@ShowDetail");
            while (cmdPos > -1)
            {
                string s = sb.ToString().Substring(cmdPos + 12);
                int p2 = s.IndexOf("@");
                s = s.Substring(0, p2);
                int idnr = int.Parse(s);
                sb.Replace("@@ShowDetail" + idnr.ToString() + "@@", GetShowDetailUrl(idnr));
                cmdPos = sb.ToString().IndexOf("@@ShowDetail");
            }

            // *** Localize
            //
            cmdPos = sb.ToString().ToUpper().IndexOf("@@LOCALIZE");
            while (cmdPos > -1)
            {
                string s = sb.ToString().Substring(cmdPos + 11);
                int p2 = s.IndexOf("@");
                s = s.Substring(0, p2);
                string lkey = s.ToUpper();
                string srepl = sb.ToString().Substring(cmdPos, 11 + p2 + 2);
                sb.Replace(srepl, General.GetString(lkey));
                cmdPos = sb.ToString().ToUpper().IndexOf("@@LOCALIZE");
            }

            // *** SortOrder images
            //
//			cmdPos = sb.ToString().ToUpper().IndexOf("@@IMGSORTORDER");
//			while (cmdPos>-1)
//			{
//				string s = sb.ToString().Substring(cmdPos + 15);
//				int p2 = s.IndexOf("@");
//				s = s.Substring(0,p2);
//				string lkey = s.ToUpper();
//				string srepl = sb.ToString().Substring(cmdPos,15+p2+2);
//				sb.Replace(srepl,Esperantus.General.GetString(lkey));
//				cmdPos = sb.ToString().ToUpper().IndexOf("@@IMGSORTORDER");
//			}
            //

            return sb.ToString();
        }

        #endregion

        #region Events and delegates

        /// <summary>
        /// Implement Esperantus.Esperantus.Localize. RaisePostBackEvent method from Esperantus.Esperantus.Localize. IPostBackEventHandler interface. 
        /// Define Esperantus.Esperantus.Localize. method of IPostBackEventHandler that raises change events.
        /// To capture Sorting request issued from XML/XSL data
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaisePostBackEvent(string eventArgument)
        {
            string[] strEvent = eventArgument.Split('|');
            if (strEvent[0] == "Sort")
            {
                UDTXSLSortEventArgs newEvent = new UDTXSLSortEventArgs(strEvent[1], strEvent[2]);
                OnSort(newEvent);
            }
            else if (strEvent[0] == "ShowDetail")
            {
                showdetailID = int.Parse(strEvent[1]);
                //UDTXSLShowDetailEventArgs newEvent = new UDTXSLShowDetailEventArgs(strEvent[1]);
                //OnShowDetail(newEvent);
            }
        }

        /// <summary>
        /// Esperantus.Esperantus.Localize. Sort event is defined using Esperantus.Esperantus.Localize. event keyword.
        /// </summary>
        public event UDTXSLSortEventHandler SortCommand;

        /// <summary>
        /// Calls Sort Delegate 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSort(UDTXSLSortEventArgs e)
        {
            if (SortCommand != null)
                SortCommand(this, e); //Invokes Esperantus.Esperantus.Localize. delegates
        }

        #endregion

        #region Private Implementation

        private XmlDocument XmlData()
        {
            // sort data view
            if (sortField.Length != 0)
            {
                dv.Sort = SortField + " " + SortOrder;
            }
            XmlDocument xmlDocument = new XmlDocument();

            XmlNode rootElement = xmlDocument.CreateElement("UserDefinedTable");

            //Add root Attributes 
            //ModuleID
            XmlAttribute newAttribute;
            newAttribute = xmlDocument.CreateAttribute("ModuleID");
            newAttribute.Value = ModuleID.ToString();
            rootElement.Attributes.Append(newAttribute);
            //ShowDetail ID
            newAttribute = xmlDocument.CreateAttribute("ShowDetail");
            newAttribute.Value = showdetailID.ToString();
            rootElement.Attributes.Append(newAttribute);
            //Language
            newAttribute = xmlDocument.CreateAttribute("xml:lang");
            newAttribute.Value = "en-us"; // Esperantus.Localize.GetCurrentNeutralCultureName();
            rootElement.Attributes.Append(newAttribute);
            newAttribute = null;

            xmlDocument.AppendChild(rootElement);

            IEnumerator iterator = dv.GetEnumerator();
            DataRowView drv;
            int i = 0;
            while (iterator.MoveNext())
            {
                drv = (DataRowView) iterator.Current;
                i++;
                if (showdetailID == 0 || showdetailID.ToString() == drv["UserDefinedRowID"].ToString())
                    //Don't boEsperantus.Esperantus.Localize.r to make xml data for ever record if we only want one 
                {
                    XmlNode rowNode = xmlDocument.CreateElement("Row");

                    XmlAttribute rowIDAttribute = xmlDocument.CreateAttribute("ID");
                    rowIDAttribute.Value = drv["UserDefinedRowID"].ToString();
                    rowNode.Attributes.Append(rowIDAttribute);

                    foreach (DataColumn dc in dv.Table.Columns)
                    {
                        if (dc.ColumnName != "UserDefinedRowID")
                        {
                            XmlNode fieldNode = xmlDocument.CreateElement(dc.ColumnName);
                            fieldNode.InnerText = drv[dc.ColumnName].ToString();
                            rowNode.AppendChild(fieldNode);
                        }
                    }

                    XmlNode extraNode;
                    //Rob Siera - 04 nov 2004 - Add EditURL to XML output
                    extraNode = xmlDocument.CreateElement("EditURL");
                    if (IsEditable)
                    {
                        extraNode.InnerText =
                            HttpUrlBuilder.BuildUrl("~/DesktopModules/UserDefinedTable/UserDefinedTableEdit.aspx",
                                                    pageID,
                                                    "&mID=" + ModuleID + "&UserDefinedRowID=" +
                                                    drv["UserDefinedRowID"].ToString());
                    }
                    else
                    {
                        extraNode.InnerText = string.Empty;
                    }
                    rowNode.AppendChild(extraNode);

                    //Rob Siera - 11 dec 2004 - Add ShowDetailURL to XML output
                    extraNode = xmlDocument.CreateElement("ShowDetailURL");
                    extraNode.InnerText = "@@ShowDetail" + drv["UserDefinedRowID"].ToString() + "@@";
                    rowNode.AppendChild(extraNode);

                    rootElement.AppendChild(rowNode);
                }
            }
            return xmlDocument;
        }


        /// <summary>
        /// Sort link creator
        /// </summary>
        /// <returns></returns>
        private string GetSortingUrl(string field, string order)
        {
            return "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "Sort|" + field + "|" + order);
        }

        /// <summary>
        /// ShowDetail link creator
        /// </summary>
        /// <returns></returns>
        private string GetShowDetailUrl(int idnr)
        {
            return "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "ShowDetail|" + idnr.ToString());
        }

        /// <summary>
        /// ShowDetail link creator
        /// </summary>
        /// <returns></returns>
        private string GetSortOrderImg(string order)
        {
            string s;
            if (order == "DESC")
            {
                s = @"<img src='" +
                    Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortdescending.gif") +
                    "' width='10' height='9' border='0'>";
            }
            else
            {
                s = @"<img src='" +
                    Path.WebPathCombine(Path.ApplicationRoot, "DesktopModules/UserDefinedTable/sortascending.gif") +
                    "' width='10' height='9' border='0'>";
            }
            return s;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string InnerXml
        {
            get
            {
                base.Document = XmlData();
                return base.Document.InnerXml;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataView UDTdata
        {
            get { return dv; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SortField
        {
            get { return sortField; }
            set { sortField = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        /// <summary>
        /// /
        /// </summary>
        public bool IsEditable
        {
            get { return isEditable; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ModuleID
        {
            get { return moduleID; }
        }

        #endregion
    }


    /// <summary>
    /// UDTXSLSortEventHandler
    /// </summary>
    public delegate void UDTXSLSortEventHandler(object sender, UDTXSLSortEventArgs e);


    /// <summary>
    /// 
    /// </summary>
    public class UDTXSLSortEventArgs : EventArgs
    {
        private string sortField;
        private string sortOrder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        public UDTXSLSortEventArgs(string sortField, string sortOrder) : base()
        {
            this.sortField = sortField;
            this.sortOrder = sortOrder;
        }

        /// <summary>
        /// 
        /// </summary>
        public string SortField
        {
            get { return sortField; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SortOrder
        {
            get { return sortOrder; }
        }
    }
}