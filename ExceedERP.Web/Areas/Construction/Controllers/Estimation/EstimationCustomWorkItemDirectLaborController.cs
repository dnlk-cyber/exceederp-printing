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

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationCustomWorkItemDirectLaborController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult DirectLabour_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            ExceedDbContext dbc = new ExceedDbContext();
            IQueryable<EstimationCustomWorkItemDirectLabor> directlabours = dbc.EstimationCustomWorkItemDirectLabor.Where(x => x.ProjectEstimationCustomWorkItemId == workItemId).AsQueryable();
            DataSourceResult result = directlabours.ToDataSourceResult(request, directlabour => new
            {
                EstimationCustomWorkItemDirectLaborId = directlabour.EstimationCustomWorkItemDirectLaborId,
                IndexedHourlySalary = directlabour.IndexedHourlySalary,
                ProjectEstimationIndexLabourId = directlabour.ProjectEstimationIndexLabourId,
                Quantity = directlabour.Quantity,
                UnitFactory = directlabour.UnitFactory,
                DailyPay = directlabour.DailyPay,
                TotalCost = directlabour.TotalCost,
                ProjectEstimationCustomWorkItemId = directlabour.ProjectEstimationCustomWorkItemId,
                ManpowerUnitCost = directlabour.ManpowerUnitCost,
                ProjectEstimationIndexLabour = directlabour.ProjectEstimationIndexLabour,
                OrgBranchName = directlabour.OrgBranchName,
                DateCreated = directlabour.DateCreated,
                LastModified = directlabour.LastModified,
                CreatedBy = directlabour.CreatedBy,
                ModifiedBy = directlabour.ModifiedBy
            });

            return Json(result);
        }
        private bool LabourExists(int workItemId, int estimationIndexLabourId)
        {
            bool laborExists = false;
            var temp = db.EstimationCustomWorkItemDirectLabor.FirstOrDefault(m => m.ProjectEstimationCustomWorkItemId == workItemId && m.ProjectEstimationIndexLabourId == estimationIndexLabourId);
            if (temp != null)
            {
                laborExists = true;
            }
            return laborExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLabour_Create([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemDirectLabor directlabour, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (LabourExists(workItemId, directlabour.ProjectEstimationIndexLabourId))
                {
                    ModelState.AddModelError("", "This Labor exists already!");
                }
                else
                {
                    var entity = new EstimationCustomWorkItemDirectLabor
                    {

                        ProjectEstimationIndexLabourId = directlabour.ProjectEstimationIndexLabourId,
                        Quantity = directlabour.Quantity,
                        UnitFactory = directlabour.UnitFactory,
                        DailyPay = GetDailyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                        IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                        TotalCost = directlabour.Quantity * directlabour.UnitFactory * this.GetIndexedHourlyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                        ProjectEstimationCustomWorkItemId = workItemId,
                        //ManpowerUnitCost = GetManPowerUnitCost(idfour),

                        OrgBranchName = directlabour.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = directlabour.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = directlabour.ModifiedBy
                    };

                    db.EstimationCustomWorkItemDirectLabor.Add(entity);
                    db.SaveChanges();

                    directlabour.IndexedHourlySalary = entity.IndexedHourlySalary;
                    directlabour.EstimationCustomWorkItemDirectLaborId = entity.EstimationCustomWorkItemDirectLaborId;
                    ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(workItemId);
                }
            }

            return Json(new[] { directlabour }.ToDataSourceResult(request, ModelState));
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
        //private decimal GetManPowerUnitCost(int id)
        //{
        //    decimal labourCost = 0;
        //    var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

        //    if (workItem != null)
        //    {
        //        var labouCosts =
        //            db.ProjectEstimationDirectLabour.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
        //        if (labouCosts.Any())
        //        {

        //            foreach (var m in labouCosts)
        //            {
        //                labourCost += m.TotalCost;
        //            }
        //        }
        //        if (workItem.LabourHourlyOutPut > 0)
        //        {
        //            labourCost = labourCost / workItem.LabourHourlyOutPut;
        //        }
        //        else
        //        {
        //            labourCost = labourCost / 1;
        //        }
        //    }
        //    totalMaterialCost = labourCost;
        //    return labourCost;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLabour_Update([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemDirectLabor directlabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemDirectLabor
                {
                    EstimationCustomWorkItemDirectLaborId = directlabour.EstimationCustomWorkItemDirectLaborId,

                    ProjectEstimationIndexLabourId = directlabour.ProjectEstimationIndexLabourId,
                    Quantity = directlabour.Quantity,
                    UnitFactory = directlabour.UnitFactory,
                    DailyPay = GetDailyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                    IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                    //TotalCost = directlabour.Quantity * directlabour.UnitFactory * this.GetIndexedHourlyPayPerJobTitle(directlabour.ProjectEstimationIndexLabourId),
                    ProjectEstimationCustomWorkItemId = directlabour.ProjectEstimationCustomWorkItemId,
                    //ManpowerUnitCost = GetManPowerUnitCost(directlabour.ProjectEstimationWorkItemId),

                    OrgBranchName = directlabour.OrgBranchName,
                    DateCreated = directlabour.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = directlabour.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                 entity.IndexedHourlySalary;

                //var local = db.Set<EstimationCustomWorkItemDirectLabor>()
                //         .Local
                //         .FirstOrDefault(f => f.EstimationCustomWorkItemDirectLaborId == directlabour.EstimationCustomWorkItemDirectLaborId);
                //if (local != null)
                //{
                //    db.Entry(local).State = EntityState.Detached;
                //}
                //db.Entry(entity).State = EntityState.Modified;

                db.EstimationCustomWorkItemDirectLabor.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                directlabour.IndexedHourlySalary = entity.IndexedHourlySalary;

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { directlabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemDirectLabor directlabour)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemDirectLabor
                {
                    EstimationCustomWorkItemDirectLaborId = directlabour.EstimationCustomWorkItemDirectLaborId,
                    IndexedHourlySalary = directlabour.IndexedHourlySalary,

                    ProjectEstimationIndexLabourId = directlabour.ProjectEstimationIndexLabourId,
                    Quantity = directlabour.Quantity,
                    UnitFactory = directlabour.UnitFactory,
                    DailyPay = directlabour.DailyPay,
                    TotalCost = directlabour.TotalCost,
                    ProjectEstimationCustomWorkItemId = directlabour.ProjectEstimationCustomWorkItemId,
                    //ManpowerUnitCost = directlabour.ManpowerUnitCost,
                    OrgBranchName = directlabour.OrgBranchName,
                    DateCreated = directlabour.DateCreated,
                    LastModified = directlabour.LastModified,
                    CreatedBy = directlabour.CreatedBy,
                    ModifiedBy = directlabour.ModifiedBy
                };

                db.EstimationCustomWorkItemDirectLabor.Attach(entity);
                db.EstimationCustomWorkItemDirectLabor.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { directlabour }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}