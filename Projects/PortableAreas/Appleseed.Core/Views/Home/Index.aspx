<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="AgilePortal.Core" %>
<%@ Import Namespace="MvcContrib" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Index</title>
</head>
<body>
    <div>
        <script language='javascript' type='text/javascript' src='<%: Url.Resource("Scripts.jquery-1.5.min.js") %>'></script>
        <link rel="stylesheet" type="text/css" href='<%: Url.Resource("Content.style.css") %>' />
        <ul>
            <li>RenderAction works !</li>
            <li>
                <img id="example" src='<%: Url.Resource("Content.img.koala.jpg") %>' alt="" width="40px" />Images
                work !</li>
            <li><span id="spn"></span></li>
            <li class="cssli" style="color: White; background-color: white">css files work !</li>
            <li class="not-cssli">css don´t :( </li>
        </ul>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $('#spn').text('.js files work !');
            });
        </script>
    </div>
</body>
</html>
