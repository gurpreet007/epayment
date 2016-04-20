<%@ Page Language="C#" AutoEventWireup="true" CodeFile="activity_log.aspx.cs" Inherits="reports" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta charset="utf-8">
	<title>e-Payment</title>
    <!--[if lt IE 9]>
        <script src="scripts/html5shiv.min.js"></script>        
   <![endif]-->
	<%--<link rel="stylesheet" href="styles/epayment.css">--%>
    <link rel="stylesheet" href="Styles/jquery-ui.min.css" type="text/css" />
    <link rel="stylesheet/less" href="styles/epayment.less" type="text/css" />
    <script src="scripts/less.min.js"></script>
</head>
<body>  
    <header class="page" id="pageHeader">	</header>
	<div id="loginInfo">
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
	</div>
    <nav id="pageNav"></nav>
    <header class="sectionHeader">Activity Log</header>
    <asp:Panel class="reportPanel" ID="panActivity" runat="server" Visible="True">
        <form id="form1" runat="server" class="tableWrapper">
            <div class="tableRow">
                <p><label for="panActivity_drpUsers">User</label></p>
                <p><asp:DropDownList ID="panActivity_drpUsers" runat="server" autofocus></asp:DropDownList></p>
            </div>
            <div class="tableRow">
                <p><label for="panActivity_drpBillType">Bill Type</label></p>
                <p><asp:DropDownList ID="panActivity_drpBillType" runat="server">
                    <asp:ListItem Value="ALL">All Bill Types</asp:ListItem>
                    <asp:ListItem Value="DSBELOW10KW">NONSAP - DSBELOW10KW (Spot Billing)</asp:ListItem>
                    <asp:ListItem Value="LS">NONSAP - LS</asp:ListItem>
                    <asp:ListItem Value="SP">NONSAP - SP</asp:ListItem>
                    <asp:ListItem Value="MS">NONSAP - MS</asp:ListItem>
                    <asp:ListItem Value="DSABOVE10KW">NONSAP - DSABOVE10KW</asp:ListItem>
                    <asp:ListItem Value="SAP_SBM_GSC">SAP - GSC/GT</asp:ListItem>
                    <asp:ListItem Value="SAP_SBM_READING">SAP - SBM Reading (MS/SP/Temp/GC)</asp:ListItem>
                    </asp:DropDownList></p>
            </div>
            <div class="tableRow">
                <p><label for="panActivity_drpDuration">Duration</label></p>
                <p><asp:DropDownList ID="panActivity_drpDuration" runat="server">
                    <asp:ListItem Value="day1">Today</asp:ListItem>
                    <asp:ListItem Value="day2">2 Days</asp:ListItem>
                    <asp:ListItem Value="day3">3 Days</asp:ListItem>
                    <asp:ListItem Value="week">Week</asp:ListItem>
                    <asp:ListItem Value="month">Month</asp:ListItem>
                    <asp:ListItem Value="year">Year</asp:ListItem>
                    <asp:ListItem Value="dates">Enter Dates</asp:ListItem>
                    </asp:DropDownList></p>
            </div> 
            <div class="tableRow exactDate">
                <p style="vertical-align:middle"><label for="sDate">Start Date</label></p>
                <p><input type="text" runat="server" id="sDate" class="dtclass" placeholder="DD-Mon-YYYY" maxlength="11"/></p>
            </div> 
            <div class="tableRow exactDate">
                <p style="vertical-align:middle"><label for="eDate">End Date</label></p>
                <p><input type="text" runat="server" id="eDate" class="dtclass" placeholder="DD-Mon-YYYY" maxlength="11"/></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Label ID="panActivity_lblMsg" class="msg" runat="server"></asp:Label></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Button ID="panActivity_btnShowCount" Text="Show Count" runat="server" onclick="btnShowCount_Click" /></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Button ID="panActivity_btnShowActivity" Text="Activity Log" runat="server" onclick="btnUserActivity_Click" /></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Button ID="panActivity_btnShowIndvRecs" Text="Bill Details" runat="server" onclick="btnShowIndvRecs_Click" /></p>
            </div>
        </form>
    </asp:Panel>
    <footer id="pageFooter" class="pageFooter"></footer>
    <script src="scripts/jquery-2.2.0.min.js?2"></script>
    <script src="Scripts/jquery-ui.min.js?2"></script>
    <script src="Scripts/moment.js?2"></script>
    <script src="scripts/common.js?2"></script>
    <script>
        $(document).ready(function () {
            $("#pageHeader").load("resources/snippets.html #snipPageHeader");
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvActivityLog").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvActivityLog").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");

            //date handling
            var curdate = GetDate();
            if ($("#sDate").val() == "") {
                $("#sDate").val(curdate);
            }
            if ($("#eDate").val() == "") {
                $("#eDate").val(curdate);
            }

            $(".exactDate").removeClass("tableRow");
            $(".exactDate").addClass("hide");
            function exactDates()
            {
                var drpDur = $("#panActivity_drpDuration option:selected").val();
                if (drpDur == "dates") {
                    $(".exactDate").removeClass("hide");
                    $(".exactDate").addClass("tableRow");
                }
                else {
                    $(".exactDate").removeClass("tableRow");
                    $(".exactDate").addClass("hide");
                }
            }
            $(".dtclass").datepicker({ dateFormat: "dd-M-yy", changeMonth: "true", changeYear: "true", maxDate: 0, showMonthAfterYear: "true" });
            exactDates();//needed incase of FF refresh while Exact Date is selected in drpdown
            $("#panActivity_drpDuration").change(exactDates);
            $("#panActivity_btnAddUser").click(function () {
                $("#panActivity_lblMsg").html("");
            })
        });
    </script>
</body>
</html>
