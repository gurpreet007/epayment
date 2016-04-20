using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class userconfig : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!common.isValidSession(Page.Session))
        {
            Response.Redirect("login.aspx");
        }

        if (!IsPostBack)
        {
            fillActions();
            common.FillInfo(Page.Session, lblLoggedInAs);
        }
    }
    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("login.aspx");
    }
    private void addUser()
    {
        string userName = panAdd_txtUsername.Value;
        string pass1 = panAdd_txtPass1.Value;
        string pass2 = panAdd_txtPass2.Value;
        string offcName = panAdd_txtOffcName.Value;
        string mobileNo = panAdd_txtMobile.Value;
        string issuedTo = panAdd_txtIssuedTo.Value;
        const string officeType = "SubDivision";
        string sql = string.Empty;

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(pass1) ||
            string.IsNullOrWhiteSpace(pass2) ||
            string.IsNullOrWhiteSpace(offcName) ||
            string.IsNullOrWhiteSpace(mobileNo) ||
            string.IsNullOrWhiteSpace(issuedTo))
        {
            panAdd_lblMsg.Text = "Error: All the fields are necessary.";
            return;
        }
        if (pass1 != pass2)
        {
            panAdd_lblMsg.Text = "Error: Passwords does not match.";
            return;
        }
        if (userName.Length < 3 || userName.Length > 64)
        {
            panAdd_lblMsg.Text = "Error: Username length must be between 3 and 64.";
            return;
        }
        if (pass1.Length < 3 || pass1.Length > 64)
        {
            panAdd_lblMsg.Text = "Error: Password length must be between 3 and 64.";
            return;
        }
        sql = string.Format("insert into onlinebill.users(userid, pass, offcname, officetype, mobileno, issuedto, dated, active)" +
                "values('{0}','{1}','{2}','{3}','{4}','{5}',sysdate,1)", userName.ToUpper(), pass1, offcName, officeType, mobileNo, issuedTo);
        try
        {
            OraDBConnection.ExecQry(sql);
            panAdd_lblMsg.Text = "User added successfully.";
            return;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("ORA-00001"))
            {
                panAdd_lblMsg.Text = "Error: User already exists.";
                return;
            }
            else
            {
                panAdd_lblMsg.Text = "Error: " + ex.Message;
            }
            return;
        }
    }
    private void resetPass()
    {
        string userID = panReset_drpUsers.SelectedValue;
        string pass1 = panReset_txtPass1.Value;
        string pass2 = panReset_txtPass2.Value;
        string sql = string.Empty;

        if (userID == "0")
        {
            return;
        }
        if (pass1 != pass2)
        {
            panReset_lblMsg.Text = "Error: Passwords does not match.";
            return;
        }
        if (pass1.Length < 3 || pass1.Length > 64)
        {
            panReset_lblMsg.Text = "Error: Password length must be between 3 and 64.";
            return;
        }

        sql = string.Format("update onlinebill.users set pass='{0}' where userid='{1}'", pass1, userID);

        try
        {
            OraDBConnection.ExecQry(sql);
            panReset_lblMsg.Text = "Password changed successfully.";
        }
        catch (Exception ex)
        {
            panReset_lblMsg.Text = "Error: " + ex.Message;
            return;
        }
    }
    private void resetUserPass()
    {
        string userID = Session["userID"].ToString();
        string currPass = panUserReset_txtCurrPass.Value;
        string pass1 = panUserReset_txtPass1.Value;
        string pass2 = panUserReset_txtPass2.Value;
        string sql = string.Empty;

        if (pass1 != pass2)
        {
            panUserReset_lblMsg.Text = "Error: Passwords does not match.";
            return;
        }
        if (pass1.Length < 3 || pass1.Length > 64)
        {
            panUserReset_lblMsg.Text = "Error: Password length must be between 3 and 64.";
            return;
        }

        sql = string.Format("select count(*) from onlinebill.users where userid='{0}' and pass='{1}' and active=1", userID, currPass);
        if (OraDBConnection.GetScalar(sql) != "1")
        {
            panUserReset_lblMsg.Text = "Error: Invalid Credentials";
            return;
        }

        sql = string.Format("update onlinebill.users set pass='{0}' where userid='{1}'", pass1, userID);
        try
        {
            OraDBConnection.ExecQry(sql);
            panUserReset_lblMsg.Text = "Password changed successfully.";
        }
        catch (Exception ex)
        {
            panUserReset_lblMsg.Text = "Error: " + ex.Message;
            return;
        }
    }
    private void fillUsers(DropDownList drpUsers, bool fillOnlyActive = true, bool addAdmin = false)
    {
        DataSet ds;
        string strAddAdmin = addAdmin ? "" : string.Format("and upper(userid) <> '{0}'", common.strAdminName);
        string strFillOnlyActive = fillOnlyActive ? "and active=1" : "";
        string sql = string.Format("select userid||' (' || offcname || ')' as usern,"+
            " userid from onlinebill.users where 1=1 {0} {1} order by userid", strAddAdmin, strFillOnlyActive);

        ds = OraDBConnection.GetData(sql);

        drpUsers.DataSource = ds;
        drpUsers.DataTextField = "usern";
        drpUsers.DataValueField = "userid";
        drpUsers.DataBind();

        drpUsers.Items.Insert(0, new ListItem("Select User", "0"));
    }
    private void activateUser()
    {
        string userID = panActivate_drpUsers.SelectedValue;
        string sql;

        if (userID == "0")
        {
            return;
        }
        if (userID == common.strAdminName)
        {
            panActivate_lblActive.Text = "Error: Cannot deactive admin.";
            return;
        }

        sql = string.Format("update onlinebill.users set active = decode(active,0,1,0) where userid='{0}'", userID);

        try
        {
            OraDBConnection.ExecQry(sql);
            panActivate_lblActive.Text = "User activation status changed successfully";
        }
        catch (Exception ex)
        {
            panActivate_lblActive.Text = "Error: " + ex.Message;
        }
    }
    private void fillActions()
    {
        if (Session["userID"].ToString() != common.strAdminName)
        {
            drpActions.Items.Clear();
            drpActions.Items.Add(new ListItem("Select Action", "0"));
            drpActions.Items.Add(new ListItem("Reset User Password", "resetuser"));
        }
    }
    protected void panAdd_AddUser_Click(object sender, EventArgs e)
    {
        addUser();
    }
    protected void panReset_btnReset_Click(object sender, EventArgs e)
    {
        resetPass();
    }
    protected void panActivate_drpUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        string userID = panActivate_drpUsers.SelectedValue;
        string sql = string.Empty;
        bool isActive;

        if (userID == "0")
        {
            panActivate_btnActivate.Text = "(Select User)";
            return;
        }

        sql = string.Format("select active from onlinebill.users where userid='{0}'", userID);
        isActive = (OraDBConnection.GetScalar(sql) == "1");

        panActivate_lblActive.Text = (isActive) ? "Currectly Active" : "Currectly Not Active";
        panActivate_btnActivate.Text = (isActive) ? "Deactivate" : "Activate";
    }
    protected void panActivate_btnActivate_Click(object sender, EventArgs e)
    {
        activateUser();
    }
    protected void drpActions_SelectedIndexChanged(object sender, EventArgs e)
    {
        string action = drpActions.SelectedValue;
        
        panAdd.Visible = false;
        panActivate.Visible = false;
        panReset.Visible = false;
        panUserReset.Visible = false;

        switch (action)
        {
            case "add":
                panAdd.Visible = true;
                break;
            case "reset":
                panReset.Visible = true;
                if (panReset_drpUsers.Items.Count == 0)
                {
                    fillUsers(panReset_drpUsers, true, true);
                }
                break;
            case "activate":
                panActivate.Visible = true;
                if (panActivate_drpUsers.Items.Count == 0)
                {
                    fillUsers(panActivate_drpUsers, false, false);
                }
                break;
            case "resetuser":
                panUserReset.Visible = true;
                break;
        }
    }
    protected void lnkBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("upload.aspx");
    }
    protected void panUserReset_btnReset_Click(object sender, EventArgs e)
    {
        resetUserPass();
    }
}