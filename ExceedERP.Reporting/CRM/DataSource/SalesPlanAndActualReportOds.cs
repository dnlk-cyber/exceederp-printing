using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesPlanAndActualReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesPlanAndActualReportViewModel> GetData(string yearId, string crmPeriodId, string branchId, string storeId)
        {
            List<SalesPlanAndActualReportViewModel> list = new List<SalesPlanAndActualReportViewModel>();

            if (string.IsNullOrEmpty(yearId) || string.IsNullOrEmpty(crmPeriodId))
            {
                return null;
            }
            else
            {
                var yId = Convert.ToInt32(yearId);
                var pId = Convert.ToInt32(crmPeriodId);

                var currentYear = db.GLFiscalYears.Find(yId);
                var currentCrmPeriod = db.CRMPeriods.Find(pId);

                HashSet<int> thisYearTodatePeriodIds = GetPreviousMonthIds(currentYear, currentCrmPeriod);
                thisYearTodatePeriodIds.Add(pId);

                //List<CashSalesInvoice> approvedInvoiceTodate = new List<CashSalesInvoice>();

                var invoiceTodate = 
                    db.CashSalesInvoices.Where(si => (si.IsManual && si.IsOnlineApproved == true) || !si.IsManual && si.IsReceiptPrinted)
                                        .Where(si => thisYearTodatePeriodIds.Contains(si.CRMPeriodId)).ToList();


                if (!string.IsNullOrEmpty(branchId))
                {
                    invoiceTodate = invoiceTodate.Where(i => i.BranchId.ToString() == branchId).ToList();
                }
                if (!string.IsNullOrEmpty(storeId))
                {
                    invoiceTodate = invoiceTodate.Where(i => i.StoreID.ToString() == storeId).ToList();
                }


                var invoiceIdsTodate = new HashSet<string>(invoiceTodate.Select(i => i.CashSalesInvoiceID.ToString()));
                var allInvoiceLineItemsTodate = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && invoiceIdsTodate.Contains(l.Voucher)).ToList();




                var invoicesOfCurrentMonth = invoiceTodate.Where(si => si.CRMPeriodId == pId).ToList();
                var invoiceIdsOfCurrentMonth = new HashSet<string>(invoicesOfCurrentMonth.Select(i => i.CashSalesInvoiceID.ToString()));
                var allInvoiceLineItemsOfCurrentMonth = allInvoiceLineItemsTodate.Where(l => invoiceIdsOfCurrentMonth.Contains(l.Voucher)).ToList();



                var categories = db.ProductCategories.Where(c => c.Parent == null).ToList();
                //var categoriesInSalesPlan = new HashSet<string>(db.AnnualSalesPlans.Where(sp => sp.GlFiscalYearId == yId).Select(p => p.SubCategoryId == null? p.SubCategoryId : p.CategoryId));
                foreach (var category in categories)
                {
                    var subCategories = db.ProductCategories.Where(c => c.Parent == category.ProductCategoryId.ToString()).ToList();
                    // has no sub categories
                    if (!subCategories.Any())
                    {
                        decimal planQty = 0;
                        decimal planValue = 0;

                        decimal actualQty = 0;
                        decimal actualValue = 0;

                        decimal planQtyTodate = 0;
                        decimal planValueTodate = 0;

                        decimal actualQtyTodate = 0;
                        decimal actualValueTodate = 0;

                        #region plan qty and value
                        //if(category)
                        var monthlyPlanTodate = db.StoreMonthlySalesPlans
                            .Where(p => thisYearTodatePeriodIds.Contains(p.CRMPeriodId) &&
                                    p.StoreAnnualSalesPlan.BranchAnnualSalesPlan.AnnualSalesPlan.CategoryId == category.ProductCategoryId.ToString())
                                    .ToList();
                        if (!string.IsNullOrEmpty(branchId))
                        {
                            monthlyPlanTodate = monthlyPlanTodate.Where(p => p.StoreAnnualSalesPlan.BranchAnnualSalesPlan.BranchId.ToString() == branchId).ToList();
                        }
                        if (!string.IsNullOrEmpty(storeId))
                        {
                            monthlyPlanTodate = monthlyPlanTodate.Where(p => p.StoreAnnualSalesPlan.SalesStoreId.ToString() == storeId).ToList();
                        }
                        if (monthlyPlanTodate.Any())
                        {
                            planQtyTodate = monthlyPlanTodate.Sum(p => p.Quantity);
                            planValueTodate = monthlyPlanTodate.Sum(p => p.Value);
                        }
                        var monthlyPlanOfCurrentMonth = monthlyPlanTodate.Where(p => p.CRMPeriodId == pId).ToList();
                        if (monthlyPlanOfCurrentMonth.Any())
                        {
                            planQty = monthlyPlanOfCurrentMonth.Sum(p => p.Quantity);
                            planValue = monthlyPlanOfCurrentMonth.Sum(p => p.Value);
                        }
                        #endregion

                        #region actual qty and value - current month and todate
                        var lineItemsOfCurrentCategoryCurrentMonth = allInvoiceLineItemsOfCurrentMonth.Where(l => l.Product.CategoryID == category.ProductCategoryId.ToString()).ToList();
                        if (lineItemsOfCurrentCategoryCurrentMonth.Any())
                        {
                            actualQty = lineItemsOfCurrentCategoryCurrentMonth.Sum(l => l.Quantity);
                            actualValue = lineItemsOfCurrentCategoryCurrentMonth.Sum(l => l.CalculatedCost);
                        }
                        var lineItemsOfCurrentCategoryTodate = allInvoiceLineItemsTodate.Where(l => l.Product.CategoryID == category.ProductCategoryId.ToString()).ToList();
                        if (lineItemsOfCurrentCategoryTodate.Any())
                        {
                            actualQtyTodate = lineItemsOfCurrentCategoryTodate.Sum(l => l.Quantity);
                            actualValueTodate = lineItemsOfCurrentCategoryTodate.Sum(l => l.CalculatedCost);
                        }
                        #endregion

                        var record = new SalesPlanAndActualReportViewModel
                        {
                            ItemType = category.Description,

                            CurrentMonthPlanQuantity = planQty,
                            CurrentMonthActualQuantity = actualQty,
                            PercentageOfCurrentMonthActualQuantity = planQty > 0 ? actualQty / planQty : 0,

                            CurrentMonthPlanValue = planValue,
                            CurrentMonthActualValue = actualValue,
                            PercentageOfCurrentMonthActualValue = planValue > 0 ? actualValue / planValue : 0,

                            YearTodatePlanQuantity = planQtyTodate,
                            YearTodateActualQuantity = actualQtyTodate,
                            PercentageOfYearTodateActualQuantity = planQtyTodate > 0 ? actualQtyTodate / planQtyTodate : 0,

                            YearTodatePlanValue = planValueTodate,
                            YearTodateActualValue = actualValueTodate,
                            PercentageOfYearTodateActualValue = planValueTodate > 0 ? actualValueTodate / planValueTodate : 0


                        };
                        list.Add(record);
                    }

                    else
                    {
                        foreach(ProductCategory subcategory in subCategories)
                        {
                            decimal planQty = 0;
                            decimal planValue = 0;

                            decimal actualQty = 0;
                            decimal actualValue = 0;

                            decimal planQtyTodate = 0;
                            decimal planValueTodate = 0;

                            decimal actualQtyTodate = 0;
                            decimal actualValueTodate = 0;

                            #region plan qty and value
                            //if(category)
                            var monthlyPlanTodate = db.StoreMonthlySalesPlans
                            .Where(p => thisYearTodatePeriodIds.Contains(p.CRMPeriodId) &&
                                        p.StoreAnnualSalesPlan.BranchAnnualSalesPlan.AnnualSalesPlan.SubCategoryId == subcategory.ProductCategoryId.ToString())
                                        .ToList();
                            if (!string.IsNullOrEmpty(branchId))
                            {
                                monthlyPlanTodate = monthlyPlanTodate.Where(p => p.StoreAnnualSalesPlan.BranchAnnualSalesPlan.BranchId.ToString() == branchId).ToList();
                            }
                            if (!string.IsNullOrEmpty(storeId))
                            {
                                monthlyPlanTodate = monthlyPlanTodate.Where(p => p.StoreAnnualSalesPlan.SalesStoreId.ToString() == storeId).ToList();
                            }
                            if (monthlyPlanTodate.Any())
                            {
                                planQtyTodate = monthlyPlanTodate.Sum(p => p.Quantity);
                                planValueTodate = monthlyPlanTodate.Sum(p => p.Value);
                            }
                            var monthlyPlanOfCurrentMonth = monthlyPlanTodate.Where(p => p.CRMPeriodId == pId).ToList();
                            if (monthlyPlanOfCurrentMonth.Any())
                            {
                                planQty = monthlyPlanOfCurrentMonth.Sum(p => p.Quantity);
                                planValue = monthlyPlanOfCurrentMonth.Sum(p => p.Value);
                            }
                            #endregion

                            #region actual qty and value
                            //var lineItemsOfCurrentCategory = allInvoiceLineItems.Where(l => l.Product.SubCategoryId == subcategory.ProductCategoryId.ToString()).ToList();
                            //if (lineItemsOfCurrentCategory.Any())
                            //{
                            //    actualQty = lineItemsOfCurrentCategory.Sum(l => l.Quantity);
                            //    actualValue = lineItemsOfCurrentCategory.Sum(l => l.TotalAmount);
                            //}
                            #endregion
                            #region actual qty and value - current month and todate
                            var lineItemsOfCurrentCategoryCurrentMonth = allInvoiceLineItemsOfCurrentMonth.Where(l => l.Product.SubCategoryId == subcategory.ProductCategoryId.ToString()).ToList();
                            if (lineItemsOfCurrentCategoryCurrentMonth.Any())
                            {
                                actualQty = lineItemsOfCurrentCategoryCurrentMonth.Sum(l => l.Quantity);
                                actualValue = lineItemsOfCurrentCategoryCurrentMonth.Sum(l => l.CalculatedCost);
                            }
                            var lineItemsOfCurrentCategoryTodate = allInvoiceLineItemsTodate.Where(l => l.Product.SubCategoryId == subcategory.ProductCategoryId.ToString()).ToList();
                            if (lineItemsOfCurrentCategoryTodate.Any())
                            {
                                actualQtyTodate = lineItemsOfCurrentCategoryTodate.Sum(l => l.Quantity);
                                actualValueTodate = lineItemsOfCurrentCategoryTodate.Sum(l => l.CalculatedCost);
                            }
                            #endregion

                            var record = new SalesPlanAndActualReportViewModel
                            {
                                ItemType = subcategory.Description,

                                CurrentMonthPlanQuantity = planQty,
                                CurrentMonthActualQuantity = actualQty,
                                PercentageOfCurrentMonthActualQuantity = planQty > 0 ? actualQty / planQty : 0,

                                CurrentMonthPlanValue = planValue,
                                CurrentMonthActualValue = actualValue,
                                PercentageOfCurrentMonthActualValue = planValue > 0 ? actualValue / planValue : 0,

                                YearTodatePlanQuantity = planQtyTodate,
                                YearTodateActualQuantity = actualQtyTodate,
                                PercentageOfYearTodateActualQuantity = planQtyTodate > 0 ? actualQtyTodate/planQtyTodate : 0,

                                YearTodatePlanValue = planValueTodate,
                                YearTodateActualValue = actualValueTodate,
                                PercentageOfYearTodateActualValue = planValueTodate > 0 ? actualValueTodate/planValueTodate:0

                            };
                            list.Add(record);
                        }
                    }
                }

            }

            return list;
        }
        private string GetCustomerCategory(string id)
        {
            return db.OrganizationCategories.FirstOrDefault(c => c.OrganizationCategoryID.ToString() == id)?.Description;
        }

        private HashSet<int> GetPreviousMonthIds(GlFiscalYear year, CRMPeriod period)
        {
            HashSet<int> ids = new HashSet<int>();

           if(year != null && period!= null)
            {
                var prevPeriods = db.CRMPeriods.Where(p => p.DateFrom >= year.DateFrom && p.DateTo <= year.DateTo && p.DateTo < period.DateFrom).ToList();
                if (prevPeriods.Any())
                {
                    ids = new HashSet<int>(prevPeriods.Select(p => p.CRMPeriodId));
                }
            }

            return ids;
        }
    }
}
