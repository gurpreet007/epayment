<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <title>e-Payment</title>
    <!--[if lt IE 9]>
        <script src="scripts/html5shiv.min.js"></script>        
   <![endif]-->
    <link rel="stylesheet/less" href="styles/epayment.less" type="text/css" />
    <script src="scripts/less.min.js"></script>
</head>
<body>
    <h3 class="login">
        <img class="login" alt="PSPCL Logo" src="resources/pspcl.gif"/>
        Punjab State Power Corporation Limited
    </h3>
    <%--<h2 class="login">ePayment Server will be down for maintenance Today (26<sup>th</sup> Nov) 03:00 PM Onwards.
    </h2>--%>
    <form runat="server" class="login">
        <header class="login">e-Payment System</header>
        <input type="text" id="txtUserID" placeholder="Location Code" runat="server" 
                        tabindex="1" class="login" maxlength="64" required/><br><br>
        <input type="password" id="txtUserPass" placeholder="Password" 
                        runat="server" tabindex="2" class="login" maxlength="64" required/><br>
        <%--<input type="text" id="txtEmpID" placeholder="Emp ID" runat="server" 
                        tabindex="3" class="login" maxlength="64" required/>
        <input type="password" id="txtHRPass" placeholder="Password" runat="server" 
                        tabindex="4" class="login" maxlength="64" required/><br/>--%>
        <asp:Button ID="btnLogin" runat="server" Text="Login" onclick="btnLogin_Click" 
                        TabIndex="5" class="login"/><br/>
        <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <div id="forIE"></div>
    <script src="scripts/jquery-2.2.0.min.js"></script>  
    <script>
        $(document).ready(function () {
            $("#pageFooter").load("resources/snippets.html #snipPageFooter", function () {
                $("#ftSupport").remove();
            });
        });
    </script>
    <script>
        function getIEVersion() {
            var rv = -1; // Return value assumes failure.
            if (navigator.appName == 'Microsoft Internet Explorer') {
                var ua = navigator.userAgent;
                var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
                if (re.test(ua) != null)
                    rv = parseFloat(RegExp.$1);
            }
            return rv;
        }
        function checkVersion() {
            var ver = getIEVersion();
            if (ver != -1) {
                if (ver <= 9.0) {
                    alert("You are using older version of Internet Explorer. Please upgrade to a latest browser.");
                    document.getElementById("forIE").innerHTML = '<div id="snipPageFooter">' +
                        'This site works best in the latest versions of '+
                        '<a target="_blank" class="pageFooter" title="Google Chrome Download Page" '+
                        'href="https://www.google.com/chrome/browser/desktop/index.html">'+
                        'Google Chrome</a>, '+
                        '<a target="_blank" class="pageFooter" title="Mozilla Firefox Download Page" '+
                        'href="https://www.mozilla.org/en-US/firefox/new/">'+
                        'Mozilla Firefox</a>, '+
                        '<a target="_blank" class="pageFooter" title="Opera Browser Download Page" '+
                        'href="http://www.opera.com/computer/windows">'+
                        'Opera</a> and '+
                        '<a target="_blank"  class="pageFooter" title="Apple Safari Download Page" '+
                        'href="http://support.apple.com/downloads/#safari">'+
                        'Apple Safari</a>.'+
                        '</div>';
                }
            }
        }
        checkVersion();
    </script>
</body>
</html>
