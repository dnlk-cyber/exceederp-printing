using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Procurement;
using System;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class MaterialPriceIndexController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaterialPriceIndex_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialPriceIndex> materialPriceIndexes = db.MaterialPriceIndexIndices;
            DataSourceResult result = materialPriceIndexes.ToDataSourceResult(request, material => new
            {
                MaterialPriceIndexId = material.MaterialPriceIndexId,
                BranchId = material.BranchId,
                SourceType = material.SourceType,
                GLPeriodId = material.GLPeriodId,
                DateOfInformation = material.DateOfInformation,
                ItemCategoryCode = material.ItemCategoryCode,
                ItemCode = material.ItemCode,
                ApprovedCost = material.ApprovedCost,
                Remark = material.Remark,
                OrginalCost = material.OrginalCost,
                ExternalCost = material.ExternalCost,
                OrginalCostGRN = material.OrginalCostGRN,
                TotalCost = material.TotalCost,
                TransportCost = material.TransportCost,
                Uom = material.Uom,
                //LoadingUnloading = material.LoadingUnloading,
                IsExpired = material.IsExpired,

                CreatedBy = material.CreatedBy,
                DateCreated = material.DateCreated,
                ModifiedBy = material.ModifiedBy,
                LastModified = material.LastModified
            });

            return Json(result);
        }
        private bool CheckExisatnce(string equipmentId, SourceType source)
        {
            var equipment = db.MaterialPriceIndexIndices.Where(x => x.ItemCode == equipmentId && x.SourceType == source).FirstOrDefault();
            if (equipment != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialPriceIndex_Create([DataSourceRequest]DataSourceRequest request, MaterialPriceIndex material)
        {
            if (ModelState.IsValid)
            {
                if (CheckExisatnce(material.ItemCode, material.SourceType))
                {
                    ModelState.AddModelError("", "This Material is already registered!!");
                }
                else if (!DateIsInPeriodRange(material.DateOfInformation, material.GLPeriodId))
                {
                    ModelState.AddModelError("", "Date must be within the period Range!!!");
                }
                else
                {
                    var entity = new MaterialPriceIndex
                    {
                        //MaterialPriceIndexId = MaterialPriceIndex.MaterialPriceIndexId,
                        BranchId = material.BranchId,
                        SourceType = SourceType.External,
                        GLPeriodId = material.GLPeriodId,
                        DateOfInformation = material.DateOfInformation,
                        InventoryItem = material.InventoryItem,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        ApprovedCost = material.ApprovedCost,
                        Remark = material.Remark,
                        OrginalCost = GetOrginalPrice(material.ItemCode),
                        ExternalCost = material.ExternalCost,
                        TotalCost = material.TotalCost,
                        OrginalCostGRN = material.OrginalCostGRN,
                        TransportCost = material.TransportCost,
                        Uom = GetUom(material.ItemCode),
                        //LoadingUnloading = material.LoadingUnloading,
                        CreatedBy = material.CreatedBy,
                        DateCreated = material.DateCreated,
                        ModifiedBy = material.ModifiedBy,
                        LastModified = material.LastModified
                    };
                    entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost ;
                    entity.ApprovedCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost ;
                    db.MaterialPriceIndexIndices.Add(entity);
                    db.SaveChanges();
                    material.MaterialPriceIndexId = entity.MaterialPriceIndexId;
                    material.OrginalCost = entity.OrginalCost;

                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialPriceIndex_Update([DataSourceRequest]DataSourceRequest request, MaterialPriceIndex material)
        {
            if (ModelState.IsValid)
            {
                if (!DateIsInPeriodRange(material.DateOfInformation, material.GLPeriodId))
                {
                    ModelState.AddModelError("", "Date must be within the period Range!!!");
                }
                else
                {
                    var entity = new MaterialPriceIndex
                    {
                        MaterialPriceIndexId = material.MaterialPriceIndexId,
                        BranchId = material.BranchId,
                        GLPeriodId = material.GLPeriodId,
                        DateOfInformation = material.DateOfInformation,
                        SourceType = material.SourceType,
                        InventoryItem = material.InventoryItem,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        ApprovedCost = material.ApprovedCost,
                        OrginalCostGRN = material.OrginalCostGRN,
                        Remark = material.Remark,
                        OrginalCost = GetOrginalPrice(material.ItemCode),
                        ExternalCost = material.ExternalCost,
                        TotalCost = material.TotalCost,
                        TransportCost = material.TransportCost,
                        Uom = GetUom(material.ItemCode),
                        //LoadingUnloading = material.LoadingUnloading,
                        CreatedBy = material.CreatedBy,
                        DateCreated = material.DateCreated,
                        ModifiedBy = material.ModifiedBy,
                        LastModified = material.LastModified
                    };
                    entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost ;

                    db.MaterialPriceIndexIndices.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    material.OrginalCost = entity.OrginalCost;
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialPriceIndex_Destroy([DataSourceRequest]DataSourceRequest request, MaterialPriceIndex material)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialPriceIndex
                {
                    MaterialPriceIndexId = material.MaterialPriceIndexId,
                    BranchId = material.BranchId,
                    GLPeriodId = material.GLPeriodId,
                    DateOfInformation = material.DateOfInformation,
                    SourceType = material.SourceType,
                    InventoryItem = material.InventoryItem,
                    ItemCategoryCode = material.ItemCategoryCode,
                    ItemCode = material.ItemCode,
                    ApprovedCost = material.ApprovedCost,
                    Remark = material.Remark,
                    OrginalCost = material.OrginalCost,
                    ExternalCost = material.ExternalCost,
                    TotalCost = material.TotalCost,
                    OrginalCostGRN = material.OrginalCostGRN,
                    TransportCost = material.TransportCost,
                    Uom = material.Uom,
                    //LoadingUnloading = material.LoadingUnloading,
                    CreatedBy = material.CreatedBy,
                    DateCreated = material.DateCreated,
                    ModifiedBy = material.ModifiedBy,
                    LastModified = material.LastModified
                };

                db.MaterialPriceIndexIndices.Attach(entity);
                db.MaterialPriceIndexIndices.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        private string GetUom(string itemCode)
        {
            var items = db.InventoryItems.Where(x => x.ItemCode == itemCode).FirstOrDefault();
            if (items != null)
            {
                return items.UoMName;
            }
            return "";
        }
        private decimal GetOrginalPrice(string itemCode)
        {
            var items = db.ItemPriceIndexes.Where(x => x.ItemCode == itemCode).FirstOrDefault();
            if (items != null)
            {
                return items.PriceBeforeVAT;
            }
            return 0;
        }
        private bool DateIsInPeriodRange(DateTime date, int periodId)
        {
            bool isInRange = false;
            var currentPeriod = db.InventoryPeriods.Find(periodId);
            if(currentPeriod != null)
            {
                if (date <= currentPeriod.DateTo && date >= currentPeriod.DateFrom)
                {
                    isInRange = true;
                }
            }
            return isInRange;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
