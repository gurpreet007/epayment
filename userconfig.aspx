<%@ Page Language="C#" AutoEventWireup="true" CodeFile="userconfig.aspx.cs" Inherits="userconfig" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta charset="utf-8">
	<title>e-Payment</title>
    <!--[if lt IE 9]>
        <script src="scripts/html5shiv.min.js"></script>        
   <![endif]-->
	<%--<link rel="stylesheet" href="styles/epayment.css">--%>
    <link rel="stylesheet/less" href="styles/epayment.less" type="text/css" />
    <script src="scripts/less.min.js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="The description of my page" />
</head>
<body>
     <header class="page" id="pageHeader">	</header>
<%--	<div id="loginInfo">
		Logged in as 
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
        (Loc: <asp:Label ID="lblLocation" runat="server"></asp:Label>, 
        Name: <asp:Label ID="lblName" runat="server"></asp:Label>)
	</div>--%>
	<div id="loginInfo">
        <asp:Label ID="lblLoggedInAs" runat="server"></asp:Label> 
	</div>
    <nav id="pageNav"></nav>
    <header class="sectionHeader">User Configuration</header>
    <form runat="server" class="tableWrapper">
        <asp:DropDownList ID="drpActions" runat="server" 
                        onselectedindexchanged="drpActions_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="0">Select Action</asp:ListItem>
                        <asp:ListItem Value="add">Add User</asp:ListItem>
                        <asp:ListItem Value="reset">Reset Password</asp:ListItem>
                        <asp:ListItem Value="activate">Activate / Deactivate User</asp:ListItem>
                    </asp:DropDownList><br><br />
        <asp:Panel class="userConfPanel" ID="panAdd" runat="server" Visible="False">
            <input id="panAdd_txtUsername"      type="text"     runat="server"  placeholder="Username"         maxlength="64"  required/><br />
            <input id="panAdd_txtPass1"         type="password" runat="server"  placeholder="Password"         maxlength="64"  required/><br />
            <input id="panAdd_txtPass2"         type="password" runat="server"  placeholder="Confirm Password" maxlength="64"  required/><br />
            <input id="panAdd_txtOffcName"      type="text"     runat="server"  placeholder="Office Name"      maxlength="200" required/><br />
            <input id="panAdd_txtIssuedTo"      type="text"     runat="server"  placeholder="Issued To"        maxlength="50"  required/><br />
            <input id="panAdd_txtMobile"        type="text"     runat="server"  placeholder="Mobile Number"    maxlength="20"  required/><br />
            <asp:Button ID="panAdd_btnAddUser"  Text="Add User" runat="server"  onclick="panAdd_AddUser_Click" />
            <asp:Label ID="panAdd_lblMsg"       class="msg"     runat="server"></asp:Label>
        </asp:Panel>
        <asp:Panel class="userConfPanel" ID="panReset" runat="server" Visible="False">
            <asp:DropDownList ID="panReset_drpUsers" runat="server">
                                </asp:DropDownList><br /><br />
            <input id="panReset_txtPass1" type="password" runat="server" placeholder="Password" maxlength="64" required/><br />
            <input id="panReset_txtPass2" type="password" runat="server" placeholder="Confirm Password" maxlength="64" required/><br />
            <asp:Button ID="panReset_btnReset" Text="Reset Password" runat="server" 
                                    onclick="panReset_btnReset_Click" />
            <asp:Label ID="panReset_lblMsg" runat="server" class="msg"/>
        </asp:Panel>
        <asp:Panel class="userConfPanel" ID="panActivate" runat="server" Visible="False">
            <asp:DropDownList ID="panActivate_drpUsers" runat="server" AutoPostBack="true" 
                                    onselectedindexchanged="panActivate_drpUsers_SelectedIndexChanged">
                                </asp:DropDownList><br />
            <asp:Label ID="panActivate_lblActive" runat="server" class="msg"></asp:Label><br />
            <asp:Button id="panActivate_btnActivate" runat="server" Text="(Select User)" 
                                    onclick="panActivate_btnActivate_Click" />
        </asp:Panel>
        <asp:Panel class="userConfPanel" ID="panUserReset" runat="server" Visible="False">
            <input id="panUserReset_txtCurrPass" type="password" runat="server" placeholder="Current Password" maxlength="64" required/><br />
            <input id="panUserReset_txtPass1" type="password" runat="server" placeholder="Password" maxlength="64" required/><br />
            <input id="panUserReset_txtPass2" type="password" runat="server" placeholder="Confirm Password" maxlength="64" required/><br />
            <asp:Button ID="panUserReset_btnReset" Text="Reset Password" runat="server" 
                                    onclick="panUserReset_btnReset_Click" />
            <asp:Label ID="panUserReset_lblMsg" runat="server" class="msg"/>
        </asp:Panel>
    </form>
    <footer id="pageFooter" class="pageFooter">	</footer>
    <script src="scripts/jquery-2.2.0.min.js"></script>
    <script>
        $(document).ready(function () {
            $("#pageHeader").load("resources/snippets.html #snipPageHeader");
            $("#pageNav").load("resources/snippets.html #snipPageNav", function () {
                $("#pageNav li").removeClass("selected");
                $("#pageNav #nvUserConfig").addClass("selected");
                $("#pageNav").hover(
                    function () { $("#pageNav li").removeClass("selected"); },
                    function () { $("#pageNav #nvUserConfig").addClass("selected"); }
                );
            });
            $("#pageFooter").load("resources/snippets.html #snipPageFooter");
        });
    </script>
</body>
</html>