using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class archive : System.Web.UI.Page
{

    private void Clear()
    {
        lblAcNo.Text = string.Empty;
        lblName.Text = string.Empty;
        lblCat.Text = string.Empty;
        lblDtUpload.Text = string.Empty;
    }
    protected void btnShow_Click(object sender, EventArgs e)
    {
        string acno = txtAcno.Value;
        string sql = string.Empty;
        DataSet ds;
        string userid = Session[common.strUserID].ToString();

        if (userid != "CBCPTA")
        {
            lblMsg.Text = "Invalid Operation";
            return;
        }

        lblMsg.Text = string.Empty;

        Clear();        
        if (String.IsNullOrEmpty(acno) || acno.Length != 12)
        {
            lblMsg.Text = "Invalid Account Number";
            return;
        }
        sql = String.Format("select cname, table_name, to_char(updatedt, 'dd-Mon-yyyy hh24:mi:ss') as dtup, if_sap from mast_account where upper(account_no) = upper('{0}')", acno);
        ds = OraDBConnection.GetData(sql);

        if (ds.Tables[0].Rows.Count != 1)
        {
            lblMsg.Text = "No such account number";
            return;
        }
        else if (ds.Tables[0].Rows[0]["if_sap"].ToString() != "N")
        {
            lblMsg.Text = "Only enter Non-SAP Account Numbers";
            return;
        }

        lblAcNo.Text = acno;
        lblName.Text = ds.Tables[0].Rows[0]["cname"].ToString();
        lblCat.Text = ds.Tables[0].Rows[0]["table_name"].ToString().Split('.')[1].ToUpper();
        lblDtUpload.Text = ds.Tables[0].Rows[0]["dtup"].ToString();
        lnkViewBill.NavigateUrl = "https://billpayment.pspcl.in/pgMaster.aspx?uc=Home&acno="+acno;
        lnkViewBill.Visible = true;
        btnArchive.Visible = true;
    }
    protected void btnArchive_Click(object sender, EventArgs e)
    {
        string acno = lblAcNo.Text;
        string tbl = lblCat.Text;
        string userid = Session[common.strUserID].ToString();
        string sql = string.Empty;
        string sqlCond = string.Empty;
        string sqlDel = string.Empty;
        string sqlIns = string.Empty;
        string sqlLog = string.Empty;
        string year = DateTime.Now.Year.ToString();
        DataSet ds;
        Dictionary<string, string> dict_acno = new Dictionary<string, string>(){
                                                   {"DSBELOW10KW","ACCOUNTNO"},
                                                   {"DSABOVE10KW","ACCOUNTNO"},
                                                   {"LS","ACCOUNTNO"},
                                                   {"MS","PRT_AC_NO"},
                                                   {"SP","ACNO"}
                                               };

        if (!dict_acno.ContainsKey(tbl))
        {
            lblMsg.Text = "Invalid Category";
            return;
        }

        sqlCond = string.Format(" upper({0}) = upper('{1}') and upper(userid) = upper('{2}') ", dict_acno[tbl], acno, userid);
        sql = string.Format("select count(*) from {0} where {1}", tbl, sqlCond);
        ds = OraDBConnection.GetData(sql);

        if (ds.Tables[0].Rows[0][0].ToString() != "1")
        {
            lblMsg.Text = "No current bill or not uploaded by this user";
            return;
        }

        sqlIns = string.Format("insert into ARCBILL.ARC_{0}_{1} select * from {1} where {2}", year, tbl, sqlCond);
        sqlDel = string.Format("delete from {0} where {1}", tbl, sqlCond);
        sqlLog = string.Format("insert into ARCHIVEBILL_LOG values ('{0}','{1}', sysdate)", acno, userid);
        sql = string.Format("BEGIN {0}; {1}; {2}; END;", sqlIns, sqlDel, sqlLog);
        try
        {
            OraDBConnection.ExecQry(sql);
            lblMsg.Text = "Record Archived";
        }
        catch (Exception ex)
        {
            lblMsg.Text = ex.Message;
        }
    }
}