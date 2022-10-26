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
    public class SalesSummaryReportOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesSummaryReportViewModel> GetSalesData(DateTime start, DateTime end, string branchId, string storeId, string customerId, string salesPerson, string status)
        {
            List<SalesSummaryReportViewModel> list = new List<SalesSummaryReportViewModel>();

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
            var invIds = new HashSet<string>(sales.Select(i => i.CashSalesInvoiceID.ToString()));
            var allLineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "2" && invIds.Contains(l.Voucher)).ToList();
            var catIds = new HashSet<string>(allLineItems.Select(l => l.Product.CategoryID));
            var allCategories = _db.ProductCategories.Where(c => catIds.Contains(c.ProductCategoryId.ToString())).ToList();
            var parentCategories =allCategories.Where(c => c.Parent == null).ToList();
            foreach (ProductCategory parentCategory in parentCategories)
            {
                var subCategories = _db.ProductCategories.Where(c => c.Parent == parentCategory.ProductCategoryId.ToString()).ToList();

                //var subCategories = allCategories.Where(c => c.Parent == parentCategory.ProductCategoryId.ToString()).ToList();
                if (!subCategories.Any())
                {
                    #region parent categories
                    var catSpecificList = allLineItems.Where(l => l.Product.CategoryID == parentCategory.ProductCategoryId.ToString()).ToList();
                    decimal adds = 0;
                    decimal discount = 0;
                    decimal subtotal = 0;
                    decimal tax = 0;
                    decimal total = 0;
                    if (catSpecificList.Any())
                    {
                        discount = catSpecificList.Sum(c => c.Discount);
                        subtotal = catSpecificList.Sum(c => c.CalculatedCost);
                        tax = catSpecificList.Sum(c => c.TaxAmount);
                        total = catSpecificList.Sum(c => c.TotalAmount);
                    }
                    var catRecord = new SalesSummaryReportViewModel
                    {
                        //ItemCategory = GetProductCategory(catId),
                        ItemCategory = parentCategory.Description,
                        Adds = adds,
                        Discount = discount,
                        SubTotal = subtotal,
                        TaxAmount = tax,
                        TotalAmount = total
                    };
                    var prodIds = new HashSet<int>(catSpecificList.Select(c => c.ThingID));
                    foreach (var productId in prodIds)
                    {
                        var prodSpecificLines = catSpecificList.Where(l => l.ThingID == productId).ToList();
                        decimal itemQty = 0;
                        decimal itemUPrice = 0;
                        decimal itemSubTotal = 0;
                        decimal itemAdds = 0;
                        decimal itemDiscount = 0;
                        decimal itemTax = 0;
                        decimal itemTotalAmount = 0;
                        if (prodSpecificLines.Any())
                        {
                            itemQty = prodSpecificLines.Sum(i => i.Quantity);
                            itemUPrice = prodSpecificLines.Average(i => i.UnitAmount);
                            itemSubTotal = prodSpecificLines.Sum(i => i.CalculatedCost);
                            itemDiscount = prodSpecificLines.Sum(i => i.Discount);
                            itemTax = prodSpecificLines.Sum(i => i.TaxAmount);
                            itemTotalAmount = prodSpecificLines.Sum(i => i.TotalAmount);
                        }

                        var selectedProduct = allProducts.FirstOrDefault(p => p.ProductID == productId);
                        if (selectedProduct != null)
                        {
                            var lineItemRecord = new SalesSummaryReportLineItemViewModel
                            {
                                ItemCode = selectedProduct.ItemCode,
                                ItemName = selectedProduct.Name,
                                UoM = selectedProduct.UOM,
                                Quantity = itemQty,
                                UnitPrice = itemUPrice,
                                Adds = itemAdds,
                                Discount = itemDiscount,
                                SubTotal = itemSubTotal,
                                TaxAmount = itemTax,
                                TotalAmount = itemTotalAmount,
                            };
                            catRecord.LineItems.Add(lineItemRecord);
                        }
                    }
                    list.Add(catRecord);
                    #endregion
                }
                else
                {
                    foreach (ProductCategory subCategory in subCategories)
                    {
                        #region sub category
                        var catSpecificList = allLineItems.Where(l => l.Product.SubCategoryId == subCategory.ProductCategoryId.ToString()).ToList();
                        decimal adds = 0;
                        decimal discount = 0;
                        decimal subtotal = 0;
                        decimal tax = 0;
                        decimal total = 0;
                        if (catSpecificList.Any())
                        {
                            discount = catSpecificList.Sum(c => c.Discount);
                            subtotal = catSpecificList.Sum(c => c.CalculatedCost);
                            tax = catSpecificList.Sum(c => c.TaxAmount);
                            total = catSpecificList.Sum(c => c.TotalAmount);
                        }
                        var catRecord = new SalesSummaryReportViewModel
                        {
                            //ItemCategory = GetProductCategory(catId),
                            ItemCategory = subCategory.Description,

                            Adds = adds,
                            Discount = discount,
                            SubTotal = subtotal,
                            TaxAmount = tax,
                            TotalAmount = total
                        };
                        var prodIds = new HashSet<int>(catSpecificList.Select(c => c.ThingID));
                        foreach (var productId in prodIds)
                        {
                            var prodSpecificLines = catSpecificList.Where(l => l.ThingID == productId).ToList();
                            decimal itemQty = 0;
                            decimal itemUPrice = 0;
                            decimal itemSubTotal = 0;
                            decimal itemAdds = 0;
                            decimal itemDiscount = 0;
                            decimal itemTax = 0;
                            decimal itemTotalAmount = 0;
                            if (prodSpecificLines.Any())
                            {
                                itemQty = prodSpecificLines.Sum(i => i.Quantity);
                                itemUPrice = prodSpecificLines.Average(i => i.UnitAmount);
                                itemSubTotal = prodSpecificLines.Sum(i => i.CalculatedCost);
                                itemDiscount = prodSpecificLines.Sum(i => i.Discount);
                                itemTax = prodSpecificLines.Sum(i => i.TaxAmount);
                                itemTotalAmount = prodSpecificLines.Sum(i => i.TotalAmount);
                            }

                            var selectedProduct = allProducts.FirstOrDefault(p => p.ProductID == productId);
                            if (selectedProduct != null)
                            {
                                var lineItemRecord = new SalesSummaryReportLineItemViewModel
                                {
                                    ItemCode = selectedProduct.ItemCode,
                                    ItemName = selectedProduct.Name,
                                    UoM = selectedProduct.UOM,
                                    Quantity = itemQty,
                                    UnitPrice = itemUPrice,
                                    Adds = itemAdds,
                                    Discount = itemDiscount,
                                    SubTotal = itemSubTotal,
                                    TaxAmount = itemTax,
                                    TotalAmount = itemTotalAmount,
                                };
                                catRecord.LineItems.Add(lineItemRecord);
                            }
                        }
                        list.Add(catRecord);
                        #endregion
                    }
                }
            }
            //foreach (var catId in catIds)
            //{
                
            //}
           
            return list;
        }


        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
        //private string GetBranch(int branchId)
        //{
        //    return _db.Branch.Find(branchId)?.Name;
        //}
        //private string GetStore(int storeId)
        //{
        //    return _db.SalesStores.Find(storeId)?.Description;
        //}
    }

}
