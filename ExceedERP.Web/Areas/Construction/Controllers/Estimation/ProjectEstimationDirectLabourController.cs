using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationDirectLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationDirectLabour_Read([DataSourceRequest]DataSourceRequest request, int idthree)
        {
            ExceedDbContext dbc = new ExceedDbContext();
            dbc = new ExceedDbContext();
            IQueryable<ProjectEstimationDirectLabour> projectestimationdirectlabour = dbc.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == idthree).AsQueryable();
            DataSourceResult result = projectestimationdirectlabour.ToDataSourceResult(request, projectEstimationDirectLabour => new
            {
                ProjectEstimationDirectLabourId = projectEstimationDirectLabour.ProjectEstimationDirectLabourId,
                IndexedHourlySalary = projectEstimationDirectLabour.IndexedHourlySalary,
                ProjectEstimationIndexLabourId = projectEstimationDirectLabour.ProjectEstimationIndexLabourId,
                Quantity = projectEstimationDirectLabour.Quantity,
                UnitFactory = projectEstimationDirectLabour.UnitFactory,
                DailyPay = projectEstimationDirectLabour.DailyPay,
                TotalCost = projectEstimationDirectLabour.TotalCost,
                ProjectEstimationWorkItemId = projectEstimationDirectLabour.ProjectEstimationWorkItemId,
                ManpowerUnitCost = projectEstimationDirectLabour.ManpowerUnitCost,
                ProjectEstimationIndexLabour = projectEstimationDirectLabour.ProjectEstimationIndexLabour,
                OrgBranchName = projectEstimationDirectLabour.OrgBranchName,
                DateCreated = projectEstimationDirectLabour.DateCreated,
                LastModified = projectEstimationDirectLabour.LastModified,
                CreatedBy = projectEstimationDirectLabour.CreatedBy,
                ModifiedBy = projectEstimationDirectLabour.ModifiedBy
            });

            return Json(result);
        }
        private bool LabourExists(int workItemId, int estimationIndexLabourId )
        {
            bool laborExists = false;
            var temp = db.ProjectEstimationDirectLabour.Where(m => m.ProjectEstimationWorkItemId == workItemId && m.ProjectEstimationIndexLabourId == estimationIndexLabourId).FirstOrDefault();
            if (temp != null)
            {
                laborExists = true;
            }
            return laborExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationDirectLabour_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationDirectLabour projectEstimationDirectLabour, int idfour)
        {
            if (ModelState.IsValid)
            {
                if(LabourExists(idfour, projectEstimationDirectLabour.ProjectEstimationIndexLabourId))
                {
                    ModelState.AddModelError("", "This Labor exists already!");
                }
                else
                {
                    var entity = new ProjectEstimationDirectLabour
                    {

                        ProjectEstimationIndexLabourId = projectEstimationDirectLabour.ProjectEstimationIndexLabourId,
                        Quantity = projectEstimationDirectLabour.Quantity,
                        UnitFactory = projectEstimationDirectLabour.UnitFactory,
                        DailyPay = GetDailyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                        IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                        TotalCost = projectEstimationDirectLabour.Quantity * projectEstimationDirectLabour.UnitFactory * this.GetIndexedHourlyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                        ProjectEstimationWorkItemId = idfour,
                        ManpowerUnitCost = GetManPowerUnitCost(idfour),
                        OrgBranchName = projectEstimationDirectLabour.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationDirectLabour.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationDirectLabour.ModifiedBy
                    };
                    //entity.TotalCost = entity.Quantity * entity.UnitFactory *
                    //                   entity.IndexedHourlySalary;
                    //db = new ExceedDbContext();
                    db.ProjectEstimationDirectLabour.Add(entity);
                    db.SaveChanges();

                    projectEstimationDirectLabour.IndexedHourlySalary = entity.IndexedHourlySalary;
                    projectEstimationDirectLabour.ProjectEstimationDirectLabourId = entity.ProjectEstimationDirectLabourId;

                    ProjectEstimationWorkItemController.UpdateWorkItemCost(idfour);
                    ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(idfour);
                }


            }

            return Json(new[] { projectEstimationDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetDailyPayPerJobTitle(int id)
        {
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.IndexedHourlySalary;
            }
            return 0;
        }

        private decimal GetIndexedHourlyPayPerJobTitle(int id)
        {
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.ApprovedIndexedHourlySalary;
            }
            return 0;
        }
        private decimal GetManPowerUnitCost(int id)
        {
            decimal labourCost = 0;
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                if (workItem.LabourHourlyOutPut > 0)
                {
                    labourCost = labourCost / workItem.LabourHourlyOutPut;
                }
                else
                {
                    labourCost = labourCost / 1;
                }
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationDirectLabour_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationDirectLabour projectEstimationDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationDirectLabour
                {
                    ProjectEstimationDirectLabourId = projectEstimationDirectLabour.ProjectEstimationDirectLabourId,

                    ProjectEstimationIndexLabourId = projectEstimationDirectLabour.ProjectEstimationIndexLabourId,
                    Quantity = projectEstimationDirectLabour.Quantity,
                    UnitFactory = projectEstimationDirectLabour.UnitFactory,
                    DailyPay = GetDailyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                    IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                    //TotalCost = projectEstimationDirectLabour.Quantity * projectEstimationDirectLabour.UnitFactory * this.GetIndexedHourlyPayPerJobTitle(projectEstimationDirectLabour.ProjectEstimationIndexLabourId),
                    ProjectEstimationWorkItemId = projectEstimationDirectLabour.ProjectEstimationWorkItemId,
                    ManpowerUnitCost = GetManPowerUnitCost(projectEstimationDirectLabour.ProjectEstimationWorkItemId),
                    OrgBranchName = projectEstimationDirectLabour.OrgBranchName,
                    DateCreated = projectEstimationDirectLabour.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEstimationDirectLabour.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                 entity.IndexedHourlySalary;
                //db = new ExceedDbContext();

                var local = db.Set<ProjectEstimationDirectLabour>()
                         .Local
                         .FirstOrDefault(f => f.ProjectEstimationDirectLabourId == projectEstimationDirectLabour.ProjectEstimationDirectLabourId);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(entity).State = EntityState.Modified;

                //db.ProjectEstimationDirectLabour.Attach(entity);
                //db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                projectEstimationDirectLabour.IndexedHourlySalary = entity.IndexedHourlySalary;

                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationDirectLabour.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationDirectLabour.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationDirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationDirectLabour projectEstimationDirectLabour)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationDirectLabour
                {
                    ProjectEstimationDirectLabourId = projectEstimationDirectLabour.ProjectEstimationDirectLabourId,
                    IndexedHourlySalary = projectEstimationDirectLabour.IndexedHourlySalary,

                    ProjectEstimationIndexLabourId = projectEstimationDirectLabour.ProjectEstimationIndexLabourId,
                    Quantity = projectEstimationDirectLabour.Quantity,
                    UnitFactory = projectEstimationDirectLabour.UnitFactory,
                    DailyPay = projectEstimationDirectLabour.DailyPay,
                    TotalCost = projectEstimationDirectLabour.TotalCost,
                    ProjectEstimationWorkItemId = projectEstimationDirectLabour.ProjectEstimationWorkItemId,
                    ManpowerUnitCost = projectEstimationDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectEstimationDirectLabour.OrgBranchName,
                    DateCreated = projectEstimationDirectLabour.DateCreated,
                    LastModified = projectEstimationDirectLabour.LastModified,
                    CreatedBy = projectEstimationDirectLabour.CreatedBy,
                    ModifiedBy = projectEstimationDirectLabour.ModifiedBy
                };

                db.ProjectEstimationDirectLabour.Attach(entity);
                db.ProjectEstimationDirectLabour.Remove(entity);
                db.SaveChanges();

                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationDirectLabour.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationDirectLabour.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
