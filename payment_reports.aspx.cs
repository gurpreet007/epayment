using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;
public partial class reports : System.Web.UI.Page
{
    private void fillVendors()
    {
        DataSet ds;
        string sql = "select vid as ven_val, vname as ven_text from ONLINEBILL.MASTER_PAYMENT_VENDOR order by vname";

        ds = OraDBConnection.GetData(sql);
        drpVendor.DataSource = ds;
        drpVendor.DataValueField = "ven_val";
        drpVendor.DataTextField = "ven_text";
        drpVendor.DataBind();
        ds.Dispose();

        drpVendor.Items.Insert(0, new ListItem("All", "ALL"));
    }
    private void fillCategories(string type = "sapd")
    {
        DataSet ds;
        string sql;
        //string sql = "select "+
        //"distinct decode(if_sap,'Y','SAP_','N','NonSAP_') || upper(trim(category)) as cat_text,"+
        //"upper(trim(category)) as cat_val,"+
        //"if_sap from payment order by if_sap desc, cat_text";

        if (type == "sapd" || type == "saps")
        {
            sql = "select distinct 'SAP_' || substr(trim(category),1,2) as cat_text, " +
                    "substr(trim(category),1,2) as cat_val from payment where if_sap = 'Y'";
            txtLoc.Attributes.Add("Placeholder", "e.g 12, 1234");
        }
        else
        {
            sql = "select distinct 'Non_SAP_' || trim(tbl_name) as cat_text, " +
            "trim(tbl_name) as cat_val from payment where if_sap='N' ";
            txtLoc.Attributes.Add("Placeholder", "e.g U, U31");
        }
        
        ds = OraDBConnection.GetData(sql);
        drpCategory.DataSource = ds;
        drpCategory.DataValueField = "cat_val";
        drpCategory.DataTextField = "cat_text";
        drpCategory.DataBind();
        ds.Dispose();
    }
    private void showDetailedReport()
    {
        string sql;
        string sDate2 = string.Empty;
        string eDate2 = string.Empty;
        bool isSAP;
        string loc;

        lblMsg.Text = "";
        if (!common.getDates(sDate.Value, eDate.Value, panActivity_drpDuration.SelectedValue, out sDate2, out eDate2))
        {
            //date error, return
            lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
            return;
        }

        isSAP = drpType.SelectedValue.StartsWith("sap");
        loc = txtLoc.Value.Trim().Trim('0').PadRight(isSAP ? 4 : 3, '_').ToUpper();

        if ((Session[common.strUserID].ToString().ToUpper() != common.strAdminName) &&
            ((isSAP && !Regex.IsMatch(loc, "^[1-9][1-9_]{3}$")) ||
            (!isSAP && !Regex.IsMatch(loc, "^[A-Z][1-9_]{2}$")))
            )
        {
            lblMsg.Text = "Invalid Location.";
            return;
        }

        if ((isSAP && !(drpCategory.SelectedItem.Text.StartsWith("SAP_"))) ||
            (!isSAP && !(drpCategory.SelectedItem.Text.StartsWith("Non_SAP_"))))
        {
            lblMsg.Text = "Invalid Category.";
            return;
        }
        sql = string.Format("select * from payment where " +
                "{0} = trim('{1}') " +
                "and txndate between to_date('{2} 00:00:00','{8}') and to_date('{3} 23:59:59','{8}') " +
            //"and txndate between '{2}' and '{3}' " +
                "and upper({4}) like '{5}' " +
                "and {6} and {7} " +
                "and if_sap = '{9}' " +
                "and status_p = 'SUCCESS' "+
            //"and recon_id is not null " +
                "order by txnid",
                isSAP ? "substr(trim(category),1,2)" : "tbl_name",                                                      //0
                drpCategory.SelectedValue,                                                                              //1
                sDate2,                                                                                                 //2
                eDate2,                                                                                                 //3
                isSAP ? "code_sdiv" : "substr(acno,1,3)",                                                               //4
                loc,                                                                                                    //5
                drpPayMode.SelectedValue == "ALL" ? "1=1" : "payment_mode = '" + drpPayMode.SelectedValue + "'",        //6
                drpVendor.SelectedValue == "ALL" ? "1=1" : "vid = '" + drpVendor.SelectedValue + "'",                   //7
                common.dtFmtOracle,                                                                                     //8
                (isSAP ? "Y" : "N"));                                                                                   //9
       
        if (!common.DownloadXLS(sql, "payment.xls", this))
        {
            lblMsg.Text = "No Such Record";
        }
    }
    private void showSummaryReport()
    {
        string sql;
        string sDate2 = string.Empty;
        string eDate2 = string.Empty;
        bool isSAP;
        string loc;
        string vendortext = string.Empty;
        string vendorid = string.Empty;
        lblMsg.Text = "";

        if (!common.getDates(sDate.Value, eDate.Value, panActivity_drpDuration.SelectedValue, out sDate2, out eDate2))
        {
            //date error, return
            lblMsg.Text = "Invalid Date. Use format DD-Mon-YYYY (e.g. 01-Jan-2016).";
            return;
        }

        isSAP = drpType.SelectedValue.StartsWith("sap");
        loc = txtLoc.Value.Trim().PadRight(isSAP ? 4 : 3, '_').ToUpper();

        if ((Session[common.strUserID].ToString().ToUpper() != common.strAdminName) && 
            ((isSAP && !Regex.IsMatch(loc, "^[1-9][1-9_]{3}$")) ||
            (!isSAP && !Regex.IsMatch(loc, "^[A-Z][1-9_]{2}$")))
            )
        {
            lblMsg.Text = "Invalid Location.";
            return;
        }

        if ((isSAP && !(drpCategory.SelectedItem.Text.StartsWith("SAP_"))) ||
           (!isSAP && !(drpCategory.SelectedItem.Text.StartsWith("Non_SAP_"))))
        {
            lblMsg.Text = "Invalid Category.";
            return;
        }

        vendortext = drpVendor.SelectedItem.Text;
        vendorid = drpVendor.SelectedValue;

        //0 = ifsap Y/N
        //1 = from date
        //2 = to date
        //3 = sap - code_sdiv, non_sap - substr(acno,1,3)
        //4 = location
        sql = string.Format("select GET_CDSNAME(a.subdivcode,'{0}','S') as SubDivisonName, a.* from ( " +
                "SELECT upper({3}) as subdivcode, " +
                "category as cat, " +
                "to_char(txndate,'dd/mm/yy') as dte, " +
                "count(*) num_trans, " +
                "sum(sop) as ssop, sum(ed) as sed, " +
                "sum(octrai) soct, sum(tot_amt) as gross_amount, " +
                "sum(surcharge) as ssur, sum(amt_after_duedate) as net_amount, " +
                "sum(amt) as amt_paid " +
                "FROM payment WHERE " +
                "txndate BETWEEN to_date('{1} 00:00:00','{6}') and to_date('{2} 23:59:59','{6}') " +
                "AND upper({3}) LIKE '{4}' " +
                "AND if_sap = '{5}' " +
                "AND status_p = 'SUCCESS' " +
                "AND {7} = trim('{8}') " +
                "{9} " +
                "group by to_char(txndate,'dd/mm/yy'), category, upper({3})) a " +
                "order by a.dte",
                (isSAP ? "Y" : "N"),                                            //0
                sDate2,                                                         //1
                eDate2,                                                         //2
                isSAP ? "code_sdiv" : "substr(acno,1,3)",                       //3
                loc,                                                            //4
                isSAP ? "Y" : "N",                                              //5
                common.dtFmtOracle,                                             //6
                isSAP ? "substr(trim(category),1,2)" : "tbl_name",              //7
                drpCategory.SelectedValue,                                      //8
                vendortext == "All" ? "" : "AND VID = " + vendorid
                );

        if (!common.DownloadXLS(sql, "summary_"+vendortext+".xls", this))
        {
            lblMsg.Text = "No Such Record";
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
            //fillUsers(panActivity_drpUsers, false, true);
            //sDate.Attributes["type"] = "date";
            //eDate.Attributes["type"] = "date";
            fillCategories();
            fillVendors();
        }
    }
    protected void drpType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string type = drpType.SelectedValue;

        if (type == "saps" || type == "nonsaps")
        {
            //divVendor.Visible = false;
            divPayMode.Visible = false;
            //divCategory.Visible = false;
        }
        else
        {
            //divVendor.Visible = true;
            divPayMode.Visible = true;
            //divCategory.Visible = true;
        }
        fillCategories(drpType.SelectedValue);
    }
    protected void btnShowReport_Click(object sender, EventArgs e)
    {
        if (drpType.SelectedValue == "sapd" || drpType.SelectedValue == "nonsapd")
        {
            showDetailedReport();
        }
        else
        {
            showSummaryReport();
        }

    }
}