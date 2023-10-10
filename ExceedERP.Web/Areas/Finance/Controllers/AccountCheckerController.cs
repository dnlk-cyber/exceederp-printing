using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.ViewModels;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Repository;
using ExceedERP.Web.Areas.Inventory.Controllers;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    [AuthorizeRoles(FinanceConstants.FinanceCheker)]
    public class AccountCheckerController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        FinanceHelper _financeHelper = new FinanceHelper();
        AccountHelper _accountHelper = new AccountHelper();
        FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        // GET: Finance/Description
       

       

       
        public ActionResult Index()
        {
            ViewData["ErrorTypeList"] = Enum.GetValues(typeof(ErrorType)).Cast<ErrorType>().Select(v => v.ToString())
                .ToList()
                .Select(e => new
                {
                    Name = e,
                    Code = e
                });

           

            return View();
        }
        public ActionResult JId()
        {
            var lines = _db.GLLedgerPostingLines.Where(x => x.Type == LedgerPostTypes.Standard && x.JournalId == 0).ToList();

            foreach (var l in lines)
            {              
               int.TryParse(l.JournalNumber.Split('-')[0],out int jid);
                l.JournalId = jid;
                _db.GLLedgerPostingLines.Attach(l);
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }
            

            return View("Success");
        }

        private List<AccountChekerVm> GetCheckerData(string errorType)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<AccountChekerVm> chekerVms = new List<AccountChekerVm>();

            switch (errorType)
            {
                case "Unsplitted_Journal":
                #region Unsplitted Journal

                    var journalSplit = _db.GLRecordJournalDetails
                        .Where(x => x.AccountSegment == null || x.AccountSegment == "").ToList();
                    if (journalSplit.Any())
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Unsplitted_Journal,
                            Description = "Unsplitted Journal",
                            Details = journalSplit.Count.ToString(),
                            Id =""
                        };
                        chekerVms.Add(cheker);
                    }

                    #endregion
                break;
                case "Unsplitted_Ledger":

                #region Unsplitted Ledger

                var ledgerSplit = _db.GLLedgerPostings
                    .Where(x => x.AccountSegment == null || x.AccountSegment == "").ToList();
                if (ledgerSplit.Any())
                {
                    var cheker = new AccountChekerVm
                    {
                        Type = ErrorType.Unsplitted_Ledger,
                        Description = "Unsplitted Ledger",
                        Details = ledgerSplit.Count.ToString(),
                        Id = ""
                    };
                    chekerVms.Add(cheker);
                }

                #endregion

                break;
                case "Wrong_Period":

                #region wrong period

                var periodList = _db.GLLedgerPostingLines
                .Select(x => x.GLPeriodId).Distinct().ToList();

                foreach (var i in periodList)
                {
                    var period = _db.GLPeriods.Find(i);
                    var wrongPeriodData = _db.GLLedgerPostingLines
                        .Where(x => x.GLPeriodId == i)
                        .Where(x => x.EffectiveDate < period.DateFrom || x.EffectiveDate > period.DateTo)
                        .ToList();
                    foreach (var data in wrongPeriodData)
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Wrong_Period,
                            Description = $"Wrong period {_db.GLPeriods.Find(i)?.Name}",
                            Details =$"Account: {data.GLLedgerPosting.AccountCode},Line: {data.GLLedgerPostingLinesID}, Journal Name: {data.JournalName}, Reference: {data.ReferenceDoc}, Period: {_db.GLPeriods.Find(i)?.Name}, Effective Date: {data.EffectiveDate}",
                            Id= data.GLLedgerPostingLinesID.ToString()
                        };
                        chekerVms.Add(cheker);
                    }

                }

                #endregion

                 break;

                case "Wrong_Account":
                #region wrong account

                var list = _db.GLLedgerPostings
                .Where(x => x.AccountCode == "").ToList();
                if (list.Any())
                {
                    var cheker = new AccountChekerVm
                    {
                        Description = "Duplicate Begning",
                        Details = list.Count.ToString()
                    };
                    //chekerVms.Add(cheker);
                }

                #endregion
                break;

                case "Duplicate_Ledger":
                #region duplicate ledger account

                    var duplicates = _db.GLLedgerPostings
                    .GroupBy(x => x.AccountCode)
                    .Where(g => g.Count() > 1)
                    .Select(y => new {Element = y.Key, Counter = y.Count()})
                    .ToList();
                foreach (var duplicate in duplicates)
                {
                    var cheker = new AccountChekerVm
                    {
                        Type = ErrorType.Duplicate_Ledger,
                        Description = $"Duplicate Ledger account {duplicate.Element}",
                        Details = duplicate.Counter.ToString(),
                        Id = duplicate.Element
                    };
                    chekerVms.Add(cheker);
                }

                    #endregion
                break;

                case "Duplicate_Ledger_Line":
                    #region duplicate ledger line

                    var duplicateLines = _db.GLLedgerPostingLines
                        .Where(x=>x.Type==LedgerPostTypes.Standard)
                        .GroupBy(x => x.JournalNumber)
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })
                        .ToList();
                    foreach (var duplicate in duplicateLines)
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Duplicate_Ledger_Line,
                            Description = $"Duplicate Ledger line {duplicate.Element}",
                            Details = duplicate.Counter.ToString(),
                            Id=duplicate.Element
                        };
                        chekerVms.Add(cheker);
                    }

                    #endregion
                    break;
                case "Duplicate_Beginning_Ledger_Line":
                    #region duplicate ledger line

                    var duplicateBegLines = _db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Begining)
                        .GroupBy(x => new { x.JournalNumber, x.GLLedgerPostingID})
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })
                        .ToList();
                    foreach (var duplicate in duplicateBegLines)
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Duplicate_Beginning_Ledger_Line,
                            Description = $"Duplicate Ledger ID {duplicate.Element.GLLedgerPostingID} {duplicate.Element.JournalNumber}",
                            Details = duplicate.Counter.ToString(),
                            Id = duplicate.Element.GLLedgerPostingID.ToString()
                        };
                        chekerVms.Add(cheker);
                    }

                    #endregion
                    break;
                case "Duplicate_Ending_Ledger_Line":
                    #region duplicate ledger line

                    var duplicateEndLines = _db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .GroupBy(x => new { x.JournalNumber, x.GLLedgerPostingID })
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })
                        .ToList();
                    foreach (var duplicate in duplicateEndLines)
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Duplicate_Ending_Ledger_Line,
                            Description = $"Duplicate Ledger ID {duplicate.Element.GLLedgerPostingID} {duplicate.Element.JournalNumber}",
                            Details = duplicate.Counter.ToString(),
                            Id = duplicate.Element.GLLedgerPostingID.ToString()
                        };
                        chekerVms.Add(cheker);
                    }

                    #endregion
                    break;

               
                case "Unposted_Journal_Line":
                #region unposted journal line

                var unpostedlines = _db.GLRecordJournalDetails
                .Where(x => x.GLRecordJournal.isPosted).AsQueryable();

                foreach (var journalDetail in unpostedlines)
                {
                    string journalNumber = journalDetail.GlRecordJournalId + "-" +
                                            journalDetail.GlRecordJournalDetailId;
                    var exist = _db.GLLedgerPostingLines.FirstOrDefault(x => x.JournalNumber == journalNumber);
                    if (exist != null)
                        continue;

                    var cheker = new AccountChekerVm
                    {
                        Type = ErrorType.Unposted_Journal_Line,
                        Description =
                            $"Unposted journal line {journalDetail.GlRecordJournalDetailId} of Journal {journalDetail.GlRecordJournalId}",
                        Details = ""
                    };
                    chekerVms.Add(cheker);
                }

                #endregion
                break;
                    
                case "Having_both_debit_credit":
                    #region Having_both_debit_credit

                    var bothDebitAndCredit = _db.GLRecordJournalDetails
                        .Where(x => x.GLRecordJournal.IsOnlineApproved&&!x.GLRecordJournal.isVoid)
                        .Where(x => x.Debit!=0 && x.Credit!=0).AsQueryable();

                    foreach (var journalDetail in bothDebitAndCredit)
                    {
                        var cheker = new AccountChekerVm
                        {
                            Type = ErrorType.Unposted_Journal_Line,
                            Description =
                                $"Has both debit and credit on journla number {journalDetail.GlRecordJournalId} and line {journalDetail.GlRecordJournalDetailId} debit value {journalDetail.Debit} and credit value {journalDetail.Credit}",
                            Details = ""
                        };
                        chekerVms.Add(cheker);
                    }

                    #endregion
                    break;
                case "Journal_Unbalanced_debit_credit":
                    #region Having_both_debit_credit

                    //var bothDebitAndCredit = _db.GLRecordJournalDetails
                    //    .Where(x => x.GLRecordJournal.IsOnlineApproved && !x.GLRecordJournal.isVoid)
                    //    .Where(x => x.Debit != 0 && x.Credit != 0).AsQueryable();

                    //foreach (var journalDetail in bothDebitAndCredit)
                    //{
                    //    var cheker = new AccountChekerVm
                    //    {
                    //        Type = ErrorType.Journal_Unbalanced_debit_credit,
                    //        Description =
                    //            $"Has both debit and credit on journla number {journalDetail.GlRecordJournalId} and line {journalDetail.GlRecordJournalDetailId} debit value {journalDetail.Debit} and credit value {journalDetail.Credit}",
                    //        Details = ""
                    //    };
                    //    chekerVms.Add(cheker);
                    //}

                    #endregion
                    break;
            }

            return chekerVms;
        }

        public ActionResult Checker_Read([DataSourceRequest]DataSourceRequest request,string type)
        {

            IQueryable<AccountChekerVm> accountChekerVms = GetCheckerData(type).AsQueryable();
            DataSourceResult result = accountChekerVms.ToDataSourceResult(request, accountChekerVm => new
            {
                Id = accountChekerVm.Id,
                Type = accountChekerVm.Type,
                Description = accountChekerVm.Description,
                Details = accountChekerVm.Details
            });

            return Json(result);
        }

        public JsonResult FixeIssue(string type,string id)
        {
            ErrorType e = (ErrorType)(int.Parse(type));
           

            switch (e.ToString())
            {
                case "Unsplitted_Journal":
                    #region Unsplitted Journal

                    _accountHelper.SplitJournalAccountCombination();

                    #endregion
                    break;
                case "Unsplitted_Ledger":

                    #region Unsplitted Ledger
                    _accountHelper.SplitLedgerAccountCombination();

                    #endregion

                    break;
                case "Wrong_Period":

                    #region wrong period

                   

                    #endregion

                    break;

                case "Wrong_Account":
                    #region wrong account

                   

                    #endregion
                    break;

                case "Duplicate_Ledger":
                    #region fix duplicate ledger 

                    var duplicateLedgers = _db.GLLedgerPostings.Where(x => x.AccountCode == id).ToList();
                    var minlineAccount = duplicateLedgers.Min(x => x.GLLedgerPostingID);
                    var toberemovedAccounts = duplicateLedgers.Where(x => x.GLLedgerPostingID != minlineAccount).ToList();
                    #region transfer to be removed account
                    foreach (var remove in toberemovedAccounts)
                    {
                        var lines = _db.GLLedgerPostingLines.Where(x => x.GLLedgerPostingID == remove.GLLedgerPostingID).ToList();
                        foreach (var line in lines)
                        {
                            line.GLLedgerPostingID = minlineAccount;
                            _db.GLLedgerPostingLines.Attach(line);
                            _db.Entry(line).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                    }
                    #endregion
                    _db.GLLedgerPostings.RemoveRange(toberemovedAccounts);
                    _db.SaveChanges();
                    #endregion
                    break;

                case "Duplicate_Ledger_Line":
                    #region duplicate ledger line

                    var duplicateledgerlines = _db.GLLedgerPostingLines.Where(x => x.JournalNumber == id).ToList();
                    var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                    var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                    _db.GLLedgerPostingLines.RemoveRange(toberemoved);
                    _db.SaveChanges();
                    //update balance monthly end
                    var minelinObj = _db.GLLedgerPostingLines.Find(minline);
                    int firstPeriodId = _fiscalYearHelper.GetFiscalYearFirstPeriod(minelinObj.GlFiscalYearId);
                    if(firstPeriodId!=0)
                        _financeHelper.UpdateAllBegEndBalancesFastP(firstPeriodId, User.Identity.Name);
                    //update balance dayly end
                    var jobUpdateStandard = BackgroundJob.Enqueue(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(firstPeriodId, User.Identity.Name));
                    #endregion
                    break;

                case "Unposted_Journal_Line":
                    #region unposted journal line

                   

                    #endregion
                    break;

                case "Having_both_debit_credit":
                    #region Having_both_debit_credit

                    

                    #endregion
                    break;
                case "Journal_Unbalanced_debit_credit":
                    #region Having_both_debit_credit

                  

                    #endregion
                    break;
            }

            var formattedData = new
            {
                Message = "Operation completed.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);          

        }

        public JsonResult FixeIssueBatch(string type)
        {
            ErrorType e = (ErrorType)Enum.Parse(typeof(ErrorType),type);

            switch (e.ToString())
            {
                case "Unsplitted_Journal":
                    #region Unsplitted Journal

                   // _accountHelper.SplitJournalAccountCombination();

                    #endregion
                    break;
                case "Unsplitted_Ledger":

                    #region Unsplitted Ledger
                    //_accountHelper.SplitLedgerAccountCombination();

                    #endregion

                    break;
                case "Wrong_Period":

                    #region wrong period



                    #endregion

                    break;

                case "Wrong_Account":
                    #region wrong account



                    #endregion
                    break;

                case "Duplicate_Ledger":
                    #region fix duplicate ledger 

                   
                    #endregion
                    break;

                case "Duplicate_Ledger_Line":
                    #region fixe duplicate  ledger line
                    _db.Database.CommandTimeout = 7200;
                    var duplicateLines = _db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Standard)
                        .GroupBy(x => x.JournalNumber)
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })
                        .AsNoTracking()
                        .ToList();

                    foreach (var duplicate in duplicateLines)
                    {
                        var duplicateledgerlines = _db.GLLedgerPostingLines
                            .Where(x => x.JournalNumber == duplicate.Element).ToList();
                        if (duplicateledgerlines.Any())
                        {
                            var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                            var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                            _db.GLLedgerPostingLines.RemoveRange(toberemoved);
                            _db.SaveChanges();
                        }

                    }

                    #endregion
                    break;

                case "Duplicate_Beginning_Ledger_Line":
                    #region fixe duplicate beginning ledger line
                    _db.Database.CommandTimeout = 7200;
                    var duplicateBegLines = _db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Begining)
                        .GroupBy(x => new { x.JournalNumber, x.GLLedgerPostingID })
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })   
                        .AsNoTracking()
                        .ToList();

                    foreach (var duplicate in duplicateBegLines)
                    {
                        var duplicateledgerlines = _db.GLLedgerPostingLines
                            .Where(x => x.JournalNumber == duplicate.Element.JournalNumber)
                            .Where(x => x.GLLedgerPostingID == duplicate.Element.GLLedgerPostingID).ToList();
                        if (duplicateledgerlines.Any())
                        {
                            var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                            var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                            _db.GLLedgerPostingLines.RemoveRange(toberemoved);
                            _db.SaveChanges();
                        }

                    }

                    #endregion
                    break;
                case "Duplicate_Ending_Ledger_Line":
                    #region fixe duplicate ending ledger line
                    _db.Database.CommandTimeout = 7200;
                    var duplicateEndLines = _db.GLLedgerPostingLines
                        .Where(x => x.Type == LedgerPostTypes.Ending)
                        .GroupBy(x => new { x.JournalNumber, x.GLLedgerPostingID })
                        .Where(g => g.Count() > 1)
                        .Select(y => new { Element = y.Key, Counter = y.Count() })
                        .AsNoTracking()
                        .ToList();

                    foreach (var duplicate in duplicateEndLines)
                    {
                        var duplicateledgerlines = _db.GLLedgerPostingLines
                            .Where(x => x.JournalNumber == duplicate.Element.JournalNumber)
                            .Where(x => x.GLLedgerPostingID == duplicate.Element.GLLedgerPostingID).ToList();
                        if (duplicateledgerlines.Any())
                        {
                            var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                            var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                            _db.GLLedgerPostingLines.RemoveRange(toberemoved);
                            _db.SaveChanges();
                        }

                    }

                    #endregion
                    break;

                case "Unposted_Journal_Line":
                    #region unposted journal line



                    #endregion
                    break;

                case "Having_both_debit_credit":
                    #region Having_both_debit_credit



                    #endregion
                    break;
                case "Journal_Unbalanced_debit_credit":
                    #region Having_both_debit_credit



                    #endregion
                    break;
            }

            var formattedData = new
            {
                Message = "Operation completed.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateBalance(string id)
        {
            int did = int.Parse(id);

            var year = _db.GLFiscalYears.Find(did);

            var firstPeriodOfYear = _db.GLPeriods.Where(x => x.GlFiscalYearId == did).FirstOrDefault(x => x.Index == 1);
                       
            if (firstPeriodOfYear != null)
            {
                _financeHelper.UpdateAllBegEndBalancesFastP(firstPeriodOfYear.GLPeriodId, User.Identity.Name);
                var formattedData = new
                {
                    Message = "Update completed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData1 = new
                {
                    Message = $"Beginning period not found for {year?.Name} please check if period index is set for all periods in the fiscal year",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult UpdateDailyBalance(string period)
        {

            if (!string.IsNullOrEmpty(period))
            {
                int did = int.Parse(period);
                _financeHelper.UpdateAllStandardBalancesFastP(did, User.Identity.Name);
                var formattedData = new
                {
                    Message = "Update completed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "Please select a period to update",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
    }
}