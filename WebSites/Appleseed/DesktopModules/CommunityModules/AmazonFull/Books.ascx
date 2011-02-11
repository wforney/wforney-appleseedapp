<%@ control autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.AmazonBooks"
    language="c#" Codebehind="Books.ascx.cs" %>
<asp:datalist id="myDataList" runat="server" cellpadding="4" enableviewstate="false"
    width="100%">
    <itemstyle verticalalign="Top" />
    <itemtemplate>
        <table>
            <tr>
                <td valign="top" width="<%# GetTdWidthPercentage(Settings["Columns"].ToString()) %>">
                    <rbfwebui:hyperlink id="BooksEditLink" runat="server" imageurl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
                        navigateurl='<%# Appleseed.Framework.HttpUrlBuilder.BuildUrl("~/DesktopModules/CommunityModules/AmazonFull/BooksEdit.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'
                        text="Edit" textkey="EDIT" visible="<%# IsEditable %>">
                    </rbfwebui:hyperlink><br>
                    <a href='http://www.amazon.com/exec/obidos/ISBN=<%# DataBinder.Eval(Container.DataItem,"ISBN") %>/<%# Settings["Promotion Code"].ToString() %>'>
                        <img border="0" src='http://images.amazon.com/images/P/<%# DataBinder.Eval(Container.DataItem,"ISBN") %>.01.MZZZZZZZ.jpg'
                            width='<%=Settings["Width"].ToString()%>'>
                    </a>
                    <br />
                    <%# GetWebServiceDetails(Settings["Amazon Dev Token"].ToString(),DataBinder.Eval(Container.DataItem,"ISBN").ToString(),Settings["Show Details"].ToString(),Settings["Promotion Code"].ToString()) %>
                    <%# DataBinder.Eval(Container.DataItem,"Caption") %>
                </td>
            </tr>
        </table>
        <br />
    </itemtemplate>
</asp:datalist>
