using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
public partial class upload : System.Web.UI.Page
{
    void UploadNonSAP(common.Categs categ)
    {
        string path = string.Empty;
        string[] lines;
        int oracle_ins = 0;
        int oracle_dup = 0;
        int oracle_err = 0;
        int start = 0;
        string userID = Session["userID"].ToString();
        //string empID = Session["empID"].ToString();
        string dtUpload = DateTime.Now.ToString(common.dtFmtDotNet);
        string strExtension = string.Empty;
        OracleConnection con;
        string[] qtdFields;
        StringBuilder sbsql = new StringBuilder(2000);
        int Check_NumFields = categ.numFields;
        string sql_backup;

        //capture sessionid
        hidSID.Value = System.Guid.NewGuid().ToString();

        if (FileUpload1.HasFile)
        {
            strExtension = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower();
            if (strExtension == ".txt" || strExtension == ".web")
            {
                path = string.Format("C:/SBM_Bills/BILL_{0}.txt", hidSID.Value);
                //path = Server.MapPath(string.Format("./bills/BILL_{0}.txt", hidSID.Value));
                FileUpload1.SaveAs(path);
            }
            else
            {
                lblMessage.Text = "Invalid File";
                return;
            }
        }

        //open file
        try
        {
            lines = System.IO.File.ReadAllLines(path);
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error Reading File: " + ex.Message;
            return;
        }

        //delete uploaded file
        System.IO.File.Delete(path);

        //if "AccountNo" word at line 0 then start from line 1 else start from line 0
        start = (lines[0].Split(categ.delimiter)[0] == "AccountNo") ? 1 : 0;

        con = OraDBConnection.ConnectionOpen();
        for (int line = start; line < lines.Length; line++)
        {
            //sanitize line
            lines[line] = lines[line].Replace("'", "").Replace("--", "");

            //get fields
            string[] fields = lines[line].Split(categ.delimiter);

            #region invalid_record_check

            /*Support modified DSABOVE (82) and DSBELOW (78) files 
             * along with old format 76 and 72 respectively */
            if (fields.Length == 79 && categ.categName=="DSBELOW10KW")
            {
                Check_NumFields = 79;
            }
            else if (fields.Length == 83 && categ.categName == "DSABOVE10KW")
            {
                Check_NumFields = 83;
            }

            //check the field length for every record 
            //to avoid crash if sbm machine malfunctions
            if (fields.Length != Check_NumFields)
            {
                //if its first line then exit
                //as its an invalid file
                if (line == start)
                {
                    lblMessage.Text = "Error. Invalid File.";
                    return;
                }
                else
                {
                    sbsql.Clear();
                    oracle_err++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL" +
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE) " +
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}')",
                        line + 1, common.strErrStyle, common.strErrStyle, common.strErrStyle,
                        hidSID.Value, dtUpload, common.dtFmtOracle, common.strErrLetter);
                    OraDBConnection.ExecQryOnConnection(con, sbsql.ToString());
                    continue;
                }
            }
            #endregion

            //set/reset sbsql
            sbsql.Clear();

            sbsql.AppendFormat("INSERT INTO {0} VALUES (", categ.tableName);

            //trim the PK fields to remove any whitespace
            fields[categ.posAccountNo] = fields[categ.posAccountNo].Trim().ToUpper();
            fields[categ.posBillCycle] = fields[categ.posBillCycle].Trim();
            fields[categ.posBillYear] = fields[categ.posBillYear].Trim();

            //make the insert query
            qtdFields = fields.Select(n => "'" + n + "'").ToArray();
            sbsql.Append(string.Join(",", qtdFields));

            //for (int field = 0; field < fields.Length; field++)
            //{
            //    sbsql.Append("'")
            //        .Append(fields[field])
            //        .Append("', ");
            //}

            //add userID, empID and date_upload at last, we already have comma and space at the end of string
            //sbsql.AppendFormat("'{0}', '{1}', to_date('{2}','{3}'))", userID, empID, dtUpload, common.dtFmtOracle);

            /*add empty values in the last fields of the tables if old 
             * format of DSA and DSB is being used*/
            if (Check_NumFields == 72 || Check_NumFields == 76)
            //if (Check_NumFields == 72)
            {
                sbsql.AppendFormat(",'','','','','','',''");
            }

            sbsql.AppendFormat(",'{0}', '{1}', to_date('{2}','{3}')); ", userID, '0', dtUpload, common.dtFmtOracle);
            sql_backup=string.Empty;
            //sql_backup = sbsql.ToString().Replace("'", "$#$");

            //insert into oracle 
            //Composite Primary Key: ACCOUNTNO, BILLCYCLE, BILLYEAR
            //to handle dups we let the exception come and then continue
            //in case of error, we record the info about record containing error and continue
            
            //append merge query
            sbsql.AppendFormat("merge into onlinebill.mast_account m1 using " +
                    "(select '{0}' as acno from dual) d on (m1.account_no=d.acno) " +
                    "when matched then update set m1.table_name = '{1}', m1.cname = '{2}', m1.category = '{3}', if_sap = 'N', m1.code_sdiv='{4}', updatedt = sysdate " +
                    "when not matched then insert (m1.account_no, m1.table_name, m1.cname, m1.category, m1.if_sap, m1.code_sdiv, updatedt) values(d.acno,'{1}','{2}','{3}','N','{4}', sysdate); ",
                    fields[categ.posAccountNo].ToUpper(), categ.tableName.ToUpper().Trim(), fields[categ.posCname].ToUpper().Trim(),
                    fields[categ.posCategory].ToUpper().Trim(), "0");

            try
            {
                //make atomic transaction and execute
                OraDBConnection.ExecQryOnConnection(con, string.Format("BEGIN {0} END;",sbsql.ToString()));
                oracle_ins++;
            }
            catch (Exception ex)
            {
                sbsql.Clear();
                if (ex.Message.Contains("ORA-00001:"))
                {
                    oracle_dup++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL"+
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE) "+
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}')",
                        line + 1, fields[categ.posAccountNo], fields[categ.posBillCycle], 
                        fields[categ.posBillYear], hidSID.Value, dtUpload, common.dtFmtOracle, common.strDupLetter);
                }
                else{
                    oracle_err++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL"+
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE, USERID, TBLNAME, QSQL) "+
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}','{8}','{9}','{10}')",
                        line + 1, common.strErrStyle, common.strErrStyle, common.strErrStyle, hidSID.Value,
                        dtUpload, common.dtFmtOracle, common.strErrLetter, userID, categ.tableName.ToUpper().Trim(), sql_backup);
                    //sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL(LINENO, SESSIONID, DATED, TYPE) "+
                    //        "VALUES({0},'{1}',to_date('{2}','{3}'),'{4}')", line + 1, hidSID.Value, dtUpload, common.dtFmtOracle,"E");
                }
                OraDBConnection.ExecQryOnConnection(con, sbsql.ToString());
            }
        }

        OraDBConnection.ConnectionClose(con);

        //enable Export Button if dup rows or error rows
        btnExport.Visible = (oracle_dup > 0 || oracle_err > 0);

        //show success message
        lblMessage.Text = String.Format("Done. Total Rows {0}. Inserted {1}. Duplicate {2}. Error {3}", 
            lines.Length, oracle_ins, oracle_dup, oracle_err);

        //insert summary in userrec
        InsertUserRec(oracle_ins, oracle_dup, oracle_err, dtUpload, categ.categName, common.strNonSAP.ToUpper());
    }
    void UploadSAP(common.Categs categ)
    {
        string path = string.Empty;
        string cypherText;
        string plainText;
        string[] lines;
        int oracle_ins = 0;
        int oracle_dup = 0;
        int oracle_err = 0;
        int start = 0;
        string userID = Session["userID"].ToString();
        //string empID = Session["empID"].ToString();
        string dtUpload = DateTime.Now.ToString(common.dtFmtDotNet);
        string strExtension = string.Empty;
        StringBuilder sbsql = new StringBuilder(2000);
        OracleConnection con;
        string[] qtdFields;
        const int POS_SCHMRDT = 7;
        const int POS_PRVMRDT = 24;
        const int POS_CURMRDT = 18;
        const int POS_BILLDT = 49;
        const int POS_DUEDT = 50;
        //int posSancLoad = 80;

        //capture sessionid
        hidSID.Value = System.Guid.NewGuid().ToString();

        if (FileUpload1.HasFile)
        {
            strExtension = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower();
            if (strExtension == ".enc")
            {
                path = string.Format("C:/SBM_Bills/BILL_{0}.txt", hidSID.Value);
                //path = Server.MapPath(string.Format("./bills/BILL_{0}.txt", hidSID.Value));
                FileUpload1.SaveAs(path);
            }
            else
            {
                lblMessage.Text = "Invalid File";
                return;
            }
        }

        //open file
        try
        {
            cypherText = System.IO.File.ReadAllText(path);
            plainText = common.Decrypt(cypherText);
            lines = plainText.Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error Reading File: " + ex.Message;
            return;
        }

        //delete uploaded file
        System.IO.File.Delete(path);

        if (lines.Length == 0)
        {
            lblMessage.Text = "No Record to Upload";
            return;
        }

        //skip header line, header lines end with H in the last field
        start = (lines[0].Split(categ.delimiter)[6] == "H") ? 1 : 0;

        //open connection to oracle
        con = OraDBConnection.ConnectionOpen();
        for (int line = start; line < lines.Length; line++)
        {
            //sanitize line
            lines[line] = lines[line].Replace("'", "").Replace("--", "");

            //get fields
            string[] fields = lines[line].Split(categ.delimiter);

            #region invalid_record_check
            //check the field length for every record 
            //to avoid crash if sbm m/c malfunctions
            //Supporting SBM Reading Data for MS/SP/Temp Records:
            //only accept this file if 
            //1. for SAP_SBM_READING type MRU field contains the word "READING"
            //2. for other types MRU field doesn't containg the word "READING"
            if (fields.Length != categ.numFields ||
                (categ.categName == "SAP_SBM_READING" &&
                fields[categ.posMRU].Trim().ToUpper() != common.strReading) ||
                (categ.categName != "SAP_SBM_READING" &&
                fields[categ.posMRU].Trim().ToUpper() == common.strReading))
            {
                //if its first line then exit as its an invalid file
                if (line == start)
                {
                    lblMessage.Text = "Error. Invalid File.";
                    return;
                }
                else
                {
                    sbsql.Clear();
                    oracle_err++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL" +
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE) " +
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}')",
                        line + 1, common.strErrStyle, common.strErrStyle, common.strErrStyle,
                        hidSID.Value, dtUpload, common.dtFmtOracle, common.strErrLetter);
                    OraDBConnection.ExecQryOnConnection(con, sbsql.ToString());
                    continue;
                }
            }
            #endregion

            //reset sbsql
            sbsql.Clear();

            sbsql.AppendFormat("INSERT INTO {0} VALUES (", categ.tableName);

            //trim the PK fields to remove any whitespace
            fields[categ.posAccountNo] = fields[categ.posAccountNo].Trim();
            fields[categ.posBillCycle] = fields[categ.posBillCycle].Trim();
            fields[categ.posBillYear] = fields[categ.posBillYear].Trim();

            //make the insert query
            qtdFields = fields.Select(n => "'" + n + "'").ToArray();
            qtdFields[POS_SCHMRDT] = string.Format("to_date({0},'dd/mm/yyyy')", qtdFields[POS_SCHMRDT]);
            qtdFields[POS_PRVMRDT] = string.Format("to_date({0},'dd/mm/yyyy')", qtdFields[POS_PRVMRDT]);
            qtdFields[POS_CURMRDT] = string.Format("to_date({0},'dd-mm-yyyy')", qtdFields[POS_CURMRDT]);
            qtdFields[POS_BILLDT] = string.Format("to_date({0},'dd-mm-yyyy')", qtdFields[POS_BILLDT]);
            qtdFields[POS_DUEDT] = string.Format("to_date({0},'dd-mm-yyyy')", qtdFields[POS_DUEDT]);
            sbsql.Append(string.Join(",", qtdFields));

            //add userID, empID, date_upload, synched, syncmsg, syncdt as NULL at last, we already have comma and space at the end of string
            //add semicolon at end of query to enable it to run in atomic BEGIN END block
            sbsql.AppendFormat(",'{0}', '{1}', to_date('{2}','{3}'), NULL, NULL, NULL); ", userID, '0', dtUpload, common.dtFmtOracle);

            //insert into oracle 
            //Primary Key: MR_DOC_NO
            //to handle dups we let the exception come and then continue
            //in case of error, we record the info about record containing error and continue

            //append merge query, with semicolon at end to make part of BEGIN END block
            //not to merge in case of SAP_SBM_READING
            if (categ.categName != "SAP_SBM_READING")
            {
                sbsql.AppendFormat("merge into onlinebill.mast_account m1 using " +
                    "(select '{0}' as acno from dual) d on (m1.account_no=d.acno) " +
                    "when matched then update set m1.table_name = '{1}', m1.cname = '{2}', m1.category = '{3}', if_sap = 'Y', m1.code_sdiv='{4}', updatedt = sysdate " +
                    "when not matched then insert (m1.account_no, m1.table_name, m1.cname, m1.category, m1.if_sap, m1.code_sdiv, updatedt) values(d.acno,'{1}','{2}','{3}','Y','{4}', sysdate); ",
                    fields[categ.posAccountNo], categ.tableName.ToUpper().Trim(), fields[categ.posCname].ToUpper().Trim(), 
                    fields[categ.posCategory].ToUpper().Trim(), fields[categ.posCode_sdiv].Trim());
            }

            try
            {
                //make atomic transaction and execute
                OraDBConnection.ExecQryOnConnection(con, string.Format("BEGIN {0} END;",sbsql.ToString()));
                oracle_ins++;
            }
            catch (Exception ex)
            {
                sbsql.Clear();
                if (ex.Message.Contains("ORA-00001:"))
                {
                    oracle_dup++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL" +
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE) " +
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}')",
                        line + 1, fields[categ.posAccountNo], fields[categ.posBillCycle],
                        fields[categ.posBillYear], hidSID.Value, dtUpload, common.dtFmtOracle, common.strDupLetter);
                }
                else
                {
                    oracle_err++;
                    sbsql.AppendFormat("INSERT INTO ONLINEBILL.DUPBILL" +
                        "(LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, SESSIONID, DATED, TYPE) " +
                        "VALUES({0},'{1}','{2}','{3}','{4}',to_date('{5}','{6}'),'{7}')",
                        line + 1, common.strErrStyle, common.strErrStyle, common.strErrStyle, 
                        hidSID.Value, dtUpload, common.dtFmtOracle, common.strErrLetter);
                }
                OraDBConnection.ExecQryOnConnection(con, sbsql.ToString());
            }
        }

        OraDBConnection.ConnectionClose(con);
        //enable Export Button if dup rows or error rows
        btnExport.Visible = (oracle_dup > 0 || oracle_err > 0);

        //show success message
        lblMessage.Text = String.Format("Done. Total Rows {0}. Inserted {1}. Duplicate {2}. Error {3}",
            lines.Length, oracle_ins, oracle_dup, oracle_err);

        //insert summary in userrec
        InsertUserRec(oracle_ins, oracle_dup, oracle_err, dtUpload, categ.categName, common.strSAP.ToUpper());
    }
    void InsertUserRec(int numInsRec, int numDupRec, int numErrRec, string dated, string categName, string billClass)
    {
        string userID = Session["userID"].ToString();
        //string empID = Session["empID"].ToString();
        string sql = string.Empty;

        //sql = string.Format("insert into onlinebill.userrec (userid, empid, insrec, duprec, errrec, dated, categ) " +
        //        "values('{0}', '{1}', '{2}', '{3}', '{4}',to_date('{5}', '{6}'), '{7}')",
        //        userID, empID, numInsRec, numDupRec, numErrRec, dated, common.dtFmtOracle, categName);
        sql = string.Format("insert into onlinebill.userrec (userid, empid, insrec, duprec, errrec, dated, categ, class) " +
                "values('{0}', '{1}', '{2}', '{3}', '{4}',to_date('{5}', '{6}'), '{7}', '{8}')",
                userID, '0', numInsRec, numDupRec, numErrRec, dated, common.dtFmtOracle, categName, billClass);
        try
        {
            OraDBConnection.ExecQry(sql);
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error in saving user record: " + ex.Message;
            return;
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
        }
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        string billClass = string.Empty;
        string billType = string.Empty;
        
        if(hidBillType.Value == "") {
            lblMessage.Text = "Select a valid Bill Type";
            return;
        }
        billClass = hidBillClass.Value;
        billType = hidBillType.Value;
        
        if(billClass==common.strSAP){
            switch (billType)
            {
                case "DSBELOW10KW":
                case "DSBELOW20KW":
                    UploadSAP(common.cat_SAP_GSC);
                    break;
                case "SBMREADING":
                    UploadSAP(common.cat_SAP_SBM_READING);
                    break;
                default:
                    lblMessage.Text = "Select a valid Bill Type";
                    return;
            }
        }
        else if(billClass == common.strNonSAP)
        {
            switch (billType)
            {
                case "LS":
                    UploadNonSAP(common.cat_LS);
                    break;
                case "SP":
                    UploadNonSAP(common.cat_SP);
                    break;
                case "MS":
                    UploadNonSAP(common.cat_MS);
                    break;
                case "DSBELOW10KW":
                    UploadNonSAP(common.cat_DSBELOW10KW);
                    break;
                case "DSABOVE10KW":
                    UploadNonSAP(common.cat_DSABOVE10KW);
                    break;
                default:
                    lblMessage.Text = "Select a valid Bill Type";
                    return;
            }
        }
        else
        {
            lblMessage.Text = "Some error occurred. Try after sometime.";
            return;
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        string strFileName;
        string sql = string.Format("select LINENO, ACCOUNTNO, BILLCYCLE, BILLYEAR, DATED, "+
            "decode(TYPE,'E','ERROR','DUPLICATE') as PROBLEM from " +
            "ONLINEBILL.dupbill where SESSIONID = '{0}' ORDER BY LINENO", hidSID.Value);
        strFileName = "not_entered_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

        common.DownloadXLS(sql, strFileName, this);
    }
}