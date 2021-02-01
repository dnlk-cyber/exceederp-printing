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
    public class ApprovalStatusForClaimsRespondedController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovalStatusForClaims_Read([DataSourceRequest]DataSourceRequest request, int cIdReadApproval)
        {
            IQueryable<ApprovalStatusForClaimsResponded> approvals = db.ApprovalStatusForClaimsResponded.Where(x => x.MonthlyProjectClaimId == cIdReadApproval);
            DataSourceResult result = approvals.ToDataSourceResult(request, approval => new
            {
                ApprovalStatusForClaimsRespondedId = approval.ApprovalStatusForClaimsRespondedId,
                MonthlyProjectClaimId = approval.MonthlyProjectClaimId,
                IsApprovalMadeInTheMonth = approval.IsApprovalMadeInTheMonth,
                ApprovalStatus = approval.ApprovalStatus,
                Points = approval.Points,

                OrgBranchName = approval.OrgBranchName,
                DateCreated = approval.DateCreated,
                LastModified = approval.LastModified,
                CreatedBy = approval.CreatedBy,
                ModifiedBy = approval.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalStatusForClaims_Create([DataSourceRequest]DataSourceRequest request, ApprovalStatusForClaimsResponded approval, int cIdCreateApproval)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalStatusForClaimsResponded
                {
                    //ApprovalStatusForClaimsRespondedId = approval.ApprovalStatusForClaimsRespondedId,
                    MonthlyProjectClaimId = cIdCreateApproval,
                    IsApprovalMadeInTheMonth = approval.IsApprovalMadeInTheMonth,
                    ApprovalStatus = approval.ApprovalStatus,
                    Points = this.CalculateScore(approval),

                    OrgBranchName = approval.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = approval.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = approval.ModifiedBy
                };

                db.ApprovalStatusForClaimsResponded.Add(entity);
                db.SaveChanges();
                approval.MonthlyProjectClaimId = entity.MonthlyProjectClaimId;

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }
            return Json(new[] { approval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalStatusForClaims_Update([DataSourceRequest]DataSourceRequest request, ApprovalStatusForClaimsResponded approval)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalStatusForClaimsResponded
                {
                    ApprovalStatusForClaimsRespondedId = approval.ApprovalStatusForClaimsRespondedId,
                    MonthlyProjectClaimId = approval.MonthlyProjectClaimId,
                    IsApprovalMadeInTheMonth = approval.IsApprovalMadeInTheMonth,
                    ApprovalStatus = approval.ApprovalStatus,
                    Points = this.CalculateScore(approval),


                    OrgBranchName = approval.OrgBranchName,
                    DateCreated = approval.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = approval.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ApprovalStatusForClaimsResponded.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { approval }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalStatusForClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApprovalStatusForClaimsResponded approval)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalStatusForClaimsResponded
                {
                    ApprovalStatusForClaimsRespondedId = approval.ApprovalStatusForClaimsRespondedId,
                    MonthlyProjectClaimId = approval.MonthlyProjectClaimId,
                    IsApprovalMadeInTheMonth = approval.IsApprovalMadeInTheMonth,
                    ApprovalStatus = approval.ApprovalStatus,
                    Points = approval.Points,

                    OrgBranchName = approval.OrgBranchName,
                    DateCreated = approval.DateCreated,
                    LastModified = approval.LastModified,
                    CreatedBy = approval.CreatedBy,
                    ModifiedBy = approval.ModifiedBy
                };

                db.ApprovalStatusForClaimsResponded.Attach(entity);
                db.ApprovalStatusForClaimsResponded.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { approval }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateScore(ApprovalStatusForClaimsResponded model)
        {
            decimal point = new decimal();
            if (model.IsApprovalMadeInTheMonth)
            {
                var status = model.ApprovalStatus;
                if (status == MonthlyClaimRequestApprovalStatus.Above_80Percent_Approved)
                {
                    point = 3;
                }
                else if (status == MonthlyClaimRequestApprovalStatus.Above_60Percent_Approved)
                {
                    point = 2;
                }
                else if (status == MonthlyClaimRequestApprovalStatus.Above_50Percent_Approved)
                {
                    point = 1;
                }
            }
            return point;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}