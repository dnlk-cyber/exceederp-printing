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
    public class ResourceEstimationLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult ResourceEstimation_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            ExceedDbContext dbc = new ExceedDbContext();
            dbc = new ExceedDbContext();
            IQueryable<ResourceEstimationLabour> labors = dbc.ResourceEstimationLabour.Where(x => x.WorkItemDurationId == itemId).AsQueryable();
            DataSourceResult result = labors.ToDataSourceResult(request, labor => new
            {
                ResourceEstimationLabourId = labor.ResourceEstimationLabourId,
                WorkItemDurationId = labor.WorkItemDurationId,
                IndexedHourlySalary = labor.IndexedHourlySalary,
                ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                Number = labor.Number,
                UnitFactor = labor.UnitFactor,
                UnitFactorAndNumberPerCrew = labor.UnitFactorAndNumberPerCrew,
                NumberPerDay = labor.NumberPerDay,
                TotalCostPerDay = labor.TotalCostPerDay,

                OrgBranchName = labor.OrgBranchName,
                DateCreated = labor.DateCreated,
                LastModified = labor.LastModified,
                CreatedBy = labor.CreatedBy,
                ModifiedBy = labor.ModifiedBy
            });

            return Json(result);
        }
        private bool LabourExists(int workItemId, ResourceEstimationLabour labor )
        {
            return db.ResourceEstimationLabour
                .FirstOrDefault(m => m.WorkItemDurationId == workItemId &&
                                m.ProjectEstimationIndexLabourId == labor.ProjectEstimationIndexLabourId &&
                                m.ResourceEstimationLabourId != labor.ResourceEstimationLabourId)
                                != null;
           
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Create([DataSourceRequest]DataSourceRequest request, ResourceEstimationLabour labor, int itemId)
        {
            if (ModelState.IsValid)
            {
                if(LabourExists(itemId, labor))
                {
                    ModelState.AddModelError("", "This Labor exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationLabour
                    {
                        ResourceEstimationLabourId = labor.ResourceEstimationLabourId,
                        ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                        Number = labor.Number,
                        UnitFactor = labor.UnitFactor,
                        WorkItemDurationId = itemId,
                        UnitFactorAndNumberPerCrew = labor.UnitFactorAndNumberPerCrew,
                        NumberPerDay = labor.NumberPerDay,
                        TotalCostPerDay = labor.TotalCostPerDay,
                        IndexedHourlySalary = ProjectEstimationHelper.GetIndexedHourlyPayPerJobTitle(labor.ProjectEstimationIndexLabourId),
                        
                        OrgBranchName = labor.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = labor.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = labor.ModifiedBy
                    };
                    entity.UnitFactorAndNumberPerCrew = entity.UnitFactor * entity.Number;
                    var parentItem = db.WorkItemDuration.Find(itemId);
                    if(parentItem != null)
                    {
                        entity.NumberPerDay = parentItem.NumberOfCrew > 0 ? parentItem.NumberOfCrew * entity.UnitFactorAndNumberPerCrew : entity.UnitFactorAndNumberPerCrew;
                    }
                    entity.TotalCostPerDay = 8 * entity.IndexedHourlySalary * entity.NumberPerDay;

                    db.ResourceEstimationLabour.Add(entity);
                    db.SaveChanges();

                    labor.IndexedHourlySalary = entity.IndexedHourlySalary;
                    labor.ResourceEstimationLabourId = entity.ResourceEstimationLabourId;
                }
            }

            return Json(new[] { labor }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Update([DataSourceRequest]DataSourceRequest request, ResourceEstimationLabour labor)
        {
            if (ModelState.IsValid)
            {
                if (LabourExists(labor.WorkItemDurationId, labor))
                {
                    ModelState.AddModelError("", "This Labor exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationLabour
                    {
                        ResourceEstimationLabourId = labor.ResourceEstimationLabourId,
                        ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                        Number = labor.Number,
                        UnitFactor = labor.UnitFactor,
                        WorkItemDurationId = labor.WorkItemDurationId,
                        UnitFactorAndNumberPerCrew = labor.UnitFactorAndNumberPerCrew,
                        NumberPerDay = labor.NumberPerDay,
                        TotalCostPerDay = labor.TotalCostPerDay,
                        IndexedHourlySalary = ProjectEstimationHelper.GetIndexedHourlyPayPerJobTitle(labor.ProjectEstimationIndexLabourId),

                        OrgBranchName = labor.OrgBranchName,
                        DateCreated = labor.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = labor.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    entity.UnitFactorAndNumberPerCrew = entity.UnitFactor * entity.Number;
                    var parentItem = db.WorkItemDuration.Find(entity.WorkItemDurationId);
                    if (parentItem != null)
                    {
                        entity.NumberPerDay = parentItem.NumberOfCrew > 0 ? parentItem.NumberOfCrew * entity.UnitFactorAndNumberPerCrew : entity.UnitFactorAndNumberPerCrew;
                    }
                    entity.TotalCostPerDay = 8 * entity.IndexedHourlySalary * entity.NumberPerDay;

                    db.ResourceEstimationLabour.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { labor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Destroy([DataSourceRequest]DataSourceRequest request, ResourceEstimationLabour labor)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ResourceEstimationLabour
                {
                    ResourceEstimationLabourId = labor.ResourceEstimationLabourId,
                    WorkItemDurationId = labor.WorkItemDurationId,
                    IndexedHourlySalary = labor.IndexedHourlySalary,
                    ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                    Number = labor.Number,
                    UnitFactor = labor.UnitFactor,
                    UnitFactorAndNumberPerCrew = labor.UnitFactorAndNumberPerCrew,
                    NumberPerDay = labor.NumberPerDay,
                    TotalCostPerDay = labor.TotalCostPerDay,
                    
                    OrgBranchName = labor.OrgBranchName,
                    DateCreated = labor.DateCreated,
                    LastModified = labor.LastModified,
                    CreatedBy = labor.CreatedBy,
                    ModifiedBy = labor.ModifiedBy
                };

                db.ResourceEstimationLabour.Attach(entity);
                db.ResourceEstimationLabour.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { labor }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
