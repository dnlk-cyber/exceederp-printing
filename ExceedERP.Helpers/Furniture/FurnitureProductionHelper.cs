using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Furniture
{
    public static class FurnitureProductionHelper
    {
        public static OperationResult GenerateItem(FurnitureJobOrderProduction model, string user)
        {
            using (var db = new ExceedDbContext())
            {
                var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.Name.ToLower().Contains("finishedgoods"));
                if (itemCategory == null)
                    return OperationResult.Fail("item category with the name finished goods not found");
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == model.JobTypeId);
                if (jobType == null)
                    return OperationResult.Fail("Job type not found!");
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
                var result = Generate_GoodReceive(model, item, user);
                if (!result.Success)
                    return result;

                return OperationResult.Ok();
            }

        }


        private static OperationResult Generate_GoodReceive(FurnitureJobOrderProduction productionJob, InventoryItem item, string user)
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
                if (!result.Success)
                    return result;
                return OperationResult.Ok();
            }
        }

        private static OperationResult Generate_GoodReceiveItem(FurnitureJobOrderProduction productionJob, InventoryItem item, int id, string user)
        {
            using (var db = new ExceedDbContext())
            {
                var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == productionJob.FurnitureJobOrderFormId);
                if (jobOrder == null)
                    return OperationResult.Fail("job order not found!");
                var JobOrderItem = db.FurnitureJobOrderItems.FirstOrDefault(x => x.JobNo == productionJob.JobNo);
                if (jobOrder == null)
                    return OperationResult.Fail("job order item not found!");
                var glTaxRate = db.FurnitureProformaInvoices
                    .FirstOrDefault(x => x.FurnitureProformaInvoiceId == jobOrder.FurnitureProformaInvoiceId);
                   
                if (glTaxRate == null)
                    return OperationResult.Fail("GL Tax rate not found!");
                GoodReceiveItem goodReceive = new GoodReceiveItem
                {
                    ItemCategoryCode = "",
                    ItemCode = "",
                    UOM = JobOrderItem.UnitOfMeasurment,
                    Quantity = JobOrderItem.Quantity,
                    UnitPrice = JobOrderItem.UnitPriceBeforeVAT,
                    GLTaxRateID = glTaxRate.GLTaxRateID,
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
                return OperationResult.Ok();
            }
        }
    }
}
