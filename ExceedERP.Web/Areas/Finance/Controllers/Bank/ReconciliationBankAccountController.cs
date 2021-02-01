using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    public class ReconciliationBankAccountController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReconcilationBankAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReconciliationBankAccount> reconcilationbankaccounts = _db.ReconcilationBankAccounts;
            DataSourceResult result = reconcilationbankaccounts.ToDataSourceResult(request, reconciliationBankAccount => new {
                ReconciliationBankAccountId = reconciliationBankAccount.ReconciliationBankAccountId,
                BankAccount = reconciliationBankAccount.BankAccount,
                Type = reconciliationBankAccount.Type,
                Reference = reconciliationBankAccount.Reference,
                Debit = reconciliationBankAccount.Debit,
                Credit = reconciliationBankAccount.Credit,
                Date = reconciliationBankAccount.Date,
                Reconciled = reconciliationBankAccount.Reconciled,
                ReconciledDate = reconciliationBankAccount.ReconciledDate,
                Check = reconciliationBankAccount.Check,
                OrgBranchName = reconciliationBankAccount.OrgBranchName,
                DateCreated = reconciliationBankAccount.DateCreated,
                LastModified = reconciliationBankAccount.LastModified,
                CreatedBy = reconciliationBankAccount.CreatedBy,
                ModifiedBy = reconciliationBankAccount.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult UnreconcilationBankAccounts_Read([DataSourceRequest]DataSourceRequest request, int? id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule != null)
            {
                DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1).Date;
                var bankaccount = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);

                IQueryable<ReconciliationBankAccount> reconcilationbankaccounts = _db.ReconcilationBankAccounts
                                                                                        .Where(x => x.BankAccount==bankaccount.AccountNumber)
                                                                                        .Where(x => x.Date < dt2)
                                                                                        .Where(x => x.Reconciled == false ||x.Reconciled && x.ReconciledDate>dt2)//from || new code
                                                                                        .AsQueryable();

                DataSourceResult result = reconcilationbankaccounts.ToDataSourceResult(request, reconciliationBankAccount => new
                {
                    ReconciliationBankAccountId = reconciliationBankAccount.ReconciliationBankAccountId,
                    Type = reconciliationBankAccount.Type,
                    Reference = reconciliationBankAccount.Reference,
                    Debit = reconciliationBankAccount.Debit,
                    Credit = reconciliationBankAccount.Credit,
                    Date = reconciliationBankAccount.Date,
                    Reconciled = reconciliationBankAccount.Reconciled,
                    ReconciledDate = reconciliationBankAccount.ReconciledDate,
                    Check = reconciliationBankAccount.Check,
                    OrgBranchName = reconciliationBankAccount.OrgBranchName,
                    DateCreated = reconciliationBankAccount.DateCreated,
                    LastModified = reconciliationBankAccount.LastModified,
                    CreatedBy = reconciliationBankAccount.CreatedBy,
                    ModifiedBy = reconciliationBankAccount.ModifiedBy
                });

                return Json(result);
            }
            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankAccounts_Create([DataSourceRequest]DataSourceRequest request, ReconciliationBankAccount reconciliationBankAccount,int id)
        {
            ModelState.Clear();

            if (reconciliationBankAccount.Debit != 0 && reconciliationBankAccount.Credit != 0)
                ModelState.AddModelError("both", "Both debit and credit can not have value at same time.");
            if (reconciliationBankAccount.Debit == 0 && reconciliationBankAccount.Credit == 0)
                ModelState.AddModelError("both", "Both debit and credit can not have zero value at same time.");
            if (reconciliationBankAccount.Debit < 0 && reconciliationBankAccount.Credit < 0)
                ModelState.AddModelError("both", "Debit and credit can not have negative value.");

            if (String.IsNullOrWhiteSpace(reconciliationBankAccount.Reference))
                ModelState.AddModelError("ref", "Please add reference.");

            reconciliationBankAccount.Type = "Manual";
            var record = _db.ReconciliationSchedules.Find(id);
            if (record != null)
            {
                reconciliationBankAccount.BankAccount = record.BankAccount;
            
                var entity = new ReconciliationBankAccount
                {
                    BankAccount= reconciliationBankAccount.BankAccount,
                    Type = reconciliationBankAccount.Type,
                    Reference = reconciliationBankAccount.Reference,
                    Debit = reconciliationBankAccount.Debit,
                    Credit = reconciliationBankAccount.Credit,
                    Date = reconciliationBankAccount.Date,
                    Reconciled = false,
                    Check = false,
                    OrgBranchName = reconciliationBankAccount.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = reconciliationBankAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = reconciliationBankAccount.ModifiedBy
                };

                _db.ReconcilationBankAccounts.Add(entity);
                _db.SaveChanges();
                reconciliationBankAccount.ReconciliationBankAccountId = entity.ReconciliationBankAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation bank record", "Success", User.Identity.Name, $"created bank record {entity.BankAccount}  {entity.Reference}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconcilationBankAccounts_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBankAccount reconciliationBankAccount)
        {
            ModelState.Clear();

            if (reconciliationBankAccount.Debit != 0 && reconciliationBankAccount.Credit != 0)
                ModelState.AddModelError("both", "Both debit and credit can not have value at same time.");
            if (reconciliationBankAccount.Debit == 0 && reconciliationBankAccount.Credit == 0)
                ModelState.AddModelError("both", "Both debit and credit can not have zero value at same time.");
            if (reconciliationBankAccount.Debit < 0 && reconciliationBankAccount.Credit < 0)
                ModelState.AddModelError("both", "Debit and credit can not have negative value.");

            if (String.IsNullOrWhiteSpace(reconciliationBankAccount.Reference))
                ModelState.AddModelError("ref", "Please add reference.");

            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankAccount
                {
                    ReconciliationBankAccountId = reconciliationBankAccount.ReconciliationBankAccountId,
                    Type = reconciliationBankAccount.Type,
                    Reference = reconciliationBankAccount.Reference,
                    Debit = reconciliationBankAccount.Debit,
                    Credit = reconciliationBankAccount.Credit,
                    Date = reconciliationBankAccount.Date,
                    Reconciled = reconciliationBankAccount.Reconciled,
                    ReconciledDate = reconciliationBankAccount.ReconciledDate,
                    Check = reconciliationBankAccount.Check,
                    OrgBranchName = reconciliationBankAccount.OrgBranchName,
                    DateCreated = reconciliationBankAccount.DateCreated,
                    LastModified = reconciliationBankAccount.LastModified,
                    CreatedBy = reconciliationBankAccount.CreatedBy,
                    ModifiedBy = reconciliationBankAccount.ModifiedBy
                };

                _db.ReconcilationBankAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation bank record", "Success", User.Identity.Name, $"updated bank record {entity.BankAccount}  {entity.Reference}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankAccount }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationBankAccount reconciliationBankAccount)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankAccount
                {
                    ReconciliationBankAccountId = reconciliationBankAccount.ReconciliationBankAccountId,
                    Type = reconciliationBankAccount.Type,
                    Reference = reconciliationBankAccount.Reference,
                    Debit = reconciliationBankAccount.Debit,
                    Credit = reconciliationBankAccount.Credit,
                    Date = reconciliationBankAccount.Date,
                    Reconciled = reconciliationBankAccount.Reconciled,
                    Check = reconciliationBankAccount.Check,
                    OrgBranchName = reconciliationBankAccount.OrgBranchName,
                    DateCreated = reconciliationBankAccount.DateCreated,
                    LastModified = reconciliationBankAccount.LastModified,
                    CreatedBy = reconciliationBankAccount.CreatedBy,
                    ModifiedBy = reconciliationBankAccount.ModifiedBy
                };

                _db.ReconcilationBankAccounts.Attach(entity);
                _db.ReconcilationBankAccounts.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted reconciliation bank " + reconciliationBankAccount.ReconciliationBankAccountId + " " + reconciliationBankAccount.BankAccount, Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankAccount }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult ReconciliationBankAccounts_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> files, int id)
        {
            #region Import

            DataSet ds = new DataSet();
            var reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            var bankaccount = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
            if (bankaccount != null)
            {
                foreach (HttpPostedFileBase fileBase in files)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["files"].FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                    {
                        string fileLocation = Server.MapPath("~/Content/Import/Finance/Bank") +
                                              DateTime.Now.Year.ToString() +
                                              DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                              DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                              DateTime.Now.Second.ToString() + Request.Files["files"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["files"].SaveAs(fileLocation);
                        var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                    ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                    ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                    ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsm")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                    ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }

                        #region collect data

                        List<ReconciliationBankAccount> imports = new List<ReconciliationBankAccount>();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                            {
                                string reference = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                                string debit = ds.Tables[0].Rows[j].ItemArray[1].ToString();
                                string credit = ds.Tables[0].Rows[j].ItemArray[2].ToString();
                                string date = ds.Tables[0].Rows[j].ItemArray[3].ToString();

                                //handle empty rows
                                if (String.IsNullOrEmpty(debit) && String.IsNullOrEmpty(credit) &&
                                    String.IsNullOrEmpty(reference) && String.IsNullOrEmpty(date))
                                    continue;

                                if (String.IsNullOrEmpty(debit))
                                    debit = "0";
                                if (String.IsNullOrEmpty(credit))
                                    credit = "0";
                                try
                                {
                                    
                                    int day = int.Parse(ds.Tables[0].Rows[j].ItemArray[3].ToString().Split(' ')[0]);
                                    int month = int.Parse(ds.Tables[0].Rows[j].ItemArray[3].ToString().Split(' ')[1]);
                                    int year = int.Parse(ds.Tables[0].Rows[j].ItemArray[3].ToString().Split(' ')[2]);
                                    date = new DateTime(year,month,day).ToShortDateString();
                                }
                                catch (Exception e)
                                {
                                    date = reconciliationSchedule?.Date.ToString();
                                }

                                var import = new ReconciliationBankAccount
                                {
                                    BankAccount = bankaccount.AccountNumber,
                                    Type = "Manual",
                                    Reconciled = false,
                                    Check = false,
                                    Reference = reference,
                                    Debit = decimal.Parse(debit),
                                    Credit = decimal.Parse(credit),
                                    Date = DateTime.Parse(date),
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now
                                };

                                imports.Add(import);
                            }
                        }

                        if (ModelState.IsValid)
                        {
                            _db.ReconcilationBankAccounts.AddRange(imports);
                            _db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation", "Success", User.Identity.Name, $"imported bank transaction for {bankaccount.AccountNumber}  {bankaccount.AccountCode}", Modules.Finance);
                            #endregion
                        }

                        #endregion

                    }

                }
            }


            #endregion
           
            return Json(new[] { reconciliationSchedule }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
