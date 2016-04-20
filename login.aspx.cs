using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Clear();
    }
    private string GetIPAddress()
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }
        else
        {
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
    private void DoLoginAudit(string userID, string userPass, bool isValid, string ip)
    {
        string sql;
        sql = string.Format("insert into onlinebill.loginaudit values('{0}','{1}',{2},'{3}',sysdate)",
            userID, userPass, isValid ? 1 : 0, ip);
        try
        {
            OraDBConnection.ExecQry(sql);
        }
        catch
        {
            Response.Redirect(Request.RawUrl);
        }
    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string userID = txtUserID.Value.Replace("'","QUOTE").ToUpper();
        string userPass = txtUserPass.Value.Replace("'", "QUOTE");
        //string empID = txtEmpID.Value.Replace("'", "");
        //string hrPass = txtHRPass.Value.Replace("'", "");
        string sql = string.Empty;
        bool appLoginOK = false;
        //bool hrLoginOK = false;

        Session.Clear();

        sql = string.Format("select count(*) from onlinebill.users where upper(userid) = upper('{0}') and pass = '{1}' and active = 1", userID, userPass);
        appLoginOK = OraDBConnection.GetScalar(sql) == "1";

        //if (appLoginOK)
        //{
        //    sql = string.Format("select count(*) from pshr.netlogin where upper(eid) = upper('{0}') and pwd='{1}'", empID, hrPass);
        //    hrLoginOK = OraDBConnection.GetScalar(sql) == "1";
        //}
        //if (appLoginOK && hrLoginOK)

        DoLoginAudit(userID, userPass, appLoginOK, GetIPAddress());

        if (appLoginOK)
        {
            string location = string.Empty;
            //string name = string.Empty;

            sql = string.Format("select offcname from onlinebill.users where upper(userid) = upper('{0}')", userID);
            location = OraDBConnection.GetScalar(sql);

            //sql = string.Format("select pshr.get_fullname({0}) from dual", empID);
            //name = OraDBConnection.GetScalar(sql);

            Session[common.strUserID] = userID;
            //Session[common.strEmpID] = empID;
            Session[common.strLocation] = location;
            //Session[common.strName] = name;

            Response.Redirect("home.aspx");
        }
        else
        {
            lblMsg.Text = "Invalid Credentials";
            return;
        }
    }
}