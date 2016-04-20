using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

public class common
{
    #region consts
    public const string strAdminName = "ADMIN";
    public const string strUserID = "userID";
    public const string strEmpID = "empID";
    public const string strLocation = "location";
    public const string strName = "name";
    public const string strSAP = "sap";
    public const string strNonSAP = "nonsap";
    public const string strIV = "C05A8F2F1C83121A";
    public const string strErrStyle = "&lt;ERR&gt;";
    public const string strErrLetter = "E";
    public const string strDupLetter = "D";
    public const string strReading = "READING";
    public const string dtFmtDotNet = "dd-MMM-yyyy HH:mm:ss";
    public const string dtFmtDotNetShort = "dd-MMM-yyyy";
    public const string dtFmtOracle = "DD-MON-YYYY HH24:MI:SS";
    public const string keystr = "1234567890abcdefabcdefab";
    #endregion

    private struct Info
    {
        public struct LS
        {
            public const string CATEG_NAME = "LS";
            public const string TABLE_NAME = "ONLINEBILL.LS";
            public const char DELIMITER = '"';
            public const int POS_CATEGORY = 6;
            public const int POS_CODESDIV = -1;
            public const int POS_CNAME = 3;
            public const int POS_MRU = -1;
            public const int POS_BILLYEAR = 2;
            public const int POS_BILLCYCLE = 1;
            public const int POS_ACNO = 0;
            public const int NUMCOLS = 111;
        }
        public struct MS
        {
            public const string CATEG_NAME = "MS";
            public const string TABLE_NAME = "ONLINEBILL.MS";
            public const char DELIMITER = '"';
            public const int POS_CATEGORY = 21;
            public const int POS_CODESDIV = -1;
            public const int POS_CNAME = 1;
            public const int POS_MRU = -1;
            public const int POS_BILLYEAR = 81;
            public const int POS_BILLCYCLE = 28;
            public const int POS_ACNO = 0;
            public const int NUMCOLS = 92;
        }
        public struct SP
        {
            public const string CATEG_NAME = "SP";
            public const string TABLE_NAME = "ONLINEBILL.SP";
            public const char DELIMITER = '"';
            public const int POS_CATEGORY = 12;
            public const int POS_CODESDIV = -1;
            public const int POS_CNAME = 1;
            public const int POS_MRU = -1;
            public const int POS_BILLYEAR = 68;
            public const int POS_BILLCYCLE = 18;
            public const int POS_ACNO = 0;
            public const int NUMCOLS = 69;
        }
        public struct DSBELOW10KW
        {
            public const string CATEG_NAME = "DSBELOW10KW";
            public const string TABLE_NAME = "ONLINEBILL.DSBELOW10KW";
            public const char DELIMITER = '"';
            public const int POS_CATEGORY = 12;
            public const int POS_CODESDIV = -1;
            public const int POS_CNAME = 1;
            public const int POS_MRU = -1;
            public const int POS_BILLYEAR = 55;
            public const int POS_BILLCYCLE = 18;
            public const int POS_ACNO = 0;
            public const int NUMCOLS = 72;
        }
        public struct DSABOVE10KW
        {
            public const string CATEG_NAME = "DSABOVE10KW";
            public const string TABLE_NAME = "ONLINEBILL.DSABOVE10KW";
            public const char DELIMITER = '"';
            public const int POS_CATEGORY = 12;
            public const int POS_CODESDIV = -1;
            public const int POS_CNAME = 1;
            public const int POS_MRU = -1;
            public const int POS_BILLYEAR = 55;
            public const int POS_BILLCYCLE = 18;
            public const int POS_ACNO = 0;
            public const int NUMCOLS = 76;
        }
        public struct SAP_SBM_GSC
        {
            public const string CATEG_NAME = "SAP_SBM_GSC";
            public const string TABLE_NAME = "ONLINEBILL.SAP_SBM_GSC";
            public const char DELIMITER = ',';
            public const int POS_CATEGORY = 75;
            public const int POS_CODESDIV = 0;
            public const int POS_CNAME = 72;
            public const int POS_MRU = 1;
            public const int POS_BILLYEAR = 18;
            public const int POS_BILLCYCLE = 48;
            public const int POS_ACNO = 11;
            public const int NUMCOLS = 122;
        }
        public struct SAP_SBM_READING
        {
            public const string CATEG_NAME = "SAP_SBM_READING";
            public const string TABLE_NAME = "ONLINEBILL.SAP_SBM_READING";
            public const char DELIMITER = ',';
            public const int POS_CATEGORY = 75;
            public const int POS_CODESDIV = 0;
            public const int POS_CNAME = 72;
            public const int POS_MRU = 1;
            public const int POS_BILLYEAR = 18;
            public const int POS_BILLCYCLE = 48;
            public const int POS_ACNO = 11;
            public const int NUMCOLS = 122;
        }
    }
    public struct Categs
    {
        public readonly string categName;
        public readonly int posAccountNo;
        public readonly int posBillCycle;
        public readonly int posBillYear;
        public readonly int posMRU;
        public readonly string tableName;
        public readonly char delimiter;
        public readonly int numFields;
        public readonly int posCname;
        public readonly int posCategory;
        public readonly int posCode_sdiv;


