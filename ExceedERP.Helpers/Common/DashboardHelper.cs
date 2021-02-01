using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Helpers.Common
{
    public  class DashboardHelper
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        public  void UpdateDashboardItemBalance()
        {

            var itembalance = _db.InventoryItembalances
                .DistinctByField(x => x.ItemCode)
                .ToList();
                //.AsQueryable()
                //.AsNoTracking();

            var stores = _db.InventoryStores.AsNoTracking().ToList();

            foreach (var i in itembalance)
            {
                foreach (var inventoryStore in stores)
                {
                    var balance = _db.StoreItemAssignments
                        .Where(x => x.StoreCode == inventoryStore.StoreCode)
                        .Where(x => x.ItemCode == i.ItemCode).ToList();

                    var exisit = _db.InventoryItembalances
                        .Where(x => x.StoreCode == inventoryStore.StoreCode)
                        .AsNoTracking()
                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);

                    if (exisit != null)
                    {
                        i.Balance = balance.Sum(x => x.Balance);
                        i.LastUpdateAt = DateTime.Now;
                        _db.InventoryItembalances.Attach(i);
                        _db.Entry(i).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new InventoryItembalance
                        {
                            StoreCode = inventoryStore.StoreCode,
                            ItemCategoryCode = i.ItemCategoryCode,
                            ItemCode = i.ItemCode,
                            Balance = balance.Sum(x => x.Balance),
                            LastUpdateAt = DateTime.Now

                        };

                        _db.InventoryItembalances.Add(entity);
                        _db.SaveChanges();
                    }
                }
            }

        }

        public  void UpdateDashboardDictionary()
        {
           
            var dictionary = _db.DashboardDictionaries.ToList();
            DateTime dt2 = DateTime.Today;
            DateTime dt1 = dt2.AddDays(-30);
            #region procurement
            _db = new ExceedDbContext();
            #region tender
            var tender = dictionary.FirstOrDefault(x => x.Category == "PROCUREMENT" && x.Name == "Tender");
            decimal tendervalue = _db.Tenders.Count(x => x.IsOnlineApproved);
            if (tender != null)
            {
                tender.Value = tendervalue;
                _db.DashboardDictionaries.Attach(tender);
                _db.Entry(tender).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PROCUREMENT",
                    Name = "Tender",
                    Value = tendervalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region proforma
            var proforma = dictionary.FirstOrDefault(x => x.Category == "PROCUREMENT" && x.Name == "Proforma");
            decimal proformavalue = _db.ProformaPurchases.Count(x => x.IsOnlineApproved);
            if (proforma != null)
            {
                proforma.Value = proformavalue;
                _db.DashboardDictionaries.Attach(proforma);
                _db.Entry(proforma).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PROCUREMENT",
                    Name = "Proforma",
                    Value = proformavalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region purchase order

            var purchaseorder = dictionary.FirstOrDefault(x => x.Category == "PROCUREMENT" && x.Name == "PO");
            decimal poValue = _db.PurchaseOrders.Count(x => x.IsOnlineApproved);
            if (purchaseorder != null)
            {
                purchaseorder.Value = poValue;
                _db.DashboardDictionaries.Attach(purchaseorder);
                _db.Entry(purchaseorder).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PROCUREMENT",
                    Name = "PO",
                    Value = poValue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #endregion

            #region finance
            _db = new ExceedDbContext();
            //receivable
            var arinvoices = _db.ARInvoices.ToList();
            var collection = _db.ARStandardCollections.ToList();
            #region open invoice
            var open = dictionary.FirstOrDefault(x => x.Category == "AR_INVOICE_COLLECTION" && x.Name == "Open Invoice");
            decimal aropenvalue =
                arinvoices.Where(x => x.IsOnlineApproved && x.InvoiceStatus == InvoiceStatus.Open)
                    .Sum(x => x.AmountDue);
            if (open != null)
            {
                open.Value = aropenvalue;
                _db.DashboardDictionaries.Attach(open);
                _db.Entry(open).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_INVOICE_COLLECTION",
                    Name = "Open Invoice",
                    Value = aropenvalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region overdue invoice
            var aroverdue = dictionary.FirstOrDefault(x => x.Category == "AR_INVOICE_COLLECTION" && x.Name == "Overdue");
            decimal aroverduevalue =
                arinvoices.Where(x => x.IsOnlineApproved && x.InvoiceStatus == InvoiceStatus.Open && x.DueDate < DateTime.Today)
                    .Sum(x => x.AmountDue);
            if (aroverdue != null)
            {
                aroverdue.Value = aroverduevalue;
                _db.DashboardDictionaries.Attach(aroverdue);
                _db.Entry(aroverdue).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_INVOICE_COLLECTION",
                    Name = "Overdue",
                    Value = aroverduevalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region collected last 30 days

            var arcollected30Days = dictionary.FirstOrDefault(x => x.Category == "AR_INVOICE_COLLECTION" && x.Name == "Collected Last 30 Days");
            decimal arcollectedlast30Value =
                collection.Where(x => x.IsOnlineApproved && x.ReceiptDate <= dt2 && x.ReceiptDate >= dt1)
                    .Sum(x => x.TotalAmount);
            if (arcollected30Days != null)
            {
                arcollected30Days.Value = arcollectedlast30Value;
                _db.DashboardDictionaries.Attach(arcollected30Days);
                _db.Entry(arcollected30Days).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_INVOICE_COLLECTION",
                    Name = "Collected Last 30 Days",
                    Value = arcollectedlast30Value

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion

            //payable
            var apinvoices = _db.APPaybleInvoices.Where(x => x.IsOnlineApproved).ToList();
            var apyment = _db.APPayments.Where(x => x.IsOnlineApproved).ToList();
            #region open invoice
            var apopen = dictionary.FirstOrDefault(x => x.Category == "AP_INVOICE_PAYMENT" && x.Name == "Open Invoice");
            decimal apopenvalue =
                apinvoices.Where(x => x.InvoiceStatus == InvoiceStatus.Open).Sum(x => x.AmountDue);
            if (apopen != null)
            {
                apopen.Value = apopenvalue;
                _db.DashboardDictionaries.Attach(apopen);
                _db.Entry(apopen).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_INVOICE_PAYMENT",
                    Name = "Open Invoice",
                    Value = apopenvalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region overdue invoice
            var apoverdue = dictionary.FirstOrDefault(x => x.Category == "AP_INVOICE_PAYMENT" && x.Name == "Overdue");
            decimal apoverduevalue =
                apinvoices.Where(x => x.InvoiceStatus == InvoiceStatus.Open && x.InvoiceDate < DateTime.Today)
                    .Sum(x => x.AmountDue);
            if (apoverdue != null)
            {
                apoverdue.Value = apoverduevalue;
                _db.DashboardDictionaries.Attach(apoverdue);
                _db.Entry(apoverdue).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_INVOICE_PAYMENT",
                    Name = "Overdue",
                    Value = apoverduevalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region paid last 30 days

            dt2 = DateTime.Today;
            dt1 = dt2.AddDays(-30);
            var appaid30Days = dictionary.FirstOrDefault(x => x.Category == "AP_INVOICE_PAYMENT" && x.Name == "Paid Last 30 Days");
            decimal appaidlast30Value =
                apyment.Where(x => x.IsOnlineApproved && x.PaymentDate <= dt2 && x.PaymentDate >= dt1)
                    .Sum(x => x.NetAmount);
            if (appaid30Days != null)
            {
                appaid30Days.Value = appaidlast30Value;
                _db.DashboardDictionaries.Attach(appaid30Days);
                _db.Entry(appaid30Days).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_INVOICE_PAYMENT",
                    Name = "Paid Last 30 Days",
                    Value = appaidlast30Value

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion

            //suppliers

            var suppliers = _db.ProcurementSuppliers.Where(x => x.IsActive).ToList();
            #region credit suppliers
            var credit = dictionary.FirstOrDefault(x => x.Category == "AP_SUPPLIER" && x.Name == "Credit");
            decimal apcreditsuppliervalue = suppliers.Count(x => x.IsCredit);
            if (credit != null)
            {
                credit.Value = apcreditsuppliervalue;
                _db.DashboardDictionaries.Attach(credit);
                _db.Entry(credit).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_SUPPLIER",
                    Name = "Credit",
                    Value = apcreditsuppliervalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region other suppliers
            var othersupliers = dictionary.FirstOrDefault(x => x.Category == "AP_SUPPLIER" && x.Name == "Others");
            decimal apothersuppliervalue = suppliers.Count(x => x.IsCredit == false);
            if (othersupliers != null)
            {
                othersupliers.Value = apothersuppliervalue;
                _db.DashboardDictionaries.Attach(othersupliers);
                _db.Entry(othersupliers).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_SUPPLIER",
                    Name = "Others",
                    Value = apothersuppliervalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region total suppliers
            var totalsupliers = dictionary.FirstOrDefault(x => x.Category == "AP_SUPPLIER" && x.Name == "Total");
            decimal aptotalsuppliervalue = apcreditsuppliervalue + apothersuppliervalue;
            if (totalsupliers != null)
            {
                totalsupliers.Value = aptotalsuppliervalue;
                _db.DashboardDictionaries.Attach(totalsupliers);
                _db.Entry(totalsupliers).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_SUPPLIER",
                    Name = "Total",
                    Value = aptotalsuppliervalue

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region cash bank
            List<string> accountValues = new List<string>();

            var cashbanks = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CashAndBank).ToList();
            var periodList = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (periodList.Any())
            {
                var sortedperiodList = periodList.OrderBy(x => x.DateTo);
                foreach (var cashbank in cashbanks)
                {
                    accountValues = new List<string>();
                    AccountHelper accountHelper = new AccountHelper();
                    #region  account range

                    var accountRange = cashbank.FinanceDashboardRanges.ToList();

                    foreach (var range in accountRange)
                    {
                        var accountValuesRange = accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                            GlSegmentTypes.Account);
                        if (accountValuesRange != null)
                            accountValues.AddRange(accountValuesRange);
                    }

                    #endregion

                    foreach (var period in sortedperiodList)
                    {
                        ReportHelper reportHelper=new ReportHelper();
                        #region get values
                        var cashandbank = dictionary.FirstOrDefault(x => x.Category == "CASH_BANK" && x.Name == cashbank.Name && x.Dimesion == period.Name);
                        #region get balance for the month
                        decimal balanceTotal = 0;
                        #region get balance
                        List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                        foreach (var account in accountValues)
                        {
                            #region get balance

                            balanceTotal = balanceTotal + reportHelper.GetAccountEndBalances(currnetMonthBalance, account);

                            #endregion

                        }


                        #endregion

                        #endregion
                        if (cashandbank != null)
                        {
                            cashandbank.Value = balanceTotal;
                            _db.DashboardDictionaries.Attach(cashandbank);
                            _db.Entry(cashandbank).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            var entity = new DashboardDictionary
                            {
                                Category = "CASH_BANK",
                                Name = cashbank.Name,
                                Dimesion = period.Name,
                                Value = balanceTotal

                            };

                            _db.DashboardDictionaries.Add(entity);
                            _db.SaveChanges();
                        }
                        #endregion

                    }

                }

            }
            #endregion
            #region other account
            List<string> otheraccountValues = new List<string>();

            var otheraccountranges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.Other).ToList();
            var otherperiodList = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (otherperiodList.Any())
            {
                var sortedperiodList = otherperiodList.OrderBy(x => x.DateTo);
                foreach (var cashbank in otheraccountranges)
                {
                    ReportHelper reportHelper = new ReportHelper();
                    AccountHelper accountHelper = new AccountHelper();
                    otheraccountValues = new List<string>();
                    #region  account range

                    var accountRange = cashbank.FinanceDashboardRanges.ToList();

                    foreach (var range in accountRange)
                    {
                        var accountValuesRange = accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                            GlSegmentTypes.Account);
                        if (accountValuesRange != null)
                            otheraccountValues.AddRange(accountValuesRange);
                    }

                    #endregion

                    foreach (var period in sortedperiodList)
                    {
                       
                        #region get values
                        var cashandbank = dictionary.FirstOrDefault(x => x.Category == "OTHER_ACCOUNT" && x.Name == cashbank.Name && x.Dimesion == period.Name);
                        #region get balance for the month
                        decimal balanceTotal = 0;
                        #region get balance
                        List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                        foreach (var account in otheraccountValues)
                        {
                            #region get balance

                            balanceTotal = balanceTotal + reportHelper.GetAccountEndBalances(currnetMonthBalance, account);

                            #endregion

                        }


                        #endregion

                        #endregion
                        if (cashandbank != null)
                        {
                            cashandbank.Value = balanceTotal;
                            _db.DashboardDictionaries.Attach(cashandbank);
                            _db.Entry(cashandbank).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            var entity = new DashboardDictionary
                            {
                                Category = "OTHER_ACCOUNT",
                                Name = cashbank.Name,
                                Dimesion = period.Name,
                                Value = balanceTotal

                            };

                            _db.DashboardDictionaries.Add(entity);
                            _db.SaveChanges();
                        }
                        #endregion

                    }

                }

            }
            #endregion
            #region income and expense
            List<string> expenseAccountValues = new List<string>();
            List<string> revenueAccountValues = new List<string>();

         
            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {
                ReportHelper reportHelper = new ReportHelper();
                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);
               
                foreach (var period in sortedperiodList)
                {
                    decimal expenseBalanceTotal = 0;
                    decimal revenueBalanceTotal = 0;
                    decimal profitBalanceTotal = 0;
                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);

                    #region get expense
                    var expense = dictionary.FirstOrDefault(x => x.Category == "INCOME_EXPENSE" && x.Name == "Expense" && x.Dimesion == period.Name);
                    #region get balance for the month
                   
                    #region get balance
                  
                    expenseBalanceTotal = currnetMonthBalance
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense)
                        .Where(x => x.Type == LedgerPostTypes.Ending).Sum(x => x.Balance);

                    #endregion

                    #endregion
                    if (expense != null)
                    {
                        expense.Value = expenseBalanceTotal;
                        _db.DashboardDictionaries.Attach(expense);
                        _db.Entry(expense).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = "INCOME_EXPENSE",
                            Name = "Expense",
                            Dimesion = period.Name,
                            Value = expenseBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get income
                    var income = dictionary.FirstOrDefault(x => x.Category == "INCOME_EXPENSE" && x.Name == "Income" && x.Dimesion == period.Name);
                    #region get balance for the month

                    #region get balance

                    revenueBalanceTotal = currnetMonthBalance
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue)
                        .Where(x => x.Type == LedgerPostTypes.Ending).Sum(x => x.Balance);

                    #endregion

                    #endregion
                    if (income != null)
                    {
                        income.Value = revenueBalanceTotal;
                        _db.DashboardDictionaries.Attach(income);
                        _db.Entry(income).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = "INCOME_EXPENSE",
                            Name = "Income",
                            Dimesion = period.Name,
                            Value = revenueBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get profit
                    var profit = dictionary.FirstOrDefault(x => x.Category == "INCOME_EXPENSE" && x.Name == "Profit" && x.Dimesion == period.Name);
                    #region get balance for the month

                    #region get balance

                    profitBalanceTotal = revenueBalanceTotal - expenseBalanceTotal;

                    #endregion

                    #endregion
                    if (profit != null)
                    {
                        profit.Value = profitBalanceTotal;
                        _db.DashboardDictionaries.Attach(profit);
                        _db.Entry(profit).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = "INCOME_EXPENSE",
                            Name = "Profit",
                            Dimesion = period.Name,
                            Value = profitBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                }

            }
            #endregion
            #endregion
        }


        public  void UpdateFinanceDocStatusDict()
        {
            var dictionary = _db.DashboardDictionaries.ToList();
            DateTime dt2 = DateTime.Today;
            DateTime dt1 = dt2.AddDays(-30);
            _db = new ExceedDbContext();
            #region ap invoice document status

            var paybleInvoice = _db.APPaybleInvoices.AsQueryable();
            #region editing invoice
            var apinvoiceeditingstatus = dictionary.FirstOrDefault(x => x.Category == "AP_DOC_STATUS" && x.Name == "Editing");
            int apinvoiceeditigcount = paybleInvoice.Count(x => x.IsSendForApproval == false);
            if (apinvoiceeditingstatus != null)
            {
                apinvoiceeditingstatus.Value = apinvoiceeditigcount;
                _db.DashboardDictionaries.Attach(apinvoiceeditingstatus);
                _db.Entry(apinvoiceeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_DOC_STATUS",
                    Name = "Editing",
                    Value = apinvoiceeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting invoice
            var apinvoicewaitingstatus = dictionary.FirstOrDefault(x => x.Category == "AP_DOC_STATUS" && x.Name == "Waiting Approval");
            int apinvoicewaitingcount = paybleInvoice.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (apinvoicewaitingstatus != null)
            {
                apinvoicewaitingstatus.Value = apinvoicewaitingcount;
                _db.DashboardDictionaries.Attach(apinvoicewaitingstatus);
                _db.Entry(apinvoicewaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = apinvoicewaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer
            var apinvoicewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "AP_DOC_STATUS" && x.Name == "Waiting Transfer");
            int apinvoicewaitingtransfercount = paybleInvoice.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (apinvoicewaitingtransferstatus != null)
            {
                apinvoicewaitingtransferstatus.Value = apinvoicewaitingtransfercount;
                _db.DashboardDictionaries.Attach(apinvoicewaitingtransferstatus);
                _db.Entry(apinvoicewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = apinvoicewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region ap payment document status

            var payments = _db.APPayments.AsQueryable();
            var miscellaneousPayments = _db.APMiscellaneousPayments.AsQueryable();
            #region editing payment
            var appaymenteditingstatus = dictionary.FirstOrDefault(x => x.Category == "AP_PAYMENT_DOC_STATUS" && x.Name == "Editing");
            int appaymenteditigcount = payments.Count(x => x.IsSendForApproval == false) + miscellaneousPayments.Count(x => x.IsSendForApproval == false);
            if (appaymenteditingstatus != null)
            {
                appaymenteditingstatus.Value = appaymenteditigcount;
                _db.DashboardDictionaries.Attach(appaymenteditingstatus);
                _db.Entry(appaymenteditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_PAYMENT_DOC_STATUS",
                    Name = "Editing",
                    Value = appaymenteditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting payment
            var appaymentwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "AP_PAYMENT_DOC_STATUS" && x.Name == "Waiting Approval");
            int appaymentwaitingcount = payments.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false) + miscellaneousPayments.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (appaymentwaitingstatus != null)
            {
                appaymentwaitingstatus.Value = appaymentwaitingcount;
                _db.DashboardDictionaries.Attach(appaymentwaitingstatus);
                _db.Entry(appaymentwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_PAYMENT_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = appaymentwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer payment
            var appaymentwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "AP_PAYMENT_DOC_STATUS" && x.Name == "Waiting Transfer");
            int appaymentwaitingtransfercount = payments.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false) + miscellaneousPayments.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (appaymentwaitingtransferstatus != null)
            {
                appaymentwaitingtransferstatus.Value = appaymentwaitingtransfercount;
                _db.DashboardDictionaries.Attach(appaymentwaitingtransferstatus);
                _db.Entry(appaymentwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AP_PAYMENT_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = appaymentwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region ar invoice document status

            var arInvoices = _db.ARInvoices.AsQueryable();
            #region editing invoice
            var arinvoiceeditingstatus = dictionary.FirstOrDefault(x => x.Category == "AR_DOC_STATUS" && x.Name == "Editing");
            int arinvoiceeditigcount = arInvoices.Count(x => x.IsSendForApproval == false);
            if (arinvoiceeditingstatus != null)
            {
                arinvoiceeditingstatus.Value = arinvoiceeditigcount;
                _db.DashboardDictionaries.Attach(arinvoiceeditingstatus);
                _db.Entry(arinvoiceeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_DOC_STATUS",
                    Name = "Editing",
                    Value = arinvoiceeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting invoice
            var arinvoicewaitingstatus = dictionary.FirstOrDefault(x => x.Category == "AR_DOC_STATUS" && x.Name == "Waiting Approval");
            int arinvoicewaitingcount = arInvoices.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (arinvoicewaitingstatus != null)
            {
                arinvoicewaitingstatus.Value = arinvoicewaitingcount;
                _db.DashboardDictionaries.Attach(arinvoicewaitingstatus);
                _db.Entry(arinvoicewaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = arinvoicewaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer
            var arinvoicewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "AR_DOC_STATUS" && x.Name == "Waiting Transfer");
            int arinvoicewaitingtransfercount = arInvoices.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (arinvoicewaitingtransferstatus != null)
            {
                arinvoicewaitingtransferstatus.Value = arinvoicewaitingtransfercount;
                _db.DashboardDictionaries.Attach(arinvoicewaitingtransferstatus);
                _db.Entry(arinvoicewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = arinvoicewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region ar collection document status

            var collections = _db.ARStandardCollections.AsQueryable();
            var miscelaneousCollections = _db.ARMiscelaneousCollections.AsQueryable();
            #region editing collection
            var arcollectioneditingstatus = dictionary.FirstOrDefault(x => x.Category == "AR_COLLECTION_DOC_STATUS" && x.Name == "Editing");
            int arcollectioneditigcount = collections.Count(x => x.IsSendForApproval == false) + miscelaneousCollections.Count(x => x.IsSendForApproval == false);
            if (arcollectioneditingstatus != null)
            {
                arcollectioneditingstatus.Value = arcollectioneditigcount;
                _db.DashboardDictionaries.Attach(arcollectioneditingstatus);
                _db.Entry(arcollectioneditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_COLLECTION_DOC_STATUS",
                    Name = "Editing",
                    Value = arcollectioneditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting collection
            var arcollectionwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "AR_COLLECTION_DOC_STATUS" && x.Name == "Waiting Approval");
            int arcollectionwaitingcount = collections.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false) + miscelaneousCollections.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (arcollectionwaitingstatus != null)
            {
                arcollectionwaitingstatus.Value = arcollectionwaitingcount;
                _db.DashboardDictionaries.Attach(arcollectionwaitingstatus);
                _db.Entry(arcollectionwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_COLLECTION_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = arcollectionwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer collection
            var arcollectionwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "AR_COLLECTION_DOC_STATUS" && x.Name == "Waiting Transfer");
            int arcollectionwaitingtransfercount = collections.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false) + miscelaneousCollections.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (arcollectionwaitingtransferstatus != null)
            {
                arcollectionwaitingtransferstatus.Value = arcollectionwaitingtransfercount;
                _db.DashboardDictionaries.Attach(arcollectionwaitingtransferstatus);
                _db.Entry(arcollectionwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "AR_COLLECTION_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = arcollectionwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region gl document status

            var journals = _db.GLRecordJournals.AsQueryable();
            #region editing journal
            var gljournaleditingstatus = dictionary.FirstOrDefault(x => x.Category == "GL_DOC_STATUS" && x.Name == "Editing");
            int gljournaleditigcount = journals.Count(x => x.IsSendForApproval == false);
            if (gljournaleditingstatus != null)
            {
                gljournaleditingstatus.Value = gljournaleditigcount;
                _db.DashboardDictionaries.Attach(gljournaleditingstatus);
                _db.Entry(gljournaleditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GL_DOC_STATUS",
                    Name = "Editing",
                    Value = gljournaleditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting journal
            var gljournalwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "GL_DOC_STATUS" && x.Name == "Waiting Approval");
            int gljournalwaitingcount = journals.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (gljournalwaitingstatus != null)
            {
                gljournalwaitingstatus.Value = gljournalwaitingcount;
                _db.DashboardDictionaries.Attach(gljournalwaitingstatus);
                _db.Entry(gljournalwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GL_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = gljournalwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for post
            var gljournalwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "GL_DOC_STATUS" && x.Name == "Waiting Transfer");
            int gljournalwaitingtransfercount = journals.Count(x => x.IsOnlineApproved && x.isPosted == false && x.isVoid == false);
            if (gljournalwaitingtransferstatus != null)
            {
                gljournalwaitingtransferstatus.Value = gljournalwaitingtransfercount;
                _db.DashboardDictionaries.Attach(gljournalwaitingtransferstatus);
                _db.Entry(gljournalwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GL_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = gljournalwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            //stock waiting to transfer
            _db = new ExceedDbContext();
            #region issue

            var issues = _db.Issues.AsQueryable();
            #region waiting for transfer issue
            var issuewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_ISSUE_DOC_STATUS" && x.Name == "Waiting Transfer");
            int issuewaitingtransfercount = issues.Count(x =>
                x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (issuewaitingtransferstatus != null)
            {
                issuewaitingtransferstatus.Value = issuewaitingtransfercount;
                _db.DashboardDictionaries.Attach(issuewaitingtransferstatus);
                _db.Entry(issuewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_ISSUE_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = issuewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region transferred issue
            var transferredissuestatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_ISSUE_DOC_STATUS" && x.Name == "Transferred");
            int transferredissueissuecount = issues.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (transferredissuestatus != null)
            {
                transferredissuestatus.Value = transferredissueissuecount;
                _db.DashboardDictionaries.Attach(transferredissuestatus);
                _db.Entry(transferredissuestatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_ISSUE_DOC_STATUS",
                    Name = "Transferred",
                    Value = transferredissueissuecount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region transfer document status

            var itemTransfers = _db.ItemTransfers.AsQueryable();
            #region waiting for transfer
            var wiatingtransfertransferSend = dictionary.FirstOrDefault(x => x.Category == "FINANCE_TRANSFERSEND_DOC_STATUS" && x.Name == "Waiting Transfer");
            int transfersendwaitingtransfercount = itemTransfers.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (wiatingtransfertransferSend != null)
            {
                wiatingtransfertransferSend.Value = transfersendwaitingtransfercount;
                _db.DashboardDictionaries.Attach(wiatingtransfertransferSend);
                _db.Entry(wiatingtransfertransferSend).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_TRANSFERSEND_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = transfersendwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region  transferred
            var transferred = dictionary.FirstOrDefault(x => x.Category == "FINANCE_TRANSFERSEND_DOC_STATUS" && x.Name == "Transferred");
            int transferredtransfercount = itemTransfers.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (transferred != null)
            {
                transferred.Value = transferredtransfercount;
                _db.DashboardDictionaries.Attach(transferred);
                _db.Entry(transferred).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_TRANSFERSEND_DOC_STATUS",
                    Name = "Transferred",
                    Value = transferredtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion

            #endregion
            #region transfer receive status

            var transferReceives = _db.ItemTransferReceive.AsQueryable();
            #region waiting for transfer transfer receive
            var transferreceivewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_TRANSFERRECEIVE_DOC_STATUS" && x.Name == "Waiting Transfer");
            int transferreceivewaitingtransfercount = transferReceives.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false) + miscelaneousCollections.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (transferreceivewaitingtransferstatus != null)
            {
                transferreceivewaitingtransferstatus.Value = transferreceivewaitingtransfercount;
                _db.DashboardDictionaries.Attach(transferreceivewaitingtransferstatus);
                _db.Entry(transferreceivewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_TRANSFERRECEIVE_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = transferreceivewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region transferred transfer receive
            var transferreceivetransferredstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_TRANSFERRECEIVE_DOC_STATUS" && x.Name == "Transferred");
            int transferreceivetransferredcount = transferReceives.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (transferreceivetransferredstatus != null)
            {
                transferreceivetransferredstatus.Value = transferreceivetransferredcount;
                _db.DashboardDictionaries.Attach(transferreceivetransferredstatus);
                _db.Entry(transferreceivetransferredstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_TRANSFERRECEIVE_DOC_STATUS",
                    Name = "Transferred",
                    Value = transferreceivetransferredcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region good receive status

            var goodReceives = _db.GoodReceives.AsQueryable();
            #region waiting for transfer
            var grnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_GRN_DOC_STATUS" && x.Name == "Waiting Transfer");
            int grnwaitingtransfercount = goodReceives.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (grnwaitingtransferstatus != null)
            {
                grnwaitingtransferstatus.Value = grnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(grnwaitingtransferstatus);
                _db.Entry(grnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_GRN_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = grnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region transferred
            var grntransferredstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_GRN_DOC_STATUS" && x.Name == "Transferred");
            int grntransferredcount = goodReceives.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (grntransferredstatus != null)
            {
                grntransferredstatus.Value = grntransferredcount;
                _db.DashboardDictionaries.Attach(grntransferredstatus);
                _db.Entry(grntransferredstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_GRN_DOC_STATUS",
                    Name = "Transferred",
                    Value = grntransferredcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion

            #endregion
            #region store return status

            var storeReturns = _db.StoreReturns.AsQueryable();
            #region waiting for transfer
            var returnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_STORERETURN_DOC_STATUS" && x.Name == "Waiting Transfer");
            int returnwaitingtransfercount = storeReturns.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (returnwaitingtransferstatus != null)
            {
                returnwaitingtransferstatus.Value = returnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(returnwaitingtransferstatus);
                _db.Entry(returnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_STORERETURN_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = returnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region  transferred
            var returntransferredstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_STORERETURN_DOC_STATUS" && x.Name == "Transferred");
            int returntransferredcount = storeReturns.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (returntransferredstatus != null)
            {
                returntransferredstatus.Value = returntransferredcount;
                _db.DashboardDictionaries.Attach(returntransferredstatus);
                _db.Entry(returntransferredstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_STORERETURN_DOC_STATUS",
                    Name = "Transferred",
                    Value = returntransferredcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region stock adjustment status

            var stockAdjustments = _db.StockAdjustments.AsQueryable();
            #region waiting for transfer
            var adjustmentwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_STOCKADJUSTMENT_DOC_STATUS" && x.Name == "Waiting Transfer");
            int adjustmentwaitingtransfercount = stockAdjustments.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (adjustmentwaitingtransferstatus != null)
            {
                adjustmentwaitingtransferstatus.Value = adjustmentwaitingtransfercount;
                _db.DashboardDictionaries.Attach(adjustmentwaitingtransferstatus);
                _db.Entry(adjustmentwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_STOCKADJUSTMENT_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = adjustmentwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region transferred
            var adjustmentransferredstatus = dictionary.FirstOrDefault(x => x.Category == "FINANCE_STOCKADJUSTMENT_DOC_STATUS" && x.Name == "Transferred");
            int adjustmenttransferredcount = stockAdjustments.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred);
            if (adjustmentransferredstatus != null)
            {
                adjustmentransferredstatus.Value = adjustmenttransferredcount;
                _db.DashboardDictionaries.Attach(adjustmentransferredstatus);
                _db.Entry(adjustmentransferredstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "FINANCE_STOCKADJUSTMENT_DOC_STATUS",
                    Name = "Transferred",
                    Value = adjustmenttransferredcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion

            #endregion

        }

        public  void UpdateProcurementDocStatusDict()
        {
            var dictionary = _db.DashboardDictionaries.ToList();
            _db = new ExceedDbContext();

            #region pr document status

            var purchaseRequisitions = _db.PurchaseRequisitions.AsQueryable();
            #region waiting for second approval
            var prwaitingreviewstatus = dictionary.FirstOrDefault(x => x.Category == "PR_DOC_STATUS" && x.Name == "Waiting Review");
            int prwaitingreviewcount = purchaseRequisitions.Count(x => x.IsOnlineApproved && x.IsSecondSendForApproval == false && x.IsVoid == false);
            if (prwaitingreviewstatus != null)
            {
                prwaitingreviewstatus.Value = prwaitingreviewcount;
                _db.DashboardDictionaries.Attach(prwaitingreviewstatus);
                _db.Entry(prwaitingreviewstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PR_DOC_STATUS",
                    Name = "Waiting Review",
                    Value = prwaitingreviewcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting pR
            var prwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "PR_DOC_STATUS" && x.Name == "Waiting Approval");
            int prwaitingcount = purchaseRequisitions.Count(x => x.IsSecondSendForApproval && x.IsSecondOnlineApproved == false);
            if (prwaitingstatus != null)
            {
                prwaitingstatus.Value = prwaitingcount;
                _db.DashboardDictionaries.Attach(prwaitingstatus);
                _db.Entry(prwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PR_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = prwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion



            #endregion
            #region po

            var purchaseOrders = _db.PurchaseOrders.AsQueryable();
            #region editing po
            var poeditingstatus = dictionary.FirstOrDefault(x => x.Category == "PO_DOC_STATUS" && x.Name == "Editing");
            int poeditigcount = purchaseOrders.Count(x => x.IsSendForApproval == false);
            if (poeditingstatus != null)
            {
                poeditingstatus.Value = poeditigcount;
                _db.DashboardDictionaries.Attach(poeditingstatus);
                _db.Entry(poeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PO_DOC_STATUS",
                    Name = "Editing",
                    Value = poeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting po
            var powaitingstatus = dictionary.FirstOrDefault(x => x.Category == "PO_DOC_STATUS" && x.Name == "Waiting Approval");
            int powaitingcount = purchaseOrders.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (powaitingstatus != null)
            {
                powaitingstatus.Value = powaitingcount;
                _db.DashboardDictionaries.Attach(powaitingstatus);
                _db.Entry(powaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PO_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = powaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer po
            var powaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "PO_DOC_STATUS" && x.Name == "Waiting Transfer");
            int powaitingtransfercount = purchaseOrders.Count(x =>
                x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (powaitingtransferstatus != null)
            {
                powaitingtransferstatus.Value = powaitingtransfercount;
                _db.DashboardDictionaries.Attach(powaitingtransferstatus);
                _db.Entry(powaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PO_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = powaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region proforma document status

            var proformaPurchases = _db.ProformaPurchases.AsQueryable();
            #region editing proforma
            var proformaeditingstatus = dictionary.FirstOrDefault(x => x.Category == "PROFORMA_DOC_STATUS" && x.Name == "Editing");
            int proformaeditigcount = proformaPurchases.Count(x => x.IsSendForApproval == false);
            if (proformaeditingstatus != null)
            {
                proformaeditingstatus.Value = proformaeditigcount;
                _db.DashboardDictionaries.Attach(proformaeditingstatus);
                _db.Entry(proformaeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PROFORMA_DOC_STATUS",
                    Name = "Editing",
                    Value = proformaeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting proforma
            var proformawaitingstatus = dictionary.FirstOrDefault(x => x.Category == "PROFORMA_DOC_STATUS" && x.Name == "Waiting Approval");
            int proformawaitingcount = proformaPurchases.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (proformawaitingstatus != null)
            {
                proformawaitingstatus.Value = proformawaitingcount;
                _db.DashboardDictionaries.Attach(proformawaitingstatus);
                _db.Entry(proformawaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "PROFORMA_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = proformawaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region tender status

            var tenders = _db.Tenders.AsQueryable();
            #region editing tender
            var tendereditingstatus = dictionary.FirstOrDefault(x => x.Category == "TENDER_DOC_STATUS" && x.Name == "Editing");
            int tendereditigcount = tenders.Count(x => x.IsSendForApproval == false);
            if (tendereditingstatus != null)
            {
                tendereditingstatus.Value = tendereditigcount;
                _db.DashboardDictionaries.Attach(tendereditingstatus);
                _db.Entry(tendereditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TENDER_DOC_STATUS",
                    Name = "Editing",
                    Value = tendereditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting tender
            var tenderwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "TENDER_DOC_STATUS" && x.Name == "Waiting Approval");
            int tenderwaitingcount = tenders.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (tenderwaitingstatus != null)
            {
                tenderwaitingstatus.Value = tenderwaitingcount;
                _db.DashboardDictionaries.Attach(tenderwaitingstatus);
                _db.Entry(tenderwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TENDER_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = tenderwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion



            #endregion
            #region contract status

            var contractManagements = _db.ContractManagements.AsQueryable();
            #region editing contract
            var contracteditingstatus = dictionary.FirstOrDefault(x => x.Category == "CONTRACT_DOC_STATUS" && x.Name == "Editing");
            int contracteditigcount = contractManagements.Count(x => x.IsSendForApproval == false);
            if (contracteditingstatus != null)
            {
                contracteditingstatus.Value = contracteditigcount;
                _db.DashboardDictionaries.Attach(contracteditingstatus);
                _db.Entry(contracteditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "CONTRACT_DOC_STATUS",
                    Name = "Editing",
                    Value = contracteditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting contract
            var contractwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "CONTRACT_DOC_STATUS" && x.Name == "Waiting Approval");
            int contractwaitingcount = contractManagements.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (contractwaitingstatus != null)
            {
                contractwaitingstatus.Value = contractwaitingcount;
                _db.DashboardDictionaries.Attach(contractwaitingstatus);
                _db.Entry(contractwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "CONTRACT_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = contractwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region service grn status

            var serviceReceives = _db.ServiceReceives.AsQueryable();
            #region editing GRN
            var sgrneditingstatus = dictionary.FirstOrDefault(x => x.Category == "SERVICEGRN_DOC_STATUS" && x.Name == "Editing");
            int sgrneditigcount = serviceReceives.Count(x => x.IsSendForApproval == false);
            if (sgrneditingstatus != null)
            {
                sgrneditingstatus.Value = sgrneditigcount;
                _db.DashboardDictionaries.Attach(sgrneditingstatus);
                _db.Entry(sgrneditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SERVICEGRN_DOC_STATUS",
                    Name = "Editing",
                    Value = sgrneditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting s grn
            var sgrnwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "SERVICEGRN_DOC_STATUS" && x.Name == "Waiting Approval");
            int sgrnwaitingcount = serviceReceives.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (sgrnwaitingstatus != null)
            {
                sgrnwaitingstatus.Value = sgrnwaitingcount;
                _db.DashboardDictionaries.Attach(sgrnwaitingstatus);
                _db.Entry(sgrnwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SERVICEGRN_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = sgrnwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer
            var sgrnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "SERVICEGRN_DOC_STATUS" && x.Name == "Waiting Transfer");
            int sgrnwaitingtransfercount = serviceReceives.Count(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false);
            if (sgrnwaitingtransferstatus != null)
            {
                sgrnwaitingtransferstatus.Value = sgrnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(sgrnwaitingtransferstatus);
                _db.Entry(sgrnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SERVICEGRN_DOC_STATUS",
                    Name = "Waiting Transfer",
                    Value = sgrnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
        }

        public  void UpdateInventoryDocStatusDict()
        {
            var dictionary = _db.DashboardDictionaries.ToList();
            _db = new ExceedDbContext();

            #region SR document status

            var storeRequisitions = _db.StoreRequisitions.AsNoTracking().AsQueryable();
            #region editing SR
            var sreditingstatus = dictionary.FirstOrDefault(x => x.Category == "SR_DOC_STATUS" && x.Name == "Editing");
            int sreditigcount = storeRequisitions.Count(x => x.IsSendForApproval == false);
            if (sreditingstatus != null)
            {
                sreditingstatus.Value = sreditigcount;
                _db.DashboardDictionaries.Attach(sreditingstatus);
                _db.Entry(sreditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_DOC_STATUS",
                    Name = "Editing",
                    Value = sreditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting SR
            var srwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "SR_DOC_STATUS" && x.Name == "Waiting Approval");
            int srwaitingcount = storeRequisitions.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (srwaitingstatus != null)
            {
                srwaitingstatus.Value = srwaitingcount;
                _db.DashboardDictionaries.Attach(srwaitingstatus);
                _db.Entry(srwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = srwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for second approval
            var srwaitingreviewstatus = dictionary.FirstOrDefault(x => x.Category == "SR_DOC_STATUS" && x.Name == "Waiting Review");
            int srwaitingreviewcount = storeRequisitions.Count(x => x.IsOnlineApproved && x.IsSecondSendForApproval == false && x.IsVoid == false);
            if (srwaitingreviewstatus != null)
            {
                srwaitingreviewstatus.Value = srwaitingreviewcount;
                _db.DashboardDictionaries.Attach(srwaitingreviewstatus);
                _db.Entry(srwaitingreviewstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_DOC_STATUS",
                    Name = "Waiting Review",
                    Value = srwaitingreviewcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region issue

            var issues = _db.Issues.AsNoTracking().AsQueryable();
            #region editing issue
            var issueeditingstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_DOC_STATUS" && x.Name == "Editing");
            int appissueeditigcount = issues.Count(x => x.IsSendForApproval == false);
            if (issueeditingstatus != null)
            {
                issueeditingstatus.Value = appissueeditigcount;
                _db.DashboardDictionaries.Attach(issueeditingstatus);
                _db.Entry(issueeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_DOC_STATUS",
                    Name = "Editing",
                    Value = appissueeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting issue
            var issuewaitingstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_DOC_STATUS" && x.Name == "Waiting Approval");
            int issuewaitingcount = issues.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (issuewaitingstatus != null)
            {
                issuewaitingstatus.Value = issuewaitingcount;
                _db.DashboardDictionaries.Attach(issuewaitingstatus);
                _db.Entry(issuewaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = issuewaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var issuewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_DOC_STATUS" && x.Name == "Approved");
            int issuewaitingtransfercount = issues.Count(x =>
                x.IsOnlineApproved);
            if (issuewaitingtransferstatus != null)
            {
                issuewaitingtransferstatus.Value = issuewaitingtransfercount;
                _db.DashboardDictionaries.Attach(issuewaitingtransferstatus);
                _db.Entry(issuewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_DOC_STATUS",
                    Name = "Approved",
                    Value = issuewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region transfer document status

            var itemTransfers = _db.ItemTransfers.AsNoTracking().AsQueryable();
            #region editing send
            var transfersendeditingstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERSEND_DOC_STATUS" && x.Name == "Editing");
            int transfersendeditigcount = itemTransfers.Count(x => x.IsSendForApproval == false);
            if (transfersendeditingstatus != null)
            {
                transfersendeditingstatus.Value = transfersendeditigcount;
                _db.DashboardDictionaries.Attach(transfersendeditingstatus);
                _db.Entry(transfersendeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERSEND_DOC_STATUS",
                    Name = "Editing",
                    Value = transfersendeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting send
            var transfersendwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERSEND_DOC_STATUS" && x.Name == "Waiting Approval");
            int transfersendwaitingcount = itemTransfers.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (transfersendwaitingstatus != null)
            {
                transfersendwaitingstatus.Value = transfersendwaitingcount;
                _db.DashboardDictionaries.Attach(transfersendwaitingstatus);
                _db.Entry(transfersendwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERSEND_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = transfersendwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var transfersendwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERSEND_DOC_STATUS" && x.Name == "Approved");
            int transfersendwaitingtransfercount = itemTransfers.Count(x => x.IsOnlineApproved);
            if (transfersendwaitingtransferstatus != null)
            {
                transfersendwaitingtransferstatus.Value = transfersendwaitingtransfercount;
                _db.DashboardDictionaries.Attach(transfersendwaitingtransferstatus);
                _db.Entry(transfersendwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERSEND_DOC_STATUS",
                    Name = "Approved",
                    Value = transfersendwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region transfer receive status

            var transferReceives = _db.ItemTransferReceive.AsNoTracking().AsQueryable();
            #region editing transfer receive
            var transferreceiveeditingstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERRECEIVE_DOC_STATUS" && x.Name == "Editing");
            int transferreceiveeditigcount = transferReceives.Count(x => x.IsSendForApproval == false);
            if (transferreceiveeditingstatus != null)
            {
                transferreceiveeditingstatus.Value = transferreceiveeditigcount;
                _db.DashboardDictionaries.Attach(transferreceiveeditingstatus);
                _db.Entry(transferreceiveeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERRECEIVE_DOC_STATUS",
                    Name = "Editing",
                    Value = transferreceiveeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting transfer receive
            var transferreceivewaitingstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERRECEIVE_DOC_STATUS" && x.Name == "Waiting Approval");
            int transferreceivewaitingcount =
                transferReceives.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (transferreceivewaitingstatus != null)
            {
                transferreceivewaitingstatus.Value = transferreceivewaitingcount;
                _db.DashboardDictionaries.Attach(transferreceivewaitingstatus);
                _db.Entry(transferreceivewaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERRECEIVE_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = transferreceivewaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved transfer receive
            var transferreceivewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "TRANSFERRECEIVE_DOC_STATUS" && x.Name == "Approved");
            int transferreceivewaitingtransfercount = transferReceives.Count(x => x.IsOnlineApproved);
            if (transferreceivewaitingtransferstatus != null)
            {
                transferreceivewaitingtransferstatus.Value = transferreceivewaitingtransfercount;
                _db.DashboardDictionaries.Attach(transferreceivewaitingtransferstatus);
                _db.Entry(transferreceivewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "TRANSFERRECEIVE_DOC_STATUS",
                    Name = "Approved",
                    Value = transferreceivewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region good receive status

            var goodReceives = _db.GoodReceives.AsNoTracking().AsQueryable();
            #region editing receive
            var gRNeditingstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_DOC_STATUS" && x.Name == "Editing");
            int gRNeditigcount = goodReceives.Count(x => x.IsSendForApproval == false);
            if (gRNeditingstatus != null)
            {
                gRNeditingstatus.Value = gRNeditigcount;
                _db.DashboardDictionaries.Attach(gRNeditingstatus);
                _db.Entry(gRNeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_DOC_STATUS",
                    Name = "Editing",
                    Value = gRNeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting receive
            var grnwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_DOC_STATUS" && x.Name == "Waiting Approval");
            int grnwaitingcount = goodReceives.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (grnwaitingstatus != null)
            {
                grnwaitingstatus.Value = grnwaitingcount;
                _db.DashboardDictionaries.Attach(grnwaitingstatus);
                _db.Entry(grnwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = grnwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var grnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_DOC_STATUS" && x.Name == "Approved");
            int grnwaitingtransfercount = goodReceives.Count(x => x.IsOnlineApproved);
            if (grnwaitingtransferstatus != null)
            {
                grnwaitingtransferstatus.Value = grnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(grnwaitingtransferstatus);
                _db.Entry(grnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_DOC_STATUS",
                    Name = "Approved",
                    Value = grnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region store return status

            var storeReturns = _db.StoreReturns.AsNoTracking().AsQueryable();
            #region editing receive
            var returneditingstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_DOC_STATUS" && x.Name == "Editing");
            int returneditigcount = storeReturns.Count(x => x.IsSendForApproval == false);
            if (returneditingstatus != null)
            {
                returneditingstatus.Value = returneditigcount;
                _db.DashboardDictionaries.Attach(returneditingstatus);
                _db.Entry(returneditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_DOC_STATUS",
                    Name = "Editing",
                    Value = returneditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting receive
            var returnwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_DOC_STATUS" && x.Name == "Waiting Approval");
            int returnwaitingcount = storeReturns.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (returnwaitingstatus != null)
            {
                returnwaitingstatus.Value = returnwaitingcount;
                _db.DashboardDictionaries.Attach(returnwaitingstatus);
                _db.Entry(returnwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = returnwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var returnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_DOC_STATUS" && x.Name == "Approved");
            int returnwaitingtransfercount = storeReturns.Count(x => x.IsOnlineApproved);
            if (returnwaitingtransferstatus != null)
            {
                returnwaitingtransferstatus.Value = returnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(returnwaitingtransferstatus);
                _db.Entry(returnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_DOC_STATUS",
                    Name = "Approved",
                    Value = returnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region stock adjustment status

            var stockAdjustments = _db.StockAdjustments.AsNoTracking().AsQueryable();
            #region editing adjustment
            var stockeditingstatus = dictionary.FirstOrDefault(x => x.Category == "STOCKADJUSTMENT_DOC_STATUS" && x.Name == "Editing");
            int stockeditigcount = stockAdjustments.Count(x => x.IsSendForApproval == false);
            if (stockeditingstatus != null)
            {
                stockeditingstatus.Value = stockeditigcount;
                _db.DashboardDictionaries.Attach(stockeditingstatus);
                _db.Entry(stockeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STOCKADJUSTMENT_DOC_STATUS",
                    Name = "Editing",
                    Value = stockeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting adjustment
            var adjustmentwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "STOCKADJUSTMENT_DOC_STATUS" && x.Name == "Waiting Approval");
            int adjustmentwaitingcount = stockAdjustments.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (adjustmentwaitingstatus != null)
            {
                adjustmentwaitingstatus.Value = adjustmentwaitingcount;
                _db.DashboardDictionaries.Attach(adjustmentwaitingstatus);
                _db.Entry(adjustmentwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STOCKADJUSTMENT_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = adjustmentwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for transfer
            var adjustmentwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "STOCKADJUSTMENT_DOC_STATUS" && x.Name == "Approved");
            int adjustmentwaitingtransfercount = stockAdjustments.Count(x => x.IsOnlineApproved);
            if (adjustmentwaitingtransferstatus != null)
            {
                adjustmentwaitingtransferstatus.Value = adjustmentwaitingtransfercount;
                _db.DashboardDictionaries.Attach(adjustmentwaitingtransferstatus);
                _db.Entry(adjustmentwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STOCKADJUSTMENT_DOC_STATUS",
                    Name = "Approved",
                    Value = adjustmentwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
        }

        public  void UpdateFixedAssetDocStatusDict()
        {
            var dictionary = _db.DashboardDictionaries.ToList();
            _db = new ExceedDbContext();

            #region SR document status

            var fixedAssetRequisitions = _db.FixedAssetRequisitions.AsQueryable();
            #region editing SR
            var sreditingstatus = dictionary.FirstOrDefault(x => x.Category == "SR_FIXEDASSET_DOC_STATUS" && x.Name == "Editing");
            int sreditigcount = fixedAssetRequisitions.Count(x => x.IsSendForApproval == false);
            if (sreditingstatus != null)
            {
                sreditingstatus.Value = sreditigcount;
                _db.DashboardDictionaries.Attach(sreditingstatus);
                _db.Entry(sreditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_FIXEDASSET_DOC_STATUS",
                    Name = "Editing",
                    Value = sreditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting SR
            var srwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "SR_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Approval");
            int srwaitingcount = fixedAssetRequisitions.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (srwaitingstatus != null)
            {
                srwaitingstatus.Value = srwaitingcount;
                _db.DashboardDictionaries.Attach(srwaitingstatus);
                _db.Entry(srwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = srwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting for second approval
            var srwaitingreviewstatus = dictionary.FirstOrDefault(x => x.Category == "SR_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Review");
            int srwaitingreviewcount = fixedAssetRequisitions.Count(x => x.IsOnlineApproved && x.IsSecondSendForApproval == false && x.IsVoid == false);
            if (srwaitingreviewstatus != null)
            {
                srwaitingreviewstatus.Value = srwaitingreviewcount;
                _db.DashboardDictionaries.Attach(srwaitingreviewstatus);
                _db.Entry(srwaitingreviewstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "SR_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Review",
                    Value = srwaitingreviewcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region issue

            var issues = _db.FixedAssetIssues.AsQueryable();
            #region editing issue
            var issueeditingstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_FIXEDASSET_DOC_STATUS" && x.Name == "Editing");
            int appissueeditigcount = issues.Count(x => x.IsSendForApproval == false);
            if (issueeditingstatus != null)
            {
                issueeditingstatus.Value = appissueeditigcount;
                _db.DashboardDictionaries.Attach(issueeditingstatus);
                _db.Entry(issueeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_FIXEDASSET_DOC_STATUS",
                    Name = "Editing",
                    Value = appissueeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting issue
            var issuewaitingstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Approval");
            int issuewaitingcount = issues.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (issuewaitingstatus != null)
            {
                issuewaitingstatus.Value = issuewaitingcount;
                _db.DashboardDictionaries.Attach(issuewaitingstatus);
                _db.Entry(issuewaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = issuewaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var issuewaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "ISSUE_FIXEDASSET_DOC_STATUS" && x.Name == "Approved");
            int issuewaitingtransfercount = issues.Count(x =>
                x.IsOnlineApproved);
            if (issuewaitingtransferstatus != null)
            {
                issuewaitingtransferstatus.Value = issuewaitingtransfercount;
                _db.DashboardDictionaries.Attach(issuewaitingtransferstatus);
                _db.Entry(issuewaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "ISSUE_FIXEDASSET_DOC_STATUS",
                    Name = "Approved",
                    Value = issuewaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region transfer document status

            var machineryAndVehicleTransfers = _db.MachineryAndVehicleTransfers.AsQueryable();
            #region editing send
            var transfersendeditingstatus = dictionary.FirstOrDefault(x => x.Category == "MACHINETRANSFER_FIXEDASSET_DOC_STATUS" && x.Name == "Editing");
            int transfersendeditigcount = machineryAndVehicleTransfers.Count(x => x.IsSendForApproval == false);
            if (transfersendeditingstatus != null)
            {
                transfersendeditingstatus.Value = transfersendeditigcount;
                _db.DashboardDictionaries.Attach(transfersendeditingstatus);
                _db.Entry(transfersendeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "MACHINETRANSFER_FIXEDASSET_DOC_STATUS",
                    Name = "Editing",
                    Value = transfersendeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting send
            var transfersendwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "MACHINETRANSFER_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Approval");
            int transfersendwaitingcount = machineryAndVehicleTransfers.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (transfersendwaitingstatus != null)
            {
                transfersendwaitingstatus.Value = transfersendwaitingcount;
                _db.DashboardDictionaries.Attach(transfersendwaitingstatus);
                _db.Entry(transfersendwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "MACHINETRANSFER_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = transfersendwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var transfersendwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "MACHINETRANSFER_FIXEDASSET_DOC_STATUS" && x.Name == "Approved");
            int transfersendwaitingtransfercount = machineryAndVehicleTransfers.Count(x => x.IsOnlineApproved);
            if (transfersendwaitingtransferstatus != null)
            {
                transfersendwaitingtransferstatus.Value = transfersendwaitingtransfercount;
                _db.DashboardDictionaries.Attach(transfersendwaitingtransferstatus);
                _db.Entry(transfersendwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "MACHINETRANSFER_FIXEDASSET_DOC_STATUS",
                    Name = "Approved",
                    Value = transfersendwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region good receive status

            var goodReceives = _db.FixedAssetReceives.AsQueryable();
            #region editing receive
            var gRNeditingstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_FIXEDASSET_DOC_STATUS" && x.Name == "Editing");
            int gRNeditigcount = goodReceives.Count(x => x.IsSendForApproval == false);
            if (gRNeditingstatus != null)
            {
                gRNeditingstatus.Value = gRNeditigcount;
                _db.DashboardDictionaries.Attach(gRNeditingstatus);
                _db.Entry(gRNeditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_FIXEDASSET_DOC_STATUS",
                    Name = "Editing",
                    Value = gRNeditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting receive
            var grnwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Approval");
            int grnwaitingcount = goodReceives.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (grnwaitingstatus != null)
            {
                grnwaitingstatus.Value = grnwaitingcount;
                _db.DashboardDictionaries.Attach(grnwaitingstatus);
                _db.Entry(grnwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = grnwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var grnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "GRN_FIXEDASSET_DOC_STATUS" && x.Name == "Approved");
            int grnwaitingtransfercount = goodReceives.Count(x => x.IsOnlineApproved);
            if (grnwaitingtransferstatus != null)
            {
                grnwaitingtransferstatus.Value = grnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(grnwaitingtransferstatus);
                _db.Entry(grnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "GRN_FIXEDASSET_DOC_STATUS",
                    Name = "Approved",
                    Value = grnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
            #region store return status

            var storeReturns = _db.FixedAssetReturns.AsQueryable();
            #region editing receive
            var returneditingstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_FIXEDASSET_DOC_STATUS" && x.Name == "Editing");
            int returneditigcount = storeReturns.Count(x => x.IsSendForApproval == false);
            if (returneditingstatus != null)
            {
                returneditingstatus.Value = returneditigcount;
                _db.DashboardDictionaries.Attach(returneditingstatus);
                _db.Entry(returneditingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_FIXEDASSET_DOC_STATUS",
                    Name = "Editing",
                    Value = returneditigcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region waiting receive
            var returnwaitingstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_FIXEDASSET_DOC_STATUS" && x.Name == "Waiting Approval");
            int returnwaitingcount = storeReturns.Count(x => x.IsSendForApproval && x.IsOnlineApproved == false);
            if (returnwaitingstatus != null)
            {
                returnwaitingstatus.Value = returnwaitingcount;
                _db.DashboardDictionaries.Attach(returnwaitingstatus);
                _db.Entry(returnwaitingstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_FIXEDASSET_DOC_STATUS",
                    Name = "Waiting Approval",
                    Value = returnwaitingcount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion
            #region approved
            var returnwaitingtransferstatus = dictionary.FirstOrDefault(x => x.Category == "STORERETURN_FIXEDASSET_DOC_STATUS" && x.Name == "Approved");
            int returnwaitingtransfercount = storeReturns.Count(x => x.IsOnlineApproved);
            if (returnwaitingtransferstatus != null)
            {
                returnwaitingtransferstatus.Value = returnwaitingtransfercount;
                _db.DashboardDictionaries.Attach(returnwaitingtransferstatus);
                _db.Entry(returnwaitingtransferstatus).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                var entity = new DashboardDictionary
                {
                    Category = "STORERETURN_FIXEDASSET_DOC_STATUS",
                    Name = "Approved",
                    Value = returnwaitingtransfercount

                };

                _db.DashboardDictionaries.Add(entity);
                _db.SaveChanges();
            }
            #endregion


            #endregion
        }

        public  void UpdateDashboardFinanceRatio()
        {

            SeedActivityRatiosDictionary();
            SeedCapitalStructureRatiosDictionary();
            SeedIncomeExpenseDictionary();
            SeedLiquidityRatiosDictionary();
            SeedProfitabilityRatiosDictionary();

        }
        #region DashBoard Seed Data


        public  void SeedIncomeExpenseDictionary()
        {

            ExceedDbContext _db = new ExceedDbContext();

            #region income and expense
            string Category = "INCOME_EXPENSE";
            List<string> expenseAccountValues = new List<string>();
            List<string> revenueAccountValues = new List<string>();


            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {
                ReportHelper reportHelper = new ReportHelper();

                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);

                foreach (var period in sortedperiodList)
                {
                    decimal expenseBalanceTotal = 0;
                    decimal revenueBalanceTotal = 0;
                    decimal profitBalanceTotal = 0;
                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                    var dictionary = _db.DashboardDictionaries.ToList();
                    #region get expense
                    var expense = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Expense" && x.Dimesion == period.Name);
                    #region get balance for the month

                    #region get balance

                    expenseBalanceTotal = currnetMonthBalance
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Expense)
                        .Where(x => x.Type == LedgerPostTypes.Ending).Sum(x => x.Balance);

                    #endregion

                    #endregion
                    if (expense != null)
                    {
                        expense.Value = expenseBalanceTotal;
                        _db.DashboardDictionaries.Attach(expense);
                        _db.Entry(expense).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Expense",
                            Dimesion = period.Name,
                            Value = expenseBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get income
                    var income = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Income" && x.Dimesion == period.Name);
                    #region get balance for the month

                    #region get balance

                    revenueBalanceTotal = currnetMonthBalance
                        .Where(x => x.GLLedgerPosting.AccountType == GlAccountTypes.Revenue)
                        .Where(x => x.Type == LedgerPostTypes.Ending).Sum(x => x.Balance);

                    #endregion

                    #endregion
                    if (income != null)
                    {
                        income.Value = revenueBalanceTotal;
                        _db.DashboardDictionaries.Attach(income);
                        _db.Entry(income).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Income",
                            Dimesion = period.Name,
                            Value = revenueBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get profit
                    var profit = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Profit" && x.Dimesion == period.Name);
                    #region get balance for the month

                    #region get balance

                    profitBalanceTotal = revenueBalanceTotal - expenseBalanceTotal;

                    #endregion

                    #endregion
                    if (profit != null)
                    {
                        profit.Value = profitBalanceTotal;
                        _db.DashboardDictionaries.Attach(profit);
                        _db.Entry(profit).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Profit",
                            Dimesion = period.Name,
                            Value = profitBalanceTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                }

            }
            #endregion

        }



        public  void SeedActivityRatiosDictionary()
        {

            ExceedDbContext _db = new ExceedDbContext();

            #region Activity Ratios

            string Category = "ACTIVITY_RATIO";
            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {
                ReportHelper reportHelper = new ReportHelper();
                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);

                foreach (var period in sortedperiodList)
                {

                    decimal InventoryBalance = decimal.Zero;
                    decimal COSTOFGOODSOLD = decimal.Zero;
                    decimal RECEIVABLE_TURN_OVERBalance = decimal.Zero;
                    decimal TOTAL_ASSETSBalance = decimal.Zero;
                    decimal SALEBalance = decimal.Zero;
                    decimal SALESBalance = decimal.Zero;
                    decimal FIXED_ASSETSBalance = decimal.Zero;
                    decimal RECEIVABLESBalance = decimal.Zero;
                    decimal SALESONCREDITBalance = decimal.Zero;
                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                    var dictionary = _db.DashboardDictionaries.ToList();



                    #region get Inventory_TurnOver
                    var InventoryTurnOver = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Inventory_TurnOver" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.INVENTORY).FirstOrDefault();
                    var COSTOFGOODSOLDRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.COST_OF_GOOD_SOLD).FirstOrDefault();



                    #region get balance
                   
                    if (INVENTORYRanges != null)
                        InventoryBalance = reportHelper.GetAccountRangeBalance(INVENTORYRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (COSTOFGOODSOLDRanges != null)
                        COSTOFGOODSOLD = reportHelper.GetAccountRangeBalance(COSTOFGOODSOLDRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Inventory_TurnOverTotal = COSTOFGOODSOLD / InventoryBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (InventoryTurnOver != null)
                    {
                        InventoryTurnOver.Value = Inventory_TurnOverTotal;
                        _db.DashboardDictionaries.Attach(InventoryTurnOver);
                        _db.Entry(InventoryTurnOver).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Inventory_TurnOver",
                            Dimesion = period.Name,
                            Value = Inventory_TurnOverTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get Receivables_TurnOver
                    var ReceivablesTurnOver = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Receivables_TurnOver" && x.Dimesion == period.Name);
                    #region get balance for the month

                    var SALESONCREDITRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SALES_ON_CREDIT).FirstOrDefault();
                    var RECEIVABLESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.RECEIVABLES).FirstOrDefault();



                    #region get balance

                    if (SALESONCREDITRanges != null)
                        SALESONCREDITBalance = reportHelper.GetAccountRangeBalance(SALESONCREDITRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    if (RECEIVABLESRanges != null)
                        RECEIVABLESBalance = reportHelper.GetAccountRangeBalance(RECEIVABLESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Receivables_TurnOverTotal = SALESONCREDITBalance / RECEIVABLESBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (ReceivablesTurnOver != null)
                    {
                        ReceivablesTurnOver.Value = Receivables_TurnOverTotal;
                        _db.DashboardDictionaries.Attach(ReceivablesTurnOver);
                        _db.Entry(ReceivablesTurnOver).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Receivables_TurnOver",
                            Dimesion = period.Name,
                            Value = Receivables_TurnOverTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get Fixed_Asset_Turnover
                    var FixedAssetTurnover = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Fixed_Asset_Turnover" && x.Dimesion == period.Name);
                    #region get balance for the month



                    var SALESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SALES).FirstOrDefault();
                    var FIXED_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.FIXED_ASSETS).FirstOrDefault();



                    #region get balance

                    if (SALESRanges != null)
                        SALESBalance = reportHelper.GetAccountRangeBalance(SALESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (FIXED_ASSETSRanges != null)
                        FIXED_ASSETSBalance = reportHelper.GetAccountRangeBalance(FIXED_ASSETSRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Fixed_Asset_TurnoverTotal = SALESBalance / FIXED_ASSETSBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (FixedAssetTurnover != null)
                    {
                        FixedAssetTurnover.Value = Fixed_Asset_TurnoverTotal;
                        _db.DashboardDictionaries.Attach(FixedAssetTurnover);
                        _db.Entry(FixedAssetTurnover).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Fixed_Asset_Turnover",
                            Dimesion = period.Name,
                            Value = Fixed_Asset_TurnoverTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get Asset_TurnOver
                    var AssetTurnOvers = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Asset_TurnOver" && x.Dimesion == period.Name);

                    #region get balance for the month



                    var SALERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SALES).FirstOrDefault();
                    var TOTAL_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();



                    #region get balance
                    if (SALERanges != null)
                        SALEBalance = reportHelper.GetAccountRangeBalance(SALERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (TOTAL_ASSETSRanges != null)
                        TOTAL_ASSETSBalance = reportHelper.GetAccountRangeBalance(TOTAL_ASSETSRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Asset_TurnOverTotal = SALEBalance / TOTAL_ASSETSBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (AssetTurnOvers != null)
                    {
                        AssetTurnOvers.Value = Asset_TurnOverTotal;
                        _db.DashboardDictionaries.Attach(AssetTurnOvers);
                        _db.Entry(AssetTurnOvers).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Asset_TurnOver",
                            Dimesion = period.Name,
                            Value = Asset_TurnOverTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get Average Collection Period
                    var AverageCollectionPeriod = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Average_Collection_Period" && x.Dimesion == period.Name);
                    #region get balance for the month


                    decimal DaysInYear = 365;
                    var RECEIVABLE_TURN_OVERRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.RECEIVABLE_TURN_OVER).FirstOrDefault();

                    #region get balance
                    if (RECEIVABLE_TURN_OVERRanges != null)
                        RECEIVABLE_TURN_OVERBalance = reportHelper.GetAccountRangeBalance(RECEIVABLE_TURN_OVERRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);


                    decimal Average_Collection_PeriodTotal = DaysInYear / RECEIVABLE_TURN_OVERBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (AverageCollectionPeriod != null)
                    {
                        AverageCollectionPeriod.Value = Average_Collection_PeriodTotal;
                        _db.DashboardDictionaries.Attach(AverageCollectionPeriod);
                        _db.Entry(AverageCollectionPeriod).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Average_Collection_Period",
                            Dimesion = period.Name,
                            Value = Average_Collection_PeriodTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion


                }

            }
            #endregion

        }


        public  void SeedLiquidityRatiosDictionary()
        {

            ExceedDbContext _db = new ExceedDbContext();

            #region Liquidity Ratios

            string Category = "LIQUIDITY_RATIO";
            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {

                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);

                ReportHelper reportHelper = new ReportHelper();
                foreach (var period in sortedperiodList)
                {
                    decimal SALESRangesBalance = decimal.Zero;
                    decimal CASHBalance = decimal.Zero;
                    decimal SHORT_TERM_MARKETABLE_SECURITIESBalance = decimal.Zero;
                    decimal CURRENT_LIABLITIEBalance = decimal.Zero;
                    decimal INVENTORYBalance = decimal.Zero;
                    decimal CURRENTLIABLITIEBalance = decimal.Zero;
                    decimal CURRENTASSETBalance = decimal.Zero;
                    decimal CURRENT_LIABLITIESBalance = decimal.Zero;
                    decimal CURRENT_ASSETBalance = decimal.Zero;


                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                    var dictionary = _db.DashboardDictionaries.ToList();



                    #region get Current Ratio
                    var CurrentRatio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Current_Ratio" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var CURRENT_ASSETRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                    var CURRENT_LIABLITIESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();

                    

                    #region get balance
                    if (CURRENT_ASSETRanges != null)
                        CURRENT_ASSETBalance = reportHelper.GetAccountRangeBalance(CURRENT_ASSETRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (CURRENT_LIABLITIESRanges != null)
                        CURRENT_LIABLITIESBalance = reportHelper.GetAccountRangeBalance(CURRENT_LIABLITIESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal CurrentRatioTotal = CURRENT_ASSETBalance / CURRENT_LIABLITIESBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (CurrentRatio != null)
                    {
                        CurrentRatio.Value = CurrentRatioTotal;
                        _db.DashboardDictionaries.Attach(CurrentRatio);
                        _db.Entry(CurrentRatio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Current_Ratio",
                            Dimesion = period.Name,
                            Value = CurrentRatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region get Quick ratio
                    var QuickRatio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Quick_Ratio" && x.Dimesion == period.Name);
                    #region get balance for the month

                    var CURRENTASSETRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                    var CURRENTLIABLITIERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();
                    var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.INVENTORY).FirstOrDefault();



                    #region get balance
                    if (CURRENTASSETRanges != null)
                        CURRENTASSETBalance = reportHelper.GetAccountRangeBalance(CURRENTASSETRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (CURRENTLIABLITIERanges != null)
                        CURRENTLIABLITIEBalance = reportHelper.GetAccountRangeBalance(CURRENTLIABLITIERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (INVENTORYRanges != null)
                        INVENTORYBalance = reportHelper.GetAccountRangeBalance(INVENTORYRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Quick_RatioTotal = (CURRENTASSETBalance - INVENTORYBalance) / CURRENTLIABLITIEBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (QuickRatio != null)
                    {
                        QuickRatio.Value = Quick_RatioTotal;
                        _db.DashboardDictionaries.Attach(QuickRatio);
                        _db.Entry(QuickRatio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Quick_Ratio",
                            Dimesion = period.Name,
                            Value = Quick_RatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                    #region get Cash_Ratio
                    var CashRatio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Cash_Ratio" && x.Dimesion == period.Name);
                    #region get balance for the month



                    var CASHRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CASH).FirstOrDefault();
                    var SHORT_TERM_MARKETABLE_SECURITIESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SHORT_TERM_MARKETABLE_SECURITIES).FirstOrDefault();
                    var CURRENT_LIABLITIERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();



                    #region get balance
                    if (CURRENT_LIABLITIERanges != null)
                        CURRENT_LIABLITIEBalance = reportHelper.GetAccountRangeBalance(CURRENT_LIABLITIERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (SHORT_TERM_MARKETABLE_SECURITIESRanges != null)
                        SHORT_TERM_MARKETABLE_SECURITIESBalance = reportHelper.GetAccountRangeBalance(SHORT_TERM_MARKETABLE_SECURITIESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (CASHRanges != null)
                        CASHBalance = reportHelper.GetAccountRangeBalance(CASHRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Cash_RatioTotal = (CASHBalance + SHORT_TERM_MARKETABLE_SECURITIESBalance) / CURRENT_LIABLITIEBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (CashRatio != null)
                    {
                        CashRatio.Value = Cash_RatioTotal;
                        _db.DashboardDictionaries.Attach(CashRatio);
                        _db.Entry(CashRatio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Cash_Ratio",
                            Dimesion = period.Name,
                            Value = Cash_RatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion



                    #region get Net working capital  ratio
                    var Net_Working_Capital_Ratio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Net_Working_Capital_Ratio" && x.Dimesion == period.Name);

                    #region get balance for the month
                    var SALESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SALES).FirstOrDefault();
                    if (SALESRanges != null)
                        SALESRangesBalance = reportHelper.GetAccountRangeBalance(SALESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Net_Working_Capital_RatioTotal = (CURRENT_ASSETBalance - CURRENTLIABLITIEBalance) / SALESRangesBalance.DivideByZeroHelper();

                    #endregion


                    if (Net_Working_Capital_Ratio != null)
                    {
                        Net_Working_Capital_Ratio.Value = Net_Working_Capital_RatioTotal;
                        _db.DashboardDictionaries.Attach(Net_Working_Capital_Ratio);
                        _db.Entry(Net_Working_Capital_Ratio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Net_Working_Capital_Ratio",
                            Dimesion = period.Name,
                            Value = Net_Working_Capital_RatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion
                }

            }
            #endregion

        }

        public  void SeedCapitalStructureRatiosDictionary()
        {

            ExceedDbContext _db = new ExceedDbContext();

            #region Capital Structure Ratios

            string Category = "CAPITAL_STRUCTURE_RATIO";
            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {

                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);

                ReportHelper reportHelper = new ReportHelper();
                foreach (var period in sortedperiodList)
                {

                    decimal TOTAL_SHAREHOLDERS_EQUITYBalance = decimal.Zero;
                    decimal TOTAL_DEBTBalance = decimal.Zero;
                    decimal TOTAL_LIABLITIESBalance = decimal.Zero;
                    decimal TOTAL_ASSETBalance = decimal.Zero;

                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);
                    var dictionary = _db.DashboardDictionaries.ToList();



                    #region get Debt-to-assets ratio
                    var Debt_To_Assets_Ratio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Debt_To_Assets_Ratio" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var TOTAL_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();
                    var TOTAL_LIABLITIESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_LIABLITIES).FirstOrDefault();


                    
                    #region get balance

                    if (TOTAL_ASSETSRanges != null)
                        TOTAL_ASSETBalance = reportHelper.GetAccountRangeBalance(TOTAL_ASSETSRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (TOTAL_LIABLITIESRanges != null)
                        TOTAL_LIABLITIESBalance = reportHelper.GetAccountRangeBalance(TOTAL_LIABLITIESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Debt_To_Assets_RatioTotal = TOTAL_ASSETBalance / TOTAL_LIABLITIESBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (Debt_To_Assets_Ratio != null)
                    {
                        Debt_To_Assets_Ratio.Value = Debt_To_Assets_RatioTotal;
                        _db.DashboardDictionaries.Attach(Debt_To_Assets_Ratio);
                        _db.Entry(Debt_To_Assets_Ratio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Debt_To_Assets_Ratio",
                            Dimesion = period.Name,
                            Value = Debt_To_Assets_RatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region get Debt-to-equity ratio
                    var Debt_To_Equity_Ratio = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Debt_To_Equity_Ratio" && x.Dimesion == period.Name);
                    #region get balance for the month

                    var TOTAL_DEBTRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_DEBT).FirstOrDefault();
                    var TOTAL_SHAREHOLDERS_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();

                    

                    #region get balance
                    if (TOTAL_DEBTRanges != null)
                        TOTAL_DEBTBalance = reportHelper.GetAccountRangeBalance(TOTAL_DEBTRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (TOTAL_SHAREHOLDERS_EQUITYRanges != null)
                        TOTAL_SHAREHOLDERS_EQUITYBalance = reportHelper.GetAccountRangeBalance(TOTAL_SHAREHOLDERS_EQUITYRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Debt_To_Equity_RatioTotal = TOTAL_DEBTBalance / TOTAL_SHAREHOLDERS_EQUITYBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (Debt_To_Equity_Ratio != null)
                    {
                        Debt_To_Equity_Ratio.Value = Debt_To_Equity_RatioTotal;
                        _db.DashboardDictionaries.Attach(Debt_To_Equity_Ratio);
                        _db.Entry(Debt_To_Equity_Ratio).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Debt_To_Equity_Ratio",
                            Dimesion = period.Name,
                            Value = Debt_To_Equity_RatioTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion


                }

            }
            #endregion

        }


        public  void SeedProfitabilityRatiosDictionary()
        {

            ExceedDbContext _db = new ExceedDbContext();

            #region Profitability Ratios

            string Category = "PROFITABILITY_RATIO";
            var recentNPeriodsInYear = _fiscalYearHelper.GetRecentNPeriodsInYear(12);

            if (recentNPeriodsInYear.Any())
            {

                var sortedperiodList = recentNPeriodsInYear.OrderBy(x => x.DateTo);
                var dictionary = _db.DashboardDictionaries.ToList();


                foreach (var period in sortedperiodList)
                {

                    decimal NET_INCOMEBalance = decimal.Zero;
                    decimal OPERATING_INCOMEBalance = decimal.Zero;
                    decimal GROSS_INCOMEBalance = decimal.Zero;
                    decimal SALESBalance = decimal.Zero;
                    decimal TOTAL_ASSETSBalance = decimal.Zero;
                    decimal TOTAL_SHAREHOLDERS_EQUITYBalance = decimal.Zero;
                    decimal COST_OF_SERVICESBalance = decimal.Zero;

                    ReportHelper reportHelper = new ReportHelper();

                    List<GLLedgerPostingLines> currnetMonthBalance = reportHelper.GetAccountBalance(period.GlFiscalYearId, period.GLPeriodId);



                    #region Gross profit margin
                    var Gross_Profit_Margin = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Gross_Profit_Margin" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var GROSS_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.GROSS_INCOME).FirstOrDefault();
                    var SALESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.SALES).FirstOrDefault();



                    #region get balance

                    if (SALESRanges != null)
                        SALESBalance = reportHelper.GetAccountRangeBalance(SALESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);
                    if (GROSS_INCOMERanges != null)
                        GROSS_INCOMEBalance = reportHelper.GetAccountRangeBalance(GROSS_INCOMERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Gross_Profit_MarginTotal = GROSS_INCOMEBalance / SALESBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (Gross_Profit_Margin != null)
                    {
                        Gross_Profit_Margin.Value = Gross_Profit_MarginTotal;
                        _db.DashboardDictionaries.Attach(Gross_Profit_Margin);
                        _db.Entry(Gross_Profit_Margin).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Gross_Profit_Margin",
                            Dimesion = period.Name,
                            Value = Gross_Profit_MarginTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Operating profit margin
                    var Operating_Profit_Margin = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Operating_Profit_Margin" && x.Dimesion == period.Name);
                    #region get balance for the month

                    var OPERATING_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.OPERATING_INCOME).FirstOrDefault();


                    #region get balance
                    if (OPERATING_INCOMERanges != null)
                        OPERATING_INCOMEBalance = reportHelper.GetAccountRangeBalance(OPERATING_INCOMERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Operating_Profit_MarginTotal = OPERATING_INCOMEBalance / SALESBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (Operating_Profit_Margin != null)
                    {
                        Operating_Profit_Margin.Value = Operating_Profit_MarginTotal;
                        _db.DashboardDictionaries.Attach(Operating_Profit_Margin);
                        _db.Entry(Operating_Profit_Margin).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Operating_Profit_Margin",
                            Dimesion = period.Name,
                            Value = Operating_Profit_MarginTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Net profit margin
                    var Net_Profit_Margin = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Net_Profit_Margin" && x.Dimesion == period.Name);
                    #region get balance for the month

                    var NET_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.NET_INCOME).FirstOrDefault();


                    #region get balance
                    if (NET_INCOMERanges != null)
                        NET_INCOMEBalance = reportHelper.GetAccountRangeBalance(NET_INCOMERanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Net_Profit_MarginTotal = NET_INCOMEBalance / SALESBalance.DivideByZeroHelper();

                    #endregion

                    #endregion
                    if (Net_Profit_Margin != null)
                    {
                        Net_Profit_Margin.Value = Net_Profit_MarginTotal;
                        _db.DashboardDictionaries.Attach(Net_Profit_Margin);
                        _db.Entry(Net_Profit_Margin).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Net_Profit_Margin",
                            Dimesion = period.Name,
                            Value = Net_Profit_MarginTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Return on assets (ROA)
                    var Return_On_Assets = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Return_On_Assets" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var TOTAL_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();



                    #region get balance

                    if (TOTAL_ASSETSRanges != null)
                        TOTAL_ASSETSBalance = reportHelper.GetAccountRangeBalance(TOTAL_ASSETSRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Return_On_AssetsTotal = NET_INCOMEBalance / TOTAL_ASSETSBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (Return_On_Assets != null)
                    {
                        Return_On_Assets.Value = Return_On_AssetsTotal;
                        _db.DashboardDictionaries.Attach(Return_On_Assets);
                        _db.Entry(Return_On_Assets).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Return_On_Assets",
                            Dimesion = period.Name,
                            Value = Return_On_AssetsTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Return on equity (ROE)
                    var Return_On_Equity = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Return_On_Equity" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var TOTAL_SHAREHOLDERS_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();



                    #region get balance

                    if (TOTAL_SHAREHOLDERS_EQUITYRanges != null)
                        TOTAL_SHAREHOLDERS_EQUITYBalance = reportHelper.GetAccountRangeBalance(TOTAL_SHAREHOLDERS_EQUITYRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Return_On_EquityTotal = NET_INCOMEBalance / TOTAL_SHAREHOLDERS_EQUITYBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (Return_On_Equity != null)
                    {
                        Return_On_Equity.Value = Return_On_EquityTotal;
                        _db.DashboardDictionaries.Attach(Return_On_Equity);
                        _db.Entry(Return_On_Equity).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Return_On_Equity",
                            Dimesion = period.Name,
                            Value = Return_On_EquityTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Cost of Goods Sold
                    var Cost_Of_Goods_Sold = dictionary.FirstOrDefault(x => x.Category == Category && x.Name == "Cost_Of_Goods_Sold" && x.Dimesion == period.Name);
                    #region get balance for the month


                    var COST_OF_SERVICESRanges = _db.FinanceDashboards.Where(x => x.Category ==FinanceDashboradCategory.COST_OF_SERVICES).FirstOrDefault();



                    #region get balance

                    if (COST_OF_SERVICESRanges != null)
                        COST_OF_SERVICESBalance = reportHelper.GetAccountRangeBalance(COST_OF_SERVICESRanges.FinanceDashboardRanges.ToList(), currnetMonthBalance);

                    decimal Cost_Of_Goods_SoldTotal = COST_OF_SERVICESBalance / SALESBalance.DivideByZeroHelper();
                    #endregion

                    #endregion
                    if (Cost_Of_Goods_Sold != null)
                    {
                        Cost_Of_Goods_Sold.Value = Cost_Of_Goods_SoldTotal;
                        _db.DashboardDictionaries.Attach(Cost_Of_Goods_Sold);
                        _db.Entry(Cost_Of_Goods_Sold).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = Category,
                            Name = "Cost_Of_Goods_Sold",
                            Dimesion = period.Name,
                            Value = Cost_Of_Goods_SoldTotal

                        };

                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion



                }

            }
            #endregion

        }


        #endregion

        public  void AddNotofication(string ip, string roles, string user, DateTime deadline, string status, string description)
        {
            GlobalNotification log = new GlobalNotification
            {
                IpAddress = ip,
                ResponsibleRoles = roles,
                ResponsibleUserName = user,
                AlarmGeneratedTime = DateTime.Now,
                DeadLine = deadline,
                Status = status,
                Description = description
            };
            //db.GlobalNotifications.Add(log);
            //db.SaveChanges();
        }

       
    }
}
