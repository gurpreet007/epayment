<%@ Page Language="C#" AutoEventWireup="true" CodeFile="download.aspx.cs" Inherits="download" %>

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
    <header class="sectionHeader">Download Billing Data (NonSAP SBM)</header>
    <form runat="server" class="tableWrapper">
        <div class="tableRow">
            <p> <label for="drpBillType">Bill Type</label></p>
            <p>
                <asp:DropDownList ID="drpBillType" runat="server">
                    <asp:ListItem Selected="True" Value="BT">Bill Type</asp:ListItem>
                    <asp:ListItem Value="DSBELOW10KW">DSBELOW10KW (Spot Billing)</asp:ListItem>
                    <%--<asp:ListItem>LS</asp:ListItem>
                    <asp:ListItem>SP</asp:ListItem>
                    <asp:ListItem>MS</asp:ListItem>--%>
                    <asp:ListItem>DSABOVE10KW</asp:ListItem>
                    </asp:DropDownList>
                <input type="button" ID="btnCount"  Value="Get Count"  hidden/>
            </p>
        </div>
        <div class="tableRow">
            <p> <label for="drpCircle">Select Circle</label></p>
            <p>
                <asp:DropDownList ID="drpCircles" runat="server">
                <asp:ListItem Selected="True" Value="ALL">All Circles</asp:ListItem>
                <asp:ListItem>A</asp:ListItem>
                <asp:ListItem>B</asp:ListItem>
                <asp:ListItem>C</asp:ListItem>
                <asp:ListItem>D</asp:ListItem>
                <asp:ListItem>E</asp:ListItem>
                <asp:ListItem>F</asp:ListItem>
                <asp:ListItem>G</asp:ListItem>
                <asp:ListItem>H</asp:ListItem>
                <asp:ListItem>I</asp:ListItem>
                <asp:ListItem>J</asp:ListItem>
                <asp:ListItem>K</asp:ListItem>
                <asp:ListItem>L</asp:ListItem>
                <asp:ListItem>M</asp:ListItem>
                <asp:ListItem>N</asp:ListItem>
                <asp:ListItem>O</asp:ListItem>
                <asp:ListItem>P</asp:ListItem>
                <asp:ListItem>Q</asp:ListItem>
                <asp:ListItem>R</asp:ListItem>
                <asp:ListItem>S</asp:ListItem>
                <asp:ListItem>T</asp:ListItem>
                <asp:ListItem>U</asp:ListItem>
                <asp:ListItem>V</asp:ListItem>
                <asp:ListItem>W</asp:ListItem>
                <asp:ListItem>X</asp:ListItem>
                <asp:ListItem>Y</asp:ListItem>
                <asp:ListItem>Z</asp:ListItem>
                </asp:DropDownList>
            </p>
        </div>
        <div class="tableRow exactDate">
            <p><label for="sDate">Start Date</label></p>
            <p><input type="text" runat="server" id="sDate" placeholder="DD/MM/YYYY"/></p>
        </div>
        <div class="tableRow exactDate">
            <p><label for="eDate">End Date</label></p>
            <p><input type="text" runat="server" id="eDate" placeholder="DD/MM/YYYY"/></p>
        </div>
        <div class="tableRow">
            <p></p>
            <p><asp:Button ID="btnDownload" Text="Get SBM Data" runat="server" onclick="btnDownload_Click"/>
            </p>
        </div>
        <div class="tableRow">
            <p></p>
            <p>
                <span class="msg"><asp:Label ID="lblMessage" runat="server" class="msg"></asp:Label></span>
            </p>
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
                    function () { $("#pageNav #nvDownload").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");

            $("#btnCount").click(function () {
                var billType = $('#drpBillType option:selected').val();
                if (billType == "BT") {
                    $("#lblMessage").html("Select a Bill Type");
                    return;
                }
                $.ajax({
                    type: "POST",
                    url: "download.aspx/GetSomething",
                    data: JSON.stringify({ 'drpVal': billType }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $("#lblMessage").html(response.d);
                    },
                    error: function (error) {
                        $("#lblMessage").html(error.statustext);
                    }
                });
            });

            //date handling
            var curdate = GetDate();
            if ($("#sDate").val() == "") {
                $("#sDate").val(curdate);
            }
            if ($("#eDate").val() == "") {
                $("#eDate").val(curdate);
            }
        });
    </script>
</body>
</html>