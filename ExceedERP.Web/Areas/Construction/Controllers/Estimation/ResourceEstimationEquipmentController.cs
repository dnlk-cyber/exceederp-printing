using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ResourceEstimationEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult ResourceEstimation_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            ExceedDbContext dbc = new ExceedDbContext();
            dbc = new ExceedDbContext();
            IQueryable<ResourceEstimationEquipment> labors = dbc.ResourceEstimationEquipments.Where(x => x.WorkItemDurationId == itemId).AsQueryable();
            DataSourceResult result = labors.ToDataSourceResult(request, equipment => new
            {
                ResourceEstimationEquipmentId = equipment.ResourceEstimationEquipmentId,
                WorkItemDurationId = equipment.WorkItemDurationId,
                HourlyCost = equipment.HourlyCost,
                FleetsID = equipment.FleetsID,
                Number = equipment.Number,
                UnitFactor = equipment.UnitFactor,
                UnitFactorAndNumberPerCrew = equipment.UnitFactorAndNumberPerCrew,
                NumberPerDay = equipment.NumberPerDay,
                TotalCostPerDay = equipment.TotalCostPerDay,
                
                OrgBranchName = equipment.OrgBranchName,
                DateCreated = equipment.DateCreated,
                LastModified = equipment.LastModified,
                CreatedBy = equipment.CreatedBy,
                ModifiedBy = equipment.ModifiedBy
            });

            return Json(result);
        }
        private bool EquipmentExists(int workItemId, ResourceEstimationEquipment equipment )
        {
            return db.ResourceEstimationEquipments
                .FirstOrDefault(m => m.WorkItemDurationId == workItemId &&
                                m.FleetsID == equipment.FleetsID &&
                                m.ResourceEstimationEquipmentId != equipment.ResourceEstimationEquipmentId)
                                != null;
           
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Create([DataSourceRequest]DataSourceRequest request, ResourceEstimationEquipment equipment, int itemId)
        {
            if (ModelState.IsValid)
            {
                if(EquipmentExists(itemId, equipment))
                {
                    ModelState.AddModelError("", "This Equipment exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationEquipment
                    {
                        ResourceEstimationEquipmentId = equipment.ResourceEstimationEquipmentId,
                        FleetsID = equipment.FleetsID,
                        Number = equipment.Number,
                        UnitFactor = equipment.UnitFactor,
                        WorkItemDurationId = itemId,
                        UnitFactorAndNumberPerCrew = equipment.UnitFactorAndNumberPerCrew,
                        NumberPerDay = equipment.NumberPerDay,
                        TotalCostPerDay = equipment.TotalCostPerDay,
                        HourlyCost = ProjectEstimationHelper.GetEquipmentRentalRate(equipment.FleetsID),
                        
                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = equipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = equipment.ModifiedBy
                    };
                    entity.UnitFactorAndNumberPerCrew = entity.UnitFactor * entity.Number;
                    var parentItem = db.WorkItemDuration.Find(itemId);
                    if(parentItem != null)
                    {
                        entity.NumberPerDay = parentItem.NumberOfCrew > 0 ? parentItem.NumberOfCrew * entity.UnitFactorAndNumberPerCrew : entity.UnitFactorAndNumberPerCrew;
                    }
                    entity.TotalCostPerDay = 8 * entity.HourlyCost * entity.NumberPerDay;

                    db.ResourceEstimationEquipments.Add(entity);
                    db.SaveChanges();

                    equipment.HourlyCost = entity.HourlyCost;
                    equipment.ResourceEstimationEquipmentId = entity.ResourceEstimationEquipmentId;
                }
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Update([DataSourceRequest]DataSourceRequest request, ResourceEstimationEquipment equipment)
        {
            if (ModelState.IsValid)
            {
                if (EquipmentExists(equipment.WorkItemDurationId, equipment))
                {
                    ModelState.AddModelError("", "This Equipment exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationEquipment
                    {
                        ResourceEstimationEquipmentId = equipment.ResourceEstimationEquipmentId,
                        FleetsID = equipment.FleetsID,
                        Number = equipment.Number,
                        UnitFactor = equipment.UnitFactor,
                        WorkItemDurationId = equipment.WorkItemDurationId,
                        UnitFactorAndNumberPerCrew = equipment.UnitFactorAndNumberPerCrew,
                        NumberPerDay = equipment.NumberPerDay,
                        TotalCostPerDay = equipment.TotalCostPerDay,
                        HourlyCost = ProjectEstimationHelper.GetEquipmentRentalRate(equipment.FleetsID),

                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = equipment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = equipment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    entity.UnitFactorAndNumberPerCrew = entity.UnitFactor * entity.Number;
                    var parentItem = db.WorkItemDuration.Find(entity.WorkItemDurationId);
                    if (parentItem != null)
                    {
                        entity.NumberPerDay = parentItem.NumberOfCrew > 0 ? parentItem.NumberOfCrew * entity.UnitFactorAndNumberPerCrew : entity.UnitFactorAndNumberPerCrew;
                    }
                    entity.TotalCostPerDay = 8 * entity.HourlyCost * entity.NumberPerDay;

                    db.ResourceEstimationEquipments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Destroy([DataSourceRequest]DataSourceRequest request, ResourceEstimationEquipment equipment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ResourceEstimationEquipment
                {
                    ResourceEstimationEquipmentId = equipment.ResourceEstimationEquipmentId,
                    WorkItemDurationId = equipment.WorkItemDurationId,
                    HourlyCost = equipment.HourlyCost,
                    FleetsID = equipment.FleetsID,
                    Number = equipment.Number,
                    UnitFactor = equipment.UnitFactor,
                    UnitFactorAndNumberPerCrew = equipment.UnitFactorAndNumberPerCrew,
                    NumberPerDay = equipment.NumberPerDay,
                    TotalCostPerDay = equipment.TotalCostPerDay,

                    OrgBranchName = equipment.OrgBranchName,
                    DateCreated = equipment.DateCreated,
                    LastModified = equipment.LastModified,
                    CreatedBy = equipment.CreatedBy,
                    ModifiedBy = equipment.ModifiedBy
                };

                db.ResourceEstimationEquipments.Attach(entity);
                db.ResourceEstimationEquipments.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
