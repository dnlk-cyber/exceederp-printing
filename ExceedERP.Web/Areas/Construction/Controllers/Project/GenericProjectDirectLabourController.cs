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

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class GenericProjectDirectLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectDirectLabour_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<GenericProjectDirectLabour> projectDirectlabours = db.GenericProjectDirectLabour.Where(x => x.GenericWorkItemPerProjectId == workItemId);
            DataSourceResult result = projectDirectlabours.ToDataSourceResult(request, projectDirectLabour => new {
                GenericProjectDirectLabourId = projectDirectLabour.GenericProjectDirectLabourId,
                Unit = projectDirectLabour.Unit,
                Quantity = projectDirectLabour.Quantity,
                UnitFactory = projectDirectLabour.UnitFactory,
                IndexedHourlySalary = projectDirectLabour.IndexedHourlySalary,
                DailyPay = projectDirectLabour.DailyPay,
                TotalCost = projectDirectLabour.TotalCost,
                ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                GenericWorkItemPerProjectId = projectDirectLabour.GenericWorkItemPerProjectId,
                ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId,

                OrgBranchName = projectDirectLabour.OrgBranchName,
                DateCreated = projectDirectLabour.DateCreated,
                LastModified = projectDirectLabour.LastModified,
                CreatedBy = projectDirectLabour.CreatedBy,
                ModifiedBy = projectDirectLabour.ModifiedBy,
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Create([DataSourceRequest]DataSourceRequest request, GenericProjectDirectLabour projectDirectLabour, int workItemId)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectDirectLabour
                {
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    //IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    IndexedHourlySalary = projectDirectLabour.IndexedHourlySalary,
                    DailyPay = GetDailyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    TotalCost = projectDirectLabour.Quantity * projectDirectLabour.UnitFactory * projectDirectLabour.IndexedHourlySalary,
                    //ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost
                    GenericWorkItemPerProjectId = workItemId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId,

                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectDirectLabour.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectDirectLabour.ModifiedBy,
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                 entity.IndexedHourlySalary;
                db.GenericProjectDirectLabour.Add(entity);
                db.SaveChanges();
                projectDirectLabour.GenericWorkItemPerProjectId = entity.GenericProjectDirectLabourId;
                //ProjectListController.UpdateProjectTotalCost(idfour);
                ProjectEstimationHelper.UpdateGenericWorkItem(workItemId);

            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Update([DataSourceRequest]DataSourceRequest request, GenericProjectDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectDirectLabour
                {
                    GenericProjectDirectLabourId = projectDirectLabour.GenericProjectDirectLabourId,
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    //IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    IndexedHourlySalary = projectDirectLabour.IndexedHourlySalary,
                    DailyPay = GetDailyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    TotalCost = projectDirectLabour.Quantity * projectDirectLabour.UnitFactory * projectDirectLabour.IndexedHourlySalary,
                    ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = projectDirectLabour.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectDirectLabour.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    GenericWorkItemPerProjectId = projectDirectLabour.GenericWorkItemPerProjectId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                entity.IndexedHourlySalary;
                db.GenericProjectDirectLabour.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, GenericProjectDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectDirectLabour
                {
                    GenericProjectDirectLabourId = projectDirectLabour.GenericProjectDirectLabourId,
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    IndexedHourlySalary = projectDirectLabour.IndexedHourlySalary,
                    DailyPay = projectDirectLabour.DailyPay,
                    TotalCost = projectDirectLabour.TotalCost,
                    ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = projectDirectLabour.DateCreated,
                    LastModified = projectDirectLabour.LastModified,
                    CreatedBy = projectDirectLabour.CreatedBy,
                    ModifiedBy = projectDirectLabour.ModifiedBy,
                    GenericWorkItemPerProjectId = projectDirectLabour.GenericWorkItemPerProjectId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };

                db.GenericProjectDirectLabour.Attach(entity);
                db.GenericProjectDirectLabour.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }
        #region Methods
        
        public decimal GetDailyPayPerJobTitle(int id)
        {
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.NetBirrHour;
            }
            return 0;
        }
        //private decimal GetManPowerUnitCost(int id)
        //{
        //    decimal labourCost = 0;
        //    totalMaterialCost = 0;
        //    var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

        //    if (workItem != null)
        //    {
        //        var labouCosts =
        //            db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == id).ToList();
        //        if (labouCosts.Any())
        //        {


        //            labourCost = labouCosts.Sum(x => x.TotalCost);

        //        }
        //        labourCost = labourCost / workItem.LabourHourlyOutPut;
        //    }
        //    totalMaterialCost = labourCost;
        //    return labourCost;
        //}

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}