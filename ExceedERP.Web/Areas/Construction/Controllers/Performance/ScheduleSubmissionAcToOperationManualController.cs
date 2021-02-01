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
    public class ScheduleSubmissionAcToOperationManualController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubmissionAcToOperationManual_Read([DataSourceRequest]DataSourceRequest request, int planIdRead)
        {
            IQueryable<RScheduleSubmissionAcToOperationManual> submissions = db.ScheduleSubmissionAcToOperationManual.Where(x => x.MonthlyProjectPlanningId == planIdRead);
            DataSourceResult result = submissions.ToDataSourceResult(request, submission => new
            {
                RScheduleSubmissionAcToOperationManualId = submission.RScheduleSubmissionAcToOperationManualId,
                MonthlyProjectPlanningId = submission.MonthlyProjectPlanningId,
                IsRevisionTime = submission.IsRevisionTime,
                SubmissionTime = submission.SubmissionTime,
                Days = submission.Days,
                Points = submission.Points,


                OrgBranchName = submission.OrgBranchName,
                DateCreated = submission.DateCreated,
                LastModified = submission.LastModified,
                CreatedBy = submission.CreatedBy,
                ModifiedBy = submission.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmissionAcToOperationManual_Create([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToOperationManual submission, int planIdCreate)
        {
            if (ModelState.IsValid)
            {
               
                var entity = new RScheduleSubmissionAcToOperationManual
                {
                    RScheduleSubmissionAcToOperationManualId = submission.RScheduleSubmissionAcToOperationManualId,
                    MonthlyProjectPlanningId = planIdCreate,
                    IsRevisionTime = submission.IsRevisionTime,
                    SubmissionTime = submission.SubmissionTime,
                    Days = submission.Days,
                    Points = this.CalculateTotalScore(submission),

                    OrgBranchName = submission.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = submission.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = submission.ModifiedBy
                };
                
                db.ScheduleSubmissionAcToOperationManual.Add(entity);
                db.SaveChanges();
                submission.RScheduleSubmissionAcToOperationManualId = entity.RScheduleSubmissionAcToOperationManualId;

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);
            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmissionAcToOperationManual_Update([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToOperationManual submission)
        {
            if (ModelState.IsValid)
            {
                var entity = new RScheduleSubmissionAcToOperationManual
                {
                    RScheduleSubmissionAcToOperationManualId = submission.RScheduleSubmissionAcToOperationManualId,
                    MonthlyProjectPlanningId = submission.MonthlyProjectPlanningId,
                    IsRevisionTime = submission.IsRevisionTime,
                    SubmissionTime = submission.SubmissionTime,
                    Days = submission.Days,
                    Points = this.CalculateTotalScore(submission),

                    OrgBranchName = submission.OrgBranchName,
                    DateCreated = submission.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = submission.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                
                db.ScheduleSubmissionAcToOperationManual.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmissionAcToOperationManual_Destroy([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToOperationManual submission)
        {
            if (ModelState.IsValid)
            {
                var entity = new RScheduleSubmissionAcToOperationManual
                {
                    RScheduleSubmissionAcToOperationManualId = submission.RScheduleSubmissionAcToOperationManualId,
                    MonthlyProjectPlanningId = submission.MonthlyProjectPlanningId,
                    IsRevisionTime = submission.IsRevisionTime,
                    SubmissionTime = submission.SubmissionTime,
                    Days = submission.Days,
                    Points = submission.Points,

                    OrgBranchName = submission.OrgBranchName,
                    DateCreated = submission.DateCreated,
                    LastModified = submission.LastModified,
                    CreatedBy = submission.CreatedBy,
                    ModifiedBy = submission.ModifiedBy
                };

                db.ScheduleSubmissionAcToOperationManual.Attach(entity);
                db.ScheduleSubmissionAcToOperationManual.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateTotalScore(RScheduleSubmissionAcToOperationManual model)
        {
            var pt = new decimal();
            var submissionTime = model.SubmissionTime;
            if (model.IsRevisionTime)
            {
                if (submissionTime == MonthlyDueDate.On_Due_Date)
                {
                    pt = 3;
                }
                else if (submissionTime == MonthlyDueDate.Before_Due_Date)
                {
                    pt = (decimal)(3 + model.Days);
                }
                else if (submissionTime == MonthlyDueDate.After_Due_Date)
                {
                    pt = (decimal)(3 - model.Days);
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