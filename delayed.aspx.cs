using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class reports : System.Web.UI.Page
{
    Dictionary<string, string> dictDueDate = new Dictionary<string, string>()
    {
        {"LS", "DUECASHDD"},
        {"MS", "PRT_DUE_DATE_CASH"},
        {"SP","DUEDATECASH"},
        {"DSABOVE10KW","DUEDATECASH"},
        {"DSBELOW10KW","DUEDATECASH"},
        {"SAP_SBM_GSC","BILL_DATE"},
        {"SAP_SBM_READING","BILL_DATE"}
    };
    Dictionary<string, string> dictMRDate = new Dictionary<string, string>()
    {
        {"LS", "ISSUEDATE"},
        {"MS", "PRT_ISSUE_DATE"},
        {"SP","ISSUEDATE"},
        {"DSABOVE10KW","ISSUEDATE"},
        {"DSBELOW10KW","ISSUEDATE"},
        {"SAP_SBM_GSC","CUR_METER_READING_DATE"},
        {"SAP_SBM_READING","CUR_METER_READING_DATE"}
    };
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
    private void DoAction(bool countOnly)
    {
        string userID = panDelayed_drpUsers.SelectedValue;
        string billType = panDelayed_drpBillType.SelectedValue;
        string sql = string.Empty;
        string sqlUser = string.Empty;
        string strSQLNoNCODE = string.Empty;
        bool xlReturn = false;
        string delayDate = string.Empty;
        string strDelayType = panDelayed_drpDelayedType.SelectedValue;
        string DueDataCol = dictDueDate[billType];
        string MRDateCol = dictMRDate[billType];
        int MRNumDays = 0;
        string sqlAction = "*";
        string strCount = string.Empty;

        if (countOnly)
        {
            sqlAction = "count(*)";
        }

        if (userID != "ALL")
        {
            sqlUser = string.Format("and userid = '{0}'", userID);
        }

        if (billType == "SAP_SBM_GSC")
        {
            strSQLNoNCODE = " and nvl(SYNCED,'NULL')<>'NCODE' ";
        }

        if (strDelayType == "UploadedAfter")
        {
            try
            {
                delayDate = DateTime.ParseExact(sDate.Value, common.dtFmtDotNetShort, null).ToString("yyyyMMdd");
            }
            catch
            {
                //date error, return
                panDelayed_lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
                return;
            }
            sql = string.Format("select {0} from onlinebill.{1} " +
                 "where 1=1 {2} and to_char(dtupload,'yyyymmdd') > '{3}' {4}",
                 sqlAction, billType, sqlUser, delayDate, strSQLNoNCODE);
        }
        else if (strDelayType == "AfterDueDate")
        {
            sql = string.Format("select {0} from onlinebill.{1} " +
                 "where 1=1 {2} and to_char(dtupload,'yyyymmdd') > to_char({3},'yyyymmdd') {4}",
                 sqlAction, billType, sqlUser, DueDataCol, strSQLNoNCODE);
        }
        else if (strDelayType == "MRD_Days")
        {
            try
            {
                MRNumDays = int.Parse(numDays.Value);
            }
            catch (Exception ex)
            {
                panDelayed_lblMsg.Text = ex.Message;
                return;
            }
            sql = string.Format("select {0} from onlinebill.{1} " +
                 "where 1=1 {2} and dtupload > {3}+{4} {5}",
                 sqlAction, billType, sqlUser, MRDateCol, MRNumDays, strSQLNoNCODE);
        }

        if (countOnly)
        {
            strCount = OraDBConnection.GetScalar(sql);
            panDelayed_lblMsg.Text = strCount;
        }
        else
        {
            xlReturn = common.DownloadXLS(sql, billType + ".xls", this);
            if (xlReturn == false)
            {
                panDelayed_lblMsg.Text = "No Record Exist";
            }
        }
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
            fillUsers(panDelayed_drpUsers, false, true);
            numDays.Attributes.Add("type","number");
            //sDate.Attributes["type"] = "date";
            //eDate.Attributes["type"] = "date";
        }
    }
    protected void btnShowCount_Click(object sender, EventArgs e)
    {
        DoAction(true);
    }
    protected void btnShowIndvRecs_Click(object sender, EventArgs e)
    {
        DoAction(false);    
    }
}