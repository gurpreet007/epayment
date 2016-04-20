using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

public partial class index : System.Web.UI.Page
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
            //valEmpID.Value = Session[common.strEmpID].ToString();
            valUserID.Value = Session[common.strUserID].ToString();
        }
    }
    [System.Web.Services.WebMethod]
    public static string GetCounts()
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        string[] arrCounts = new string[5];
        System.Data.DataSet ds;

        string sql = "select " +
            "(select count(*) from onlinebill.ls) lscnt," +
            "(select count(*) from onlinebill.ms) mscnt," +
            "(select count(*) from onlinebill.sp) spcnt," +
            "(select count(*) from onlinebill.dsabove10kw) dsacnt," +
            "(select count(*) from onlinebill.dsbelow10kw) dsbcnt " +
            "from dual";

        ds = OraDBConnection.GetData(sql);
        arrCounts[0] = ds.Tables[0].Rows[0]["lscnt"].ToString();
        arrCounts[1] = ds.Tables[0].Rows[0]["mscnt"].ToString();
        arrCounts[2] = ds.Tables[0].Rows[0]["spcnt"].ToString();
        arrCounts[3] = ds.Tables[0].Rows[0]["dsacnt"].ToString();
        arrCounts[4] = ds.Tables[0].Rows[0]["dsbcnt"].ToString();
        return js.Serialize(arrCounts);
    }
    [System.Web.Services.WebMethod]
    public static string GetDates(string userID, string empID)
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        string[] arrCounts = new string[6];
        System.Data.DataSet ds;

        //string sql = string.Format("select"+
        //    "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') "+
        //    "from onlinebill.ls where userid='{0}' and empid ='{1}') lsDt," +
        //    "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') "+
        //    "from onlinebill.ms where userid='{0}' and empid ='{1}') msDt," +
        //    "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') "+
        //    "from onlinebill.sp where userid='{0}' and empid ='{1}') spDt," +
        //    "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') "+
        //    "from onlinebill.dsabove10kw where userid='{0}' and empid ='{1}') dsaDt," +
        //    "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') "+
        //    "from onlinebill.dsbelow10kw where userid='{0}' and empid ='{1}') dsbDt " +
        //    "from dual", userID,empID
        //    );

        string sql = string.Format("select" +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.ls where userid='{0}') lsDt," +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.ms where userid='{0}') msDt," +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.sp where userid='{0}') spDt," +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.dsabove10kw where userid='{0}') dsaDt," +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.dsbelow10kw where userid='{0}') dsbDt, " +
            "(select nvl(to_char(max(dtupload),'DD-MON-YYYY HH24:MI:SS'),'Never') " +
            "from onlinebill.sap_sbm_gsc where userid='{0}') dsbSapDt " +
            "from dual", userID, empID
            );
        ds = OraDBConnection.GetData(sql);
        arrCounts[0] = ds.Tables[0].Rows[0]["lsDt"].ToString();
        arrCounts[1] = ds.Tables[0].Rows[0]["msDt"].ToString();
        arrCounts[2] = ds.Tables[0].Rows[0]["spDt"].ToString();
        arrCounts[3] = ds.Tables[0].Rows[0]["dsaDt"].ToString();
        arrCounts[4] = ds.Tables[0].Rows[0]["dsbDt"].ToString();
        arrCounts[5] = ds.Tables[0].Rows[0]["dsbSapDt"].ToString();
        return js.Serialize(arrCounts);
    }
}