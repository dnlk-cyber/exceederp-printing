using ExceedERP.Core.Domain.Finance;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

using System.Globalization;
using System;
using System.Collections.Generic;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.Data.Entity;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Web.Areas.Finance.Controllers
{

    public class FinanceReportsController : Controller
    {
        // GET: Finance/FinanceReports
        private ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult APTravelOrderPayList()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult AccountPayableReceivable()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult AuditTrail()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult RatioAnalysis()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult RatioSummary()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult ManualVoidDocs()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BalanceSheetReport()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BalanceSheetSingleQuarterReport()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BalanceSheetByBranchOrProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult IncomeStatementSingleQuarterReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult IncomeStatementReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult AccountScheduleReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult AccountScheduleSingleQuarterReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BankJournal()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BankReconciliationStatement(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult GeneralLedgerBalanceXlS()        
        {            
            List<SelectListItem> yearSelectList = _db.GLFiscalYears.OrderByDescending(x => x.DateFrom).Select(genre => new SelectListItem() { Value = genre.GlFiscalYearId.ToString(), Text = genre.Name }).ToList();           
            
            var segmentTypes=_db.GlSegmentSetups.Where(x=>x.SegmentType==DynamicSegmentTypes.BusinessUnit).FirstOrDefault();
            List<SelectListItem> bizSelectList = _db.GlChartOfAccountSegments.Where(x=>x.GLSegmentSetupId== segmentTypes.GLSegmentSetupId).OrderByDescending(x => x.Values).Select(genre => new SelectListItem() { Value = genre.Values, Text = genre.Description }).ToList();

            ViewBag.yearSelector = yearSelectList;
            ViewBag.bizUnitSelector = bizSelectList;
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport,FinanceConstants.AccountReceivableReport)]
        public ActionResult GeneralLedgerBalance()
        {
            return View();
        }

        private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

      
     [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult GeneralLedgerBalanceSummary()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableReport)]
        public ActionResult ApJournalEntries()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult ArJournalEntries()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult GlJournalStatus()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult ArTransferStatus()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableReport)]
        public ActionResult ApTransferStatus()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult InventoryTransferStatus()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableReport)]
        public ActionResult ApAgingByInvoice()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableReport)]
        public ActionResult ApAgingBySupplier()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult ArAgingByInvoice()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult ArAgingByCustomer()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult ChartOfAccount()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.AccountPayableReport)]
        public ActionResult SupplierList()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult CustomerList()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult TrialBalance()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult TrialBalanceBySubAccount()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport,FinanceConstants.AccountPayableReport)]
        public ActionResult AccountAnalysisBySegment()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport, FinanceConstants.AccountPayableReport)]
        public ActionResult AccountAnalysis()
        {
            return View();
        }

        #region IFRS REPORTS
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult ScoiByBizUnit()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult Scoi()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult ScoiByProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult SociAllProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult SofpByBizUnit()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult Sofp()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult SofpbyBranchOrProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult Socf()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, PlanningRoles.Planning_User)]
        public ActionResult Socie()
        {
            return View();
        }


        #endregion


        #region gaap
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult BalanceSheet()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult IncomeStatement()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult IncomeStatementByBranch()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult CashFlow()
        {
            return View();
        }

        #endregion

        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult CustomerLedger()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountPayableReport)]
        public ActionResult SupplierLedger()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountPayableReport)]
        public ActionResult EmployeeLedger()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountPayableReport)]
        public ActionResult PurchaserLedger()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport, FinanceConstants.AccountPayableReport)]
        public ActionResult TaxReport()
        {
            return View();
        }

        #region sales related
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult DereferedRevenue()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesInvoiceReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesStatusReport()
        {
            return View();
        }
        //not in menue
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesJournalReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult RevenueByBranchReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult ReceiptsReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult CommissionByPrtnerReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult CustomerTransactionReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SupplierTransactionReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult InvoiceRegisterReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]

        public ActionResult PartnerAndAgentSettlementReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesOrderReport()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport, FinanceConstants.AccountReceivableReport)]
        public ActionResult SalesOrderRegisterReport()
        {
            return View();
        }
        #endregion
    }
}