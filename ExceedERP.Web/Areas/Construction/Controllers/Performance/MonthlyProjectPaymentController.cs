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
    public class MonthlyProjectPaymentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyPayment_Read([DataSourceRequest]DataSourceRequest request, int periodIdReadPayment)
        {
            IQueryable<MonthlyProjectPayment> payments = db.MonthlyProjectPayment.Where(x => x.PeriodMappingId == periodIdReadPayment);
            DataSourceResult result = payments.ToDataSourceResult(request, payment => new
            {
                MonthlyProjectPaymentId = payment.MonthlyProjectPaymentId,
                PeriodMappingId = payment.PeriodMappingId,
                ApprovedTillPreMonth = payment.ApprovedTillPreMonth,
                ExpectedToBeApprovedThisMonth = payment.ExpectedToBeApprovedThisMonth,
                AmountApprovedThisMonth = payment.AmountApprovedThisMonth,
                AmounNotApprovedThisMonth = payment.AmounNotApprovedThisMonth,
                ApprovedToDate = payment.ApprovedToDate,
                AmountNotApprovedToDate = payment.AmountNotApprovedToDate,
                PriceAdjustmentTillPreMonth = payment.PriceAdjustmentTillPreMonth,
                PriceAdjustmentThisMonth = payment.PriceAdjustmentThisMonth,
                PriceAdjustmentApprovedToDate = payment.PriceAdjustmentApprovedToDate,
                MatOnSiteThisMonth = payment.MatOnSiteThisMonth,
                MatOnSiteApprovedThisMonth = payment.MatOnSiteApprovedThisMonth,
                MatOnSiteNotAppprovedThiMonth = payment.MatOnSiteNotAppprovedThiMonth,
                ApprovalTime = payment.ApprovalTime,
                Days = payment.Days,
                Points = payment.Points,

                OrgBranchName = payment.OrgBranchName,
                DateCreated = payment.DateCreated,
                LastModified = payment.LastModified,
                CreatedBy = payment.CreatedBy,
                ModifiedBy = payment.ModifiedBy

            });

            return Json(result);
        }
        private bool MonthlyProjectPaymentExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectPayment.Where(p => p.PeriodMappingId == periodId).FirstOrDefault();
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPayment_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectPayment payment, int periodIdCreatePayment)
        {
            if (ModelState.IsValid)
            {
                if (MonthlyProjectPaymentExists(periodIdCreatePayment))
                {
                    ModelState.AddModelError("", "Payment Evaluation for this month exists already!!");
                }
                else
                {
                    var entity = new MonthlyProjectPayment
                {
                    MonthlyProjectPaymentId = payment.MonthlyProjectPaymentId,
                    PeriodMappingId = periodIdCreatePayment,
                    ApprovedTillPreMonth = payment.ApprovedTillPreMonth,
                    ExpectedToBeApprovedThisMonth = PerformanceEvnHelper.GetExpectedToBeApprovedThisMonth(payment, periodIdCreatePayment),
                    AmountApprovedThisMonth = payment.AmountApprovedThisMonth,
                    AmounNotApprovedThisMonth = PerformanceEvnHelper.GetAmountNotApprovedThisMonth(payment, periodIdCreatePayment),
                    ApprovedToDate = PerformanceEvnHelper.GetApprovedAmountToDate(payment),
                    AmountNotApprovedToDate = PerformanceEvnHelper.GetAmountNotApprovedToDate(payment, periodIdCreatePayment),
                    PriceAdjustmentTillPreMonth = payment.PriceAdjustmentTillPreMonth,
                    PriceAdjustmentThisMonth = payment.PriceAdjustmentThisMonth,
                    PriceAdjustmentApprovedToDate = payment.PriceAdjustmentApprovedToDate,
                    MatOnSiteThisMonth = payment.MatOnSiteThisMonth,
                    MatOnSiteApprovedThisMonth = payment.MatOnSiteApprovedThisMonth,
                    MatOnSiteNotAppprovedThiMonth = PerformanceEvnHelper.GetMatOnSiteNotApprovedThisMonth(payment),
                    ApprovalTime = payment.ApprovalTime,
                    Days = payment.Days,
                    //Points = this.CalculateApprovalTimeScore(payment, periodIdCreatePayment),
                    Points = payment.Points,

                    OrgBranchName = payment.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = payment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = payment.ModifiedBy
                };
                //entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                //entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;

                db.MonthlyProjectPayment.Add(entity);
                db.SaveChanges();
                payment.MonthlyProjectPaymentId = entity.MonthlyProjectPaymentId;

                }
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPayment_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectPayment payment)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPayment
                {
                    MonthlyProjectPaymentId = payment.MonthlyProjectPaymentId,
                    PeriodMappingId = payment.PeriodMappingId,
                    ApprovedTillPreMonth = payment.ApprovedTillPreMonth,
                    ExpectedToBeApprovedThisMonth = PerformanceEvnHelper.GetExpectedToBeApprovedThisMonth(payment, payment.PeriodMappingId),
                    AmountApprovedThisMonth = payment.AmountApprovedThisMonth,
                    AmounNotApprovedThisMonth = PerformanceEvnHelper.GetAmountNotApprovedThisMonth(payment, payment.PeriodMappingId),
                    ApprovedToDate = PerformanceEvnHelper.GetApprovedAmountToDate(payment),
                    AmountNotApprovedToDate = PerformanceEvnHelper.GetAmountNotApprovedToDate(payment, payment.PeriodMappingId),
                    PriceAdjustmentTillPreMonth = payment.PriceAdjustmentTillPreMonth,
                    PriceAdjustmentThisMonth = payment.PriceAdjustmentThisMonth,
                    PriceAdjustmentApprovedToDate = payment.PriceAdjustmentApprovedToDate,
                    MatOnSiteThisMonth = payment.MatOnSiteThisMonth,
                    MatOnSiteApprovedThisMonth = payment.MatOnSiteApprovedThisMonth,
                    MatOnSiteNotAppprovedThiMonth = PerformanceEvnHelper.GetMatOnSiteNotApprovedThisMonth(payment),
                    ApprovalTime = payment.ApprovalTime,
                    Days = payment.Days,
                    //Points = this.CalculateApprovalTimeScore(payment, payment.PeriodMappingId),
                    Points = payment.Points,

                    OrgBranchName = payment.OrgBranchName,
                    DateCreated = payment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = payment.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                //entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                //entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;

                db.MonthlyProjectPayment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPayment_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectPayment payment)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPayment
                {
                    MonthlyProjectPaymentId = payment.MonthlyProjectPaymentId,
                    PeriodMappingId = payment.PeriodMappingId,
                    ApprovedTillPreMonth = payment.ApprovedTillPreMonth,
                    ExpectedToBeApprovedThisMonth = payment.ExpectedToBeApprovedThisMonth,
                    AmountApprovedThisMonth = payment.AmountApprovedThisMonth,
                    AmounNotApprovedThisMonth = payment.AmounNotApprovedThisMonth,
                    ApprovedToDate = payment.ApprovedToDate,
                    AmountNotApprovedToDate = payment.AmountNotApprovedToDate,
                    PriceAdjustmentTillPreMonth = payment.PriceAdjustmentTillPreMonth,
                    PriceAdjustmentThisMonth = payment.PriceAdjustmentThisMonth,
                    PriceAdjustmentApprovedToDate = payment.PriceAdjustmentApprovedToDate,
                    MatOnSiteThisMonth = payment.MatOnSiteThisMonth,
                    MatOnSiteApprovedThisMonth = payment.MatOnSiteApprovedThisMonth,
                    MatOnSiteNotAppprovedThiMonth = payment.MatOnSiteNotAppprovedThiMonth,
                    ApprovalTime = payment.ApprovalTime,
                    Days = payment.Days,
                    Points = payment.Points,

                    OrgBranchName = payment.OrgBranchName,
                    DateCreated = payment.DateCreated,
                    LastModified = payment.LastModified,
                    CreatedBy = payment.CreatedBy,
                    ModifiedBy = payment.ModifiedBy
                };

                db.MonthlyProjectPayment.Attach(entity);
                db.MonthlyProjectPayment.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateApprovalTimeScore(MonthlyProjectPayment model, int evaluationPeriodId)
        {
            var currentEvaluationPeriod = db.PeriodMapping.Find(evaluationPeriodId);
            if(currentEvaluationPeriod != null)
            {

            }
            var pt = new decimal();
            var approvalTime = model.ApprovalTime;
            
            if (approvalTime == MonthlyDueDate.On_Due_Date)
            {
                pt = 0;
            }
            else if (approvalTime == MonthlyDueDate.Before_Due_Date)
            {
                pt = (decimal)(model.Days);
            }
            else if (approvalTime == MonthlyDueDate.After_Due_Date)
            {
                pt = (decimal)(0 - model.Days);
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