        public Categs(string categName, int posAccountNo, int posBillCycle,
            int posBillYear, int posMRU, string tableName, char delimiter, int numFields, 
            int posCname, int posCategory, int posCode_sdiv)
        {
            this.categName = categName;
            this.posAccountNo = posAccountNo;
            this.posBillCycle = posBillCycle;
            this.posBillYear = posBillYear;
            this.posMRU = posMRU;
            this.tableName = tableName;
            this.delimiter = delimiter;
            this.numFields = numFields;
            this.posCname = posCname;
            this.posCategory = posCategory;
            this.posCode_sdiv = posCode_sdiv;
        }
    };

    //SAP
    public readonly static Categs cat_SAP_GSC = new Categs(Info.SAP_SBM_GSC.CATEG_NAME, Info.SAP_SBM_GSC.POS_ACNO, Info.SAP_SBM_GSC.POS_BILLCYCLE,
        Info.SAP_SBM_GSC.POS_BILLYEAR, Info.SAP_SBM_GSC.POS_MRU, Info.SAP_SBM_GSC.TABLE_NAME, Info.SAP_SBM_GSC.DELIMITER, Info.SAP_SBM_GSC.NUMCOLS,
        Info.SAP_SBM_GSC.POS_CNAME, Info.SAP_SBM_GSC.POS_CATEGORY, Info.SAP_SBM_GSC.POS_CODESDIV);

    public readonly static Categs cat_SAP_SBM_READING = new Categs(Info.SAP_SBM_READING.CATEG_NAME, Info.SAP_SBM_READING.POS_ACNO, Info.SAP_SBM_READING.POS_BILLCYCLE,
        Info.SAP_SBM_READING.POS_BILLYEAR, Info.SAP_SBM_READING.POS_MRU, Info.SAP_SBM_READING.TABLE_NAME, Info.SAP_SBM_READING.DELIMITER, Info.SAP_SBM_READING.NUMCOLS,
        Info.SAP_SBM_READING.POS_CNAME, Info.SAP_SBM_READING.POS_CATEGORY, Info.SAP_SBM_READING.POS_CODESDIV);

    //Non SAP
    public readonly static Categs cat_LS = new Categs(Info.LS.CATEG_NAME, Info.LS.POS_ACNO, Info.LS.POS_BILLCYCLE, 
        Info.LS.POS_BILLYEAR, Info.LS.POS_MRU, Info.LS.TABLE_NAME, Info.LS.DELIMITER, Info.LS.NUMCOLS, 
        Info.LS.POS_CNAME, Info.LS.POS_CATEGORY, Info.LS.POS_CODESDIV);

    public readonly static Categs cat_SP = new Categs(Info.SP.CATEG_NAME, Info.SP.POS_ACNO, Info.SP.POS_BILLCYCLE, 
        Info.SP.POS_BILLYEAR, Info.SP.POS_MRU, Info.SP.TABLE_NAME, Info.SP.DELIMITER, Info.SP.NUMCOLS, 
        Info.SP.POS_CNAME, Info.SP.POS_CATEGORY, Info.SP.POS_CODESDIV);

    public readonly static Categs cat_MS = new Categs(Info.MS.CATEG_NAME, Info.MS.POS_ACNO, Info.MS.POS_BILLCYCLE, 
        Info.MS.POS_BILLYEAR, Info.MS.POS_MRU, Info.MS.TABLE_NAME, Info.MS.DELIMITER, Info.MS.NUMCOLS, 
        Info.MS.POS_CNAME, Info.MS.POS_CATEGORY, Info.MS.POS_CODESDIV);

    public readonly static Categs cat_DSBELOW10KW = new Categs(Info.DSBELOW10KW.CATEG_NAME, Info.DSBELOW10KW.POS_ACNO, Info.DSBELOW10KW.POS_BILLCYCLE, 
        Info.DSBELOW10KW.POS_BILLYEAR, Info.DSBELOW10KW.POS_MRU, Info.DSBELOW10KW.TABLE_NAME, Info.DSBELOW10KW.DELIMITER, Info.DSBELOW10KW.NUMCOLS, 
        Info.DSBELOW10KW.POS_CNAME, Info.DSBELOW10KW.POS_CATEGORY, Info.DSBELOW10KW.POS_CODESDIV);

    public readonly static Categs cat_DSABOVE10KW = new Categs(Info.DSABOVE10KW.CATEG_NAME, Info.DSABOVE10KW.POS_ACNO, Info.DSABOVE10KW.POS_BILLCYCLE, 
        Info.DSABOVE10KW.POS_BILLYEAR, Info.DSABOVE10KW.POS_MRU, Info.DSABOVE10KW.TABLE_NAME, Info.DSABOVE10KW.DELIMITER, Info.DSABOVE10KW.NUMCOLS, 
        Info.DSABOVE10KW.POS_CNAME, Info.DSABOVE10KW.POS_CATEGORY, Info.DSABOVE10KW.POS_CODESDIV);

