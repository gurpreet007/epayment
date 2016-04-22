<%@ Page Language="C#" AutoEventWireup="true" CodeFile="archive.aspx.cs" Inherits="archive" %>

<!DOCTYPE html>
<html>
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
    <header class="page" id="pageHeader">	</header>
    <div id="loginInfo">
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
    </div>
    <nav id="pageNav"></nav>
    <header class="sectionHeader">Archive Bill</header>
    <form id="Form1" runat="server" class="tableWrapper">
        <div class="tableRow">
            <p></p>
            <p style="margin-left:0px; padding-left:0px;"><input type="text" runat="server" id="txtAcno" maxlength="12" placeholder="Non-SAP Ac. No."/></p>
        </div>
        <div class="tableRow">
            <p></p>
            <asp:Button ID="btnShow" Text="Show Info" runat="server" onclientclick="$('#lblMsg').text('')" onclick="btnShow_Click" />
        </div>
        <div class="tableRow">
            <p>Ac. No.:</p>
            <p><asp:Label ID="lblAcNo" runat="server"></asp:Label></p>
        </div>
        <div class="tableRow">
            <p>Name:</p>
            <p><asp:Label ID="lblName" runat="server"></asp:Label></p>
        </div>
        <div class="tableRow">
            <p>Category:</p>
            <p><asp:Label ID="lblCat" runat="server"></asp:Label></p>
        </div>
        <div class="tableRow">
            <p>Date Upload:</p>
            <p><asp:Label ID="lblDtUpload" runat="server"></asp:Label></p>
        </div>
        <div class="tableRow">
            <p></p>
            <p><asp:Label ID="lblMsg" class="msg" runat="server"></asp:Label></p>
        </div>
        <div class="tableRow">
            <p><asp:HyperLink ID="lnkViewBill" runat="server" Target="_blank" Visible="False">View Bill</asp:HyperLink></p>
            <p><asp:Button ID="btnArchive" Text="Archive Bill" runat="server" onclientclick="$('#lblMsg').text('')" onclick="btnArchive_Click" Visible="False" /></p>
        </div>
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <script src="scripts/jquery-2.2.0.min.js"></script>
    <script src="scripts/common.js"></script>
    <script>
        $(function () {
            $("#pageHeader").load("resources/snippets.html #snipPageHeader")
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvDownload").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvArchive").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");
        });
    </script>
</body>
</html>