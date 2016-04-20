<%@ Page Language="C#" AutoEventWireup="true" CodeFile="upload.aspx.cs" Inherits="upload" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
	<title>e-Payment</title>
    <!--[if lt IE 9]>
        <script src="scripts/html5shiv.min.js"></script>        
   <![endif]-->
	<%--<link rel="stylesheet" href="styles/epayment.css">--%>
    <link rel="stylesheet/less" href="styles/epayment.less" type="text/css" />
    <script src="scripts/less.min.js"></script>
</head>
<body> 
    <header class="page" id="pageHeader">	</header>
	<div id="loginInfo">
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
	</div>
    <nav id="pageNav"></nav>
    <header class="sectionHeader">Upload Billing Data</header>
    <form runat="server" class="tableWrapper">
        <div class="tableRow">
            <p></p>
            <p>
                <input type="radio" id="idBillClass1" class="billClass" name="billClass" value="sap" checked>SAP
                <input type="radio" id="idBillClass2" class="billClass" name="billClass" value="nonsap">Non-SAP
            </p>
        </div>
        <div class="tableRow">
            <p> <label for="drpBillType">Bill Type</label></p>
            <p id="ddlHere">
                <select ID='drpBillType' runat='server'>
                    <option Value='BT'>Bill Type</option>
                </select>
            </p>
        </div>
        <div class="tableRow">
            <p><label for="FileUpload1">Select Bill File</label></p>
		    <p><asp:FileUpload ID="FileUpload1" runat="server"/></p>
        </div>
        <div class="tableRow">
            <p></p>
            <p><asp:Button ID="btnUpload" runat="server" onclick="btnUpload_Click" 
                        Text="Upload Bill" required/>
            </p>
        </div>
        <div class="tableRow">
            <p></p>
            <p>
                <span class="msg"><asp:Label ID="lblMessage" runat="server" class="msg"></asp:Label></span>
            </p>
        </div>
        <div class="tableRow">
            <p></p>
            <p>
                <asp:Button ID="btnExport" runat="server" onclick="btnExport_Click" 
                        Text="Error Details" Visible="False" />
                <asp:HiddenField ID="hidSID" runat="server" />
                <asp:HiddenField ID="hidBillType" runat="server" />
                <asp:HiddenField ID="hidBillClass" runat="server" />
            </p>
        </div>
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <script src="scripts/jquery-2.2.0.min.js"></script>
    <script>
        $(function () {
            $("#drpBillType").change(function () {
                $("#hidBillType").val($(this).val());
            });
            $("#idBillClass1,#idBillClass2").change(function () {
                var value = "nonsap";
                if ($("#idBillClass1").is(":checked")) {
                    value = "sap";
                }
                $("#drpBillType").prop("selectedIndex", 0);
                $("#hidBillType").val($("#drpBillType").val());
                $("#hidBillClass").val(value);

                if (value == "sap") {
                    FillSAP();
                }
                else {
                    FillNonSAP();
                }
            });
            if ($("#hidBillClass").val() == "nonsap") {
                $("#idBillClass2").prop("checked", true);
                FillNonSAP();
            }
            else {
                $("#idBillClass1").prop("checked", true);
                $("#hidBillClass").val("sap");
                FillSAP();
            }
//            if ($("#hidBillType").val() == "") {
//                $("#hidBillType").val("BT");
//            }
            function FillNonSAP() {
                var arrTypeText = ["Select", "DSBELOW10KW (Spot Billing)", "LS", "SP", "MS", "DSABOVE10KW"];
                var arrTypeVal = ["", "DSBELOW10KW", "LS", "SP", "MS", "DSABOVE10KW"];

                $("#drpBillType").html("");

                var index = 0;
                for (index = 0; index < arrTypeText.length; index++) {
                    var el;
                    el = document.createElement("option");
                    el.textContent = arrTypeText[index];
                    el.value = arrTypeVal[index];
                    $("#drpBillType").append(el);
                }
                if ($("#hidBillType").val() != "") {
                    $("#drpBillType").val($("#hidBillType").val());
                }
            }
            function FillSAP() {
                var arrTypeText = ["Select", "GSC/GT", "SBM Reading (MS/SP/Temp/GC)"];
                var arrTypeVal = ["", "DSBELOW10KW", "SBMREADING"];

                $("#drpBillType").html("");

                var index = 0;
                for (index = 0; index < arrTypeText.length; index++) {
                    var el;
                    el = document.createElement("option");
                    el.textContent = arrTypeText[index];
                    el.value = arrTypeVal[index];
                    $("#drpBillType").append(el);
                }
                if ($("#hidBillType").val() != "") {
                    $("#drpBillType").val($("#hidBillType").val());
                }
            }
            $("#pageHeader").load("resources/snippets.html #snipPageHeader")
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvUpload").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvUpload").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");
            $("#FileUpload1").click(function () {
                $("#lblMessage").text("");
            });
            //            $(".billClass").change(ChangeVals);
            //            $("#drpBillType").change(SetHidBillType);
        });
    </script>
</body>
</html>
