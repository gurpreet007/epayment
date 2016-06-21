<%@ Page Language="C#" AutoEventWireup="true" CodeFile="delayed.aspx.cs" Inherits="reports" %>

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
    <header class="sectionHeader">Delayed Log</header>
    <asp:Panel class="reportPanel" ID="panDelayed" runat="server" Visible="True">
        <form id="form1" runat="server" class="tableWrapper">
            <div class="tableRow">
                <p><label for="panDelayed_drpUsers">User</label></p>
                <p><asp:DropDownList ID="panDelayed_drpUsers" runat="server" autofocus></asp:DropDownList></p>
            </div>
            <div class="tableRow">
                <p><label for="panDelayed_drpBillType">Bill Type</label></p>
                <p><asp:DropDownList ID="panDelayed_drpBillType" runat="server">
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
                <p><label for="panDelayed_drpDelayedType">Delayed Type</label></p>
                <p><asp:DropDownList ID="panDelayed_drpDelayedType" runat="server">
                    <asp:ListItem Value="AfterDueDate">Uploaded after Due Date</asp:ListItem>
                    <asp:ListItem Value="MRD_Days">Upload after MRD + ... Days</asp:ListItem>
                    <asp:ListItem Value="UploadedAfter">Uploaded after ... Date</asp:ListItem>
                    </asp:DropDownList></p>
            </div> 
            <div class="tableRow exactDate">
                <p style="vertical-align:middle"><label for="sDate">Enter Date</label></p>
                <p><input type="text" runat="server" id="sDate" class="dtclass" placeholder="DD-Mon-YYYY" maxlength="11"/></p>
            </div> 
            <div class="tableRow mrddays">
                <p style="vertical-align:middle"><label for="numDays">Enter Days</label></p>
                <p><input type="text" runat="server" id="numDays" class="numclass" maxlength="3" value="5" min="1" max="100"/></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Label ID="panDelayed_lblMsg" class="msg" runat="server"></asp:Label></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Button ID="panDelayed_btnShowCount" Text="Show Count" runat="server" onclick="btnShowCount_Click" /></p>
            </div>
            <div class="tableRow">
                <p></p>
                <p><asp:Button ID="panDelayed_btnShowIndvRecs" Text="Bill Details" runat="server" onclick="btnShowIndvRecs_Click" /></p>
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
                $("#pageNav #nvDelayed").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvDelayed").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");

            //date handling
            var curdate = GetDate();
            if ($("#sDate").val() == "") {
                $("#sDate").val(curdate);
            }

            $(".exactDate").removeClass("tableRow");
            $(".exactDate").addClass("hide");
            $(".mrddays").removeClass("tableRow");
            $(".mrddays").addClass("hide");
            function exactDates() {
                var drpDur = $("#panDelayed_drpDelayedType option:selected").val();
                if (drpDur == "UploadedAfter") {
                    $(".exactDate").removeClass("hide");
                    $(".exactDate").addClass("tableRow");
                    $(".mrddays").removeClass("tableRow");
                    $(".mrddays").addClass("hide");
                }
                else if (drpDur == "MRD_Days") {
                    $(".mrddays").removeClass("hide");
                    $(".mrddays").addClass("tableRow");
                    $(".exactDate").removeClass("tableRow");
                    $(".exactDate").addClass("hide");
                    var billType = $("#panDelayed_drpBillType option:selected").val();
                    if ($("#panDelayed_lblMsg").text() == "") {
                        if (billType == "SAP_SBM_GSC" || billType == "SAP_SBM_READING") {
                            $("#panDelayed_lblMsg").text("Using Cur_Meter_Reading_Date");
                        }
                        else {
                            $("#panDelayed_lblMsg").text("Using IssueDate");
                        }
                    }
                }
                else {
                    $(".exactDate").removeClass("tableRow");
                    $(".exactDate").addClass("hide");
                    $(".mrddays").removeClass("tableRow");
                    $(".mrddays").addClass("hide");
                }
            }
            $(".dtclass").datepicker({ dateFormat: "dd-M-yy", changeMonth: "true", changeYear: "true", maxDate: 0, showMonthAfterYear: "true" });

            //needed incase of FF refresh while Exact Date is selected in drpdown
            exactDates();

            //change event of dropdown
            $("#panDelayed_drpDelayedType").change(exactDates);
        });
    </script>
</body>
</html>
