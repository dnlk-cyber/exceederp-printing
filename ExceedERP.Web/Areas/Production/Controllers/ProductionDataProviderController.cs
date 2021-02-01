using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers
{
    public class ProductionDataProviderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //
        //  Summary :
        //      Returns the activities that are done in any stages of 
        //      production and processing 
        public JsonResult GetActivityTypes()
        {
            var activityTypes = db.ProductionSettings;
            return Json(activityTypes.Select(at => new { at.ProductionSettingId, Value = at.Value + " - [" + at.Type.ToString() + "]" }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCropNames()
        {
            var crops = db.Crops;
            return Json(crops.Select(c => new { c.CropId, c.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNaturalGumNames()
        {
            var crops = db.NaturalGums;
            return Json(crops.Select(c => new { c.NaturalGumId, c.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGlFiscalYears()
        {
            var years = db.GLFiscalYears;
            return Json(years.Select(y => new { y.GlFiscalYearId, y.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductionYears()
        {
            var years = db.ProductionYears;
            return Json(years.Select(y => new { y.ProductionYearId, y.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFarmingPlots(int branchId)
        {
            var plots = db.Plots.Where(p => p.ProductionSiteId == branchId);
            return Json(plots.Select(p => new { p.PlotCode, p.PlotId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFarmingPlotsInProductionIte(int siteId) {
            // return 'free' farming  blocks or plots in this site
            var plots = db.Plots.Where(p => p.ProductionSiteId == siteId && p.ProductionSettingId == 0);
            return Json(plots.Select(p => new { p.PlotCode, p.PlotId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranches()
        {
            var branches = db.Branch;
            return Json(branches.Select(b => new { b.Name, b.BranchId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProductionSites()
        {
            var sites = db.ProductionSites;

            return Json(sites.Select(s => new { s.ProductionSiteId, s.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSitesInBranch(int branchId)
        {
            var sites = db.ProductionSites.Where(s => s.BranchId == branchId);
            return Json(sites.Select(s => new { s.ProductionSiteId, s.Name }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// all activites : production and processing for both seed and natural gum
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProductionActivities()
        {
            var activites = db.ProductionActivities;
            return Json(activites.Select(a => new { a.ProductionActivityId, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeedProductionActivities()
        {
            var activities = db.ProductionActivities.Where(a => a.ActivityType.Type == Core.Domain.Production.ProductionSettingType.SeedProductionStage);
            return Json(activities.Select(a => new { a.ProductionActivityId, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeedProcessingActivities()
        {
            var activities = db.ProductionActivities.Where(a => a.ActivityType.Type == Core.Domain.Production.ProductionSettingType.SeedProcessingStage);
            return Json(activities.Select(a => new { a.ProductionActivityId, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGumProductionActivities()
        {
            var activities = db.ProductionActivities.Where(a => a.ActivityType.Type == Core.Domain.Production.ProductionSettingType.NaturalGumProductionStage);
            return Json(activities.Select(a => new { a.ProductionActivityId, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGumProcessingActivities()
        {
            var activities = db.ProductionActivities.Where(a => a.ActivityType.Type == Core.Domain.Production.ProductionSettingType.NaturalGumProcessingStage);
            return Json(activities.Select(a => new { a.ProductionActivityId, a.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult AllItemCategoryFromInventory()
        {
            var items = db.ItemCategorys.Where(c => !c.HaveParent).ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemCategoryOfMaterial()
        {

            //var joinedCategories = (from cIC in db.ProductionItemCategories
            //                        join mPI in db.MaterialPriceIndexIndices
            //                        on cIC.ItemCategoryCode equals mPI.ItemCategoryCode
            //                        //where wi.ProjectInformationId == currentProjectIdR
            //                        select new
            //                        {
            //                            categoryCode = mPI.ItemCategoryCode,
            //                            name = cIC.Name
            //                        }).ToList();
            //var codes = new HashSet<string>(joinedCategories.Select(ic => ic.categoryCode));

            var items = db.ProductionItemCategories;
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategoriesAvailableInStore(string storeCode, string filterString)
        {
            var listToReturn = new List<ProductionItemCategory>();

            var storeReceives = db.ProductionStoreReceives.Where(s => s.StoreCode == storeCode).ToList();
            if(storeReceives.Any())
            {
                var ids = new HashSet<int>(storeReceives.Select(s => s.ProductionStoreReceiveId));
                var items = db.ProductionStoreReceiveItems.Where(i => ids.Contains(i.ProductionStoreReceiveId));
                if (items.Any())
                {
                    var catIds = new HashSet<string>(items.Select(i => i.ItemCategoryCode));
                    listToReturn = db.ProductionItemCategories.Where(c => catIds.Contains(c.ItemCategoryCode)).ToList();
                }
            }
            if(filterString != null)
            {
                filterString = filterString.ToLower();
                listToReturn = listToReturn.Where(c => c.Name.Contains(filterString)).ToList();
            }
            return Json(listToReturn.Select(c => new { c.ItemCategoryCode, c.Name }), JsonRequestBehavior.AllowGet);

        }
        //  Summary :
        //      Get all inventory items under production item categories
        public JsonResult GetAllItems()
        {
            var categoryObjects = db.ProductionItemCategories.Select(c => new { c.ItemCategoryCode }).ToList();
            var codes = new HashSet<string>(categoryObjects.Select(c => c.ItemCategoryCode));
            var items = db.InventoryItems.Where(i => codes.Contains(i.ItemCategoryCode));
            
            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsUnderCategory(string ItemCategoryCode, string filterString)
        {
            //
            // find only the items that exist in the material price index calculation
            //var joinedItems = (from ii in db.InventoryItems
            //                   join mPI in db.MaterialPriceIndexIndices
            //                   on ii.ItemCode equals mPI.ItemCode
            //                   where mPI.ItemCategoryCode == ItemCategoryCode
            //                   select new
            //                   {
            //                       itemCode = mPI.ItemCode
            //                   }).ToList();
            //var codes = new HashSet<string>(joinedItems.Select(ic => ic.itemCode));
            //var items = db.InventoryItems.Where(item => (codes.Contains(item.ItemCode)));
            var items = db.InventoryItems.Where(itm => itm.ItemCategoryCode == ItemCategoryCode);
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(filterString));
            }
            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsUnderCategoryAndAvailableInStore(string ItemCategoryCode, string storeCode, string filterString)
        {
            var items = db.InventoryItems.Where(itm => itm.ItemCategoryCode == ItemCategoryCode).ToList();

            var storeReceives = db.ProductionStoreReceives.Where(s => s.StoreCode == storeCode).ToList();
            if (storeReceives.Any())
            {
                var ids = new HashSet<int>(storeReceives.Select(s => s.ProductionStoreReceiveId));
                var receiveItems = db.ProductionStoreReceiveItems.Where(i => ids.Contains(i.ProductionStoreReceiveId));
                if (receiveItems.Any())
                {
                    var itemCodes = new HashSet<string>(receiveItems.Select(i => i.ItemCode));
                    items = items.Where(c => itemCodes.Contains(c.ItemCode)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(filterString)).ToList();
            }
            return Json(items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductionPeriods()
        {
            var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open);
            return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductionPeriodsUnderSpecificYear(int id)
        {
            var currentYear = db.ProductionYears.Find(id);
            if (currentYear != null)
            {
                var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open && p.DateFrom >= currentYear.DateFrom && p.DateTo <= currentYear.DateTo);
                return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetProductionPeriodsUnderBudgetYear(int id)
        {
            var currentYear = db.GLFiscalYears.Find(id);
            if (currentYear != null)
            {
                var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open && p.DateFrom >= currentYear.DateFrom && p.DateTo <= currentYear.DateTo);
                return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetProductionPeriodsForCleanedItemSale(int itemId)
        {
            var item = db.CleanedProductSaleItems.Find(itemId);
            if(item != null)
            {
                var currentYear = db.GLFiscalYears.Find(item.CleanedProductSale.GlFiscalYearId);
                if (currentYear != null)
                {
                    var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open && p.DateFrom >= currentYear.DateFrom && p.DateTo <= currentYear.DateTo);
                    return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
                }
            }
           
            return null;
        }
        public JsonResult GetProductionPeriodsForRawItemPurchase(int itemId)
        {
            var item = db.RawProductPurchaseItems.Find(itemId);
            if (item != null)
            {
                var currentYear = db.GLFiscalYears.Find(item.RawProductPurchase.GlFiscalYearId);
                if (currentYear != null)
                {
                    var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open && p.DateFrom >= currentYear.DateFrom && p.DateTo <= currentYear.DateTo);
                    return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
        public JsonResult GetProductionPeriodsForCustomCleaning(int itemId)
        {
            var item = db.CustomCleaningAndStorageItems.Find(itemId);
            if (item != null)
            {
                var currentYear = db.GLFiscalYears.Find(item.CustomCleaningAndStorage.GlFiscalYearId);
                if (currentYear != null)
                {
                    var periods = db.ProductionPeriods.Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open && p.DateFrom >= currentYear.DateFrom && p.DateTo <= currentYear.DateTo);
                    return Json(periods.Select(p => new { p.ProductionPeriodId, p.Name }), JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
        public JsonResult GetSeedProductionOrderItems()
        {
            var items = db.SeedProductionOrderItems.Where(po => po.Status == Core.Domain.Production.ProductStatus.RawProduct);
            return Json(items.Select(itm => new { itm.SeedProductionOrderItemId, itm.ProductionCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGumProductionOrderItems()
        {
            var items = db.GumProductionOrderItems.Where(po => po.Status == Core.Domain.Production.ProductStatus.RawProduct);
            return Json(items.Select(itm => new { itm.GumProductionOrderItemId, itm.ProductionCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProcessingPlants(int type)
        {
            var processingPlants = db.ProcessingPlants.Where(p => p.PlantType == (Core.Domain.Production.ProcessingPlantType)type);
            return Json(processingPlants.Select(p => new { p.ProcessingPlantId, p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryStores()
        {
            var stores = db.InventoryStores;
            return Json(stores.Select(st => new { st.StoreCode, st.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGLTaxRates()
        {
            var taxTypes = db.GLTaxRates.Where(tr => tr.ApplyToAr);
            return Json(taxTypes.Select(p => new { GLTaxRateID = p.GLTaxRateID, TaxName = p.TaxName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeedProductionItems()
        {
            var items = db.SeedProductionOrderItems.Where(poi => poi.Status == Core.Domain.Production.ProductStatus.Production);

            return Json(items.Select(itm => new { itm.SeedProductionOrderItemId, itm.ProductionCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGumProductionItems()
        {
            var items = db.GumProductionOrderItems.Where(poi => poi.Status == Core.Domain.Production.ProductStatus.Production);

            return Json(items.Select(itm => new { itm.GumProductionOrderItemId, itm.ProductionCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeedProcessingItems()
        {
            var items = db.SeedProcessingOrderItems;

            return Json(items.Select(itm => new { itm.SeedProcessingOrderItemId, itm.ProcessingCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeedProcessingItemsOfSpecificCrop(int cropId)
        {
            var crop = db.Crops.Find(cropId);
            if(crop != null)
            {
                var items = db.SeedProcessingOrderItems.Where(po => po.ItemCode == crop.ItemCode);

                return Json(items.Select(itm => new { itm.SeedProcessingOrderItemId, itm.ProcessingCode }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetGumProcessingItems()
        {
            var items = db.GumProcessingOrderItems;

            return Json(items.Select(itm => new { itm.GumProcessingOrderItemId, itm.ProcessingCode }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGumProcessingItemsOfSpecificGum(int gumId)
        {

            var gum = db.NaturalGums.Find(gumId);
            if (gum != null)
            {
                var items = db.GumProcessingOrderItems.Where(po => po.ItemCode == gum.ItemCode);

                return Json(items.Select(itm => new { itm.GumProcessingOrderItemId, itm.ProcessingCode }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetCostCategories()
        {
            var items = db.ProductionAndProcessingCostCategories.Where(cc => cc.ParentId <= 0 || cc.ParentId == null);

            return Json(items.Select(itm => new { itm.ProductionAndProcessingCostCategoryId, itm.Title }), JsonRequestBehavior.AllowGet);
        }
        
        public string GetPeriodRanges(int periodId)
        {
            string range = null;
            var currentPeriod = db.ProductionPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                var start = currentPeriod.DateFrom.ToString("MMMM dd, yyyy");
                var end = currentPeriod.DateTo.ToString("MMMM dd, yyyy");
                if(!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    range = start + "_" + end;
                }
            }

            return range;
        }
    }
}