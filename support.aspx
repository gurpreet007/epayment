<%@ Page Language="C#" AutoEventWireup="true" CodeFile="support.aspx.cs" Inherits="index" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta charset="utf-8">
	<title>e-Payment</title>
    <!--[if lt IE 9]>
        <script src="scripts/html5shiv.min.js"></script>        
   <![endif]-->
    <link rel="stylesheet/less" href="styles/epayment.less" type="text/css" />
    <script src="scripts/less.min.js"></script>
</head>
<body>  
    <header class="page" id="pageHeader">	</header>
	<div id="loginInfo">
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
	</div>
    <nav id="pageNav"></nav>
    <header class="sectionHeader">PSPCL Helpdesk:</header>
    <form runat="server" class="tableWrapper">
        <div class="tableRow">
            <p>Phone 1:</p>
            <p>96461-19129</p>
        </div>
        <div class="tableRow">
            <p>Phone 2:</p>
            <p>96461-85451</p>
        </div>
        <div class="tableRow">
            <p>Phone 3:</p>
            <p>96461-85458</p>
        </div>
        <div class="tableRow">
            <p>Phone 4:</p>
            <p>96461-85476</p>
        </div>
        <div class="tableRow">
            <p>Phone 5:</p>
            <p>96461-85489</p>
        </div>
        <div class="tableRow">
            <p>Email:</p>
            <p><a href="mailto:helpdesk-pspcl@pspcl.in">helpdesk-pspcl@pspcl.in</a></p>
        </div>
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <script src="scripts/jquery-2.2.0.min.js"></script>
    <script>
        $(document).ready(function () {
            $("#pageHeader").load("resources/snippets.html #snipPageHeader");
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvSupport").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvSupport").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter", function () {
                $("#ftSupport").remove();
            });
        });
    </script>
</body> 
</html>