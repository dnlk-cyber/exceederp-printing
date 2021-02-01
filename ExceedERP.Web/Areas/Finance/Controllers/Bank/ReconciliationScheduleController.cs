using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    public class ReconciliationScheduleController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        public ActionResult Index()
        {
            var tax = _db.BankAccounts.ToList();

            ViewData["accounts"] = tax
                   .Select(e => new BankAccount
                   {
                       AccountNumber = e.AccountNumber,
                       AccountName = e.AccountNumber + "-" + e.AccountName
                   })
                   .OrderBy(e => e.AccountName);

            return View();
        }

        public ActionResult ReconciliationSchedules_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReconciliationSchedule> reconciliationschedules = _db.ReconciliationSchedules;
            DataSourceResult result = reconciliationschedules.ToDataSourceResult(request, reconciliationSchedule => new
            {
                ReconciliationScheduleId = reconciliationSchedule.ReconciliationScheduleId,
                BankAccount = reconciliationSchedule.BankAccount,
                StatementNum = reconciliationSchedule.StatementNum,
                Date = reconciliationSchedule.Date,
                BankEndingBalance = reconciliationSchedule.BankEndingBalance,
                IsSendForApproval = reconciliationSchedule.IsSendForApproval,
                IsOnlineTransferred = reconciliationSchedule.IsOnlineTransferred,
                IsOnlineTransferredBy = reconciliationSchedule.IsOnlineTransferredBy,
                OnlineTransferredTime = reconciliationSchedule.OnlineTransferredTime,
                Status = reconciliationSchedule.Status,
                IsOnlineApproved = reconciliationSchedule.IsOnlineApproved,
                OnlineApprovedBy = reconciliationSchedule.OnlineApprovedBy,
                OnlineApprovedTime = reconciliationSchedule.OnlineApprovedTime,
                IsVoid = reconciliationSchedule.IsVoid,
                VoidBy = reconciliationSchedule.VoidBy,
                VoidTime = reconciliationSchedule.VoidTime,
                OrgBranchName = reconciliationSchedule.OrgBranchName,
                DateCreated = reconciliationSchedule.DateCreated,
                LastModified = reconciliationSchedule.LastModified,
                CreatedBy = reconciliationSchedule.CreatedBy,
                ModifiedBy = reconciliationSchedule.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationSchedules_Create([DataSourceRequest]DataSourceRequest request, ReconciliationSchedule reconciliationSchedule)
        {
            #region validation

            var period = _fiscalYearHelper.GetPeriodOfDate(reconciliationSchedule.Date);
            var periodObj = _db.GLPeriods.Find(period);

            var exisit = _db.ReconciliationSchedules
                .Where(x => x.Date >= periodObj.DateFrom && x.Date <= periodObj.DateTo)
                .Where(x=>x.BankAccount==reconciliationSchedule.BankAccount).ToList();
            if(exisit.Any())
                ModelState.AddModelError("exits","A schedule exisits in the same period.");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ReconciliationSchedule
                {
                    BankAccount = reconciliationSchedule.BankAccount,
                    StatementNum = reconciliationSchedule.StatementNum,
                    Date = reconciliationSchedule.Date.Date,
                    BankEndingBalance = reconciliationSchedule.BankEndingBalance,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                    DateCreated = DateTime.Now,
                    LastModified = reconciliationSchedule.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = reconciliationSchedule.ModifiedBy
                };

                _db.ReconciliationSchedules.Add(entity);
                _db.SaveChanges();
                reconciliationSchedule.ReconciliationScheduleId = entity.ReconciliationScheduleId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation schedule", "Success", User.Identity.Name, $"created schedule {entity.ReconciliationScheduleId} of {entity.BankAccount}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationSchedules_Update([DataSourceRequest]DataSourceRequest request, ReconciliationSchedule reconciliationSchedule)
        {
            #region validation

            var period = _fiscalYearHelper.GetPeriodOfDate(reconciliationSchedule.Date);
            var periodObj = _db.GLPeriods.Find(period);

            var exisit = _db.ReconciliationSchedules
                .Where(x => x.Date >= periodObj.DateFrom && x.Date <= periodObj.DateTo)
                .Where(x => x.BankAccount == reconciliationSchedule.BankAccount)
                .Where(x=>x.ReconciliationScheduleId!=reconciliationSchedule.ReconciliationScheduleId).ToList();

            if (exisit.Any())
                ModelState.AddModelError("exits", "A schedule exisits in the same period.");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationSchedule
                {
                    ReconciliationScheduleId = reconciliationSchedule.ReconciliationScheduleId,
                    BankAccount = reconciliationSchedule.BankAccount,
                    StatementNum = reconciliationSchedule.StatementNum,
                    Date = reconciliationSchedule.Date.Date,
                    BankEndingBalance = reconciliationSchedule.BankEndingBalance,
                    OrgBranchName = reconciliationSchedule.OrgBranchName,
                    DateCreated = reconciliationSchedule.DateCreated,

                    IsSendForApproval = reconciliationSchedule.IsSendForApproval,
                    IsOnlineTransferred = reconciliationSchedule.IsOnlineTransferred,
                    IsOnlineTransferredBy = reconciliationSchedule.IsOnlineTransferredBy,
                    OnlineTransferredTime = reconciliationSchedule.OnlineTransferredTime,
                    Status = reconciliationSchedule.Status,
                    IsOnlineApproved = reconciliationSchedule.IsOnlineApproved,
                    OnlineApprovedBy = reconciliationSchedule.OnlineApprovedBy,
                    OnlineApprovedTime = reconciliationSchedule.OnlineApprovedTime,
                    IsVoid = reconciliationSchedule.IsVoid,
                    VoidBy = reconciliationSchedule.VoidBy,
                    VoidTime = reconciliationSchedule.VoidTime,
                    LastModified = DateTime.Now,
                    CreatedBy = reconciliationSchedule.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ReconciliationSchedules.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation schedule", "Success", User.Identity.Name, $"updated schedule {entity.ReconciliationScheduleId} of {entity.BankAccount}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationSchedules_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationSchedule reconciliationSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationSchedule
                {
                    ReconciliationScheduleId = reconciliationSchedule.ReconciliationScheduleId,
                    BankAccount = reconciliationSchedule.BankAccount,
                    StatementNum = reconciliationSchedule.StatementNum,
                    Date = reconciliationSchedule.Date,
                    BankEndingBalance = reconciliationSchedule.BankEndingBalance,
                    OrgBranchName = reconciliationSchedule.OrgBranchName,
                    DateCreated = reconciliationSchedule.DateCreated,
                    LastModified = reconciliationSchedule.LastModified,
                    CreatedBy = reconciliationSchedule.CreatedBy,
                    ModifiedBy = reconciliationSchedule.ModifiedBy
                };

                _db.ReconciliationSchedules.Attach(entity);
                _db.ReconciliationSchedules.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted bank reconciliation " + reconciliationSchedule.ReconciliationScheduleId + " " + reconciliationSchedule.BankAccount, Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationSchedule }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        // [Authorize(Roles = Bank + "," + Bank_Admin)]
        public ActionResult Reconcile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule == null)
            {
                return HttpNotFound();
            }
            //DateTime dt1 = _fiscalYearHelper.GetStartOfMonth(reconciliationSchedule.Date).Date; //old
            //DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).Date.AddDays(1);//old

            DateTime dt1 = _fiscalYearHelper.GetStartOfPeriod(reconciliationSchedule.Date).Date;//new
            DateTime dt2 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).Date.AddDays(1);//new

            var bankaccountobj = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);

            var beginningbookdata = _db.ReconcilationBookAccounts.Where(x => x.Date < dt1).ToList();

            var bookdata = _db.ReconcilationBookAccounts.Where(x => x.Date >= dt1)
                                                             .Where(x => x.Date < dt2).ToList();

            var unreconcileddata = _db.ReconcilationBookAccounts.Where(x => x.Date < dt2)
                                                                .Where(x => x.Reconciled == false).ToList();

            ViewBag.Unreconciledbook = Convert.ToDecimal(unreconcileddata.Sum(x => x.Debit) - unreconcileddata.Sum(x => x.Credit)).ToString("#,##0.00");

            var unreconciledbakdata = _db.ReconcilationBankAccounts.Where(x => x.Date < dt2)
                                                   .Where(x => x.Reconciled == false).ToList();

            ViewBag.UnreconciledBank = Convert.ToDecimal(unreconciledbakdata.Sum(x => x.Debit) - unreconciledbakdata.Sum(x => x.Credit)).ToString("#,##0.00");

            decimal tDeposits = 0;
            decimal tPayments = 0;
            decimal tUnreconciledDiff = 0;

            decimal bookBeginingBalance = 0;
            decimal bookEndingBalance = 0;
            decimal sumOfDebits = 0;
            decimal sumOfCredits = 0;

            if (beginningbookdata.Any())
            {
                //bookBeginingBalance = beginningbookdata.Sum(x => x.Debit) - beginningbookdata.Sum(x => x.Credit);
            }

            if (bookdata.Any())
            {
                sumOfDebits = bookdata.Sum(x => x.Debit);
                sumOfCredits = bookdata.Sum(x => x.Credit);
            }

            //bookEndingBalance = bookBeginingBalance + sumOfDebits - sumOfCredits;

            #region book beginning and ending balance

            int periodid = _fiscalYearHelper.GetPeriodOfDate(reconciliationSchedule.Date.Date);
            if (periodid != 0)
            {
                var period = _db.GLPeriods.Find(periodid);
                if (period != null)
                {
                    int yearid = period.GlFiscalYearId;
                    List<GLLedgerPostingLines> balances = _reportHelper.GetAccountBalance(yearid, periodid);
                    if (bankaccountobj != null)
                    {
                        string nataccount = _accountHelper.GetNaturalAccountSegment(bankaccountobj.AccountCode);
                        string subaccount = _accountHelper.GetSubAccountSegment(bankaccountobj.AccountCode);
                        bookBeginingBalance = _reportHelper.GetAccountBegBalancesWithSubAccount(balances,nataccount,subaccount);
                        bookEndingBalance = _reportHelper.GetAccountEndBalancesWithSubAccount(balances, nataccount, subaccount);
                    }
                }
            }

            #endregion
            ViewBag.Receipts = sumOfDebits;
            ViewBag.Payment = sumOfCredits;
            ViewBag.bookBeginingBalance = bookBeginingBalance;
            ViewBag.bookEndingBalance = bookEndingBalance;

            if (unreconcileddata.Any())
            {
                tDeposits = unreconcileddata.Sum(x => x.Debit);
                tPayments = unreconcileddata.Sum(x => x.Credit);
            }

            ViewBag.Tdeposit = tDeposits;
            ViewBag.TOutstanding = tPayments;

            tUnreconciledDiff = reconciliationSchedule.BankEndingBalance - tPayments + tDeposits - bookBeginingBalance;

            ViewBag.TUnreconciledDifference = tUnreconciledDiff;

            return View(reconciliationSchedule);
        }

        // [Authorize(Roles = Bank + "," + Bank_Admin)]
        public ActionResult GetUnReconciledbook(int? id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
           // DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1);//old
            DateTime dt2 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).AddDays(1);

            var data = _db.ReconcilationBookAccounts.Where(x => x.Date < dt2)
                                                   .Where(x => x.Reconciled == false).ToList();
            if (amount != null)
            {
                data = data.Where(x => x.Debit == amount || x.Credit == amount).ToList();
            }


            decimal sumOfDebits = 0;
            decimal sumOfCredits = 0;
            sumOfDebits = data.Sum(x => x.Debit);
            sumOfCredits = data.Sum(x => x.Credit);

            ViewBag.Unreconcile_dbook = Convert.ToDecimal(sumOfDebits - sumOfCredits).ToString("#,##0.00");
            return PartialView(data.ToList().OrderByDescending(x => x.Date));
        }

        // [Authorize(Roles = Bank + "," + Bank_Admin)]
        public ActionResult GetUnReconciledbank(int? id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
           // DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1);//old
            DateTime dt2 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).AddDays(1);//new

            var data = _db.ReconcilationBankAccounts.Where(x => x.Date <= dt2)
                                                   .Where(x => x.Reconciled == false).ToList();

            if (amount != null)
            {
                data = data.Where(x => x.Debit == amount || x.Credit == amount).ToList();
            }

            decimal sumOfDebits = 0;
            decimal sumOfCredits = 0;
            sumOfDebits = data.Sum(x => x.Debit);
            sumOfCredits = data.Sum(x => x.Credit);

            ViewBag.Unreconcile_dbank = Convert.ToDecimal(sumOfDebits - sumOfCredits).ToString("#,##0.00");
            return PartialView(data.ToList().OrderByDescending(x => x.Date));
        }

        // [Authorize(Roles = Bank + "," + Bank_Admin)]
        [HttpPost]
        public ActionResult DeleteUnReconciledbank(int? id)
        {
            try
            {
                ReconciliationBankAccount bank = _db.ReconcilationBankAccounts.Find(id);
                _db.ReconcilationBankAccounts.Remove(bank);
                _db.SaveChanges();
                return Content(Boolean.TrueString.ToUpper());
            }
            catch
            {
                return Content(Boolean.FalseString.ToUpper());
            }
        }

        // [Authorize(Roles = Bank + "," + Bank_Admin)]
        [HttpPost]
        public JsonResult QuickSaveUnreconciledbank(ReconciliationBankAccount reconcilationBankAccount)
        {
            reconcilationBankAccount.DateCreated = DateTime.Now;
            reconcilationBankAccount.CreatedBy = User.Identity.Name;
            reconcilationBankAccount.Reconciled = false;
            reconcilationBankAccount.Check = false;
            if (reconcilationBankAccount.Debit.ToString() == "")
                reconcilationBankAccount.Debit = 0;
            if (reconcilationBankAccount.Credit.ToString() == "")
                reconcilationBankAccount.Credit = 0;
            if (ModelState.IsValid)
            {
                _db.ReconcilationBankAccounts.Add(reconcilationBankAccount);
                _db.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Please fill reference.";
            }

            return Json(reconcilationBankAccount, "json");
        }

        [AuthorizeRoles(FinanceConstants.BankReconciliationUser)]
        public ActionResult ReconcileBook(int? id)
        {
            try
            {
                ReconciliationBookAccount reconcilationBookAccount = _db.ReconcilationBookAccounts.Find(id);
                reconcilationBookAccount.Reconciled = true;
                _db.Entry(reconcilationBookAccount).State = EntityState.Modified;
                _db.SaveChanges();
                return Content(Boolean.TrueString.ToUpper());
            }
            catch (Exception)
            {
                return Content(Boolean.FalseString.ToUpper());
            }
        }

       // [AuthorizeRoles(FinanceConstants.BankReconciliationUser)]
        public JsonResult ReconcileBank(int id, int reconid)
        {
            var schedule = _db.ReconciliationSchedules.Find(reconid);
            var reconciliationBankAccount = _db.ReconcilationBankAccounts.Find(id);
            if (schedule != null && reconciliationBankAccount != null)
            {
                reconciliationBankAccount.Reconciled = true;
                reconciliationBankAccount.ReconciledDate = schedule.Date;
                _db.Entry(reconciliationBankAccount).State = EntityState.Modified;
                _db.SaveChanges();
            }

            if (schedule == null)
            {
                var formattedData = new
                {
                    Message = "Schedule not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (reconciliationBankAccount == null)
            {
                var formattedData = new
                {
                    Message = "Record not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "Status Updated.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DrillDownBookSource(string id)
        {
            var header = new BookDrillDownVm();
            
            var book = _db.ReconcilationBookAccounts.FirstOrDefault(x => x.SourceID == id);
            if (book != null)
            {
                int parentid = int.Parse(book.SourceID.Split('-')[0]);
                int lineid = int.Parse(book.SourceID.Split('-')[1]);

                switch (book.Source)
                {
                    case "GeneralLedger":

                        #region GL

                        var glrecord = _db.GLRecordJournals.Find(parentid);
                        var glLines = _db.GLRecordJournalDetails.Where(x=>x.GlRecordJournalId==parentid).ToList();
                        if (glrecord != null)
                        {
                             header = new BookDrillDownVm
                            {
                                Id = glrecord.GlRecordJournalId.ToString(),
                                Description = glrecord.Description,
                                FiscalYear = glrecord.FiscalYear,
                                PeriodName = glrecord.PeriodName,
                                CategoryName = glrecord.CategoryName,
                                SourcesName = glrecord.SourcesName,
                                Reference = glrecord.JournalName,
                                Date = glrecord.EffectiveDate,
                                BookDrillDownLineVms=new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in glLines)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.GlRecordJournalDetailId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc =line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit =line.Credit,
                                    AdditionalDescription =line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }

                            
                        }

                        #endregion
                        return View(header);

                    case "AR_Invoice":

                        #region AR_Invoice

                        var arInvoice = _db.ARInvoices.Find(parentid);
                        var arInvoiceDistributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == parentid).ToList();
                        if (arInvoice != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = arInvoice.ArInvoiceId.ToString(),
                                Description = arInvoice.CustomerName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = arInvoice.Reference,
                                Date = arInvoice.Date,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in arInvoiceDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ArInvoiceDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AR_Collection":

                        #region AR_Collection

                        var standardCollection = _db.ARStandardCollections.Find(parentid);
                        var arStandardCollectionDistributions = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == parentid).ToList();
                        if (standardCollection != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = standardCollection.ArStandardCollectionId.ToString(),
                                Description = standardCollection.CustomerName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = standardCollection.Reference,
                                Date = standardCollection.ReceiptDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in arStandardCollectionDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ArStandardCollectionDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AR_MiscCollection":

                        #region AR_MiscCollection

                        var arMiscelaneousCollection = _db.ARMiscelaneousCollections.Find(parentid);
                        var arMiscelaneousCollectionDistributions = _db.ARMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollectionID == parentid).ToList();
                        if (arMiscelaneousCollection != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = arMiscelaneousCollection.ARMiscelaneousCollectionID.ToString(),
                                Description = arMiscelaneousCollection.CustomerName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = arMiscelaneousCollection.ReceiptNumber,
                                Date = arMiscelaneousCollection.ReceiptDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in arMiscelaneousCollectionDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ARMiscelaneousCollectionDistributionID.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_Invoice":

                        #region AP_Invoice

                        var apPaybleInvoices = _db.APPaybleInvoices.Find(parentid);
                        var apPaybleInvoiceDistributions = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoiceDistributionId == parentid).ToList();
                        if (apPaybleInvoices != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apPaybleInvoices.ApPaybleInvoicesId.ToString(),
                                Description = apPaybleInvoices.SupplierName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apPaybleInvoices.InvoiceNumber,
                                Date = apPaybleInvoices.InvoiceDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apPaybleInvoiceDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApPaybleInvoiceDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_Payment":
                    case "AP_PrePayment":

                        #region AP_Payment

                        var apPayment = _db.APPayments.Find(parentid);
                        var apPaymentDistributions = _db.APPaymentDistributions.Where(x => x.ApPaymentId == parentid).ToList();
                        if (apPayment != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apPayment.ApPaymentId.ToString(),
                                Description = apPayment.SupplierName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apPayment.InvoiceNumber,
                                Date = apPayment.PaymentDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apPaymentDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApPaymentDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_MiscPayment":

                        #region AP_MiscPayment

                        var apMiscellaneousPayment = _db.APMiscellaneousPayments.Find(parentid);
                        var apMiscellaneousPaymentDistributions = _db.APMiscellaneousPaymentDistributions.Where(x => x.ApMiscellaneousPaymentId == parentid).ToList();
                        if (apMiscellaneousPayment != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apMiscellaneousPayment.ApMiscellaneousPaymentId.ToString(),
                                Description = apMiscellaneousPayment.SupplierName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apMiscellaneousPayment.PaymentReference,
                                Date = apMiscellaneousPayment.PaymentDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apMiscellaneousPaymentDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApMiscellaneousPaymentDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_TravelOrder":

                        #region AP_TravelOrder

                        var apTravelOrder = _db.ApTravelOrders.Find(parentid);
                        var apTravelOrderDistributions = _db.ApTravelOrderDistributions.Where(x => x.ApTravelOrderId == parentid).ToList();
                        if (apTravelOrder != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apTravelOrder.ApTravelOrderId.ToString(),
                                Description = apTravelOrder.PurposeOfTravel,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apTravelOrder.Reference,
                                Date = apTravelOrder.RequestDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apTravelOrderDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApTravelOrderDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_TravelExpenseClaim":

                        #region AP_TravelExpenseClaim

                        var travelExpenseClaim = _db.ApTravelExpenseClaims.Find(parentid);
                        var apTravelExpenseDistributions = _db.ApTravelExpenseDistributions.Where(x => x.ApTravelExpenseClaimId == parentid).ToList();
                        if (travelExpenseClaim != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = travelExpenseClaim.ApTravelExpenseClaimId.ToString(),
                                Description = travelExpenseClaim.PurposeOfTravel,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = travelExpenseClaim.Reference,
                                Date = travelExpenseClaim.ClaimDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apTravelExpenseDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApTravelExpenseDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_PettyCash":

                        #region AP_PettyCash

                        var apPettyCash = _db.ApPettyCash.Find(parentid);
                        var apPettyCashDistributions = _db.ApPettyCashDistributions.Where(x => x.ApPettyCashId == parentid).ToList();
                        if (apPettyCash != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apPettyCash.ApPettyCashId.ToString(),
                                Description = apPettyCash.PurposeOfPayment,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apPettyCash.Reference,
                                Date = apPettyCash.Date,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apPettyCashDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApPettyCashDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    case "AP_Direct_Payment":

                        #region AP_Direct_Payment

                        var apDirectPayment = _db.ApDirectPayments.Find(parentid);
                        var apDirectPaymentDistributions = _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == parentid).ToList();
                        if (apDirectPayment != null)
                        {
                            header = new BookDrillDownVm
                            {
                                Id = apDirectPayment.ApDirectPaymentId.ToString(),
                                Description = apDirectPayment.SupplierName,
                                //FiscalYear = arInvoice.,
                                //PeriodName = arInvoice.PeriodName,
                                //CategoryName = arInvoice.CategoryName,
                                //SourcesName = arInvoice.SourcesName,
                                Reference = apDirectPayment.PaymentReference,
                                Date = apDirectPayment.PaymentDate,
                                BookDrillDownLineVms = new List<BookDrillDownLineVm>()

                            };

                            foreach (var line in apDirectPaymentDistributions)
                            {
                                var newLine = new BookDrillDownLineVm
                                {
                                    LineId = line.ApDirectPaymentDistributionId.ToString(),
                                    AccountCode = line.AccountCode,
                                    AccountDesc = line.AccountDesc,
                                    Debit = line.Debit,
                                    Credit = line.Credit,
                                    AdditionalDescription = line.AdditionalDescription
                                };
                                header.BookDrillDownLineVms.Add(newLine);
                            }


                        }

                        #endregion
                        return View(header);

                    default:
                       break;
                       
                }
            }
           return null;
        }


        [AuthorizeRoles(FinanceConstants.BankReconciliationUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ReconciliationSchedules.Find(id);

            #region check segment length validation

            var distributions = _db.ReconciliationBankServiceChargeDistributions.Where(x => x.ReconciliationScheduleId == id).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
            {
                string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #region debit credit validation
                if (dist.Debit != 0 && dist.Credit != 0)
                {
                    message = $"{dist.AccountCode} Account has both debit and credit value.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit == 0 && dist.Credit == 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit > 0 && dist.Credit > 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit < 0 || dist.Credit < 0)
                {
                    message = $"{dist.AccountCode} Negative values are not allowed.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }


            #endregion
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;

            try
            {
                var distribution = _db.ReconciliationBankServiceChargeDistributions.Where(x =>
                    x.ReconciliationScheduleId == record.ReconciliationScheduleId).ToList();
                sumTotalDisDebit = distribution.Sum(x => x.Debit);
                sumTotalDisCredit = distribution.Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ReconciliationSchedules.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.BankReconciliationUser)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ReconciliationSchedules.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                   .Where(x => x.Source == JournalSources.BankServiceCharge.ToString())
                   .Where(x => x.SourceId == record.ReconciliationScheduleId.ToString())
                   .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ReconciliationBankServiceChargeDistributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ReconciliationBankServiceChargeDistributions.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = year.Name;
                string glperiod = year.GLPeriod.FirstOrDefault().Name;
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
                    .Where(x => x.DateTo >= record.Date.Date).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.SingleOrDefault();
                        glperiod = myPeriod.Name;
                        glyear = myPeriod.GlFiscalYear.Name.ToString();
                        if (myPeriod.Status == FiscalYearStatus.Close)
                        {
                            var formattedData = new
                            {
                                Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (!period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are no periods defined that contain the payment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else if (period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the payment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }

                #endregion
              
                if (record.ReconciliationBankServiceChargeDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "Bank Service Charge",
                        Description = !String.IsNullOrEmpty(description) ? description : "Bank Service Charge",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Bank Service Charge",
                        SourcesName = JournalSources.BankServiceCharge.ToString(),
                        JournalNumber = "BANKRECON." + record.ReconciliationScheduleId.ToString(),
                        SourceId = record.ReconciliationScheduleId.ToString(),
                        Source = JournalSources.BankServiceCharge.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ReconciliationBankServiceChargeDistributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion
                record.IsOnlineTransferred = true;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.BankReconciliationVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ReconciliationSchedules.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ServiceCharges.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ReconciliationSchedules.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "bank reconciliation", "Success", User.Identity.Name, $"void bank reconciliation {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.ServiceCharges.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please fill service charge amount and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
