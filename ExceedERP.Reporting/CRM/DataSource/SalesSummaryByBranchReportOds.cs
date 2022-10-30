using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesSummaryByBranchReportOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesSummaryByBranchReportViewModel> GetSalesData(DateTime start, DateTime end, string customerId, string salesPerson, string status)
        {
            List<SalesSummaryByBranchReportViewModel> list = new List<SalesSummaryByBranchReportViewModel>();

            var temp = _db.CashSalesInvoices.Where(s => s.IsOnlineApproved).ToList();
            var sales = new List<CashSalesInvoice>();
            var allProducts = _db.Products.ToList();

            foreach (var o in temp)
            {
                // manually sold invoices
                // they are not printed in POS
                // they use hard-copy receipt. Thus they dont have ReceiptPrintTime
                if (o.IsManual)
                {
                    if (o.Date.Date <= end.Date && o.Date.Date >= start.Date)
                    {
                        sales.Add(o);
                    }
                }
                else
                {
                    if (o.IsReceiptPrinted)
                    {
                        if (o.InvoicePrintTime != null && o.InvoicePrintTime?.Date <= end.Date && o.InvoicePrintTime?.Date >= start.Date)
                        {
                            sales.Add(o);
                        }
                    }
                    //else
                    //{
                    //    if (o.Date.Date <= end.Date && o.Date.Date >= start.Date)
                    //        sales.Add(o);
                    //}
                }
            }

            if (customerId != null)
            {
                int cId = int.Parse(customerId);
                sales = sales.Where(o => o.OrganizationCustomerId == cId).ToList();
            }

            if (!string.IsNullOrEmpty(salesPerson))
            {
                sales = sales.Where(s => s.InvoicePrintedBy == salesPerson).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                sales = sales.Where(s => s.LastObjectState == status).ToList();
            }


            var branches = _db.Branch.ToList();
            foreach (var branch in branches)
            {
                var branchSpecificSales = sales.Where(s => s.BranchId == branch.BranchId).ToList();
                var invIdsOfBranch = new HashSet<string>(branchSpecificSales.Select(i => i.CashSalesInvoiceID.ToString()));
                var allLineItemsOfBranch = _db.LineItems.Where(l => l.VoucherDefinitionID == "2" && invIdsOfBranch.Contains(l.Voucher)).ToList();

                var prodIdsOfBranchSales = new HashSet<int>(allLineItemsOfBranch.Select(l => l.ThingID));
                var products = _db.Products.Where(p => prodIdsOfBranchSales.Contains(p.ProductID)).ToList();
                foreach (Product product in products)
                {
                    var productSpecificLineItems = allLineItemsOfBranch.Where(l => l.ThingID == product.ProductID).ToList();
                    decimal totalQty = 0;
                    decimal totalAmount = 0;
                    if (productSpecificLineItems.Any())
                    {
                        totalQty = productSpecificLineItems.Sum(l => l.Quantity);
                        totalAmount = productSpecificLineItems.Sum(l => l.TotalAmount);
                    }
                    var record = new SalesSummaryByBranchReportViewModel
                    {
                        Branch = branch.Name,
                        Category = GetProductCategory(product.CategoryID),
                        ItemName = product.Name,
                        UoM = product.UOM,
                        Quantity = totalQty,
                        TotalAmount = totalAmount
                    };
                    list.Add(record);
                }
            }
            return list;
        }


        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }

    }

}
