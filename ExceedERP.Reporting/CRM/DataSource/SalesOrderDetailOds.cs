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
    public class SalesOrderDetailOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesOrderDetailViewModel> GetSalesOrderData(string branchId, string storeId, string customerId, DateTime start, DateTime end)
        {
            //if (branchId == null)
            //{
            //    return null;
            //}
            var temp = _db.SalesOrderVouchers.ToList();
            var orders = new List<SalesOrderVoucher>();

            foreach (var o in temp)
            {
                if(o.Date.Date <= end.Date && o.Date.Date >= start.Date)
                {
                    orders.Add(o);
                }
            }
            if (branchId != null)
            {
                int bId = int.Parse(branchId);
                orders = orders.Where(o => o.BranchId == bId).ToList();
            }
            if(customerId != null)
            {
                int cId = int.Parse(customerId);
                orders = orders.Where(o => o.OrganizationCustomerId == cId).ToList();
            }

            List<SalesOrderDetailViewModel> list = new List<SalesOrderDetailViewModel>();
            var tempList = new List<SalesOrderVoucher>();
            tempList.AddRange(orders);
            if (!string.IsNullOrEmpty(storeId))
            {
                int sId = int.Parse(storeId);
                var lineItemsInStore =
                    _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.StoreID == sId).ToList();
                var vIds = new HashSet<string>(lineItemsInStore.Select(l => l.Voucher));
                foreach (SalesOrderVoucher so in tempList)
                {
                    var voucher = so.SalesOrderVoucherID.ToString();
                    if (!vIds.Contains(voucher))
                    {
                        orders.Remove(so);
                    }
                }
            }
            foreach (SalesOrderVoucher so in orders)
            {
                //var record = new SalesOrderDetailViewModel
                //{
                //    Customer = so.OrganizationCustomer.TradeName,
                //    OrderType = so.PaymentMethod,
                //    OrderNumber = so.SalesOrderVoucherID,
                //    OrderReference = "",
                //    OrderDate = so.Date,
                //    //OrderDueDate = so.Dude,
                //    OrderStatus = so.LastObjectState,
                //    //QuotationTerm = GetQuotationTerm(so.QuotationTermId),
                //};

                var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "1" && l.Voucher == so.SalesOrderVoucherID.ToString()).ToList();
                if (lineItems.Any())
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        var lineRecord = new SalesOrderDetailViewModel
                        {
                            Customer = so.OrganizationCustomer.TradeName,
                            OrderType = so.PaymentMethod,
                            OrderNumber = so.SalesOrderVoucherID,
                            OrderReference = "",
                            OrderDate = so.Date,
                            OrderStatus = so.LastObjectState,

                            Class = lineItem.Class,
                            Variety = lineItem.Variety,
                            ItemCategory = GetProductCategory(lineItem.Product.CategoryID),
                            ItemCode = lineItem.Product.ItemCode,
                            ItemName = lineItem.Product.Name,
                            Quantity = lineItem.Quantity,
                            Store = lineItem.SalesStore.Description,
                            UoM = lineItem.Product.UOM,
                            UnitPrice = lineItem.UnitAmount,
                            SubTotal = lineItem.CalculatedCost,
                            Tax = lineItem.TaxAmount,
                            TotalAmount = lineItem.TotalAmount,
                            Remarks = lineItem.Remark
                        };
                        list.Add(lineRecord);
                    }
                }
            }
            return list;
        }

        //private string GetQuotationTerm(int id)
        //{
        //    return _db.QuotationTerms.Find(id)?.QuotationInDays + " days";
        //}
        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
    }

}
