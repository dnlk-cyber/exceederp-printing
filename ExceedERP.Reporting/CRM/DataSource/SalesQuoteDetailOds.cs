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
    public class SalesQuoteDetailOds
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesQuoteDetailViewModel> GetSalesQuoteData(string branchId, string storeId,string customerId, DateTime start, DateTime end)
        {
            //if (branchId == null)
            //{
            //    return null;
            //}


            List<SalesQuoteDetailViewModel> list = new List<SalesQuoteDetailViewModel>();
            var salesQuotes = new List<SalesQuoteVoucher>();
            var qts = _db.SalesQuoteVouchers.ToList();
            foreach(var q in qts)
            {
                if(q.Date.Date <= end.Date && q.Date >= start.Date)
                {
                    salesQuotes.Add(q);
                }
            }
            if (branchId != null)
            {
                int bId = int.Parse(branchId);

                salesQuotes = salesQuotes.Where(s => s.BranchId == bId).ToList();
            }
            if(customerId != null)
            {
                int cId = int.Parse(customerId);
                salesQuotes = salesQuotes.Where(s => s.OrganizationCustomerId == cId).ToList();

            }
            var tempList = new List<SalesQuoteVoucher>();
            tempList.AddRange(salesQuotes);
            if (!string.IsNullOrEmpty(storeId))
            {
                int sId = int.Parse(storeId);
                var lineItemsInStore =
                    _db.LineItems.Where(l => l.VoucherDefinitionID == "0" && l.StoreID == sId).ToList();
                var vIds = new HashSet<string>(lineItemsInStore.Select(l => l.Voucher));
                foreach (SalesQuoteVoucher sq in tempList)
                {
                    var voucher = sq.SalesQuoteVoucherID.ToString();
                    if (!vIds.Contains(voucher))
                    {
                        salesQuotes.Remove(sq);
                    }
                }
            }
            foreach (SalesQuoteVoucher sq in salesQuotes)
            {
                //var recorddd = new SalesQuoteDetailViewModel
                //{
                //    Customer = sq.OrganizationCustomer.TradeName,
                //    OrderType = sq.PaymentMethod,
                //    QuoteNumber = sq.SalesQuoteVoucherID,
                //    QuoteReference = "",
                //    QuoteDate = sq.Date,
                //    QuoteDueDate = sq.DueDate,
                //    QuoteStatus = sq.LastObjectState,
                //    QuotationTerm = GetQuotationTerm(sq.QuotationTermId),
                //};

                string term = GetQuotationTerm(sq.QuotationTermId);
                var lineItems = _db.LineItems.Where(l => l.VoucherDefinitionID == "0" && l.Voucher == sq.SalesQuoteVoucherID.ToString()).ToList();
                if (lineItems.Any())
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        var record = new SalesQuoteDetailViewModel
                        {
                            Customer = sq.OrganizationCustomer.TradeName,
                            OrderType = sq.PaymentMethod,
                            QuoteNumber = sq.SalesQuoteVoucherID,
                            QuoteReference = "",
                            QuoteDate = sq.Date,
                            QuoteDueDate = sq.DueDate,
                            QuoteStatus = sq.LastObjectState,
                            QuotationTerm = term,


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
                        //record.LineItems.Add(lineRecord);
                        list.Add(record);
                    }
                }

                //list.Add(record);
            }
            return list;
        }

        private string GetQuotationTerm(int id)
        {
            var term = _db.QuotationTerms.Find(id);
            return term != null? term.QuotationInDays + " days" : "";
        }
        private string GetProductCategory(string categoryId)
        {
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId)?.Description;
        }
    }

}
