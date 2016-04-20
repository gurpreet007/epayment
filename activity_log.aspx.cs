using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class reports : System.Web.UI.Page
{
    private void fillUsers(DropDownList drpUsers, bool fillOnlyActive = true, bool addAdmin = false)
    {
        DataSet ds;
        bool isAdmin = Session[common.strUserID].ToString().Equals(common.strAdminName);
        string sql = string.Empty;
        string strAddAdmin = addAdmin ? string.Empty : string.Format("and upper(userid) <> '{0}'", common.strAdminName);
        string strFillOnlyActive = fillOnlyActive ? "and active=1" : string.Empty;
        string strUser = isAdmin ? string.Empty : string.Format("and upper(userid) = '{0}'", Session[common.strUserID].ToString());
        string strOnlyExisting = "and userid in (select distinct userid from onlinebill.userrec)";

        sql = string.Format("select userid||' (' || offcname || ')' as usern," +
                " userid from onlinebill.users where 1=1 {0} {1} {2} {3} order by userid", 
                strAddAdmin, strFillOnlyActive, strUser, strOnlyExisting);
        ds = OraDBConnection.GetData(sql);

        drpUsers.DataSource = ds;
        drpUsers.DataTextField = "usern";
        drpUsers.DataValueField = "userid";
        drpUsers.DataBind();

        if (isAdmin)
        {
            drpUsers.Items.Insert(0, new ListItem("All Users", "ALL"));
        }
        ds.Clear();
        ds.Dispose();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!common.isValidSession(Page.Session))
        {
            Response.Redirect("login.aspx");
        }

        if (!IsPostBack)
        {
            common.FillInfo(Page.Session, lblLoggedInAs);
            fillUsers(panActivity_drpUsers, false, true);
            //sDate.Attributes["type"] = "date";
            //eDate.Attributes["type"] = "date";
        }
    }
    protected void btnUserActivity_Click(object sender, EventArgs e)
    {
        string userID = panActivity_drpUsers.SelectedValue;
        string billType = panActivity_drpBillType.SelectedValue;
        string sql = string.Empty;
        string sqlUser = string.Empty;
        string sqlBillType = string.Empty;
        string sDate2 = string.Empty;
        string eDate2 = string.Empty;
        bool xlReturn = false;

        if (!common.getDates(sDate.Value, eDate.Value, panActivity_drpDuration.SelectedValue, out sDate2, out eDate2))
        {
            //date error, return
            panActivity_lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
            return;
        }

        if(userID != "ALL") {
            sqlUser = string.Format("and userid = '{0}'",userID);
        }
        if(billType != "ALL") {
            sqlBillType = string.Format("and categ = '{0}'",billType);
        }
        //sql = string.Format("select userid, categ as category, empid, " +
        //         "insrec as Rec_Inserted, du5prec as Rec_Duplicate, " +
        //         "to_char(dated,'DD-MON-YYYY HH24:MI:SS') as dated from onlinebill.userrec " +
        //         "where 1=1 {0} {1} and dated between '{2}' and '{3}' order by dated desc", 
        //         sqlUser, sqlBillType, sDate, eDate);
        sql = string.Format("select userid, categ as category, class as BillClass, " +
                 "insrec as Rec_Inserted, duprec as Rec_Duplicate, " +
                 "to_char(dated,'DD-MON-YYYY HH24:MI:SS') as dated from onlinebill.userrec " +
                 "where 1=1 {0} {1} and trunc(dated) between '{2}' and '{3}' order by dated desc",
                 sqlUser, sqlBillType, sDate2, eDate2);
        xlReturn = common.DownloadXLS(sql, "activity.xls", this);

        
        if(xlReturn == false)
        {
            panActivity_lblMsg.Text = "No Record Exist";
        }
    }
    protected void btnShowCount_Click(object sender, EventArgs e)
    {
        string userID = panActivity_drpUsers.SelectedValue;
        string billType = panActivity_drpBillType.SelectedValue;
        string sql = string.Empty;
        string sqlUser = string.Empty;
        string sDate2 = string.Empty;
        string eDate2 = string.Empty;
        string strCount = string.Empty;
        string strSQLSynced = string.Empty;

        if (!common.getDates(sDate.Value, eDate.Value, panActivity_drpDuration.SelectedValue, out sDate2, out eDate2))
        {
            //date error, return
            panActivity_lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
            return;
        }

        if (userID != "ALL")
        {
            sqlUser = string.Format("and userid = '{0}'", userID);
        }
        if (billType == "ALL")
        {
            panActivity_lblMsg.Text = "Please select a Bill Type";
            return;
        }
        else if (billType == "SAP_SBM_GSC")
        {
            strSQLSynced = " and nvl(SYNCED,'NULL')<>'NCODE' ";
        }

        sql = string.Format("select count(*) from onlinebill.{0} " +
                 "where 1=1 {1} and trunc(dtupload) between '{2}' and '{3}' {4}",
                 billType, sqlUser, sDate2, eDate2, strSQLSynced);

        strCount = OraDBConnection.GetScalar(sql);
        panActivity_lblMsg.Text = strCount;
    }
    protected void btnShowIndvRecs_Click(object sender, EventArgs e)
    {
        string userID = panActivity_drpUsers.SelectedValue;
        string billType = panActivity_drpBillType.SelectedValue;
        string sql = string.Empty;
        string sqlUser = string.Empty;
        string sDate2 = string.Empty;
        string eDate2 = string.Empty;
        string strCount = string.Empty;
        string strSQLSynced = string.Empty;
        bool xlReturn = false;

        if (!common.getDates(sDate.Value, eDate.Value, panActivity_drpDuration.SelectedValue, out sDate2, out eDate2))
        {
            //date error, return
            panActivity_lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
            return;
        }
        
        if (userID != "ALL")
        {
            sqlUser = string.Format("and userid = '{0}'", userID);
        }
        if (billType == "ALL")
        {
            panActivity_lblMsg.Text = "Please select a Bill Type";
            return;
        }
        else if (billType == "SAP_SBM_GSC")
        {
            strSQLSynced = " and nvl(SYNCED,'NULL')<>'NCODE' ";
        }

        sql = string.Format("select * from onlinebill.{0} " +
                 "where 1=1 {1} and trunc(dtupload) between '{2}' and '{3}' {4}",
                 billType, sqlUser, sDate2, eDate2, strSQLSynced);

        xlReturn = common.DownloadXLS(sql, billType+".xls", this);


        if (xlReturn == false)
        {
            panActivity_lblMsg.Text = "No Record Exist";
        }
    }
}