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
    public class RequirementForMonthlyClaimsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RequirementForMonthlyClaims_Read([DataSourceRequest]DataSourceRequest request, int cIdReadRequirement)
        {
            IQueryable<RequirementForMonthlyClaims> requirements = db.RequirementForMonthlyClaims.Where(x => x.MonthlyProjectClaimId == cIdReadRequirement);
            DataSourceResult result = requirements.ToDataSourceResult(request, requirement => new
            {
                RequirementForMonthlyClaimsId = requirement.RequirementForMonthlyClaimsId,
                MonthlyProjectClaimId = requirement.MonthlyProjectClaimId,
                Description = requirement.Description,
                MaxAllowablePoints = requirement.MaxAllowablePoints,
                Points = requirement.Points,

                OrgBranchName = requirement.OrgBranchName,
                DateCreated = requirement.DateCreated,
                LastModified = requirement.LastModified,
                CreatedBy = requirement.CreatedBy,
                ModifiedBy = requirement.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequirementForMonthlyClaims_Create([DataSourceRequest]DataSourceRequest request, RequirementForMonthlyClaims requirement, int cIdCreateRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new RequirementForMonthlyClaims
                {
                    //RequirementForMonthlyClaimsId = requirement.RequirementForMonthlyClaimsId,
                    MonthlyProjectClaimId = cIdCreateRequirement,
                    Description = requirement.Description,
                    MaxAllowablePoints = requirement.MaxAllowablePoints,
                    Points = requirement.MaxAllowablePoints != null &&
                             requirement.Points > requirement.MaxAllowablePoints ?
                             (decimal)requirement.MaxAllowablePoints : requirement.Points,

                    OrgBranchName = requirement.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = requirement.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = requirement.ModifiedBy
                };

                db.RequirementForMonthlyClaims.Add(entity);
                db.SaveChanges();
                requirement.RequirementForMonthlyClaimsId = entity.RequirementForMonthlyClaimsId;

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }
            return Json(new[] { requirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequirementForMonthlyClaims_Update([DataSourceRequest]DataSourceRequest request, RequirementForMonthlyClaims requirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new RequirementForMonthlyClaims
                {
                    RequirementForMonthlyClaimsId = requirement.RequirementForMonthlyClaimsId,
                    MonthlyProjectClaimId = requirement.MonthlyProjectClaimId,
                    Description = requirement.Description,
                    MaxAllowablePoints = requirement.MaxAllowablePoints,
                    Points = requirement.MaxAllowablePoints != null &&
                             requirement.Points > requirement.MaxAllowablePoints ?
                             (decimal)requirement.MaxAllowablePoints : requirement.Points,


                    OrgBranchName = requirement.OrgBranchName,
                    DateCreated = requirement.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = requirement.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.RequirementForMonthlyClaims.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { requirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequirementForMonthlyClaims_Destroy([DataSourceRequest]DataSourceRequest request, RequirementForMonthlyClaims requirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new RequirementForMonthlyClaims
                {
                    RequirementForMonthlyClaimsId = requirement.RequirementForMonthlyClaimsId,
                    MonthlyProjectClaimId = requirement.MonthlyProjectClaimId,
                    Description = requirement.Description,
                    MaxAllowablePoints = requirement.MaxAllowablePoints,
                    Points = requirement.Points,

                    OrgBranchName = requirement.OrgBranchName,
                    DateCreated = requirement.DateCreated,
                    LastModified = requirement.LastModified,
                    CreatedBy = requirement.CreatedBy,
                    ModifiedBy = requirement.ModifiedBy
                };

                db.RequirementForMonthlyClaims.Attach(entity);
                db.RequirementForMonthlyClaims.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { requirement }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}