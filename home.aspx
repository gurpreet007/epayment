<%@ Page Language="C#" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="index" %>

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
    <header class="sectionHeader">Last Data Entered:</header>
    <form runat="server" class="tableWrapper">
        <div class="tableRow">
            <p>LS:</p>
            <p>
                <span><asp:Label ID="lblLSCount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p>MS:</p>
            <p>
                <span><asp:Label ID="lblMSCount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p>SP:</p>
            <p>
                <span><asp:Label ID="lblSPCount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p>DS ABOVE 10KW:</p>
            <p>
                <span><asp:Label ID="lblDSACount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p>DS BELOW 10KW:</p>
            <p>
                <span><asp:Label ID="lblDSBCount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p>SAP DS BELOW 10KW:</p>
            <p>
                <span><asp:Label ID="lblSapDSBCount" runat="server" text="-"></asp:Label></span>
            </p>
        </div>
        <input type="hidden" id="valUserID" runat="server" />
        <input type="hidden" id="valEmpID" runat="server" />
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <script src="scripts/jquery-2.2.0.min.js"></script>
    <script>
        $(document).ready(function () {
            $("#pageHeader").load("resources/snippets.html #snipPageHeader");
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvHome").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvHome").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");
            function GetDates()
            {
                var userid = $('#valUserID').val();
                var empid = $('#valEmpID').val();
                $.ajax({
                    type: "POST",
                    url: "home.aspx/GetDates",
                    data: '{userID:"' + userid + '", empID: "' + empid + '"}',
                    contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (response) {
                        var cnts = JSON.parse(response.d);
                        $('#lblLSCount').html(cnts[0]);
                        $('#lblMSCount').html(cnts[1]);
                        $('#lblSPCount').html(cnts[2]);
                        $('#lblDSACount').html(cnts[3]);
                        $('#lblDSBCount').html(cnts[4]);
                        $('#lblSapDSBCount').html(cnts[5]);
                    },
                    error: function (error) {
                        $("#lblLSCount").html(error.statusText);
                    }
                });
            }
            GetDates();
        });
    </script>
</body> 
</html>