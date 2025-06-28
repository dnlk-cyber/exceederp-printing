using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.SalesStores.Datasource
{
    [DataObject]
    public class CashSalesInvoiceODS
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashSalesInvoiceVM> GetParams(string storeId, string productId)
        {
            int.TryParse(storeId, out int storeID);
            int.TryParse(productId, out int productID);

            var result = new List<CashSalesInvoiceVM>();
            string SaleQuoteDefination = "0";

            var store = db.SalesStores.FirstOrDefault(x => x.SalesStoreId == storeID);
            if (store == null)
                return result; // Return empty if store not found

            // Get all products if no specific product is selected
            var products = (productID > 0)
                ? db.Products.Where(p => p.ProductID == productID).ToList()
                : db.Products.ToList();

            foreach (var product in products)
            {
                var fromInventory = db.InventoryStores.FirstOrDefault(sc => sc.StoreCode == store.StoreCode);
                if (fromInventory == null)
                    continue;

                var storeItemsList = db.StoreItemAssignments
                    .Where(x => x.ItemCode == product.ItemCode && x.StoreCode == fromInventory.StoreCode)
                    .ToList();

                if (!storeItemsList.Any())
                    continue;

                var notApprovedQuotes = db.SalesQuoteVouchers.Where(s => !s.IsOnlineApproved).ToList();
                var notApprovedQuoteIds = new HashSet<string>(notApprovedQuotes.Select(q => q.SalesQuoteVoucherID.ToString()));

                var salesQuoteItems = db.LineItems
                    .Where(l => l.ThingID == product.ProductID
                                && l.VoucherDefinitionID == SaleQuoteDefination
                                && l.StoreID == store.SalesStoreId
                                && notApprovedQuoteIds.Contains(l.Voucher))
                    .ToList();

                var approvedNotShippedInvoices = db.CashSalesInvoices
                    .Where(c => c.IsOnlineApproved && !c.IsShipped)
                    .ToList();
                var approvedNotShippedInvoiceIds = new HashSet<string>(approvedNotShippedInvoices.Select(c => c.CashSalesInvoiceID.ToString()));

                var approvedNotShippedItems = db.LineItems
                    .Where(l => l.ThingID == product.ProductID
                                && l.VoucherDefinitionID == "2"
                                && l.StoreID == store.SalesStoreId
                                && approvedNotShippedInvoiceIds.Contains(l.Voucher))
                    .ToList();

                decimal quoteQty = salesQuoteItems.Sum(s => s.Quantity);
                decimal notShippedInvoiceQty = approvedNotShippedItems.Sum(l => l.Quantity);
                decimal totalActualBalance = storeItemsList.Sum(s => s.Balance);
                decimal reserved = quoteQty + notShippedInvoiceQty;

                result.Add(new CashSalesInvoiceVM
                {
                    reservedBalance = reserved,
                    stockBalance = totalActualBalance,
                    netBalance = totalActualBalance - reserved,
                    ItemName = product.Name
                });
            }

            return result;
        }
    }

    public class CashSalesInvoiceVM
    {
        public decimal stockBalance { get; set; }
        public decimal reservedBalance { get; set; }
        public decimal netBalance { get; set; }
        public string ItemName { get; set; }
    }
}