    public static bool DownloadXLS(string sql, string filename, Page pg)
    {
        System.Data.DataSet ds = OraDBConnection.GetData(sql);

        if (ds.Tables[0].Rows.Count == 0)
        {
            return false;
        }
        DataGrid dg = new DataGrid();
        dg.DataSource = ds;
        dg.DataBind();
        pg.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
        pg.Response.Charset = "";
        pg.Response.ContentType = "application/vnd.xls";
        System.IO.StringWriter stringwrite = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter htmlwrite = new System.Web.UI.HtmlTextWriter(stringwrite);
        //htmlwrite.WriteLine("TITLE");
        dg.RenderControl(htmlwrite);
        pg.Response.Write(stringwrite.ToString());
        pg.Response.End();
        dg.Dispose();
        return true;
    }
    public static void FillInfo(System.Web.SessionState.HttpSessionState mySession, Label lblLoggedIn)
    {
        //lblLoggedIn.Text = string.Format("Loc: {0} ({1}),  User: {2} ({3})", 
        //    mySession[strLocation], mySession[strUserID], mySession[strName], mySession[strEmpID]);
        lblLoggedIn.Text = string.Format("Loc: {0} ({1})", mySession[strLocation], mySession[strUserID]);
    }
    public static bool isValidSession(System.Web.SessionState.HttpSessionState mySession)
    {
        //if (mySession[strUserID] == null ||
        //   mySession[strEmpID] == null ||
        //   mySession[strUserID].ToString() == string.Empty ||
        //   mySession[strEmpID].ToString() == string.Empty ||
        //   mySession[strEmpID].ToString().Length != 6)
        if (mySession[strUserID] == null ||
           mySession[strUserID].ToString() == string.Empty)
        {
            return false;
        }
        return true;
    }
    public static string HexAsciiConvert(string hex)
    {

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= hex.Length - 2; i += 2)
        {

            sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hex.Substring(i, 2),
            System.Globalization.NumberStyles.HexNumber))));
        }
        return sb.ToString();
    }
    public static string Decrypt(string cipherString, bool useHashing=false)
    {
        byte[] keyArray;
        byte[] keyIv;
        byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        string ivstr = HexAsciiConvert(strIV);
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keystr));
            keyIv = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(ivstr));
            hashmd5.Clear();
        }
        else
        {
            keyArray = ASCIIEncoding.ASCII.GetBytes(keystr);
            keyIv = ASCIIEncoding.ASCII.GetBytes(ivstr);
        }
        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.KeySize = 192;
        tdes.Key = keyArray;
        tdes.IV = keyIv;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        tdes.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    public static bool getDates(string sDate, string eDate, string duration, out string startDate, out string endDate)
    {
        startDate = DateTime.Now.ToString(common.dtFmtDotNetShort);
        endDate = DateTime.Now.ToString(common.dtFmtDotNetShort);
        try
        {
            switch (duration)
            {
                case "dates":
                    startDate = DateTime.ParseExact(sDate, common.dtFmtDotNetShort, null).ToString(common.dtFmtDotNetShort);
                    endDate = DateTime.ParseExact(eDate, common.dtFmtDotNetShort, null).ToString(common.dtFmtDotNetShort);
                    break;
                case "day1":
                    startDate = endDate;
                    break;
                case "day2":
                    startDate = DateTime.ParseExact(endDate, common.dtFmtDotNetShort, null).AddDays(-1).ToString(common.dtFmtDotNetShort);
                    break;
                case "day3":
                    startDate = DateTime.ParseExact(endDate, common.dtFmtDotNetShort, null).AddDays(-2).ToString(common.dtFmtDotNetShort);
                    break;
                case "week":
                    startDate = DateTime.ParseExact(endDate, common.dtFmtDotNetShort, null).AddDays(-7).ToString(common.dtFmtDotNetShort);
                    break;
                case "month":
                    startDate = DateTime.ParseExact(endDate, common.dtFmtDotNetShort, null).AddMonths(-1).ToString(common.dtFmtDotNetShort);
                    break;
                case "year":
                    startDate = DateTime.ParseExact(endDate, common.dtFmtDotNetShort, null).AddYears(-1).ToString(common.dtFmtDotNetShort);
                    break;
            }
        }
        catch (System.FormatException)
        {
            return false;
        }
        return true;
    }
}

namespace ExtensionMethods
{
    public static class Extensions
    {
        public static StringBuilder FixedFormat(this string str, StringBuilder sb, int desiredWidth)
        {
            sb.Clear();
            if (str.Length >= desiredWidth)
            {
                sb.Append(str, 0, desiredWidth);
            }
            else
            {
                sb.Append(str);
                sb.Append(' ', desiredWidth - str.Length);
            }
            return sb;
        }
    }
}