using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ResourceUtilizationDirectLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult UtilizationDirectLabour_Read([DataSourceRequest]DataSourceRequest request, int idthree)
        {
            IQueryable<ResourceUtilizationDirectLabour> projectdirectlabour = db.ResourceUtilizationDirectLabours.Where(x => x.ResourceUtilizationWorkItemId == idthree);
            DataSourceResult result = projectdirectlabour.ToDataSourceResult(request, projectDirectLabour => new {
                ResourceUtilizationDirectLabourId = projectDirectLabour.ResourceUtilizationDirectLabourId,
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
                ResourceUtilizationWorkItemId = projectDirectLabour.ResourceUtilizationWorkItemId,
                ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
            });

            return Json(result);
        }
        private bool LaborExists(ResourceUtilizationDirectLabour labour, int utilizationWorkItemId)
        {
            bool laborExists = false;
            var labor = db.ResourceUtilizationDirectLabours.FirstOrDefault(
                e => e.ResourceUtilizationWorkItemId == utilizationWorkItemId && e.ProjectEstimationIndexLabourId == labour.ProjectEstimationIndexLabourId);
            if (labor != null)
            {
                laborExists = true;
            }
            return laborExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationDirectLabour_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDirectLabour projectDirectLabour, int idfour)
        {
            if (ModelState.IsValid)
            {
                if (LaborExists(projectDirectLabour, idfour))
                {
                    ModelState.AddModelError("", "Labor Registered already!");
                }
                else
                {
                    var entity = new ResourceUtilizationDirectLabour
                    {
                        Unit = projectDirectLabour.Unit,
                        Quantity = projectDirectLabour.Quantity,
                        UnitFactory = projectDirectLabour.UnitFactory,
                        IndexedHourlySalary = GetIndexedHourlyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                        DailyPay = GetDailyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                        TotalCost = projectDirectLabour.Quantity * projectDirectLabour.UnitFactory * projectDirectLabour.DailyPay,
                        ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                        OrgBranchName = projectDirectLabour.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectDirectLabour.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectDirectLabour.ModifiedBy,
                        ResourceUtilizationWorkItemId = idfour,
                        ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                    };
                    entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                       entity.IndexedHourlySalary;
                    db.ResourceUtilizationDirectLabours.Add(entity);
                    db.SaveChanges();
                    projectDirectLabour.ResourceUtilizationDirectLabourId = entity.ResourceUtilizationDirectLabourId;
                    //ProjectListController.UpdateProjectTotalCost(idfour);

                    ResourceUtilizationController controller = new ResourceUtilizationController();
                    controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
                }
                
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationDirectLabour_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationDirectLabour
                {
                    ResourceUtilizationDirectLabourId = projectDirectLabour.ResourceUtilizationDirectLabourId,
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    IndexedHourlySalary = GetIndexedHourlyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    DailyPay = GetDailyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    TotalCost = projectDirectLabour.Quantity * projectDirectLabour.UnitFactory * projectDirectLabour.DailyPay,
                    ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = projectDirectLabour.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectDirectLabour.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ResourceUtilizationWorkItemId = projectDirectLabour.ResourceUtilizationWorkItemId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                entity.IndexedHourlySalary;
                db.ResourceUtilizationDirectLabours.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //ProjectListController.UpdateProjectTotalCost(projectDirectLabour.WorkItemPerProjectId);

                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationDirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationDirectLabour
                {
                    ResourceUtilizationDirectLabourId = projectDirectLabour.ResourceUtilizationDirectLabourId,
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    IndexedHourlySalary  = projectDirectLabour.IndexedHourlySalary,
                    DailyPay = projectDirectLabour.DailyPay,
                    TotalCost = projectDirectLabour.TotalCost,
                    ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = projectDirectLabour.DateCreated,
                    LastModified = projectDirectLabour.LastModified,
                    CreatedBy = projectDirectLabour.CreatedBy,
                    ModifiedBy = projectDirectLabour.ModifiedBy,
                    ResourceUtilizationWorkItemId = projectDirectLabour.ResourceUtilizationWorkItemId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };

                db.ResourceUtilizationDirectLabours.Attach(entity);
                db.ResourceUtilizationDirectLabours.Remove(entity);
                db.SaveChanges();

                //ProjectListController.UpdateProjectTotalCost(projectDirectLabour.WorkItemPerProjectId);

                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }
        #region Methods

        private  decimal GetIndexedHourlyPayPerJobTitle(int id)
        {
            var dbs = new ExceedDbContext();
            var jobTitles = dbs.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.ApprovedIndexedHourlySalary;
            }
            return 0;
        }
        private decimal GetDailyPayPerJobTitle(int id)
        {
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.NetBirrHour;
            }
            return 0;
        }
        private decimal GetManPowerUnitCost(int id)
        {
            decimal labourCost = 0;
            totalMaterialCost = 0;
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectDirectLabour.Where(x => x.WorkItemPerProjectId == id).ToList();
                if (labouCosts.Any())
                {


                    labourCost = labouCosts.Sum(x => x.TotalCost);

                }
                labourCost = labourCost / workItem.LabourHourlyOutPut;
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}