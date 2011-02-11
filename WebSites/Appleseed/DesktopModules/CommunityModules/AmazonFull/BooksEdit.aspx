<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ page autoeventwireup="false" inherits="Appleseed.Content.Web.Modules.AmazonBooksEdit"
    language="c#" Codebehind="BooksEdit.aspx.cs" %>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server" enctype="multipart/form-data">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server">
                </portal:banner>
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tbody>
                        <tr>
                            <td colspan="3" nowrap="nowrap">
                                <div class="Head">
                                    Book Details</div>
                                <hr noshade="noshade" size="1" />
                                <p>
                                    this Amazon Module <b>(bulid #2004-3)<br />
                                    </b>created by Charles Carroll of <a href="http://www.learnasp.com" target="learnasp">
                                        learnAsp.com</a> and Rahul Xing of <a href="http://www.KonoTree.com" target="kono">KonoTree.com</a></p>
                                <p>
                                    <rbfwebui:linkbutton id="topUpdateButton" runat="server" cssclass="CommandButton"
                                        text="Update">Update</rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="topCancelButton" runat="server" causesvalidation="False"
                                        cssclass="CommandButton" text="Cancel">Cancel</rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="topDeleteButton" runat="server" causesvalidation="False"
                                        cssclass="CommandButton" text="Delete this item">Delete this item</rbfwebui:linkbutton>
                                    <br />
                                </p>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="SubHead" nowrap="nowrap">
                                ISBN:
                            </td>
                            <td>
                                <asp:textbox id="ISBNField" runat="server" columns="30" cssclass="NormalTextBox"
                                    maxlength="100" width="390"></asp:textbox>
                            </td>
                            <td class="Normal" width="250">
                                <p>
                                    <asp:requiredfieldvalidator id="ISBNRequiredValidator" runat="server" controltovalidate="ISBNField"
                                        display="Static" errormessage="You Must Enter a Valid ISBN"></asp:requiredfieldvalidator></p>
                            </td>
                        </tr>
                        <tr>
                            <td class="SubHead" nowrap="nowrap" valign="top">
                                Add a caption:</td>
                            <td>
                                <asp:textbox id="CaptionTextBox" runat="server" rows="16" textmode="MultiLine" width="390px"></asp:textbox></td>
                            <td class="Normal" width="250">
                            </td>
                        </tr>
                        <tr>
                            <td class="SubHead" colspan="3" nowrap="nowrap" valign="top">
                                <p>
                                </p>
                                    <rbfwebui:linkbutton id="bottomUpdateButton" runat="server" cssclass="CommandButton"
                                        text="Update">Update</rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="bottomCancelButton" runat="server" causesvalidation="False"
                                        cssclass="CommandButton" text="Cancel">Cancel</rbfwebui:linkbutton>&nbsp;
                                    <rbfwebui:linkbutton id="bottomDeleteButton" runat="server" causesvalidation="False"
                                        cssclass="CommandButton" text="Delete this item">Delete this item</rbfwebui:linkbutton>
                                    <hr align="left" noshade="noshade" size="1" width="100%" />
                                    <p>
                                    </p>
                                <p>
                                    <span class="Normal">
                                        <rbfwebui:localize id="CreatedLabelLiteral" runat="server" text="Created By: " textkey="CREATED_BY">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedBy" runat="server"></rbfwebui:label>&nbsp;
                                        <rbfwebui:localize id="CreatedDateLiteral" runat="server" text="On: " textkey="CREATED_ON">
                                        </rbfwebui:localize>&nbsp;
                                        <rbfwebui:label id="CreatedDate" runat="server"></rbfwebui:label></span>
                                </p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="rb_AlternatePortalFooter">
                <div class="rb_AlternatePortalFooter">
                    <foot:footer id="Footer" runat="server">
                    </foot:footer></div>
               
            </div>
        </div>
    </form>
</body>
</html>
