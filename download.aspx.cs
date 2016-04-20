using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
//using ExtensionMethods;
using System.Data;
using System.Net.Mail;

public partial class download : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!common.isValidSession(Page.Session))
        {
            Response.Redirect("login.aspx");
        }

        if (!IsPostBack)
        {
            common.FillInfo(Page.Session, lblLoggedInAs);

            if (Session[common.strUserID].ToString() != common.strAdminName)
            {
                //btnEmpty.Visible = false;
            }
            sDate.Attributes["type"] = "date";
            eDate.Attributes["type"] = "date";
        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        //MailMessage mailMsg = new MailMessage();

        //mailMsg.From = new MailAddress("seitpspcl@gmail.com", "PSPCL");

        //string[] tosend_mail_ids = { "bsbhogal@gmail.com", "gurpreet007@gmail.com" };

        //foreach (string str in tosend_mail_ids)
        //    mailMsg.To.Add(new MailAddress(str));

        //mailMsg.Subject = "Bill Payment Data";
        //mailMsg.Body = "Please find attached the payment data.";

        //SmtpClient smtpClient = new SmtpClient();
        //smtpClient.Host = "smtp.gmail.com";
        //smtpClient.EnableSsl = true;
        //smtpClient.UseDefaultCredentials = false;
        //smtpClient.Credentials = new System.Net.NetworkCredential("seitpspcl@gmail.com", "pspcl123");
        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //smtpClient.Send(mailMsg);

        //return;

        string startDate = sDate.Value;
        string endDate = eDate.Value;
        string billType = drpBillType.SelectedValue;
        string circleID = drpCircles.SelectedValue;
        string dtFormat = startDate.Contains("-") ? "yyyy-MM-dd" : "dd/MM/yyyy";
        StringBuilder sbSql;
        StringBuilder sbResult;
        string sql = string.Empty;
        DataSet ds;
        int rowCount;

        lblMessage.Text = string.Empty;

        if (billType == "BT")
        {
            lblMessage.Text = "Select a valid Bill Type";
            return;
        }

        try
        {
            startDate = DateTime.ParseExact(startDate, dtFormat, null).ToString("dd-MMM-yyyy");
            endDate = DateTime.ParseExact(endDate, dtFormat, null).ToString("dd-MMM-yyyy");
        }
        catch (System.FormatException)
        {
            lblMessage.Text = "Invalid Date. Use format dd/mm/yyyy.";
            return;
        }

        if (billType == common.cat_DSBELOW10KW.categName)
        {
            sbSql = new StringBuilder(1000);
            sbSql.Append("select RPAD(ACCOUNTNO,12) acno, RPAD(NAME,20), RPAD(ADDRESS,20), RPAD(ISSUEDATE,11), RPAD(DUEDATECASH,11), ");
            sbSql.Append("RPAD(DUEDATECHEQUE,11), RPAD(VILLCITYNAME,20), RPAD(SUBDIVNAME,18), RPAD(BILLNO,6), RPAD(CURRENTREADINGDATE,11), ");
            sbSql.Append("RPAD(PREVREADINGDATE,11), RPAD(BILLPERIOD,4), RPAD(TARIFFTYPE,3), RPAD(CONNECTEDLOAD,7), RPAD(PREVBILLSTATUS,7), ");
            sbSql.Append("RPAD(METERNO,9), RPAD(SECURITYAMT,7), RPAD(BILLSTATUS,3), RPAD(BILLCYCLE,2), RPAD(BILLGROUP,1), ");
            sbSql.Append("RPAD(METERREADINGCURRENT,8), RPAD(METERREADINGPREV,8), RPAD(LINECTRATIO,5), RPAD(METERCTRATIO,5), ");
            sbSql.Append("RPAD(METERMULTIPLIER,7), RPAD(OVERMETERMULTIPLIER,7), RPAD(METERCODE,1), RPAD(UNITSCONSUMEDNEW,6), ");
            sbSql.Append("RPAD(UNITSCONSUMEDOLD,6), RPAD(TOTALUNITSCONSUMED,6), ");
            sbSql.Append("RPAD(CURRENTSOP,8), RPAD(CURRENTED,7), RPAD(OCTROI,6), RPAD(METERRENT,4), RPAD(SERVICERENT,4), RPAD(ADJAMT,7), ");
            sbSql.Append("RPAD(ADJPERIOD,8), RPAD(ADJREASON,12), RPAD(CONSUNITS,5), RPAD(FIXEDCHARGES,7), ");
            sbSql.Append("RPAD(FUELCOST,5), RPAD(VOLTCLASSCHARGES,6), RPAD(PREVTOTALARR,6), RPAD(CURRENTTOTALARREAR,6), RPAD(OTHERCHARGES,4), ");
            sbSql.Append("RPAD(SUNDARYCHARGES,9), RPAD(SUNDARYALLOWANCE,9), RPAD(CURRENTROUNDEDAMT,2), RPAD(PREVROUNDEDAMT,2), RPAD(TOTALNETSOP,7), ");
            sbSql.Append("RPAD(TOTALNETED,7), RPAD(TOTALNETOCTROI,7), RPAD(TOTALAMT,7), RPAD(TOTALSURCHARGE,7), RPAD(TOTALAMTGROSS,8), ");
            sbSql.Append("RPAD(BILLYEAR,4), RPAD(SUNDARYMESSAGE,19), RPAD(SUNDARYDTFROM,6), RPAD(SUNDARYDTTO,6), RPAD(LINE1,70), ");
            sbSql.Append("RPAD(LINE2,70), RPAD(PREVCYCLECONSUMPTION1,6), RPAD(PREVCYCLECONSUMPTION2,6), RPAD(PREVCYCLECONSUMPTION3,6), ");
            sbSql.Append("RPAD(PREVCYCLECONSUMPTION4,6), RPAD(PREVCYCLECONSUMPTION5,6), RPAD(PREVCYCLECONSUMPTION6,6), RPAD(AMTPAID,7), ");
            sbSql.Append("RPAD(AMTDATE,11), RPAD(WEBCCCR,9), ");
            sbSql.Append("RPAD(FINANCIALYEAR,6), RPAD(SPOTREC,1) from onlinebill.dsbelow10kw ");
            sbSql.AppendFormat("where TRUNC(DTUPLOAD) between '{0}' and '{1}' ", startDate, endDate);
            if (circleID != "ALL")
            {
                sbSql.AppendFormat("and ACCOUNTNO like '{0}%' ", circleID);
            }
        }
        else if (billType == common.cat_DSABOVE10KW.categName)
        {
            sbSql = new StringBuilder(1000);
            sbSql.Append("select RPAD(ACCOUNTNO,12), RPAD(NAME,20), RPAD(ADDRESS,20), RPAD(ISSUEDATE,11), ");
            sbSql.Append("RPAD(DUEDATECASH,11), RPAD(DUEDATECHEQUE,11), RPAD(VILLCITYNAME,20), RPAD(SUBDIVNAME,18), ");
            sbSql.Append("RPAD(BILLNO,6), RPAD(CURRENTREADINGDATE,11), RPAD(PREVREADINGDATE,11), RPAD(BILLPERIOD,4), ");
            sbSql.Append("RPAD(TARRIFTYPE,3), RPAD(CONNECTEDLOAD,7), RPAD(PREVBILLSTATUS,7), RPAD(METERNO,9), ");
            sbSql.Append("RPAD(SECURITYAMT,7), RPAD(METERSTATUS,3), RPAD(BILLCYCLE,2), RPAD(BILLGROUP,1), ");
            sbSql.Append("RPAD(METERREADINGCURRENT,8), RPAD(METERREADINGPREV,8), RPAD(LINECTRATIO,5), RPAD(METERCTRATIO,5), ");
            sbSql.Append("RPAD(METERMULTIPLIER,7), RPAD(OVERMETERMULTIPLIER,7), RPAD(METERCODE,1), RPAD(UNITSCONSUMEDNEW,6), ");
            sbSql.Append("RPAD(UNITSCONSUMEDOLD,6), RPAD(TOTALUNITSCONSUMED,6), RPAD(CURRENTSOP,8), RPAD(CURRENTED,7), ");
            sbSql.Append("RPAD(CURRENTOCTROI,6), RPAD(METERRENT,4), RPAD(SERVICERENT,4), RPAD(TOTALADJAMT,7), ");
            sbSql.Append("RPAD(TOTALADJPERIOD,8), RPAD(TOTALADJREASON,12), RPAD(CONCLUNITS,6), RPAD(FIXEDCHARGES,6), ");
            sbSql.Append("RPAD(FUELCOSTCHARGES,5), RPAD(VOLTAGECLASSCHARGES,6), RPAD(PREVTOTALARREARS,6), RPAD(CURRENTTOTALARREARS,7), ");
            sbSql.Append("RPAD(OTHERCHARGES,4), RPAD(SUNDARYCHARGES,9), RPAD(SUNDARYALLOWANCES,9), RPAD(CURRENTROUNDINGAMT,2), ");
            sbSql.Append("RPAD(PREVROUNDAMT,2), RPAD(TOTALNETSOP,8), RPAD(TOTALNETED,8), RPAD(TOTALNETOCTROI,8), ");
            sbSql.Append("RPAD(TOTALAMT,8), RPAD(TOTALSURCHARGE,7), RPAD(TOTALAMTGROSS,9), RPAD(BILLYEAR,4), ");
            sbSql.Append("RPAD(SUNDARYMESSAGE,19), RPAD(SUNDARYDATEFROM,6), RPAD(SUNDARYDATETO,6), RPAD(LINE1,70), ");
            sbSql.Append("RPAD(LINE2,70), RPAD(TOTALSURCHARGES,7), RPAD(TOTALGROSS,9), RPAD(DUEDATECASH2,9), ");
            sbSql.Append("RPAD(TOTALSURCHARGE2,7), RPAD(TOTALGROSS2,9), RPAD(CONSUMPTIONCYCLE1,6), RPAD(CONSUMPTIONCYCLE2,6), ");
            sbSql.Append("RPAD(CONSUMPTIONCYCLE3,6), RPAD(CONSUMPTIONCYCLE4,6), RPAD(CONSUMPTIONCYCLE5,6), RPAD(CONSUMPTIONCYCLE6,6), ");
            sbSql.Append("RPAD(AMTPAID,7), RPAD(AMTDATE,11), RPAD(WEBCCCR,10), RPAD(FINYEAR,6) ");
            sbSql.Append("from ONLINEBILL.DSABOVE10KW ");
            sbSql.AppendFormat("where TRUNC(DTUPLOAD) between '{0}' and '{1}' ", startDate, endDate);
            if (circleID != "ALL")
            {
                sbSql.AppendFormat("and ACCOUNTNO like '{0}%' ", circleID);
            }
        }
        else
        {
            lblMessage.Text = "Download for this category is not supported";
            return;
        }

        try
        {
            ds = OraDBConnection.GetData(sbSql.ToString());
            rowCount = ds.Tables[0].Rows.Count;
            if (rowCount == 0)
            {
                lblMessage.Text = "No Record for selected criteria";
                return;
            }
            sbResult = new StringBuilder(rowCount * 800);
            foreach (DataRow drow in ds.Tables[0].Rows)
            {
                sbResult.AppendLine(string.Join("\"", drow.ItemArray));
            }

            DownloadFile(billType, circleID, startDate, endDate, sbResult);
        }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message;
        }
    }

    private void DownloadFile(string billType, string circleID, 
        string startDate, string endDate, StringBuilder sbResult)
    {
        string sdate = string.Empty;
        string edate = string.Empty;

        sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null).ToString("yyyyMMdd");
        edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null).ToString("yyyyMMdd");

        string fileName = string.Format("{0}-{1}-{2}-{3}.txt", circleID, "DS", sdate, edate);
        Response.Clear();
        Response.ClearHeaders();

        Response.AddHeader("Content-Length", sbResult.Length.ToString());
        Response.ContentType = "text/plain";
        Response.AppendHeader("content-disposition", "attachment;filename="+fileName);

        Response.Write(sbResult.ToString());
        sbResult.Clear();
        Response.End();
    }

    [System.Web.Services.WebMethod]
    public static string GetSomething(string drpVal)
    {
        string sql = "select count(*) from onlinebill."+drpVal;
        return OraDBConnection.GetScalar(sql);
    }
}