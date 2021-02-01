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

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class ProjectPlanningQualityAndAcceptanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QualityAndAcceptance_Read([DataSourceRequest]DataSourceRequest request, int planIdReadAcceptance)
        {
            IQueryable<ProjectPlanningQualityAndAcceptance> acceptances = db.ProjectPlanningQualityAndAcceptance.Where(x => x.MonthlyProjectPlanningId == planIdReadAcceptance);
            DataSourceResult result = acceptances.ToDataSourceResult(request, acceptance => new
            {
                ProjectPlanningQualityAndAcceptanceId = acceptance.ProjectPlanningQualityAndAcceptanceId,
                MonthlyProjectPlanningId = acceptance.MonthlyProjectPlanningId,
                IsApprovalMadeInTheMonth = acceptance.IsApprovalMadeInTheMonth,
                ApprovalType = acceptance.ApprovalType,
                points = acceptance.points,

                OrgBranchName = acceptance.OrgBranchName,
                DateCreated = acceptance.DateCreated,
                LastModified = acceptance.LastModified,
                CreatedBy = acceptance.CreatedBy,
                ModifiedBy = acceptance.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityAndAcceptance_Create([DataSourceRequest]DataSourceRequest request, ProjectPlanningQualityAndAcceptance acceptance, int planIdCreateAcceptance)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPlanningQualityAndAcceptance
                {
                    MonthlyProjectPlanningId = planIdCreateAcceptance,
                    IsApprovalMadeInTheMonth = acceptance.IsApprovalMadeInTheMonth,
                    ApprovalType = acceptance.ApprovalType,
                    points = this.CalculateTotalScore(acceptance),

                    OrgBranchName = acceptance.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = acceptance.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = acceptance.ModifiedBy
                };

                db.ProjectPlanningQualityAndAcceptance.Add(entity);
                db.SaveChanges();
                acceptance.ProjectPlanningQualityAndAcceptanceId = entity.ProjectPlanningQualityAndAcceptanceId;

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }
            return Json(new[] { acceptance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityAndAcceptance_Update([DataSourceRequest]DataSourceRequest request, ProjectPlanningQualityAndAcceptance acceptance)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPlanningQualityAndAcceptance
                {
                    ProjectPlanningQualityAndAcceptanceId = acceptance.ProjectPlanningQualityAndAcceptanceId,
                    MonthlyProjectPlanningId = acceptance.MonthlyProjectPlanningId,
                    IsApprovalMadeInTheMonth = acceptance.IsApprovalMadeInTheMonth,
                    ApprovalType = acceptance.ApprovalType,
                    points = this.CalculateTotalScore(acceptance),


                    OrgBranchName = acceptance.OrgBranchName,
                    DateCreated = acceptance.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = acceptance.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectPlanningQualityAndAcceptance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { acceptance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityAndAcceptance_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPlanningQualityAndAcceptance acceptance)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPlanningQualityAndAcceptance
                {
                    ProjectPlanningQualityAndAcceptanceId = acceptance.ProjectPlanningQualityAndAcceptanceId,
                    MonthlyProjectPlanningId = acceptance.MonthlyProjectPlanningId,
                    IsApprovalMadeInTheMonth = acceptance.IsApprovalMadeInTheMonth,
                    ApprovalType = acceptance.ApprovalType,
                    points = acceptance.points,

                    OrgBranchName = acceptance.OrgBranchName,
                    DateCreated = acceptance.DateCreated,
                    LastModified = acceptance.LastModified,
                    CreatedBy = acceptance.CreatedBy,
                    ModifiedBy = acceptance.ModifiedBy
                };

                db.ProjectPlanningQualityAndAcceptance.Attach(entity);
                db.ProjectPlanningQualityAndAcceptance.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { acceptance }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateTotalScore(ProjectPlanningQualityAndAcceptance model)
        {
            var pt = new decimal();
            var approvalType = model.ApprovalType;
            if (model.IsApprovalMadeInTheMonth)
            {
                if (approvalType == RevisedScheduleApprovalStatus.Approved_Without_Correction)
                {
                    pt = 2;
                }
                else if (approvalType == RevisedScheduleApprovalStatus.Approved_After_One_Correction)
                {
                    pt = 1;
                }
                else if (approvalType == RevisedScheduleApprovalStatus.Approved_After_Two_Correction)
                {
                    pt = 0;
                }
                else 
                {
                    pt = 0;
                }
            }
            return pt;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}