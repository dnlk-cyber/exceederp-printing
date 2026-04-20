using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{

    public static class PrintCloseJobHelper
    {
        

        public static OperationResult GenerateItems(PrintingProductionFollowUp model, string user)
        {
            using (var db = new ExceedDbContext())
            {
                var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"));
                if (itemCategory == null)
                    return OperationResult.Fail("Item Category with name Finishedgoods not found! make sure to include it in inventory item");
               
                var item = new InventoryItem
                {
                    ItemCategoryCode = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"))?.ItemCategoryCode ?? "",
                    SubCategoryCode = "",
                    ItemCode = model.PrintingJobOrderItemId.ToString(),
                    Name = model.PrintJobType.ToString(),
                    UoMName = "",

                };
                db.InventoryItems.Add(item);
                db.SaveChanges();
                var result = Generate_GoodReceive(model, item, user);
                if (!result.Success) return result;

                return OperationResult.Ok("");
            }
            
        }

        private static OperationResult Generate_GoodReceive(PrintingProductionFollowUp productionJob, InventoryItem item, string user)
        {
            using (var db = new ExceedDbContext())
            {
                GoodReceive goodReceive = new GoodReceive()
                {
                    StoreCode = db.InventoryStores.FirstOrDefault(x => x.Name.Contains("furniture"))?.StoreCode ?? "",
                    ReceiveDate = DateTime.Today,
                    GLPeriodId = db.GLPeriods.FirstOrDefault(x => x.DateFrom < DateTime.Today && x.DateTo > DateTime.Today)?.GLPeriodId ?? 0,
                    ReceiveTypeId = db.ReceiveTypes.FirstOrDefault(x => x.Type != GoodReceiveType.Other)?.ReceiveTypeId ?? 0,
                    ReceiveRef = "",
                    Reference = "",
                    SupplierId = "",
                    SupplierName = "",
                    SupplierInvoiceNo = "",
                    
                };
                db.GoodReceives.Add(goodReceive);
                db.SaveChanges();
                var result = Generate_GoodReceiveItem(productionJob, item, goodReceive.GoodReceiveId, user);
                if (!result.Success) return result;
                return OperationResult.Ok();
            }
            
        }

        private static OperationResult Generate_GoodReceiveItem(PrintingProductionFollowUp productionJob, InventoryItem item, int id, string user)
        {
            using(var db = new ExceedDbContext())
            {
                var jobOrderItem = db.PrintingJobOrderItems.FirstOrDefault(x => x.PrintingJobOrderItemId == productionJob.PrintingJobOrderItemId);
                if (jobOrderItem == null)
                    return OperationResult.Fail("Job order item not found!");
                var jobOrder = db.PrintingJobOrders.FirstOrDefault(x => x.PrintingJobOrderId == jobOrderItem.PrintingJobOrderId);
                if (jobOrder == null)
                    return OperationResult.Fail("Job Order not found!");
                //var proformaInvoice = db.PrintingProformaInvoices
                //    .FirstOrDefault(x => x.PrintingProformaInvoiceId == jobOrder.PrintingProformaInvoiceId);

                var proformaInvoiceItem = db.PrintingProformaInvoiceItems
                    .SingleOrDefault(x => x.PrintingProformaInvoiceId == jobOrder.PrintingProformaInvoiceId && x.PrintJobType == jobOrderItem.PrintJobType);

                if (proformaInvoiceItem == null) return OperationResult.Fail("Proforma invoice not found!");

                GoodReceiveItem goodReceive = new GoodReceiveItem
                {
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    UOM = item.UoMName,
                    Quantity = jobOrderItem.Quantity,
                    UnitPrice = jobOrderItem.UnitPrice,
                    GLTaxRateID = (int)proformaInvoiceItem.GLTaxRateID,
                    SubTotal = jobOrderItem.TotalPrice,
                    TaxAmount = jobOrderItem.VAT,
                    Total = jobOrderItem.TotalPrice,
                    Specification = "",
                    CreatedBy = user,
                    DateCreated = DateTime.Now,
                    GoodReceiveId = id
                };
                db.GoodReceiveItems.Add(goodReceive);
                db.SaveChanges();
                return OperationResult.Ok("");
            }
           
        }
    }
}
