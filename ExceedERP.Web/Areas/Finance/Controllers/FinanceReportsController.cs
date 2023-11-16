using ExceedERP.Core.Domain.Finance;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;
using OfficeOpenXml;
using System.Web;
using OfficeOpenXml.Table;
using System.Globalization;
using System;
using System.IO;
using System.Net.Mime;
using System.Collections.Generic;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.ComponentModel;
using System.Data.Entity;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Reporting.Finance.ViewModel;
using DocumentFormat.OpenXml.Drawing;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
   
    public class FinanceReportsController : Controller
    {
        // GET: Finance/FinanceReports
        private ExceedDbContext _db = new ExceedDbContext();

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

        [HttpGet]
        public ActionResult WebSample1()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Sheet 1");
                sheet.Cells["A1:C1"].Merge = true;
                sheet.Cells["A1"].Style.Font.Size = 18f;
                sheet.Cells["A1"].Style.Font.Bold = true;
                sheet.Cells["A1"].Value = "Simple example 1";
                var excelData = package.GetAsByteArray();
                var fileName = "MyWorkbook.xlsx";
                return File(excelData, ContentType, fileName);
            }


        }

        [HttpGet]
        public ActionResult GetGLBalanceXLS(string yearSelector, string bizUnitSelector)
        {
            string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //using (var package = new ExcelPackage())
            //{
            //    var sheet = package.Workbook.Worksheets.Add("Sheet 1");
            //    sheet.Cells["A1:C1"].Merge = true;
            //    sheet.Cells["A1"].Style.Font.Size = 18f;
            //    sheet.Cells["A1"].Style.Font.Bold = true;
            //    sheet.Cells["A1"].Value = "Simple example 1"+ bizUnitSelector;
            //    var excelData = package.GetAsByteArray();
            //    var fileName = "MyWorkbook.xlsx";
            //    return File(excelData, ContentType, fileName);
            //}

            _db.Database.CommandTimeout = 7200;//added
            List<GeneralLedgerVm> balances = new List<GeneralLedgerVm>();

            if (!String.IsNullOrEmpty(yearSelector))
            {
                FiscalYearHelper fiscalYearHelper=new FiscalYearHelper();
                AccountHelper _accountHelper = new AccountHelper();
                int yearId=int.Parse(yearSelector);
                int fromPeriodD = fiscalYearHelper.GetFiscalYearFirstPeriod(yearId);
                int toPeriodD = fiscalYearHelper.GetFiscalYearLastPeriod(yearId); ;

                var from = _db.GLPeriods
                   .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                   .FirstOrDefault(x => x.GLPeriodId == fromPeriodD);

                var to = _db.GLPeriods
                    .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                    .FirstOrDefault(x => x.GLPeriodId == toPeriodD);


                var periodList = _db.GLPeriods.Where(x => x.DateFrom >= from.DateFrom)
                                              .Where(x => x.DateTo <= to.DateTo)
                                                .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId, x.GlFiscalYear, x.Name })//added
                                                .ToList();


                var ledgers = _db.GLLedgerPostings
                    .Include(x => x.GLLedgerPostingLines)                    
                    .Select(x => new
                    {
                        x.AccountCode,
                        x.SubAccountSegment,
                        x.CostCenterSegment,
                        x.AccountType,
                        x.AccountSegment,
                        x.BranchSegment,
                        x.BusinessUnit,
                        x.Segment1,
                        x.Segment2,
                        x.Segment3,
                        x.Segment4,
                        x.Segment5,
                        x.Segment6,
                        x.Segment7,
                        x.Segment8,
                        x.GLLedgerPostingLines
                        // GLLedgerPostingLines = x.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22)

                    })
                    .AsNoTracking().AsQueryable();

                #region get covered periods for sorting data
                var sortedPeriods = periodList.OrderBy(x => x.DateTo).Select(x => new { x.GLPeriodId }).ToList();
                var lastPeriod = periodList.OrderByDescending(x => x.DateTo).Take(1).FirstOrDefault();

                #endregion

                if (!String.IsNullOrEmpty(bizUnitSelector))
                    ledgers = ledgers.Where(x => x.BusinessUnit == bizUnitSelector).AsQueryable();


                foreach (var ledgerPosting in ledgers)
                {


                    var ledgeritem = new GeneralLedgerVm
                    {
                        AccountCode = ledgerPosting.AccountCode,
                        AccountDescription = _accountHelper.GetAccountDesc(ledgerPosting.AccountCode),
                        GeneralLedgerVmLines = new List<GeneralLedgerVmLines>(),
                        GeneralLedgerVmLastLineses = new List<GeneralLedgerVmLastLines>()
                    };

                    #region current transaction - all

                    foreach (var period in sortedPeriods)
                    {
                        #region beginning

                        var begginningRecords = ledgerPosting.GLLedgerPostingLines
                            .Where(x => x.GLPeriodId == period.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Begining).ToList();
                        foreach (var balance in begginningRecords)
                        {
                            var lines = new GeneralLedgerVmLines
                            {
                                Id = balance.GLLedgerPostingLinesID.ToString(),
                                Date = balance.EffectiveDate,
                                Reference = balance.JournalReference,
                                Description = balance.Description,
                                Journal = balance.CategoryName,
                                TransDesc = balance.AdditionalDescription,
                                Debit = balance.Debit,
                                Credit = balance.Credit,
                                Balance = balance.Balance,
                                Type = (int)balance.Type,
                                Remark = balance.Remark
                            };

                            ledgeritem.GeneralLedgerVmLines.Add(lines);
                        }

                        #endregion
                        #region standard

                        var standardRecords = ledgerPosting.GLLedgerPostingLines
                            .Where(x => x.GLPeriodId == period.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Standard)
                            .ToList().OrderBy(x => x.EffectiveDate).ThenBy(x => x.GLLedgerPostingLinesID);
                        foreach (var balance in standardRecords)
                        {
                            var lines = new GeneralLedgerVmLines
                            {
                                Id = balance.GLLedgerPostingLinesID.ToString(),
                                Date = balance.EffectiveDate,
                                Reference = balance.JournalReference,
                                Description = balance.Description,
                                ReferenceDoc = balance.ReferenceDoc,
                                PaymentReference = balance.PaymentReference,
                                InvoiceReferenceDoc = balance.InvoiceReferenceDoc,
                                Journal = balance.CategoryName,
                                TransDesc = balance.AdditionalDescription,
                                Debit = balance.Debit,
                                Credit = balance.Credit,
                                Balance = balance.Balance,
                                Type = (int)balance.Type,
                                Remark = balance.Remark
                            };

                            ledgeritem.GeneralLedgerVmLines.Add(lines);
                        }

                        #region netchange movement

                        decimal netChange = 0;
                        //decimal netDebit = 0;
                        //decimal netCredit = 0;

                        var debitchange = standardRecords.Sum(x => x.Debit);
                        var creditchange = standardRecords.Sum(x => x.Credit);


                        if (ledgerPosting.AccountType == GlAccountTypes.Asset ||
                            ledgerPosting.AccountType == GlAccountTypes.Expense)
                        {
                            netChange = debitchange - creditchange;
                            //if (netChange > 0)
                            //    netDebit = netChange;
                            //else
                            //    netCredit = Math.Abs(netChange);
                        }
                        else
                        {
                            netChange = creditchange - debitchange;
                            //if (netChange > 0)
                            //    netCredit = netChange;
                            //else
                            //    netDebit = Math.Abs(netChange);
                        }

                        var netChangeLine = new GeneralLedgerVmLines
                        {
                            Journal = "Movement in Current Period",
                            Debit = debitchange,
                            Credit = creditchange,
                            Balance = netChange,
                            Type = 3
                        };

                        ledgeritem.GeneralLedgerVmLines.Add(netChangeLine);

                        #endregion

                        #endregion
                        #region ending

                        var endingRecords = ledgerPosting.GLLedgerPostingLines
                            .Where(x => x.GLPeriodId == period.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                        foreach (var balance in endingRecords)
                        {
                            var lines = new GeneralLedgerVmLines
                            {
                                Id = balance.GLLedgerPostingLinesID.ToString(),
                                Date = balance.EffectiveDate,
                                Reference = balance.JournalReference,
                                Description = balance.Description,
                                Journal = balance.CategoryName,
                                TransDesc = balance.AdditionalDescription,
                                Debit = balance.Debit,
                                Credit = balance.Credit,
                                Balance = balance.Balance,
                                Type = (int)balance.Type,
                                Remark = balance.Remark
                            };

                            ledgeritem.GeneralLedgerVmLines.Add(lines);
                        }

                        #endregion
                    }

                    if (lastPeriod != null)
                    {
                        #region ending of last period

                        var lastendingRecords = ledgerPosting.GLLedgerPostingLines
                            .Where(x => x.GLPeriodId == lastPeriod.GLPeriodId)
                            .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                        foreach (var balance in lastendingRecords)
                        {
                            var lines = new GeneralLedgerVmLastLines
                            {
                                Id = balance.GLLedgerPostingLinesID.ToString(),
                                Date = balance.EffectiveDate,
                                Reference = balance.JournalReference,
                                Description = balance.Description,
                                Journal = balance.CategoryName,
                                TransDesc = balance.AdditionalDescription,
                                Debit = balance.Debit,
                                Credit = balance.Credit,
                                Balance = balance.Balance,
                                Type = (int)balance.Type,
                                Remark = balance.Remark
                            };

                            ledgeritem.GeneralLedgerVmLastLineses.Add(lines);
                        }

                        #endregion
                    }

                    #endregion

                    ledgeritem.GrandDebit = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Debit);
                    ledgeritem.GrandCredit = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Credit);
                    ledgeritem.GrandBalance = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Balance);

                    balances.Add(ledgeritem);
                }
            }

            var sortedBalances= balances.OrderBy(x => x.AccountCode);


            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Tables");

                // headers
                sheet.Cells["A1"].Value = "Month";
                sheet.Cells["B1"].Value = "Sales";
                sheet.Cells["C1"].Value = "VAT";
                sheet.Cells["D1"].Value = "Total";

                // Fill the table range with some data...
                var rnd = new Random();
                for (var row = 2; row < 12; row++)
                {
                    sheet.Cells[row, 1].Value = new DateTimeFormatInfo().GetMonthName(row);
                    sheet.Cells[row, 2].Value = rnd.Next(10000, 100000);
                    sheet.Cells[row, 3].Formula = $"B{row} * 0.25";
                    sheet.Cells[row, 4].Formula = $"B{row} + C{row}";
                }
                sheet.Cells["B2:D13"].Style.Numberformat.Format = "€#,##0.00";

                // Table range including header row
                var range = sheet.Cells["A1:D11"];

                // create the table
                var table = sheet.Tables.Add(range, "myTable");
                // configure the table
                table.ShowHeader = true;
                table.ShowFirstColumn = true;
                table.TableStyle = TableStyles.Dark2;
                // add a totals row under the data
                table.ShowTotal = true;
                table.Columns[1].TotalsRowFunction = RowFunctions.Sum;
                table.Columns[2].TotalsRowFunction = RowFunctions.Sum;
                table.Columns[3].TotalsRowFunction = RowFunctions.Sum;

                // Calculate all the formulas including the totals row.
                // This will give input to the AutofitColumns call
                range.Calculate();
                range.AutoFitColumns();

                var excelData = package.GetAsByteArray();
                var fileName = "MyWorkbook.xlsx";
                return File(excelData, ContentType, fileName);
            }
        }
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

        #region ifrs
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult ScoiByBizUnit()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult Scoi()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult ScoiByProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult SociAllProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult SofpByBizUnit()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult Sofp()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult SofpbyBranchOrProject()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
        public ActionResult Socf()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.FinanceReport)]
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