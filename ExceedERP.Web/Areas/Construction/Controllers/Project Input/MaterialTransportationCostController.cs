using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class MaterialTransportationCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult TransportCosts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialTransportationCost> materialCosts = db.MaterialTransportationCost;
            DataSourceResult result = materialCosts.ToDataSourceResult(request, materialCost => new
            {
                MaterialTransportationCostId = materialCost.MaterialTransportationCostId,
                BranchId = materialCost.BranchId,
                UoM = materialCost.UoM,
                TrackCapacityPerTrip = materialCost.TrackCapacityPerTrip,
                AverageSpeedOfTrack = materialCost.AverageSpeedOfTrack,
                ItemCategoryCode = materialCost.ItemCategoryCode,
                ItemCode = materialCost.ItemCode,
                RentalCostWithFuel = materialCost.RentalCostWithFuel,
                DriverPerdiem = materialCost.DriverPerdiem,
                AverageDistanceFromSourceToSite = materialCost.AverageDistanceFromSourceToSite,
                AssistantDriverPerdiem = materialCost.AssistantDriverPerdiem,
                RequiredTime = materialCost.RequiredTime,
                TotalCost = materialCost.TotalCost,
                CostPerUnit = materialCost.CostPerUnit,
                LoadingUnloadingCost = materialCost.LoadingUnloadingCost,
                TotalCostPerUnit = materialCost.TotalCostPerUnit,


                CreatedBy = materialCost.CreatedBy,
                DateCreated = materialCost.DateCreated,
                ModifiedBy = materialCost.ModifiedBy,
                LastModified = materialCost.LastModified
            });

            return Json(result);
        }
        private bool MaterialTransportationCostExists(string itemCode, int branchId)
        {
            var materialCost = db.MaterialTransportationCost.FirstOrDefault(x => x.ItemCode == itemCode && x.BranchId == branchId);
            if (materialCost != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransportCost_Create([DataSourceRequest]DataSourceRequest request, MaterialTransportationCost materialCost)
        {
            if (ModelState.IsValid)
            {
                if (MaterialTransportationCostExists(materialCost.ItemCode, materialCost.BranchId))
                {
                    ModelState.AddModelError("", "Transportaion cost for this Material exists already!!");
                }
                //else if (!DateIsInPeriodRange(materialCost.DateOfInformation, materialCost.GLPeriodId))
                //{
                //    ModelState.AddModelError("", "Date must be within the period Range!!!");
                //}
                else
                {
                    var entity = new MaterialTransportationCost
                    {
                        MaterialTransportationCostId = materialCost.MaterialTransportationCostId,
                        BranchId = materialCost.BranchId,
                        UoM = GetUom(materialCost.ItemCode),
                        TrackCapacityPerTrip = materialCost.TrackCapacityPerTrip,
                        AverageSpeedOfTrack = materialCost.AverageSpeedOfTrack,
                        ItemCategoryCode = materialCost.ItemCategoryCode,
                        ItemCode = materialCost.ItemCode,
                        RentalCostWithFuel = materialCost.RentalCostWithFuel,
                        AverageDistanceFromSourceToSite = materialCost.AverageDistanceFromSourceToSite,
                        DriverPerdiem = materialCost.DriverPerdiem,
                        AssistantDriverPerdiem = materialCost.AssistantDriverPerdiem,
                        RequiredTime = materialCost.RequiredTime,
                        TotalCost = materialCost.TotalCost,
                        CostPerUnit = materialCost.CostPerUnit,
                        LoadingUnloadingCost = materialCost.LoadingUnloadingCost,
                        TotalCostPerUnit = materialCost.TotalCostPerUnit,

                        CreatedBy = materialCost.CreatedBy,
                        DateCreated = materialCost.DateCreated,
                        ModifiedBy = materialCost.ModifiedBy,
                        LastModified = materialCost.LastModified
                    };
                    entity.RequiredTime = entity.AverageSpeedOfTrack > 0 ? entity.AverageDistanceFromSourceToSite / entity.AverageSpeedOfTrack : 0;
                    var totalRentalCost = entity.RequiredTime * entity.RentalCostWithFuel * 2;
                    var totalPerdiemCost = (entity.DriverPerdiem * entity.RequiredTime * 2) / 8 + (entity.AssistantDriverPerdiem * entity.RequiredTime * 2) / 8;
                    entity.TotalCost = totalRentalCost + totalPerdiemCost;
                    entity.CostPerUnit = entity.TotalCost / entity.TrackCapacityPerTrip;
                    entity.TotalCostPerUnit = entity.CostPerUnit + entity.LoadingUnloadingCost;

                    //entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost + entity.LoadingUnloading;
                    //entity.ApprovedCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost + entity.LoadingUnloading;

                    db.MaterialTransportationCost.Add(entity);
                    db.SaveChanges();
                    materialCost.MaterialTransportationCostId = entity.MaterialTransportationCostId;

                }
            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransportCost_Update([DataSourceRequest]DataSourceRequest request, MaterialTransportationCost materialCost)
        {
            if (ModelState.IsValid)
            {
                //if (!DateIsInPeriodRange(materialCost.DateOfInformation, materialCost.GLPeriodId))
                //{
                //    ModelState.AddModelError("", "Date must be within the period Range!!!");
                //}
                //else
                //{
                var entity = new MaterialTransportationCost
                {
                    MaterialTransportationCostId = materialCost.MaterialTransportationCostId,
                    BranchId = materialCost.BranchId,
                    UoM = GetUom(materialCost.ItemCode),
                    TrackCapacityPerTrip = materialCost.TrackCapacityPerTrip,
                    AverageSpeedOfTrack = materialCost.AverageSpeedOfTrack,
                    ItemCategoryCode = materialCost.ItemCategoryCode,
                    ItemCode = materialCost.ItemCode,
                    RentalCostWithFuel = materialCost.RentalCostWithFuel,
                    DriverPerdiem = materialCost.DriverPerdiem,
                    AssistantDriverPerdiem = materialCost.AssistantDriverPerdiem,
                    AverageDistanceFromSourceToSite = materialCost.AverageDistanceFromSourceToSite,
                    RequiredTime = materialCost.RequiredTime,
                    TotalCost = materialCost.TotalCost,
                    CostPerUnit = materialCost.CostPerUnit,
                    LoadingUnloadingCost = materialCost.LoadingUnloadingCost,
                    TotalCostPerUnit = materialCost.TotalCostPerUnit,

                    CreatedBy = materialCost.CreatedBy,
                    DateCreated = materialCost.DateCreated,
                    ModifiedBy = materialCost.ModifiedBy,
                    LastModified = materialCost.LastModified
                };
                entity.RequiredTime = entity.AverageSpeedOfTrack > 0 ? entity.AverageDistanceFromSourceToSite / entity.AverageSpeedOfTrack : 0;
                var totalRentalCost = entity.RequiredTime * entity.RentalCostWithFuel * 2;
                var totalPerdiemCost = (entity.DriverPerdiem * entity.RequiredTime * 2) / 8 + (entity.AssistantDriverPerdiem * entity.RequiredTime * 2) / 8;
                entity.TotalCost = totalRentalCost + totalPerdiemCost;
                entity.CostPerUnit = entity.TotalCost / entity.TrackCapacityPerTrip;
                entity.TotalCostPerUnit = entity.CostPerUnit + entity.LoadingUnloadingCost;

                db.MaterialTransportationCost.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //}
            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransportCost_Destroy([DataSourceRequest]DataSourceRequest request, MaterialTransportationCost materialCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialTransportationCost
                {
                    MaterialTransportationCostId = materialCost.MaterialTransportationCostId,
                    BranchId = materialCost.BranchId,
                    UoM = materialCost.UoM,
                    TrackCapacityPerTrip = materialCost.TrackCapacityPerTrip,
                    AverageSpeedOfTrack = materialCost.AverageSpeedOfTrack,
                    ItemCategoryCode = materialCost.ItemCategoryCode,
                    ItemCode = materialCost.ItemCode,
                    RentalCostWithFuel = materialCost.RentalCostWithFuel,
                    AverageDistanceFromSourceToSite = materialCost.AverageDistanceFromSourceToSite,
                    DriverPerdiem = materialCost.DriverPerdiem,
                    AssistantDriverPerdiem = materialCost.AssistantDriverPerdiem,
                    RequiredTime = materialCost.RequiredTime,
                    TotalCost = materialCost.TotalCost,
                    CostPerUnit = materialCost.CostPerUnit,
                    LoadingUnloadingCost = materialCost.LoadingUnloadingCost,
                    TotalCostPerUnit = materialCost.TotalCostPerUnit,

                    CreatedBy = materialCost.CreatedBy,
                    DateCreated = materialCost.DateCreated,
                    ModifiedBy = materialCost.ModifiedBy,
                    LastModified = materialCost.LastModified
                };


                db.MaterialTransportationCost.Attach(entity);
                db.MaterialTransportationCost.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
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

        public decimal GetMaterialTransportaionCost(string itemCode) {
            return ConstructionInputsHelper.GetMateriaTrasportCost(itemCode);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}