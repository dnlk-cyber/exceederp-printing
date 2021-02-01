using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace ExceedERP.Helpers.Finance
{
    public partial class FinanceHelper
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["ExceedDbContext"].ConnectionString;

        private void PostRawSql1(int journalId, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            var gLRecordJournal = GetJournalById(journalId);

            if (gLRecordJournal != null)
            {
                var journallines = GetJournalDetails(journalId).ToList();

                foreach (var journalline in journallines)
                {
                    string journalNumber = journalline.GlRecordJournalId + "-" + journalline.GlRecordJournalDetailId;

                    if (IsLinePosted(journalNumber))
                        continue;

                    #region search if ledger is already created

                    //var ledger = context.GLLedgerPostings.FirstOrDefault(x =>
                    //    x.AccountCode.ToUpper() == journalline.AccountCode.ToUpper());

                    var period = GetPeriod(gLRecordJournal.PeriodName.ToUpper());

                    #endregion

                    int ledgerId = GetLedgerId(journalline.AccountCode);

                    if (ledgerId != 0)
                    {
                        #region post ledger line

                        var ledgerpostline = new GLLedgerPostingLines
                        {
                            GLLedgerPostingID = ledgerId,
                            JournalNumber = journalNumber,
                            JournalReference = gLRecordJournal.JournalNumber,
                            EffectiveDate = gLRecordJournal.EffectiveDate,
                            JournalName = gLRecordJournal.JournalName,
                            CategoryName = String.IsNullOrEmpty(gLRecordJournal.CategoryName) ? " " : gLRecordJournal.CategoryName,
                            SourcesName = gLRecordJournal.SourcesName,
                            Remark = String.IsNullOrEmpty(gLRecordJournal.Remark)?" ": gLRecordJournal.Remark,
                            AdditionalDescription = String.IsNullOrEmpty(journalline.AdditionalDescription) ? " " : journalline.AdditionalDescription,
                            ReferenceDoc = String.IsNullOrEmpty(gLRecordJournal.ReferenceDoc) ? " " : gLRecordJournal.ReferenceDoc,
                            InvoiceReferenceDoc = String.IsNullOrEmpty(gLRecordJournal.InvoiceReferenceDoc) ? " " : gLRecordJournal.InvoiceReferenceDoc,
                            PaymentReference = String.IsNullOrEmpty(gLRecordJournal.PaymentReference) ? " " : gLRecordJournal.PaymentReference, Description ="NA",
                            CreatedBy = user,
                            DateCreated = DateTime.Now,
                            LastModified=DateTime.Now,
                            ModifiedBy="raw sql",
                            OrgBranchName="1"
                        };
                        if (period != null)
                        {
                            ledgerpostline.GLPeriodId = period.GLPeriodId;
                            ledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                        }

                        if (journalline.Debit > 0)
                        {
                            #region debit balance

                            ledgerpostline.Debit = journalline.Debit;
                            ledgerpostline.Credit = 0;

                            #endregion
                        }
                        else if (journalline.Credit > 0)
                        {
                            #region credit balance

                            ledgerpostline.Credit = journalline.Credit;
                            ledgerpostline.Debit = 0;

                            #endregion
                        }

                        //context.GLLedgerPostingLines.Add(ledgerpostline);
                        //context.SaveChanges();
                        AddLedgerLine(ledgerpostline);

                        #endregion
                    }
                    else
                    {
                        #region create ledger

                        var newledgerpost = new GLLedgerPosting
                        {
                            AccountCode = journalline.AccountCode.ToUpper()
                        };
                        AccountHelper accountHelper = new AccountHelper();
                        newledgerpost.AccountDesc = accountHelper.GetAccountDesc(newledgerpost.AccountCode);
                        newledgerpost.AccountType = accountHelper.GetAccountType(newledgerpost.AccountCode);
                        newledgerpost.AccountSegment = "NA";
                        newledgerpost.BranchSegment = "NA";
                        newledgerpost.SubAccountSegment = "NA";
                        newledgerpost.OrgBranchName = "NA";
                        newledgerpost.CreatedBy = "NA";
                        newledgerpost.DateCreated = DateTime.Now;
                        newledgerpost.ModifiedBy = "NA";
                        newledgerpost.LastModified = DateTime.Now;
                        //context.GLLedgerPostings.Add(newledgerpost);
                        //context.SaveChanges();

                        AddLedger(newledgerpost);

                        #endregion

                        #region post ledger line

                        var newledgerpostline = new GLLedgerPostingLines
                        {
                            GLLedgerPosting = newledgerpost,
                            JournalNumber = journalNumber,
                            JournalReference = gLRecordJournal.JournalNumber,
                            EffectiveDate = gLRecordJournal.EffectiveDate,
                            JournalName = gLRecordJournal.JournalName,
                            CategoryName = String.IsNullOrEmpty(gLRecordJournal.CategoryName) ? " " : gLRecordJournal.CategoryName,
                            SourcesName = gLRecordJournal.SourcesName,
                            Remark = String.IsNullOrEmpty(gLRecordJournal.Remark) ? " " : gLRecordJournal.Remark,
                            AdditionalDescription = String.IsNullOrEmpty(journalline.AdditionalDescription) ? " " : journalline.AdditionalDescription,
                            ReferenceDoc = String.IsNullOrEmpty(gLRecordJournal.ReferenceDoc) ? " " : gLRecordJournal.ReferenceDoc,
                            InvoiceReferenceDoc = String.IsNullOrEmpty(gLRecordJournal.InvoiceReferenceDoc) ? " " : gLRecordJournal.InvoiceReferenceDoc,
                            PaymentReference = String.IsNullOrEmpty(gLRecordJournal.PaymentReference) ? " " : gLRecordJournal.PaymentReference,
                            Description = "NA",
                            CreatedBy = user,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            ModifiedBy = "raw sql",
                            OrgBranchName = "1"
                        };
                        if (period != null)
                        {
                            newledgerpostline.GLPeriodId = period.GLPeriodId;
                            newledgerpostline.GlFiscalYearId = period.GlFiscalYearId;
                        }

                        if (journalline.Debit > 0)
                        {
                            #region debit balance

                            newledgerpostline.Debit = journalline.Debit;
                            newledgerpostline.Credit = 0;

                            #endregion
                        }
                        else if (journalline.Credit > 0)
                        {
                            #region credit balance

                            newledgerpostline.Credit = journalline.Credit;
                            newledgerpostline.Debit = 0;

                            #endregion
                        }

                        //context.GLLedgerPostingLines.Add(newledgerpostline);
                        //context.SaveChanges();

                        AddLedgerLine(newledgerpostline);

                        #endregion

                        #region split ledger account

                        if (accountHelper.IsDynamicCoA())
                        {
                            _dynamicAccountHelper.SplitLedgerAccount(newledgerpost);
                        }
                        else
                        {
                            accountHelper.SplitLedgerAccount(newledgerpost);
                        }

                        #endregion
                    }

                }

                #region update journal posting status

                gLRecordJournal.Posting = "POSTED";
                gLRecordJournal.Status = "POSTED";
                gLRecordJournal.isPosted = true;
                gLRecordJournal.PostedBy = user;
                gLRecordJournal.PostedTime = DateTime.Now;
                db.Entry(gLRecordJournal).State = EntityState.Modified;
                db.SaveChanges();

                #endregion

                #region update ledger balances

                //update ledger
                foreach (var journalline in journallines)
                {
                    UpdateBegEndBalances(journalline.AccountCode, gLRecordJournal.PeriodName,user);
                }

                #endregion
            }

        }


        public GlRecordJournal GetJournalById(int? id)
        {
            GlRecordJournal glRecordJournal = new GlRecordJournal();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT * FROM EXCEED.GlRecordJournal WHERE GlRecordJournalId= " + id;
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    glRecordJournal.GlRecordJournalId = Convert.ToInt32(rdr["GlRecordJournalId"]);
                    glRecordJournal.Description = rdr["Description"].ToString();
                    glRecordJournal.JournalNumber = rdr["JournalNumber"].ToString();
                    glRecordJournal.SourcesName = rdr["SourcesName"].ToString();
                    glRecordJournal.CategoryName = rdr["CategoryName"].ToString();
                    glRecordJournal.JournalName = rdr["JournalName"].ToString();
                    glRecordJournal.PeriodName = rdr["PeriodName"].ToString();
                    glRecordJournal.EffectiveDate = DateTime.Parse(rdr["EffectiveDate"].ToString());
                }
            }
            return glRecordJournal;
        }
        public IEnumerable<GLRecordJournalDetail> GetJournalDetails(int id)
        {
            List<GLRecordJournalDetail> list = new List<GLRecordJournalDetail>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT * FROM  EXCEED.GLRecordJournalDetail WHERE GlRecordJournalId= " + id;
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    GLRecordJournalDetail employee = new GLRecordJournalDetail
                    {
                        GlRecordJournalId = Convert.ToInt32(rdr["GlRecordJournalId"]),
                        GlRecordJournalDetailId = Convert.ToInt32(rdr["GlRecordJournalDetailId"]),
                        AccountCode = rdr["AccountCode"].ToString(),
                        Debit = decimal.Parse(rdr["Debit"].ToString()),
                        Credit = decimal.Parse(rdr["Credit"].ToString()),
                        AdditionalDescription = rdr["AdditionalDescription"].ToString()
                    };


                    list.Add(employee);
                }
                con.Close();
            }
            return list;
        }

        public void AddLedgerLine(GLLedgerPostingLines line)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqladd = "Insert into  EXCEED.GLLedgerPostingLines (GLLedgerPostingID,JournalNumber,EffectiveDate,Debit," +
                                "Credit,Balance,OrgBranchName,DateCreated,LastModified,CreatedBy,ModifiedBy,Type," +
                                "GlFiscalYearId,GLPeriodId,JournalName,Description,JournalReference,CategoryName,SourcesName,Remark,AdditionalDescription," +
                                "ReferenceDoc,PaymentReference,InvoiceReferenceDoc) " +
                                "Values(@GLLedgerPostingID,@JournalNumber,@EffectiveDate,@Debit,@Credit,@Balance,@OrgBranchName,@DateCreated,@LastModified,@CreatedBy,@ModifiedBy,@Type,@GlFiscalYearId,@GLPeriodId,@JournalName,@Description,@JournalReference,@CategoryName,@SourcesName,@Remark" +
                                ",@AdditionalDescription,@ReferenceDoc,@PaymentReference,@InvoiceReferenceDoc)";
                SqlCommand cmd = new SqlCommand(sqladd, con);
               

                cmd.Parameters.AddWithValue("@GLLedgerPostingID", line.GLLedgerPostingID);
                cmd.Parameters.AddWithValue("@JournalNumber", line.JournalNumber);
                cmd.Parameters.AddWithValue("@EffectiveDate", line.EffectiveDate);
                cmd.Parameters.AddWithValue("@Debit", line.Debit);
                cmd.Parameters.AddWithValue("@Credit", line.Credit);
                cmd.Parameters.AddWithValue("@Balance", line.Balance);
                cmd.Parameters.AddWithValue("@OrgBranchName", line.OrgBranchName);
                cmd.Parameters.AddWithValue("@DateCreated", line.DateCreated);
                cmd.Parameters.AddWithValue("@LastModified", line.LastModified);
                cmd.Parameters.AddWithValue("@CreatedBy", line.CreatedBy);
                cmd.Parameters.AddWithValue("@ModifiedBy", line.ModifiedBy);
                cmd.Parameters.AddWithValue("@Type", line.Type);
                cmd.Parameters.AddWithValue("@GlFiscalYearId", line.GlFiscalYearId);
                cmd.Parameters.AddWithValue("@GLPeriodId", line.GLPeriodId);
                cmd.Parameters.AddWithValue("@JournalName", line.JournalName);
                cmd.Parameters.AddWithValue("@Description", line.Description);
                cmd.Parameters.AddWithValue("@JournalReference", line.JournalReference);
                cmd.Parameters.AddWithValue("@CategoryName", line.CategoryName);
                cmd.Parameters.AddWithValue("@SourcesName", line.SourcesName);
                cmd.Parameters.AddWithValue("@Remark", line.Remark);
                cmd.Parameters.AddWithValue("@AdditionalDescription", line.AdditionalDescription);
                cmd.Parameters.AddWithValue("@ReferenceDoc", line.ReferenceDoc);
                cmd.Parameters.AddWithValue("@PaymentReference", line.PaymentReference);
                cmd.Parameters.AddWithValue("@InvoiceReferenceDoc", line.InvoiceReferenceDoc);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public int AddLedger(GLLedgerPosting ledger)
        {
            int insertedId = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sqladd = "Insert into  EXCEED.GLLedgerPosting (AccountCode,AccountDesc,AccountType,AccountSegment,BranchSegment,OrgBranchName,DateCreated,LastModified,CreatedBy,ModifiedBy) " +
                                "values" +"(@AccountCode,@AccountDesc,@AccountType,@AccountSegment,@BranchSegment,@OrgBranchName,@DateCreated,@LastModified,@CreatedBy,@ModifiedBy" +
                                "); SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(sqladd, con);


               
                cmd.Parameters.AddWithValue("@AccountCode", ledger.AccountCode);
                cmd.Parameters.AddWithValue("@AccountDesc", ledger.AccountDesc);
                cmd.Parameters.AddWithValue("@AccountType", ledger.AccountType);
                cmd.Parameters.AddWithValue("@AccountSegment", ledger.AccountSegment);
                cmd.Parameters.AddWithValue("@BranchSegment", ledger.BranchSegment);
                cmd.Parameters.AddWithValue("@OrgBranchName", ledger.OrgBranchName);
                cmd.Parameters.AddWithValue("@DateCreated", ledger.DateCreated);
                cmd.Parameters.AddWithValue("@LastModified", ledger.LastModified);
                cmd.Parameters.AddWithValue("@CreatedBy", ledger.CreatedBy);
                cmd.Parameters.AddWithValue("@ModifiedBy", ledger.ModifiedBy);

                insertedId = Convert.ToInt32(cmd.ExecuteScalar());

               
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return insertedId;
        }
        public bool IsLinePosted(string journalNumber)
        {

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT JournalNumber FROM  EXCEED.GLLedgerPostingLines WHERE JournalNumber= '" + journalNumber + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    return true;
                }
            }
            return false;
        }
        public int GetLedgerId(string accountCode)
        {
            int ledgerid = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT GLLedgerPostingID FROM  EXCEED.GLLedgerPosting WHERE AccountCode= '" + accountCode + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                   ledgerid= Convert.ToInt32(rdr["GLLedgerPostingID"]);
                }
            }
            return ledgerid;
        }
        public GLPeriod GetPeriod(string periodName)
        {
            GLPeriod glPeriod = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT GLPeriodId,GlFiscalYearId FROM  EXCEED.GLPeriod WHERE Name= '" + periodName + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    glPeriod = new GLPeriod
                    {
                        GLPeriodId = Convert.ToInt32(rdr["GLPeriodId"]),
                        GlFiscalYearId = Convert.ToInt32(rdr["GlFiscalYearId"])
                    };
                }
            }
            return glPeriod;
        }
    }
}
