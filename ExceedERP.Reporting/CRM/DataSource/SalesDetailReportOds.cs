using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class SalesDetailReportOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesDetailReportViewModel> GetSalesData(DateTime start, DateTime end, string branchId, string storeId,string categoryId, string customerId, string salesPerson, string status)
        {
            List<SalesDetailReportViewModel> list = new List<SalesDetailReportViewModel>();

            //var temp = _db.CashSalesInvoices.Where(s => s.IsOnlineApproved).ToList();
            //var sales = new List<CashSalesInvoice>();
            var sales = _db.CashSalesInvoices.Where(o =>
           (o.IsOnlineApproved && o.IsManual && DbFunctions.TruncateTime(o.Date) <= DbFunctions.TruncateTime(end) && DbFunctions.TruncateTime(o.Date) >= DbFunctions.TruncateTime(start)) ||
           (!o.IsManual && o.IsReceiptPrinted && o.InvoicePrintTime != null && DbFunctions.TruncateTime(o.InvoicePrintTime.Value) <= DbFunctions.TruncateTime(end) && DbFunctions.TruncateTime(o.InvoicePrintTime.Value) >= DbFunctions.TruncateTime(start))).ToList();

            //foreach (var o in temp)
            //{
            //    // manually sold invoices
            //    // they are not printed in POS
            //    // they use hard-copy receipt. Thus they dont have ReceiptPrintTime
            //    if (o.IsManual)
            //    {
            //        if (o.Date.Date <= end.Date && o.Date.Date >= start.Date)
            //        {
            //            sales.Add(o);
            //        }
            //    }
            //    else
            //    {
            //        if (o.IsReceiptPrinted)
            //        {
            //            if (o.InvoicePrintTime != null && o.InvoicePrintTime?.Date <= end.Date && o.InvoicePrintTime?.Date >= start.Date)
            //            {
            //                sales.Add(o);
            //            }
            //        }
            //        //else
            //        //{
            //        //    if (o.Date.Date <= end.Date && o.Date.Date >= start.Date)
            //        //        sales.Add(o);
            //        //}
            //    }
            //}
            if (branchId != null)
            {
                int bId = int.Parse(branchId);
                sales = sales.Where(o => o.BranchId == bId).ToList();
            }
            if (customerId != null)
            {
                int cId = int.Parse(customerId);
                sales = sales.Where(o => o.OrganizationCustomerId == cId).ToList();
            }
            if (storeId != null)
            {
                int sId = int.Parse(storeId);

                sales = sales.Where(s => s.StoreID == sId).ToList();
            }
            if (!string.IsNullOrEmpty(salesPerson))
            {
                sales = sales.Where(s => s.InvoicePrintedBy == salesPerson).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                sales = sales.Where(s => s.LastObjectState == status).ToList();
            }
            //var tempList = new List<CashSalesInvoice>();
            //tempList.AddRange(sales);
            //if (!string.IsNullOrEmpty(storeId))
            //{
            //    var lineItemsInStore =
            //        _db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.StoreID == sId).ToList();
            //    var vIds = new HashSet<string>(lineItemsInStore.Select(l => l.Voucher));
            //    foreach (CashSalesInvoice so in tempList)
            //    {
            //        var voucher = so.CashSalesInvoiceID.ToString();
            //        if (!vIds.Contains(voucher))
            //        {
            //            sales.Remove(so);
            //        }
            //    }
            //}
            foreach (CashSalesInvoice inv in sales)
            {
                string brachName = GetBranch(inv.BranchId);
                string storeName = GetStore(inv.StoreID);

                var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == inv.CashSalesInvoiceID.ToString()).ToList();
                if (!string.IsNullOrEmpty(categoryId))
                {
                    int.TryParse(categoryId, out int catId);
                    lineItems = lineItems.Where(l => l.Product.CategoryID == categoryId).ToList();
                }
                if (lineItems.Any())
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        var lineRecord = new SalesDetailReportViewModel
                        {
                            ItemCategory = GetProductCategory(lineItem.Product.CategoryID),
                            ItemCode = lineItem.Product.ItemCode,
                            ItemName = lineItem.Product.Name,
                            UoM = lineItem.Product.UOM,
                            Quantity = lineItem.Quantity,
                            UnitPrice = lineItem.UnitAmount,
                            SubTotal = lineItem.CalculatedCost,
                            Adds = 0,
                            Discount = lineItem.Discount,
                            TaxAmount = lineItem.TaxAmount,
                            TotalAmount = lineItem.TotalAmount,

                            CustomerName = inv.OrganizationCustomer.TradeName,
                            CustomerAddress = "",
                            CustomerTIN = inv.OrganizationCustomer.TinNo,
                            SalesDate = inv.Date,
                            Type = inv.PaymentMethod,
                            SystemReference = inv.CashSalesInvoiceID.ToString(),
                            DocumentReference = inv.ReceiptReference,
                            FsInvoiceNumber = inv.FsInvoiceNo,
                            SalesPerson = inv.InvoicePrintedBy,
                            Branch = brachName,
                            Shop = storeName,
                            Remark = lineItem.Remark,
                        };
                        list.Add(lineRecord);
                    }
                }
            }
            return list;
        }


        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
        private string GetBranch(int branchId)
        {
            return _db.Branch.Find(branchId)?.Name;
        }
        private string GetStore(int storeId)
        {
            return _db.SalesStores.Find(storeId)?.Description;
        }
    }

}
