﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectDirectLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectDirectLabour_Read([DataSourceRequest]DataSourceRequest request, int idthree)
        {
            IQueryable<ProjectDirectLabour> projectdirectlabour = db.ProjectDirectLabour.Where( x=> x.WorkItemPerProjectId ==idthree) ;
            DataSourceResult result = projectdirectlabour.ToDataSourceResult(request, projectDirectLabour => new {
                ProjectDirectLabourId = projectDirectLabour.ProjectDirectLabourId,
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
                ModifiedBy = projectDirectLabour.ModifiedBy ,
                WorkItemPerProjectId= projectDirectLabour.WorkItemPerProjectId,
                ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Create([DataSourceRequest]DataSourceRequest request, ProjectDirectLabour projectDirectLabour,int idfour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectDirectLabour
                {
                    Unit = projectDirectLabour.Unit,
                    Quantity = projectDirectLabour.Quantity,
                    UnitFactory = projectDirectLabour.UnitFactory,
                    //IndexedHourlySalary = this.GetIndexedHourlyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    IndexedHourlySalary = projectDirectLabour.IndexedHourlySalary,
                    DailyPay = GetDailyPayPerJobTitle(projectDirectLabour.ProjectEstimationIndexLabourId),
                    TotalCost = projectDirectLabour.Quantity * projectDirectLabour.UnitFactory * projectDirectLabour.IndexedHourlySalary,
                    ManpowerUnitCost = projectDirectLabour.ManpowerUnitCost,
                    OrgBranchName = projectDirectLabour.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectDirectLabour.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = projectDirectLabour.ModifiedBy ,
                    WorkItemPerProjectId =idfour ,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                 entity.IndexedHourlySalary;
                db.ProjectDirectLabour.Add(entity);
                db.SaveChanges();
                projectDirectLabour.ProjectDirectLabourId = entity.ProjectDirectLabourId;
                ProjectListController.UpdateProjectTotalCost(idfour);
               
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Update([DataSourceRequest]DataSourceRequest request, ProjectDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectDirectLabour
                {
                    ProjectDirectLabourId = projectDirectLabour.ProjectDirectLabourId,
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
                    LastModified = DateTime .Now ,
                    CreatedBy = projectDirectLabour.CreatedBy,
                    ModifiedBy = User.Identity .Name  ,
                    WorkItemPerProjectId = projectDirectLabour.WorkItemPerProjectId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };
                entity.TotalCost = entity.Quantity * entity.UnitFactory *
                                entity.IndexedHourlySalary;
                db.ProjectDirectLabour.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(projectDirectLabour.WorkItemPerProjectId);
               
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, ProjectDirectLabour projectDirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectDirectLabour
                {
                    ProjectDirectLabourId = projectDirectLabour.ProjectDirectLabourId,
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
                    ModifiedBy = projectDirectLabour.ModifiedBy ,
                    WorkItemPerProjectId = projectDirectLabour.WorkItemPerProjectId,
                    ProjectEstimationIndexLabourId = projectDirectLabour.ProjectEstimationIndexLabourId
                };

                db.ProjectDirectLabour.Attach(entity);
                db.ProjectDirectLabour.Remove(entity);
                db.SaveChanges();

                ProjectListController.UpdateProjectTotalCost(projectDirectLabour.WorkItemPerProjectId);
                
            }

            return Json(new[] { projectDirectLabour }.ToDataSourceResult(request, ModelState));
        }
        #region Methods
        public decimal GetIndexedHourlyPayPerJobTitle(int id)
        {
            var jobTitle = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitle != null)
            {
                return jobTitle.ApprovedIndexedHourlySalary;
            }
            return 0;
        }
        public decimal GetDailyPayPerJobTitle(int id)
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
                    db.ProjectDirectLabour .Where(x => x.WorkItemPerProjectId == id).ToList();
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
