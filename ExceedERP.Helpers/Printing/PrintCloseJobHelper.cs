using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Manufacturing.Production;
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
        

        public static bool GenerateItems(FurnitureJobOrderProduction model, string user)
        {
            using (var db = new ExceedDbContext())
            {
                var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"));
                if (itemCategory == null)
                    return false;
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == model.JobTypeId);
                if (jobType == null)
                    return false;
                var item = new InventoryItem
                {
                    ItemCategoryCode = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"))?.ItemCategoryCode ?? "",
                    SubCategoryCode = "",
                    ItemCode = model.JobNo,
                    Name = jobType?.JobTypeName ?? "",
                    UoMName = model.Unit,

                };
                db.InventoryItems.Add(item);
                db.SaveChanges();
                if (!Generate_GoodReceive(model, item, user)) return false;

                return true;
            }
            
        }

        private static bool Generate_GoodReceive(FurnitureJobOrderProduction productionJob, InventoryItem item, string user)
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
                if (!Generate_GoodReceiveItem(productionJob, item, goodReceive.GoodReceiveId, user)) return false;
                return true;
            }
            
        }

        private static bool Generate_GoodReceiveItem(FurnitureJobOrderProduction productionJob, InventoryItem item, int id, string user)
        {
            using(var db = new ExceedDbContext())
            {
                var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == productionJob.FurnitureJobOrderFormId);
                if (jobOrder == null)
                    return false;
                var JobOrderItem = db.FurnitureJobOrderItems.FirstOrDefault(x => x.JobNo == productionJob.JobNo);
                if (jobOrder == null)
                    return false;
                var glTaxRateId = db.FurnitureProformaInvoices
                    .FirstOrDefault(x => x.FurnitureProformaInvoiceId == jobOrder.FurnitureProformaInvoiceId)
                    ?.GLTaxRateID ?? 0;
                GoodReceiveItem goodReceive = new GoodReceiveItem
                {
                    ItemCategoryCode = "",
                    ItemCode = "",
                    UOM = JobOrderItem.UnitOfMeasurment,
                    Quantity = JobOrderItem.Quantity,
                    UnitPrice = JobOrderItem.UnitPriceBeforeVAT,
                    GLTaxRateID = glTaxRateId,
                    SubTotal = JobOrderItem.UnitPriceWithVAT,
                    TaxAmount = JobOrderItem.VAT,
                    Total = JobOrderItem.UnitPriceWithVAT * JobOrderItem.Quantity,
                    Specification = "",
                    CreatedBy = user,
                    DateCreated = DateTime.Now,
                    GoodReceiveId = id
                };
                db.GoodReceiveItems.Add(goodReceive);
                db.SaveChanges();
                return true;
            }
           
        }
    }
}
