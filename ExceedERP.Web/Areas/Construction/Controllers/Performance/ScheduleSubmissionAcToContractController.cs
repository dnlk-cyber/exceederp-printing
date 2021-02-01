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
    public class ScheduleSubmissionAcToContractController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ScheduleSubmissionAcToContract_Read([DataSourceRequest]DataSourceRequest request, int planIdReadContract)
        {
            IQueryable<RScheduleSubmissionAcToContract> submissions = db.RScheduleSubmissionAcToContract.Where(x => x.MonthlyProjectPlanningId == planIdReadContract);
            DataSourceResult result = submissions.ToDataSourceResult(request, submission => new
            {
                RScheduleSubmissionAcToContractId = submission.RScheduleSubmissionAcToContractId,
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
        public ActionResult ScheduleSubmissionAcToContract_Create([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToContract submission, int planIdCreateContract)
        {
            if (ModelState.IsValid)
            {

                var entity = new RScheduleSubmissionAcToContract
                {
                    MonthlyProjectPlanningId = planIdCreateContract,
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

                db.RScheduleSubmissionAcToContract.Add(entity);
                db.SaveChanges();
                submission.RScheduleSubmissionAcToContractId = entity.RScheduleSubmissionAcToContractId;

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleSubmissionAcToContract_Update([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToContract submission)
        {
            if (ModelState.IsValid)
            {
                var entity = new RScheduleSubmissionAcToContract
                {
                    RScheduleSubmissionAcToContractId = submission.RScheduleSubmissionAcToContractId,
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

                db.RScheduleSubmissionAcToContract.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleSubmissionAcToContract_Destroy([DataSourceRequest]DataSourceRequest request, RScheduleSubmissionAcToContract submission)
        {
            if (ModelState.IsValid)
            {
                var entity = new RScheduleSubmissionAcToContract
                {
                    RScheduleSubmissionAcToContractId = submission.RScheduleSubmissionAcToContractId,
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

                db.RScheduleSubmissionAcToContract.Attach(entity);
                db.RScheduleSubmissionAcToContract.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { submission }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateTotalScore(RScheduleSubmissionAcToContract model)
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