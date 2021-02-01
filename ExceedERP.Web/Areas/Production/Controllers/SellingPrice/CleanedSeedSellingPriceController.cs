using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class CleanedSeedSellingPriceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult SellingPrices_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CleanedSeedSellingPrice> items = db.CleanedSeedSellingPrices;
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                CropId = item.CropId,
                ProductionYearId = item.ProductionYearId,
                SellingPrice = item.SellingPrice,
                ApprovedSellingPrice = item.ApprovedSellingPrice,
                

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult SellingPriceEdit_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var record = db.CleanedSeedSellingPrices.Find(id);
            List<CleanedSeedSellingPriceEditViewModel> list = new List<CleanedSeedSellingPriceEditViewModel>();
            if(record != null)
            {
                var vm = new CleanedSeedSellingPriceEditViewModel
                {
                    CleanedSeedSellingPriceId = record.CleanedSeedSellingPriceId,
                    Crop = record.Crop.Name,
                    UoM = InventoryHelper.GetUnitOfMeasurment(record.Crop.ItemCode),
                    SellingPrice = record.SellingPrice,
                    ApprovedSellingPrice = record.ApprovedSellingPrice
                };
                list.Add(vm);
            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        private bool SeedInProductionYearExists(CleanedSeedSellingPrice model)
        {
            return 
                db.CleanedSeedSellingPrices
                .FirstOrDefault(cs => cs.CropId == model.CropId && 
                                    cs.ProductionYearId == model.ProductionYearId && 
                                    cs.CleanedSeedSellingPriceId != model.CleanedSeedSellingPriceId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Create([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPrice item)
        {
            if (ModelState.IsValid)
            {
                if (SeedInProductionYearExists(item))
                {
                    ModelState.AddModelError("", "Item Exists already!");
                }
                else
                {
                    var entity = new CleanedSeedSellingPrice
                    {
                        CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                        CropId = item.CropId,
                        ProductionYearId = item.ProductionYearId,
                        SellingPrice = item.SellingPrice,
                        ApprovedSellingPrice = item.ApprovedSellingPrice,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy
                    };
                    entity.ApprovedSellingPrice = entity.SellingPrice;

                    db.CleanedSeedSellingPrices.Add(entity);
                    db.SaveChanges();
                    item.CleanedSeedSellingPriceId = entity.CleanedSeedSellingPriceId;
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Update([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPrice item)
        {
            if (ModelState.IsValid)
            {
                if (SeedInProductionYearExists(item))
                {
                    ModelState.AddModelError("", "Item Exists already!");
                }
                else
                {
                    var entity = new CleanedSeedSellingPrice
                    {
                        CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                        CropId = item.CropId,
                        ProductionYearId = item.ProductionYearId,
                        SellingPrice = item.SellingPrice,
                        ApprovedSellingPrice = item.ApprovedSellingPrice,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };


                    db.CleanedSeedSellingPrices.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult SellingPrice_UpdatePrice([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPriceEditViewModel viewModel)
        {
            var entity = db.CleanedSeedSellingPrices.Find(viewModel.CleanedSeedSellingPriceId);
            if(entity != null)
            {
                entity.ApprovedSellingPrice = viewModel.ApprovedSellingPrice;

                db.CleanedSeedSellingPrices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Destroy([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPrice item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedSeedSellingPrice
                {
                    CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                    CropId = item.CropId,
                    ProductionYearId = item.ProductionYearId,
                    SellingPrice = item.SellingPrice,
                    ApprovedSellingPrice = item.ApprovedSellingPrice,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy
                };

                db.CleanedSeedSellingPrices.Attach(entity);
                db.CleanedSeedSellingPrices.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        //private string[] GetInventoryItemCodes(int seedProcessingItemId)
        //{
        //    string[] codes = new string[2];
        //    var seedProcessingItem = db.SeedProcessingOrderItems.Find(seedProcessingItemId);
        //    if( seedProcessingItem != null)
        //    {
                
        //            var currentSeedItem = db.SeedProductionOrderItems.Find(seedProcessingItem.SeedProductionOrderItemId);
        //            if(currentSeedItem != null)
        //            {
        //                var crop = db.Crops.Find(currentSeedItem.CropId);
        //                if(crop != null)
        //                {
        //                    codes[0] = crop.ItemCode;
        //                    codes[1] = crop.ItemCategoryCode;
        //                }
        //            }
        //    }
            
        //    return codes;
        //}
        //private decimal[]  GetProductionItemQuantities(int parentId, CleanedSeedSellingPrice record) {
        //    var itemToreturn = new CleanedSeedSellingPrice();
        //    decimal[] valuesToReturn = new decimal[2];
        //    var parentItem = db.ProductionStoreReceives.Find(parentId);

        //    if(parentItem != null)
        //    {
        //        if(parentItem.ProductType == ProductType.Seed)
        //        {
        //            var productionItem = db.SeedProductionOrderItems.Find(record.SeedProductionOrderItemId);
        //            if(productionItem != null)
        //            {
        //                valuesToReturn[0] = productionItem.QuantityProduced;
        //                valuesToReturn[1] = productionItem.TotalCost;
        //            }
        //        }
        //        else
        //        {
        //            var productionItem = db.GumProductionOrderItems.Find(record.GumProductionOrderItemId);
        //            if(productionItem != null)
        //            {
        //                valuesToReturn[0] = productionItem.QuantityProduced;
        //                valuesToReturn[1] = productionItem.TotalCost;
        //            }
        //        }
        //    }
        //    return valuesToReturn;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}